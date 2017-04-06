using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        private static readonly object sync = new object();
        private readonly static HashSet<String> dictionary = new HashSet<string>();
        
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
            BoggleDB = ConfigurationManager.ConnectionStrings["BoggleDB"].ConnectionString;
        }


        /// <summary>
        /// Handles a call to the server to create a new user based on the nickname passed in.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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
                
                if (user.Nickname == null || user.Nickname.Trim().Length == 0 || user.Nickname.Trim().Length > 50)
                {
                    SetStatus(Forbidden);
                    return null;
                }

                
                using (SqlConnection conn = new SqlConnection(BoggleDB))
                {
                    // Connections must be opened
                    conn.Open();

                    
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        
                        using (SqlCommand command =
                            new SqlCommand("insert into Users (UserToken, Nickname) values(@UserToken, @Nickname)",
                                            conn,
                                            trans))
                        {
                            // We generate the userID to use.
                            string UserToken = Guid.NewGuid().ToString();

                            
                            command.Parameters.AddWithValue("@UserToken", UserToken);
                            command.Parameters.AddWithValue("@Nickname", user.Nickname.Trim());

                            token.UserToken = UserToken;
                            
                            command.ExecuteNonQuery();
                            SetStatus(Created);

                            trans.Commit();
                            return token;
                        }
                    }
                }
            }
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
                if (game.UserToken == null || game.UserToken == "")
                {
                    SetStatus(Forbidden);
                    return null;
                }

                using (SqlConnection conn = new SqlConnection(BoggleDB))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {

                        
                        using (SqlCommand command = new SqlCommand("select UserToken from Users where UserToken = @UserToken", conn, trans))
                        {
                            command.Parameters.AddWithValue("@UserToken", game.UserToken);

                            //if there are no rows with usertoken then user is not registered yet
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                
                                if (!reader.HasRows)
                                {
                                    SetStatus(Forbidden);
                                    return null;
                                }
                            }
                            //if they enter an invalid timelimit 
                            if (game.TimeLimit < 5 || game.TimeLimit > 120)
                            {
                                SetStatus(Forbidden);
                                return null;
                            }
                        }

                        using (SqlCommand command = new SqlCommand("select GameState, GameID, TimeLimit, Player1 from Games where GameState = @GameState", conn, trans))
                        {
                            command.Parameters.AddWithValue("@GameState", "pending");
                            bool hasRows;
                            string tempPlayer1 = "";
                            int tempTimeLimit = 0;
                            int tempGameID = 0;

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                hasRows = reader.HasRows;
                                if (hasRows)
                                {
                                    reader.Read();
                                    tempPlayer1 = (string)reader["Player1"];
                                    tempTimeLimit = (int)reader["TimeLimit"];
                                    tempGameID = (int)reader["GameID"];
                                }
                            }                       

                            if (hasRows)
                            {
                                //makes sure player doesnt play against themself
                                if (tempPlayer1 == game.UserToken)
                                {
                                    SetStatus(Conflict);
                                    return null;
                                }

                                using (SqlCommand updateCommand = new SqlCommand("UPDATE Games SET Player2 = @Player2, GameState = @GameState, Board = @Board, TimeLimit = @TimeLimit, StartTime = @StartTime where GameState = 'pending'", conn, trans))
                                {
                                    updateCommand.Parameters.AddWithValue("@Player2", game.UserToken);
                                    updateCommand.Parameters.AddWithValue("@GameState", "active");
                                    updateCommand.Parameters.AddWithValue("@TimeLimit", (game.TimeLimit + tempTimeLimit) / 2);
                                    BoggleBoard board = new BoggleBoard();
                                    updateCommand.Parameters.AddWithValue("@Board", board.ToString());
                                    updateCommand.Parameters.AddWithValue("@StartTime", DateTime.Now);
                                    
                                    updateCommand.ExecuteNonQuery();
                                    SetStatus(Created);
                                    trans.Commit();
                                    ID returnID = new ID();
                                    returnID.GameID = tempGameID.ToString();
                                    return returnID;
                                }
                            }


                            using (SqlCommand insertCommand = new SqlCommand("insert into Games (Player1, GameState, TimeLimit) output inserted.GameID values(@Player1, @GameState, @TimeLimit)", conn, trans))
                            {
                                insertCommand.Parameters.AddWithValue("@Player1", game.UserToken);
                                insertCommand.Parameters.AddWithValue("@GameState", "pending");
                                insertCommand.Parameters.AddWithValue("@TimeLimit", game.TimeLimit);
                                
                                insertCommand.ExecuteNonQuery();
                                SetStatus(Accepted);
                                trans.Commit();

                            }
                            using (SqlCommand selectCommand = new SqlCommand("select GameID from Games where GameState = @GameState", conn, trans))
                            {
                                selectCommand.Parameters.AddWithValue("@GameState", "pending");

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    reader.Read();
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
        

        /// <summary>
        /// Handles a request to cancel searching for a game.
        /// </summary>
        /// <param name="token"></param>
        public void CancelJoin(Token token)
        {
            lock (sync)
            {
                using (SqlConnection conn = new SqlConnection(BoggleDB))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {

                        using (SqlCommand command = new SqlCommand("select Player1 from Games where GameState = @GameState", conn, trans))
                        {
                            command.Parameters.AddWithValue("@GameState", "pending");

                            using (SqlDataReader reader = command.ExecuteReader())
                            {

                                if (!reader.Read() || (string)reader["Player1"] != token.UserToken)
                                {
                                    SetStatus(Forbidden);
                                    return;
                                }

                            }
                            using (SqlCommand deleteCommand = new SqlCommand("delete from Games where GameState = @GameState", conn, trans))
                            {
                                deleteCommand.Parameters.AddWithValue("@GameState", "pending");
                                deleteCommand.ExecuteNonQuery();
                                SetStatus(OK);
                                trans.Commit();
                                return;
                            }
                        }
                    }
                }              
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
                using (SqlConnection conn = new SqlConnection(BoggleDB))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        playWord.Word = playWord.Word.Trim();
                        playWord.Word = playWord.Word.ToUpper();

                        GameStatusReturn r = GetGameStatus("yes", GameID);

                        if (playWord.Word == null || playWord.Word.Equals("") || playWord.UserToken == null || playWord.UserToken.Equals(""))
                        {
                            SetStatus(Forbidden);
                            return null;
                        }

                        //Checks if usertoken is in our database
                        using (SqlCommand command = new SqlCommand("select Nickname from Users where UserToken = @UserToken", conn, trans))
                        {
                            command.Parameters.AddWithValue("@UserToken", playWord.UserToken);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (!reader.HasRows)
                                {
                                    SetStatus(Forbidden);
                                    return null;
                                }
                            }
                        }
                        string tempPlayer1 = "";
                        string tempPlayer2 = "";
                        BoggleBoard tempBoard;
                        //Need gamestate, board, player1, player2, needs to match gameID
                        using (SqlCommand command = new SqlCommand("select Player1, Player2, GameState, Board from Games where GameID = @GameID", conn, trans))
                        {
                            command.Parameters.AddWithValue("@GameID", GameID);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if(!reader.HasRows)
                                {
                                    SetStatus(Forbidden);
                                    return null;
                                }
                                reader.Read();
                                if(reader["GameState"].ToString() != "active")
                                {
                                    SetStatus(Conflict);
                                    return null;
                                }
                                if (!reader["Player1"].Equals(playWord.UserToken) && !reader["Player2"].Equals(playWord.UserToken))
                                {
                                    SetStatus(Forbidden);
                                    return null;
                                }
                                tempPlayer1 = reader["Player1"].ToString();
                                tempPlayer2 = reader["Player2"].ToString();
                                tempBoard = new BoggleBoard(reader["Board"].ToString());
                            }
                        }
                        
                        SetStatus(OK);
                        WordPlayed wordPlayed = new WordPlayed();
                        ScoreChange score = new ScoreChange();
                        wordPlayed.Word = playWord.Word;
                        bool isPlayer1 = tempPlayer1.Equals(playWord.UserToken);
                        if (playWord.Word.Length <= 2)
                        {
                            wordPlayed.Score = 0;
                            score.Score = 0;
                        }
                        else if (tempBoard.CanBeFormed(playWord.Word) && dictionary.Contains(playWord.Word))
                        {
                            switch (playWord.Word.Length)
                            {
                                case 3:
                                    score.Score = 1;
                                    break;
                                case 4:
                                    score.Score = 1;
                                    break;
                                case 5:
                                    score.Score = 2;
                                    break;
                                case 6:
                                    score.Score = 3;
                                    break;
                                case 7:
                                    score.Score = 5;
                                    break;
                                default:
                                    score.Score = 11;
                                    break;
                            }

                            //if duplicate word then score is 0
                            using (SqlCommand command = new SqlCommand("select Word from Words where GameID = @GameID and UserToken = @UserToken and Word = @Word", conn, trans))
                            {
                                command.Parameters.AddWithValue("@GameID", GameID);
                                command.Parameters.AddWithValue("@Word", playWord.Word);
                                if (isPlayer1)
                                {
                                    command.Parameters.AddWithValue("@UserToken", tempPlayer1);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue("@UserToken", tempPlayer2);
                                }
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if(reader.HasRows)
                                    {
                                        score.Score = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            score.Score = -1;
                        }
                        //update DB, return score
                        using (SqlCommand insertCommand = new SqlCommand("insert into Words (Word, GameID, UserToken, Score) values(@Word, @GameID, @UserToken, @Score)", conn, trans))
                        {
                            insertCommand.Parameters.AddWithValue("@Word", playWord.Word);
                            insertCommand.Parameters.AddWithValue("@GameID", GameID);
                            if(isPlayer1)
                            {
                                insertCommand.Parameters.AddWithValue("@UserToken", tempPlayer1);
                            }
                            else
                            {
                                insertCommand.Parameters.AddWithValue("@UserToken", tempPlayer2);
                            }
                            insertCommand.Parameters.AddWithValue("@Score", score.Score);

                            insertCommand.ExecuteNonQuery();
                            trans.Commit();
                        }
                        return score;
                    }
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
            lock (sync)
            {
                using (SqlConnection conn = new SqlConnection(BoggleDB))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        string tempGameState = "";
                        int tempGameID = 0;
                        string tempBoard = "";
                        int tempTimeLeft = 0;
                        DateTime tempStartTime = new DateTime();
                        int tempTimeLimit = 0;
                        string tempPlayer1 = "";
                        string tempPlayer2 = "";
                        dynamic tempPlayer1Info = new ExpandoObject();
                        dynamic tempPlayer2Info = new ExpandoObject();
                        tempPlayer1Info.Nickname = "";
                        tempPlayer1Info.Score = 0;
                        tempPlayer1Info.WordsList = new List<WordPlayed>();
                        tempPlayer2Info.Nickname = "";
                        tempPlayer2Info.Score = 0;
                        tempPlayer2Info.WordsList = new List<WordPlayed>();

                        //update info from games
                        using (SqlCommand command = new SqlCommand("select * from Games where GameID = @GameID", conn, trans))
                        {
                            command.Parameters.AddWithValue("@GameID", GameID);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    tempGameState = (string)reader["GameState"];
                                    
                                    if (tempGameState == "pending") //Response if pending   || reader["Player2"] == System.DBNull
                                    {
                                        GameStatusReturn gameStatusReturnPending = new GameStatusReturn();
                                        gameStatusReturnPending.GameState = "pending";
                                        return gameStatusReturnPending;
                                    }
                                    tempGameID = (int)reader["GameID"];
                                    tempPlayer1 = (string)reader["Player1"];
                                    
                                    tempPlayer2 = (string)reader["Player2"];
                                    
                                    tempBoard = (string)reader["Board"];
                                    tempStartTime = (DateTime)reader["StartTime"];
                                    tempTimeLimit = (int)reader["TimeLimit"];
                                }
                                else
                                {
                                    SetStatus(Forbidden);
                                    return null;
                                }
                            }
                        }

                        //update info from Words
                        using (SqlCommand command = new SqlCommand("select * from Words where GameID = @GameID", conn, trans))
                        {
                            command.Parameters.AddWithValue("@GameID", GameID);

                            
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    WordPlayed wordPlayed = new WordPlayed();
                                    wordPlayed.Word = (string)reader["Word"];
                                    wordPlayed.Score = (int)reader["Score"];
                                    if (reader["UserToken"].ToString() == tempPlayer1)
                                    {
                                        tempPlayer1Info.Score += (int)reader["Score"];
                                        tempPlayer1Info.WordsList.Add(wordPlayed);
                                    }
                                    else
                                    {
                                        tempPlayer2Info.Score += (int)reader["Score"];
                                        tempPlayer2Info.WordsList.Add(wordPlayed);
                                    }
                                }
                            }
                        }

                        //Get nickname from Users for player 1
                        using (SqlCommand command = new SqlCommand("select * from Users where UserToken = @UserToken", conn, trans))
                        {
                            command.Parameters.AddWithValue("@UserToken", tempPlayer1);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                reader.Read();
                                tempPlayer1Info.Nickname = reader["Nickname"];
                            }
                        }

                        //Get nickname from Users for player 2
                        using (SqlCommand command = new SqlCommand("select * from Users where UserToken = @UserToken", conn, trans))
                        {
                            command.Parameters.AddWithValue("@UserToken", tempPlayer2);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                reader.Read();
                                tempPlayer2Info.Nickname = reader["Nickname"];
                            }
                        }

                        GameStatusReturn gameStatusReturn = new GameStatusReturn();                       

                        //update time left if game is active
                        if (tempGameState == "active")
                        {
                            tempTimeLeft = tempTimeLimit - (int)(DateTime.Now - tempStartTime).TotalSeconds;
                            if (tempTimeLeft <= 0)
                            {
                                tempGameState = "completed";
                                tempTimeLeft = 0;
                                //Need to update DB for gamestate
                                using (SqlCommand updateCommand = new SqlCommand("UPDATE Games SET GameState = @GameState", conn, trans))
                                {
                                    updateCommand.Parameters.AddWithValue("@GameState", "completed");
                                    updateCommand.ExecuteNonQuery();
                                    trans.Commit();
                                }
                            }
                        }

                       
                        

                        
                        if (tempGameState == "active") //Response if active
                        {
                            if (brief == "yes") //Active + brief = yes
                            {
                                gameStatusReturn.GameState = tempGameState;
                                gameStatusReturn.TimeLeft = tempTimeLeft;
                                gameStatusReturn.Player1 = new PlayerReturn();
                                gameStatusReturn.Player2 = new PlayerReturn();
                                gameStatusReturn.Player1.Score = tempPlayer1Info.Score;
                                gameStatusReturn.Player2.Score = tempPlayer2Info.Score;
                            }
                            else //Active without brief = yes
                            {
                                gameStatusReturn.GameState = tempGameState;
                                gameStatusReturn.Board = tempBoard;
                                gameStatusReturn.TimeLimit = tempTimeLimit;
                                gameStatusReturn.TimeLeft = tempTimeLeft;

                                PlayerReturn Player1 = new PlayerReturn();
                                Player1.Nickname = tempPlayer1Info.Nickname;
                                Player1.Score = tempPlayer1Info.Score;
                                Player1.WordsPlayed = tempPlayer1Info.WordsList;

                                PlayerReturn Player2 = new PlayerReturn();
                                Player2.Nickname = tempPlayer2Info.Nickname;
                                Player2.Score = tempPlayer2Info.Score;
                                Player2.WordsPlayed = tempPlayer2Info.WordsList;

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
                                gameStatusReturn.Player1.Score = tempPlayer1Info.Score;
                                gameStatusReturn.Player2.Score = tempPlayer2Info.Score;
                            }
                            else //Completed and no brief=yes
                            {
                                gameStatusReturn.GameState = tempGameState;
                                gameStatusReturn.Board = tempBoard;
                                gameStatusReturn.TimeLimit = tempTimeLimit;
                                gameStatusReturn.TimeLeft = tempTimeLeft;

                                PlayerReturn Player1 = new PlayerReturn();
                                Player1.Nickname = tempPlayer1Info.Nickname;
                                Player1.Score = tempPlayer1Info.Score;
                                Player1.WordsPlayed = tempPlayer1Info.WordsList;

                                PlayerReturn Player2 = new PlayerReturn();
                                Player2.Nickname = tempPlayer2Info.Nickname;
                                Player2.Score = tempPlayer2Info.Score;
                                Player2.WordsPlayed = tempPlayer2Info.WordsList;

                                gameStatusReturn.Player1 = Player1;
                                gameStatusReturn.Player2 = Player2;
                            }
                        }
                        return gameStatusReturn;
                    }
                }
            }
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
