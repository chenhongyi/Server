namespace Model
{
    public class ServiceConst
{
        public const string DataApiWebsockets = "data";
        public const string ListenerRemoting = "Remoting1";
        public const string ListenerWebsocket = "Websocket1";
   public const string ListenerOwin = "Owin1";
    }

    /// <summary>
    /// 客户端请求消息定义
    /// </summary>
    public enum WSRequestMsgID
    {
        /// <summary>
        /// 添加道具进背包请求
        /// </summary>
        AddItemReq = 1000,
        /// <summary>
        ///
        /// </summary>
        BuyLandReq = 1011,
        /// <summary>
        /// 时装id
        /// </summary>
        ChangeAvatarReq = 1001,
        /// <summary>
        ///
        /// </summary>
        CompanyLvUpReq = 1002,
        /// <summary>
        /// 
        /// </summary>
        ConnectingReq = 1003,
        /// <summary>
        ///
        /// </summary>
        CreateBuildReq = 1015,
        /// <summary>
        ///
        /// </summary>
        CreateCompanyReq = 1004,
        /// <summary>
        /// NewRoleResult
        /// </summary>
        CreateRoleReq = 1005,
        /// <summary>
        ///
        /// </summary>
        DeleteRoleReq = 1006,
        /// <summary>
        ///
        /// </summary>
        DepartmentUpdateReq = 1012,
        /// <summary>
        ///
        /// </summary>
        DestoryBuildReq = 1016,
        /// <summary>
        ///
        /// </summary>
        GetMapReq = 1013,
        /// <summary>
        /// 客户端请求加入游戏
        /// </summary>
        JoinGameReq = 1007,
        /// <summary>
        ///
        /// </summary>
        RemoveItemReq = 1008,
        /// <summary>
        ///
        /// </summary>
        SellItemReq = 1009,
        /// <summary>
        /// 使用物品
        /// </summary>
        UseItemReq = 1010,
        /// <summary>
        /// 店铺扩建
        /// </summary>
        BuildExtendReq = 1023,
        /// <summary>
        /// 建筑升级
        /// </summary>
        BuildLvUpReq = 1022,
        /// <summary>
        /// 道具购买
        /// </summary>
        ItemBuyReq = 1017,
        /// <summary>
        /// 客户端请求用户的房间信息
        /// </summary>
        RoomReq = 1014,
        /// <summary>
        /// 请求更新房间配置
        /// </summary>
        RoomConfigUpdateReq = 1018,
        /// <summary>
        ///升级
        /// </summary>
        RoomBuyReq = 1019,
        /// <summary>
        /// 房间出售
        /// </summary>
        RoomSellReq = 1020,
        /// <summary>
        /// 拜访用户房间
        /// </summary>
        RoomVisitReq = 1021,
    }
    /// <summary>
    /// 服务器返回消息定义
    /// </summary>
    public enum WSResponseMsgID
    {
        /// <summary>
        /// 店铺扩建成功
        /// </summary>
        BuildExtendResult = 1032,
        /// <summary>
        /// 店铺扩建失败
        /// </summary>
        BuildExtendFailedResult = 1033,
        /// <summary>
        /// 建筑升级
        /// </summary>
        BuildLvUpResult = 1031,
        /// <summary>
        /// 建筑升级
        /// </summary>
        BuildLvUpFailedResult = 1038,
        /// <summary>
        /// 道具购买返回
        /// </summary>
        ItemBuyResult = 1025,
        /// <summary>
        /// 道具更新
        /// </summary>
        UpdateItemResult = 1030,
        /// <summary>
        /// 返回给客户端的房间信息
        /// </summary>
        RoomResult = 1022,
        /// <summary>
        /// 请求更新房间配置
        /// </summary>
        RoomConfigUpdateResult = 1026,
        /// <summary>
        ///升级
        /// </summary>
        RoomBuyResult = 1027,
        /// <summary>
        /// 房间出售
        /// </summary>
        RoomSellResult = 1028,
        /// <summary>
        /// 拜访用户房间
        /// </summary>
        RoomVisitResult = 1029,
        /// <summary>
        /// 更新部门投资部
        /// </summary>
        TCDepartmentInvestmentResult = 1034,
        /// <summary>
        /// 更新部门财务部
        /// </summary>
        TCDepartmentFinanceResult = 1035,
        /// <summary>
        /// 更新部门 人事部
        /// </summary>
        TCDepartmentPersonnelResult = 1036,
        /// <summary>
        /// 更新部门 市场部
        /// </summary>
        TCDepartmentMarketResult = 1037,
        /// <summary>
        /// 添加道具进背包返回
        /// </summary>
        AddItemResult = 1000,
        /// <summary>
        /// y坐标
        /// </summary>
        BuyLandResult = 1016,
        /// <summary>
        ///
        /// </summary>
        ChangeAvatarResult = 1001,
        /// <summary>
        ///
        /// </summary>
        CompanyLvUpResult = 1002,
        /// <summary>
        ///
        /// </summary>
        ConnectingResult = 1003,
        /// <summary>
        ///
        /// </summary>
        CreateBuildResult = 1024,
        /// <summary>
        ///
        /// </summary>
        CreateCompanyResult = 1004,
        /// <summary>
        /// 客户端的请求：创建新角色 返回值
        /// </summary>
        CreateRoleResult = 1005,
        /// <summary>
        ///
        /// </summary>
        DeleteRoleResult = 1006,
        /// <summary>
        ///
        /// </summary>
        DepartmentUpdateResult = 1017,
        /// <summary>
        ///
        /// </summary>
        DestoryBuildResult = 1023,
        /// <summary>
        ///
        /// </summary>
        GetMapResult = 1018,
        /// <summary>
        ///
        /// </summary>
        GoldChangedResult = 1019,
        /// <summary>
        /// 各种证书的经验值
        /// </summary>
        JoinGameResult = 1007,
        /// <summary>
        ///
        /// </summary>
        LoadFinanceLogResult = 1020,
        /// <summary>
        /// 移除物品返回
        /// </summary>
        RemoveItemResult = 1008,
        /// <summary>
        /// 出售道具返回
        /// </summary>
        SellItemResult = 1009,
        /// <summary>
        ///
        /// </summary>
        TCFinanceLogChangedResult = 1021,
        /// <summary>
        ///
        /// </summary>
        TCLevelUpResult = 1010,
        /// <summary>
        ///
        /// </summary>
        TCRoleBagChangeResult = 1011,
        /// <summary>
        ///
        /// </summary>
        TCRoleInfoChangeResult = 1012,
        /// <summary>
        ///
        /// </summary>
        UpdateAvatarResult = 1013,
        /// <summary>
        ///
        /// </summary>
        UpdateShenjiaResult = 1014,
        /// <summary>
        /// 使用物品返回
        /// </summary>
        UseItemResult = 1015,
    }
}
