using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

namespace Adfs.Server.Api.App_Start
{
	public partial class Startup
	{
        public void ConfigureOAuth(IAppBuilder app)
        {
            // Setup Authorization Server
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                ApplicationCanDisplayErrors = true,
                //AccessTokenFormat = new 
                TokenEndpointPath = new PathString(),
                AuthorizeEndpointPath = new PathString(),
#if DEBUG
                AllowInsecureHttp = true,
#endif
                // Authorization server provider which controls the lifecycle of Authorization Server
                Provider = new OAuthAuthorizationServerProvider
                {
                    OnValidateClientAuthentication = ValidateClientAuthentication,
                    OnGrantResourceOwnerCredentials = GrantResourceOwnerCredentials,
                }
            });
        }

        private Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //string clientId;
            //string clientSecret;
            //if (context.TryGetBasicCredentials(out clientId, out clientSecret) ||
            //    context.TryGetFormCredentials(out clientId, out clientSecret))
            //{
            //    if (clientId == Clients.Client1.Id && clientSecret == Clients.Client1.Secret)
            //    {
            //        context.Validated();
            //    }
            //    else if (clientId == Clients.Client2.Id && clientSecret == Clients.Client2.Secret)
            //    {
            //        context.Validated();
            //    }
            //}
            context.Validated();
            return Task.FromResult(0);
        }

        private Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(new GenericIdentity(context.UserName, OAuthDefaults.AuthenticationType), context.Scope.Select(x => new Claim("urn:oauth:scope", x)));

            context.Validated(identity);

            return Task.FromResult(0);
        }
    }
}