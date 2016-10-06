using Adfs.Server.Api.Models;
using Novell.Directory.Ldap;
using System;
using System.Threading.Tasks;

namespace Adfs.Server.Api.Providers
{
    public class DomainAuthenticationProvider : IDisposable
    {
        private readonly ApplicationUser _credentials;
        private readonly LdapOptions _ldapOptions;

        public DomainAuthenticationProvider(LdapOptions ldapOptions)
        {
            this._ldapOptions = ldapOptions;
        }
        public DomainAuthenticationProvider(ApplicationUser credentials, LdapOptions ldapOptions)
        {
            _credentials = credentials;
            _ldapOptions = ldapOptions;
        }

        public async Task<bool> IsValid(ApplicationUser credentials)
        {
            bool correct = false;

            try
            {
                LdapConnection conn = new LdapConnection();
                conn.Connect(_ldapOptions.server, Convert.ToInt32(_ldapOptions.porta));
                conn.Bind(String.Format("CN={0},{1},{2}", credentials.UserName, _ldapOptions.ou, _ldapOptions.dc), credentials.Password);
                conn.Disconnect();

                correct = true;
            }
            catch (LdapException e)
            {
                Console.WriteLine("Error:" + e.LdapErrorMessage);

            }

            return correct;
        }
        public async Task<bool> IsValid()
        {
            return await this.IsValid(this._credentials);
        }

        public static DomainAuthenticationProvider Create()
        {
            return new DomainAuthenticationProvider(new LdapOptions()
            {
                domain = System.Configuration.ConfigurationManager.AppSettings["as:domain"], //"sp13dev.com",
                server = System.Configuration.ConfigurationManager.AppSettings["as:server"],//"10.1.1.60",
                porta = System.Configuration.ConfigurationManager.AppSettings["as:port"],//"389",
                ou = System.Configuration.ConfigurationManager.AppSettings["as:ou"],//"Ou=Usuarios_Externos_Vitrine",
                dc = System.Configuration.ConfigurationManager.AppSettings["as:dc"]//"DC=sp13dev,DC=com"
            });
        }

        public class LdapOptions
        {

            public string domain { get; set; }

            public string server { get; set; }

            public string porta { get; set; }

            public string ou { get; set; }

            public string dc { get; set; }
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
        // ~DomainAuthenticationProvider() {
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