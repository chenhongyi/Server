using Microsoft.ServiceFabric.Services.Remoting;
using Model.MsgQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicGate.Interface
{
    public interface IPublicGate:IService
    {
        Task SendOne(MsgQueueList msg);


        Task SendAll(MsgQueueList msg);
    }
}
