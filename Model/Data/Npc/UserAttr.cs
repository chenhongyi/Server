using System;
using System.Runtime.Serialization;

namespace Model.Data.Npc
{
    [DataContract]
    public class UserAttr
    {
        /// <summary>
        /// 属性
        /// </summary>
        [DataMember] public int UserAttrID { get; set; } = 0;
        /// <summary>
        /// 值
        /// </summary>
        [DataMember] public int Count { get; set; } = 0;

    }
}
