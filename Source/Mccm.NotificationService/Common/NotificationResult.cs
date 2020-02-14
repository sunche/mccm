using System;

namespace Mccm.NotificationService.Common
{
    /// <summary>
    /// Notification sending result.
    /// </summary>
    public class NotificationResult
    {
        /// <summary>
        /// Error.
        /// </summary>
        public NotificationError Error { get; private set; }

        /// <summary>
        /// Is success flag.
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Get successful response.
        /// </summary>
        /// <returns></returns>
        public static NotificationResult Successful()
        {
            return new NotificationResult { Success = true };
        }

        /// <summary>
        /// Get response with error.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static NotificationResult WithError(NotificationErrorCode errorCode, string message)
        {
            return new NotificationResult { Success = false, Error = new NotificationError(errorCode, message) };
        }
    }
}
