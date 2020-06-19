using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using ASaE.Models;
using Owin.Security.Providers.VKontakte;
using Owin.Security.Providers.VKontakte.Provider;
using System.Threading.Tasks;

namespace ASaE
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            app.UseVKontakteAuthentication(new VKontakteAuthenticationOptions
            {
                ClientId = "7462964",
                ClientSecret = "Zqloa3Eg2OhJZOUaxrsd",
                Provider = new VKontakteAuthenticationProvider
                {
                    OnAuthenticated = context =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("vk:id", context.Id));             // Сохраняем данные из вк
                        context.Identity.AddClaim(new System.Security.Claims.Claim("vk:token", context.AccessToken));
                        context.Identity.AddClaim(new System.Security.Claims.Claim("vk:first_name", context.User.Value<string>("first_name")));
                        context.Identity.AddClaim(new System.Security.Claims.Claim("vk:last_name", context.User.Value<string>("last_name")));
                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}