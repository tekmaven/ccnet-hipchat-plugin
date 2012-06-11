using System;
using System.Collections.Specialized;
using System.Net;

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

            if(String.IsNullOrEmpty(Message))
            {
                Message = "build complete";
            }

            if(displayDuration)
            {
                Message = String.Format("{0} (duration {1})", Message, duration);
            }

            if(!HideResult)
            {
                Message = String.Format("{0}. Result: {1}.", Message, link);
            }

            var message = string.Format("{0} {1}", result.ProjectName, Message);
            
            var notify = result.Succeeded;
            var color = result.Succeeded ? HipChatClient.BackgroundColor.green : HipChatClient.BackgroundColor.red;

            if(HideResult)
            {
                color = HipChatClient.BackgroundColor.yellow;
            }

            var client = new HipChatClient(AuthToken, RoomId, From);
            client.SendMessage(message, color, notify);
        }
    }
}
