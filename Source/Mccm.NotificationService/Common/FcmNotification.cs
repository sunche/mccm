using System;
using System.Text;
using Newtonsoft.Json;

namespace Mccm.NotificationService.Common
{
    /// <summary>
    /// FireBase notification.
    /// </summary>
    public class FcmNotification : NotificationBase
    {
        private readonly string title;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public FcmNotification(string key, object value) : base(key, value)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public FcmNotification(string title, string key, object value) : base(key, value)
        {
            this.title = title;
        }

        /// <summary>
        /// Get payload of notification.
        /// </summary>
        /// <returns></returns>
        public string GetPayload(string target, bool isTargetTopic, bool isTesting)
        {
            var payload = new StringBuilder();
            payload.Append($"{{\"message\":{{\"notification\":{GetContent()},");

            if (isTargetTopic)
            {
                payload.Append("\"topic\":");
            }
            else
            {
                payload.Append("\"token\":");
            }

            payload.Append($"\"{target}\"}}");

            if (isTesting)
            {
                payload.Append(",\"validate_only\":true");
            }

            payload.Append("}");
            return payload.ToString();
        }

        private string GetContent()
        {
            var notification = new StringBuilder();
            notification.Append("{\"body\":\"" +
                $"{JsonConvert.SerializeObject(BodyValues, Formatting.None).Replace("\"", "\\\"")}" +
                "\"");

            if (!string.IsNullOrEmpty(title))
            {
                notification.Append($",\"title\":\"{title}\"");
            }

            notification.Append("}");
            return notification.ToString();
        }
    }
}
