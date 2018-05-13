
namespace OidcMiddleware.AspNetCore.Authentication.Line
{
    /// <summary>
    /// Default values for LINE authentication
    /// </summary>
    public static class LineDefaults
    {
        public const string AuthenticationScheme = "LINE";

        public static readonly string DisplayName = "LINE";

        public static readonly string AuthorizationEndpoint = "https://access.line.me/oauth2/v2.1/authorize";

        public static readonly string TokenEndpoint = "https://api.line.me/oauth2/v2.1/token";
    }
}
