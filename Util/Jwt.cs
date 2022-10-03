namespace BankAPI.Util
{
    public class Jwt
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Issuer { get; set; }
        public string Key { get; set; }
        public string Audience { get; set; }

    }
}
