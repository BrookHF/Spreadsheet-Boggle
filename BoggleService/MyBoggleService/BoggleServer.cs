using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;
using System.Dynamic;

namespace Boggle
{
    public class BoggleServer
    {
        /// <summary>
        /// Launches a BoggleServer on port 60000.  Keeps the main
        /// thread active so we can send output to the console.
        /// </summary>
        static void Main(string[] args)
        {
            new BoggleServer(60000);
            Console.ReadLine();
        }

        // Listens for incoming connection requests
        private TcpListener server;

        // All the clients that have connected but haven't closed
        private List<ClientConnection> clients = new List<ClientConnection>();

        // Read/write lock to coordinate access to the clients list
        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();



        /// <summary>
        /// Creates a BoggleServer that listens for connection requests on port 60000.
        /// </summary>
        public BoggleServer(int port)
        {
            // A TcpListener listens for incoming connection requests
            server = new TcpListener(IPAddress.Any, port);

            // Start the TcpListener
            server.Start();

            // Ask the server to call ConnectionRequested at some point in the future when 
            // a connection request arrives.  It could be a very long time until this happens.
            // The waiting and the calling will happen on another thread.  BeginAcceptSocket 
            // returns immediately, and the constructor returns to Main.
            server.BeginAcceptSocket(ConnectionRequested, null);
        }

        /// <summary>
        /// This is the callback method that is passed to BeginAcceptSocket.  It is called
        /// when a connection request has arrived at the server.
        /// </summary>
        private void ConnectionRequested(IAsyncResult result)
        {
            // We obtain the socket corresonding to the connection request.  Notice that we
            // are passing back the IAsyncResult object.
            Socket s = server.EndAcceptSocket(result);

            // We ask the server to listen for another connection request.  As before, this
            // will happen on another thread.
            server.BeginAcceptSocket(ConnectionRequested, null);

            // We create a new ClientConnection, which will take care of communicating with
            // the remote client.  We add the new client to the list of clients, taking 
            // care to use a write lock.
            try
            {
                sync.EnterWriteLock();
                clients.Add(new ClientConnection(s, this));
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }

        /// <summary>
        /// Remove c from the client list.
        /// </summary>
        public void RemoveClient(ClientConnection c)
        {
            try
            {
                sync.EnterWriteLock();
                clients.Remove(c);
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }
    }

    /// <summary>
    /// Represents a connection with a remote client.  Takes care of receiving and sending
    /// information to that client according to the protocol.
    /// </summary>
    public class ClientConnection
    {
        // Incoming/outgoing is UTF8-encoded.  This is a multi-byte encoding.  The first 128 Unicode characters
        // (which corresponds to the old ASCII character set and contains the common keyboard characters) are
        // encoded into a single byte.  The rest of the Unicode characters can take from 2 to 4 bytes to encode.
        private static System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

        // Buffer size for reading incoming bytes
        private const int BUFFER_SIZE = 1024;

        // The socket through which we communicate with the remote client
        private Socket socket;

        // Text that has been received from the client but not yet dealt with
        private StringBuilder incoming;

        // Text that needs to be sent to the client but which we have not yet started sending
        private StringBuilder outgoing;

        // For decoding incoming UTF8-encoded byte streams.
        private Decoder decoder = encoding.GetDecoder();

        // Buffers that will contain incoming bytes and characters
        private byte[] incomingBytes = new byte[BUFFER_SIZE];
        private char[] incomingChars = new char[BUFFER_SIZE];

        // Records whether an asynchronous send attempt is ongoing
        private bool sendIsOngoing = false;

        // For synchronizing sends
        private readonly object sendSync = new object();
        private readonly object readSync = new object();

        // Bytes that we are actively trying to send, along with the
        // index of the leftmost byte whose send has not yet been completed
        private byte[] pendingBytes = new byte[0];
        private int pendingIndex = 0;

        // Name of chatter or null if unknown
        private BoggleServer server;
        private static bool finishedHeader;
        private static int ContentLength;
        private static HttpStatusCode status;
        private static BoggleService boggleService;

        private static String url;
        private static String type;

        /// <summary>
        /// Creates a ClientConnection from the socket, then begins communicating with it.
        /// </summary>
        public ClientConnection(Socket s, BoggleServer server)
        {
            // Record the socket and server and initialize incoming/outgoing
            this.server = server;
            socket = s;
            incoming = new StringBuilder();
            outgoing = new StringBuilder();
            ContentLength = 0;
            finishedHeader = false;
            boggleService = new BoggleService();

            try
            {
                // Ask the socket to call MessageReceive as soon as up to 1024 bytes arrive.
                socket.BeginReceive(incomingBytes, 0, incomingBytes.Length,
                                    SocketFlags.None, MessageReceived, null);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        /// <summary>
        /// Called when some data has been received.
        /// </summary>
        private void MessageReceived(IAsyncResult result)
        {
            // Figure out how many bytes have come in
            int bytesRead = socket.EndReceive(result);

            // Convert the bytes into characters and appending to incoming
            int charsRead = decoder.GetChars(incomingBytes, 0, bytesRead, incomingChars, 0, false);
            incoming.Append(incomingChars, 0, charsRead);
            Console.WriteLine(incoming);

            String line;
            string body = "";


            if (!finishedHeader)
            {
                url = "";
                type = "";
                for (int i = 0; i < incoming.Length; i++)
                {
                    if (incoming[i] == '\n')
                    {
                        line = incoming.ToString(0, i + 1);

                        String[] words = line.Split(' ');
                        type = words[0].ToLower();
                        url = words[1];

                        incoming.Remove(0, i + 1);
                        break;
                    }
                }

                for (int i = 0; i < incoming.Length; i++)
                {
                    if (incoming[i] == '\n')
                    {
                        line = incoming.ToString(0, i + 1);

                        if (incoming[0] == '\r' && incoming[1] == '\n')
                        {
                            incoming.Remove(0, i + 1);
                            finishedHeader = true;
                            break;
                        }
                        else
                        {
                            String[] words = line.Split(' ');
                            if (words[0].ToLower() == "content-length:")
                            {
                                int.TryParse(words[1], out ContentLength);
                            }
                        }
                        incoming.Remove(0, i + 1);
                        i = 0;
                    }
                }
            }
            if(finishedHeader)
            {

                dynamic output = new ExpandoObject();
                String[] urlArray = url.Split('/');

                if (type == "get" && urlArray[urlArray.Length - 2] == "games") //Get game status
                {
                    String[] Parameters = urlArray[urlArray.Length - 1].Split('?');
                    if (Parameters.Length > 1 && Parameters[1] == "Brief=yes") //brief = yes
                    {
                        output = boggleService.GetGameStatus("yes", Parameters[0], out status);
                    }
                    else //brief
                    {
                        output = boggleService.GetGameStatus("no", Parameters[0], out status);
                    }
                    SendMessage(formRespone(output, status));
                    incoming.Clear();
                    finishedHeader = false;
                }
                else if (incoming.Length >= ContentLength)
                {
                    body = incoming.ToString();
                    if (type != null)
                    {
                        if (type == "post" && urlArray[urlArray.Length - 1] == "users") //Create User
                        {
                            User input = new User();
                            dynamic tempObject = JsonConvert.DeserializeObject(body);
                            input.Nickname = tempObject.Nickname;
                            output = boggleService.CreateUser(input, out status);
                        }
                        if (type == "post" && urlArray[urlArray.Length - 1] == "games") //Join game
                        {
                            Game input = new Game();
                            dynamic tempObject = JsonConvert.DeserializeObject(body);
                            input.UserToken = tempObject.UserToken;
                            input.TimeLimit = tempObject.TimeLimit;
                            output = boggleService.JoinGame(input, out status);
                        }

                        if (type == "put" && urlArray[urlArray.Length - 1] == "games") //Cancel Join Request
                        {
                            Token input = new Token();
                            dynamic tempObject = JsonConvert.DeserializeObject(body);
                            input.UserToken = tempObject.UserToken;
                            boggleService.CancelJoin(input, out status);
                        }
                        if (type == "put" && urlArray[urlArray.Length - 2] == "games") //Play Word
                        {
                            PlayWord input = new PlayWord();
                            dynamic tempObject = JsonConvert.DeserializeObject(body);
                            input.UserToken = tempObject.UserToken;
                            input.Word = tempObject.Word;
                            output = boggleService.PlayWord(input, urlArray[urlArray.Length - 1], out status);
                        }
                        SendMessage(formRespone(output, status));
                    }
                    incoming.Clear();
                    finishedHeader = false;
                }

                try
                {
                    // Ask for some more data
                    socket.BeginReceive(incomingBytes, 0, incomingBytes.Length,
                        SocketFlags.None, MessageReceived, null);
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        private string formRespone(dynamic output, HttpStatusCode status)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("HTTP/1.1 " + (Int32)status + " " + status + "\r\n");
            sb.Append("Content-Length: " + JsonConvert.SerializeObject(output).Length + "\r\n");
            sb.Append("Content-Type: application/json; charset=utf-8" + "\r\n");
            sb.Append("\r\n");
            sb.Append(JsonConvert.SerializeObject(output));
            Console.WriteLine(sb.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// Sends a string to the client
        /// </summary>
        public void SendMessage(string lines)
        {
            // Get exclusive access to send mechanism
            lock (sendSync)
            {
                // Append the message to the outgoing lines
                outgoing.Append(lines);

                // If there's not a send ongoing, start one.
                if (!sendIsOngoing)
                {
                    sendIsOngoing = true;
                    SendBytes();
                }
            }
        }

        /// <summary>
        /// Attempts to send the entire outgoing string.
        /// This method should not be called unless sendSync has been acquired.
        /// </summary>
        private void SendBytes()
        {
            // If we're in the middle of the process of sending out a block of bytes,
            // keep doing that.
            if (pendingIndex < pendingBytes.Length)
            {
                try
                {
                    socket.BeginSend(pendingBytes, pendingIndex, pendingBytes.Length - pendingIndex,
                                     SocketFlags.None, MessageSent, null);
                }
                catch (ObjectDisposedException)
                {
                }
            }

            // If we're not currently dealing with a block of bytes, make a new block of bytes
            // out of outgoing and start sending that.
            else if (outgoing.Length > 0)
            {
                pendingBytes = encoding.GetBytes(outgoing.ToString());
                pendingIndex = 0;
                outgoing.Clear();
                try
                {
                    socket.BeginSend(pendingBytes, 0, pendingBytes.Length,
                                     SocketFlags.None, MessageSent, null);
                }
                catch (ObjectDisposedException)
                {
                }
            }

            // If there's nothing to send, shut down for the time being.
            else
            {
                sendIsOngoing = false;
            }
        }

        /// <summary>
        /// Called when a message has been successfully sent
        /// </summary>
        private void MessageSent(IAsyncResult result)
        {
            // Find out how many bytes were actually sent
            int bytesSent = socket.EndSend(result);

            // Get exclusive access to send mechanism
            lock (sendSync)
            {
                // The socket has been closed
                if (bytesSent == 0)
                {
                    socket.Close();
                    server.RemoveClient(this);
                    Console.WriteLine("Socket closed");
                }
                // Update the pendingIndex and keep trying
                else
                {
                    pendingIndex += bytesSent;
                    SendBytes();
                }
            }
        }
    }
}
