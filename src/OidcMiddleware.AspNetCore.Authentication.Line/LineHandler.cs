using System;
using System.Globalization;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

using OidcMiddleware.AspNetCore.Authentication.Line;

namespace OidcMiddleware.Extensions.DependencyInjection.Line
{
    public class LineHandler : OAuthHandler<LineOptions>
    {
        public LineHandler(IOptionsMonitor<LineOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(
            ClaimsIdentity identity,
            AuthenticationProperties properties,
            OAuthTokenResponse tokens)
        {
            // LINE has no UserInfo Endpoint so claims are created from IdToken.
            // https://developers.line.me/ja/docs/line-login/web/integrate-line-login/

            var idToken = tokens.Response.Value<string>("id_token");
            var idTokenSplit = idToken.Split('.');

            var headerDecoded = Encoding.UTF8.GetString(DecodeBase64Url(idTokenSplit[0]));
            var payloadDecoded = JObject.Parse(Encoding.UTF8.GetString(DecodeBase64Url(idTokenSplit[1])));
            var signatureDecoded = DecodeBase64Url(idTokenSplit[2]);

            var isValid = ValidateSignature(Options.ChannelSecret, idTokenSplit[0], idTokenSplit[1], signatureDecoded);
            if (!isValid)
            {
                throw new AuthenticationException(string.Format(CultureInfo.CurrentCulture, Resources.Exception_SpecifiedTokenIsInvalid, idToken));
            }

            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, payloadDecoded);
            context.RunClaimActions();

            await Events.CreatingTicket(context);

            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }

        private byte[] DecodeBase64Url(string encoded)
        {
            var decoded = new StringBuilder(encoded);

            var rem = decoded.Length % 4;
            if (rem > 0)
            {
                decoded.Append(new String('=', 4 - rem));
            }
            decoded = decoded.Replace('-', '+').Replace('_', '/');

            return Convert.FromBase64String(decoded.ToString());
        }

        private bool ValidateSignature(string key, string header, string payload, byte[] signature)
        {
            using (var sha = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(header + "." + payload));
                return hash.SequenceEqual(signature);
            }
        }
    }
}