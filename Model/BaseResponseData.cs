using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class BaseResponseData
    {
        public WsResult Result { get; set; } = WsResult.Success;
    }
}
