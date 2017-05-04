using ProtoBuf;

namespace Model
{
    [ProtoContract]
    public class WsResponseMessage
    {
        [ProtoMember(1)] public int MsgId;
        [ProtoMember(2)] public int Result;
        [ProtoMember(3)] public byte[] Value;
    }
}
