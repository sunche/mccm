using System;
using System.Runtime.Serialization;

namespace Mccm.NotificationService.Contracts
{
    [DataContract]
    public class FcmErrorInfo
    {
        [DataMember(Name = "status")]
        public FcmErrorStatus ErrorStatus { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}
