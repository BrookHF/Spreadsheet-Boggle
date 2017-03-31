using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Boggle
{
    [ServiceContract]
    public interface IBoggleService
    {
        /// <summary>
        /// Sends back index.html as the response body.
        /// </summary>
        [WebGet(UriTemplate = "/api")]
        Stream API();

        [WebInvoke(Method = "POST", UriTemplate = "/users")]
        Token CreateUser(User user);

        [WebInvoke(Method = "POST", UriTemplate = "/games")]
        ID JoinGame(Game game);

        [WebInvoke(Method = "PUT", UriTemplate = "/games")]
        void CancelJoin(Token token);

        [WebInvoke(Method = "PUT", UriTemplate = "/games/{GameID}")]
        ScoreChange PlayWord(PlayWord playWord, string GameID);

        [WebGet(UriTemplate = "/games/{GameID}?Brief={brief}")]
        GameStatusReturn GetGameStatus(string brief, string GameID);
    }
}
