using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class GetMapResult : BaseResponseData
    {
        [ProtoMember(1)] public Dictionary<int, LoadLandInfo> LoadLandInfoDic { get; set; } = new Dictionary<int, LoadLandInfo>();
        [ProtoMember(2)] public List<LoadBuildInfo> LoadBuildInfo { get; set; } = new List<LoadBuildInfo>();
    }
    [ProtoContract]
    public class LoadLandInfo
    {
        /// <summary>
        /// empty =0,OtherLand = 1,YourLand = 2, OtherBuild = 3,YourBuild = 4, BanningLand = 5,BanningBuild = 6
        /// </summary>
        [ProtoMember(1)] public int State { get; set; }

        [ProtoMember(2)] public int PosX { get; set; }
        [ProtoMember(3)] public int PoxY { get; set; }
        [ProtoMember(4)] public string RoleId { get; set; }

        [ProtoMember(5)] public string BuildId { get; set; }
    }

    [ProtoContract]
    public class LoadBuildInfo
    {
        [ProtoMember(1)] public string BuildId { get; set; }
        [ProtoMember(2)] public int BuildType { get; set; }
        [ProtoMember(3)] public string Name { get; set; }
        [ProtoMember(4)] public int TodayCanAdvartise { get; set; }
        [ProtoMember(5)] public int Level { get; set; }
        [ProtoMember(6)] public int Popularity { get; set; }
        [ProtoMember(7)] public int Star { get; set; }
        [ProtoMember(8)] public int Employee { get; set; }
        [ProtoMember(9)] public long GetMoney { get; set; }
        [ProtoMember(10)] public int Pos { get; set; }
        [ProtoMember(11)] public string RoleId { get; set; }
        [ProtoMember(12)] public int CustomerAddtion { get; set; }
        [ProtoMember(13)] public int CurExtendLv { get; set; }
        [ProtoMember(14)] public int Income { get; set; }
        [ProtoMember(15)] public long CostGold { get; set; }
    }
}









