
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
    public class DataDictionaryDAO
    {
        public DataTable GetColDicBase()
        {
            string sql = "SELECT * FROM t_config_col_dict_base";
            DataTable dtColbase = new DbHelper().Fill(sql);
            return dtColbase;
        }

        public void CreateBusinessDictTable(DataTable dtColbase, DataTable dtCodes, string fileCodeName)
        {
            DataTable dtColDict = dtColbase.Copy();//克隆
            dtColDict.TableName = "t_config_col_dict";
            dtColDict.Columns.Add("code", typeof(string));

            string colCreating = "CREATE TABLE [" + fileCodeName + "]( \r\n";
            colCreating += "[Unique_code] [numeric](18, 0) IDENTITY(1,1) NOT NULL,\r\n";
            for (int i = 0; i < dtColbase.Rows.Count; i++)
            {
                DataRow[] drs = dtCodes.Select("Unique_code=" + dtColbase.Rows[i]["col_datatype"].ToString());
                if (drs.Length > 0)
                {
                    string colNull = dtColbase.Rows[i]["col_null"].ToString().ToLower().Equals("true") ? "NULL" : "NOT NULL";
                    string numbericCol_zero = drs[0]["code_value"].ToString().ToLower().Equals("numeric") ? ",0" : "";
                    string colInfo = "[" + dtColbase.Rows[i]["col_name"].ToString() + "] [" + drs[0]["code_value"].ToString() + "](" + dtColbase.Rows[i]["col_maxlen"].ToString() + numbericCol_zero + ") " + colNull + ",\r\n";
                    colCreating += colInfo;
                }
                dtColDict.Rows[i]["code"] = fileCodeName;
            }


            colCreating += "import_bundle NVARCHAR(50) NULL,yw NVARCHAR(MAX) NULL,yw_xml XML NULL,";//导入目录批次号、原文字段
            colCreating += " CONSTRAINT [PK_t_" + fileCodeName + "] PRIMARY KEY CLUSTERED \r\n";
            colCreating += "([Unique_code] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] \r\n";
            colCreating += ") ON [PRIMARY]";

            new DbHelper().ExecuteNonQuery(colCreating);//创建档案业务类型表
            new SqlHelper().SqlBulkCopyData(dtColDict);//将t_config_col_dict_base中的数据，拷贝到t_config_col_dict表
        }

        /// <summary>
        /// 取得指定档案库类型对应的字段信息：字段名和显示名
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <returns></returns>
        public DataTable GetColNameShowNameByCodeName(string fileCodeName)
        {
            string sql = "SELECT Unique_code,col_name,show_name FROM t_config_col_dict WHERE code='" + fileCodeName + "'";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        /// <summary>
        /// 取得指定档案库类型对应的字段信息,显示列名为汉字
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <returns></returns>
        public DataTable GetColInfoByCodeName(string fileCodeName)
        {
            //string fieldName = "col_name,show_name,col_datatype,col_maxlen,col_null,col_default,col_use,query_flag,field_type,comments,Unique_code";
            string fieldNameWithComment = "col_name AS '列名',show_name AS '显示名',col_datatype AS '数据类型',col_maxlen '最大长度',col_null AS '可为空?',col_default '默认值',col_use AS '可用?',query_flag AS '可查?',field_type AS '字段类型',comments AS '备注',Unique_code";
            string sql = "SELECT " + fieldNameWithComment + " FROM t_config_col_dict WHERE code='" + fileCodeName + "'";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        /// <summary>
        /// 取得指定档案库类型对应的字段信息
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <returns></returns>
        public DataTable GetColInfoByCodeName2(string fileCodeName)
        {
            string sql = "SELECT col_name,show_name,col_show_type,col_show_value,col_null,col_order,col_use,comments,Unique_code FROM t_config_col_dict WHERE code=@codeName ORDER BY CAST(col_order AS INT) ASC";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("codeName", fileCodeName);
            DbParameter[] param = new DbParameter[] { para1 };
            DataTable dt = helper.Fill(sql, param);
            return dt;
        }

        /// <summary>
        /// 取得指定档案库类型对应的字段信息
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <returns></returns>
        public DataTable GetColInfoByCodeName3(string fileCodeName)
        {
            string sql = "SELECT col_name,show_name,col_null,col_maxlen,col_show_value FROM t_config_col_dict WHERE code='" + fileCodeName + "'";
            DataTable fieldDt = new DbHelper().Fill(sql);
            return fieldDt;
        }

        /// <summary>
        /// 删除指定字段并在档案库类型中删除相应字段
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="colVal"></param>
        /// <param name="uniqueCode"></param>
        public void DeleteSpecificDataDictRecord(string fileCodeName, object colVal, object uniqueCode)
        {
            string sql = "DELETE FROM t_config_col_dict WHERE Unique_code=@Unique_code \r\n";
            sql += "ALTER TABLE " + fileCodeName + " DROP COLUMN " + colVal.ToString();
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("Unique_code", uniqueCode);
            DbParameter[] param = new DbParameter[] { para1 };
            helper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 在公共数据字典中删除某字段
        /// </summary>
        /// <param name="uniqueCode"></param>
        public void DeleteSpecificDataDictRecord(object uniqueCode)
        {
            string sql = "DELETE FROM t_config_col_dict_base WHERE Unique_code=@Unique_code";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("Unique_code", uniqueCode);
            DbParameter[] param = new DbParameter[] { para1 };
            helper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 判断字段是否存在
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="colVal"></param>
        /// <returns></returns>
        public object VerifyFieldExist(string fileCodeName,object colVal)
        {
            string sql = "SELECT COUNT(b.name) FROM sysobjects a INNER JOIN syscolumns b \r\n";
            sql += "ON a.id=b.id AND a.xtype='U' \r\n";
            sql += "WHERE a.name='" + fileCodeName + "' AND b.name ='" + colVal.ToString() + "'\r\n";
            object rowCount = new DbHelper().ExecuteScalar(sql);
            return rowCount;
        }

        /// <summary>
        /// 增加字段
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="colVal"></param>
        /// <param name="dataTypeCodeValue"></param>
        /// <param name="maxLen"></param>
        /// <param name="numbericCol_zero"></param>
        /// <param name="colNullValue"></param>
        public void AddFieldToTable(string fileCodeName, object colVal, string dataTypeCodeValue, object maxLen, string numbericCol_zero, string colNullValue)
        {
            string sql = "ALTER TABLE " + fileCodeName + " ADD " + colVal.ToString() + " " + dataTypeCodeValue + "(" + maxLen.ToString() + numbericCol_zero + ") " + colNullValue;
            new DbHelper().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 更改字段
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="colVal"></param>
        /// <param name="dataTypeCodeValue"></param>
        /// <param name="maxLen"></param>
        /// <param name="numbericCol_zero"></param>
        /// <param name="colNullValue"></param>
        public void ModifyFieldInTable(string fileCodeName, object colVal, string dataTypeCodeValue, object maxLen, string numbericCol_zero, string colNullValue)
        {
            string sql = "ALTER TABLE " + fileCodeName + " ALTER COLUMN " + colVal.ToString() + " " + dataTypeCodeValue + "(" + maxLen.ToString() + numbericCol_zero + ") " + colNullValue;
            new DbHelper().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 添加字段信息
        /// </summary>
        /// <param name="fileCodeName"></param>
        /// <param name="colVal"></param>
        /// <param name="showVal"></param>
        /// <param name="dataTypeCode"></param>
        /// <param name="maxLen"></param>
        /// <param name="colNull"></param>
        /// <param name="colUse"></param>
        /// <param name="colQuery"></param>
        /// <param name="fieldType"></param>
        /// <param name="colDefault"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public object AddColInfo(string fileCodeName, object colVal, object showVal, string dataTypeCode, object maxLen, object colNull, object colUse, object colQuery, object fieldType, object colDefault, object comment)
        {
            string sql = "INSERT INTO t_config_col_dict(code,col_name,show_name,col_datatype,col_maxlen,col_null,col_default,col_use,query_flag,field_type,comments) \r\n";
            sql += "VALUES(@code,@col_name,@show_name,@col_datatype,@col_maxlen,@col_null,@col_default,@col_use,@query_flag,@field_type,@comments) \r\n";
            sql += "SELECT TOP 1 Unique_code FROM t_config_col_dict ORDER BY Unique_code DESC";
            DbHelper helper = new DbHelper();
            DbParameter para0 = helper.MakeInParam("code", fileCodeName);
            DbParameter para1 = helper.MakeInParam("col_name", colVal);
            DbParameter para2 = helper.MakeInParam("show_name", showVal);
            DbParameter para3 = helper.MakeInParam("col_datatype", dataTypeCode);
            DbParameter para4 = helper.MakeInParam("col_maxlen", maxLen);
            DbParameter para5 = helper.MakeInParam("col_null", colNull);
            DbParameter para6 = helper.MakeInParam("col_use", colUse);
            DbParameter para7 = helper.MakeInParam("query_flag", colQuery);
            DbParameter para8 = helper.MakeInParam("field_type", fieldType);
            DbParameter para9 = helper.MakeInParam("col_default", colDefault);
            DbParameter para10 = helper.MakeInParam("comments", comment);
            DbParameter[] param = new DbParameter[] { para0, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10 };
            object lastUniqueCode = helper.ExecuteScalar(sql, param);
            return lastUniqueCode;
        }

        /// <summary>
        /// 修改字段信息
        /// </summary>
        /// <param name="uniqueCode"></param>
        /// <param name="colVal"></param>
        /// <param name="showVal"></param>
        /// <param name="dataTypeCode"></param>
        /// <param name="maxLen"></param>
        /// <param name="colNull"></param>
        /// <param name="colUse"></param>
        /// <param name="colQuery"></param>
        /// <param name="fieldType"></param>
        /// <param name="colDefault"></param>
        /// <param name="comment"></param>
        public void UpdateColInfo(object uniqueCode, object colVal, object showVal, string dataTypeCode, object maxLen, object colNull, object colUse, object colQuery, object fieldType, object colDefault, object comment)
        {
            string sql = "UPDATE t_config_col_dict SET show_name=@show_name,col_datatype=@col_datatype,col_maxlen=@col_maxlen,col_null=@col_null,\r\n";
            sql += "col_default=@col_default,col_use=@col_use,query_flag=@query_flag,field_type=@field_type,comments=@comments \r\n";
            sql += "WHERE Unique_code=@Unique_code";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("col_name", colVal);
            DbParameter para2 = helper.MakeInParam("show_name", showVal);
            DbParameter para3 = helper.MakeInParam("col_datatype", dataTypeCode);
            DbParameter para4 = helper.MakeInParam("col_maxlen", maxLen);
            DbParameter para5 = helper.MakeInParam("col_null", colNull);
            DbParameter para6 = helper.MakeInParam("col_use", colUse);
            DbParameter para7 = helper.MakeInParam("query_flag", colQuery);
            DbParameter para8 = helper.MakeInParam("field_type", fieldType);
            DbParameter para9 = helper.MakeInParam("col_default", colDefault);
            DbParameter para10 = helper.MakeInParam("comments", comment);
            DbParameter para11 = helper.MakeInParam("Unique_code", uniqueCode);
            DbParameter[] param = new DbParameter[] { para1, para2, para3, para4, para5, para6, para7, para8, para9, para10, para11 };
            helper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 更新字段信息
        /// </summary>
        /// <param name="uniquecode"></param>
        /// <param name="showTypeName"></param>
        /// <param name="showTypeValue"></param>
        /// <param name="colNull"></param>
        /// <param name="colUse"></param>
        /// <param name="colorder"></param>
        /// <param name="comments"></param>
        public void UpdateColInfo(object uniquecode, object showTypeName, object showTypeValue, bool colNull, bool colUse, object colorder, object comments)
        {
            string sql = "UPDATE t_config_col_dict SET col_show_type=@col_show_type,col_show_value=@col_show_value,col_null=@col_null,col_use=@col_use,col_order=@col_order,comments=@comments \r\n";
            sql += "WHERE Unique_code=@Unique_code";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("Unique_code", uniquecode);
            DbParameter para2 = helper.MakeInParam("col_show_type", showTypeName);
            DbParameter para3 = helper.MakeInParam("col_show_value", showTypeValue);
            DbParameter para4 = helper.MakeInParam("col_null", colNull);
            DbParameter para5 = helper.MakeInParam("col_use", colUse);
            DbParameter para6 = helper.MakeInParam("col_order", colorder.ToString());
            DbParameter para7 = helper.MakeInParam("comments", comments.ToString());
            DbParameter[] param = new DbParameter[] { para1, para2, para3, para4, para5, para6, para7 };
            helper.ExecuteNonQuery(sql, param);
        }

        /// <summary>
        /// 获得公共数据字典中的字段信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetPublicDictionaryCols()
        {
            //string fieldName = "col_name,show_name,col_datatype,col_maxlen,col_null,col_default,col_use,query_flag,field_type,comments,Unique_code";
            string fieldNameWithComment = "col_name AS '列名',show_name AS '显示名',col_datatype AS '数据类型',col_maxlen '最大长度',col_null AS '可为空?',col_default '默认值',col_use AS '可用?',query_flag AS '可查?',field_type AS '字段类型',comments AS '备注',Unique_code";
            string sql = "SELECT " + fieldNameWithComment + " FROM t_config_col_dict_base";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;

        }

        /// <summary>
        /// 保存公共数据字典记录
        /// </summary>
        /// <param name="colVal"></param>
        /// <param name="showVal"></param>
        /// <param name="dataTypeCode"></param>
        /// <param name="maxLen"></param>
        /// <param name="colNull"></param>
        /// <param name="colUse"></param>
        /// <param name="colQuery"></param>
        /// <param name="fieldType"></param>
        /// <param name="colDefault"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public object SaveCodeBase(object colVal, object showVal, object dataTypeCode, object maxLen, object colNull, object colUse, object colQuery, object fieldType, object colDefault, object comment)
        {
            string sql = "INSERT INTO t_config_col_dict_base(col_name,show_name,col_datatype,col_maxlen,col_null,col_default,col_use,query_flag,field_type,comments) \r\n";
            sql += "VALUES(@col_name,@show_name,@col_datatype,@col_maxlen,@col_null,@col_default,@col_use,@query_flag,@field_type,@comments) \r\n";
            sql += "SELECT TOP 1 Unique_code FROM t_config_col_dict_base ORDER BY Unique_code DESC";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("col_name", colVal);
            DbParameter para2 = helper.MakeInParam("show_name", showVal);
            DbParameter para3 = helper.MakeInParam("col_datatype", dataTypeCode);
            DbParameter para4 = helper.MakeInParam("col_maxlen", maxLen);
            DbParameter para5 = helper.MakeInParam("col_null", colNull);
            DbParameter para6 = helper.MakeInParam("col_use", colUse);
            DbParameter para7 = helper.MakeInParam("query_flag", colQuery);
            DbParameter para8 = helper.MakeInParam("field_type", fieldType);
            DbParameter para9 = helper.MakeInParam("col_default", colDefault);
            DbParameter para10 = helper.MakeInParam("comments", comment);
            DbParameter[] param = new DbParameter[] { para1, para2, para3, para4, para5, para6, para7, para8, para9, para10 };
            object lastUniqueCode = helper.ExecuteScalar(sql, param);
            return lastUniqueCode;
        }

        /// <summary>
        /// 更新公共数据字典记录
        /// </summary>
        /// <param name="uniqueCode"></param>
        /// <param name="colVal"></param>
        /// <param name="showVal"></param>
        /// <param name="dataTypeCode"></param>
        /// <param name="maxLen"></param>
        /// <param name="colNull"></param>
        /// <param name="colUse"></param>
        /// <param name="colQuery"></param>
        /// <param name="fieldType"></param>
        /// <param name="colDefault"></param>
        /// <param name="comment"></param>
        public void UpdateCodeBase(object uniqueCode, object colVal, object showVal, object dataTypeCode, object maxLen, object colNull, object colUse, object colQuery, object fieldType, object colDefault, object comment)
        {
            string sql = "UPDATE t_config_col_dict_base SET col_name=@col_name,show_name=@show_name,col_datatype=@col_datatype,col_maxlen=@col_maxlen,col_null=@col_null,\r\n";
            sql += "col_default=@col_default,col_use=@col_use,query_flag=@query_flag,field_type=@field_type,comments=@comments \r\n";
            sql += "WHERE Unique_code=@Unique_code";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("col_name", colVal);
            DbParameter para2 = helper.MakeInParam("show_name", showVal);
            DbParameter para3 = helper.MakeInParam("col_datatype", dataTypeCode);
            DbParameter para4 = helper.MakeInParam("col_maxlen", maxLen);
            DbParameter para5 = helper.MakeInParam("col_null", colNull);
            DbParameter para6 = helper.MakeInParam("col_use", colUse);
            DbParameter para7 = helper.MakeInParam("query_flag", colQuery);
            DbParameter para8 = helper.MakeInParam("field_type", fieldType);
            DbParameter para9 = helper.MakeInParam("col_default", colDefault);
            DbParameter para10 = helper.MakeInParam("comments", comment);
            DbParameter para11 = helper.MakeInParam("Unique_code", uniqueCode);
            DbParameter[] param = new DbParameter[] { para1, para2, para3, para4, para5, para6, para7, para8, para9, para10, para11 };
            helper.ExecuteNonQuery(sql, param);
        }
    }
}
