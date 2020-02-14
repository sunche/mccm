using System;
using System.Runtime.Serialization;

namespace Mccm.NotificationService.Contracts
{
    [DataContract]
    public class FcmResponse
    {
        [DataMember(Name = "error")]
        public FcmErrorInfo Error { get; set; }
    }
}
