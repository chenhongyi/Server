using System;
using System.Runtime.Serialization;

namespace Model.Data.Account
{
    [DataContract]
    public class Passport
    {
        [DataMember] public string PassportID { get; set; }

        [DataMember] public Guid AccountID { get; set; }

        [DataMember] public string IMEI { get; set; }
    }
}
