namespace Movies.Api.Auth;

public static class AuthConstants
{
    public const string AdminPolicy = "Admin";
    public const string AdminClaim = "admin";
    
    public const string TrustedMember = "Trusted";
    public const string TrustedClaim = "trusted_member";
    
    public const string ApiKeyHeaderName = "x-api-key";
}



