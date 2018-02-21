namespace UPP.Security
{
    /// <summary>
    /// Define all of the claims used by UPP. A claim is a statement about a user. The specific claims
    /// returned may be dependent on the scopes the client requests, i.e. unless the client asks for the 
    /// 'address' scope, no mailing address will be provided.
    /// </summary>
    public static class Claims
    {
        // This identity is a hauler (person that drives a truck)
        public const string HAULER = "hauler";
    }
}
