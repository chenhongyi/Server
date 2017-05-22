using Microsoft.ServiceFabric.Services.Remoting;
using Model;
using Model.MsgQueue;
using Model.ViewModels;
using SuperSocket.WebSocket;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LogicServer.Interface
{
    public interface ILogicServer : IService
    {
        Task<AccountResult> Register(string pid, string pwd, string imei, string retailID, byte mobileType, string ipAddress);

        Task<AccountResult> Passport(string imei);
        Task<AccountResult> Login(string pid, string pwd, string imei, string ip);

        Task<byte[]> ProcessWsMessageAsync1(byte[] wsrequest, string session, CancellationToken cancellationToken);

        //TODO: 修改密码没做
        //Task<string> Password();
    }
}
