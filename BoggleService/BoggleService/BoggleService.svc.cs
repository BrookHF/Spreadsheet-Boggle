using System;
using System.Collections.Generic;
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

        private readonly static Dictionary<string, User> users = new Dictionary<string, User>();
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

        /// <summary>
        /// Demo.  You can delete this.
        /// </summary>
        public string WordAtIndex(int n)
        {
            if (n < 0)
            {
                SetStatus(Forbidden);
                return null;
            }

            string line;
            using (StreamReader file = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "dictionary.txt"))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (n == 0) break;
                    n--;
                }
            }

            if (n == 0)
            {
                SetStatus(OK);
                return line;
            }
            else
            {
                SetStatus(Forbidden);
                return null;
            }
        }

        /// <summary>
        /// Handles a call to the server to create a new user based on the nickname passed in.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Token CreateUser(User user)
        {
            lock (sync)
            {
                //For the first launch of the server, populate the dictionary
                if (dictionary.Count == 0)
                {
                    FillDictionary();
                }
                user.Nickname = user.Nickname.Trim();

                //Checks to make sure a non-empty nickname was provided
                if (user.Nickname == null || user.Nickname.Equals(""))
                {
                    SetStatus(Forbidden);
                    return null;
                }

                Token token = new Token();
                token.UserToken = Guid.NewGuid().ToString(); //Generates new token for user

                users.Add(token.UserToken, user); //Stores nickname + token in dictionary.
                SetStatus(Created);
                return token;  
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
                //checks to make sure user is registered, and game time is withing 5-120 seconds.
                if (!users.ContainsKey(game.UserToken) || game.TimeLimit<5 || game.TimeLimit>120)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                //if they are the first player to join the game
                if(CurrGame == null)
                {
                    
                    CurrGame = new GameStatus();
                    User user = new User();
                    users.TryGetValue(game.UserToken, out user);
                    //CurrGame.Player1 = new Player();
                    CurrGame.Player1.Nickname = user.Nickname;
                    CurrGame.Player1.UserToken = game.UserToken;
                    CurrGame.Player1.Score = 0;
                    CurrGame.Player1.WordsList = new List<WordPlayed>();
                    CurrGame.TimeLimit = game.TimeLimit;
                    SetStatus(Accepted);
                    ID returnID = new ID();
                    returnID.GameID = GameIDCount.ToString();
                    return returnID;
                }
                else //if they are the 2nd player to join game
                {
                    //makes sure player doesnt play against themself
                    if (CurrGame.Player1.UserToken.Equals(game.UserToken))
                    {
                        SetStatus(Conflict);
                        return null;
                    }

                    User user = new User();
                    users.TryGetValue(game.UserToken, out user);
                    CurrGame.GameState = "active";
                    CurrGame.Player2.Nickname = user.Nickname;
                    CurrGame.Player2.UserToken = game.UserToken;
                    CurrGame.Player2.Score = 0;
                    CurrGame.Player2.WordsList = new List<WordPlayed>();
                    CurrGame.TimeLimit = (CurrGame.TimeLimit + game.TimeLimit) / 2;
                    CurrGame.TimeLeft = CurrGame.TimeLimit;
                    CurrGame.startTime = DateTime.Now;
                    gameStatus.Add(GameIDCount.ToString(), CurrGame);
                    CurrGame = null;
                    SetStatus(Created);
                    ID returnID = new ID();
                    returnID.GameID = GameIDCount.ToString();
                    GameIDCount++;
                    return returnID;
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
                if (playWord.Word == null || playWord.Word.Equals("") || playWord.UserToken == null || playWord.UserToken.Equals(""))
                {
                    SetStatus(Forbidden);
                    return null;
                }
                GameStatus status;
                if(!gameStatus.TryGetValue(GameID, out status) || (!status.Player1.UserToken.Equals(playWord.UserToken)  && !status.Player2.UserToken.Equals(playWord.UserToken)))
                {
                    SetStatus(Forbidden);
                    return null;
                }
                if(!status.GameState.Equals("active"))
                {
                    SetStatus(Conflict);
                    return null;
                }
                SetStatus(OK);
                WordPlayed wordPlayed = new WordPlayed();
                ScoreChange score = new ScoreChange();
                wordPlayed.Word = playWord.Word;
                bool isPlayer1 = status.Player1.UserToken.Equals(playWord.UserToken);
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
                        case 1: score.Score = 0;
                            break;
                        case 2: score.Score = 0;
                            break;
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
                        status.Player1.WordsList.Add(wordPlayed);
                    }
                    else
                    {
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
                        status.Player1.WordsList.Add(wordPlayed);
                    }
                    else
                    {
                        status.Player2.WordsList.Add(wordPlayed);
                    }
                    return score;
                }
            }     
        }

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
                    
                //if (status.TimeLeft <= 0)
                //{
                //    status.TimeLeft = 0;
                //    status.GameState = "completed";
                //    if (brief == "yes") //Completed + brief = yes
                //    {
                //        gameStatusReturn.GameState = status.GameState;
                //        gameStatusReturn.TimeLeft = status.TimeLeft;
                //        gameStatusReturn.Player1 = new PlayerReturn();
                //        gameStatusReturn.Player2 = new PlayerReturn();
                //        gameStatusReturn.Player1.Score = status.Player1.Score;
                //        gameStatusReturn.Player2.Score = status.Player2.Score;
                //    }
                //    else //Completed and no brief=yes
                //    {
                //        gameStatusReturn.GameState = status.GameState;
                //        gameStatusReturn.Board = status.Board.ToString();
                //        gameStatusReturn.TimeLimit = status.TimeLimit;
                //        gameStatusReturn.TimeLeft = 0;

                //        PlayerReturn Player1 = new PlayerReturn();
                //        Player1.Nickname = status.Player1.Nickname;
                //        Player1.Score = status.Player1.Score;
                //        Player1.WordsPlayed = status.Player1.WordsList;

                //        PlayerReturn Player2 = new PlayerReturn();
                //        Player2.Nickname = status.Player2.Nickname;
                //        Player2.Score = status.Player2.Score;
                //        Player2.WordsPlayed = status.Player2.WordsList;

                //        gameStatusReturn.Player1 = Player1;
                //        gameStatusReturn.Player2 = Player2;
                //    }
                //    return gameStatusReturn;
                //}
                if (brief != null) //Active + brief = yes
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
                if (brief != null) //Completed + brief = yes
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

        ///// <summary>
        ///// Helper method to obtain a nickname from a user token
        ///// </summary>
        ///// <param name="userToken"></param>
        ///// <returns></returns>
        //private string getNickname(string userToken)
        //{
        //    User name;
        //    if(users.TryGetValue(userToken, out name))
        //    {
        //        return name.Nickname;
        //    }
        //    else
        //    {
        //        return null;
        //    }
            
        //}

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
