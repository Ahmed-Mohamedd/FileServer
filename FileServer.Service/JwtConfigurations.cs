namespace FileServer.Service
{
    public class JwtConfigurations
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double ExpiresOn { get; set; }
    }
}
