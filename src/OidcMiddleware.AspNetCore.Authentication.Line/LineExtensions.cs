using System;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

using OidcMiddleware.AspNetCore.Authentication.Line;
using OidcMiddleware.Extensions.DependencyInjection.Line;

namespace OidcMiddleware.Extensions.DependencyInjection
{
    public static class LineExtensions
    {
        public static AuthenticationBuilder AddLine(this AuthenticationBuilder builder)
            => builder.AddLine(LineDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddLine(this AuthenticationBuilder builder, Action<LineOptions> configureOptions)
            => builder.AddLine(LineDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddLine(this AuthenticationBuilder buidler, string authenticationScheme, Action<LineOptions> configureOptions)
            => buidler.AddLine(authenticationScheme, LineDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddLine(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<LineOptions> configureOptions)
            => builder.AddOAuth<LineOptions, LineHandler>(authenticationScheme, displayName, configureOptions);
    }
}
