using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Npc
{
    [ProtoContract]
    public class BaseNpc
    {
        /// <summary>
        /// 角色编号 第几个角色
        /// </summary>
        [ProtoMember(1)] public int Number { get; set; } 
        /// <summary>
        /// 姓名
        /// </summary>
        [ProtoMember(2)] public string Name { get; set; }    
        /// <summary>
        /// 等级
        /// </summary>
        [ProtoMember(3)] public int Level { get; set; } = 0;
        /// <summary>
        /// 经验值
        /// </summary>
        [ProtoMember(4)] public int Exp { get; set; } = 0;


    }
}
