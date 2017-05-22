using ProtoBuf;


namespace Model.RequestData
{
    /// <summary>
    /// 客户端请求接入服务器
    /// </summary>
    /// 
    [ProtoContract]
    public class ConnectingReq
    {
        [ProtoMember(1)] public string Token { get; set; }
    }
}

