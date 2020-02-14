using System;

namespace Mccm.NotificationService.Common
{
    /// <summary>
    /// Contain information about notification error.
    /// </summary>
    public class NotificationError
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="code">Error code.</param>
        /// <param name="message">Message.</param>
        public NotificationError(NotificationErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Error code.
        /// </summary>
        public NotificationErrorCode Code { get; }

        /// <summary>
        /// Message.
        /// </summary>
        public string Message { get; }
    }
}
