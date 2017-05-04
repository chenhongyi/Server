using AutoData;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Npc
{
    [ProtoContract]
    public class UserRole : BaseNpc
    {
        public UserRole()
        {

        }
        public UserRole(int sex, string name)
        {
            TxtReader.Init();
            Character c = Character.GetIndex(sex);
            this.Level = c.Level;
            this.Name = name;
            this.Sex = sex;
            this.Desc = c.Desc;
        }
        /// <summary>
        /// 账号关联
        /// </summary>
        public Guid AccountId { get; set; }

        /// <summary>
        /// vip级别
        /// </summary>
        [ProtoMember(100)] public byte VipLevel { get; set; }
        /// <summary>
        /// 角色代币
        /// </summary>
        [ProtoMember(101)] public int Gold { get; set; }
        /// <summary>
        /// 性别 0为默认 1为男 2 为女
        /// </summary>
        [ProtoMember(102)] public int Sex { get; set; } = 0;
        /// <summary>
        /// 透波模型id 默认为0
        /// </summary>
        [ProtoMember(103)] public int HeadModelId { get; set; } = 0;
        /// <summary>
        /// 上半身模型id 默认为0
        /// </summary>
        [ProtoMember(104)] public int BodyModelId { get; set; } = 0;
        /// <summary>
        /// 下半身模型id 默认为0
        /// </summary>
        [ProtoMember(105)] public int FootModelId { get; set; } = 0;
        /// <summary>
        ///  智力  贸易-初级  科技-中级  金融-高级
        /// </summary>
        [ProtoMember(106)] public int Intelligence { get; set; } = 50;
        /// <summary>
        /// 魅力    主播-初级   模特-中级   影视-高级
        /// </summary>
        [ProtoMember(107)] public int Charm { get; set; } = 50;
        /// <summary>
        /// 亲和力   快餐-初级   料理-中级   酒店-高级
        /// </summary>
        [ProtoMember(108)] public int Affinity { get; set; } = 50;
        /// <summary>
        /// 专注    食品-初级   服装-中级   家电-高级
        /// </summary>
        [ProtoMember(109)] public int Concentration { get; set; } = 50;
        /// <summary>
        /// 体质    采集-初级   冶炼-中级   建筑-高级
        /// </summary>
        [ProtoMember(110)] public int Constitution { get; set; } = 50;
        /// <summary>
        ///身价
        /// </summary>
        [ProtoMember(111)] public int SocialStatus { get; set; } = 0;
        /// <summary>
        /// 各种证书
        /// </summary>
        [ProtoMember(112)] public byte[] Certificates { get; set; }
        /// <summary>
        /// 各种证书的经验值
        /// </summary>
        [ProtoMember(113)] public int[] CertificatesExp { get; set; }
        /// <summary>
        /// 角色描述 简介
        /// </summary>
        [ProtoMember(114)] public string Desc { get; set; }


        #region 原始写法

        ///// <summary>
        ///// 文职证书    1 贸易-初级  2科技-中级  3金融-高级
        ///// </summary>
        //[ProtoMember(112)] public byte CivilianCertificate { get; set; } = 0;
        ///// <summary>
        ///// 文职证书当前经验值
        ///// </summary>
        //[ProtoMember(113)] public int CCExp { get; set; }
        ///// <summary>
        ///// 演艺证书  1主播-初级   2模特-中级  3 影视-高级
        ///// </summary>
        //[ProtoMember(114)] public byte PerformanceCertificate { get; set; } = 0;
        ///// <summary>
        ///// 演艺证书当前经验值
        ///// </summary>
        //[ProtoMember(115)] public int PCExp { get; set; }
        ///// <summary>
        ///// 餐饮证书   1快餐-初级   2料理-中级   3酒店-高级
        ///// </summary>
        //[ProtoMember(116)] public byte FoodCertificate { get; set; } = 0;
        ///// <summary>
        ///// 餐饮证书当前经验值
        ///// </summary>
        //[ProtoMember(117)] public int FCExp { get; set; }
        ///// <summary>
        ///// 制造证书      1食品-初级  2 服装-中级 3  家电-高级
        ///// </summary>
        //[ProtoMember(118)] public byte ManufactureCertificate { get; set; } = 0;
        ///// <summary>
        /////制造证书当前经验值
        ///// </summary>
        //[ProtoMember(119)] public int MCExp { get; set; }
        ///// <summary>
        ///// 冶炼证书  1采集-初级   2冶炼-中级   3建筑-高级
        ///// </summary>
        //[ProtoMember(120)] public byte SmeltCertificate { get; set; } = 0;
        ///// <summary>
        ///// 冶炼证书当前经验值
        ///// </summary>
        //[ProtoMember(121)] public int SCExp { get; set; }      
        /// <summary>
        /// 账号关联
        /// </summary>

        #endregion
    }

    public class UserRoleResult
    {
        public int Status { get; set; } = 0;

#if DEBUG
        public BaseNpc BaseNpc { get; set; }
#else
         public UserRole UserRole { get; set; }
#endif




    }
}
