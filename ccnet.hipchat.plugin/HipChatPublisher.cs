using System.Collections.Specialized;
using System.Net;
using System.Text;

using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;

namespace ccnet.hipchat.plugin
{
    [ReflectorType("hipchat")]
    public class HipChatPublisher : ITask
    {
        [ReflectorProperty("https")]
        public bool IsHttps { get; set; }

        [ReflectorProperty("auth-token")]
        public string AuthToken { get; set; }

        [ReflectorProperty("room-id")]
        public string RoomId { get; set; }

        [ReflectorProperty("from")]
        public string From { get; set; }

        public void Run(IIntegrationResult result)
        {
            var message = string.Format("[CCNET] - {0} build complete. Result: {1}", result.ProjectName, result.Status);
            var url = string.Format("http{0}://api.hipchat.com/v1/rooms/message/?auth_token={1}", IsHttps ? "s" : "", AuthToken);

            var data = new NameValueCollection {
                { "room_id", RoomId },
                { "from", From },
                { "message", message },
                { "color", result.Succeeded ? "green" : "red" },
                { "notify", result.Succeeded ? "0" : "1" }
            };

            var client = new WebClient();
            client.UploadValues(url, "POST", data);
        }
    }
}
