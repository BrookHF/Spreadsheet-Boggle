using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using System.Timers;
using System.Windows;
using static System.Net.HttpStatusCode;

namespace Boggle
{
    public class BoggleService : IBoggleService
    {
        private static string BoggleDB;
        private readonly static Dictionary<string, GameStatus> gameStatus = new Dictionary<string, GameStatus>();
        private static readonly object sync = new object();
        private static int GameIDCount = 100;
        private readonly static HashSet<String> dictionary = new HashSet<string>();
        private static GameStatus CurrGame = null;
        
        /// <summary>
        /// The most recent call to SetStatus determines the response code used when
        /// an http response is sent.
        /// </summary>
        /// <param name="status"></param>
        private static void SetStatus(HttpStatusCode status)
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = status;
        } 

        /// <summary>
        /// Returns a Stream version of index.html.
        /// </summary>
        /// <returns></returns>
        public Stream API()
        {
            SetStatus(OK);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            return File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "index.html");
        }

        static BoggleService()
        {
            // Saves the connection string for the database.  A connection string contains the
            // information necessary to connect with the database server.  When you create a
            // DB, there is generally a way to obtain the connection string.  From the Server
            // Explorer pane, obtain the properties of DB to see the connection string.

            // The connection string of my ToDoDB.mdf shows as
            //
            //    Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="C:\Users\zachary\Source\CS 3500 S16\examples\ToDoList\ToDoListDB\App_Data\ToDoDB.mdf";Integrated Security=True
            //
            // Unfortunately, this is absolute pathname on my computer, which means that it
            // won't work if the solution is moved.  Fortunately, it can be shorted to
            //
            //    Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="|DataDirectory|\ToDoDB.mdf";Integrated Security=True
            //
            // You should shorten yours this way as well.
            //
            // Rather than build the connection string into the program, I store it in the Web.config
            // file where it can be easily found and changed.  You should do that too.
            BoggleDB = ConfigurationManager.ConnectionStrings["BoggleDB"].ConnectionString;
        }

        /// <summary>
        /// Handles a call to the server to create a new user based on the nickname passed in.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        //public Token CreateUser(User user)
        //{
        //    lock (sync)
        //    {
        //        //For the first launch of the server, populate the dictionary
        //        if (dictionary.Count == 0)
        //        {
        //            FillDictionary();
        //        }

        //        if (user.Nickname == null)
        //        {
        //            SetStatus(Forbidden);
        //            return null;
        //        }

        //        user.Nickname = user.Nickname.Trim();

        //        //Checks to make sure a non-empty nickname was provided
        //        if (user.Nickname.Equals(""))
        //        {
        //            SetStatus(Forbidden);
        //            return null;
        //        }

        //        Token token = new Token();
        //        token.UserToken = Guid.NewGuid().ToString(); //Generates new token for user

        //        users.Add(token.UserToken, user); //Stores nickname + token in dictionary.
        //        SetStatus(Created);
        //        return token;  
        //    }
        //}

        public Token CreateUser(User user)
        {
            lock(sync)
            {
                Token token = new Token();

                //For the first launch of the server, populate the dictionary
                if (dictionary.Count == 0)
                {
                    FillDictionary();
                }

                // This validates the user.Name property.  It is best to do any work that doesn't
                // involve the database before creating the DB connection or after closing it.
                if (user.Nickname == null || user.Nickname.Trim().Length == 0 || user.Nickname.Trim().Length > 50)
                {
                    SetStatus(Forbidden);
                    return null;
                }

                // The first step to using the DB is opening a connection to it.  Creating it in a
                // using block guarantees that the connection will be closed when control leaves
                // the block.  As you'll see below, I also follow this pattern for SQLTransactions,
                // SqlCommands, and SqlDataReaders.
                using (SqlConnection conn = new SqlConnection(BoggleDB))
                {
                    // Connections must be opened
                    conn.Open();

                    // Database commands should be executed within a transaction.  When commands 
                    // are executed within a transaction, either all of the commands will succeed
                    // or all will be canceled.  You don't have to worry about some of the commands
                    // changing the DB and others failing.
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        // An SqlCommand executes a SQL statement on the database.  In this case it is an
                        // insert statement.  The first parameter is the statement, the second is the
                        // connection, and the third is the transaction.  
                        //
                        // Note that I use symbols like @UserID as placeholders for values that need to appear
                        // in the statement.  You will see below how the placeholders are replaced.  You may be
                        // tempted to simply paste the values into the string, but this is a BAD IDEA that violates
                        // a cardinal rule of DB Security 101.  By using the placeholder approach, you don't have
                        // to worry about escaping special characters and you don't have to worry about one form
                        // of the SQL injection attack.
                        using (SqlCommand command =
                            new SqlCommand("insert into Users (UserToken, Nickname) values(@UserToken, @Nickname)",
                                            conn,
                                            trans))
                        {
                            // We generate the userID to use.
                            string UserToken = Guid.NewGuid().ToString();

                            // This is where the placeholders are replaced.
                            command.Parameters.AddWithValue("@UserToken", UserToken);
                            command.Parameters.AddWithValue("@Nickname", user.Nickname.Trim());

                            token.UserToken = UserToken;
                            // This executes the command within the transaction over the connection.  The number of rows
                            // that were modified is returned.  Perhaps I should check and make sure that 1 is returned
                            // as expected.
                            command.ExecuteNonQuery();
                            SetStatus(Created);

                            // Immediately before each return that appears within the scope of a transaction, it is
                            // important to commit the transaction.  Otherwise, the transaction will be aborted and
                            // rolled back as soon as control leaves the scope of the transaction. 
                            trans.Commit();
                            return token;
                        }
                    }
                }
            }
            

            // The method can terminate with a thrown exception for any number of reasons.  For example:
            //
            //  - The DB connection might fail
            //  - The DB may find it necessary to abort the transaction because of a conflict with
            //     some other user's transaction
            //  - The insert may fail because it violates a primary key constraint 
            //
            // In a more robust implementation, we would want to catch and deal with exceptions if.  For example:
            //
            //  - If the DB aborts a transaction, we could just try it again.
            //  - If the userID violates a uniqueness constraint because it is a duplicate, we could generate
            //     a different one and try again.
            //
            // An unhandled exception in a REST request will not crash the server.  In a production server, we 
            // would want to record all unhandled exceptions to a log file for later examination.  Otherwise,
            // problems might remain undetected.  There is a probably a way to configure IIS so that it automtically 
            // logs exceptions.  
            //
            // For your purposes, it is best to simply let unhandled exceptions propagate.  This will make
            // debugging easier.
        }

        /// <summary>
        /// Handles call to server to join a game.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public ID JoinGame(Game game)
        {
            lock (sync)
            {
                if (game.UserToken == null)
                {
                    SetStatus(Forbidden);
                    return null;
                }

                using (SqlConnection conn = new SqlConnection(BoggleDB))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {

                        // Here, the SqlCommand is a select query.  We are interested in whether item.UserID exists in
                        // the Users table.
                        using (SqlCommand command = new SqlCommand("select UserToken from Users where UserToken = @UserToken", conn, trans))
                        {
                            command.Parameters.AddWithValue("@UserToken", game.UserToken);

                            // This executes a query (i.e. a select statement).  The result is an
                            // SqlDataReader that you can use to iterate through the rows in the response.
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                // In this we don't actually need to read any data; we only need
                                // to know whether a row was returned.
                                if (reader["UserToken"] == null)
                                {
                                    SetStatus(Forbidden);
                                    return null;
                                }
                            }
                            if (game.TimeLimit < 5 || game.TimeLimit > 120)
                            {
                                SetStatus(Forbidden);
                                return null;
                            }
                        }

                        using (SqlCommand command = new SqlCommand("select GameState, GameID, TimeLimit, Player1 from Games where GameState = @GameState", conn, trans))
                        {
                            command.Parameters.AddWithValue("@GameState", "pending");

                            // This executes a query (i.e. a select statement).  The result is an
                            // SqlDataReader that you can use to iterate through the rows in the response.
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                //There is not a pending game
                                if (!reader.HasRows)
                                {

                                    using (SqlCommand insertCommand = new SqlCommand("insert into Games (Player1, GameState, TimeLimit) output inserted.ItemID values(@Player1, @GameState, @TimeLimit)", conn, trans))
                                    {
                                        command.Parameters.AddWithValue("@Player1", game.UserToken);
                                        command.Parameters.AddWithValue("@GameState", "pending");
                                        command.Parameters.AddWithValue("@TimeLimit", game.TimeLimit);

                                        // We execute the command with the ExecuteScalar method, which will return to
                                        // us the requested auto-generated ItemID.
                                        string itemID = command.ExecuteScalar().ToString();
                                        SetStatus(Created);
                                        trans.Commit();
                                        ID returnID = new ID();
                                        returnID.GameID = GameIDCount.ToString();
                                        return returnID;
                                    }
                                }
                                //There is a pending game

                                else {
                                    //makes sure player doesnt play against themself
                                    if(reader["Player1"].ToString() == game.UserToken)
                                    {
                                        SetStatus(Conflict);
                                        return null;
                                    }

                                    using (SqlCommand updateCommand = new SqlCommand("UPDATE Games SET player2 = @Player2, GameState = @GameState, Board = @Board, TimeLimit = @TimeLimit, StartTime = @StartTime", conn, trans))
                                    {
                                        command.Parameters.AddWithValue("@Player2", game.UserToken);
                                        command.Parameters.AddWithValue("@GameState", "active");
                                        command.Parameters.AddWithValue("@TimeLimit", (game.TimeLimit + (int)reader["TimeLimit"]) / 2);
                                        BoggleBoard board = new BoggleBoard();
                                        command.Parameters.AddWithValue("@Board", board.ToString());
                                        command.Parameters.AddWithValue("@StartTime", DateTime.Now);

                                        // We execute the command with the ExecuteScalar method, which will return to
                                        // us the requested auto-generated ItemID.
                                        string itemID = command.ExecuteScalar().ToString();
                                        SetStatus(Created);
                                        trans.Commit();
                                        ID returnID = new ID();
                                        returnID.GameID = reader["GameID"].ToString();
                                        return returnID;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            }
        

        /// <summary>
        /// Handles a request to cancel searching for a game.
        /// </summary>
        /// <param name="token"></param>
        public void CancelJoin(Token token)
        {
            lock (sync)
            {
                if(CurrGame == null || !users.ContainsKey(token.UserToken) || CurrGame.Player1.UserToken != token.UserToken)
                {
                    SetStatus(Forbidden);
                    return;
                }
                //if there is a game with 1 player in it, and the player in it matches the player's token passed in, then sets the game to null.
                CurrGame = null;
                SetStatus(OK);
            }
        }

        /// <summary>
        /// PlayWord 
        /// </summary>
        /// <param name="playWord"></param>
        /// <param name="GameID"></param>
        /// <returns></returns>
        public ScoreChange PlayWord(PlayWord playWord, string GameID)
        {
            lock (sync)
            {
                playWord.Word = playWord.Word.Trim();
                playWord.Word = playWord.Word.ToUpper();

                GameStatusReturn r = GetGameStatus("yes", GameID);


                if (playWord.Word == null || playWord.Word.Equals("") || playWord.UserToken == null || playWord.UserToken.Equals(""))
                {
                    SetStatus(Forbidden);
                    return null;
                }
                GameStatus status;
                if(!gameStatus.TryGetValue(GameID, out status))
                {
                    if (GameIDCount.ToString() == GameID) {
                        SetStatus(Conflict);
                        return null;
                    }
                    SetStatus(Forbidden);
                    return null;
                }
                else if((!status.Player1.UserToken.Equals(playWord.UserToken)  && !status.Player2.UserToken.Equals(playWord.UserToken)))
                {
                    SetStatus(Forbidden);
                    return null;
                }
                if (!status.GameState.Equals("active"))
                {
                    SetStatus(Conflict);
                    return null;
                }
                SetStatus(OK);
                WordPlayed wordPlayed = new WordPlayed();
                ScoreChange score = new ScoreChange();
                wordPlayed.Word = playWord.Word;
                bool isPlayer1 = status.Player1.UserToken.Equals(playWord.UserToken);
                if (playWord.Word.Length <= 2)
                {
                    if (isPlayer1)
                    {
                        wordPlayed.Score = 0;
                        status.Player1.WordsList.Add(wordPlayed);
                        score.Score = 0;
                        return score;
                    }
                    else
                    {
                        wordPlayed.Score = 0;
                        status.Player2.WordsList.Add(wordPlayed);
                        score.Score = 0;
                        return score;
                    }
                }
                if (status.Board.CanBeFormed(playWord.Word) && dictionary.Contains(playWord.Word))
                {
                    if(isPlayer1)
                    {
                        if (!status.Player1.WordsPlayed.Add(playWord.Word))
                        {
                            wordPlayed.Score = 0;
                            status.Player1.WordsList.Add(wordPlayed);
                            score.Score = 0;
                            return score;
                        }
                    }
                    else
                    {
                        if (!status.Player2.WordsPlayed.Add(playWord.Word))
                        {
                            wordPlayed.Score = 0;
                            status.Player2.WordsList.Add(wordPlayed);
                            score.Score = 0;
                            return score;
                        }
                    }
                    switch (playWord.Word.Length)
                    {
                        case 3: score.Score = 1;
                            break;
                        case 4: score.Score = 1;
                            break;
                        case 5: score.Score = 2;
                            break;
                        case 6: score.Score = 3;
                            break;
                        case 7: score.Score = 5;
                            break;
                        default: score.Score = 11;
                            break;

                    }
                    wordPlayed.Score = score.Score;
                    if(isPlayer1)
                    {
                        status.Player1.Score += score.Score;
                        status.Player1.WordsList.Add(wordPlayed);
                    }
                    else
                    {
                        status.Player2.Score += score.Score;
                        status.Player2.WordsList.Add(wordPlayed);
                    }                 
                    return score;
                }
                else
                {
                    
                    score.Score = -1;
                    wordPlayed.Score = score.Score;
                    if (isPlayer1)
                    {
                        status.Player1.Score += score.Score;
                        status.Player1.WordsList.Add(wordPlayed);
                    }
                    else
                    {
                        status.Player2.Score += score.Score;
                        status.Player2.WordsList.Add(wordPlayed);
                    }
                    return score;
                }
            }     
        }

        /// <summary>
        /// If GameID is invalid, responds with status 403 (Forbidden).
        /// Otherwise, returns information about the game named by GameID as illustrated below.
        /// Note that the information returned depends on whether "Brief=yes" was included as a parameter 
        /// as well as on the state of the game. Responds with status code 200 (OK). 
        /// Note: The Board and Words are not case sensitive.
        /// </summary>
        /// <param name="brief"></param>
        /// <param name="GameID"></param>
        /// <returns></returns>
        public GameStatusReturn GetGameStatus(string brief, string GameID)
        {
            //if gameID is invalid, respond with 403 forbidden
            GameStatus status;
            GameStatusReturn gameStatusReturn = new GameStatusReturn();
            if (!gameStatus.ContainsKey(GameID)&& !GameIDCount.ToString().Equals(GameID))
            {
                SetStatus(Forbidden);
                return null;
            }
            
            if (!gameStatus.TryGetValue(GameID, out status))
            {
                status = CurrGame;
            }
            if(status.GameState != "pending")
            {
                status.TimeLeft = status.TimeLimit - (int)(DateTime.Now - status.startTime).TotalSeconds;
                if (status.TimeLeft <= 0)
                {
                    status.GameState = "completed";
                    status.TimeLeft = 0;
                }
            }
            
            if (status.GameState == "pending") //Response if pending
            {
                gameStatusReturn.GameState = "pending";
                return gameStatusReturn;
            }
            else if (status.GameState == "active") //Response if active
            {
                if (brief == "yes") //Active + brief = yes
                {
                    gameStatusReturn.GameState = status.GameState;
                    gameStatusReturn.TimeLeft = status.TimeLeft;
                    gameStatusReturn.Player1 = new PlayerReturn();
                    gameStatusReturn.Player2 = new PlayerReturn();
                    gameStatusReturn.Player1.Score = status.Player1.Score;
                    gameStatusReturn.Player2.Score = status.Player2.Score;
                }
                else //Active without brief = yes
                {
                    gameStatusReturn.GameState = status.GameState;
                    gameStatusReturn.Board = status.Board.ToString();
                    gameStatusReturn.TimeLimit = status.TimeLimit;
                    gameStatusReturn.TimeLeft = status.TimeLeft;
                    PlayerReturn Player1 = new PlayerReturn();
                    Player1.Nickname = status.Player1.Nickname;
                    Player1.Score = status.Player1.Score;
                    Player1.WordsPlayed = status.Player1.WordsList;

                    PlayerReturn Player2 = new PlayerReturn();
                    Player2.Nickname = status.Player2.Nickname;
                    Player2.Score = status.Player2.Score;
                    Player2.WordsPlayed = status.Player2.WordsList;

                    gameStatusReturn.Player1 = Player1;
                    gameStatusReturn.Player2 = Player2;

                }
            }
            else //Completed
            {
                if (brief == "yes") //Completed + brief = yes
                {
                    gameStatusReturn.GameState = "completed";
                    gameStatusReturn.TimeLeft = 0;
                    gameStatusReturn.Player1 = new PlayerReturn();
                    gameStatusReturn.Player2 = new PlayerReturn();
                    gameStatusReturn.Player1.Score = status.Player1.Score;
                    gameStatusReturn.Player2.Score = status.Player2.Score;
                }
                else //Completed and no brief=yes
                {
                    gameStatusReturn.GameState = status.GameState;
                    gameStatusReturn.Board = status.Board.ToString();
                    gameStatusReturn.TimeLimit = status.TimeLimit;
                    gameStatusReturn.TimeLeft = status.TimeLeft;
                    PlayerReturn Player1 = new PlayerReturn();
                    Player1.Nickname = status.Player1.Nickname;
                    Player1.Score = status.Player1.Score;
                    Player1.WordsPlayed = status.Player1.WordsList;

                    PlayerReturn Player2 = new PlayerReturn();
                    Player2.Nickname = status.Player2.Nickname;
                    Player2.Score = status.Player2.Score;
                    Player2.WordsPlayed = status.Player2.WordsList;

                    gameStatusReturn.Player1 = Player1;
                    gameStatusReturn.Player2 = Player2;
                }
            }
            return gameStatusReturn;

        }

        

        /// <summary>
        /// Populates our dictionary from dictionary.txt
        /// </summary>
        private void FillDictionary()
        {
            string line;
            using (StreamReader file = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "dictionary.txt"))
            {
                while ((line = file.ReadLine()) != null)
                {
                    dictionary.Add(line);
                }
            }
        }
    }
}
