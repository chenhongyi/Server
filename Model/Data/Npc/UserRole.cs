using AutoData;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace Model.Data.Npc
{
    [DataContract]
    public class UserRole //: BaseNpc
    {
        public UserRole()
        {
            this.UserAttr = new List<UserAttr>();
            this.Avatar = new List<int>();
        }
        public UserRole(int sex, string name, Guid accountId)
        {
            TxtReader.Init();
            this.AccountId = accountId;
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Sex = sex;
            this.UserAttr = new List<UserAttr>();
            this.Avatar = new List<int>();
            this.Certificates = new int[] { 0 };
            this.CertificatesExp = new int[] { 0 };
            Character c = Character.GetIndex(1);
            if (sex == 1) c = Character.GetForId(1);
            else if (sex == 2) c = Character.GetForId(2);
            if (c != null)
            {
                this.Icon = c.Icon;
                this.Type = c.Type;
                this.Level = c.Level;
                foreach (var attr in c.Attribute)
                {
                    this.UserAttr.Add(new Npc.UserAttr()
                    {
                        Count = attr.Count,
                        UserAttrID = attr.AttributeID
                    });
                }
            }
            this.Avatar = c.Avatar;
            this.Desc = c.Desc;
        }


        [DataMember] public Guid Id { get; set; }
        /// <summary>
        /// 角色编号 第几个角色
        /// </summary>
        [DataMember] public int Number { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember] public string Name { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        [DataMember] public int Level { get; set; }
        /// <summary>
        /// 经验值
        /// </summary>
        [DataMember] public long Exp { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [DataMember] public string Icon { get; set; }

        /// <summary>
        /// 类型 
        /// </summary>
        [DataMember] public int Type { get; set; }

        /// <summary>
        /// 账号关联
        /// </summary>
        [DataMember] public Guid AccountId { get; set; }
        /// <summary>
        /// vip级别
        /// </summary>
        [DataMember] public int VipLevel { get; set; }
        /// <summary>
        /// 角色代币
        /// </summary>
        [DataMember] public long Gold { get; set; }
        /// <summary>
        /// 性别 0为默认 1为男 2 为女
        /// </summary>
        [DataMember] public int Sex { get; set; }
        /// <summary>
        ///  智力  贸易-初级  科技-中级  金融-高级
        /// </summary>
        [DataMember] public int Intelligence { get; set; }
        /// <summary>
        /// 魅力    主播-初级   模特-中级   影视-高级
        /// </summary>
        [DataMember] public int Charm { get; set; }
        /// <summary>
        /// 亲和力   快餐-初级   料理-中级   酒店-高级
        /// </summary>
        [DataMember] public int Affinity { get; set; }
        /// <summary>
        /// 专注    食品-初级   服装-中级   家电-高级
        /// </summary>
        [DataMember] public int Concentration { get; set; }
        /// <summary>
        /// 体质    采集-初级   冶炼-中级   建筑-高级
        /// </summary>
        [DataMember] public int Constitution { get; set; }
        /// <summary>
        ///身价
        /// </summary>
        [DataMember] public long SocialStatus { get; set; }
        /// <summary>
        /// 各种证书
        /// </summary>
        [DataMember] public int[] Certificates { get; set; }
        /// <summary>
        /// 各种证书的经验值
        /// </summary>
        [DataMember] public int[] CertificatesExp { get; set; }
        /// <summary>
        /// 角色描述 简介
        /// </summary>
        [DataMember] public string Desc { get; set; }

        /// <summary>
        /// 角色属性
        /// </summary>
        [DataMember] public List<UserAttr> UserAttr { get; set; }
        /// <summary>
        /// 外观
        /// </summary>
        [DataMember] public List<int> Avatar { get; set; }

    }

}
