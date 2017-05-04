using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting;
using LogicServer.Interface;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Shared;
using Model;
using Microsoft.AspNetCore.Http;
using Shared.Serializers;

namespace Account.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly string _sign;
        private readonly ILogicServer _account;
        public AccountController()
        {
            _sign = "123456789";
            _account = ConnectionFactory.CreateAccountService();
        }
        [HttpGet]
        public BaseResponse Register(string pid, string pwd, string imei, int screenX, int screenY, string retailID, string sign, byte mobileType = 0)
        {
            #region 非空验证
            if (string.IsNullOrEmpty(pid) || string.IsNullOrEmpty(pwd) || string.IsNullOrEmpty(sign))
            {
                return new BaseResponse()
                {
                    Handler = nameof(AccountController.Register),
                    StateCode = StateCode.ParamsError,
                    StateDescription = StateDescription.ParamsError
                };
            }
            #endregion

            #region Sign验证
            if (!CheckSign(HttpContext, sign))
            {
                return new BaseResponse()
                {
                    Handler = nameof(AccountController.Register),
                    StateCode = StateCode.SignError,
                    StateDescription = StateDescription.SignError
                };
            }
            #endregion

            var ip = HttpContext.Connection.RemoteIpAddress.ToString();

            //逻辑处理
            var result = _account.Register(pid, pwd, imei, retailID, mobileType, ip).Result;
            if (result != null)
            {
                switch (result.Status)
                {
                    case ARsult.ServerError:
                        return new BaseResponse()
                        {
                            Handler = nameof(AccountController.Register),
                            StateCode = StateCode.ServerError,
                            StateDescription = StateDescription.ServerError
                        };
                    case ARsult.Ok:
                        return new BaseResponse()
                        {
                            Handler = nameof(AccountController.Register)
                        };
                    case ARsult.AccountIsExists:
                        return new BaseResponse()
                        {
                            Handler = nameof(AccountController.Register),
                            StateCode = StateCode.AccountIsExist,
                            StateDescription = StateDescription.AccountIsExist
                        };
                    case ARsult.PassWordError:
                        return new BaseResponse()
                        {
                            Handler = nameof(AccountController.Register),
                            StateCode = StateCode.PasswordError,
                            StateDescription = StateDescription.PasswordError
                        };
                    case ARsult.NoneAccount:
                        return new BaseResponse()
                        {
                            Handler = nameof(AccountController.Register),
                            StateCode = StateCode.AccountNotExist,
                            StateDescription = StateDescription.AccountNotExist
                        };
                }
            }
            return new BaseResponse()
            {
                Handler = nameof(AccountController.Register),
                StateCode = StateCode.ServerIsBusy,
                StateDescription = StateDescription.ServerIsBusy
            };
        }


        #region 验证md5
        private bool CheckSign(HttpContext httpContext, string sign)
        {
        #if DEBUG
                    return true;
        #endif
            //TODO 开发阶段 取消掉
            string param = httpContext.Request.QueryString.Value;
            var index = param.IndexOf("&sign");
            param = param.Substring(1, index - 1);  //拿到正确的参数  + sign  进行 md5  然后与 传入的比较
            param = param + "&sign=" + _sign;
            if (sign.Equals(MD5Helper.GetMD5Str(param)))
                return true;
            return false;
        }
        #endregion

        [HttpGet]
        public BaseResponse Passport(string imei, string sign)
        {
            #region 非空验证
            if (string.IsNullOrEmpty(imei) || string.IsNullOrEmpty(sign))
            {
                return new BaseResponse()
                {
                    Handler = nameof(this.Passport),
                    StateCode = StateCode.ParamsError,
                    StateDescription = StateDescription.ParamsError
                };
            }
            #endregion
            #region Sign验证
            if (!CheckSign(HttpContext, sign))
            {
                return new BaseResponse()
                {
                    Handler = nameof(AccountController.Passport),
                    StateCode = StateCode.SignError,
                    StateDescription = StateDescription.SignError
                };
            }
            #endregion
            //逻辑处理
            var result = _account.Passport(imei).Result;
            if (result != null)
            {
                switch (result.Status)
                {
                    case ARsult.ServerError:
                        return new BaseResponse()
                        {
                            Handler = nameof(AccountController.Passport),
                            StateCode = StateCode.ServerError,
                            StateDescription = StateDescription.ServerError
                        };
                    case ARsult.Ok:
                        return new BaseResponse()
                        {
                            Handler = nameof(AccountController.Passport)
                        };
                    case ARsult.AccountIsExists:
                        return new BaseResponse()
                        {
                            Handler = nameof(AccountController.Passport),
                            StateCode = StateCode.AccountIsExist,
                            StateDescription = StateDescription.AccountIsExist
                        };
                    case ARsult.PassWordError:
                        return new BaseResponse()
                        {
                            Handler = nameof(AccountController.Passport),
                            StateCode = StateCode.PasswordError,
                            StateDescription = StateDescription.PasswordError
                        };
                    case ARsult.NoneAccount:
                        return new BaseResponse()
                        {
                            Handler = nameof(AccountController.Passport),
                            StateCode = StateCode.AccountNotExist,
                            StateDescription = StateDescription.AccountNotExist
                        };
                }

            }
            return new BaseResponse()
            {
                Handler = nameof(AccountController.Register),
                StateCode = StateCode.ServerIsBusy,
                StateDescription = StateDescription.ServerIsBusy
            };
        }

        [HttpGet]
        public BaseResponse Login(int mobileType, string pid, string pwd, string imei, int screenX, int screexY, string retailID, string retailUser, string retailToken, string sign)
        {
            #region 非空检查
            if (string.IsNullOrEmpty(sign))
            {
                return new BaseResponse()
                {
                    Handler = nameof(this.Login),
                    StateCode = StateCode.ParamsError,
                    StateDescription = StateDescription.ParamsError
                };
            }
            #endregion
            #region sign验证
            if (!CheckSign(HttpContext, sign))
            {
                return new BaseResponse()
                {
                    Handler = nameof(AccountController.Login),
                    StateCode = StateCode.SignError,
                    StateDescription = StateDescription.SignError
                };
            }
            #endregion

            #region 是否是第三方
            if (!string.IsNullOrEmpty(retailID) && !string.IsNullOrEmpty(retailUser) && !string.IsNullOrEmpty(retailToken))
            {
                //第三方登录
                //TODO:
            }
            #endregion
            else
            {
                //非第三方登录 正常流程
                //逻辑处理
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var result = _account.Login(pid, pwd, imei, ip).Result;
                if (result != null)
                {
                    switch (result.Status)
                    {
                        case ARsult.ServerError:
                            return new BaseResponse()
                            {
                                Handler = nameof(AccountController.Login),
                                StateCode = StateCode.ServerError,
                                StateDescription = StateDescription.ServerError
                            };
                        case ARsult.Ok:
                            return new BaseResponse()
                            {
                                Data = new { Token = result.TokenResult.SignToken },
                                Handler = nameof(AccountController.Login)
                            };
                        case ARsult.AccountIsExists:
                            return new BaseResponse()
                            {
                                Handler = nameof(AccountController.Login),
                                StateCode = StateCode.AccountIsExist,
                                StateDescription = StateDescription.AccountIsExist
                            };
                        case ARsult.PassWordError:
                            return new BaseResponse()
                            {
                                Handler = nameof(AccountController.Login),
                                StateCode = StateCode.PasswordError,
                                StateDescription = StateDescription.PasswordError
                            };
                        case ARsult.NoneAccount:
                            return new BaseResponse()
                            {
                                Handler = nameof(AccountController.Login),
                                StateCode = StateCode.AccountNotExist,
                                StateDescription = StateDescription.AccountNotExist
                            };
                    }
                }

            }
            return new BaseResponse()
            {
                Handler = nameof(AccountController.Register),
                StateCode = StateCode.ServerIsBusy,
                StateDescription = StateDescription.ServerIsBusy
            };
        }

        [HttpGet]
        public void Password()
        {
        }

        [HttpGet]
        public void Notice()
        {
        }

        [HttpGet]
        public void Delete(int id)
        {
        }
    }
}
