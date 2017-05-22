using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Data.Business
{
    [DataContract]
    public class FinanceLogData
    {
        [DataMember] public DateTime Time { get; set; } = DateTime.Now;
        [DataMember] public string EventName { get; set; }
        [DataMember] public FinanceLogType Type { get; set; }
        [DataMember] public int MoneyType { get; set; }
        [DataMember] public long Count { get; set; }
    }

    [DataContract]
    public enum FinanceLogType
    {
        [EnumMember] DonateGold = 1,//捐献金砖
        [EnumMember] GetGain = 2,   //领取店铺收益
        [EnumMember] BuyClothes = 3,//够买时装
        [EnumMember] SellItem = 4,  //卖出物品
        [EnumMember] BuyItems = 5 //购买物品
    }
}
