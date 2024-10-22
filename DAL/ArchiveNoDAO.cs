using DotNet.DbUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class ArchiveNoDAO
    {
        /// <summary>
        /// 取得指定档案库类型对应的档号字段信息
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <returns></returns>
        public DataTable GetArchiveNoDataByCodeName(string fileCodeName)
        {
            string sql = "SELECT * FROM t_config_archive_num_makeup WHERE code_name='" + fileCodeName + "'";
            DataTable dtmakeup = new DbHelper().Fill(sql);
            return dtmakeup;
        }

        /// <summary>
        /// 从档案库中取得档号值
        /// </summary>
        /// <param name="dhfield"></param>
        /// <param name="fileCodeName"></param>
        /// <param name="uniquecode"></param>
        /// <returns></returns>
        public object GetArchiveNoVale(object dhfield, string fileCodeName, string uniquecode)
        {
            string sql = "SELECT " + dhfield.ToString() + " FROM " + fileCodeName + " WHERE Unique_code=" + uniquecode;
            object dh = new DbHelper().ExecuteScalar(sql);
            return dh;
        }

        /// <summary>
        /// 取得指定档案库类型对应的档号字段信息
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <returns></returns>
        public object GetArchiveNoDataByCodeName2(string fileCodeName)
        {
            string sql = string.Empty;
            sql = "SELECT archive_num_field FROM t_config_archive_num_makeup WHERE code_name='" + fileCodeName + "'";
            object archiveNoFieldName = new DbHelper().ExecuteScalar(sql);
            return archiveNoFieldName;
        }

        /// <summary>
        /// 增加或更新某档案库类型对应的档号规则
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="archive_num_parts_amount"></param>
        /// <param name="archive_prefix"></param>
        /// <param name="archive_body"></param>
        /// <param name="connect_char"></param>
        /// <param name="archive_num_field"></param>
        /// <returns></returns>
        public int InsertOrUpdateArchiveNoInfo(string fileCodeName, string archive_num_parts_amount, string archive_prefix, string archive_body, string connect_char, string archive_num_field)
        {
            string sql = string.Empty;
            sql += "IF (SELECT COUNT(*) FROM t_config_archive_num_makeup WHERE code_name='" + fileCodeName + "') = 0 \r\n";
            sql += "BEGIN \r\n";
            sql += "INSERT t_config_archive_num_makeup(archive_num_parts_amount,archive_prefix,archive_body,connect_char,archive_num_field,code_name)\r\n";
            sql += "VALUES('" + archive_num_parts_amount + "','" + archive_prefix + "','" + archive_body + "','" + connect_char + "','" + archive_num_field + "','" + fileCodeName + "') \r\n";
            sql += "END \r\n";
            sql += "ELSE \r\n";
            sql += "BEGIN \r\n";
            sql += "UPDATE  t_config_archive_num_makeup SET archive_num_parts_amount='" + archive_num_parts_amount + "',archive_prefix='" + archive_prefix + "',archive_body='" + archive_body + "',\r\n";
            sql += "connect_char='" + connect_char + "', archive_num_field='" + archive_num_field + "' WHERE code_name='" + fileCodeName + "'\r\n";
            sql += "END \r\n";
            int result = new DbHelper().ExecuteNonQuery(sql);
            return result;
        }

        /// <summary>
        /// 批量生成档号
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="dhfield"></param>
        /// <param name="dhstr"></param>
        /// <param name="whereCondtionForAchiveNumMakeup"></param>
        public void MakeArchiveNumber(string fileCodeName, string dhfield, string dhstr, string whereCondtionForAchiveNumMakeup)
        {
            //sql = "DECLARE @uniquecode int \r\n";
            //sql += "DECLARE " + declareStr + "\r\n";
            //sql += "DECLARE mycursor CURSOR FOR ";
            //sql += "(SELECT Unique_code," + selectField + " FROM " + this.FileCodeName + " WHERE " + this.WhereCondtionForAchiveNumMakeup + ") \r\n";
            //sql += "OPEN mycursor \r\n";
            //sql += "FETCH NEXT FROM mycursor INTO @uniquecode," + fetchInto + "\r\n";
            //sql += "WHILE @@FETCH_STATUS = 0 \r\n";
            //sql += "BEGIN \r\n";
            string sql = "UPDATE " + fileCodeName + " SET " + dhfield + "=" + dhstr + " WHERE " + whereCondtionForAchiveNumMakeup + "\r\n";
            //sql += "FETCH NEXT FROM mycursor INTO @uniquecode," + fetchInto + "\r\n";
            //sql += "END \r\n";
            //sql += "DEALLOCATE mycursor";

            //sql = "DECLARE @REFID INT, @ORDERID VARCHAR(30) \r\n";
            //sql += "DECLARE " + declareStr + "\r\n";
            //sql += "SELECT  REFID = IDENTITY(INT , 1, 1), DealFlg = 0, " + fetchInto2 + " INTO #Temp_Lists FROM " + this.FileCodeName + " WHERE " + this.WhereCondtionForAchiveNumMakeup + ") \r\n";
            //sql += "SELECT @REFID = MIN(REFID) FROM #Temp_Lists WHERE DealFlg = 0 \r\n";
            //sql += "WHILE @REFID IS NOT NULL \r\n";
            //sql += " BEGIN \r\n";
            //sql += " SELECT " + fetchInto + " FROM  #Temp_Lists WHERE REFID = @REFID \r\n";
            //sql += " UPDATE " + this.FileCodeName + " SET " + dhfield + "=" + dhstr + " WHERE " + this.WhereCondtionForAchiveNumMakeup + "\r\n";
            //sql += " UPDATE #Temp_Lists SET DealFlg = 1 WHERE REFID = @REFID \r\n";
            //sql += " SELECT @REFID = MIN(REFID) FROM #Temp_Lists WHERE DealFlg = 0 AND REFID > @REFID \r\n";
            //sql += " END \r\n";
            //sql += "DROP TABLE #Temp_Lists";

            new DbHelper().ExecuteNonQuery(sql);
        }
    }
}
