using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 返回处理结果码
    /// </summary>
    public static class WsResult
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const int Success = 0;
        /// <summary>
        /// 重名
        /// </summary>
        public const int DuplicationName = 1;
        /// <summary>
        /// 已有过多角色
        /// </summary>
        public const int MoreRoles = 2;
        /// <summary>
        /// 参数错误
        /// </summary>
        public const int ParamsError = 3;
        /// <summary>
        /// Token不存在
        /// </summary>
        public const int TokenIsNotExists = 4;
        /// <summary>
        /// Token过期
        /// </summary>
        public const int TokenTimeOut = 5;
        /// <summary>
        /// 用户不存在
        /// </summary>
        public const int NoneUser = 6;
        /// <summary>
        /// 未定义错误
        /// </summary>
        public const int Error = 100;
        /// <summary>
        /// 没有处理程序
        /// </summary>
        public const int NoneActionFunc = 101;

        
    }
}
