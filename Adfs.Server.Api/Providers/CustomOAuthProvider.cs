using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Adfs.Server.Api.Models;

namespace Adfs.Server.Api.Providers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult(0);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var user = new ApplicationUser(context.UserName, context.Password);

            var domainManager = context.OwinContext.Get<DomainAuthenticationProvider>();

            if (!(await domainManager.IsValid(user)))
            {
                context.SetError("invalid_grant", "Usuário ou senha incorretos.");
                return;
            }

            var federation = context.OwinContext.Get<FederationProvider>();

            System.IdentityModel.Tokens.SecurityToken token;
            if ((token = (await federation.SignIn(user))) == null)
            {
                context.SetError("invalid_grant", "Usuário ou senha incorretos.");
                return;
            }

            ClaimsIdentity oAuthIdentity = new ClaimsIdentity("JWT");

            var ticket = new AuthenticationTicket(oAuthIdentity, null);

            context.Validated(ticket);
        }
    }
}