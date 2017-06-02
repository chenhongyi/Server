using Model.Data.Npc;
using Model.Protocol;
using Model.ResponseData.father;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class JoinGameResult : BaseResponseData
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [ProtoMember(11)] public string RoleId { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [ProtoMember(12)] public string Icon { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [ProtoMember(13)] public string Name { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        [ProtoMember(14)] public int Level { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [ProtoMember(15)] public int Type { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [ProtoMember(16)] public int Sex { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        [ProtoMember(17)] public List<UserAttr> UserAttr { get; set; } = new List<UserAttr>();

        /// <summary>
        /// 角色初始外观
        /// </summary>
        [ProtoMember(18)] public List<int> Avatar { get; set; } = new List<int>();

        /// <summary>
        /// 描述
        /// </summary>
        [ProtoMember(19)] public string Desc { get; set; }

        /// <summary>
        /// vip级别
        /// </summary>
        [ProtoMember(20)] public int VipLevel { get; set; }
        /// <summary>
        /// 拥有代币
        /// </summary>
        [ProtoMember(21)] public long Gold { get; set; }

        /// <summary>
        ///  智力  贸易-初级  科技-中级  金融-高级
        /// </summary>
        [ProtoMember(22)] public int Intelligence { get; set; }
        /// <summary>
        /// 魅力    主播-初级   模特-中级   影视-高级
        /// </summary>
        [ProtoMember(23)] public int Charm { get; set; }
        /// <summary>
        /// 亲和力   快餐-初级   料理-中级   酒店-高级
        /// </summary>
        [ProtoMember(24)] public int Affinity { get; set; }
        /// <summary>
        /// 专注    食品-初级   服装-中级   家电-高级
        /// </summary>
        [ProtoMember(25)] public int Concentration { get; set; }
        /// <summary>
        /// 体质    采集-初级   冶炼-中级   建筑-高级
        /// </summary>
        [ProtoMember(26)] public int Constitution { get; set; }
        /// <summary>
        ///身价
        /// </summary>
        [ProtoMember(27)] public long SocialStatus { get; set; }
        /// <summary>
        /// 各种证书
        /// </summary>
        [ProtoMember(28)] public int[] Certificates { get; set; }
        /// <summary>
        /// 各种证书的经验值
        /// </summary>
        [ProtoMember(29)] public int[] CertificatesExp { get; set; }
        [ProtoMember(30)] public BagInfo RoleBag { get; set; }
        [ProtoMember(31)] public long Exp { get; set; }
        [ProtoMember(32)] public LoadCompanyInfo CompanyInfo { get; set; } = new LoadCompanyInfo();
        [ProtoMember(33)] public LoadDepartMentInfo DepartInfoInfo { get; set; } = new LoadDepartMentInfo();
        [ProtoMember(34)] public List<LoadFinanceLogInfo> FinanceLogInfo { get; set; } = new List<LoadFinanceLogInfo>();
        [ProtoMember(35)] public GetMapResult MapInfo { get; set; } = new GetMapResult();
        [ProtoMember(36)] public RoomResult Room { get; set; }
    }

    [ProtoContract]
    public class UserAttr
    {
        [ProtoMember(100)] public int UserAttrID { get; set; }
        [ProtoMember(101)] public int Count { get; set; }
    }
    [ProtoContract]
    public class CanChangeAttr
    {
        [ProtoMember(1)] public List<UserAttr> UserAttr { get; set; }

        [ProtoMember(2)] public List<int> Avatar { get; set; }

        /// <summary>
        ///  智力  贸易-初级  科技-中级  金融-高级
        /// </summary>
        [ProtoMember(3)] public int Intelligence { get; set; }
        /// <summary>
        /// 魅力    主播-初级   模特-中级   影视-高级
        /// </summary>
        [ProtoMember(4)] public int Charm { get; set; }
        /// <summary>
        /// 亲和力   快餐-初级   料理-中级   酒店-高级
        /// </summary>
        [ProtoMember(5)] public int Affinity { get; set; }
        /// <summary>
        /// 专注    食品-初级   服装-中级   家电-高级
        /// </summary>
        [ProtoMember(6)] public int Concentration { get; set; }
        /// <summary>
        /// 体质    采集-初级   冶炼-中级   建筑-高级
        /// </summary>
        [ProtoMember(7)] public int Constitution { get; set; }
        /// <summary>
        ///身价
        /// </summary>
        [ProtoMember(8)] public int SocialStatus { get; set; }
        /// <summary>
        /// 各种证书
        /// </summary>
        [ProtoMember(9)] public int[] Certificates { get; set; }
        /// <summary>
        /// 各种证书的经验值
        /// </summary>
        [ProtoMember(10)] public int[] CertificatesExp { get; set; }
        /// <summary>
        /// vip级别
        /// </summary>
        [ProtoMember(11)] public int VipLevel { get; set; }

    }

    [ProtoContract]
    public class BagInfo
    {
        /// <summary>
        /// 最大格子数量
        /// </summary>
        [ProtoMember(1)] public long MaxCellNumber { get; set; }
        /// <summary>
        /// 当前已使用格子数量
        /// </summary>
        [ProtoMember(2)] public long CurUsedCell { get; set; }

        [ProtoMember(3)] public Dictionary<int, LoadRoleBagInfo> Items { get; set; } = new Dictionary<int, LoadRoleBagInfo>();
    }
    [ProtoContract]
    public class LoadRoleBagInfo
    {
        /// <summary>
        /// 当前叠加数量
        /// </summary>
        [ProtoMember(306)] public long CurCount { get; set; }
        /// <summary>
        /// 占用空间 当道具数量超出999时  占用空间+1
        /// </summary>
        [ProtoMember(307)] public long OnSpace { get; set; }
    }
    [ProtoContract]
    public class LoadCompanyInfo
    {
        [ProtoMember(1)] public string Id { get; set; }
        [ProtoMember(2)] public string Name { get; set; }
        [ProtoMember(3)] public int Level { get; set; }
        [ProtoMember(4)] public long NDExp { get; set; }
        [ProtoMember(5)] public long CurExp { get; set; }
        [ProtoMember(6)] public int CurPersonnelLv { get; set; }
        [ProtoMember(7)] public int CurFinanceLv { get; set; }
        [ProtoMember(8)] public int CurMarketLv { get; set; }
        [ProtoMember(9)] public int CurInvestmentLv { get; set; }
        [ProtoMember(10)] public long CostGold { get; set; }
        [ProtoMember(11)] public int Basicpassenger { get; set; }
        [ProtoMember(12)] public int DepartLvLimit { get; set; }
        [ProtoMember(13)] public float CompanyAddition { get; set; }
        [ProtoMember(14)] public long SocialStatus { get; set; }
        [ProtoMember(15)] public string CreateTime { get; set; }
    }
    [ProtoContract]
    public class LoadDepartMentInfo
    {
        [ProtoMember(1)] public FinanceInfo FinanceInfo { get; set; }
        [ProtoMember(2)] public PersonnelInfo PersonnelInfo { get; set; }
        [ProtoMember(3)] public MarketInfo MarketInfo { get; set; }
        [ProtoMember(4)] public InvestmentInfo InvestmentInfo { get; set; }

    }

    [ProtoContract]
    public class InvestmentInfo
    {
        [ProtoMember(1)] public int Level { get; set; }
        [ProtoMember(2)] public int CurDirectorCounts { get; set; }
        [ProtoMember(3)] public int CurStaff { get; set; }
        [ProtoMember(4)] public int CurRealestate { get; set; }
        [ProtoMember(5)] public int CurStore { get; set; }
        [ProtoMember(6)] public int CurExtension { get; set; }
    }

    [ProtoContract]
    public class MarketInfo
    {
        [ProtoMember(1)] public int Level { get; set; }
        [ProtoMember(2)] public int CurDirectorCounts { get; set; }
        [ProtoMember(3)] public int CurStaff { get; set; }
        [ProtoMember(7)] public float CurStoreAddtion { get; set; }
        [ProtoMember(8)] public int CurPropagandaCounts { get; set; }
        [ProtoMember(9)] public int CurStrategicCounts { get; set; }
        [ProtoMember(10)] public int CurPurchaseCounts { get; set; }
    }

    [ProtoContract]
    public class PersonnelInfo
    {
        [ProtoMember(1)] public int Level { get; set; }
        [ProtoMember(2)] public int CurDirectorCounts { get; set; }
        [ProtoMember(3)] public int CurStaff { get; set; }
        [ProtoMember(11)] public int CurTalentLv { get; set; }
    }

    [ProtoContract]
    public class FinanceInfo
    {
        [ProtoMember(1)] public int Level { get; set; }
        [ProtoMember(2)] public int CurDirectorCounts { get; set; }
        [ProtoMember(3)] public int CurStaff { get; set; }
        [ProtoMember(12)] public int MakerCoinCounts { get; set; }
    }

    [ProtoContract]
    public class LoadFinanceLogInfo
    {
        [ProtoMember(1)] public string Time { get; set; }
        [ProtoMember(2)] public string EventName { get; set; }
        [ProtoMember(3)] public int Type { get; set; }
        [ProtoMember(4)] public int MoneyType { get; set; }
        [ProtoMember(5)] public long Count { get; set; }
        [ProtoMember(6)] public bool AorD { get; set; } = true;
    }
}
