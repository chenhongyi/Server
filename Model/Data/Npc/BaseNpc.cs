using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Data.Npc
{
    [DataContract]
    public class BaseNpc
    {
        [DataMember] public Guid Id { get; set; }
        /// <summary>
        /// 角色编号 第几个角色
        /// </summary>
        [DataMember] public int Number { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember] public string Name { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        [DataMember] public int Level { get; set; } 
        /// <summary>
        /// 经验值
        /// </summary>
        [DataMember] public int Exp { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [DataMember] public string Icon { get; set; }

        /// <summary>
        /// 类型 
        /// </summary>
        [DataMember] public int Type { get; set; }


    }
}
