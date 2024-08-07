namespace Movies.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";
    
    public static class Movies
    {
        private const string Base = $"{ApiBase}/Movies";

        public const string Create = Base;
    }
    
}