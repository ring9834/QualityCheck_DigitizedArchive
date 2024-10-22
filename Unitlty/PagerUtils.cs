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
    public class PagerUtils
    {
        public static DataTable GetPagedDataTable(string tableStr, string fieldStr, string whereStr, string sortStr, int pageIndex, int pageSize, ref int pageCout, ref int recordCount)
        {
            string sql = "IF OBJECT_ID(N'" + tableStr + "',N'U') IS NOT NULL \r\n";
            sql += "BEGIN \r\n";
            sql += "  SELECT * FROM (SELECT ROW_NUMBER() \r\n";
            sql += "  OVER(ORDER BY " + sortStr + ") AS rownum, " + fieldStr + " FROM " + tableStr + " WHERE " + whereStr + " ) AS DWHERE \r\n";
            sql += "  WHERE rownum BETWEEN CAST(((" + pageIndex + "-1)*" + pageSize + " + 1) as nvarchar(20)) \r\n";
            sql += "  AND cast((" + pageIndex + "*" + pageSize + ") as nvarchar(20)) \r\n";
            sql += "END \r\n";
            DataTable dt = new DbHelper().Fill(sql);

            sql = "IF OBJECT_ID(N'" + tableStr + "',N'U') IS NOT NULL \r\n";
            sql += "  SELECT COUNT(1) FROM " + tableStr + " WHERE " + whereStr;
            object rc = new DbHelper().ExecuteScalar(sql);
            if (rc != null) { recordCount = int.Parse(rc.ToString()); }
            else { recordCount = 0; }
            int mod = recordCount % pageSize;
            if (mod == 0) { pageCout = recordCount / pageSize; }
            else { pageCout = recordCount / pageSize + 1; }

            if (dt.Columns.Count > 0)
                dt.Columns.RemoveAt(0);
            return dt;
        }

        public static DataTable GetPagedDataTable(string tableStr, string fieldStr, string whereStr, string sortStr, List<string> whereFieldArray, List<string> whereFieldValueArray, int pageIndex, int pageSize, ref int pageCout, ref int recordCount)
        {
            string sql = "IF OBJECT_ID(N'" + tableStr + "',N'U') IS NOT NULL \r\n";
            sql += "BEGIN \r\n";
            sql += "  SELECT * FROM (SELECT ROW_NUMBER() \r\n";
            sql += "  OVER(ORDER BY " + sortStr + ") AS rownum, " + fieldStr + " FROM " + tableStr + " WHERE " + whereStr + " ) AS DWHERE \r\n";
            sql += "  WHERE rownum BETWEEN CAST(((" + pageIndex + "-1)*" + pageSize + " + 1) as nvarchar(20)) \r\n";
            sql += "  AND cast((" + pageIndex + "*" + pageSize + ") as nvarchar(20)) \r\n";
            sql += "END \r\n";
            DbHelper helper = new DbHelper();
            DbParameter[] param = new DbParameter[whereFieldArray.Count];
            for (int i = 0; i < whereFieldArray.Count; i++)
            {
                param[i] = helper.MakeInParam(whereFieldArray[i], whereFieldValueArray[i]);
            }
            DataTable dt = helper.Fill(sql, param);

            sql = "IF OBJECT_ID(N'" + tableStr + "',N'U') IS NOT NULL \r\n";
            sql += "  SELECT COUNT(1) FROM " + tableStr + " WHERE " + whereStr;
            object rc = new DbHelper().ExecuteScalar(sql,param);
            if (rc != null) { recordCount = int.Parse(rc.ToString()); }
            else { recordCount = 0; }
            int mod = recordCount % pageSize;
            if (mod == 0) { pageCout = recordCount / pageSize; }
            else { pageCout = recordCount / pageSize + 1; }

            if (dt.Columns.Count > 0)
                dt.Columns.RemoveAt(0);
            return dt;
        }
    }
}
