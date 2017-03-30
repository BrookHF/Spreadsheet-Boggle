using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boggle
{
    /// <summary>
    /// User class, stores nickname of user 
    /// </summary>
    public class User
    {
        public string Nickname { get; set; }

    }

    /// <summary>
    /// Token class, stores a User Token
    /// </summary>
    public class Token
    {
        public string UserToken { get; set; }
    }

    /// <summary>
    /// Game class, stores UserToken and TimeLimit
    /// </summary>
    public class Game
    {
        public string UserToken { get; set; }
        public int TimeLimit { get; set; }
    }

    /// <summary>
    /// UserInfo stores users nickname and gameID
    /// </summary>
    public class UserInfo
    {
        public String Nickname { get; set; } 
        public int GameID { get; set; }
    }

    /// <summary>
    /// ID class, stores gameID
    /// </summary>
    public class ID
    {
        public int GameID { get; set; }
        public ID(int id)
        {
            GameID = id;
        }
    }

    /// <summary>
    /// Playword class, stores a UserToken and a Word played
    /// </summary>
    public class PlayWord
    {
        public string UserToken { get; set; }
        public string Word { get; set; }
    }

    /// <summary>
    /// ScoreChange, stores score
    /// </summary>
    public class ScoreChange
    {
        public int Score { get; set; }
    }

    /// <summary>
    /// GameStatus class, stores all of the data for a game
    /// </summary>
    public class GameStatus
    {
        public string GameState { get; set; }
        public DateTime startTime { get; set; }
        public BoggleBoard Board { get; set; }
        public int TimeLimit { get; set; }
        public int TimeLeft { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public GameStatus()
        {
            GameState = "pending";
            Player1 = new Player();
            Player2 = new Player();
            Board = new BoggleBoard();
        }

    }

    /// <summary>
    /// Player class, stores all of the information of a player in a game.
    /// </summary>
    public class Player
    {
        public string Nickname { get; set; }
        public string UserToken { get; set; }
        public int Score { get; set; }
        public HashSet<String> WordsPlayed { get; set; }
        public Player()
        {
            WordsPlayed = new HashSet<String>();
        }
        public List<WordPlayed> WordsList { get; set; }
    }

    /// <summary>
    /// Stores a Word and the score it is worth
    /// </summary>
    public class WordPlayed
    {
        public string Word { get; set; }
        public int Score { get; set; }
    }
}