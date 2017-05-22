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
AddItemReq = 1000,
ChangeAvatarReq = 1001,
CompanyLvUpReq = 1002,
ConnectingReq = 1003,
CreateCompanyReq = 1004,
CreateRoleReq = 1005,
DeleteRoleReq = 1006,
DepartmentUpdateReq = 1012,
JoinGameReq = 1007,
RemoveItemReq = 1008,
SellItemReq = 1009,
TestReq = 1011,
UseItemReq = 1010,
}
    /// <summary>
    /// 服务器返回消息定义
    /// </summary>
    public enum WSResponseMsgID
    {
AddItemResult = 1000,
ChangeAvatarResult = 1001,
CompanyLvUpResult = 1002,
ConnectingResult = 1003,
CreateCompanyResult = 1004,
CreateRoleResult = 1005,
DeleteRoleResult = 1006,
DepartmentUpdateResult = 1017,
JoinGameResult = 1007,
RemoveItemResult = 1008,
SellItemResult = 1009,
TCLevelUpResult = 1010,
TCRoleBagChangeResult = 1011,
TCRoleInfoChangeResult = 1012,
TestResult = 1016,
UpdateAvatarResult = 1013,
UpdateShenjiaResult = 1014,
UseItemResult = 1015,
}
}
