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
    public class ConfigCodeDAO
    {
        /// <summary>
        /// 通过key获得辅助代码（编码）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DataTable GetCodes(string key)
        {
            string sql = "SELECT Unique_code,code_name,code_value FROM t_config_codes WHERE parent_code IN (SELECT Unique_code FROM t_config_codes_base WHERE code_key='" + key + "')";
            DataTable dtCodes = new DbHelper().Fill(sql);
            return dtCodes;
        }

        /// <summary>
        /// 获得辅助代码基类
        /// </summary>
        /// <returns></returns>
        public DataTable GetCodeBases()
        {
            string sql = "SELECT * FROM t_config_codes_base";
            DataTable dt_attch = new DbHelper().Fill(sql);
            return dt_attch;
        }

        /// <summary>
        /// 获得辅助代码几类对应的分类列表
        /// </summary>
        /// <param name="baseId"></param>
        /// <returns></returns>
        public DataTable GetCodesByBase(string baseId)
        {
            string sql = "SELECT * FROM t_config_codes WHERE parent_code=" + baseId;
            DataTable dt2 = new DbHelper().Fill(sql);
            return dt2;
        }

        /// <summary>
        /// 删除指定辅助编码和基类编码
        /// </summary>
        /// <param name="uniquecode"></param>
        public void DeleteCode(object uniquecode)
        {
            string sql = "DELETE FROM t_config_codes WHERE parent_code=@Unique_code \r\n";
            sql += "DELETE FROM t_config_codes_base WHERE Unique_code=@Unique_code";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("Unique_code", uniquecode);
            DbParameter[] param = new DbParameter[] { para1 };
            helper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 删除指定辅助编码
        /// </summary>
        /// <param name="uniquecode"></param>
        public void DeleteCode2(object uniquecode)
        {
            string sql = "DELETE FROM t_config_codes WHERE Unique_code=@Unique_code";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("Unique_code", uniquecode);
            DbParameter[] param = new DbParameter[] { para1 };
            helper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 添加辅助编码基类
        /// </summary>
        /// <param name="codeKey"></param>
        /// <param name="baseName"></param>
        /// <param name="comments"></param>
        /// <returns></returns>
        public object AddCodeBase(string codeKey, string baseName, string comments)
        {
            string sql = "INSERT INTO t_config_codes_base(code_key,base_name,comments) VALUES(@code_key,@base_name,@comments) \r\n";
            sql += "SELECT TOP 1 Unique_code FROM t_config_codes_base ORDER BY Unique_code DESC";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("code_key", codeKey);
            DbParameter para2 = helper.MakeInParam("base_name", baseName);
            DbParameter para3 = helper.MakeInParam("comments", comments);
            DbParameter[] param = new DbParameter[] { para1, para2, para3 };
            object lastUniqueCode = helper.ExecuteScalar(sql, param);
            return lastUniqueCode;
        }

        /// <summary>
        /// 更新辅助代码基类
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="comments"></param>
        /// <param name="uniquecode"></param>
        public void UpdateCodeBase(string baseName, string comments, object uniquecode)
        {
            string sql = "UPDATE t_config_codes_base SET base_name=@base_name,comments=@comments WHERE Unique_code=@Unique_code";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("base_name", baseName);
            DbParameter para2 = helper.MakeInParam("comments", comments);
            DbParameter para3 = helper.MakeInParam("Unique_code", uniquecode);
            DbParameter[] param = new DbParameter[] { para1, para2, para3 };
            helper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 增加辅助代码
        /// </summary>
        /// <param name="parentCode"></param>
        /// <param name="codeName"></param>
        /// <param name="codeValue"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public object AddCode(object parentCode, string codeName, string codeValue, object order)
        {
            string sql = "INSERT INTO t_config_codes(parent_code,code_name,code_value,order_id) VALUES(@parent_code,@code_name,@code_value,@order_id) \r\n";
            sql += "SELECT TOP 1 Unique_code FROM t_config_codes ORDER BY Unique_code DESC";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("parent_code", parentCode.ToString());
            DbParameter para2 = helper.MakeInParam("code_name", codeName);
            DbParameter para3 = helper.MakeInParam("code_value", codeValue);
            DbParameter para4 = helper.MakeInParam("order_id", order);
            DbParameter[] param = new DbParameter[] { para1, para2, para3, para4 };
            object lastUniqueCode = helper.ExecuteScalar(sql, param);
            return lastUniqueCode;
        }

        /// <summary>
        /// 更新辅助代码
        /// </summary>
        /// <param name="codeName"></param>
        /// <param name="codeValue"></param>
        /// <param name="order"></param>
        /// <param name="uniquecode"></param>
        public void UpadteCode(string codeName, string codeValue, object order, object uniquecode)
        {
            string sql = "UPDATE t_config_codes SET code_name=@code_name,code_value=@code_value,order_id=@order_id WHERE Unique_code=@Unique_code";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("code_name", codeName);
            DbParameter para2 = helper.MakeInParam("order_id", order);
            DbParameter para3 = helper.MakeInParam("Unique_code", uniquecode);
            DbParameter para4 = helper.MakeInParam("code_value", codeValue);
            DbParameter[] param = new DbParameter[] { para1, para2, para3, para4 };
            helper.ExecuteNonQuery(sql, param);
        }
    }
}
