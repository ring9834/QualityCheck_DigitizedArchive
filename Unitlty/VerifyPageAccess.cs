using DotNet.DbUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prj_FileManageNCheckApp
{
    public class VerifyPageAccess
    {
        public VerifyPageAccess()
        {

        }

        public static void VerifyButtonsAccessOnPage(Form form, string roleId)
        {
            string sql = "SELECT deal_search,deal_add,deal_update,deal_delete \r\n";
            sql += " FROM t_config_access WHERE role_id='" + roleId + "' AND page_name_directedto='" + form.Name + "'";
            DataTable dtFromDB = new DbHelper().Fill(sql);
            if (dtFromDB.Rows.Count > 0)
            {
                DataRow dr = dtFromDB.Rows[0];
                bool deal_search = Boolean.Parse(dr["deal_search"].ToString());
                bool deal_add = Boolean.Parse(dr["deal_add"].ToString());
                bool deal_update = Boolean.Parse(dr["deal_update"].ToString());
                bool deal_delete = Boolean.Parse(dr["deal_delete"].ToString());

                for (int i = 0; i < form.Controls.Count; i++)
                {
                    DealWithButtonsInControl(form.Controls[i], deal_search, deal_add, deal_update, deal_delete);
                }
            }
            dtFromDB.Dispose();
        }

        static void DealWithButtonsInControl(Control ctl, bool deal_search, bool deal_add, bool deal_update, bool deal_delete)
        {
            for (int i = 0; i < ctl.Controls.Count; i++)
            {
                Control control = ctl.Controls[i];
                if (control.Tag != null)
                {
                    if (control.Tag.ToString().Equals("deal_search"))
                    {
                        control.Visible = deal_search;
                    }
                    if (control.Tag.ToString().Equals("deal_add"))
                    {
                        control.Visible = deal_add;
                    }
                    if (control.Tag.ToString().Equals("deal_update"))
                    {
                        control.Visible = deal_update;
                    }
                    if (control.Tag.ToString().Equals("deal_delete"))
                    {
                        control.Visible = deal_delete;
                    }
                }
                if (control.HasChildren)
                {
                    DealWithButtonsInControl(control, deal_search, deal_add, deal_update, deal_delete);
                }
            }
        }
    }
}
