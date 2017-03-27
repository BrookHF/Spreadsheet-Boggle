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

    public class ID
    {
        public int GameID { get; set; }
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

    public class GameStatusPending
    {
        public string GameState { get; set; }
    }

    public class GameStatusBrief
    {
        public string GameState { get; set; }
        public int TimeLeft { get; set; }
        public PlayerBrief Player1 { get; set; }
        public PlayerBrief Player2 { get; set; }
    }

    public class GameStatusNoBrief
    {
        public string GameState { get; set; }
        public string Board { get; set; }
        public int TimeLimit { get; set; }
        public int TimeLeft { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
    }

    public class GameStatusCompleted
    {
        public string GameState { get; set; }
        public string Board { get; set; }
        public int TimeLimit { get; set; }
        public int TimeLeft { get; set; }
        public PlayerCompleted Player1 { get; set; }
        public PlayerCompleted Player2 { get; set; }
    }

    public class PlayerCompleted
    {
        public string Nickname { get; set; }
        public int Score { get; set; }
        public List<WordPlayed> WordsPlayed { get; set; }
    }

    public class WordPlayed
    {
        public string Word { get; set; } 
        public int score { get; set; }
    }

    public class Player
    {
        public string Nickname { get; set; }
        public int Score { get; set; }
    }

    public class PlayerBrief
    {
        public int Score { get; set; }
    }
}