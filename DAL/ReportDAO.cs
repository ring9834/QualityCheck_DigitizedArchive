using DotNet.DbUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class ReportDAO
    {
        /// <summary>
        /// 获得统计类型列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetStatisticTypes()
        {
            string sql = "SELECT Unique_code,code_name,code_value FROM t_config_codes WHERE parent_code IN(SELECT Unique_code FROM t_config_codes_base WHERE code_key='TJLX')";
            DataTable dt_tjlx = new DbHelper().Fill(sql);
            return dt_tjlx;
        }

        /// <summary>
        /// 通过报表类型，获得报表条件
        /// </summary>
        /// <param name="reportType"></param>
        /// <returns></returns>
        public object GetReportCondition(string reportType)
        {
            string sql = "SELECT report_condtion FROM t_report_condtion WHERE report_Type='" + reportType + "'";
            object reportCondition = new DbHelper().ExecuteScalar(sql);
            return reportCondition;
        }

        /// <summary>
        /// 保存为报表模板
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="xml"></param>
        public void SaveReportTemplate(string reportType,string xml)
        {
            string sql = "SELECT * FROM t_report_condtion WHERE report_Type='" + reportType + "' \r\n";//保存为统计模板
            sql += "IF @@Rowcount = 0 \r\n";
            sql += "    BEGIN \r\n";
            sql += "      INSERT INTO t_report_condtion(report_Type,report_condtion) VALUES('" + reportType + "','" + xml + "')\r\n";
            sql += "    END \r\n";
            sql += "ELSE \r\n";
            sql += "    BEGIN \r\n";
            sql += "      UPDATE t_report_condtion SET report_condtion='" + xml + "' WHERE report_Type='" + reportType + "' \r\n";
            sql += "    END \r\n";
            new DbHelper().ExecuteNonQuery(sql);
        }

    }
}
