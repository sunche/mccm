using System;
using System.Threading.Tasks;
using Mccm.NotificationService.Common;

namespace Mccm.NotificationService.Services
{
    /// <summary>
    /// Broker for firebase cloud messaging(google/android).
    /// </summary>
    public interface IFcmBroker: IDisposable
    {
        /// <summary>
        /// Create new notification.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <returns><see cref="FcmNotification"/>.</returns>
        FcmNotification CreateNotification(string key, object value);

        /// <summary>
        /// Create new notification.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <returns><see cref="FcmNotification"/>.</returns>
        FcmNotification CreateNotification(string title, string key, object value);

        /// <summary>
        /// Register device to topic.
        /// </summary>
        /// <param name="deviceToken">Device token.</param>
        /// <param name="topic">Topic.</param>
        /// <param name="fcmServerKey">Firebase server key.</param>
        /// <returns></returns>
        Task<NotificationResult> RegisterToTopic(string deviceToken, string topic, string fcmServerKey);

        /// <summary>
        /// Send notification.
        /// </summary>
        /// <param name="notification">Notification.</param>
        /// <param name="deviceToken">Device token.</param>
        /// <returns></returns>
        Task<NotificationResult> Send(FcmNotification notification, string deviceToken);

        /// <summary>
        /// Send notification.
        /// </summary>
        /// <param name="notification">Notification.</param>
        /// <param name="topic">Topic.</param>
        /// <returns></returns>
        Task<NotificationResult> SendToTopic(FcmNotification notification, string topic);
    }
}
