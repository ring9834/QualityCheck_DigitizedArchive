using DotNet.DbUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class SearchDAO
    {
        /// <summary>
        /// 获得已经配置的用于搜索的字段信息
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <returns></returns>
        public DataTable GetFieldsUsedForSearch(string fileCodeName)
        {
            string sql = "SELECT t2.col_name,t2.show_name,t3.code_value,t3.code_name FROM t_config_primary_search AS t1 INNER JOIN t_config_col_dict AS t2 ON t1.selected_code=t2.Unique_code\r\n ";
            sql += "INNER JOIN t_config_codes AS t3 ON t1.search_code=t3.Unique_code \r\n";
            sql += "WHERE t2.code='" + fileCodeName + "' ORDER BY t1.order_number ASC";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        /// <summary>
        /// 获得已配置（已选）字段信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetConfigurededFieds(string fileCodeName)
        {
            string sql = "SELECT t2.Unique_code,t2.col_name,t2.show_name AS '已选字段',t1.search_code FROM t_config_primary_search AS t1 INNER JOIN t_config_col_dict AS t2 ON t1.selected_code=t2.Unique_code WHERE t2.code='" + fileCodeName + "' ORDER BY order_number ASC";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        /// <summary>
        /// 保存配置的搜索字段
        /// </summary>
        /// <param name="selectedcode"></param>
        /// <param name="uniqueCode"></param>
        /// <param name="i"></param>
        public void SaveConfiguredFieldsForSearch(string selectedcode, int uniqueCode, int i)
        {
            string sql = "DECLARE @colName nvarchar(50) \r\n";
            sql += "SELECT @colName = (SELECT col_name FROM t_config_col_dict WHERE Unique_code IN(SELECT selected_code FROM t_config_primary_search WHERE selected_code=" + selectedcode + "))\r\n";
            sql += "IF @colName IS NULL\r\n";
            sql += "INSERT t_config_primary_search(selected_code,order_number,search_code) VALUES(" + selectedcode + ",'" + i.ToString() + "'," + uniqueCode + ")\r\n";
            sql += "ELSE \r\n";
            sql += "UPDATE t_config_primary_search SET order_number='" + i.ToString() + "',search_code=" + uniqueCode + " WHERE selected_code=" + selectedcode + "\r\n";
            new DbHelper().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 删除原来在现在不在已配置字段列表中的字段
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="codeStr"></param>
        public void DeleteFieldsNotInConfiguredFieldList(string fileCodeName, string codeStr)
        {
            string sql = "DELETE FROM t_config_primary_search WHERE selected_code NOT IN(" + codeStr + ") AND selected_code IN (SELECT Unique_code FROM t_config_col_dict WHERE code='" + fileCodeName + "')";
            new DbHelper().ExecuteNonQuery(sql);
        }
    }
}
