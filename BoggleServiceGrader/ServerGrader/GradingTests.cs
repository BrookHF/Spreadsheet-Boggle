using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using Boggle;
using System.Dynamic;
using Newtonsoft.Json;
using static System.Net.HttpStatusCode;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ServerGrader
{
    /// <summary>
    /// NOTE:  The service must already be running elsewhere, such as in a separate Visual Studio
    /// or on a remote server, before these tests are run.  When the tests are started, the pending
    /// game should contain NO players.
    /// 
    /// For best results, run these tests against a server to which you have exlusive access.
    /// Othewise, competing users may interfere with the tests.
    /// </summary>
    [TestClass]
    public class GradingTests
    {
        /// <summary>
        /// Creates an HttpClient for communicating with the boggle server.
        /// </summary>
        private static HttpClient CreateClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:60000");
            //client.BaseAddress = new Uri("http://cs3500-boggle-s17.azurewebsites.net");
            return client;
        }

        /// <summary>
        /// Helper for serializaing JSON.
        /// </summary>
        private static StringContent Serialize(dynamic json)
        {
            return new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// All legal words
        /// </summary>
        private static readonly ISet<string> dictionary;

        static GradingTests()
        {
            dictionary = new HashSet<string>();
            using (StreamReader words = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"/dictionary.txt"))
            {
                string word;
                while ((word = words.ReadLine()) != null)
                {
                    dictionary.Add(word.ToUpper());
                }
            }
        }


        /// <summary>
        /// Given a board configuration, returns all the valid words.
        /// </summary>
        private static List<string> AllValidWords(string board)
        {
            BoggleBoard bb = new BoggleBoard(board);
            List<string> validWords = new List<string>();
            foreach (string word in dictionary)
            {
                if (word.Length > 2 && bb.CanBeFormed(word))
                {
                    validWords.Add(word);
                }
            }
            return validWords;
        }

        /// <summary>
        /// Given a board configuration, returns as many words of different lengths as possible.
        /// </summary>
        private static List<string> DifferentLengthWords(string board)
        {
            List<string> variety = new List<string>();
            List<string> allWords = AllValidWords(board);
            for (int i = 3; i <= 10; i++)
            {
                int length = i;
                string word = allWords.Find(w => w.Length == length);
                if (word != null)
                {
                    variety.Add(word);
                }
            }
            return variety;
        }

        /// <summary>
        /// Returns the score for a word.
        /// </summary>
        private static int GetScore(string word)
        {
            if (!dictionary.Contains(word) && word.Length >= 3)
            {
                return -1;
            }
            switch (word.Length)
            {
                case 1:
                case 2:
                    return 0;
                case 3:
                case 4:
                    return 1;
                case 5:
                    return 2;
                case 6:
                    return 3;
                case 7:
                    return 5;
                default:
                    return 11;
            }
        }

        /// <summary>
        /// Makes a user and asserts that the resulting status code is equal to the
        /// status parameter.  Returns a Task that will produce the new userID.
        /// </summary>
        private async Task<string> MakeUser(String nickname, HttpStatusCode status = 0)
        {
            dynamic name = new ExpandoObject();
            name.Nickname = nickname;

            using (HttpClient client = CreateClient())
            {
                HttpResponseMessage response = await client.PostAsync("/BoggleService.svc/users", Serialize(name));
                if (status != 0) Assert.AreEqual(status, response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    String result = await response.Content.ReadAsStringAsync();
                    dynamic user = JsonConvert.DeserializeObject(result);
                    return user.UserToken;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Joins the game and asserts that the resulting status code is equal to the parameter status.
        /// Returns a Task that will produce the new GameID.
        /// </summary>
        private async Task<string> JoinGame(String player, int timeLimit, HttpStatusCode status = 0)
        {
            dynamic user = new ExpandoObject();
            user.UserToken = player;
            user.TimeLimit = timeLimit;

            using (HttpClient client = CreateClient())
            {
                HttpResponseMessage response = await client.PostAsync("/BoggleService.svc/games", Serialize(user));
                if (status != 0) Assert.AreEqual(status, response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    String result = await response.Content.ReadAsStringAsync();
                    dynamic game = JsonConvert.DeserializeObject(result);
                    return game.GameID;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Joins the game returns a Task that will produce the resulting status.
        /// </summary>
        private async Task<HttpStatusCode> JoinGameStatus(String player)
        {
            dynamic user = new ExpandoObject();
            user.UserToken = player;
            user.TimeLimit = 10;

            using (HttpClient client = CreateClient())
            {
                HttpResponseMessage response = await client.PostAsync("/BoggleService.svc/games", Serialize(user));
                return response.StatusCode;
            }
        }

        /// <summary>
        /// Cancels the pending game and asserts that the resulting status code is
        /// equal to the parameter status.
        /// </summary>
        private async Task CancelGame(String player, HttpStatusCode status = 0)
        {
            dynamic user = new ExpandoObject();
            user.UserToken = player;

            using (HttpClient client = CreateClient())
            {
                HttpResponseMessage response = await client.PutAsync("/BoggleService.svc/games", Serialize(user));
                if (status != 0) Assert.AreEqual(status, response.StatusCode);
            }
        }

        /// <summary>
        /// Gets the status for the specified game and value of brief.  Asserts that the resulting
        /// status code is equal to the parameter status.  Returns a task that produces the object
        /// returned by the service.
        /// </summary>
        private async Task<dynamic> GetStatus(String game, string brief, HttpStatusCode status = 0)
        {
            using (HttpClient client = CreateClient())
            {
                HttpResponseMessage response = await client.GetAsync("/BoggleService.svc/games/" + game + "?brief=" + brief);
                if (status != 0) Assert.AreEqual(status, response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    String result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject(result);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Plays a word and asserts that the resulting status code is equal to the parameter
        /// status.  Returns a task that will produce the score of the word.
        /// </summary>
        private async Task<int> PlayWord(String player, String game, String word, HttpStatusCode status = 0)
        {
            dynamic play = new ExpandoObject();
            play.UserToken = player;
            play.Word = word;

            using (HttpClient client = CreateClient())
            {
                HttpResponseMessage response = await client.PutAsync("/BoggleService.svc/games/" + game, Serialize(play));
                if (status != 0) Assert.AreEqual(status, response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    String result = await response.Content.ReadAsStringAsync();
                    dynamic score = JsonConvert.DeserializeObject(result);
                    return score.Score;
                }
                else
                {
                    return -2;
                }
            }
        }

        /// <summary>
        /// Makes sure that the pending game has no players.
        /// </summary>
        public void Reset()
        {
            string player1 = MakeUser("a").Result;
            string player2 = MakeUser("b").Result;
            if (JoinGameStatus(player1).Result == Accepted)
            {
                JoinGameStatus(player2).Wait();
            }
        }


        // Null player name
        [TestMethod]
        public void MakeUser1()
        {
            MakeUser(null, Forbidden).Wait();
        }

        // Empty player name
        [TestMethod]
        public void MakeUser2()
        {
            MakeUser("", Forbidden).Wait();
        }

        // Successful creation status
        [TestMethod]
        public void MakeUser3()
        {
            MakeUser("Player", Created).Wait();
        }

        // Successful creation
        [TestMethod]
        public void MakeUser4()
        {
            string player = MakeUser("Player").Result;
            Assert.IsTrue(player.Length > 0);
        }


        // Bad UserID
        [TestMethod]
        public void JoinGame1()
        {
            JoinGame("xyzzy123", 10, Forbidden).Wait();
        }

        // Bad time limit
        [TestMethod]
        public void JoinGame2()
        {
            String player1 = MakeUser("Player 1").Result;
            JoinGame(player1, 1, Forbidden).Wait();
        }

        // Bad time limit
        [TestMethod]
        public void JoinGame3()
        {
            String player1 = MakeUser("Player 1", Created).Result;
            JoinGame(player1, 130, Forbidden).Wait();
        }

        // First player join correct status
        [TestMethod]
        public void JoinGame4()
        {
            Reset();
            String player1 = MakeUser("Player 1").Result;
            JoinGame(player1, 10, Accepted).Wait();
        }

        // First player join successful
        [TestMethod]
        public void JoinGame5()
        {
            Reset();
            String player1 = MakeUser("Player 1").Result;
            string game = JoinGame(player1, 10).Result;
            Assert.IsTrue(game.Length > 0);
        }

        // Same player can't play both sides
        [TestMethod]
        public void JoinGame6()
        {
            Reset();
            String player1 = MakeUser("Player 1").Result;
            JoinGame(player1, 10).Wait();
            JoinGame(player1, 20, Conflict).Wait();
        }

        // Game started correct status
        [TestMethod]
        public void JoinGame7()
        {
            Reset();
            String player1 = MakeUser("Player 1").Result;
            String player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            JoinGame(player2, 20, Created).Wait();
        }

        // Game started successfully
        [TestMethod]
        public void JoinGame8()
        {
            Reset();
            String player1 = MakeUser("Player 1").Result;
            String player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 20).Result;
            Assert.IsTrue(game2.Length > 0);
        }

        // Game started successfully
        [TestMethod]
        public void JoinGame9()
        {
            Reset();
            String player1 = MakeUser("Player 1").Result;
            String player2 = MakeUser("Player 2").Result;
            string game1 = JoinGame(player1, 10).Result;
            string game2 = JoinGame(player2, 20).Result;
            Assert.AreEqual(game1, game2);
        }

        // Two games started successfully
        [TestMethod]
        public void JoinGame10()
        {
            Reset();
            String player1 = MakeUser("Player 1").Result;
            String player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game1 = JoinGame(player2, 20).Result;

            String player3 = MakeUser("Player 3").Result;
            String player4 = MakeUser("Player 4").Result;
            JoinGame(player3, 10).Wait();
            string game2 = JoinGame(player4, 20).Result;

            Assert.AreNotEqual(game1, game2);
        }



        // Can't cancel with a bad UserID
        [TestMethod]
        public void TestCancelGame1()
        {
            Reset();
            CancelGame("xyzzy123", Forbidden).Wait(); ;
        }

        // Can't cancel a game you're not a part of
        [TestMethod]
        public void TestCancelGame2()
        {
            Reset();
            String player1 = MakeUser("Player 1").Result;
            String player2 = MakeUser("Player 2").Result;
            String game1 = JoinGame(player1, 10).Result;
            CancelGame(player2, Forbidden).Wait();
        }

        // First player can't cancel a running game
        [TestMethod]
        public void TestCancelGame3()
        {
            Reset();
            String player1 = MakeUser("Player 1").Result;
            String player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            JoinGame(player2, 10).Wait();
            CancelGame(player1, Forbidden).Wait();
        }

        // Second player can't cancel a running game
        [TestMethod]
        public void TestCancelGame4()
        {
            Reset();
            String player1 = MakeUser("Player 1").Result;
            String player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            JoinGame(player2, 10).Wait();
            CancelGame(player2, Forbidden).Wait();
        }

        // Correct cancellation status
        [TestMethod]
        public void TestCancelGame5()
        {
            Reset();
            String player1 = MakeUser("Player 1").Result;
            JoinGame(player1, 10).Wait();
            CancelGame(player1, OK).Wait();
        }

        // Game really was cancelled
        [TestMethod]
        public void TestCancelGame6()
        {
            Reset();
            String player1 = MakeUser("Player 1").Result;
            JoinGame(player1, 10).Wait();
            CancelGame(player1, OK).Wait();
            JoinGame(player1, 10, Accepted).Wait();
        }


        // Word can't be played in pending game
        [TestMethod]
        public void TestPlayWord1()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string game1 = JoinGame(player1, 10).Result;
            PlayWord(player1, game1, "a", Conflict).Wait();
        }

        // Word can't be played in completed game
        [TestMethod]
        public void TestPlayWord2()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 6).Wait();
            string game2 = JoinGame(player2, 6).Result;
            Thread.Sleep(7000);
            PlayWord(player1, game2, "a", Conflict).Wait();
        }

        // Word can't be played by a non-participant
        [TestMethod]
        public void TestPlayWord3()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            string player3 = MakeUser("Player 3").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            PlayWord(player3, game2, "a", Forbidden).Wait();
        }

        // Empty word can't be played
        [TestMethod]
        public void TestPlayWord4()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            PlayWord(player2, game2, "", Forbidden).Wait();
        }

        // Player must be valid
        [TestMethod]
        public void TestPlayWord5()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            PlayWord("xyzzy123", game2, "", Forbidden).Wait();
        }

        // Game must be valid
        [TestMethod]
        public void TestPlayWord6()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            PlayWord(player1, "xyzzy123", "", Forbidden).Wait();
        }

        // Bad word correct status for player 1
        [TestMethod]
        public void TestPlayWord7()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            PlayWord(player1, game2, "xyzzy123", OK).Wait();
        }

        // Bad word correct status for player 2
        [TestMethod]
        public void TestPlayWord8()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            PlayWord(player1, game2, "xyzzy123", OK).Wait();
        }

        // Bad word correct score for player 1
        [TestMethod]
        public void TestPlayWord9()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            int score = PlayWord(player1, game2, "xyzzy123").Result;
            Assert.AreEqual(-1, score);
        }

        // Bad word correct score for player 2
        [TestMethod]
        public void TestPlayWord10()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            int score = PlayWord(player2, game2, "xyzzy123").Result;
            Assert.AreEqual(-1, score);
        }

        // Too short correct status for player 1
        [TestMethod]
        public void TestPlayWord11()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            PlayWord(player1, game2, "be", OK).Wait();
        }

        // Too short correct status for player 2
        [TestMethod]
        public void TestPlayWord12()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            PlayWord(player2, game2, "be", OK).Wait();
        }

        // Too short correct score for player 1
        [TestMethod]
        public void TestPlayWord13()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            int score = PlayWord(player1, game2, "be").Result;
            Assert.AreEqual(0, score);
        }

        // Too short correct score for player 2
        [TestMethod]
        public void TestPlayWord14()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            int score = PlayWord(player2, game2, "be").Result;
            Assert.AreEqual(0, score);
        }

        // Good word correct status for player 1
        [TestMethod]
        public void TestPlayWord15()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            string board = GetStatus(game2, "no").Result.Board;
            List<string> words = DifferentLengthWords(board);
            words.Sort((s1, s2) => s1.Length - s2.Length);
            PlayWord(player1, game2, words[0], OK).Wait();
        }

        // Good word correct status for player 2
        [TestMethod]
        public void TestPlayWord16()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            string board = GetStatus(game2, "no").Result.Board;
            List<string> words = DifferentLengthWords(board);
            words.Sort((s1, s2) => s1.Length - s2.Length);
            PlayWord(player2, game2, words[0], OK).Wait();
        }

        // Good word correct score for player 1
        [TestMethod]
        public void TestPlayWord17()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            string board = GetStatus(game2, "no").Result.Board;
            List<string> words = DifferentLengthWords(board);
            words.Sort((s1, s2) => s1.Length - s2.Length);
            int score = PlayWord(player1, game2, words[0]).Result;
            Assert.AreEqual(GetScore(words[0]), score);
        }

        // Good word correct score for player 2
        [TestMethod]
        public void TestPlayWord18()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 10).Result;
            string board = GetStatus(game2, "no").Result.Board;
            List<string> words = DifferentLengthWords(board);
            words.Sort((s1, s2) => s1.Length - s2.Length);
            int score = PlayWord(player2, game2, words[0]).Result;
            Assert.AreEqual(GetScore(words[0]), score);
        }


        // Status of bad game
        [TestMethod]
        public void TestStatus1()
        {
            GetStatus("xyzzy123", "no", Forbidden).Wait();
        }

        // Status of pending game
        [TestMethod]
        public void TestStatus2()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string game1 = JoinGame(player1, 10).Result;
            dynamic status = GetStatus(game1, "no").Result;
            Assert.AreEqual("pending", status.GameState.ToString());
        }

        // GameState of active game
        [TestMethod]
        public void TestStatus3()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 20).Result;
            string state = GetStatus(game2, "no").Result.GameState;
            Assert.AreEqual("active", state);
        }

        // Board of active game
        [TestMethod]
        public void TestStatus4()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 20).Result;
            string board = GetStatus(game2, "no").Result.Board;
            Assert.AreEqual(16, board.Length);
        }

        // TimeLimit of active game
        [TestMethod]
        public void TestStatus5()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 20).Result;
            int limit = GetStatus(game2, "no").Result.TimeLimit;
            Assert.AreEqual(15, limit);
        }

        // TimeLeft of active game
        [TestMethod]
        public void TestStatus6()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 20).Result;
            int left = GetStatus(game2, "no").Result.TimeLeft;
            Assert.IsTrue(left <= 15 && left > 0);
        }

        // Nickname 1 of active game
        [TestMethod]
        public void TestStatus7()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 20).Result;
            string name1 = GetStatus(game2, "no").Result.Player1.Nickname;
            Assert.AreEqual("Player 1", name1);
        }

        // Nickname 2 of active game
        [TestMethod]
        public void TestStatus8()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 20).Result;
            string name2 = GetStatus(game2, "no").Result.Player2.Nickname;
            Assert.AreEqual("Player 2", name2);
        }

        // Score 1 of active game
        [TestMethod]
        public void TestStatus9()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 20).Result;
            int score1 = GetStatus(game2, "no").Result.Player1.Score;
            Assert.AreEqual(0, score1);
        }

        // Score 2 of active game
        [TestMethod]
        public void TestStatus10()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 20).Result;
            int score2 = GetStatus(game2, "no").Result.Player2.Score;
            Assert.AreEqual(0, score2);
        }

        // Score 1 of active game after playing word
        [TestMethod]
        public void TestStatus11()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 20).Result;
            PlayWord(player1, game2, "XYZZY").Wait();
            int score1 = GetStatus(game2, "no").Result.Player1.Score;
            Assert.AreEqual(-1, score1);
        }

        // Score 2 of active game after playing word
        [TestMethod]
        public void TestStatus12()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 10).Wait();
            string game2 = JoinGame(player2, 20).Result;
            PlayWord(player2, game2, "XYZZY").Wait();
            int score2 = GetStatus(game2, "no").Result.Player2.Score;
            Assert.AreEqual(-1, score2);
        }

        // GameState of completed game
        [TestMethod]
        public void TestStatus13()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 6).Wait();
            string game2 = JoinGame(player2, 6).Result;
            Thread.Sleep(7000);
            string state = GetStatus(game2, "no").Result.GameState;
            Assert.AreEqual("completed", state);
        }

        // TimeLimit of completed game
        [TestMethod]
        public void TestStatus14()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 6).Wait();
            string game2 = JoinGame(player2, 6).Result;
            Thread.Sleep(7000);
            int limit = GetStatus(game2, "no").Result.TimeLimit;
            Assert.AreEqual(6, limit);
        }

        // TimeLeft of completed game
        [TestMethod]
        public void TestStatus15()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 6).Wait();
            string game2 = JoinGame(player2, 6).Result;
            Thread.Sleep(7000);
            int left = GetStatus(game2, "no").Result.TimeLeft;
            Assert.AreEqual(0, left);
        }

        // Score 1 of completed game
        [TestMethod]
        public void TestStatus16()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 6).Wait();
            string game2 = JoinGame(player2, 6).Result;
            PlayWord(player1, game2, "XYZZY").Wait();
            PlayWord(player1, game2, "IS").Wait();
            string board = GetStatus(game2, "no").Result.Board;
            List<string> words = DifferentLengthWords(board);
            PlayWord(player1, game2, words[0]).Wait();
            PlayWord(player1, game2, words[1]).Wait();
            Thread.Sleep(7000);
            int score1 = GetStatus(game2, "no").Result.Player1.Score;
            Assert.AreEqual(-1 + GetScore(words[0]) + GetScore(words[1]), score1);
        }

        // Score 2 of completed game
        [TestMethod]
        public void TestStatus17()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 6).Wait();
            string game2 = JoinGame(player2, 6).Result;
            PlayWord(player2, game2, "XYZZY").Wait();
            PlayWord(player2, game2, "IS").Wait();
            string board = GetStatus(game2, "no").Result.Board;
            List<string> words = DifferentLengthWords(board);
            PlayWord(player2, game2, words[0]).Wait();
            PlayWord(player2, game2, words[1]).Wait();
            Thread.Sleep(7000);
            int score1 = GetStatus(game2, "no").Result.Player2.Score;
            Assert.AreEqual(-1 + GetScore(words[0]) + GetScore(words[1]), score1);
        }

        // Words 1 of completed game
        [TestMethod]
        public void TestStatus18()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 6).Wait();
            string game2 = JoinGame(player2, 6).Result;
            PlayWord(player1, game2, "XYZZY").Wait();
            PlayWord(player1, game2, "IS").Wait();
            string board = GetStatus(game2, "no").Result.Board;
            List<string> allWords = DifferentLengthWords(board);
            foreach (string w in DifferentLengthWords(board))
            {
                PlayWord(player1, game2, w).Wait();
            }
            Thread.Sleep(7000);
            List<dynamic> wordscores = new List<dynamic>(GetStatus(game2, "no").Result.Player1.WordsPlayed);
            wordscores.Sort((x, y) => x.Word.CompareTo(y.Word));
            allWords.Add("XYZZY");
            allWords.Add("IS");
            allWords.Sort();
            for (int i = 0; i < allWords.Count; i++)
            {
                Assert.AreEqual(allWords[i].ToUpper(), wordscores[i].Word.ToString().ToUpper());
            }
        }

        // Words 2 of completed game
        [TestMethod]
        public void TestStatus19()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 6).Wait();
            string game2 = JoinGame(player2, 6).Result;
            PlayWord(player2, game2, "XYZZY").Wait();
            PlayWord(player2, game2, "IS").Wait();
            string board = GetStatus(game2, "no").Result.Board;
            List<string> allWords = DifferentLengthWords(board);
            foreach (string w in DifferentLengthWords(board))
            {
                PlayWord(player2, game2, w).Wait();
            }
            Thread.Sleep(7000);
            List<dynamic> wordscores = new List<dynamic>(GetStatus(game2, "no").Result.Player2.WordsPlayed);
            wordscores.Sort((x, y) => x.Word.CompareTo(y.Word));
            allWords.Add("XYZZY");
            allWords.Add("IS");
            allWords.Sort();
            for (int i = 0; i < allWords.Count; i++)
            {
                Assert.AreEqual(allWords[i].ToUpper(), wordscores[i].Word.ToString().ToUpper());
            }
        }

        // Scores 1 of completed game
        [TestMethod]
        public void TestStatus20()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 6).Wait();
            string game2 = JoinGame(player2, 6).Result;
            PlayWord(player1, game2, "XYZZY").Wait();
            PlayWord(player1, game2, "IS").Wait();
            string board = GetStatus(game2, "no").Result.Board;
            List<string> allWords = DifferentLengthWords(board);
            foreach (string w in DifferentLengthWords(board))
            {
                PlayWord(player1, game2, w).Wait();
            }
            Thread.Sleep(7000);
            List<dynamic> wordscores = new List<dynamic>(GetStatus(game2, "no").Result.Player1.WordsPlayed);
            wordscores.Sort((x, y) => x.Word.CompareTo(y.Word));
            allWords.Add("XYZZY");
            allWords.Add("IS");
            allWords.Sort();
            for (int i = 0; i < allWords.Count; i++)
            {
                Assert.AreEqual(GetScore(allWords[i]), (int)wordscores[i].Score);
            }
        }

        // Scores 2 of completed game
        [TestMethod]
        public void TestStatus21()
        {
            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 6).Wait();
            string game2 = JoinGame(player2, 6).Result;
            PlayWord(player2, game2, "XYZZY").Wait();
            PlayWord(player2, game2, "IS").Wait();
            string board = GetStatus(game2, "no").Result.Board;
            List<string> allWords = DifferentLengthWords(board);
            foreach (string w in DifferentLengthWords(board))
            {
                PlayWord(player2, game2, w).Wait();
            }
            Thread.Sleep(7000);
            List<dynamic> wordscores = new List<dynamic>(GetStatus(game2, "no").Result.Player2.WordsPlayed);
            wordscores.Sort((x, y) => x.Word.CompareTo(y.Word));
            allWords.Add("XYZZY");
            allWords.Add("IS");
            allWords.Sort();
            for (int i = 0; i < allWords.Count; i++)
            {
                Assert.AreEqual(GetScore(allWords[i]), (int)wordscores[i].Score);
            }
        }

        /// <summary>
        /// Test game timing
        /// </summary>
        [TestMethod]
        public void TestStatus22()
        {
            String player1 = MakeUser("Player 1", Created).Result;
            String player2 = MakeUser("Player 2", Created).Result;
            JoinGame(player1, 10, Accepted).Wait();
            String game2 = JoinGame(player2, 10, Created).Result;

            for (int time = 10; time > 0; time--)
            {
                int left = GetStatus(game2, "no").Result.TimeLeft;
                Assert.IsTrue(left <= time + 1 && left >= time - 1 && left <= 10 && left >= 0);
                Thread.Sleep(1000);
            }

            Thread.Sleep(1000);
            int timeLeft = GetStatus(game2, "no").Result.TimeLeft;
            Assert.AreEqual(0, timeLeft);
        }



        // Simulates a full game
        public string SimulateGame(out List<string> p1Words, out List<string> p2Words, AutoResetEvent resetEvent)
        {
            p1Words = new List<string>();
            p2Words = new List<string>();

            Reset();
            string player1 = MakeUser("Player 1").Result;
            string player2 = MakeUser("Player 2").Result;
            JoinGame(player1, 30).Wait();
            string game2 = JoinGame(player2, 30).Result;

            resetEvent.Set();

            string board = GetStatus(game2, "no").Result.Board;
            List<string> allWords = AllValidWords(board);

            DateTime start = DateTime.Now;

            int count = 0;
            foreach (string w in allWords)
            {
                if (count > 30)
                {
                    break;
                }
                else if (count % 2 == 0)
                {
                    p1Words.Add(w);
                    PlayWord(player1, game2, w).Wait();
                }
                else
                {
                    p2Words.Add(w);
                    PlayWord(player2, game2, w).Wait();
                }
                count++;
            }

            
            do
            {
                Thread.Sleep(1000);
            }
            while (DateTime.Now.Subtract(start).TotalSeconds < 32);

            return game2;
        }


        // Play one game with correct scores
        public void Play(AutoResetEvent resetEvent)
        {
            List<string> p1Words, p2Words;
            string game = SimulateGame(out p1Words, out p2Words, resetEvent);
            string board = GetStatus(game, "no").Result.Board;
            List<dynamic> wordscores1 = new List<dynamic>(GetStatus(game, "no").Result.Player1.WordsPlayed);
            List<dynamic> wordscores2 = new List<dynamic>(GetStatus(game, "no").Result.Player2.WordsPlayed);           
            wordscores1.Sort((x, y) => x.Word.CompareTo(y.Word));
            wordscores1.Sort((x, y) => x.Word.CompareTo(y.Word));
            p1Words.Sort();
            p2Words.Sort();
            Assert.AreEqual(p1Words.Count, wordscores1.Count);
            Assert.AreEqual(p2Words.Count, wordscores2.Count);

            int total1 = 0;
            for (int i = 0; i < p1Words.Count; i++)
            {
                Assert.AreEqual(p1Words[i].ToUpper(), wordscores1[i].Word.ToString().ToUpper());
                Assert.AreEqual(GetScore(p1Words[i]), (int)wordscores1[i].Score);
                total1 += GetScore(p1Words[i]);
            }

            int total2 = 0;
            for (int i = 0; i < p2Words.Count; i++)
            {
                Assert.AreEqual(p2Words[i].ToUpper(), wordscores2[i].Word.ToString().ToUpper());
                Assert.AreEqual(GetScore(p2Words[i]), (int)wordscores2[i].Score);
                total2 += GetScore(p2Words[i]);
            }

            int score1 = GetStatus(game, "no").Result.Player1.Score;
            int score2 = GetStatus(game, "no").Result.Player2.Score;
            Assert.AreEqual(total1, score1);
            Assert.AreEqual(total2, score2);
        }


        // Play one game with correct scores
        [TestMethod]
        public void PlayOne ()
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            Play(resetEvent);
        }

        // Play five simultaneous games with correct scores
        [TestMethod]
        public void PlayFive()
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(Task.Run(() => Play(resetEvent)));
                resetEvent.WaitOne();
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Gets the status and asserts that it is as described in the parameters.
        /// </summary>
        private void CheckStatus(string game, string state, string brief, string p1, string p2, string n1, string n2, string b,
                                 List<string> w1, List<string> w2, List<int> s1, List<int> s2, int timeLimit)
        {
            dynamic status = GetStatus(game, brief, OK).Result;
            Assert.AreEqual(state, (string)status.GameState);

            if (state == "pending")
            {
                Assert.IsNull(status.TimeLimit);
                Assert.IsNull(status.TimeLeft);
                Assert.IsNull(status.Board);
                Assert.IsNull(status.Player1);
                Assert.IsNull(status.Player2);
            }
            else if (brief == "yes")
            {
                Assert.IsNull(status.TimeLimit);
                Assert.IsNull(status.Board);
                Assert.IsNull(status.Player1.WordsPlayed);
                Assert.IsNull(status.Player1.Nickname);
                Assert.IsNull(status.Player2.WordsPlayed);
                Assert.IsNull(status.Player2.Nickname);
            }
            else if (state == "active")
            {
                Assert.IsNull(status.Player1.WordsPlayed);
                Assert.IsNull(status.Player2.WordsPlayed);
            }

            if (state == "active" || state == "completed")
            {
                Assert.IsTrue((int)status.TimeLeft <= timeLimit);
                if (state == "active")
                {
                    Assert.IsTrue((int)status.TimeLeft > 0);
                }
                else
                {
                    Assert.IsTrue((int)status.TimeLeft >= 0);
                }

                int total1 = 0;
                for (int i = 0; i < s1.Count; i++)
                {
                    total1 += s1[i];
                }
                Assert.AreEqual(total1, (int)status.Player1.Score);

                int total2 = 0;
                for (int i = 0; i < s2.Count; i++)
                {
                    total2 += s2[i];
                }
                Assert.AreEqual(total2, (int)status.Player2.Score);

                if (brief != "yes")
                {
                    Assert.AreEqual(b, (string)status.Board);
                    Assert.AreEqual(timeLimit, (int)status.TimeLimit);
                    Assert.AreEqual(n1, (string)status.Player1.Nickname);
                    Assert.AreEqual(n2, (string)status.Player2.Nickname);

                    if (state == "completed")
                    {
                        List<dynamic> words1 = new List<dynamic>(status.Player1.WordsPlayed);
                        List<dynamic> words2 = new List<dynamic>(status.Player2.WordsPlayed);
                        Assert.AreEqual(w1.Count, words1.Count);
                        Assert.AreEqual(w2.Count, words2.Count);

                        for (int i = 0; i < w1.Count; i++)
                        {
                            Assert.AreEqual(w1[i], (string)words1[i].Word);
                            Assert.AreEqual(s1[i], (int)words1[i].Score);
                        }

                        for (int i = 0; i < w2.Count; i++)
                        {
                            Assert.AreEqual(w2[i], (string)words2[i].Word);
                            Assert.AreEqual(s2[i], (int)words2[i].Score);
                        }
                    }
                }
            }
        }
    }
}
