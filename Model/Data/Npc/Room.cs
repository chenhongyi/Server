using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Model.Data.Npc
{
    [DataContract]
    public class Room
    {
        [DataMember] public string RoleId { get; set; }
        /// <summary>
        /// 当前使用的房间
        /// </summary>
        [DataMember] public int RoomId { get; set; }
        /// <summary>
        /// 当前房间的配置
        /// </summary>
        [DataMember] public Dictionary<int, int> Config { get; set; }
    }
}