using DotNet.DbUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class DataFlowDAO
    {
        /// <summary>
        /// 档案数据流程，如，从加工方-监理方-验收费
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataFlow()
        {
            string sql = "SELECT * FROM t_config_dataflow";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        /// <summary>
        /// 根据角色获得数据流信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public DataTable GetDataFlowDataByRole(string roleId)
        {
            string sql0 = "SELECT * FROM t_config_dataflow WHERE role_id='" + roleId + "'";
            DataTable dt0 = new DbHelper().Fill(sql0);
            return dt0;

        }

        /// <summary>
        /// 获得流程中的角色，从流程的前到后
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataFlowRoles()
        {
            string sql0 = "SELECT role_id FROM t_config_dataflow ORDER BY Unique_code ASC";
            DataTable dt0 = new DbHelper().Fill(sql0);
            return dt0;
        }
    }
}
