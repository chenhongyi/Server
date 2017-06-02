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
using Microsoft.ServiceFabric.Data.Collections;
using Model.Protocol;
using LogicServer.Data.Helper;
using System.Runtime.Remoting.Messaging;

namespace LogicServer
{
    /// <summary>
    /// 通过 Service Fabric 运行时为每个服务副本创建此类的一个实例。
    /// </summary>
    internal sealed class LogicServer : StatefulService, ILogicServer, IWebSocketConnectionHandler
    {
        public static LogicServer Instance;

        private static IReliableStateManager _stateManager;
        private static readonly ILogger Logger = LoggerFactory.GetLogger(nameof(LogicServer));

        private long lastCommitLsn;
        private long lastTransactionId;

        public LogicServer(StatefulServiceContext context)
            : base(context)
        {
            Instance = this;

            TxtReader.Init();
            _stateManager = this.StateManager;
            _stateManager.TransactionChanged += this.OnTransactionChangedHadler;
            _stateManager.StateManagerChanged += this.OnStateManagerChangeHandler;
        }

        #region 当前唯一线程内角色
        public class CurrUser
        {
            public Account account = new Account();
            public UserRole role = new UserRole();
            public string sessionId;
            public byte[] bytes;
            public Bag bag = new Bag();
            //  public static CurrUser Empty = new CurrUser();
        }

        #endregion


        private static void SetRole(string sessionId, Account account, UserRole role, byte[] data, Bag bag)
        {
            var user = User;
            user.role = role;
            user.account = account;
            user.sessionId = sessionId;
            user.bytes = data;
            user.bag = bag;
        }

        const string UserString = "user";
        public static CurrUser User
        {
            get
            {
                CurrUser _User = (CurrUser)CallContext.LogicalGetData(UserString);
                if (_User == null)
                {
                    _User = new CurrUser();
                    CallContext.LogicalSetData(UserString, _User);
                }
                return _User;
            }
        }

        /// <summary>
        /// 可靠状态管理器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStateManagerChangeHandler(object sender, NotifyStateManagerChangedEventArgs e)
        {
            if (e.Action == NotifyStateManagerChangedAction.Rebuild)
            {
                // this.ProcessStataManagerRebuildNotification(e);

                return;
            }

            //   this.ProcessStateManagerSingleEntityNotification(e);
        }
        // private void ProcessStateManagerSingleEntityNotification(NotifyStateManagerChangedEventArgs e)
        // {
        //     var operation = e as NotifyStateManagerSingleEntityChangedEventArgs;

        //     if (operation.Action == NotifyStateManagerChangedAction.Add)
        //     {
        //         if (operation.ReliableState is IReliableDictionary<TKey, TValue>)
        //         {
        //             var dictionary = (IReliableDictionary<TKey, TValue>)operation.ReliableState;
        //             dictionary.RebuildNotificationAsyncCallback = this.OnDictionaryRebuildNotificationHandlerAsync;
        //             dictionary.DictionaryChanged += this.OnDictionaryChangedHandler;
        //         }
        //     }
        // }
        // public async Task OnDictionaryRebuildNotificationHandlerAsync(
        //IReliableDictionary<TKey, TValue> origin,
        //NotifyDictionaryRebuildEventArgs<TKey, TValue> rebuildNotification)
        // {
        //     this.secondaryIndex.Clear();

        //     var enumerator = e.State.GetAsyncEnumerator();
        //     while (await enumerator.MoveNextAsync(CancellationToken.None))
        //     {
        //         this.secondaryIndex.Add(enumerator.Current.Key, enumerator.Current.Value);
        //     }
        // }


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

                // this.lastCommittedTransactionList.Add(e.Transaction.TransactionId);
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
            public Func<Task<BaseResponseData>> handle;
            public bool ndAccount = true;//是否需要登录
            public bool ndRole = true;//是否需要用户
            public bool ndBag = true;
        }
        Dictionary<WSRequestMsgID, RequestMsg> dicRequest = new Dictionary<WSRequestMsgID, RequestMsg>()
        {
            { WSRequestMsgID.ConnectingReq, new RequestMsg(){ send = WSResponseMsgID.ConnectingResult, handle =  Connecting, ndAccount = false, ndRole = false,ndBag=false }},//连接
            { WSRequestMsgID.CreateRoleReq, new RequestMsg(){ send = WSResponseMsgID.CreateRoleResult, handle =  CreateNewRole, ndAccount = true, ndRole = false,ndBag=false }}, //创建角色
            { WSRequestMsgID.JoinGameReq, new RequestMsg(){ send = WSResponseMsgID.JoinGameResult,     handle =  JoinGame,ndRole=false, ndAccount = false,ndBag=false }},//进入游戏
            { WSRequestMsgID.UseItemReq,new RequestMsg(){ send = WSResponseMsgID.UseItemResult,handle = UseItem,ndAccount = false  } },//使用物品
             { WSRequestMsgID.SellItemReq,new RequestMsg(){ send = WSResponseMsgID.SellItemResult,handle = SellItem,ndAccount =false } },  //出售道具
            {WSRequestMsgID.ChangeAvatarReq,new RequestMsg(){ send = WSResponseMsgID.ChangeAvatarResult,handle = ChangeAvater,ndAccount =false} },  //换装
            { WSRequestMsgID.CreateCompanyReq,new RequestMsg(){ send = WSResponseMsgID.CreateCompanyResult,handle=CreateCompany,ndAccount=false} }, //创建公司
            {WSRequestMsgID.CompanyLvUpReq,new RequestMsg(){ send = WSResponseMsgID.CompanyLvUpResult,handle=CompanyLvUp,ndAccount=false} },//公司升级
            { WSRequestMsgID.DepartmentUpdateReq,new RequestMsg(){ send = WSResponseMsgID.DepartmentUpdateResult,handle = DepartmentLvUp,ndAccount=false} },    //升级部门
            { WSRequestMsgID.GetMapReq,new RequestMsg(){ send = WSResponseMsgID.GetMapResult,handle=GetMapCell,ndAccount = false,ndRole=false,ndBag=false} },   //请求地图信息
            { WSRequestMsgID.BuyLandReq,new RequestMsg(){ send = WSResponseMsgID.BuyLandResult,handle  = BuyLand,ndAccount =false} },  //购买土地
            { WSRequestMsgID.CreateBuildReq,new RequestMsg(){ send = WSResponseMsgID.CreateBuildResult, handle = CreateBuild,ndAccount = false} },//创建店铺
            { WSRequestMsgID.DestoryBuildReq,new RequestMsg(){ send = WSResponseMsgID.DestoryBuildResult,handle = DestoryBuild,ndAccount = false} },//摧毁店铺

            { WSRequestMsgID.RoomReq, new RequestMsg(){ send = WSResponseMsgID.RoomResult, handle = RoomController.Instance.GetRoom } },

            #if DEBUG   
            { WSRequestMsgID.AddItemReq,new RequestMsg(){send=WSResponseMsgID.AddItemResult,handle=AddItem,ndAccount=false} }, //添加道具
            //{ },
#endif
            
           
           // { WSRequestMsgID.RemoveItemReq,new RequestMsg(){ send = WSResponseMsgID.RemoveItemResult,handle = RemoveItem,isAccount =false,isRole = true } },    //删除道具
            //{ WSRequestMsgID.DeleteRoleReq, new RequestMsg(){ send = WSResponseMsgID.DeleteRoleResult, handle =  , isAccount = false, isRole = true }}, //删除角色
        };

        private static async Task<BaseResponseData> DestoryBuild()
        {
            DestoryBuildResult result = new DestoryBuildResult();
            if (User.bytes == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<DestoryBuildReq>(User.bytes);
            if (data == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            return await BuildController.Instance.DestoryBuild(data.Id, result);
        }

        private static async Task<BaseResponseData> CreateBuild()
        {
            CreateBuildResult result = new CreateBuildResult();
            if (User.bytes == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<CreateBuildReq>(User.bytes);
            if (data == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }



            //检查钱够不够
            var config = AutoData.Building.GetForId(data.ShopType);
            long needMoney = config.BuildingCost.Count;
            var moneyType = config.BuildingCost.GoldType;
            if (needMoney <= 0)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            if (!(BagController.Instance.CheckMoney(needMoney, moneyType)))
            {
                result.Result = GameEnum.WsResult.NotEnoughMoney;
                return result;
            }

            return await BuildController.Instance.CreateBuild(data, result);
        }

        private static async Task<BaseResponseData> BuyLand()
        {
            BuyLandResult result = new BuyLandResult();
            if (User.bytes == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<BuyLandReq>(User.bytes);
            if (data == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var landConfig = LandInfo.GetForLevel(data.Level);
            long needMoney = landConfig.Price.Count;
            var moneyType = landConfig.Price.CurrencyID;
            if (needMoney <= 0)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            if (!(BagController.Instance.CheckMoney(needMoney, moneyType)))
            {
                result.Result = GameEnum.WsResult.NotEnoughMoney;
                return result;
            }
            return await LandController.Instance.BuyLand(data.Pos, result);
        }





        /// <summary>
        /// 请求地图信息
        /// </summary>
        /// <param name="account"></param>
        /// <param name="role"></param>
        /// <param name="sid"></param>
        /// <param name="bytes">坐标数组</param>
        /// <returns></returns>
        private static async Task<BaseResponseData> GetMapCell()
        {
            GetMapResult result = new GetMapResult();
            if (User.bytes == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<GetMapReq>(User.bytes);
            if (data == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            return await LandController.Instance.GetLandCell(data.Pos, result);
        }

        private static async Task<BaseResponseData> DepartmentLvUp()
        {
            DepartmentUpdateResult result = new DepartmentUpdateResult();
            if (User.bytes == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<DepartmentUpdateReq>(User.bytes);
            if (data == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var type = (GameEnum.DepartMentType)data.Type;
            return await DepartmentController.Instance.DepartmentLvUp(result, type);
        }

        private static async Task<BaseResponseData> CompanyLvUp()
        {
            CompanyLvUpResult result = new CompanyLvUpResult();

            return await CompanyController.Instance.CompanyLvUp(result);
        }

        private static async Task<BaseResponseData> CreateCompany()
        {
            CreateCompanyResult result = new CreateCompanyResult();
            if (User.bytes == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<CreateCompanyReq>(User.bytes);
            if (data == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var name = data.Name;
            var role = LogicServer.User.role;
            return await CompanyController.Instance.CreateCompany(role.Id, name, result);

        }


        /// <summary>
        /// 换装
        /// </summary>
        /// <param name="account"></param>
        /// <param name="role"></param>
        /// <param name="sid"></param>
        /// <param name="bytes">时装id数组</param>
        /// <returns></returns>
        private static async Task<BaseResponseData> ChangeAvater()
        {
            ChangeAvatarResult result = new ChangeAvatarResult();
            if (User.bytes == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<ChangeAvatarReq>(User.bytes);
            if (data == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var id = data.Id;
            return await BagController.Instance.ChangeAvatar(id, result);
        }

        /// <summary>
        /// 出售物品
        /// </summary>
        /// <param name="account"></param>
        /// <param name="role"></param>
        /// <param name="sid"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static async Task<BaseResponseData> SellItem()
        {
            SellItemResult result = new SellItemResult();
            if (User.bytes == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<SellItemReq>(User.bytes);
            if (data == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var id = data.ItemId;
            var count = data.Count;
            if (count <= 0)
            {
                result.Result = GameEnum.WsResult.PositiveInteger;
                return result;
            }

            return await BagController.Instance.SellItemsAsync(id, count);
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="account"></param>
        /// <param name="role"></param>
        /// <param name="sid"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static async Task<BaseResponseData> AddItem()
        {
            try
            {
                AddItemResult result = new AddItemResult();
                if (User.bytes == null)
                {
                    result.Result = GameEnum.WsResult.ParamsError;
                    return result;
                }
                var data = await InitHelpers.GetPse().DeserializeAsync<AddItemReq>(User.bytes);
                if (data == null)
                {
                    result.Result = GameEnum.WsResult.ParamsError;
                    return result;
                }
                var id = data.ItemId;
                var count = data.Count;
                return await BagController.Instance.AddItemToRoleBag(id, count);
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
        private static async Task<BaseResponseData> UseItem()
        {
            UseItemResult result = new UseItemResult();
            if (User.bytes == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<UseItemReq>(User.bytes);
            if (data == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var id = data.ItemId;
            var count = data.Count;
            return await BagController.Instance.UseItemsAsync(_stateManager, User.role.Id, id, count);
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

        }


        public async Task<byte[]> ProcessWsMessageAsync(byte[] wsrequest, string session, CancellationToken cancellationToken)
        {
            Logger.Debug(nameof(this.ProcessWsMessageAsync));
            WsRequestMessage mrequest = await InitHelpers.GetMse().DeserializeAsync<WsRequestMessage>(wsrequest);
            WsResponseMessage mresponse = new WsResponseMessage();
            mresponse.Result = (int)WsResult.Success;
            if (dicRequest.ContainsKey((WSRequestMsgID)mrequest.MsgId))
            {
                Account account = null;
                UserRole role = null;
                Bag bag = null;
                var info = dicRequest[(WSRequestMsgID)mrequest.MsgId];
                if (info.ndAccount)
                {
                    account = await GetAccount(session);


                    //   account = await DataHelper.GetAccountBySessionAsync(this.StateManager, session);
                    if (account == null)
                    {
                        mresponse.Result = (int)WsResult.NotAccount;
                        return await InitHelpers.GetMse().SerializeAsync(mresponse);
                    }
                }
                if (info.ndRole)
                {
                    role = await RoleDataHelper.Instance.GetRoleBySidAsync(session);
                    //role = await DataHelper.GetOnlineRoleBySessionAsync(this.StateManager, session);
                    if (role == null)
                    {
                        mresponse.Result = (int)WsResult.NotRole;
                        return await InitHelpers.GetMse().SerializeAsync(mresponse);
                    }
                    bag = await BagDataHelper.Instance.GetBagByRoleId(role.Id);
                    if (bag == null)
                        bag = new Bag();
                }
                if (mresponse.Result == (int)WsResult.Success)
                {
                    SetRole(session, account, role, mrequest.Data, bag);
                    var ret = await info.handle();
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

        private async Task<Account> GetAccount(string session)
        {
            var pid = await SidUidDataHelper.Instance.GetUserNameBySidAsync(session);
            return await AccountDataHelper.Instance.GetAccountByUserNameAsync(pid);
        }

        /// <summary>
        /// 进入游戏 初始化角色和场景数据
        /// need TODO 场景数据
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private static async Task<BaseResponseData> JoinGame()
        {
            var result = new JoinGameResult();
            var data = new JoinGameReq();
            string sessionId = User.sessionId;
            if (User.bytes != null)
            {
                data = await InitHelpers.GetPse().DeserializeAsync<JoinGameReq>(User.bytes);
                if (data == null)
                {
                    //请求参数为空
                    result.Result = GameEnum.WsResult.RoleIdIsNull;
                    return result;
                }
            }
            else
            {
                //请求参数为空
                result.Result = GameEnum.WsResult.RoleIdIsNull;
                return result;
            }
            var roleId = new Guid(data.RoleId);
            var user = await RoleDataHelper.Instance.GetRoleByRoleIdAsync(roleId);

            if (user != null) //找到角色数据 绑定在线状态  返回数据给用户  设置状态为登录状态
            {
                //查找session用来绑定
                //查找用户是否在线
                var sid = await SidRoleIdDataHelper.Instance.GetSidByRoleIdAsync(roleId);    //查找当前连接是否有角色
                if (sid != null) //有旧的session
                {
                    if (sid.Equals(sessionId))   //检查session保存的roleid是不是和新登入的一致
                    {   //session一致  有可能是断线重连

                    }
                    else
                    {
                        //不一致  可能是从别的地方连接
                        await RoleDataHelper.Instance.RemoveRoleBySidAsync(sid);
                        await RoleDataHelper.Instance.RemoveRoleBySidAsync(sessionId);
                        await RoleDataHelper.Instance.SetRoleBySidAsync(sessionId, user);
                    }
                    //用户已经在线  进行通知处理  匹配 imei处理 预防盗号处理
                    //TODO
                    await SidRoleIdDataHelper.Instance.UpdateSidByRoleIdAsync(roleId, sessionId);
                }
                else
                {   //没有旧的session
                    await RoleDataHelper.Instance.RemoveRoleBySidAsync(sessionId);
                    await RoleDataHelper.Instance.SetRoleBySidAsync(sessionId, user);
                    //切记 下线时候要删除  或者在心跳包收不到的情况下     也要删除
                    await SidRoleIdDataHelper.Instance.SetSidByRoleIdAsync(roleId, sessionId);
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
                var bagInfo = await BagDataHelper.Instance.GetBagByRoleId(roleId);
                result.CompanyInfo = await CompanyController.Instance.GetCompanyInfoByRoleId(roleId);
                result.DepartInfoInfo = await DepartmentController.Instance.GetDepartmentInfoByRoleId(roleId);
                result.FinanceLogInfo = await FinanceLogController.Instance.GetFinanceLog(roleId);
                result.MapInfo = await LandController.Instance.GetRoleLandShopInfo(roleId);
                result.Room = await RoomController.Instance.GetRoom(user.Id) as RoomResult;
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
                if (bagInfo != null)
                {
                    result.RoleBag = new BagInfo()
                    {
                        CurUsedCell = bagInfo.CurUsedCell,
                        MaxCellNumber = bagInfo.MaxCellNumber,
                        Items = BagController.Instance.GetRoleItems(bagInfo.Items)
                    };
                }
                else
                {
                    bagInfo = new Bag();
                    result.RoleBag = new BagInfo()
                    {
                        CurUsedCell = bagInfo.CurUsedCell,
                        MaxCellNumber = bagInfo.MaxCellNumber,
                        Items = BagController.Instance.GetRoleItems(bagInfo.Items)
                    };
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
        private static async Task<BaseResponseData> CreateNewRole()//string sessionId, string name, int sex)
        {
            CreateRoleResult result = new CreateRoleResult();
            var account = User.account;
            var sessionId = User.sessionId;
            if (User.bytes == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<CreateRoleReq>(User.bytes);
            var name = data.Name;
            var sex = data.Sex;
            if (string.IsNullOrEmpty(name))
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var allRoles = await RoleListDataHelper.Instance.GetRoleListByAccountIdAsync(account.AccountID);
            if (allRoles != null)
            {
                if (account.RoleNumber >= 10)
                {
                    result.ErrorDesc = "角色数量过多，请删除部分角色再创建";
                    return result;
                }
                //检查是否重名
                if (await IdRoleDataHelper.Instance.CheckIdRoleByRoleNameAsync(name))
                {
                    result.ErrorDesc = "角色名重复";
                    return result;  //重名直接返回
                }
                //构造新角色
                UserRole user = new UserRole(sex, name, account.AccountID);
                IdRole idRole = new IdRole(user.Name, user.Id);

                await IdRoleDataHelper.Instance.SetIdRoleByRoleNameAsync(name, idRole);
                await RoleDataHelper.Instance.SetRoleByRoleIdAsync(user.Id, user);


                List<UserRole> roles = new List<UserRole>();
                roles.AddRange(allRoles);
                roles.Add(user);
                await RoleListDataHelper.Instance.UpdateRoleListByAccountIdAsync(account.AccountID, roles);
                await RoleDataHelper.Instance.UpdateRoleBySidAsync(sessionId, user);

                result.ErrorDesc = "";
                result.Name = name;
                result.Sex = sex;
                result.RoleId = user.Id.ToString();
            }
            else
            {
                ///没有角色，新增一个
                //检查重名
                if (await IdRoleDataHelper.Instance.CheckIdRoleByRoleNameAsync(name))
                {
                    result.ErrorDesc = "角色名重复";
                    return result;
                }
                //构造新角色
                UserRole user = new UserRole(sex, name, account.AccountID);
                IdRole idRole = new IdRole(user.Name, user.Id);


                await IdRoleDataHelper.Instance.SetIdRoleByRoleNameAsync(name, idRole);
                await RoleDataHelper.Instance.SetRoleByRoleIdAsync(user.Id, user);

                List<UserRole> roles = new List<UserRole>();
                roles.Add(user);

                await RoleListDataHelper.Instance.SetRoleListByAccountIdAsync(user.AccountId, roles);
                await RoleDataHelper.Instance.SetRoleBySidAsync(sessionId, user);
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
        private static async Task<BaseResponseData> Connecting()//ConnectingReq data, string sessionId)
        {

            ConnectingResult result = new ConnectingResult();
            var data = await InitHelpers.GetPse().DeserializeAsync<ConnectingReq>(User.bytes);
            string sessionId = User.sessionId;
            var signToken = data.Token;
            if (signToken == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            result.RoleLists = new List<RoleLists>();
            var token = await TokenDataHelper.Instance.GetTokenBySignToken(signToken);
            if (token != null)
            {
                //判断token时效
                if (token.ExpireTime >= DateTime.Now)
                {
                    var account1 = await AccountDataHelper.Instance.GetAccountByUserNameAsync(token.UserPassport);


                    var sid = await SidUidDataHelper.Instance.GetSidByUserNameAsync(token.UserPassport);
                    var act = await SidUidDataHelper.Instance.GetUserNameBySidAsync(sessionId);


                    //检查 账号和上次保存的账号是否匹配
                    if (!string.IsNullOrEmpty(act) && !string.IsNullOrEmpty(sid))
                    {//都存在 
                        if (Command.CheckPassport(act, token.UserPassport) && Command.CheckSession(sessionId, sid))
                        {   //都相同
                            //直接给登录
                            //TODO：踢下线 之后直接允许登录  或者允许重连
                            goto EqualsReturn;
                        }
                    }
                    if (!string.IsNullOrEmpty(act)) //表示session下面已经绑定账号了
                    {
                        //找到账号
                        if (!Command.CheckPassport(act, token.UserPassport))
                        {   //账号不相同
                            await SidUidDataHelper.Instance.RemoveUserNameBySidAsync(sessionId);
                            await SidUidDataHelper.Instance.UpdateUserNameBySidAsync(sessionId, token.UserPassport);
                            await SidUidDataHelper.Instance.RemoveSidByUserNameAsync(token.UserPassport);
                            await SidUidDataHelper.Instance.UpdateSidByUserNameAsync(token.UserPassport, sessionId);
                        }
                        else
                        {   //账号相同
                            await SidUidDataHelper.Instance.UpdateSidByUserNameAsync(token.UserPassport, sessionId); //更新session
                            if (!string.IsNullOrEmpty(sid))
                            {
                                await SidUidDataHelper.Instance.RemoveUserNameBySidAsync(sid);
                                await SidUidDataHelper.Instance.UpdateUserNameBySidAsync(sessionId, token.UserPassport);
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
                            await SidUidDataHelper.Instance.RemoveSidByUserNameAsync(token.UserPassport);
                            await SidUidDataHelper.Instance.UpdateSidByUserNameAsync(token.UserPassport, sessionId);
                            await SidUidDataHelper.Instance.UpdateUserNameBySidAsync(sessionId, token.UserPassport);
                        }
                        else
                        {//都不存在 直接绑定
                            await SidUidDataHelper.Instance.UpdateSidByUserNameAsync(token.UserPassport, sessionId);
                            await SidUidDataHelper.Instance.UpdateUserNameBySidAsync(sessionId, token.UserPassport);


                        }

                    }
                    EqualsReturn:
                    var roles = await RoleListDataHelper.Instance.GetRoleListByAccountIdAsync(account1.AccountID);
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
                            return result;
                        }
                        else
                        {
                            result.Result = GameEnum.WsResult.NotRole;
                            //无角色 返回-5
                            return result;
                        }
                    }
                    else
                    {
                        result.Result = GameEnum.WsResult.NotRole;
                        //无角色 返回-5
                        return result;
                    }
                    //当用户下线时 删除 token
                }

                else
                {
                    result.Result = GameEnum.WsResult.TokenTimeOut;
                    //token 超时
                    return result;
                }
            }
            else
            {
                result.Result = GameEnum.WsResult.TokenNotExists;
                //token不存在
                //拒绝连接
                return result;
            }
        }

        private static async Task SessionBindUserPort(string oldSid, string newSid, string oldAct, string newAct)
        {
            if (!string.IsNullOrEmpty(oldAct))
                await SidUidDataHelper.Instance.RemoveSidByUserNameAsync(oldAct);
            if (!string.IsNullOrEmpty(newAct))
                await SidUidDataHelper.Instance.RemoveSidByUserNameAsync(newAct);
            if (!string.IsNullOrEmpty(oldSid))
                await SidUidDataHelper.Instance.RemoveUserNameBySidAsync(oldSid);
            if (!string.IsNullOrEmpty(newSid))
                await SidUidDataHelper.Instance.RemoveUserNameBySidAsync(newSid);



            await SidUidDataHelper.Instance.SetSidByUserNameAsync(oldAct, newSid);
            await SidUidDataHelper.Instance.SetUserNameBySidAsync(newSid, oldAct);
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

            var oldAccount = await AccountDataHelper.Instance.GetAccountByUserNameAsync(pid);

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
            await AccountDataHelper.Instance.SetAccountByUserNameAsync(pid, newAccount);
            await AccountDataHelper.Instance.SetAccountByAccountIdAsync(newAccount.AccountID, newAccount);
            Login login = new Login()
            {
                AccountId = newAccount.AccountID,
                Pid = pid
            };
            await LoginDataHelper.Instance.SetLoginByUserName(pid, login);
            if (!string.IsNullOrEmpty(imei))
            {   //保存一键注册接口
                Passport passport = new Passport()
                {
                    AccountID = newAccount.AccountID,
                    IMEI = imei,
                    PassportID = newAccount.UserName
                };
                await PassportDataHelper.Instance.SetPassportByIMEI(imei, passport);
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

            var pot = await PassportDataHelper.Instance.GetPassportByIMEI(imei);
            if (pot != null)
            {
                var account = await AccountDataHelper.Instance.GetAccountByUserNameAsync(pot.PassportID);
                if (account != null)
                {
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
                await PassportDataHelper.Instance.SetPassportByIMEI(imei, passport);
                await AccountDataHelper.Instance.SetAccountByUserNameAsync(account.UserName, account);
                await AccountDataHelper.Instance.SetAccountByAccountIdAsync(account.AccountID, account);
                await LoginDataHelper.Instance.SetLoginByIMEI(imei, login);

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

                var login = await LoginDataHelper.Instance.GetLoginByUserName(imei);
                if (login != null)
                {
                    var account = await AccountDataHelper.Instance.GetAccountByUserNameAsync(login.Pid);
                    if (account != null)
                    {   //账号存在
                        account.LastLoginTime = DateTime.Now;
                        await AccountDataHelper.Instance.UpdateAccountByUserNameAsync(account.UserName, account);
                        Token token = new Token()
                        {
                            SignToken = Guid.NewGuid().ToString(),
                            TokenID = account.AccountID,
                            UserPassport = account.UserName
                        };
                        await TokenDataHelper.Instance.SetTokenBySignToken(token.SignToken, token);

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

                    var login = await LoginDataHelper.Instance.GetLoginByUserName(pid);
                    if (login != null)
                    {
                        var userName = login.Pid;
                        var account = await AccountDataHelper.Instance.GetAccountByUserNameAsync(userName);
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
                                await TokenDataHelper.Instance.UpdateTokenBySignToken(token.SignToken, token);
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



    }
}