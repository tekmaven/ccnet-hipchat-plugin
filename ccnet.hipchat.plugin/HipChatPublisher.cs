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
            var duration = result.EndTime - result.StartTime;
            var buildTime = string.Format("{0}:{1}.{2}", duration.Minutes, duration.Seconds, duration.Milliseconds);

            var logpath = LogFileUtil.CreateUrl(result);
            var link = string.Format(@"<a href=""{0}"">{1}</a>", LogFileUtil.CreateUrl(result), result.Status);

            var message = string.Format("{0} build complete (duration {1}). Result: {2}", result.ProjectName, buildTime, link);

            var data = new NameValueCollection {
                { "room_id", RoomId },
                { "from", From },
                { "message", message },
                { "color", result.Succeeded ? "green" : "red" },
                { "notify", result.Succeeded ? "0" : "1" }
            };

            var client = new WebClient();
            var url = string.Format("http{0}://api.hipchat.com/v1/rooms/message/?auth_token={1}", IsHttps ? "s" : "", AuthToken);
            client.UploadValues(url, "POST", data);
        }
    }
}
