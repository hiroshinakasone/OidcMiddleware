using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using OidcMiddleware.AspNetCore.Authentication.Line;
using OidcMiddleware.Extensions.DependencyInjection;

namespace LineLoginSample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = LineDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddLine(options =>
            {
                options.ChannelId = "YOUR_APP_CHANNEL_ID";
                options.ChannelSecret = "YOUR_APP_CHANNEL_SECRET";
                options.CallbackPath = new PathString("/your/app/callback/path");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseAuthentication();

                app.Run(async (context) =>
                {
                    var user = context.User;
                    if (user == null || !user.Identity.IsAuthenticated)
                    {
                        // goes to line login
                        await context.ChallengeAsync(LineDefaults.AuthenticationScheme);
                        return;
                    }

                    context.Response.ContentType = "text/plain; charset=UTF-8";
                    foreach (var claim in user.Claims)
                    {
                        await context.Response.WriteAsync($"{claim.Type}: {claim.Value}\n");
                    }
                });

            }
        }
    }
}
