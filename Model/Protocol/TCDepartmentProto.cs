using Model.ResponseData;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Protocol
{
    /// <summary>
    /// 更新部门投资部
    /// </summary>
    [ProtoContract]
    public class TCDepartmentInvestmentResult
    {
        [ProtoMember(1)] public InvestmentInfo InvestmentInfo { get; set; }
    }

    /// <summary>
    /// 更新部门财务部
    /// </summary>
    [ProtoContract]
    public class TCDepartmentFinanceResult
    {
        [ProtoMember(1)] public FinanceInfo FinanceInfo { get; set; }
    }

    /// <summary>
    /// 更新部门 人事部
    /// </summary>
    [ProtoContract]
    public class TCDepartmentPersonnelResult
    {
        [ProtoMember(1)] public PersonnelInfo PersonnelInfo { get; set; }
    }

    /// <summary>
    /// 更新部门 市场部
    /// </summary>
    [ProtoContract]
    public class TCDepartmentMarketResult
    {
        [ProtoMember(1)] public MarketInfo MarketInfo { get; set; }
    }
}
