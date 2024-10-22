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
    public class ContentFileDAO
    {
        /// <summary>
        /// 添加挂接记录
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="datafrom"></param>
        /// <returns></returns>
        public DataTable AddArchiveLinkRecord(string fileCodeName, string datafrom)
        {
            string sql = "INSERT  t_archive_link(code_name,data_from,start_time) \r\n";
            sql += " VALUES('" + fileCodeName + "','" + datafrom + "','" + DateTime.Now.ToString() + "'); \r\n";
            sql += "SELECT TOP 1 Unique_code FROM t_archive_link WHERE code_name='" + fileCodeName + "' ORDER BY Unique_code DESC";
            DataTable uniquecodeDt = new DbHelper().Fill(sql);
            return uniquecodeDt;
        }

        /// <summary>
        /// 更新挂接记录
        /// </summary>
        /// <param name="total"></param>
        /// <param name="success"></param>
        /// <param name="endtime"></param>
        /// <param name="uniquecode"></param>
        public void UpdateArchiveLinkRecord(int total, int success, string endtime, string uniquecode)
        {
            string sql = "UPDATE t_archive_link SET total_count='" + total + "',success_count='" + success + "',end_time='" + endtime + "',link_status='2' WHERE Unique_code=" + uniquecode.ToString();
            new DbHelper().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 动态创建临时表，此临时表用于批量挂接
        /// </summary>
        /// <param name="contentFormat"></param>
        /// <param name="tempTableName"></param>
        public void CreateTempTableForArchiveLink(string contentFormat, string tempTableName)
        {
            string sql = string.Empty;
            if (contentFormat.ToLower().Contains("pdf"))
            {
                sql = "IF OBJECT_ID(N'" + tempTableName + "',N'U') IS NOT NULL \r\n";
                sql += "DELETE FROM " + tempTableName + " \r\n";
                sql += "ELSE \r\n";
                sql += "CREATE TABLE [dbo].[" + tempTableName + "]( \r\n";
                sql += "[DH] [nvarchar](50) NOT NULL,[FilePath] [nvarchar](500) NOT NULL \r\n";
                sql += ") ON [PRIMARY]";
            }
            else//JPG，TIFF等图片格式
            {
                sql = "IF OBJECT_ID(N'" + tempTableName + "',N'U') IS NOT NULL \r\n";
                sql += "DELETE FROM " + tempTableName + " \r\n";
                sql += "ELSE \r\n";
                sql += "CREATE TABLE [dbo].[" + tempTableName + "]( \r\n";
                sql += "[DH] [nvarchar](50) NOT NULL,[FilePath] [xml] \r\n";
                sql += ") ON [PRIMARY]";
            }
            new DbHelper().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 批量更新原文字段（添加原文信息）PDF
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="tempTableName"></param>
        /// <param name="archiveNoFieldName"></param>
        /// <returns></returns>
        public DataTable AddYWPath(string fileCodeName, string tempTableName, string archiveNoFieldName)
        {
            string sql = "UPDATE t1 SET yw= t2.FilePath FROM " + fileCodeName + " t1 INNER JOIN " + tempTableName + " t2 ON t1." + archiveNoFieldName + "=t2.DH \r\n";
            sql += "SELECT @@ROWCOUNT \r\n";
            sql += "DROP TABLE " + tempTableName;
            DataTable successcountDt = new DbHelper().Fill(sql);//别使用new DbHelper().ExcuteScaler，多线程时会报错。
            return successcountDt;
        }

        /// <summary>
        /// 批量更新原文字段（添加原文信息）IMAGE
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="tempTableName"></param>
        /// <param name="archiveNoFieldName"></param>
        /// <returns></returns>
        public DataTable AddImageYWPath(string fileCodeName, string tempTableName, string archiveNoFieldName)
        {
            string sql = "UPDATE t1 SET yw_xml= t2.FilePath FROM " + fileCodeName + " t1 INNER JOIN " + tempTableName + " t2 ON t1." + archiveNoFieldName + "=t2.DH \r\n";
            sql += "SELECT @@ROWCOUNT \r\n";
            sql += "DROP TABLE " + tempTableName;
            DataTable successcountDt = new DbHelper().Fill(sql);//别使用new DbHelper().ExcuteScaler，多线程时会报错。
            return successcountDt;
        }


        /// <summary>
        /// 判断原文是否已挂接
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public object VerifyYWAlreadyLinked(string fileCodeName, string where)
        {
            string sql0 = "SELECT COUNT(1) FROM " + fileCodeName + " WHERE " + where + " AND yw IS NOT NULL";
            object recCount = new DbHelper().ExecuteScalar(sql0);
            return recCount;
        }

        /// <summary>
        /// 判断原文是否已挂接
        /// </summary>
        /// <param name="uniqueCode"></param>
        /// <returns></returns>
        public object VerifyYWAlreadyLinked2(string uniqueCode,string fileCodeName)
        {
            string sql = "SELECT yw FROM " + fileCodeName + " WHERE Unique_code=" + uniqueCode;
            object ywPathStr = new DbHelper().ExecuteScalar(sql);
            return ywPathStr;
        }

        /// <summary>
        /// 判断IMAGE原文是否挂接
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="archiveNoFieldName"></param>
        /// <param name="uniquecode"></param>
        /// <returns></returns>
        public DataTable VerifyImageYWAlreadyLinked(string fileCodeName, string archiveNoFieldName, string uniquecode)
        {
            string sql = "SELECT yw_xml," + archiveNoFieldName + " FROM " + fileCodeName + " WHERE Unique_code=" + uniquecode;
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }
    }
}
