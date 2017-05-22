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
    public enum WsResult
    {
        /// <summary>
        /// 正常
        /// </summary>
        Success = 0,
        /// <summary>
        /// 重名
        /// </summary>
        DuplicationName = 1,
        /// <summary>
        /// 已有过多角色
        /// </summary>
        MoreRoles = 2,
        /// <summary>
        /// 参数错误
        /// </summary>
        ParamsError = 3,
        /// <summary>
        /// Token不存在
        /// </summary>
        TokenIsNotExists = 4,
        /// <summary>
        /// Token过期
        /// </summary>
        TokenTimeOut = 5,
        /// <summary>
        /// 用户不存在
        /// </summary>
        NotAccount = 6,
        /// <summary>
        /// 角色id为空
        /// </summary>
        RoleIdIsNull = 7,
        /// <summary>
        /// 角色不存在
        /// </summary>
        RoleIsNotExists = 8,

        /// <summary>
        /// 账号中无角色
        /// </summary>
        NotRole = 9,
        /// <summary>
        /// 道具类型不符
        /// </summary>
        ItemTypeError = 10,
        /// <summary>
        /// 背包中没有该道具
        /// </summary>
        NotItemInBag = 11,
        /// <summary>
        /// 没有道具配置信息
        /// </summary>
        NotItemConfig = 12,
        /// <summary>
        /// 没有达到道具的使用级别
        /// </summary>
        NeedLevel = 13,
        /// <summary>
        /// 背包空间不够
        /// </summary>
        NotEnoughBagSpace = 14,
        /// <summary>
        /// 用户背包错误 
        /// </summary>
        RoleBagErr = 15,
        /// <summary>
        /// 道具数量不足
        /// </summary>
        NotEnoughItem = 16,
        /// <summary>
        /// 删除道具出错
        /// </summary>
        RemoveItemErr = 17,
        /// <summary>
        /// 公司升级失败 条件不足
        /// </summary>
        CompanyLvUpFailed = 18,
        /// <summary>
        /// 没有找到公司
        /// </summary>
        NotFoundCompany = 19,
        /// <summary>
        /// 部门无效  没有找到部门
        /// </summary>
        DepartmentInvalid = 20,
        /// <summary>
        /// 部门升级失败
        /// </summary>
        DepartmentLvUpErr = 21,
        /// <summary>
        /// 升级失败 条件没满足
        /// </summary>
        DepartmentLvUpFailed = 22,
        /// <summary>
        /// 配置文件错误
        /// </summary>
        ConfigErr = 23,
        /// <summary>
        /// 不能创建过多公司
        /// </summary>
        MoreCompany = 24,
        /// <summary>
        /// 不能创建过多部门
        /// </summary>
        MoreDepartment = 25,

        /// <summary>
        /// 需要大于0 的正整数
        /// </summary>
        PositiveInteger = 26,
        /// <summary>
        /// 未定义错误
        /// </summary>
        Error = 100,
        /// <summary>
        /// 没有处理程序
        /// </summary>
        NoneActionFunc = 101,

    }
}
