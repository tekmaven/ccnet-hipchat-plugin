using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Exortech.NetReflector;
using HipChat;
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

        [ReflectorProperty("message", Required=false)]
        public string Message { get; set; }

        [ReflectorProperty("hide-result", Required = false)]
        public bool HideResult { get; set; }

        public void Run(IIntegrationResult result)
        {
            var displayDuration = false;
            var duration = TimeSpan.Zero;
            if (result.EndTime != DateTime.MinValue)
            {
                duration = result.EndTime - result.StartTime;
                displayDuration = true;
            }
            var link = String.Format(@"<a href=""{0}"">{1}</a>", result.ProjectUrl, result.Status);
            
            var message = new StringBuilder();
            message.Append(result.ProjectName);
            message.Append(" ");

            if(String.IsNullOrEmpty(Message))
            {
                message.Append("build complete");
            }
            else
            {
                message.Append(Message);
            }

            message.Append(" ");

            if(displayDuration)
            {
                message.AppendFormat("(duration {0})", duration);
            }

            message.Append(". ");

            if(!HideResult)
            {
                message.AppendFormat("Result: {0}.", link);
            }
            
            var notify = result.Succeeded;
            var color = result.Succeeded ? HipChatClient.BackgroundColor.green : HipChatClient.BackgroundColor.red;

            if(HideResult)
            {
                color = HipChatClient.BackgroundColor.yellow;
            }

            var client = new HipChatClient(AuthToken, RoomId, From);
            client.SendMessage(message.ToString(), color, notify);
        }
    }
}
