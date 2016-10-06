using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adfs.Server.Api.Models
{
    public class ApplicationUser
    {
        public ApplicationUser(string userName, string password)
        {
            this.UserName = userName;
            this.Password = password;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}