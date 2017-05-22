using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using LogicServer.Interface;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Shared.Serializers;
using Model;
using Logging;
using System.Linq;
using Shared;
using Shared.Websockets;
using AutoData;
using Model.RequestData;
using Model.Data.Npc;
using Model.Data.Account;
using Model.ViewModels;
using Model.ResponseData;
using Microsoft.ServiceFabric.Data.Notifications;
using LogicServer.Data;
using Microsoft.ServiceFabric.Data;
using Model.Data.General;
using System.Collections.ObjectModel;
using Model.ResponseData.father;
using Model.MsgQueue;
using SuperSocket.WebSocket;
using PublicGate.Interface;
using LogicServer.Controllers;
using Model.Data.Business;

namespace LogicServer
{
    /// <summary>
    /// 通过 Service Fabric 运行时为每个服务副本创建此类的一个实例。
    /// </summary>
    internal sealed class LogicServer : StatefulService, ILogicServer, IWebSocketConnectionHandler
    {

        private static IReliableStateManager _stateManager;
        private static readonly ILogger Logger = LoggerFactory.GetLogger(nameof(LogicServer));

        private long lastCommitLsn;
        private long lastTransactionId;

        public LogicServer(StatefulServiceContext context)
            : base(context)
        {
            TxtReader.Init();
            _stateManager = this.StateManager;
            _stateManager.TransactionChanged += this.OnTransactionChangedHadler;
            _stateManager.StateManagerChanged += this.OnStateManagerChangeHandler;
        }



        /// <summary>
        /// 可靠状态管理器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStateManagerChangeHandler(object sender, NotifyStateManagerChangedEventArgs e)
        {
            // throw new NotImplementedException();
        }


        /// <summary>
        /// 事务通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTransactionChangedHadler(object sender, NotifyTransactionChangedEventArgs e)
        {
            if (e.Action == NotifyTransactionChangedAction.Commit)
            {
                this.lastCommitLsn = e.Transaction.CommitSequenceNumber;
                this.lastTransactionId = e.Transaction.TransactionId;

                //TODO 发生改变时 进行通知

                //this.lastCommittedTransactionList.Add(e.Transaction.TransactionId);
            }
        }

        /// <summary>
        /// 这是服务副本的主入口点。
        /// 在此服务副本成为主服务并具有写状态时，将执行此方法。
        /// </summary>
        /// <param name="cancellationToken">已在 Service Fabric 需要关闭此服务副本时取消。</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            //尝试进行备份    //BackupOption.Full 完整备份
            //BackupDescription myBackupDescription = new BackupDescription(BackupOption.Full, this.BackupCallbackAsync);

            //await this.BackupAsync(myBackupDescription);


            // TODO: 将以下示例代码替换为你自己的逻辑 
            //       或者在服务不需要此 RunAsync 重写时删除它。
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

        }

        class RequestMsg
        {
            public WSResponseMsgID send;
            public Func<Account, UserRole, string, byte[], Task<BaseResponseData>> handle;
            public bool isAccount;//是否需要登录
            public bool isRole;//是否需要用户
        }
        Dictionary<WSRequestMsgID, RequestMsg> dicRequest = new Dictionary<WSRequestMsgID, RequestMsg>()
        {
            { WSRequestMsgID.ConnectingReq, new RequestMsg(){ send = WSResponseMsgID.ConnectingResult, handle =  Connecting, isAccount = false, isRole = false }},//连接
            { WSRequestMsgID.CreateRoleReq, new RequestMsg(){ send = WSResponseMsgID.CreateRoleResult, handle =  CreateNewRole, isAccount = true, isRole = false }}, //创建角色
            { WSRequestMsgID.JoinGameReq, new RequestMsg(){ send = WSResponseMsgID.JoinGameResult,     handle =  JoinGame, isAccount = false, isRole = false }},//进入游戏
            { WSRequestMsgID.UseItemReq,new RequestMsg(){ send = WSResponseMsgID.UseItemResult,handle = UseItem,isAccount = false ,isRole = true } },//使用物品
             { WSRequestMsgID.SellItemReq,new RequestMsg(){ send = WSResponseMsgID.SellItemResult,handle = SellItem,isAccount =false,isRole = true } },  //出售道具
            {WSRequestMsgID.ChangeAvatarReq,new RequestMsg(){ send = WSResponseMsgID.ChangeAvatarResult,handle = ChangeAvater,isAccount =false,isRole=true} },  //换装
            { WSRequestMsgID.CreateCompanyReq,new RequestMsg(){ send = WSResponseMsgID.CreateCompanyResult,handle=CreateCompany,isAccount=false,isRole=true} }, //创建公司
            {WSRequestMsgID.CompanyLvUpReq,new RequestMsg(){ send = WSResponseMsgID.CompanyLvUpResult,handle=CompanyLvUp,isAccount=false,isRole=true} },//公司升级
            { WSRequestMsgID.DepartmentUpdateReq,new RequestMsg(){ send = WSResponseMsgID.DepartmentUpdateResult,handle = DepartmentLvUp,isAccount=false,isRole=true} },
            #if DEBUG   
            { WSRequestMsgID.AddItemReq,new RequestMsg(){send=WSResponseMsgID.AddItemResult,handle=AddItem,isAccount=false,isRole=true } }, //添加道具
            //{ },
#endif
            
           
           // { WSRequestMsgID.RemoveItemReq,new RequestMsg(){ send = WSResponseMsgID.RemoveItemResult,handle = RemoveItem,isAccount =false,isRole = true } },    //删除道具
            //{ WSRequestMsgID.DeleteRoleReq, new RequestMsg(){ send = WSResponseMsgID.DeleteRoleResult, handle =  , isAccount = false, isRole = true }}, //删除角色
        };

        private static async Task<BaseResponseData> DepartmentLvUp(Account account, UserRole role, string sid, byte[] bytes)
        {
            DepartmentUpdateResult result = new DepartmentUpdateResult();
            if (bytes == null)
            {
                result.Result = WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<DepartmentUpdateReq>(bytes);
            if (data == null)
            {
                result.Result = WsResult.ParamsError;
                return result;
            }
            var type = (DepartMentType)data.Type;
            return await CompanyController.Instance.DepartmentLvUp(_stateManager, role, result, type);
        }

        private static async Task<BaseResponseData> CompanyLvUp(Account account, UserRole role, string sid, byte[] bytes)
        {
            CompanyLvUpResult result = new CompanyLvUpResult();

            return await CompanyController.Instance.CompanyLvUp(_stateManager, role, result);
        }

        private static async Task<BaseResponseData> CreateCompany(Account account, UserRole role, string sid, byte[] bytes)
        {
            CreateCompanyResult result = new CreateCompanyResult();
            if (bytes == null)
            {
                result.Result = WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<CreateCompanyReq>(bytes);
            if (data == null)
            {
                result.Result = WsResult.ParamsError;
                return result;
            }
            var name = data.Name;

            return await CompanyController.Instance.CreateCompany(_stateManager, role, name, result);

        }


        /// <summary>
        /// 换装
        /// </summary>
        /// <param name="account"></param>
        /// <param name="role"></param>
        /// <param name="sid"></param>
        /// <param name="bytes">时装id数组</param>
        /// <returns></returns>
        private static async Task<BaseResponseData> ChangeAvater(Account account, UserRole role, string sid, byte[] bytes)
        {
            ChangeAvatarResult result = new ChangeAvatarResult();
            if (bytes == null)
            {
                result.Result = WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<ChangeAvatarReq>(bytes);
            if (data == null)
            {
                result.Result = WsResult.ParamsError;
                return result;
            }
            var id = data.Id;
            return await BagController.GetBagHandler().ChangeAvatar(_stateManager, role, id, result);
        }

        /// <summary>
        /// 出售物品
        /// </summary>
        /// <param name="account"></param>
        /// <param name="role"></param>
        /// <param name="sid"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static async Task<BaseResponseData> SellItem(Account account, UserRole role, string sid, byte[] bytes)
        {
            SellItemResult result = new SellItemResult();
            if (bytes == null)
            {
                result.Result = WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<SellItemReq>(bytes);
            if (data == null)
            {
                result.Result = WsResult.ParamsError;
                return result;
            }
            var id = data.ItemId;
            var count = data.Count;
            if (count <= 0)
            {
                result.Result = WsResult.PositiveInteger;
                return result;
            }
            return await BagController.GetBagHandler().SellItemsAsync(_stateManager, role.Id, id, count);
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="account"></param>
        /// <param name="role"></param>
        /// <param name="sid"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static async Task<BaseResponseData> AddItem(Account account, UserRole role, string sid, byte[] bytes)
        {
            try
            {
                AddItemResult result = new AddItemResult();
                if (bytes == null)
                {
                    result.Result = WsResult.ParamsError;
                    return result;
                }
                var data = await InitHelpers.GetPse().DeserializeAsync<AddItemReq>(bytes);
                if (data == null)
                {
                    result.Result = WsResult.ParamsError;
                    return result;
                }
                var id = data.ItemId;
                var count = data.Count;
                return await BagController.GetBagHandler().AddItemToRoleBag(_stateManager, role.Id, id, count);
            }
            catch (Exception ex)
            {
                //日志
                throw ex;
            }
        }


        /// <summary>
        /// 使用道具
        /// </summary>
        /// <param name="account"></param>
        /// <param name="role"></param>
        /// <param name="sid"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static async Task<BaseResponseData> UseItem(Account account, UserRole role, string sid, byte[] bytes)
        {
            UseItemResult result = new UseItemResult();
            if (bytes == null)
            {
                result.Result = WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<UseItemReq>(bytes);
            if (data == null)
            {
                result.Result = WsResult.ParamsError;
                return result;
            }
            var id = data.ItemId;
            var count = data.Count;
            return await BagController.GetBagHandler().UseItemsAsync(_stateManager, role.Id, id, count);
        }


        /// <summary>
        /// 处理文本消息
        /// </summary>
        /// <param name="wsrequest"></param>
        /// <param name="sessionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ProcessWsMessageAsync(string sessionId, CancellationToken cancellationToken)
        {
            var role = await DataHelper.GetOnlineRoleBySessionAsync(this.StateManager, sessionId);
            await LevelUp(role);
        }
        private static async Task LevelUp(UserRole role)
        {
            await DataHelper.RoleLevelUpAsync(_stateManager, role.Id);
        }
        /// <summary>
        /// IWebSocketListener.ProcessWsMessageAsync 二进制消息分发过来进行处理
        /// </summary>
        public async Task<byte[]> ProcessWsMessageAsync(byte[] wsrequest, string sessionId, CancellationToken cancellationToken)
        {
            Logger.Debug(nameof(this.ProcessWsMessageAsync));
            WsRequestMessage mrequest = await InitHelpers.GetMse().DeserializeAsync<WsRequestMessage>(wsrequest);
            WsResponseMessage mresponse = new WsResponseMessage();
            mresponse.Result = (int)WsResult.Success;
            if (dicRequest.ContainsKey((WSRequestMsgID)mrequest.MsgId))
            {
                Account account = null;
                UserRole role = null;
                var info = dicRequest[(WSRequestMsgID)mrequest.MsgId];
                if (info.isAccount) //账号required
                {
                    account = await DataHelper.GetAccountBySessionAsync(this.StateManager, sessionId);
                    if (account == null)
                    {
                        mresponse.Result = (int)WsResult.NotAccount;
                        return await InitHelpers.GetMse().SerializeAsync(mresponse);
                    }
                }
                if (info.isRole)    //角色required
                {
                    role = await DataHelper.GetOnlineRoleBySessionAsync(this.StateManager, sessionId);
                    if (role == null)
                    {
                        mresponse.Result = (int)WsResult.NotRole;
                        return await InitHelpers.GetMse().SerializeAsync(mresponse);
                    }
                }
                if (mresponse.Result == (int)WsResult.Success)  //成功返回值
                {
                    var ret = await info.handle(account, role, sessionId, mrequest.Data);
                    mresponse.Result = (int)ret.Result;
                    mresponse.MsgId = (int)info.send;
                    mresponse.Value = await InitHelpers.GetMse().SerializeAsync(ret);
                }
            }
            else
            {
                mresponse.Result = (int)WsResult.NoneActionFunc;
            }
            return await InitHelpers.GetMse().SerializeAsync(mresponse);
        }


        public async Task<byte[]> ProcessWsMessageAsync1(byte[] wsrequest, string session, CancellationToken cancellationToken)
        {
            Logger.Debug(nameof(this.ProcessWsMessageAsync));
            WsRequestMessage mrequest = await InitHelpers.GetMse().DeserializeAsync<WsRequestMessage>(wsrequest);
            WsResponseMessage mresponse = new WsResponseMessage();
            mresponse.Result = (int)WsResult.Success;
            if (dicRequest.ContainsKey((WSRequestMsgID)mrequest.MsgId))
            {
                Account account = null;
                UserRole role = null;
                var info = dicRequest[(WSRequestMsgID)mrequest.MsgId];
                if (info.isAccount) //账号required
                {
                    account = await DataHelper.GetAccountBySessionAsync(this.StateManager, session);
                    if (account == null)
                    {
                        mresponse.Result = (int)WsResult.NotAccount;
                        return await InitHelpers.GetMse().SerializeAsync(mresponse);
                    }
                }
                if (info.isRole)    //角色required
                {
                    role = await DataHelper.GetOnlineRoleBySessionAsync(this.StateManager, session);
                    if (role == null)
                    {
                        mresponse.Result = (int)WsResult.NotRole;
                        return await InitHelpers.GetMse().SerializeAsync(mresponse);
                    }
                }
                if (mresponse.Result == (int)WsResult.Success)  //成功返回值
                {
                    var ret = await info.handle(account, role, session, mrequest.Data);
                    mresponse.Result = (int)ret.Result;
                    mresponse.MsgId = (int)info.send;
                    mresponse.Value = await InitHelpers.GetMse().SerializeAsync(ret);
                }
            }
            else
            {
                mresponse.Result = (int)WsResult.NoneActionFunc;
            }
            return await InitHelpers.GetMse().SerializeAsync(mresponse);

        }

        /// <summary>
        /// 进入游戏 初始化角色和场景数据
        /// need TODO 场景数据
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private static async Task<BaseResponseData> JoinGame(Account account, UserRole role, string sessionId, byte[] bytes)
        {
            var result = new JoinGameResult();
            var data = new JoinGameReq();
            if (bytes != null)
            {
                data = await InitHelpers.GetPse().DeserializeAsync<JoinGameReq>(bytes); if (data == null)
                {
                    //请求参数为空
                    result.Result = WsResult.RoleIdIsNull;
                    return result;
                }
            }
            else
            {
                //请求参数为空
                result.Result = WsResult.RoleIdIsNull;
                return result;
            }


            var roleId = new Guid(data.RoleId);
            var user = await DataHelper.GetRoleInfoByRoleIdAsync(_stateManager, roleId);
            if (user != null) //找到角色数据 绑定在线状态  返回数据给用户  设置状态为登录状态
            {
                //查找session用来绑定
                //查找用户是否在线
                //var online = await DataHelper.FindOnlinePlayerByRoleIdAsync(_stateManager, user.Id);
                var sid = await DataHelper.GetOnlineSessionByRoleIdAsync(_stateManager, roleId);    //查找当前连接是否有角色
                if (sid != null) //有旧的session
                {
                    if (sid.Equals(sessionId))   //检查session保存的roleid是不是和新登入的一致
                    {   //session一致  有可能是断线重连

                    }
                    else
                    {
                        //不一致  可能是从别的地方连接
                        await DataHelper.RemoveOnlineRoleBySessionAsync(_stateManager, sid);                //移除旧的
                        await DataHelper.RemoveOnlineRoleBySessionAsync(_stateManager, sessionId);
                        await DataHelper.BindOnlineRoleBySessionAsync(_stateManager, sessionId, user);      //绑定新的新session绑定
                    }
                    //用户已经在线  进行通知处理  匹配 imei处理 预防盗号处理
                    //TODO
                    await DataHelper.UpdateOnlineSessionByRoleIdAsync(_stateManager, roleId, sessionId, sid);    //用新账号绑定新的session
                }
                else
                {   //没有旧的session
                    await DataHelper.RemoveOnlineRoleBySessionAsync(_stateManager, sessionId);  //尝试移除
                    await DataHelper.BindOnlineRoleBySessionAsync(_stateManager, sessionId, user);  //添加角色进入在线表 session匹配
                    //切记 下线时候要删除  或者在心跳包收不到的情况下     也要删除
                    await DataHelper.BindOnlineSessionByRoleIdAsync(_stateManager, roleId, sessionId);
                }
                //构造返回值
                List<Model.ResponseData.UserAttr> u = new List<Model.ResponseData.UserAttr>();
                foreach (var item in user.UserAttr)
                {
                    u.Add(new Model.ResponseData.UserAttr()
                    {
                        Count = item.Count,
                        UserAttrID = item.UserAttrID
                    });
                }
                //Load Bag info
                var BagInfo = await DataHelper.GetRoleBagByRoleIdAsync(_stateManager, user.Id);
                result.CompanyInfo = await CompanyController.Instance.GetCompanyInfoByRoleId(_stateManager, user.Id);
                result.DepartInfoInfo = await CompanyController.Instance.GetDepartmentInfoByRoleId(_stateManager, user.Id);
                if (BagInfo != null)
                {
                    // BagInfo = await DataHelper.ResetUserBagByRoleIdAsync(_stateManager, user.Id);

                    result.UserAttr = u;
                    result.Affinity = user.Affinity;
                    result.Avatar = user.Avatar;
                    result.Certificates = user.Certificates;
                    result.CertificatesExp = user.CertificatesExp;
                    result.Charm = user.Charm;
                    result.Concentration = user.Concentration;
                    result.Constitution = user.Constitution;
                    result.Desc = user.Desc;
                    result.Gold = user.Gold;
                    result.Exp = user.Exp;
                    result.Icon = user.Icon;
                    result.RoleId = user.Id.ToString();
                    result.Intelligence = user.Intelligence;
                    result.Level = user.Level;
                    result.Name = user.Name;
                    result.Sex = user.Sex;
                    result.SocialStatus = user.SocialStatus;
                    result.Type = user.Type;
                    result.VipLevel = user.VipLevel;
                    result.RoleBag = new BagInfo()
                    {
                        CurUsedCell = BagInfo.CurUsedCell,
                        MaxCellNumber = BagInfo.MaxCellNumber,
                        Items = GetRoleItems(BagInfo.Items)
                    };
                }
            }
            return result;
        }


        /// <summary>
        /// 构造背包数据
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static List<RoleItems> GetRoleItems(List<Model.Data.General.Item> items)
        {
            var result = new List<RoleItems>();
            if (items.Any())
            {
                foreach (var item in items)
                {
                    result.Add(new RoleItems()
                    {
                        Id = item.Id,
                        CurCount = item.CurCount,
                        OnSpace = item.OnSpace,
                    });
                    //#endif
                }
            }
            return result;
        }

        /// <summary>
        /// 创建新角色
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="name"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        private static async Task<BaseResponseData> CreateNewRole(Account account, UserRole role, string session, byte[] bytes)//string sessionId, string name, int sex)
        {
            CreateRoleResult result = new CreateRoleResult();
            var data = await InitHelpers.GetPse().DeserializeAsync<CreateRoleReq>(bytes);
            var name = data.Name;
            var sex = data.Sex;
            if (string.IsNullOrEmpty(name))
            {
                result.Result = WsResult.ParamsError;
                return result;
            }
            var allRoles = await DataHelper.GetRoleListsByAccountIdAsync(_stateManager, account.AccountID);
            //检查是否有角色
            if (allRoles != null)
            {
                //检查可以创建的角色 暂定允许创建10个角色
                if (account.RoleNumber >= 10)
                {
                    result.ErrorDesc = "角色数量过多，请删除部分角色再创建";
                    return result;
                }
                //检查是否重名
                if (await DataHelper.CheckRoleNameIsExistsByNameAsync(_stateManager, name))
                {
                    result.ErrorDesc = "角色名重复";
                    return result;  //重名直接返回
                }
                //构造新角色
                UserRole user = new UserRole(sex, name, account.AccountID);

                IdRole idRole = new IdRole(user.Name, user.Id);//构造角色名和id绑定表

                await DataHelper.SetRoleNameBindRoleIdAsync(_stateManager, name, idRole); //保存id和角色绑定列表
                await DataHelper.BindRoleInfoByRoleIdAsync(_stateManager, user.Id, user);  //保存角色表 

                //await DataHelper.InitRoleInfo(_stateManager, user.Id);  //初始化角色属性

                List<UserRole> roles = new List<UserRole>();
                roles.AddRange(allRoles);           //把原来的角色取出来 
                roles.Add(user);                //加入一个新角色  
                                                //更新数据
                                                //如果是增加角色 最后一个参数为true
                await DataHelper.UpdateRoleListsByAccountId(_stateManager, account.AccountID, roles, true);

                //保存新角色

                //一切正常的返回值
                result.ErrorDesc = "";
                result.Name = name;
                result.Sex = sex;
                result.RoleId = user.Id.ToString();
            }
            else
            {
                ///没有角色，新增一个
                //检查重名
                if (await DataHelper.CheckRoleNameIsExistsByNameAsync(_stateManager, name))
                {
                    result.ErrorDesc = "角色名重复";
                    return result;  //重名直接返回
                }
                //判断男女
                var character = GetDefaultCharacter(sex);
                //构造新角色
                UserRole user = new UserRole(sex, name, account.AccountID);
                IdRole idRole = new IdRole(user.Name, user.Id);//构造角色名和id绑定表


                await DataHelper.SetRoleNameBindRoleIdAsync(_stateManager, name, idRole); //保存id和角色绑定列表
                await DataHelper.BindRoleInfoByRoleIdAsync(_stateManager, user.Id, user);  //保存角色表 

                //await DataHelper.InitRoleInfo(_stateManager, user.Id);
                //TODO //初始化角色属性
                List<UserRole> roles = new List<UserRole>();
                roles.Add(user);

                await DataHelper.AddNewRoleListsByAccountIdAsync(_stateManager, account.AccountID, roles);  //保存角色表 

                //保存新角色

                //一切正常的返回值
                result.ErrorDesc = "";
                result.Name = name;
                result.Sex = sex;
                result.RoleId = user.Id.ToString();
            }
            return result;
        }




        /// <summary>
        /// 对连接服务器请求做出回应  绑定账号 并且给出当前账号下面的角色数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        private static async Task<BaseResponseData> Connecting(Account account, UserRole role, string sessionId, byte[] bytes)//ConnectingReq data, string sessionId)
        {
            ConnectingResult result = new ConnectingResult();
            var data = await InitHelpers.GetPse().DeserializeAsync<ConnectingReq>(bytes);
            var signToken = data.Token;
            if (signToken == null)
            {
                result.Result = WsResult.ParamsError;
                return result;
            }
            result.RoleLists = new List<RoleLists>();
            var token = await DataHelper.GetTokenBySignTokenAsync(_stateManager, signToken);    //取得token
            if (token != null)
            {
                //判断token时效
                if (token.ExpireTime >= DateTime.Now)
                {
                    var account1 = await DataHelper.GetAccountByUserNameAsync(_stateManager, token.UserPassport);
                    //token有效 并且账号存在 尝试获取session是否存在  用来判断是否已经绑定session


                    var sid = await DataHelper.GetSessionByUserNameAsync(_stateManager, token.UserPassport);
                    var act = await DataHelper.GetUserNameBySessionAsync(_stateManager, sessionId);


                    //检查 账号和上次保存的账号是否匹配
                    if (!string.IsNullOrEmpty(act) && !string.IsNullOrEmpty(sid))
                    {//都存在 
                        if (DataHelper.CheckPassport(act, token.UserPassport) && DataHelper.CheckSession(sessionId, sid))
                        {   //都相同
                            //直接给登录
                            //TODO：踢下线 之后直接允许登录  或者允许重连
                            goto EqualsReturn;
                        }
                    }
                    if (!string.IsNullOrEmpty(act)) //表示session下面已经绑定账号了
                    {
                        //找到账号
                        if (!DataHelper.CheckPassport(act, token.UserPassport))
                        {   //账号不相同
                            await DataHelper.RemoveUserNameBySession(_stateManager, sessionId);
                            await DataHelper.BindUserNameBySessionAsync(_stateManager, sessionId, token.UserPassport);
                            await DataHelper.RemoveSessionByUserName(_stateManager, token.UserPassport);
                            await DataHelper.BindSessionByUserName(_stateManager, token.UserPassport, sessionId);
                        }
                        else
                        {   //账号相同
                            await DataHelper.UpdateSessionByUserNameAsync(_stateManager, token.UserPassport, sessionId); //更新session
                            if (!string.IsNullOrEmpty(sid))
                            {
                                await DataHelper.RemoveUserNameBySession(_stateManager, sid);
                                await DataHelper.BindUserNameBySessionAsync(_stateManager, sessionId, token.UserPassport);
                            }
                        }
                    }
                    else
                    {   //没找到账号
                        //session没绑定账号
                        //查找账号是否绑定session
                        if (!string.IsNullOrEmpty(sid))
                        {
                            //旧session存在  账号已经绑定session
                            await DataHelper.RemoveSessionByUserName(_stateManager, token.UserPassport);
                            await DataHelper.BindSessionByUserName(_stateManager, token.UserPassport, sessionId);
                            await DataHelper.BindUserNameBySessionAsync(_stateManager, sessionId, token.UserPassport);
                        }
                        else
                        {//都不存在 直接绑定
                            await DataHelper.BindSessionByUserName(_stateManager, token.UserPassport, sessionId);   //给账号绑定 session
                            await DataHelper.BindUserNameBySessionAsync(_stateManager, sessionId, token.UserPassport);     //绑定
                        }

                    }
                    EqualsReturn:
                    //给予用户当前角色列表数据
                    var roles = await DataHelper.GetRoleListsByAccountIdAsync(_stateManager, account1.AccountID);
                    if (roles != null)
                    {
                        if (roles.Any())
                        {//有角色 构造返回值
                            foreach (var item in roles)
                            {
                                result.RoleLists.Add(new RoleLists()
                                {
                                    Name = item.Name,
                                    RoleId = item.Id.ToString(),
                                    Sex = item.Sex
                                });
                            }
                        }
                        else
                        {
                            result.Result = WsResult.NotRole;
                            //无角色 返回-5
                            return result;
                        }
                    }
                    else
                    {
                        result.Result = WsResult.NotRole;
                        //无角色 返回-5
                        return result;
                    }
                    //当用户下线时 删除 token
                }

                else
                {
                    result.Result = WsResult.TokenTimeOut;
                    //token 超时
                    return result;
                }
            }
            else
            {
                result.Result = WsResult.TokenIsNotExists;
                //token不存在
                //拒绝连接
                return result;
            }
            //TODO: 修正返回值
            return result;
        }

        private static async Task SessionBindUserPort(string oldSid, string newSid, string oldAct, string newAct)
        {
            if (!string.IsNullOrEmpty(oldAct))
                await DataHelper.RemoveSessionByUserName(_stateManager, oldAct);   //尝试移除
            if (!string.IsNullOrEmpty(newAct))
                await DataHelper.RemoveSessionByUserName(_stateManager, newAct);   //尝试移除
            if (!string.IsNullOrEmpty(oldSid))
                await DataHelper.RemoveUserNameBySession(_stateManager, oldSid);  //不管session有没有 都尝试移除 移除session绑定的账号 重新绑定
            if (!string.IsNullOrEmpty(newSid))
                await DataHelper.RemoveUserNameBySession(_stateManager, newSid);  //不管session有没有 都尝试移除 移除session绑定的账号 重新绑定

            await DataHelper.BindSessionByUserName(_stateManager, oldAct, newSid);   //给账号绑定session
            await DataHelper.BindUserNameBySessionAsync(_stateManager, newSid, oldAct);//绑定
        }


        /// <summary>
        ///可选择性地替代以创建侦听器(如 HTTP、服务远程、WCF 等)，从而使此服务副本可处理客户端或用户请求。
        /// </summary>
        /// <remarks>
        ///有关服务通信的详细信息，请参阅 https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>侦听器集合。</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new List<ServiceReplicaListener>()
            {
                new ServiceReplicaListener(
                    (context) =>
                        this.CreateServiceRemotingListener(context)),
                new ServiceReplicaListener(
                    context=>
                    new WebSocketListener("WsServiceEndpoint","LogicServerWS",context,()=>this),
                ServiceConst.ListenerWebsocket)
            };
        }






        #region 账号服务器相关
        /// <summary>
        /// 正常注册
        /// </summary>
        /// <param name="pid">账号</param>
        /// <param name="pwd">密码</param>
        /// <param name="imei">设备码</param>
        /// <param name="retailID">第三方id</param>
        /// <param name="mobileType">手机类型</param>
        /// <param name="ipAddress">ip地址</param>
        /// <returns>token</returns>
        public async Task<AccountResult> Register(string pid, string pwd, string imei, string retailID, byte mobileType, string ipAddress)
        {
            Account newAccount = new Account(imei, mobileType, ipAddress)
            {
                AccountID = Guid.NewGuid(),
                Password = pwd,
                UserName = pid,
            };

            //首先查找pid是否存在
            var oldAccount = await DataHelper.GetAccountByUserNameAsync(this.StateManager, pid);
            if (oldAccount != null)
            {
                //直接返回账号已存在
                return new AccountResult()
                {
                    Status = ARsult.AccountIsExists,    //账号已存在
                    TokenResult = null
                };
            }
            //不存在 新增账号 保存
            await DataHelper.BindAccountByUserNameAsync(this.StateManager, pid, newAccount);
            await DataHelper.BindAccountByAccountIdAsync(this.StateManager, newAccount.AccountID, newAccount);
            Login login = new Login()
            {
                AccountId = newAccount.AccountID,
                Pid = pid
            };
            await DataHelper.BindLoginInfoByUserNameAsync(this.StateManager, pid, login); //添加登录数据接口
            if (!string.IsNullOrEmpty(imei))
            {   //保存一键注册接口
                Passport passport = new Passport()
                {
                    AccountID = newAccount.AccountID,
                    IMEI = imei,
                    PassportID = newAccount.UserName
                };
                await DataHelper.BindPassportByImeiAsync(this.StateManager, imei, passport);
            }
            return new AccountResult()
            {
                Status = ARsult.Ok,
                TokenResult = null
            };

        }

        /// <summary>
        /// 一键注册
        /// </summary>
        /// <param name="imei">设备码</param>
        /// <returns>token</returns>
        public async Task<AccountResult> Passport(string imei)
        {


            var pot = await DataHelper.GetPassportByImeiAsync(this.StateManager, imei); //找通行证
            if (pot != null)
            {
                var account = await DataHelper.GetAccountByUserNameAsync(this.StateManager, pot.PassportID);
                if (account != null)
                {
                    //账号存在 直接返回
                    return new AccountResult()
                    {
                        Status = ARsult.AccountIsExists,
                        TokenResult = null
                    };
                }
            }
            else
            {
                //没找到 自动注册一个
                Account account = new Account()
                {
                    AccountID = Guid.NewGuid(),
                };
                Passport passport = new Passport()
                {
                    AccountID = account.AccountID,
                    IMEI = imei,
                    PassportID = account.AccountID.ToString()
                };
                Login login = new Login()
                {
                    AccountId = account.AccountID,
                    Pid = imei
                };
                await DataHelper.BindPassportByImeiAsync(this.StateManager, imei, passport);//注册passport表
                await DataHelper.BindAccountByUserNameAsync(this.StateManager, account.UserName, account); //注册账号表
                await DataHelper.BindAccountByAccountIdAsync(this.StateManager, account.AccountID, account); //注册账号表
                await DataHelper.BindLoginInfoByUserNameAsync(this.StateManager, imei, login);//注册登录表
                return new AccountResult()
                {
                    Status = ARsult.Ok
                };

            }
            return null;
        }

        /// <summary>
        /// 非第三方登录 账号和设备码必选其一
        /// </summary>
        /// <param name="pid">账号</param>
        /// <param name="pwd">密码</param>
        /// <param name="imei">设备码</param>
        /// <returns></returns>
        public async Task<AccountResult> Login(string pid, string pwd, string imei, string ip)
        {
            //可以使用账号登录，也可以使用设备码登录
            if (!string.IsNullOrEmpty(imei))
            {       //使用设备码登录
                    //查询登录表中是否有数据

                var login = await DataHelper.GetLoginInfoByUserNameAsync(this.StateManager, imei);
                if (login != null)
                {
                    var account = await DataHelper.GetAccountByUserNameAsync(this.StateManager, login.Pid);
                    if (account != null)
                    {   //账号存在
                        account.LastLoginTime = DateTime.Now;
                        await DataHelper.UpdateAccountByUserNameAsync(this.StateManager, account.UserName, account);  //更新账号登录时间
                        Token token = new Token()
                        {
                            SignToken = Guid.NewGuid().ToString(),
                            TokenID = account.AccountID,
                            UserPassport = account.UserName
                        };
                        await DataHelper.BindTokenBySignTokenAsync(this.StateManager, token.SignToken, token);

                        return new AccountResult()
                        {
                            TokenResult = token
                        };
                    }

                }

            }
            else
            {
                //设备码为空 使用账号密码登录
                if (!string.IsNullOrEmpty(pid) && !string.IsNullOrEmpty(pwd))
                {
                    //验证账号密码

                    var login = await DataHelper.GetLoginInfoByUserNameAsync(this.StateManager, pid);
                    if (login != null)
                    {
                        var userName = login.Pid;
                        var account = await DataHelper.GetAccountByUserNameAsync(this.StateManager, userName);
                        if (account != null)
                        {
                            //比对密码
                            if (pwd.Equals(account.Password))
                            {
                                //密码正确 返回token
                                //更新 token
                                Token token = new Token()
                                {
                                    SignToken = Guid.NewGuid().ToString(),
                                    TokenID = account.AccountID,
                                    UserPassport = pid
                                };
                                await DataHelper.BindTokenBySignTokenAsync(this.StateManager, token.SignToken, token);
                                return new AccountResult()
                                {
                                    TokenResult = token
                                };
                            }
                            else
                            {
                                //密码不对
                                return new AccountResult()
                                {
                                    Status = ARsult.PassWordError
                                };
                            }

                        }
                        //没找到账号 返回
                        //TODO 需要标记状态
                        return new AccountResult()
                        {
                            Status = ARsult.NoneAccount
                        };
                    }
                    return new AccountResult()
                    {
                        Status = ARsult.NoneAccount
                    };
                }
            }
            return new AccountResult()
            {
                Status = ARsult.ServerError
            };

        }
        #endregion


        #region helpers
        private static Character GetDefaultCharacter(int sex)
        {
            return Character.GetForId(sex);
        }



        #endregion
    }
}