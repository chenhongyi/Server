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
        /// <summary>
        /// 事件名称
        /// </summary>
        [DataMember] public string EventName { get; set; } = string.Empty;
        /// <summary>
        /// 事件类型
        /// </summary>
        [DataMember] public int Type { get; set; }
        /// <summary>
        /// 消耗货币类型
        /// </summary>
        [DataMember] public int MoneyType { get; set; }
        /// <summary>
        /// 消耗货币数量
        /// </summary>
        [DataMember] public long Count { get; set; }

        /// <summary>
        /// ture 是 增加 false是减少
        /// </summary>
        [DataMember] public bool AorD { get; set; } = true;
    }
}
