using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Data.Business
{
    [DataContract]
    public class ShopProperty
    {
        public ShopProperty(string name, int shopType)
        {
            this.Id = Guid.NewGuid().ToString();
            this.ShopType = shopType;
            this.Name = name;
            this.TodayCanAdvartise = 0;
            this.Level = 1;
            this.Popularity = 1;
            this.Star = 1;
            this.Employee = 0;
            this.GetMoney = 0;
        }

        [DataMember] public string Id { get; set; }
        [DataMember] public int ShopType { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        [DataMember] public string Name { get; set; }

        /// <summary>
        /// 今日已宣传次数
        /// </summary>
        [DataMember] public int TodayCanAdvartise { get; set; } = 0;

        /// <summary>
        /// 级别
        /// </summary>
        [DataMember] public int Level { get; set; } = 1;

        /// <summary>
        /// 当前知名度
        /// </summary>
        [DataMember] public int Popularity { get; set; } = 1;

        /// <summary>
        /// 当前星级
        /// </summary>
        [DataMember] public int Star { get; set; } = 1;


        /// <summary>
        /// 当前拥有员工数量
        /// </summary>
        [DataMember] public int Employee { get; set; } = 0;


        /// <summary>
        /// 今日产出 从凌晨3点算起
        /// </summary>
        [DataMember] public long GetMoney { get; set; } = 0;

    }
}
