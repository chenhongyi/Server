using System;
using System.Runtime.Serialization;

namespace Model.Data.Account
{
    [DataContract]
    public class Token
    {
        public Token()
        {
            SetTime();
        }
        [DataMember] public Guid TokenID { get; set; }
        [DataMember] public string UserPassport { get; set; }    //用户账号
        [DataMember] public string SignToken { get; set; }     //登录凭据
        [DataMember] public DateTime ExpireTime { get; set; }   //过期时间

        private void SetTime()
        {
            this.ExpireTime = DateTime.Now.AddDays(1);
        }
    }
}
