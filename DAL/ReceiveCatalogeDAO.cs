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
    public class ReceiveCatalogeDAO
    {
        /// <summary>
        /// 保存导入目录的历史记录
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="excelFileName"></param>
        /// <param name="importTime"></param>
        /// <param name="importUser"></param>
        /// <param name="recordNumber"></param>
        /// <param name="configName"></param>
        /// <param name="configXML"></param>
        /// <returns></returns>
        public object SaveImportCatalogeRecord(string fileCodeName, string excelFileName, string importTime, string importUser, string recordNumber, string configName, string configXML)
        {
            string sql = "INSERT t_config_imp_catalogs_rec(table_code,excel_file_name,import_time,import_user,record_number,config_name,config_xml) \r\n";
            sql += "VALUES('" + fileCodeName + "','" + excelFileName + "','" + importTime + "','" + importUser + "','" + recordNumber + "','" + configName + "','" + configXML + "')";
            new DbHelper().ExecuteNonQuery(sql);//返回本次导入的批次号
            sql = "SELECT TOP 1 Unique_code from t_config_imp_catalogs_rec ORDER BY Unique_code DESC";
            object bundleNumber = new DbHelper().ExecuteScalar(sql);
            return bundleNumber;
        }

        /// <summary>
        /// 判断目录是否已经接收过
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileCodeName"></param>
        /// <returns></returns>
        public object VerifyIsCatalogueAcceptedAgain(DataTable dt,string fileCodeName)
        {
            string condition = string.Empty;
            DbHelper helper = new DbHelper();
            DbParameter[] param = new DbParameter[dt.Columns.Count];
            int rowCount = dt.Rows.Count;
            Random random = new Random();
            int index = random.Next(1, rowCount);//取小于记录行数的随即数

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataRow dr = dt.Rows[index];
                string colName = dt.Columns[i].ColumnName;
                if (i == 0)
                    condition = colName + "=@" + colName;
                else
                    condition += " AND " + colName + "=@" + colName;
                param[i] = helper.MakeInParam(colName, dr[colName]);
            }
            string sql = "SELECT COUNT(*) AS count FROM " + fileCodeName + " WHERE " + condition;
            object count = helper.ExecuteScalar(sql, param);
            return count;
        }

        /// <summary>
        /// 取得某表格各列定义的允许长度
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <returns></returns>
        public DataTable GetColumnLengthInfoByFileCodeName(string fileCodeName)
        {
            string checkSql = "SELECT name,TYPE_NAME(SYSTEM_TYPE_ID)AS fieldType,MAX_LENGTH AS fieldLenth FROM sys.columns WHERE OBJECT_ID=OBJECT_ID('" + fileCodeName + "')";
            DataTable dtCheck = new DbHelper().Fill(checkSql);
            return dtCheck;
        }

        /// <summary>
        /// 根据目录导入批次得到第一条记录的档号值，用以判断档号是否生成
        /// </summary>
        /// <param name="dhFieldName"></param>
        /// <param name="fileCodeName"></param>
        /// <param name="bundleNumber"></param>
        /// <returns></returns>
        public object GetDHValueByImportBundle(object dhFieldName, string fileCodeName, string bundleNumber)
        {
            string sql = "SELECT " + dhFieldName.ToString() + " FROM " + fileCodeName + " WHERE import_bundle= '" + bundleNumber + "'";
            object dhRecord = new DbHelper().ExecuteScalar(sql);
            return dhRecord;
        }
    }
}
