using System;

namespace Mccm.NotificationService.Common
{
    /// <summary>
    /// Notification error codes.
    /// </summary>
    public enum NotificationErrorCode
    {
        InvalidDeviceToken,
        AccessDenied,
        ExpiredAccessToken,
        BadRequest,
        Unspecified
    }
}
