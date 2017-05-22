using Model.Data.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Data.Npc
{
    /// <summary>
    /// 背包
    /// </summary>
    [DataContract]
    public class Bag
    {
        /// <summary>
        /// 最大格子数量
        /// </summary>
        [DataMember] public int MaxCellNumber { get; set; } = 999;
        /// <summary>
        /// 当前已使用格子数量
        /// </summary>
        [DataMember] public int CurUsedCell { get; set; } = 0;

        /// <summary>
        /// 二次密码
        /// </summary>
        [DataMember] public string Password { get; set; }

        [DataMember] public List<Item> Items { get; set; } = new List<Item>();

    }
}
