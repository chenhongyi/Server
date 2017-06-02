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
        public Bag(Bag bag)
        {
            this.CurUsedCell = bag.CurUsedCell;
            this.MaxCellNumber = bag.MaxCellNumber;
            this.Items = bag.Items;
        }
        public Bag()
        {
            this.MaxCellNumber = 999;
            this.CurUsedCell = 0;
            this.Items = new Dictionary<int, Item>();
#if DEBUG

            this.Items.Add(1, new Item()//钞票
            {
                CurCount = 100000,
                OnSpace = 0
            });
            this.Items.Add(2, new Item()//金砖
            {
                OnSpace = 0,
                CurCount = 100000
            });
            this.Items.Add(3, new Item()//创客币
            {
                OnSpace = 0,
                CurCount = 100000
            });

            {
                //new Item {  CurCount = 500000, Id = 1, OnSpace = 0}, //钞票
                //new Item {  CurCount = 500000, Id = 2, OnSpace = 0}, //金砖
                //new Item {  CurCount = 500000, Id = 3, OnSpace = 0}  //创客币
            };
#else
             this.Items.Add(1, new Item()//钞票
            {
                CurCount = 0,
                OnSpace = 0
            });
            this.Items.Add(2, new Item()//金砖
            {
                OnSpace = 0,
                CurCount = 0
            });
            this.Items.Add(3, new Item()//创客币
            {
                OnSpace = 0,
                CurCount = 0
            });
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


        [DataMember] public Dictionary<int, Item> Items { get; set; }

    }
}
