using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class BaseResponseData
    {
        public GameEnum.WsResult Result { get; set; } = GameEnum.WsResult.Success;
    }
}
