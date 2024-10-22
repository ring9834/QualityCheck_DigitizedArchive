
using DotNet.DbUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class FieldShowListDAO
    {
        /// <summary>
        /// 获得指定档案库类型对应的字段显示列表
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <returns></returns>
        public DataTable GetFieldsShownInList(string fileCodeName)
        {
            string sql = "SELECT t2.Unique_code,t2.col_name,t2.show_name FROM t_config_field_show_list AS t1 INNER JOIN t_config_col_dict AS t2 ON t1.selected_code=t2.Unique_code WHERE t2.code='" + fileCodeName + "' ORDER BY CAST(t1.order_number AS INT) ASC";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        /// <summary>
        /// 获得指定档案库类型对应的字段显示列表
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <returns></returns>
        public DataTable GetFieldsShownInList2(string fileCodeName)
        {
            string sql = "SELECT t2.col_name,t2.show_name FROM t_config_field_show_list AS t1 INNER JOIN t_config_col_dict AS t2 ON t1.selected_code=t2.Unique_code WHERE t2.code='" + fileCodeName + "' ORDER BY CAST(t1.order_number AS INT) ASC";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        /// <summary>
        /// 保存显示列表，包括显示顺序
        /// </summary>
        /// <param name="selectedcode"></param>
        /// <param name="i"></param>
        public void SaveFieldsShowList(string selectedcode,int i)
        {
            string sql = "DECLARE @colName nvarchar(50) \r\n";
            sql += "SELECT @colName = (SELECT col_name FROM t_config_col_dict WHERE Unique_code IN(SELECT selected_code FROM t_config_field_show_list WHERE selected_code=" + selectedcode + "))\r\n";
            sql += "IF @colName IS NULL\r\n";
            sql += "INSERT t_config_field_show_list(selected_code,order_number) VALUES(" + selectedcode + ",'" + i.ToString() + "')\r\n";
            sql += "ELSE \r\n";
            sql += "UPDATE t_config_field_show_list SET order_number='" + i.ToString() + "' WHERE selected_code=" + selectedcode + "\r\n";
            new DbHelper().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 删除原来在列表中，但后来不再列表中的列
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="codeStr"></param>
        public void DeleteFieldsFromList(string fileCodeName, string codeStr)
        {
            string sql = "DELETE FROM t_config_field_show_list WHERE selected_code NOT IN(" + codeStr + ") AND selected_code IN (SELECT Unique_code FROM t_config_col_dict WHERE code='" + fileCodeName + "')";
            new DbHelper().ExecuteNonQuery(sql);
        }
    }
}
