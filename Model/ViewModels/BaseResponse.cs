namespace Model
{
    public class BaseResponse
    {
        public StateCode StateCode { get; set; } = 0;
        public string StateDescription { get; set; } = "";
        public string Version { get; set; } = "1.0";

        public string Handler { get; set; }
        public object Data { get; set; } = null;
    }

    public enum StateCode
    {
        OK = 0,
        ServerError = 100,
        SignError = 101,
        NoAction = 102,
        PasswordError = 103,
        AccountNotExist = 104,
        TokenInvaild = 105,
        TokenExpire = 106,
        ParamsError = 107,
        ServerIsBusy = 108,
        AccountIsExist = 109,
    }

    public static class StateDescription
    {
        public static string ServerError = "服务器错误";
        public static string SignError = "签名密钥错误";
        public static string NoAction = "没有响应的处理程序";
        public static string PasswordError = "密码错误";
        public static string TokenInvaild = "Token无效";
        public static string TokenExpire = "Token过期";
        public static string AccountNotExist = "账号不存在";
        public static string ParamsError = "必要参数缺失";
        public static string ServerIsBusy = "服务器繁忙";
        public static string AccountIsExist = "账号已存在";
    }
}
