using System;
using System.Runtime.Serialization;

namespace Mccm.NotificationService.Contracts
{
    [DataContract]
    public class ApnsError
    {
        [DataMember(Name = "reason")]
        public string Reason { get; set; }
    }
}
