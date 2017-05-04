using ProtoBuf;

namespace Model
{
    [ProtoContract]
    public class WsRequestMessage
    {
        [ProtoMember(1)] public int     PartitionKey;
        [ProtoMember(2)] public int     MsgId;  //消息id
        [ProtoMember(3)] public string  SignKey; //signkey
        [ProtoMember(4)] public byte[]  Data;   //data
    }
}
