using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Data.Business
{

    [DataContract]
    public class LandData
    {
        public LandData() { }

        public LandData(int pos, Guid roleId)
        {
            int y = pos % 10000;
            int x = (pos - y) / 1000;
            this.State = (int)GameEnum.MapStatus.Saled;
            this.PosX = x;
            this.PoxY = y;
            this.RoleId = roleId.ToString();
        }

        [DataMember] public int State { get; set; }

        /// <summary>
        /// x坐标
        /// </summary>
        [DataMember] public int PosX { get; set; }
        /// <summary>
        /// y坐标
        /// </summary>
        [DataMember] public int PoxY { get; set; }
        [DataMember] public string RoleId { get; set; }
        [DataMember] public string ShopId { get; set; }
    }
}
