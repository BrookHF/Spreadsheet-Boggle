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

        /// <summary>
        /// Creates a user based on username input
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/users")]
        Token CreateUser(User user);

        /// <summary>
        /// Adds a user to a game
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/games")]
        ID JoinGame(Game game);

        /// <summary>
        /// Cancels a user looking for a game
        /// </summary>
        /// <param name="token"></param>
        [WebInvoke(Method = "PUT", UriTemplate = "/games")]
        void CancelJoin(Token token);

        /// <summary>
        /// Plays a word for a user and gets points
        /// </summary>
        /// <param name="playWord"></param>
        /// <param name="GameID"></param>
        /// <returns></returns>
        [WebInvoke(Method = "PUT", UriTemplate = "/games/{GameID}")]
        ScoreChange PlayWord(PlayWord playWord, string GameID);

        /// <summary>
        /// Returns information about game status
        /// </summary>
        /// <param name="brief"></param>
        /// <param name="GameID"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/games/{GameID}?Brief={brief}")]
        GameStatusReturn GetGameStatus(string brief, string GameID);
    }
}
