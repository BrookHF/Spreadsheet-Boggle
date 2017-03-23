using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoggleClient
{
    public class Controller
    {
        private RegisterForm registerWindow;
        private PlayForm playWindow;
        private GameTimeForm gameTimeWindow;
        private HttpClient model;

        public CancellationTokenSource tokenSource { get; private set; }
        private dynamic userToken;
        public bool registerWindowIsHidden { get; private set; }
        public dynamic gameID { get; private set; }
        public dynamic gameState { get; private set; }
        public bool gameStarted { get; private set; }

        private System.Windows.Forms.Timer timer;
        private string domain;

        public Controller(RegisterForm registerWindow)
        {
            this.registerWindow = registerWindow;
            this.playWindow = new PlayForm();
            //playWindow.Show();
            this.gameTimeWindow = new GameTimeForm();
            this.model = new HttpClient();
            this.timer = new System.Windows.Forms.Timer();

            timer.Tick += HandleUpdate;

            registerWindow.LoginEvent += HandleLogin;
            registerWindow.CancelRegisterEvent += HandleCancelRegister;
            registerWindow.HelpGameRulesEvent += HandleHelpGameRules;
            registerWindow.HelpHowToPlayEvent += HandleHelpHowToPlay;
            registerWindow.HelpHowToRegisterEvent += HandleHelpHowToRegister;

            playWindow.EnterEvent += HandleEnter;
            playWindow.LeaveEvent += HandleLeave;
            playWindow.PlayFormClosingEvent += HandleFormClosing;

            gameTimeWindow.PlayEvent += HandlePlay;
            gameTimeWindow.CancelJoinEvent += HandleCancelJoin;
            gameTimeWindow.GameTimeFormClosingEvent += HandleFormClosing;
        }

        private void HandleFormClosing()
        {
            registerWindow.Close();
        }

        private async void HandleCancelJoin()
        {
            using (HttpClient client = CreateClient())
            {
                // Create the parameter
                dynamic games = new ExpandoObject();
                games.UserToken = userToken;

                // Compose and send the request.
                tokenSource = new CancellationTokenSource();
                StringContent content = new StringContent(JsonConvert.SerializeObject(games), Encoding.UTF8, @"application/json");
                HttpResponseMessage response = await client.PostAsync("games", content, tokenSource.Token);
            }
            gameTimeWindow.SearchingMessageVisible(false);
            timer.Stop();
            gameTimeWindow.Hide();
            registerWindow.Show();
        }

        private async void HandlePlay(string gameTime)
        {
            int time = 0;
            if (!int.TryParse(gameTime, out time) || (time < 5 || time > 120))
            {
                MessageBox.Show("Must be integer between 5 to 120");
                return;
            }

            using (HttpClient client = CreateClient())
            {
                // Create the parameter
                dynamic games = new ExpandoObject();
                games.UserToken = userToken.UserToken;
                games.TimeLimit = time;

                // Compose and send the request.
                tokenSource = new CancellationTokenSource();
                StringContent content = new StringContent(JsonConvert.SerializeObject(games), Encoding.UTF8, @"application/json");
                HttpResponseMessage response = await client.PostAsync("games", content, tokenSource.Token);

                // Deal with the response
                if (response.IsSuccessStatusCode)
                {
                    String result = response.Content.ReadAsStringAsync().Result;
                    gameID = JsonConvert.DeserializeObject(result);
                    //playWindow.Show();
                    registerWindow.Hide();
                    registerWindowIsHidden = true;
                }
                else
                {
                    gameID = null;
                    MessageBox.Show("Join not successful!");
                }
                if(gameID != null)
                {
                    // Compose and send the request.
                    tokenSource = new CancellationTokenSource();
                    response = await client.GetAsync("games/" + gameID, tokenSource.Token);
                    if (response.IsSuccessStatusCode)
                    {
                        String result = response.Content.ReadAsStringAsync().Result;
                        gameState = (string)JsonConvert.DeserializeObject(result);
                    }
                    gameTimeWindow.SearchingMessageVisible(true);
                    timer.Start();
                }
                
            }
        }

        private async void HandleUpdate(object o, EventArgs e)
        {
            using (HttpClient client = CreateClient())
            {
                tokenSource = new CancellationTokenSource();
                HttpResponseMessage response;
                if (!gameStarted)
                {
                    response = await client.GetAsync("games/" + gameID.GameID, tokenSource.Token);
                }
                else
                {
                    response = await client.GetAsync("games/" + gameID.GameID+"?Brief=yes", tokenSource.Token);
                }
                    
                if (response.IsSuccessStatusCode)
                {
                    
                    String result = response.Content.ReadAsStringAsync().Result;
                    gameState = JsonConvert.DeserializeObject(result);
                    if (!gameStarted)
                    {
                        string temp = gameState.GameState;

                        if (temp.Equals("active"))
                        {
                            gameStarted = true;
                            playWindow.Show();
                            gameTimeWindow.SearchingMessageVisible(false);
                            gameTimeWindow.Hide();
                            playWindow.UpdateNamesAndBoard((string)gameState.Player1.Nickname, (string)gameState.Player2.Nickname, (string)gameState.Board);

                        }
                    }
                    else
                    {
                        playWindow.UpdateScoresAndTime((string)gameState.Player1.Score.ToString(), (string)gameState.Player2.Score.ToString(), (string)gameState.TimeLeft.ToString(), this.domain);
                        string temp = gameState.GameState;
                        if (temp.Equals("completed"))
                        {
                            timer.Stop();
                            gameStarted = false;
                            showResult();
                        }
                    }
                }
            }
        }

        private async void showResult()
        {
            using (HttpClient client = CreateClient())
            {
                tokenSource = new CancellationTokenSource();
                HttpResponseMessage response;
                response = await client.GetAsync("games/" + gameID.GameID, tokenSource.Token);
                if (response.IsSuccessStatusCode)
                {
                    String result = response.Content.ReadAsStringAsync().Result;
                    gameState = JsonConvert.DeserializeObject(result);
                    String player1Words = "Player 1: " + gameState.Player1.Nickname + " Score: " + gameState.Player1.Score + "\r" + " Words Played: \r";
                    String player2Words = "Player 2: " + gameState.Player2.Nickname + " Score: " + gameState.Player2.Score + "\r" + " Words Played: \r";

                    
                    foreach(dynamic d in gameState.Player1.WordsPlayed)
                    {
                        player1Words += d.Word + ": " + d.Score + "\r";
                    }
                    foreach (dynamic d in gameState.Player2.WordsPlayed)
                    {
                        player2Words += d.Word + ": " + d.Score + "\r";
                    }
                    MessageBox.Show(player1Words + player2Words);
                }
            }
        }
        private void HandleHelpHowToPlay()
        {
            MessageBox.Show("Once you've found a word, type it into the text box below the gameboard and timer, then press enter. ");
        }

        private void HandleHelpGameRules()
        {
            MessageBox.Show("Words can only be formed from adjoining letters. Letters must join in the proper sequence to spell a word. They may join horizontally, vertically, or diagonally, to the left, right, or up and down. No letter cube, however, may be used more than once within a single word. ");
        }

        private void HandleHelpHowToRegister()
        {
            MessageBox.Show("Enter a desired user name as well as the domain of the server you wish to connect to. Domain url can be formatted as either ending in /BoggleService.svc/ or domain name (For example: http://cs3500-boggle-s17.azurewebsites.net)");
        }

        private void HandleLeave()
        {
            registerWindow.Show();
            timer.Stop();
            playWindow.Hide();
        }

        private async void HandleEnter(string word)
        {
            using (HttpClient client = CreateClient())
            {
                dynamic play = new ExpandoObject();
                play.UserToken = userToken.UserToken;
                play.Word = word;

                // Compose and send the request.
                tokenSource = new CancellationTokenSource();
                StringContent content = new StringContent(JsonConvert.SerializeObject(play), Encoding.UTF8, @"application/json");

                tokenSource = new CancellationTokenSource();
                HttpResponseMessage response = await client.PutAsync("games/" + gameID.GameID, content,tokenSource.Token);

            }

        }

        private async void HandleLoadPlayForm()
        {
            using (HttpClient client = CreateClient())
            {
                tokenSource = new CancellationTokenSource();
                HttpResponseMessage response = await client.GetAsync("games/" + gameID.GameID, tokenSource.Token);
                if (response.IsSuccessStatusCode)
                {

                    String result = response.Content.ReadAsStringAsync().Result;
                    gameState = JsonConvert.DeserializeObject(result);
                    playWindow.UpdateNamesAndBoard(gameState.Player1.Nickname, gameState.Player2.Nickname, gameState.Board);

                }
            }
        }

        private void HandleCancelRegister()
        {
            tokenSource.Cancel();
            
        }

        private async void HandleLogin(string domain, string nickname)
        {
            this.domain = domain;
            //baseAddress = new Uri(domain);
            try
            {
                registerWindow.EnableControls(false);
                using (HttpClient client = CreateClient())
                {
                    // Create the parameter
                    dynamic user = new ExpandoObject();
                    user.Nickname = nickname;

                    // Compose and send the request.
                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, @"application/json");
                    HttpResponseMessage response = await client.PostAsync("users", content, tokenSource.Token);

                    // Deal with the response
                    if (response.IsSuccessStatusCode)
                    {
                        String result = response.Content.ReadAsStringAsync().Result;
                        userToken = JsonConvert.DeserializeObject(result);
                        gameTimeWindow.Show();
                        registerWindow.Hide();
                        registerWindowIsHidden = true;
                    }
                    else
                    {
                        MessageBox.Show("Login not successful.");
                    }
                }
            }
            catch
            {
            }
            finally
            {
                registerWindow.EnableControls(true);
            }
        }
        private HttpClient CreateClient()
        {
            // Create a client whose base address is the GitHub server
            HttpClient client = new HttpClient();
            try
            {
                if (domain.Length > 19 && domain.Substring(domain.Length - 19).Equals("/BoggleService.svc/"))
                {
                    client.BaseAddress = new Uri(this.domain);
                }
                else
                {
                    client.BaseAddress = new Uri(this.domain + "/BoggleService.svc/");
                }
            }
            catch
            {
                MessageBox.Show("Invalid domain format");
            }
            
            
            
            // Tell the server that the client will accept this particular type of response data
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");


            // There is more client configuration to do, depending on the request.
            return client;
        }
    }
}
