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

        private System.Timers.Timer timer;
        private Uri baseAddress;

        public Controller(RegisterForm registerWindow)
        {
            this.registerWindow = registerWindow;
            this.playWindow = new PlayForm();
            this.gameTimeWindow = new GameTimeForm();
            this.model = new HttpClient();
            this.timer = new System.Timers.Timer(1000);

            timer.Elapsed += async (sender, e) => await HandleUpdate();

            registerWindow.LoginEvent += HandleLogin;
            registerWindow.CancelRegisterEvent += HandleCancelRegister;

            //playWindow.EnterEvent += HandleEnter;
            //playWindow.LeaveEvent += HandleLeave;
            //playWindow.HelpGameRulesEvent += HandleHelpGameRules;

            gameTimeWindow.PlayEvent += HandlePlay;
            gameTimeWindow.CancelJoinEvent += HandleCancelJoin;
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
                    playWindow = new PlayForm();
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

        private async Task HandleUpdate()
        {
            using (HttpClient client = CreateClient())
            {
                tokenSource = new CancellationTokenSource();
                HttpResponseMessage response = await client.GetAsync("games/" + gameID.GameID, tokenSource.Token);
                if (response.IsSuccessStatusCode)
                {
                    
                    String result = response.Content.ReadAsStringAsync().Result;
                    gameState = JsonConvert.DeserializeObject(result);
                    string temp = gameState.GameState;

                    if ("active".Equals(temp))
                    {
                        timer.Stop();
                        
                        playWindow = new PlayForm();
                        playWindow.Show();
                        playWindow.UpdateNamesAndBoard((string)gameState.Player1.Nickname, (string)gameState.Player2.Nickname, (string)gameState.Board);
                    }
                    
                }
            }
        }

        private void HandleHelpHowToPlay()
        {
            throw new NotImplementedException();
        }

        private void HandleHelpGameRules()
        {
            throw new NotImplementedException();
        }

        private void HandleLeave()
        {
            throw new NotImplementedException();
        }

        private void HandleEnter(string obj)
        {
            throw new NotImplementedException();
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
                        MessageBox.Show("Login not successful!");
                    }
                }
            }
            catch (TaskCanceledException)
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
            client.BaseAddress = new Uri("http://cs3500-boggle-s17.azurewebsites.net/BoggleService.svc/"); 
            //client.BaseAddress = this.baseAddress;

            // Tell the server that the client will accept this particular type of response data
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");


            // There is more client configuration to do, depending on the request.
            return client;
        }
    }
}
