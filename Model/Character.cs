using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Character
    {
        public Guid CharacterId { get; set; }       //等于 account的唯一标识

        public byte Number { get; set; }        //角色序号  这是该账号下的第几个角色
        public string Name { get; set; }        //角色名字

        public List<DateTime> loginTime { get; set; }   //登录时间 保留10条

        public void SetLoginTime()
        {
            DateTime time = DateTime.Now;
            if (loginTime.Count == 10)
            {
                loginTime.RemoveAt(0);
            }
            loginTime.Add(time);
        }

        public List<DateTime> GetLoginTime()
        {
            if (loginTime.Any())
            {
                return loginTime;
            }
            return null;
        }

        public bool IsDelete { get; set; } = false; //逻辑删除
        public bool IsBan { get; set; } = false;    //逻辑禁用
        public int Gold { get; set; } = 0;//代币

        public int Hp { get; set; } //当前血量
        public int Mp { get; set; }    //当前蓝量

        public int Power { get; set; } //当前能量

        public int Exp { get; set; }  //当前经验值
    }
}
