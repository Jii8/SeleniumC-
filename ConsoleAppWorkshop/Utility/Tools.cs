using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareDev_Test
{
    class Tools
    {
       /// <summary>
       /// To read value from the app config file based on the key
       /// </summary>
       /// <param name="key"></param>
       /// <returns></returns>
        public static string ReadConfigValue(string key)
        {
            System.Configuration.AppSettingsReader cfgConfiguration = new System.Configuration.AppSettingsReader();
            string value;

            try
            {
                value = cfgConfiguration.GetValue(key, typeof(string)).ToString();
                value = value.Trim();
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("does not exist in the appSettings configuration") > -1)
                {
                    value = null;
                }
                else
                {
                    throw new Exception("Tools.ReadConfigValue : " + ex.Message, ex);
                }
            }
            
            return value;
        }
    }
}
