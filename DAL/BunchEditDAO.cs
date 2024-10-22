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
    public class BunchEditDAO
    {
        /// <summary>
        /// 获得一个供选择的值列表，供某字段使用，如密级、保管期限等字段
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public DataTable GetColumnSelectingValuesByColName(string fileCodeName, string colName)
        {
            string sql = "SELECT code_name,code_value,Unique_code FROM t_config_codes WHERE parent_code IN(SELECT col_show_value FROM t_config_col_dict WHERE col_name=@col_name AND code=@code)";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("code", fileCodeName);
            DbParameter para2 = helper.MakeInParam("col_name", colName);
            DbParameter[] param = new DbParameter[] { para1, para2 };
            DataTable dt = helper.Fill(sql, param);
            return dt;
        }

        /// <summary>
        /// 内容全部替换
        /// </summary>
        /// <param name="whereFieldList"></param>
        /// <param name="whereFieldValueList"></param>
        /// <param name="fileCodeName"></param>
        /// <param name="colName"></param>
        /// <param name="colValue"></param>
        /// <param name="where"></param>
        public void ModifyFieldValueWholy(List<string> whereFieldList, List<string> whereFieldValueList, string fileCodeName, string colName, string colValue, string where)
        {
            string sql = string.Empty;
            DbHelper helper = new DbHelper();
            DbParameter[] param = new DbParameter[whereFieldList.Count + 1];
            for (int i = 0; i < whereFieldList.Count; i++)
            {
                param[i] = helper.MakeInParam(whereFieldList[i], whereFieldValueList[i]);
            }
            sql = "UPDATE " + fileCodeName + " SET " + colName + "=@Replacement WHERE " + where;
            DbParameter para2 = helper.MakeInParam("Replacement", colValue);
            param[whereFieldList.Count] = para2;
            //DbParameter[] param = new DbParameter[] { para2 };
            helper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 内容部分替换
        /// </summary>
        /// <param name="whereFieldList"></param>
        /// <param name="whereFieldValueList"></param>
        /// <param name="fileCodeName"></param>
        /// <param name="colName"></param>
        /// <param name="valueReplacing"></param>
        /// <param name="valueReplaced"></param>
        /// <param name="where"></param>
        public void ModifyFieldValuePartly(List<string> whereFieldList, List<string> whereFieldValueList, string fileCodeName, string colName, string valueReplacing, string valueReplaced, string where)
        {
            string sql = string.Empty;
            DbHelper helper = new DbHelper();
            DbParameter[] param = new DbParameter[whereFieldList.Count];
            for (int i = 0; i < whereFieldList.Count; i++)
            {
                param[i] = helper.MakeInParam(whereFieldList[i], whereFieldValueList[i]);
            }
            sql = "UPDATE " + fileCodeName + " SET " + colName + "=REPLACE(" + colName + ",'" + valueReplacing + "','" + valueReplaced + "')\r\n";
            sql += "WHERE " + where;
            helper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 空白内容替换为指定值
        /// </summary>
        /// <param name="whereFieldList"></param>
        /// <param name="whereFieldValueList"></param>
        /// <param name="fileCodeName"></param>
        /// <param name="colName"></param>
        /// <param name="colValue"></param>
        /// <param name="where"></param>
        public void ModifyNullFieldValue(List<string> whereFieldList, List<string> whereFieldValueList, string fileCodeName, string colName, string colValue, string where)
        {
            string sql = string.Empty;
            DbHelper helper = new DbHelper();
            DbParameter[] param = new DbParameter[whereFieldList.Count];
            for (int i = 0; i < whereFieldList.Count; i++)
            {
                param[i] = helper.MakeInParam(whereFieldList[i], whereFieldValueList[i]);
            }
            sql = "UPDATE " + fileCodeName + " SET " + colName + "='" + colValue + "'\r\n";
            sql += "WHERE (ltrim(rtrim(" + colName + "))='' OR " + colName + " IS NULL) AND " + where;
            helper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 删除查询到的列表中所有值
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="where"></param>
        public void DeleteRecordsSelected(string fileCodeName, string where)
        {
            string sql = "DELETE FROM " + fileCodeName + " WHERE " + where;
            new DbHelper().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 从指定档案库中删除某一条记录
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="uniqueCode"></param>
        public void DeleteSpecificRecordFromStoreHouseTable(string fileCodeName, object uniqueCode)
        {
            string sql = "DELETE FROM " + fileCodeName + " WHERE Unique_code=@Unique_code";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("Unique_code", uniqueCode);
            DbParameter[] param = new DbParameter[] { para1 };
            helper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 修改指定档案库中一条记录
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="count"></param>
        /// <param name="setStrForUpdate"></param>
        /// <param name="uniqueCode"></param>
        public void ModifyARecordInStoreHouseTable(string fileCodeName, string setStrForUpdate, string uniqueCode, DbParameter[] param)
        {
            DbHelper helper = new DbHelper();
            string sql = "UPDATE " + fileCodeName + " SET " + setStrForUpdate + " WHERE Unique_code=" + uniqueCode;
            helper.ExecuteNonQuery(sql, param);
        }
    }
}
