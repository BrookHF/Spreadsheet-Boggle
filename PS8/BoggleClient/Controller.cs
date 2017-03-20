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
        public dynamic userToken { get; private set; }

        public Controller(RegisterForm registerWindow)
        {
            this.registerWindow = registerWindow;
            this.model = new HttpClient();

            registerWindow.LoginEvent += HandleLogin;
            registerWindow.CancelRegisterEvent += HandleCancelRegister;

            //playWindow.LoadPlayFormEvent += HandleLoadPlayForm;
            //playWindow.EnterEvent += HandleEnter;
            //playWindow.LeaveEvent += HandleLeave;
            //playWindow.HelpGameRulesEvent += HandleHelpGameRules;
            //playWindow.HelpHowToPlayEvent += HandleHelpHowToPlay;

            //gameTimeWindow.PlayEvent += HandlePlay;
            //gameTimeWindow.CancelJoinEvent += HandleCancelJoin;
        }

        private void HandleCancelJoin()
        {
            throw new NotImplementedException();
        }

        public Controller(PlayForm playWindow)
        {
            this.playWindow = playWindow;
            this.model = new HttpClient();
            
            
        }
        public Controller(GameTimeForm gameTimeWindow)
        {
            this.gameTimeWindow = gameTimeWindow;
            this.model = new HttpClient();

            
        }

        private void HandlePlay(string obj)
        {
            throw new NotImplementedException();
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
            throw new TaskCanceledException();
        }

        private async void HandleLogin(string domain, string nickname)
        {
            try
            {
                
                registerWindow.EnableControls(false);
                using (HttpClient client = CreateClient(domain))
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

                        gameTimeWindow = new GameTimeForm();
                        gameTimeWindow.Show();
                        registerWindow.Hide();
                        //view.UserRegistered = true;
                    }
                    else
                    {
                        MessageBox.Show("Login not successful!");
                    }
                }
            }
            catch (TaskCanceledException e)
            {
            }
            finally
            {
                registerWindow.EnableControls(true);
            }

            
        }
        private HttpClient CreateClient(string domain)
        {
            // Create a client whose base address is the GitHub server
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(domain);

            // Tell the server that the client will accept this particular type of response data
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");


            // There is more client configuration to do, depending on the request.
            return client;
        }
    }
}
