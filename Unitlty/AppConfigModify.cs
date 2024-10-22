using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Prj_FileManageNCheckApp
{
    public class AppConfigModify
    {
        public static void ModifyItem(string keyName, string newKeyValue)
        {
            //修改配置文件中键为keyName的项的值  
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[keyName].Value = newKeyValue;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }  
    }

}
