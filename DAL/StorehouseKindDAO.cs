using DotNet.DbUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class StorehouseKindDAO
    {
        public DataTable LoadStoreHouseTree()
        {
            string sql = "SELECT Unique_code,super_id,name '档案类型名',code '档案类型值',node_type '是否非空节点',order_id '排序',has_content '是否有原文',content_type '原文类型',use_flag '是否可用' FROM t_config_type_tree";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        public DataTable LoadStoreHouseTree2()
        {
            string sql = "SELECT t1.Unique_code,t1.super_id,t1.name,code FROM t_config_type_tree AS t1";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        /// <summary>
        /// 当删除一个档案库类型时，同时删除与之相关的表中数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="uniquecode"></param>
        public void ClearTempDataFromTables(string table, object uniquecode)
        {
            string sql = "DELETE FROM t_config_type_tree WHERE Unique_code=@Unique_code \r\n";
            sql += "IF OBJECT_ID(N'[" + table + "]',N'U') IS NOT NULL \r\n";
            sql += "BEGIN \r\n";
            sql += "  DELETE FROM [" + table + "] \r\n";
            sql += "  DELETE FROM t_config_field_show_list WHERE selected_code IN(SELECT Unique_code FROM t_config_col_dict WHERE code='" + table + "') \r\n";
            sql += "  DELETE FROM t_config_manual_check WHERE selected_code IN(SELECT Unique_code FROM t_config_col_dict WHERE code='" + table + "') \r\n";
            sql += "  DELETE FROM t_config_fields_group_for_autocheck WHERE selected_code IN(SELECT Unique_code FROM t_config_col_dict WHERE code='" + table + "') \r\n";
            sql += "  DELETE FROM t_config_primary_search WHERE selected_code IN(SELECT Unique_code FROM t_config_col_dict WHERE code='" + table + "') \r\n";
            sql += "  DELETE FROM t_config_col_dict WHERE code='" + table + "' \r\n";
            sql += "  DELETE FROM t_config_field_for_autocheck WHERE table_code='" + table + "' \r\n";
            sql += "  DROP TABLE [" + table + "]\r\n";
            sql += "END \r\n";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("Unique_code", uniquecode);
            DbParameter[] param = new DbParameter[] { para1 };
            helper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 删除名带temp的临时表
        /// </summary>
        public void DropTablesWithTempName()
        {
            string sqla = "DECLARE @tableName VARCHAR(20),@count INT,@i INT,@SQL VARCHAR(200); \r\n";
            sqla += "SELECT * INTO #tmpTable FROM (SELECT name,ROW_NUMBER() over(order by name) as myRow FROM sys.objects WHERE type = 'U' AND name LIKE '%temp%') b; \r\n";
            sqla += "SET @i=1; \r\n";
            sqla += "SELECT @count=COUNT(*) FROM #tmpTable; \r\n";
            sqla += "WHILE (@i <= @count) \r\n";
            sqla += "BEGIN \r\n";
            sqla += "  SELECT @tableName=name FROM #tmpTable WHERE myRow = @i; \r\n";
            sqla += "  SET @SQL = 'DROP TABLE ' + @tableName; \r\n";
            sqla += "  EXEC(@SQL); \r\n";
            sqla += "  SET @i=@i+1; \r\n";
            sqla += "END \r\n";
            sqla += "DROP TABLE #tmpTable \r\n";
            new DbHelper().ExecuteNonQuery(sqla);
        }

        /// <summary>
        /// 增加档案库类型的节点
        /// </summary>
        /// <param name="superId"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="nodeType"></param>
        /// <param name="userFlag"></param>
        /// <param name="orderId"></param>
        /// <param name="hasContent"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public object AddStoreHouseNode(object superId,object name,object code,object nodeType,object userFlag,object orderId,object hasContent,object contentType)
        {
            string sql = "INSERT INTO t_config_type_tree(name,code,super_id,order_id,node_type,has_content,content_type,use_flag) \r\n";
            sql += "VALUES(@name,@code,@super_id,@order_id,@node_type,@has_content,@content_type,@use_flag) \r\n";
            sql += "SELECT TOP 1 Unique_code FROM t_config_type_tree ORDER BY Unique_code DESC";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("super_id", superId);
            DbParameter para2 = helper.MakeInParam("name", name.ToString());
            DbParameter para3 = helper.MakeInParam("code", code.ToString());
            DbParameter para4 = helper.MakeInParam("node_type", nodeType);
            DbParameter para5 = helper.MakeInParam("use_flag", userFlag);
            DbParameter para6 = helper.MakeInParam("order_id", orderId == DBNull.Value ? 0 : int.Parse(orderId.ToString()));
            DbParameter para7 = helper.MakeInParam("has_content", hasContent);
            DbParameter para8 = helper.MakeInParam("content_type", contentType);
            DbParameter[] param = new DbParameter[] { para1, para2, para3, para4, para5, para6, para7, para8 };
            object newUniqueId = helper.ExecuteScalar(sql, param);
            return newUniqueId;
        }
        /// <summary>
        /// 更新档案库类型的节点
        /// </summary>
        /// <param name="uniqueCode"></param>
        /// <param name="name"></param>
        /// <param name="orderId"></param>
        /// <param name="contentType"></param>
        public void UpdateStoreHouseNode(object uniqueCode,object name,object orderId,object contentType)
        {
            string sql = "UPDATE t_config_type_tree SET name=@name,order_id=@order_id,content_type=@content_type WHERE Unique_code=@Unique_code";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("Unique_code", uniqueCode);
            DbParameter para2 = helper.MakeInParam("name", name.ToString());
            DbParameter para3 = helper.MakeInParam("order_id", orderId == DBNull.Value ? 0 : int.Parse(orderId.ToString()));
            DbParameter para4 = helper.MakeInParam("content_type", contentType);
            DbParameter[] param = new DbParameter[] { para1, para2, para3, para4 };
            helper.ExecuteNonQuery(sql, param);
        }
    }
}
