using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareDev_Test
{
    class Account
    {

        public string UserName { get; }
        public string Password { get; }

        public Account()
        {
            UserName = Tools.ReadConfigValue("UserName");
            Password = Tools.ReadConfigValue("Password");
        }

    }
}
