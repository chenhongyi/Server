using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Token
    {
        public Token()
        {
            SetTime();
        }
        public Guid TokenID { get; set; }
        public string UserPassport { get; set; }    //用户账号
        public string SignToken { get; set; }     //登录凭据
        public DateTime ExpireTime { get; set; }   //过期时间

        private void SetTime()
        {
            this.ExpireTime = DateTime.Now.AddDays(1);
        }
    }
}
