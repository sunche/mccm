using System;
using System.Collections.Generic;

namespace Mccm.NotificationService.Common
{
    /// <summary>
    /// Base notification.
    /// </summary>
    public abstract class NotificationBase
    {
        /// <summary>
        /// Values.
        /// </summary>
        protected readonly Dictionary<string, object> BodyValues;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">key.</param>
        /// <param name="value">value.</param>
        protected NotificationBase(string key, object value)
        {
            BodyValues = new Dictionary<string, object> { { key, value } };
        }

        /// <summary>
        /// Add record to notification.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">value.</param>
        public void AddValue(string key, object value)
        {
            BodyValues.Add(key, value);
        }
    }
}
