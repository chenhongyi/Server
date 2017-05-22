using ProtoBuf;


namespace Model.RequestData
{
    /// <summary>
    /// 客户端请求创建新角色
    /// </summary>
    /// NewRoleResult
    [ProtoContract]
    public class CreateRoleReq
    {
        [ProtoMember(1)] public string Name { get; set; }
        [ProtoMember(2)] public int Sex { get; set; }

    }
}
