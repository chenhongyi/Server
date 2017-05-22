using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Data.Npc
{
    /// <summary>
    /// 角色名和id绑定表
    /// </summary>
    [DataContract]
    public class IdRole
    {
        public IdRole() { }
        public IdRole(string name, Guid id)
        {
            this.Name = name;
            this.Id = id;
        }

        /// <summary>
        /// 角色名
        /// </summary>
        [DataMember] public string Name { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        [DataMember] public Guid Id { get; set; }
    }
}
