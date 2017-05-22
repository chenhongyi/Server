using AutoData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Data.Business
{
    [DataContract]
    public class DepartmentGroup
    {
        /// <summary>
        /// 财务部
        /// </summary>
        [DataMember] public Department Finance { get; set; } = new Department(DepartMentType.Finance);
        /// <summary>
        /// 人事部
        /// </summary>
        [DataMember] public Department Personnel { get; set; } = new Department(DepartMentType.Personnel);
        /// <summary>
        /// 市场部
        /// </summary>
        [DataMember] public Department Market { get; set; } = new Department(DepartMentType.Market);
        /// <summary>
        /// 投资部
        /// </summary>
        [DataMember] public Department Investment { get; set; } = new Department(DepartMentType.Investment);
    }

    [DataContract]
    public class Department
    {
        public Department()
        {

        }
#if DEBUG
        public Department(DepartMentType type)
        {
            var department = DepartmentInfo.GetForId(100001);
            switch (type)
            {
                case DepartMentType.Finance:    //财务部初始化数值
                    department = DepartmentInfo.GetForId(130001);
                    break;
                case DepartMentType.Personnel:  //人事部初始化数值
                    department = DepartmentInfo.GetForId(100001);
                    this.CurTalentLv = 20;
                    break;
                case DepartMentType.Market:     //市场部初始化数值
                    department = DepartmentInfo.GetForId(120001);
                    this.CurStoreAddtion = 20.00f;
                    break;
                case DepartMentType.Investment: //投资部初始化数值
                    department = DepartmentInfo.GetForId(110001);
                    break;
            }
            this.Id = Guid.NewGuid();
            this.Type = type;
            this.Level = department.level;
            this.MakerCoinCounts = 1;
            this.CurPurchaseCounts = 1;
            this.CurStrategicCounts = 1;
            this.CurStore = 10;
            this.CurStaff = 10;
            this.CurRealestate = 10;
            this.CurPropagandaCounts = 1;
            this.CurExtension = 10;
            this.CurDirectorCounts = 20;
        }
#else

        public Department(DepartMentType type)
        {
            var department = DepartmentInfo.GetForId(100001);
            switch (type)
            {
                case DepartMentType.Finance:    //财务部初始化数值
                    department = DepartmentInfo.GetForId(130001);
                    break;
                case DepartMentType.Personnel:  //人事部初始化数值
                    department = DepartmentInfo.GetForId(100001);
                    this.CurTalentLv = department.TalentLvLimit;
                    break;
                case DepartMentType.Market:     //市场部初始化数值
                    department = DepartmentInfo.GetForId(120001);
                    this.CurStoreAddtion = department.StoreAddtion;
                    break;
                case DepartMentType.Investment: //投资部初始化数值
                    department = DepartmentInfo.GetForId(110001);
                    break;
            }
            this.Id = Guid.NewGuid();
            this.Type = type;
            this.Level = department.level;
            this.MakerCoinCounts = 0;
            this.CurPurchaseCounts = 0;
            this.CurStrategicCounts = 0;
            this.CurStore = 0;
            this.CurStaff = 0;
            this.CurRealestate = 0;
            this.CurPropagandaCounts = 0;
            this.CurExtension = 0;
            this.CurDirectorCounts = 0;
        }
#endif
        [DataMember] public Guid Id { get; set; }
        /// <summary>
        /// 类型  财务部  人事部 投资部 市场部
        /// </summary>
        [DataMember] public DepartMentType Type { get; set; }
        /// <summary>
        /// 当前级别
        /// </summary>
        [DataMember] public int Level { get; set; }
        /// <summary>
        /// 当前拥有主管人数
        /// </summary>
        [DataMember] public int CurDirectorCounts { get; set; }
        /// <summary>
        /// 当前拥有员工
        /// </summary>
        [DataMember] public int CurStaff { get; set; }
        /// <summary>
        /// 当前人才市场等级 人事部
        /// </summary>
        [DataMember] public int CurTalentLv { get; set; }
        /// <summary>
        /// 当前地产数量 投资部
        /// </summary>
        [DataMember] public int CurRealestate { get; set; }
        /// <summary>
        /// 当前拥有店铺 投资部
        /// </summary>
        [DataMember] public int CurStore { get; set; }
        /// <summary>
        /// 当前扩建星级 投资部
        /// </summary>
        [DataMember] public int CurExtension { get; set; }
        /// <summary>
        /// 当前店铺产出加成百分比    市场部
        /// </summary>
        [DataMember] public float CurStoreAddtion { get; set; }
        /// <summary>
        /// 当前已宣传次数 市场部
        /// </summary>
        [DataMember] public int CurPropagandaCounts { get; set; }
        /// <summary>
        /// 当前已使用策略打击次数 市场部
        /// </summary>
        [DataMember] public int CurStrategicCounts { get; set; }
        /// <summary>
        /// 当前公司已采购次数 市场部
        /// </summary>
        [DataMember] public int CurPurchaseCounts { get; set; }
        /// <summary>
        /// 当前已兑换创客币次数 财务部
        /// </summary>
        [DataMember] public int MakerCoinCounts { get; set; }
    }

    [DataContract]
    public enum DepartMentType
    {
        /// <summary>
        /// 财务部
        /// </summary>
        [EnumMember] Finance = 1,
        /// <summary>
        /// 人事部
        /// </summary>
        [EnumMember] Personnel = 2,
        /// <summary>
        /// 市场部
        /// </summary>
        [EnumMember] Market = 3,
        /// <summary>
        /// 投资部
        /// </summary>
        [EnumMember] Investment = 4,
    }
}
