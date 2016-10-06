using Adfs.Server.Api.Models;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.Protocols.WSTrust.Bindings;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading.Tasks;

namespace Adfs.Server.Api.Providers
{
    public class FederationProvider : IDisposable
    {
        private readonly string stsEndpoint;// = "https://adfs.sp13dev.com/adfs/services/trust/13/usernamemixed";
        private string relyingPartyUri;// = "urn:sharepoint:solhm";

        public FederationProvider(string relyingParty = "urn:sharepoint:solhm", string stsEndpoint = "https://adfs.sp13dev.com/adfs/services/trust/13/usernamemixed")
        {
            this.stsEndpoint = stsEndpoint;
            this.relyingPartyUri = relyingParty;
        }

        public async Task<System.IdentityModel.Tokens.SecurityToken> SignIn(ApplicationUser user)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
               delegate (object sender, X509Certificate certificate, X509Chain chain,
                   SslPolicyErrors sslPolicyErrors) { return true; };

            Microsoft.IdentityModel.Protocols.WSTrust.WSTrustChannelFactory factory =
                new Microsoft.IdentityModel.Protocols.WSTrust.WSTrustChannelFactory(
            new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential),
            new EndpointAddress(stsEndpoint));

            factory.TrustVersion = TrustVersion.WSTrust13;

            // Username and Password here...
            factory.Credentials.UserName.UserName = user.UserName;// "93305273100";
            factory.Credentials.UserName.Password = user.Password;// "Caixa123";

            RequestSecurityToken rst = new RequestSecurityToken
            {
                RequestType = Microsoft.IdentityModel.Protocols.WSTrust.WSTrust13Constants.RequestTypes.Issue,
                AppliesTo = new System.ServiceModel.EndpointAddress(relyingPartyUri),
                KeyType = Microsoft.IdentityModel.Protocols.WSTrust.WSTrust13Constants.KeyTypes.Bearer,
            };

            Microsoft.IdentityModel.Protocols.WSTrust.IWSTrustChannelContract channel = factory.CreateChannel();

            System.IdentityModel.Tokens.SecurityToken token = channel.Issue(rst);

            return token;
        }

        public static FederationProvider Create()
        {
            return new FederationProvider();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FederationProvider() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}