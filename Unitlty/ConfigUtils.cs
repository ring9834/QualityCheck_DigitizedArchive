using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.DbUtilities;

namespace Prj_FileManageNCheckApp
{
    public class ConfigUtils
    {
        public static bool VerifyIfBusinessDictionaryConfigured(string tableName)
        {
            string sql = "SELECT COUNT(*) FROM  t_config_col_dict WHERE code='"+ tableName +"'";
            object count = new DbHelper().ExecuteScalar(sql);
            if (count == null || count == DBNull.Value)
            {
                return false;
            }
            if (int.Parse(count.ToString()) > 0)
            {
                return true;
            }
            return false;
        }
    }
}
