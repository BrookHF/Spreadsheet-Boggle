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
        public object gameID { get; private set; }
        public string gameState { get; private set; }

        private Uri baseAddress;

        public Controller(RegisterForm registerWindow)
        {
            this.registerWindow = registerWindow;
            this.playWindow = new PlayForm();
            this.gameTimeWindow = new GameTimeForm();
            this.model = new HttpClient();

            registerWindow.LoginEvent += HandleLogin;
            registerWindow.CancelRegisterEvent += HandleCancelRegister;

            //playWindow.LoadPlayFormEvent += HandleLoadPlayForm;
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
        }

        private async void HandlePlay(string gameTime)
        {
            int time = 0;
            if (!int.TryParse(gameTime, out time) || (time < 5 || time > 120))
            {
                MessageBox.Show("Must be integer between 5 to 120");
                return;
            }
            registerWindow.EnableControls(false);
            using (HttpClient client = CreateClient())
            {
                // Create the parameter
                dynamic games = new ExpandoObject();
                games.UserToken = userToken.ToString();
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
                    MessageBox.Show("game ID:" + gameID);
                    playWindow = new PlayForm();
                    registerWindow.Hide();
                    registerWindowIsHidden = true;
                    //view.UserRegistered = true;
                }
                else
                {
                    MessageBox.Show("Login not successful!");
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
                }
                while (gameState == "pending")
                {
                    tokenSource = new CancellationTokenSource();
                    response = await client.GetAsync("games/" + gameID, tokenSource.Token);
                    if (response.IsSuccessStatusCode)
                    {
                        String result = response.Content.ReadAsStringAsync().Result;
                        gameState = (string)JsonConvert.DeserializeObject(result);
                    }
                    Thread.Sleep(500);
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

        private void HandleLoadPlayForm()
        {
            throw new NotImplementedException();
        }

        private void HandleCancelRegister()
        {
            if(!registerWindowIsHidden)
            {
                throw new TaskCanceledException();
            }
            
        }

        private async void HandleLogin(string domain, string nickname)
        {
            baseAddress = new Uri(domain);
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
                        //view.UserRegistered = true;
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
            //client.BaseAddress = new Uri("http://cs3500-boggle-s17.azurewebsites.net/BoggleService.svc/"); 
            client.BaseAddress = this.baseAddress;

            // Tell the server that the client will accept this particular type of response data
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");


            // There is more client configuration to do, depending on the request.
            return client;
        }
    }
}
