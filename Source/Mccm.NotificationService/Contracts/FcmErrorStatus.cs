using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mccm.NotificationService.Contracts
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FcmErrorStatus
    {
        [EnumMember(Value = "INVALID_ARGUMENT")]
        InvalidArgument,

        [EnumMember(Value = "UNAUTHENTICATED")]
        Unauthenticated,

        [EnumMember(Value = "NOT_FOUND")]
        NotFound
    }
}
