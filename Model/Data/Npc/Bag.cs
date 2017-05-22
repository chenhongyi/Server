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
        public Bag()
        {
            this.MaxCellNumber = 999;
            this.CurUsedCell = 0;
#if DEBUG
            this.Items = new List<Item>{
            new Item {  CurCount = 50000000, Id = 1, OnSpace = 0}, //钞票
            new Item {  CurCount = 50000000, Id = 2, OnSpace = 0}, //金砖
            new Item {  CurCount = 50000000, Id = 3, OnSpace = 0}  //创客币
            };
#else
              this.Items = new List<Item>{
            new Item {  CurCount = 0, Id = 1, OnSpace = 0}, //钞票
            new Item {  CurCount = 0, Id = 2, OnSpace = 0}, //金砖
            new Item {  CurCount = 0, Id = 3, OnSpace = 0}  //创客币
            };
#endif
        }
        /// <summary>
        /// 最大格子数量
        /// </summary>
        [DataMember] public int MaxCellNumber { get; set; }
        /// <summary>
        /// 当前已使用格子数量
        /// </summary>
        [DataMember] public long CurUsedCell { get; set; }

        /// <summary>
        /// 二次密码
        /// </summary>
        [DataMember] public string Password { get; set; }

        [DataMember] public List<Item> Items { get; set; }

    }
}
