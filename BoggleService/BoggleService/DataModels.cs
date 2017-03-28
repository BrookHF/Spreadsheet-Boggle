using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boggle
{
    public class User
    {
        public string Nickname { get; set; }

    }

    public class Token
    {
        public string UserToken { get; set; }
    }

    public class Game
    {
        public string UserToken { get; set; }
        public int TimeLimit { get; set; }
    }

    public class UserInfo
    {
        public String Nickname { get; set; } 
        public int GameID { get; set; }
    }

    public class ID
    {
        public int GameID { get; set; }
        public ID(int id)
        {
            GameID = id;
        }
    }

    public class PlayWord
    {
        public string UserToken { get; set; }
        public string Word { get; set; }
    }

    public class ScoreChange
    {
        public int Score { get; set; }
    }

    public class GameStatus
    {
        public string GameState { get; set; }
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

    public class WordPlayed
    {
        public string Word { get; set; }
        public int Score { get; set; }
    }
}