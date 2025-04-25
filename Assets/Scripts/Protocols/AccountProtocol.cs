using Protocols.DTOs;
using System.Collections.Generic;

namespace Protocols
{
    // ++++++++++++++++++ url: account/login ++++++++++++++++++
    public class LoginReq
    {
        public string AccountName { get; set; }
        public string Password { get; set; }
        // public string GamePotUserId { get; set; }
        // public string RemoteIp { get; set; }
        // public string GamePotToken { get; set; }
        // public string Platform { get; set; }
    }

    public class LoginRes : ResponsePacketBase
    {
        public string Token { get; set; }
    }

    // ++++++++++++++++++ url: account/refreshToken ++++++++++++++++++
    public class TokenRefreshReq { }
    public class TokenRefreshRes : ResponsePacketBase
    {
        public string Token { get; set; }
    }

    // ++++++++++++++++++ url: account/info ++++++++++++++++++
    public class AccountInfoReq
    {
        public string AccountName { get; set; }
    }

    public class AccountInfoRes : ResponsePacketBase
    {
        public AccountDto Account { get; set; }
    }
    
    public class AccountCurrencyInfoRes : ResponsePacketBase
    {
        public CurrencyInfoDto Currency { get; set; }
    }
}
