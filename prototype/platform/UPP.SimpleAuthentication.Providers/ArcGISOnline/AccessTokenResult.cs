namespace UPP.SimpleAuthentication.Providers.ArcGISOnline
{
    public class AccessTokenResult
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string UserName { get; set; }
        public string RefreshToken { get; set; }
    }
}
