using DotNet.DbUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class ManualCheckDAO
    {
        /// <summary>
        /// 获取已经配置好的手动质检所按分类字段，如，全宗号和目录号，或全宗号和年度
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <returns></returns>
        public DataTable GetConfiguredFieldsForManualCheck(string fileCodeName)
        {
            string sql = "SELECT t2.Unique_code,t2.col_name,t2.show_name FROM t_config_manual_check AS t1 INNER JOIN t_config_col_dict AS t2 ON t1.selected_code=t2.Unique_code WHERE t2.code='" + fileCodeName + "' ORDER BY order_number ASC";
            DataTable dtFromDB = new DbHelper().Fill(sql);
            return dtFromDB;
        }

        /// <summary>
        /// 根据所选档案库类型，对其进行手动质检分类方案配置保存
        /// </summary>
        /// <param name="selectedcode"></param>
        /// <param name="i"></param>
        public void SaveManualCheckConfiguration(string selectedcode,int i)
        {
            string sql = "DECLARE @colName nvarchar(50) \r\n";
            sql += "SELECT @colName = (SELECT col_name FROM t_config_col_dict WHERE Unique_code IN(SELECT selected_code FROM t_config_manual_check WHERE selected_code=" + selectedcode + "))\r\n";
            sql += "IF @colName IS NULL\r\n";
            sql += "INSERT t_config_manual_check(selected_code,order_number) VALUES(" + selectedcode + ",'" + i.ToString() + "')\r\n";
            sql += "ELSE \r\n";
            sql += "UPDATE t_config_manual_check SET order_number='" + i.ToString() + "' WHERE selected_code=" + selectedcode + "\r\n";
            new DbHelper().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 删除原来在配置方案中，但现在又不再其中的配置项
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="codeStr"></param>
        public void DeleteConfigurationItemsEarlyExisted(string fileCodeName, string codeStr)
        {
            string sql = "DELETE FROM t_config_manual_check WHERE selected_code NOT IN(" + codeStr + ") AND selected_code IN (SELECT Unique_code FROM t_config_col_dict WHERE code='" + fileCodeName + "')";
            new DbHelper().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 错误修正后，更新修正者信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="uniquecode"></param>
        public void UpdateCorrectedStatus(string userName, string uniquecode)
        {
            string sql = "UPDATE t_archive_manual_check_rec SET is_corrected ='1',correct_user_id='" + userName + "',corrected_time='" + DateTime.Now.ToString() + "' WHERE Unique_code=" + uniquecode;
            new DbHelper().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 获得手动检测出的问题记录（档号列表）
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <returns></returns>
        public DataTable GetManualCheckedRecords(string fileCodeName)
        {
            string sql = "SELECT dh FROM t_archive_manual_check_rec WHERE code_name='" + fileCodeName + "'";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        /// <summary>
        /// 创建临时表，如果没有临时表，就先创建临时表，后缀加上“_manual_check”
        /// </summary>
        /// <param name="creatingTableFields"></param>
        /// <param name="fileCodeName"></param>
        public void CreateTempTableForManualCheck(string creatingTableFields,string fileCodeName)
        {
            string creatingFields = creatingTableFields + ",record_count int,Unique_code numeric(18, 0) IDENTITY(1,1) NOT NULL,";
            string sql = "IF OBJECT_ID(N'" + fileCodeName + "_manual_check" + "',N'U') IS NULL \r\n";
            sql += "  BEGIN \r\n";
            sql += "    CREATE TABLE [dbo].[" + fileCodeName + "_manual_check" + "]( \r\n";
            sql += creatingFields + " \r\n";
            sql += "    CONSTRAINT [PK_t_" + fileCodeName + "_manual_check] PRIMARY KEY CLUSTERED \r\n";
            sql += "    (Unique_code ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] \r\n";
            sql += "    ) ON [PRIMARY] \r\n";
            sql += "  END \r\n";
            sql += "ELSE \r\n";
            sql += "  BEGIN \r\n";
            sql += "    DROP TABLE [dbo].[" + fileCodeName + "_manual_check" + "] \r\n";//删掉表格重建，因为字段有可能因为配置的变化而变化
            sql += "    CREATE TABLE [dbo].[" + fileCodeName + "_manual_check" + "]( \r\n";
            sql += creatingFields + " \r\n";
            sql += "    CONSTRAINT [PK_t_" + fileCodeName + "_manual_check] PRIMARY KEY CLUSTERED \r\n";
            sql += "    (Unique_code ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] \r\n";
            sql += "    ) ON [PRIMARY] \r\n";
            sql += "  END \r\n";
            new DbHelper().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// //筛选出可以供手动质检的记录项 使用 EXCEPT
        /// </summary>
        /// <param name="fieldStrWithoutAlias"></param>
        /// <param name="fileCodeName"></param>
        /// <param name="fieldGroupString"></param>
        /// <param name="condtionStr"></param>
        public void SelectDataForManualCheck(string fieldStrWithoutAlias, string fileCodeName, string fieldGroupString, string condtionStr)
        {
            string sql = "SELECT DISTINCT " + fieldStrWithoutAlias + ",(SELECT COUNT(*) FROM " + fileCodeName + " AS t2 WHERE " + condtionStr + ") AS 'record_count' \r\n";
            sql += "INTO #temp_manual_check FROM " + fileCodeName + " AS t1 \r\n";
            sql += "IF OBJECT_ID(N'" + fileCodeName + "_workflow_rec" + "',N'U') IS NOT NULL \r\n";
            sql += "  BEGIN \r\n";
            sql += "    SELECT " + fieldGroupString + " AS 'fg' INTO #temp_field_group FROM #temp_manual_check EXCEPT SELECT " + fieldGroupString + " AS 'fg' FROM " + fileCodeName + "_workflow_rec \r\n";
            sql += "    IF @@ROWCOUNT > 0 \r\n";
            sql += "    BEGIN \r\n";
            sql += "      INSERT INTO " + fileCodeName + "_manual_check(" + fieldStrWithoutAlias + ",record_count) SELECT " + fieldStrWithoutAlias + ",record_count FROM #temp_manual_check WHERE " + fieldGroupString + " IN(SELECT fg FROM #temp_field_group) \r\n";
            sql += "      DROP TABLE #temp_field_group \r\n";
            sql += "    END \r\n";
            sql += "  END \r\n";
            sql += "ELSE \r\n";
            sql += "  BEGIN \r\n";
            sql += "    INSERT INTO " + fileCodeName + "_manual_check(" + fieldStrWithoutAlias + ",record_count) SELECT " + fieldStrWithoutAlias + ",record_count FROM #temp_manual_check \r\n";
            sql += "  END \r\n";
            sql += "DROP TABLE #temp_manual_check \r\n";
            new DbHelper().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 某个流程节点已经完成质检或验收，点提交按钮，记录工作流状态
        /// </summary>
        /// <param name="creatingTableFields"></param>
        /// <param name="fileCodeName"></param>
        /// <param name="fieldStrWithoutAlias"></param>
        /// <param name="where"></param>
        /// <param name="nextRoleId"></param>
        /// <param name="complete"></param>
        /// <param name="uniquecode"></param>
        public void SummitWhenCompleteThisCheck(string creatingTableFields, string fileCodeName, string fieldStrWithoutAlias, string where, string nextRoleId, int complete, string uniquecode)
        {
            string creatingFields = creatingTableFields + ",record_count int,role_id NVARCHAR(20),is_completed bit DEFAULT(0),Unique_code numeric(18, 0) IDENTITY(1,1) NOT NULL,";
            string sql = "IF OBJECT_ID(N'" + fileCodeName + "_workflow_rec" + "',N'U') IS NULL \r\n";
            sql += "BEGIN \r\n";
            sql += "  CREATE TABLE [dbo].[" + fileCodeName + "_workflow_rec" + "]( \r\n";
            sql += creatingFields + " \r\n";
            sql += "  CONSTRAINT [PK_t_" + fileCodeName + "_workflow_rec] PRIMARY KEY CLUSTERED \r\n";
            sql += "  (Unique_code ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] \r\n";
            sql += "  ) ON [PRIMARY] \r\n";
            sql += "END \r\n";
            sql += "SELECT * FROM " + fileCodeName + "_workflow_rec WHERE " + where + " \r\n";
            sql += "IF @@Rowcount = 0 \r\n";
            sql += "    BEGIN \r\n";
            sql += "      INSERT INTO " + fileCodeName + "_workflow_rec(" + fieldStrWithoutAlias + ",record_count) SELECT " + fieldStrWithoutAlias + ",record_count FROM " + fileCodeName + "_manual_check WHERE Unique_code=" + uniquecode + "\r\n";
            sql += "      DECLARE @UniqueCodeLastInserted int \r\n";
            sql += "      SELECT @UniqueCodeLastInserted = (SELECT TOP 1 Unique_code FROM " + fileCodeName + "_workflow_rec ORDER BY Unique_code DESC) \r\n";
            sql += "      UPDATE " + fileCodeName + "_workflow_rec SET role_id='" + nextRoleId + "',is_completed=" + complete + " WHERE Unique_code=@UniqueCodeLastInserted";
            sql += "    END \r\n";
            sql += "ELSE \r\n";
            sql += "    BEGIN \r\n";
            sql += "      UPDATE " + fileCodeName + "_workflow_rec SET role_id='" + nextRoleId + "',is_completed=" + complete + " WHERE Unique_code=" + uniquecode + "\r\n";
            sql += "    END \r\n";
            new DbHelper().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 收集问题记录
        /// </summary>
        /// <param name="dh"></param>
        /// <param name="errorType"></param>
        /// <param name="description"></param>
        /// <param name="fileCodeName"></param>
        /// <param name="recordTime"></param>
        /// <param name="UserId"></param>
        public void CollectErrorData(string dh, string errorType, string description, string fileCodeName, string recordTime,string UserId)
        {
            string sql = "INSERT t_archive_manual_check_rec(dh,error_type,description,is_corrected,code_name,record_time,user_id) VALUES(@dh,@error_type,@description,@is_corrected,@code_name,@record_time,@user_id)";
            DbHelper helpService = new DbHelper();
            DbParameter[] dbps = new DbParameter[7];
            dbps[0] = new SqlParameter("@dh", dh);
            dbps[1] = new SqlParameter("@error_type", errorType);
            dbps[2] = new SqlParameter("@description", description);
            dbps[3] = new SqlParameter("@is_corrected", '0');
            dbps[4] = new SqlParameter("@code_name", fileCodeName);
            dbps[5] = new SqlParameter("@record_time", recordTime);
            dbps[6] = new SqlParameter("@user_id", UserId);
            helpService.ExecuteNonQuery(sql, dbps);
        }
    }
}
