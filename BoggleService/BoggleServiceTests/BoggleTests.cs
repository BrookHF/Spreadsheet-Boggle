using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.HttpStatusCode;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Dynamic;

namespace Boggle
{
    /// <summary>
    /// Provides a way to start and stop the IIS web server from within the test
    /// cases.  If something prevents the test cases from stopping the web server,
    /// subsequent tests may not work properly until the stray process is killed
    /// manually.
    /// </summary>
    public static class IISAgent
    {
        // Reference to the running process
        private static Process process = null;

        /// <summary>
        /// Starts IIS
        /// </summary>
        public static void Start(string arguments)
        {
            if (process == null)
            {
                ProcessStartInfo info = new ProcessStartInfo(Properties.Resources.IIS_EXECUTABLE, arguments);
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.UseShellExecute = false;
                process = Process.Start(info);
            }
        }

        /// <summary>
        ///  Stops IIS
        /// </summary>
        public static void Stop()
        {
            if (process != null)
            {
                process.Kill();
            }
        }
    }
    [TestClass]
    public class BoggleTests
    {
        /// <summary>
        /// This is automatically run prior to all the tests to start the server
        /// </summary>
        [ClassInitialize()]
        public static void StartIIS(TestContext testContext)
        {
            IISAgent.Start(@"/site:""BoggleService"" /apppool:""Clr4IntegratedAppPool"" /config:""..\..\..\.vs\config\applicationhost.config""");
        }

        /// <summary>
        /// This is automatically run when all tests have completed to stop the server
        /// </summary>
        [ClassCleanup()]
        public static void StopIIS()
        {
            IISAgent.Stop();
        }

        private RestTestClient client = new RestTestClient("http://localhost:60000/BoggleService.svc/");

        /// <summary>
        /// Note that DoGetAsync (and the other similar methods) returns a Response object, which contains
        /// the response Stats and the deserialized JSON response (if any).  See RestTestClient.cs
        /// for details.
        /// </summary>
        [TestMethod]
        public void TestMethodDemo()
        {
            Response r = client.DoGetAsync("word?index={0}", "-5").Result;
            Assert.AreEqual(Forbidden, r.Status);

            r = client.DoGetAsync("word?index={0}", "5").Result;
            Assert.AreEqual(OK, r.Status);

            string word = (string)r.Data;
            Assert.AreEqual("AAL", word);
        }

        /// <summary>
        /// Tests to make sure a valid nickname returns a Created Response on Create User.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);

            string word = (string)r.Data;
        }

        /// <summary>
        /// Tests to make sure a null nickname and an empty nickname return 403 forbidden on Create User.
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {
            dynamic user = new ExpandoObject();
            user.Nickname = "";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);

            string word = (string)r.Data;
        }

        /// <summary>
        /// Tests to make sure a valid nickname returns a UserToken on Create User.
        /// </summary>
        [TestMethod]
        public void TestMethod3()
        {
            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;

            string token = (string)r.Data;
            Assert.IsTrue(token.Length > 0);
        }

        /// <summary>
        /// Tests to make sure if UserToken is invalid, TimeLimit less than 5, or TimeLimit greater than 120, 
        /// responds with status 403 (Forbidden) on join game.
        /// </summary>
        [TestMethod]
        public void TestMethod4()
        {
            dynamic game = new ExpandoObject();
            game.UserToken = "9eb536af-50a1-476f-856e-ffff8f1b25d2";
            game.TimeLimit = 10;
            Response r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Forbidden, r.Status);

            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            r = client.DoPostAsync("users", user).Result;
            string token = (string)r.Data;

            string word = (string)r.Data;
            dynamic game2 = new ExpandoObject();
            game2.UserToken = token;
            game2.TimeLimit = 150;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Forbidden, r.Status);

        }

        /// <summary>
        /// Tests to make sure if UserToken is already a player in the pending game, responds with status 409 (Conflict) on join game.
        /// </summary>
        [TestMethod]
        public void TestMethod5()
        {

            dynamic user = new ExpandoObject();
            user.Nickname = "mj";
            Response r = client.DoPostAsync("users", user).Result;
            string token = (string)r.Data;

            string word = (string)r.Data;
            dynamic game = new ExpandoObject();
            game.UserToken = token;
            game.TimeLimit = 10;
            r = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Forbidden, r.Status);

        }
    }
}
