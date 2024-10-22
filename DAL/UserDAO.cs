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
    public class UserDAO
    {
        public DataTable GetUserByNameAndPwd(string name, string pwd)
        {
            DbHelper helper = new DbHelper();
            string sql = "SELECT Unique_code,role_id FROM t_user WHERE user_name=@user_name AND password=@password";
            DbParameter para1 = helper.MakeInParam("user_name", name);
            DbParameter para2 = helper.MakeInParam("password", pwd);
            DbParameter[] param = new DbParameter[] { para1, para2 };
            DataTable dt = helper.Fill(sql, param);
            return dt;
        }

        public DataTable LoadRoels()
        {
            //string fieldName = "role_name,Unique_code";
            string fieldNameWithComment = "role_name AS '角色名',Unique_code";
            string sql = "SELECT " + fieldNameWithComment + " FROM t_config_role";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        public DataTable LoadRoles2()
        {
            string sql = "SELECT Unique_code,parent_id,role_name FROM t_config_role";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        public void DeleteRole(object uniqueCode)
        {
            string sql = "DELETE FROM t_config_role WHERE Unique_code=@Unique_code";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("Unique_code", uniqueCode);
            DbParameter[] param = new DbParameter[] { para1 };
            helper.ExecuteNonQuery(sql, param);
        }

        public object AddRole(object colVal)
        {
            string sql = "INSERT INTO t_config_role(role_name) VALUES(@role_name) \r\n";
            sql += "SELECT TOP 1 Unique_code FROM t_config_role ORDER BY Unique_code DESC";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("role_name", colVal);
            DbParameter[] param = new DbParameter[] { para1 };
            object lastUniqueCode = helper.ExecuteScalar(sql, param);
            return lastUniqueCode;
        }

        public void UpdateRole(object colVal, object uniqueCode)
        {
            string sql = "UPDATE t_config_role SET role_name=@role_name WHERE Unique_code=@Unique_code";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("role_name", colVal);
            DbParameter para2 = helper.MakeInParam("Unique_code", uniqueCode);
            DbParameter[] param = new DbParameter[] { para1, para2 };
            helper.ExecuteNonQuery(sql, param);
        }

        public DataTable GetAccessesByRoleId(string uniqueCode)
        {
            string sql = "SELECT * FROM t_config_access WHERE role_id='" + uniqueCode + "'";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        public DataTable GetAccessesByRoleId2(string uniqueCode)
        {
            string sql = "SELECT t1.Unique_code,t1.id,t1.parent_id,t1.caption AS '功能列表',t1.name AS '功能触发ID',t1.page_name_directedto AS '页面名称',t2.role_name AS '角色',t1.deal_visible AS '可查看',t1.deal_search AS '可搜索',t1.deal_add AS '可增加',t1.deal_update AS '可修改',t1.deal_delete AS '可删除' \r\n";
            sql += " FROM t_config_access t1 INNER JOIN t_config_role t2 ON t1.role_id=t2.Unique_code WHERE t1.role_id='" + uniqueCode + "'";
            DataTable dtFromDB = new DbHelper().Fill(sql);
            return dtFromDB;
        }

        public void UpdateAccesses(object valueObj1, object valueObj2, object valueObj3, object valueObj4, object valueObj5, object valueObj6)
        {
            string sql = "UPDATE t_config_access SET deal_visible=@deal_visible,deal_search=@deal_search,deal_add=@deal_add,deal_update=@deal_update,deal_delete=@deal_delete \r\n";
            sql += "WHERE Unique_code=@Unique_code";

            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("deal_visible", valueObj1);
            DbParameter para2 = helper.MakeInParam("deal_search", valueObj2);
            DbParameter para3 = helper.MakeInParam("deal_add", valueObj3);
            DbParameter para4 = helper.MakeInParam("deal_update", valueObj4);
            DbParameter para5 = helper.MakeInParam("deal_delete", valueObj5);
            DbParameter para6 = helper.MakeInParam("Unique_code", valueObj6);
            DbParameter[] param = new DbParameter[] { para1, para2, para3, para4, para5, para6 };
            helper.ExecuteNonQuery(sql, param);
        }

        public DataTable LoadUsers()
        {
            //string fieldName = "user_name,nick_name,work_place,tel,role_id,password,Unique_code";
            string fieldNameWithComment = "user_name AS '用户名',nick_name AS '昵称',work_place AS '工作单位',tel AS '电话',role_id,password AS '密码',Unique_code";
            string sql = "SELECT " + fieldNameWithComment + " FROM t_user";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        public void DeleteUser(object uniqueCode)
        {
            string sql = "DELETE FROM t_user WHERE Unique_code=@Unique_code";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("Unique_code", uniqueCode);
            DbParameter[] param = new DbParameter[] { para1 };
            helper.ExecuteNonQuery(sql, param);
        }

        public object AddUser(object userName, object nickName, object pwd, object workPlace, object tel, object roleObj)
        {
            string sql = "INSERT INTO t_user(user_name,nick_name,password,work_place,tel,role_id) VALUES(@user_name,@nick_name,@password,@work_place,@tel,@role_id) \r\n";
            sql += "SELECT TOP 1 Unique_code FROM t_user ORDER BY Unique_code DESC";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("user_name", userName);
            DbParameter para2 = helper.MakeInParam("nick_name", nickName.ToString());
            DbParameter para3 = helper.MakeInParam("password", pwd.ToString());
            DbParameter para4 = helper.MakeInParam("work_place", workPlace.ToString());
            DbParameter para5 = helper.MakeInParam("tel", tel.ToString());
            DbParameter para6 = helper.MakeInParam("role_id", ((AttachedCodeClass)roleObj).UniqueCode);
            DbParameter[] param = new DbParameter[] { para1, para2, para3, para4, para5, para6 };
            object lastUniqueCode = helper.ExecuteScalar(sql, param);
            return lastUniqueCode;
        }

        public void UpdateUser(object userName, object nickName, object pwd, object workPlace, object tel, object roleObj, object uniqueCode)
        {
            string sql = "UPDATE t_user SET user_name=@user_name,nick_name=@nick_name,password=@password,work_place=@work_place,tel=@tel,role_id=@role_id WHERE Unique_code=@Unique_code";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("user_name", userName);
            DbParameter para2 = helper.MakeInParam("nick_name", nickName.ToString());
            DbParameter para3 = helper.MakeInParam("password", pwd.ToString());
            DbParameter para4 = helper.MakeInParam("work_place", workPlace.ToString());
            DbParameter para5 = helper.MakeInParam("tel", tel.ToString());
            DbParameter para6 = helper.MakeInParam("role_id", ((AttachedCodeClass)roleObj).UniqueCode);
            DbParameter para7 = helper.MakeInParam("Unique_code", uniqueCode);
            DbParameter[] param = new DbParameter[] { para1, para2, para3, para4, para5, para6, para7 };
            helper.ExecuteNonQuery(sql, param);
        }
    }
}
