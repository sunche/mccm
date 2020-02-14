using System;
using Newtonsoft.Json;

namespace Mccm.NotificationService.Common
{
    /// <summary>
    /// Apns notification.
    /// </summary>
    public class ApnsNotification : NotificationBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public ApnsNotification(string key, object value) : base(key, value)
        {
        }

        /// <summary>
        /// Get payload of notification.
        /// </summary>
        /// <returns></returns>
        public string GetPayload()
        {
            return $"{{\"aps\":{JsonConvert.SerializeObject(BodyValues, Formatting.None)}}}";
        }
    }
}
