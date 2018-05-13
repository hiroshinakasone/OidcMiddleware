using System;
using System.Globalization;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

using OidcMiddleware.AspNetCore.Authentication.Line;


namespace OidcMiddleware.Extensions.DependencyInjection.Line
{
    public class LineOptions : OAuthOptions
    {
        public LineOptions()
        {
            CallbackPath = new PathString("/signin-line");
            AuthorizationEndpoint = LineDefaults.AuthorizationEndpoint;
            TokenEndpoint = LineDefaults.TokenEndpoint;

            Scope.Add("openid");
            Scope.Add("profile");
            Scope.Add("email");

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            ClaimActions.MapJsonKey("picture", "picture");
        }

        public override void Validate()
        {
            if (string.IsNullOrEmpty(ChannelId))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.Exception_OptionMustBeProvided, nameof(ChannelId)));
            }

            if (string.IsNullOrEmpty(ChannelSecret))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.Exception_OptionMustBeProvided, nameof(ChannelSecret)));
            }

            base.Validate();
        }

        // LINE uses a non-standard term for this field.
        /// <summary>
        /// Gets or sets the LINE-assigned channelId.
        /// </summary>
        public string ChannelId
        {
            get { return ClientId; }
            set { ClientId = value; }
        }

        // LINE uses a non-standard term for this field.
        /// <summary>
        /// Gets or sets the LINE-assigned channelSecret.
        /// </summary>
        public string ChannelSecret
        {
            get { return ClientSecret; }
            set { ClientSecret = value; }
        }
    }

}
