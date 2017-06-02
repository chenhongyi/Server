using AutoData;
using Microsoft.ServiceFabric.Data.Collections;
using Model.Data.Account;
using Model.Data.Npc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Model.Data.General;
using Model;
using Model.MsgQueue;
using Shared.Serializers;
using Shared;
using Model.Data.Business;
using GameEnum;
using Model.ResponseData;
using LogicServer.Data.Helper;

namespace LogicServer.Data
{

    public static class Command
    {

        /// <summary>
        /// 检查登入的账号和通过session保存的账号是否相同
        /// </summary>
        /// <param name="act"></param>
        /// <param name="userPassport"></param>
        public static bool CheckPassport(string act, string userPassport)
        {
            return act.Equals(userPassport);
        }
        /// <summary>
        /// 检查session和上次保存的是否相同
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static bool CheckSession(string sessionId, string sid)
        {
            return sid.Equals(sessionId);
        }
    }
}
