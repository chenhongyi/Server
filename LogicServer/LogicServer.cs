using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using LogicServer.Interface;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Shared.Serializers;
using Model;
using Logging;
using Shared;
using Shared.Websockets;
using AutoData;
using Model.RequestData;
using System.Net;
using Model.Npc;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace LogicServer
{
    /// <summary>
    /// 通过 Service Fabric 运行时为每个服务副本创建此类的一个实例。
    /// </summary>
    internal sealed class LogicServer : StatefulService, ILogicServer, IWebSocketConnectionHandler
    {
        IWsSerializer pserializer;
        ProtobufWsSerializer mserializer;
        private const string AccountsCollectionName = "accounts";
        private const string TokenCollectionName = "tokens";
        private const string PassportCollectionName = "passports";
        private const string LoginCollectionName = "logins";
        private const string OnlinePlayerName = "onlinePlayers";
        private const string SessionCollectionName = "sessions";
        private const string RoleName = "roles";
        private IReliableDictionary<string, Account> accountsCollection = null;         //账号数据
        private IReliableDictionary<string, Token> tokenCollection = null;              //token数据  key是 guid.tostring()
        private IReliableDictionary<string, Passport> passportCollection = null;        //通行证数据
        private IReliableDictionary<string, Login> loginCollection = null;              //Login数据
        private IReliableDictionary<string, Guid> onlinePlayerCollection = null;        //在线玩家
        private IReliableDictionary<string, string> sessionCollection = null;             //session数据绑定   先绑定到账号上 角色登陆后 修改为绑定到角色上  角色下线 再改为账号 断开连接 删除session
        private IReliableDictionary<string, UserRole> roleCollection = null;
        private static List<Avatar> avatar = null;
        private static List<Item> item = null;
        private static List<Character> character = null;
        private static List<Level> lv = null;
        private static List<Roomfurniture> rf = null;

        private static readonly ILogger Logger = LoggerFactory.GetLogger(nameof(LogicServer));
        public LogicServer(StatefulServiceContext context)
            : base(context)
        {

            //初始化配置数据
            TxtReader.Init();
            this.pserializer = SerializerFactory.CreateSerializer();
            mserializer = new ProtobufWsSerializer();
            avatar = Avatar.GetAll(); ;
            item = Item.GetAll();
            character = Character.GetAll();
            lv = Level.GetAll();
            rf = Roomfurniture.GetAll();
        }

        /// <summary>
        /// IWebSocketListener.ProcessWsMessageAsync 消息分发过来进行处理
        /// </summary>
        public async Task<byte[]> ProcessWsMessageAsync(byte[] wsrequest, string sessionId, CancellationToken cancellationToken)
        {
            Logger.Debug(nameof(this.ProcessWsMessageAsync));

            WsRequestMessage mrequest = await mserializer.DeserializeAsync<WsRequestMessage>(wsrequest);

            WsResponseMessage mresponse = new WsResponseMessage();
            ///事件集中在这里处理  并且可以保存状态
            switch (mrequest.MsgId)
            {
                #region 用户连接逻辑服务器 WSMsgID.Connecting = 1000
                //TODO 需要检查重复登录
                case WSMsgID.Connecting:
                    {
                        mresponse.MsgId = WSMsgID.Connecting;
                        var data = await this.pserializer.DeserializeAsync<ConnectingReq>(mrequest.Data);
                        int ret = await this.BindAccount(data, sessionId);
                        switch (ret)
                        {
                            case 0://ok
                                mresponse.Result = WsResult.Success;
                                break;
                            case -1://参数为空
                                mresponse.Result = WsResult.ParamsError;
                                break;
                            case -2://token不存在
                                mresponse.Result = WsResult.TokenIsNotExists;
                                break;
                            case -3://token超时
                                mresponse.Result = WsResult.TokenTimeOut;
                                break;
                            case -4://用户不存在
                                mresponse.Result = WsResult.NoneUser;
                                break;
                        }
                    }
                    return await mserializer.SerializeAsync(mresponse);
                #endregion
                #region 创建角色 WSMsgID.CreateRole = 1001
                case WSMsgID.CreateRole:
                    {
                        mresponse.MsgId = WSMsgID.CreateRole;
                        var data = await this.pserializer.DeserializeAsync<NewRoleReq>(mrequest.Data);
                        UserRoleResult ret = await this.CreateNewRole(sessionId, data.Name, data.Sex);
                        switch (ret.Status)
                        {
                            case 0: //正常  返回角色数据
#if DEBUG
                                mresponse.Value = await mserializer.SerializeAsync(ret.BaseNpc);
#else
                                mresponse.Value = await mserializer.SerializeAsync(ret.UserRole);
#endif
                                mresponse.Result = WsResult.Success;
                                break;
                            case -1: //重名
                                mresponse.Result = WsResult.DuplicationName;

                                break;
                            case -2:    //角色数量限制
                                mresponse.Result = WsResult.MoreRoles;
                                break;
                        }
                    }
                    return await mserializer.SerializeAsync(mresponse);
                #endregion
                #region 删除角色 WSMsgID.DelRole = 1003
                case WSMsgID.DelRole:
                    {
                        mresponse.MsgId = WSMsgID.DelRole;

                    }
                    return await mserializer.SerializeAsync(mresponse);
                #endregion
                #region 进入游戏 WSMsgID.JoinGame = 1002
                case WSMsgID.JoinGame:
                    {
                        mresponse.MsgId = WSMsgID.JoinGame;
                    }
                    return await mserializer.SerializeAsync(mresponse);
                    #endregion
            }
            mresponse.Result = WsResult.NoneActionFunc;
            return await mserializer.SerializeAsync(mresponse);
        }




#if DEBUG
#endif
        private async Task<UserRoleResult> CreateNewRole(string sessionId, string name, int sex)
        {
           
            this.sessionCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(SessionCollectionName);
            this.accountsCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Account>>(AccountsCollectionName);
            this.roleCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, UserRole>>(RoleName);
            UserRoleResult result = new UserRoleResult();
            using (var tx = this.StateManager.CreateTransaction())
            {
                var sid = await this.sessionCollection.TryGetValueAsync(tx, sessionId);
                if (sid.HasValue)
                {
                    var usr = await this.accountsCollection.TryGetValueAsync(tx, sid.Value);
                    if (usr.HasValue)
                    {
                        //检查可以创建的角色 暂定允许创建5个角色
                        if (usr.Value.RoleNumber >= 5)
                        {
                            result.Status = -2;
                            return result;
                        }

                        //检查是否重名
                        var nameHas = await this.roleCollection.TryGetValueAsync(tx, name);
                        if (nameHas.HasValue)
                        {
                            result.Status = -1;
                            return result;  //重名直接返回
                        }
#if DEBUG
                        result.BaseNpc = new BaseNpc()
                        {
                            Exp = 100,
                            Level = 10,
                            Name = name,
                            Number = 1
                        };
#else
                        result.UserRole = new UserRole();
                        result.UserRole.AccountId = usr.Value.AccountID;
                        result.UserRole.Certificates = new byte[] { 0, 0, 0, 0, 0 };
                        result.UserRole.CertificatesExp = new int[] { 0, 0, 0, 0, 0 };

                        await this.roleCollection.AddAsync(tx, name, result.UserRole);
#endif
                        await tx.CommitAsync();
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// 对连接服务器请求做出回应
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        private async Task<int> BindAccount(ConnectingReq data, string sessionId)
        {
            if (data == null || string.IsNullOrEmpty(sessionId))
            {
                return -1;  //参数为空返回-1
            }
            this.tokenCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Token>>(TokenCollectionName);
            this.accountsCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Account>>(AccountsCollectionName);
            this.sessionCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(SessionCollectionName);
            using (var tx = this.StateManager.CreateTransaction())
            {
                var td = await this.tokenCollection.TryGetValueAsync(tx, data.Token); //取得token

                if (td.HasValue)
                {
                    //判断token时效
                    if (td.Value.ExpireTime >= DateTime.Now)
                    {
                        var usr = await this.accountsCollection.TryGetValueAsync(tx, td.Value.UserPassport);
                        if (usr.HasValue)
                        {
                            //token有效 并且账号存在 尝试获取session是否存在  用来判断是否已经登录
                            var isOnline = await this.sessionCollection.TryGetValueAsync(tx, sessionId);
                            if (isOnline.HasValue)
                            {
                                //账号已经登录
                                //TODO  检查 imei  和  其他的是否匹配   进行kick或者其他操作
                            }
                            else
                            {
                                //取得账号  对session进行绑定 
                                await this.sessionCollection.AddAsync(tx, sessionId, usr.Value.UserName); //添加在线表中
                                await tx.CommitAsync();
                            }
                            //当用户下线时 删除 token
                        }
                        else
                        {
                            //用户不存在
                            return -4;
                        }
                    }
                    else
                    {
                        //token 超时
                        return -3;
                    }
                }
                else
                {
                    //token不存在
                    //拒绝连接
                    return -2;
                }
                //TODO: 修正返回值
                return 0;
            }
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

        /// <summary>
        /// 这是服务副本的主入口点。
        /// 在此服务副本成为主服务并具有写状态时，将执行此方法。
        /// </summary>
        /// <param name="cancellationToken">已在 Service Fabric 需要关闭此服务副本时取消。</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {

            // TODO: 将以下示例代码替换为你自己的逻辑 
            //       或者在服务不需要此 RunAsync 重写时删除它。

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
            this.accountsCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Account>>(AccountsCollectionName);
            this.passportCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Passport>>(PassportCollectionName);
            this.loginCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Login>>(LoginCollectionName);
            Account user = new Account(imei, mobileType, ipAddress)
            {
                AccountID = Guid.NewGuid(),
                Password = pwd,
                UserName = pid,
            };
            using (var tx = this.StateManager.CreateTransaction())
            {
                //首先查找pid是否存在
                var oldUser = await this.accountsCollection.TryGetValueAsync(tx, pid);
                if (oldUser.HasValue)
                {
                    //账号存在
                    //直接返回账号已存在
                    //返回调用login流程
                    return new AccountResult()
                    {
                        Status = ARsult.AccountIsExists,    //账号已存在
                        TokenResult = null
                    };
                }
                await this.accountsCollection.AddAsync(tx, pid, user);
                Login login = new Login()
                {
                    AccountId = user.AccountID,
                    Pid = pid
                };
                await this.loginCollection.AddAsync(tx, pid, login);    //登录数据接口
                if (!string.IsNullOrEmpty(imei))
                {   //保存一键注册接口
                    Passport passport = new Passport()
                    {
                        AccountID = user.AccountID,
                        IMEI = imei,
                        PassportID = user.UserName
                    };
                    await this.passportCollection.AddAsync(tx, imei, passport);
                }
                await tx.CommitAsync();
                return new AccountResult()
                {
                    Status = ARsult.Ok,
                    TokenResult = null
                };
            }
        }

        /// <summary>
        /// 一键注册
        /// </summary>
        /// <param name="imei">设备码</param>
        /// <returns>token</returns>
        public async Task<AccountResult> Passport(string imei)
        {
            this.accountsCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Account>>(AccountsCollectionName);
            this.passportCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Passport>>(PassportCollectionName);
            this.loginCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Login>>(LoginCollectionName);
            using (var tx = this.StateManager.CreateTransaction())
            {
                var pot = await this.passportCollection.TryGetValueAsync(tx, imei); //找与设备相符的通行证
                if (pot.HasValue)
                {
                    var user = await this.accountsCollection.TryGetValueAsync(tx, pot.Value.PassportID);
                    if (user.HasValue)
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
                    Login login = new Model.Login()
                    {
                        AccountId = account.AccountID,
                        Pid = imei
                    };
                    await this.passportCollection.AddAsync(tx, imei, passport);  //注册passport表
                    await this.accountsCollection.AddAsync(tx, account.UserName, account);   //注册账号表
                    await this.loginCollection.AddAsync(tx, imei, login);   //注册登录表

                    await tx.CommitAsync();
                    return new AccountResult()
                    {
                        Status = ARsult.Ok
                    };
                }
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
            this.loginCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Login>>(LoginCollectionName);
            this.accountsCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Account>>(AccountsCollectionName);
            this.tokenCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Token>>(TokenCollectionName);
            //可以使用账号登录，也可以使用设备码登录
            if (!string.IsNullOrEmpty(imei))
            {       //使用设备码登录
                    //查询登录表中是否有数据

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var login = await this.loginCollection.TryGetValueAsync(tx, imei);
                    if (login.HasValue)
                    {
                        var account = await this.accountsCollection.TryGetValueAsync(tx, login.Value.Pid);    //取账号
                        if (account.HasValue)
                        {   //账号存在
                            account.Value.LastLoginTime = DateTime.Now;
                            await this.accountsCollection.TryUpdateAsync(tx, account.Value.UserName, account.Value, null); //更新账号
                            Token token = new Token()
                            {
                                SignToken = Guid.NewGuid().ToString(),
                                TokenID = account.Value.AccountID,
                                UserPassport = account.Value.UserName
                            };
                            await this.tokenCollection.TryAddAsync(tx, token.SignToken, token); //写入token列表了

                            await tx.CommitAsync();
                            return new AccountResult()
                            {
                                TokenResult = token
                            };
                        }

                    }

                }
            }
            else
            {
                //设备码为空 使用账号密码登录
                if (!string.IsNullOrEmpty(pid) && !string.IsNullOrEmpty(pwd))
                {
                    using (var tx = this.StateManager.CreateTransaction())
                    {
                        //验证账号密码
                        var login = await this.loginCollection.TryGetValueAsync(tx, pid);
                        if (login.HasValue)
                        {
                            var userName = login.Value.Pid;
                            var account = await this.accountsCollection.TryGetValueAsync(tx, userName);
                            if (account.HasValue)
                            {
                                //比对密码
                                if (pwd.Equals(account.Value.Password))
                                {
                                    //密码正确 返回token
                                    //更新 token
                                    Token token = new Token()
                                    {
                                        SignToken = Guid.NewGuid().ToString(),
                                        TokenID = account.Value.AccountID,
                                        UserPassport = pid
                                    };
                                    await this.tokenCollection.TryAddAsync(tx, token.SignToken, token);
                                    await tx.CommitAsync();
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
                        }
                        //没找到账号 返回
                        //TODO 需要标记状态
                        return new AccountResult()
                        {
                            Status = ARsult.NoneAccount
                        };
                    }
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
