using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Data.General
{
    /// <summary>
    /// 道具
    /// </summary>
    [DataContract]
    public class Item
    {
        /// <summary>
        /// 编号
        /// </summary>
        [DataMember] public int Id { get; set; }
        /// <summary>
        /// 当前叠加数量
        /// </summary>
        [DataMember] public long CurCount { get; set; } 
        /// <summary>
        /// 占用空间 当道具数量超出999时  占用空间+1
        /// </summary>
        [DataMember] public long OnSpace { get; set; } 

    }
}
