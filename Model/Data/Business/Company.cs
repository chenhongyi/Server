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
    public class Company
    {
        public Company()
        {

        }

#if DEBUG
        public Company(string name)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.CreateTime = DateTime.Now;
            var comp = CompanyInfo.GetForId(1);
            if (comp != null)
            {
                this.CurExp = 200000;
                this.Level = comp.Id;
                //this.CurFinanceLv = 20;
                //this.CurInvestmentLv = 20;
                //this.CurMarketLv =20;
                //this.CurPersonnelLv = 20;
            }
            //TODO
        }
#else


        public Company(string name)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.CreateTime = DateTime.Now;
            var comp = CompanyInfo.GetForId(1);
            if (comp!=null)
            {
                this.CurExp = 0;
                this.Level = comp.Id;
                this.CurFinanceLv = comp.FinanceLv;
                this.CurInvestmentLv = comp.InvestmentLv;
                this.CurMarketLv = comp.MarketLv;
                this.CurPersonnelLv = comp.PersonnelLv;
            }
            //TODO
        }
#endif
        /// <summary>
        /// id
        /// </summary>
        [DataMember] public Guid Id { get; set; }
        /// <summary>
        /// 公司名
        /// </summary>
        [DataMember] public string Name { get; set; }
        /// <summary>
        /// 当前级别
        /// </summary>
        [DataMember] public int Level { get; set; }
        /// <summary>
        /// 当前营收  当前经验值
        /// </summary>
        [DataMember] public long CurExp { get; set; }
        /// <summary>
        /// 当前人事部等级
        /// </summary>
        //[DataMember] public int CurPersonnelLv { get; set; }
        ///// <summary>
        /////当前财务部等级
        ///// </summary>
        //[DataMember] public int CurFinanceLv { get; set; }
        ///// <summary>
        ///// 当前市场部等级
        ///// </summary>
        //[DataMember] public int CurMarketLv { get; set; }
        ///// <summary>
        ///// 当前投资部等级
        ///// </summary>
        //[DataMember] public int CurInvestmentLv { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember] public DateTime CreateTime { get; set; }
    }
}
