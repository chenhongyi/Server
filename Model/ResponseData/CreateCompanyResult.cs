using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ResponseData
{
    [ProtoContract]
    public class CreateCompanyResult:BaseResponseData
    {
        [ProtoMember(1)] public LoadCompanyInfo CompanyInfo { get; set; } = new LoadCompanyInfo();
        [ProtoMember(2)] public LoadDepartMentInfo DepartmentInfo { get; set; } = new LoadDepartMentInfo();
    }
}
