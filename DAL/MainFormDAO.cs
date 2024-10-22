using DotNet.DbUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class MainFormDAO
    {
        public DataTable LoadTree()
        {
            string sql = "SELECT t1.Unique_code,t1.super_id,t1.name,code,t1.node_type,t2.code_value FROM t_config_type_tree AS t1 LEFT JOIN t_config_codes AS t2 ON t1.content_type=t2.Unique_code";
            DataTable overAll = new DbHelper().Fill(sql);
            return overAll;
        }

        public DataTable VerifyMenusVisibilityOnUserLoggedIn(string roleId)//根据登录者的权限，设置工具栏各栏目的可见性
        {
            string sql = "SELECT caption,name,page_name_directedto,deal_visible,deal_search,deal_add,deal_update,deal_delete \r\n";
            sql += " FROM t_config_access WHERE role_id='" + roleId + "'";
            DataTable dtFromDB = new DbHelper().Fill(sql);
            return dtFromDB;
        }

    }
}
