using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace TongJi.Web.Notifications
{
    public static class Notification
    {
        public static int MaxHistory = 20;
        private static Dictionary<string, DateTime> _lastGenerateMessageTime = new Dictionary<string, DateTime>();
        private static Dictionary<string, Queue<string>> _messageQueues = new Dictionary<string, Queue<string>>();
        private static Dictionary<string, Queue<HistoryMessage>> _messageHistoryQueues = new Dictionary<string, Queue<HistoryMessage>>();

        public static TimeSpan MessageGenerateInterval = TimeSpan.FromMinutes(1);
        public static event Action MessageGenerate;
        public static void OnMessageGenerate()
        {
            if (MessageGenerate != null)
            {
                MessageGenerate();
                _lastGenerateMessageTime[WebMatrix.WebData.WebSecurity.CurrentUserName] = DateTime.Now;
            }
        }

        public static void TryGenerateMessage()
        {
            if (!_lastGenerateMessageTime.ContainsKey(WebMatrix.WebData.WebSecurity.CurrentUserName))
            {
                OnMessageGenerate();
            }
            else
            {
                DateTime lastTime = _lastGenerateMessageTime[WebMatrix.WebData.WebSecurity.CurrentUserName];
                if (DateTime.Now - lastTime > MessageGenerateInterval)
                {
                    OnMessageGenerate();
                }
            }
        }

        //static Notification()
        //{
        //    HttpContext.Current.Cache["MessageQueues"] = new Dictionary<string, Queue<string>>();
        //    HttpContext.Current.Cache["MessageHistoryQueues"] = new Dictionary<string, Queue<HistoryMessage>>();
        //}

        public static Queue<string> GetMessageQueue(string username)
        {
            var queues = _messageQueues; //HttpContext.Current.Cache["MessageQueues"] as Dictionary<string, Queue<string>>;
            if (!queues.ContainsKey(username))
            {
                queues[username] = new Queue<string>();
            }
            return queues[username];
        }

        public static Queue<HistoryMessage> GetMessageHistoryQueue(string username)
        {
            var queues = _messageHistoryQueues; //HttpContext.Current.Cache["MessageHistoryQueues"] as Dictionary<string, Queue<HistoryMessage>>;
            if (!queues.ContainsKey(username))
            {
                queues[username] = new Queue<HistoryMessage>();
            }
            return queues[username];
        }

        public static void EnqueueMessage(string message) // newly 20130428
        {
            EnqueueMessage(WebMatrix.WebData.WebSecurity.CurrentUserName, message);
        }

        public static void EnqueueMessage(string username, string message)
        {
            GetMessageQueue(username).Enqueue(message);
            PushHistory(username, message);
        }

        private static void PushHistory(string username, string message)
        {
            var queue = GetMessageHistoryQueue(username);
            queue.Enqueue(new HistoryMessage { time = DateTime.Now, message = message });
            if (queue.Count > MaxHistory)
            {
                queue.Dequeue();
            }
        }

        public static string DequeueMessage() // newly 20130428
        {
            return DequeueMessage(WebMatrix.WebData.WebSecurity.CurrentUserName);
        }

        public static string DequeueMessage(string username)
        {
            var queue = GetMessageQueue(username);
            if (queue.Count > 0)
            {
                return GetMessageQueue(username).Dequeue();
            }
            else
            {
                return string.Empty;
            }
        }

        public static string[] DequeueAllMessages(string username)
        {
            var queue = GetMessageQueue(username);
            return Enumerable.Range(0, queue.Count).Select(i => queue.Dequeue()).ToArray();
        }
    }

    public class HistoryMessage
    {
        public DateTime time { get; set; }
        public string message { get; set; }
    }
}
