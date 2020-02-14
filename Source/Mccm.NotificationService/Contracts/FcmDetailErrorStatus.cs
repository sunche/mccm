using System;
using System.Runtime.Serialization;

namespace Mccm.NotificationService.Contracts
{
    /// <summary>
    /// https://firebase.google.com/docs/reference/fcm/rest/v1/ErrorCode
    /// </summary>
    [DataContract]
    public enum FcmDetailErrorStatus
    {
        /// <summary>
        /// No more information is available about this error.
        /// </summary>
        [EnumMember(Value = "UNSPECIFIED_ERROR")]
        UnspecifiedError,

        /// <summary>
        /// Request parameters were invalid. An extension of type google.rpc.BadRequest
        /// is returned to specify which field was invalid.
        /// </summary>
        [EnumMember(Value = "INVALID_ARGUMENT")]
        InvalidArgument,

        /// <summary>
        /// App instance was unregistered from FCM. This usually means that the token
        /// used is no longer valid and a new one must be used.
        /// </summary>
        [EnumMember(Value = "UNREGISTERED")]
        Unregistered,

        /// <summary>
        /// The authenticated sender ID is different from the sender ID for the registration token.
        /// </summary>
        [EnumMember(Value = "SENDER_ID_MISMATCH")]
        SenderIdMismath,

        /// <summary>
        /// Sending limit exceeded for the message target. An extension of type
        /// google.rpc.QuotaFailure is returned to specify which quota got exceeded.
        /// </summary>
        [EnumMember(Value = "QUOTA_EXCEEDED")]
        QuotaExceeded,

        /// <summary>
        /// APNs certificate or auth key was invalid or missing.
        /// </summary>
        [EnumMember(Value = "APNS_AUTH_ERROR")]
        ApnsAuthError,

        /// <summary>
        /// The server is overloaded.
        /// </summary>
        [EnumMember(Value = "UNAVAILABLE")]
        Unavailable,

        /// <summary>
        /// An unknown internal error occurred.
        /// </summary>
        [EnumMember(Value = "INTERNAL")]
        Internal
    }
}
