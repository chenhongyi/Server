using Model.Data.Account;

namespace Model.ViewModels
{
    public class AccountResult
    {
        public Token TokenResult { get; set; } = new Token();
        public ARsult Status { get; set; } = ARsult.Ok;
    }

    public enum ARsult
    {
        ServerError = -1,
        Ok = 0,
        AccountIsExists = 101,
        PassWordError = 102,
        NoneAccount = 103,
    }

}
