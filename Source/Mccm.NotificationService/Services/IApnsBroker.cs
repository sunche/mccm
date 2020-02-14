using System;
using System.Threading.Tasks;
using Mccm.NotificationService.Common;

namespace Mccm.NotificationService.Services
{
    /// <summary>
    /// Interface apns broker(Apple service).
    /// </summary>
    public interface IApnsBroker: IDisposable
    {
        /// <summary>
        /// Create notification.
        /// </summary>
        /// <param name="key">Notification key.</param>
        /// <param name="value">Notification value.</param>
        /// <returns>Notification.</returns>
        ApnsNotification CreateNotification(string key, string value);

        /// <summary>
        /// Send notification.
        /// </summary>
        /// <param name="notification">Notification.</param>
        /// <param name="deviceToken">Device token.</param>
        /// <param name="isVoip">Notification is Voip flag.</param>
        /// <returns>Result.</returns>
        Task<NotificationResult> Send(ApnsNotification notification, string deviceToken, bool isVoip);
    }
}
