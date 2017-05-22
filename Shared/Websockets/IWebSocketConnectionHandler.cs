using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Websockets
{
    public interface IWebSocketConnectionHandler
    {
        /// <summary>
        /// 处理二进制消息程序
        /// </summary>
        /// <param name="wsrequest"></param>
        /// <param name="sessionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<byte[]> ProcessWsMessageAsync(byte[] wsrequest, string sessionId, CancellationToken cancellationToken);


        /// <summary>
        /// 处理文本消息
        /// </summary>
        /// <param name="wsrequest"></param>
        /// <param name="sessionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ProcessWsMessageAsync(string sessionId, CancellationToken cancellationToken);

    } 
}
