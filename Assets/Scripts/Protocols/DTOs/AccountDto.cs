namespace Protocols.DTOs
{
    public class AccountDto
    {
        public string UserName { get; set; }
        public int LevelCode { get; set; }
        public int CurrentExp { get; set; }
    }

    public class CurrencyInfoDto
    {
        public int Gold { get; set; }
        public int Cash { get; set; }
        public int Diamonds { get; set; }
    }
}
