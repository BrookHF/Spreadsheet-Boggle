// Written by Joe Zachary for CS 3500, November 2012
// Revised by Joe Zachary April 2016
// Revised extensively by Joe Zachary April 2017

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace CustomNetworking
{
    /// <summary>
    /// The type of delegate that is called when a StringSocket send has completed.
    /// </summary>
    public delegate void SendCallback(bool wasSent, object payload);

    /// <summary>
    /// The type of delegate that is called when a receive has completed.
    /// </summary>
    public delegate void ReceiveCallback(String s, object payload);

    /// <summary> 
    /// A StringSocket is a wrapper around a Socket. It provides methods that
    /// asynchronously read lines of text (strings terminated by newlines) and 
    /// write strings. (As opposed to Sockets, which read and write raw bytes.)  
    ///
    /// StringSockets are thread safe.  This means that two or more threads may
    /// invoke methods on a shared StringSocket without restriction.  The
    /// StringSocket takes care of the synchronization.
    /// 
    /// Each StringSocket contains a Socket object that is provided by the client.  
    /// A StringSocket will work properly only if the client refrains from calling
    /// the contained Socket's read and write methods.
    /// 
    /// We can write a string to a StringSocket ss by doing
    /// 
    ///    ss.BeginSend("Hello world", callback, payload);
    ///    
    /// where callback is a SendCallback (see below) and payload is an arbitrary object.
    /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
    /// successfully written the string to the underlying Socket, or failed in the 
    /// attempt, it invokes the callback.  The parameter to the callback is the payload.  
    /// 
    /// We can read a string from a StringSocket ss by doing
    /// 
    ///     ss.BeginReceive(callback, payload)
    ///     
    /// where callback is a ReceiveCallback (see below) and payload is an arbitrary object.
    /// This is non-blocking, asynchronous operation.  When the StringSocket has read a
    /// string of text terminated by a newline character from the underlying Socket, or
    /// failed in the attempt, it invokes the callback.  The parameters to the callback are
    /// a string and the payload.  The string is the requested string (with the newline removed).
    /// </summary>

    public class StringSocket : IDisposable
    {
        // Underlying socket
        private Socket socket;

        // Encoding used for sending and receiving
        private Encoding encoding;

        // For decoding incoming UTF8-encoded byte streams.
        private Decoder decoder;

        // Buffer size for reading incoming bytes
        private const int BUFFER_SIZE = 1024;

        // Buffers that will contain incoming bytes and characters
        private byte[] incomingBytes = new byte[BUFFER_SIZE];
        private char[] incomingChars = new char[BUFFER_SIZE];

        // Records whether an asynchronous send attempt is ongoing
        private bool sendIsOngoing = false;

        // For synchronizing sends
        private readonly object sendSync = new object();

        // Bytes that we are actively trying to send, along with the
        // index of the leftmost byte whose send has not yet been completed
        private byte[] pendingBytes = new byte[0];
        private int pendingIndex = 0;

        private StringBuilder incoming = new StringBuilder();

        private class sendCallbackPair
        {
            public string str { get; set; }
            public SendCallback callback { get; set; }
            public object payload { get; set; }
            public sendCallbackPair(string str, SendCallback callback, object payload)
            {
                this.str = str;
                this.callback = callback;
                this.payload = payload;
            }
        }

        private class recieveCallbackPair
        {
            public string str { get; set; }
            public ReceiveCallback callback { get; set; }
            public object payload { get; set; }
            public recieveCallbackPair(string str, ReceiveCallback callback, object payload)
            {
                this.str = str;
                this.callback = callback;
                this.payload = payload;
            }
        }

        private sendCallbackPair currSentPair;
        private recieveCallbackPair currRecievePair;


        private Queue<sendCallbackPair> sendQue = new Queue<sendCallbackPair>();
        private Queue<recieveCallbackPair> recieveQue = new Queue<recieveCallbackPair>();

        /// <summary>
        /// Creates a StringSocket from a regular Socket, which should already be connected.  
        /// The read and write methods of the regular Socket must not be called after the
        /// StringSocket is created.  Otherwise, the StringSocket will not behave properly.  
        /// The encoding to use to convert between raw bytes and strings is also provided.
        /// </summary>
        internal StringSocket(Socket s, Encoding e)
        {
            socket = s;
            encoding = e;
            decoder = encoding.GetDecoder();

            // Ask the socket to call MessageReceive as soon as up to 1024 bytes arrive.
            socket.BeginReceive(incomingBytes, 0, incomingBytes.Length,
                                SocketFlags.None, MessageReceived, null);
        }

        /// <summary>
        /// Shuts down this StringSocket.
        /// </summary>
        public void Shutdown(SocketShutdown mode)
        {
            socket.Shutdown(mode);
        }

        /// <summary>
        /// Closes this StringSocket.
        /// </summary>
        public void Close()
        {
            socket.Close();
        }

        /// <summary>
        /// We can write a string to a StringSocket ss by doing
        /// 
        ///    ss.BeginSend("Hello world", callback, payload);
        ///    
        /// where callback is a SendCallback (see below) and payload is an arbitrary object.
        /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
        /// successfully written the string to the underlying Socket it invokes the callback.  
        /// The parameters to the callback are true and the payload.
        /// 
        /// If it is impossible to send because the underlying Socket has closed, the callback 
        /// is invoked with false and the payload as parameters.
        ///
        /// This method is non-blocking.  This means that it does not wait until the string
        /// has been sent before returning.  Instead, it arranges for the string to be sent
        /// and then returns.  When the send is completed (at some time in the future), the
        /// callback is called on another thread.
        /// 
        /// This method is thread safe.  This means that multiple threads can call BeginSend
        /// on a shared socket without worrying around synchronization.  The implementation of
        /// BeginSend must take care of synchronization instead.  On a given StringSocket, each
        /// string arriving via a BeginSend method call must be sent (in its entirety) before
        /// a later arriving string can be sent.
        /// </summary>
        public void BeginSend(String s, SendCallback callback, object payload)
        {
            // TODO: Implement BeginSend
            // Get exclusive access to send mechanism

            lock (sendSync)
            {

                sendQue.Enqueue(new sendCallbackPair(s, callback, payload));

                // If there's not a send ongoing, start one.
                if (!sendIsOngoing)
                {
                    Console.WriteLine("Appending a " + s.Length + " char line, starting send mechanism");
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
                Console.WriteLine("\tSending " + (pendingBytes.Length - pendingIndex) + " bytes");
                socket.BeginSend(pendingBytes, pendingIndex, pendingBytes.Length - pendingIndex,
                                 SocketFlags.None, MessageSent, null);
            }
            else if (sendQue.Count > 0)
            {
                //currSentPair.callback(true, currSentPair.payload);
                currSentPair = sendQue.Dequeue();
                pendingIndex = 0;
                pendingBytes = encoding.GetBytes(currSentPair.str);
                socket.BeginSend(pendingBytes, 0, pendingBytes.Length,
                                 SocketFlags.None, MessageSent, null);
            }
            else
            {
                currSentPair.callback(true, currSentPair.payload);
                // If there's nothing to send, shut down for the time being.
                Console.WriteLine("Shutting down send mechanism\n");
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
            Console.WriteLine("\t" + bytesSent + " bytes were successfully sent");

            // Get exclusive access to send mechanism
            lock (sendSync)
            {
                if (bytesSent != 0)
                {
                    pendingIndex += bytesSent;
                    SendBytes();
                }
            }
        }

        /// <summary>
        /// We can read a string from the StringSocket by doing
        /// 
        ///     ss.BeginReceive(callback, payload)
        ///     
        /// where callback is a ReceiveCallback (see below) and payload is an arbitrary object.
        /// This is non-blocking, asynchronous operation.  When the StringSocket has read a
        /// string of text terminated by a newline character from the underlying Socket, it 
        /// invokes the callback.  The parameters to the callback are a string and the payload.  
        /// The string is the requested string (with the newline removed).
        /// 
        /// Alternatively, we can read a string from the StringSocket by doing
        /// 
        ///     ss.BeginReceive(callback, payload, length)
        ///     
        /// If length is negative or zero, this behaves identically to the first case.  If length
        /// is positive, then it reads and decodes length bytes from the underlying Socket, yielding
        /// a string s.  The parameters to the callback are s and the payload
        ///
        /// In either case, if there are insufficient bytes to service a request because the underlying
        /// Socket has closed, the callback is invoked with null and the payload.
        /// 
        /// This method is non-blocking.  This means that it does not wait until a line of text
        /// has been received before returning.  Instead, it arranges for a line to be received
        /// and then returns.  When the line is actually received (at some time in the future), the
        /// callback is called on another thread.
        /// 
        /// This method is thread safe.  This means that multiple threads can call BeginReceive
        /// on a shared socket without worrying around synchronization.  The implementation of
        /// BeginReceive must take care of synchronization instead.  On a given StringSocket, each
        /// arriving line of text must be passed to callbacks in the order in which the corresponding
        /// BeginReceive call arrived.
        /// 
        /// Note that it is possible for there to be incoming bytes arriving at the underlying Socket
        /// even when there are no pending callbacks.  StringSocket implementations should refrain
        /// from buffering an unbounded number of incoming bytes beyond what is required to service
        /// the pending callbacks.
        /// </summary>
        public void BeginReceive(ReceiveCallback callback, object payload, int length = 0)
        {
            // TODO: Implement BeginReceive
            if(currRecievePair != null)
            {
                recieveQue.Enqueue(new recieveCallbackPair("", callback, payload));
            }
            else
            {
                currRecievePair = new recieveCallbackPair("", callback, payload);
            }
            
            socket.BeginReceive(incomingBytes, 0, incomingBytes.Length,
                    SocketFlags.None, MessageReceived, null);
        }

        private void MessageReceived(IAsyncResult result)
        {
            int bytesRead = socket.EndReceive(result);

            // Otherwise, decode and display the incoming bytes.  Then request more bytes.
            if (bytesRead != 0)
            {
                // Echo any complete lines, after capitalizing them
                int lastNewline = -1;
                int charsRead = decoder.GetChars(incomingBytes, 0, bytesRead, incomingChars, 0, false);
                incoming.Append(incomingChars, 0, charsRead);
                for (int i = 0; i < incoming.Length; i++)
                {
                    
                    if (incoming[i] == '\n')
                    {
                        String line = incoming.ToString(0, i);
                        currRecievePair.callback(line, currRecievePair.payload);
                        lastNewline = i;
                        if (recieveQue.Count > 0)
                        {
                            currRecievePair = recieveQue.Dequeue();
                        }
                    }
                }
                incoming.Remove(0, lastNewline + 1);
            }

            // Ask for some more data
            socket.BeginReceive(incomingBytes, 0, incomingBytes.Length,
                    SocketFlags.None, MessageReceived, null);
        }
    
        /// <summary>
        /// Frees resources associated with this StringSocket.
        /// </summary>
        public void Dispose()
        {
            Shutdown(SocketShutdown.Both);
            Close();
        }
    }  
}
