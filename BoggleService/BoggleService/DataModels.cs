using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
    [DataContractAttribute]
    public class Game
    {
        [DataMember]
        public string UserToken { get; set; }
        [DataMember]
        public int TimeLimit { get; set; }
    }

    /// <summary>
    /// UserInfo stores users nickname and gameID
    /// </summary>
    [DataContractAttribute]
    public class UserInfo
    {
        [DataMember]
        public String Nickname { get; set; }
        [DataMember]
        public int GameID { get; set; }
    }

    /// <summary>
    /// ID class, stores gameID
    /// </summary>
    [DataContractAttribute]
    public class ID
    {
        [DataMember]
        public string GameID { get; set; }
    }

    /// <summary>
    /// Playword class, stores a UserToken and a Word played
    /// </summary>
    [DataContractAttribute]
    public class PlayWord
    {
        [DataMember]
        public string UserToken { get; set; }
        [DataMember]
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
    [DataContractAttribute]
    public class GameStatus
    {
        [DataMember]
        public string GameState { get; set; }
        [DataMember]
        public DateTime startTime { get; set; }
        [DataMember]
        public BoggleBoard Board { get; set; }
        [DataMember]
        public int TimeLimit { get; set; }
        [DataMember]
        public int TimeLeft { get; set; }
        [DataMember]
        public Player Player1 { get; set; }
        [DataMember]
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
    [DataContractAttribute]
    public class Player
    {
        [DataMember]
        public string Nickname { get; set; }
        [DataMember]
        public string UserToken { get; set; }
        [DataMember]
        public int Score { get; set; }
        [DataMember]
        public HashSet<String> WordsPlayed { get; set; }
        public Player()
        {
            WordsPlayed = new HashSet<String>();
        }
        [DataMember]
        public List<WordPlayed> WordsList { get; set; }
    }

    /// <summary>
    /// Stores a Word and the score it is worth
    /// </summary>
    [DataContractAttribute]
    public class WordPlayed
    {
        [DataMember]
        public string Word { get; set; }
        [DataMember]
        public int Score { get; set; }
    }

    /// <summary>
    /// GameStatus class, stores all of the data for a game
    /// </summary>
    [DataContractAttribute]
    public class GameStatusReturn
    {
        [DataMember]
        public string GameState { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public String Board { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int TimeLimit { get; set; }
        [DataMember]
        public int TimeLeft { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public PlayerReturn Player1 { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public PlayerReturn Player2 { get; set; }


    }

    [DataContractAttribute]
    public class PlayerReturn
    {
        [DataMember(EmitDefaultValue = false)]
        public string Nickname { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string UserToken { get; set; }
        [DataMember]
        public int Score { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public List<WordPlayed> WordsPlayed { get; set; }
    }
}