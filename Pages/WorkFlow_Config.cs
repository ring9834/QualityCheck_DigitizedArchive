using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DotNet.DbUtilities;
using DevExpress.XtraEditors.Controls;
using System.Xml;
using System.Data.Common;

namespace Prj_FileManageNCheckApp
{
    public partial class WorkFlow_Config : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }
        public WorkFlow_Config()
        {
            InitializeComponent();
        }

        public WorkFlow_Config(string fileCodeName)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
        }

        public void InitiateDatas(ParametersForChildForms parameters)
        {
            if (parameters != null)
                this.FileCodeName = parameters.FileCodeName;
            PageControlLocation.MakeControlCenter(panelControl1.Parent, panelControl1);
            //LoadRolesForWorkFlow();
        }

        void LoadRolesForWorkFlow()
        {
            string sql = "SELECT role_name,Unique_code FROM t_config_role ORDER BY Unique_code ASC";
            DataTable dtFromDB = new DbHelper().Fill(sql);
            checkedListBoxControl_config_roleForWorkFlow.Items.Clear();
            checkedListBoxControl_roleForWorkFlow_selected.Items.Clear();
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["Unique_code"], dtFromDB.Rows[i]["role_name"].ToString());
                checkedListBoxControl_config_roleForWorkFlow.Items.Add(item);
            }
            dtFromDB.Dispose();

            sql = "SELECT super_access_role FROM t_config_dataflow";
            dtFromDB = new DbHelper().Fill(sql);
            if (dtFromDB.Rows.Count > 0)
            {
                string xml = dtFromDB.Rows[0]["super_access_role"].ToString();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNodeList nodeList = doc.SelectNodes(@"RoleForWorkFlowConfig/ConfigItem");
                if (nodeList.Count > 0)
                {
                    string roleString = string.Empty;
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        if (i == 0)
                        {
                            roleString = "'" + nodeList[i].Attributes["RoleId"].Value + "'";
                        }
                        else
                        {
                            roleString += "," + "'" + nodeList[i].Attributes["RoleId"].Value + "'";
                        }
                    }
                    sql = "SELECT role_name,Unique_code FROM t_config_role WHERE Unique_code IN(" + roleString + ")  ORDER BY Unique_code ASC";
                    dtFromDB = new DbHelper().Fill(sql);
                    for (int i = 0; i < dtFromDB.Rows.Count; i++)
                    {
                        CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["Unique_code"], dtFromDB.Rows[i]["role_name"].ToString());
                        checkedListBoxControl_roleForWorkFlow_selected.Items.Add(item);
                        CheckedListBoxItem item_delete = checkedListBoxControl_config_roleForWorkFlow.Items.SingleOrDefault(t => t.Value.ToString().Equals(item.Value.ToString()));
                        checkedListBoxControl_config_roleForWorkFlow.Items.Remove(item_delete);
                    }
                    dtFromDB.Dispose();
                }
                else
                {
                    MessageBox.Show("可控制整个流程的角色还未配置！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            XtraWorkFlow wf = new XtraWorkFlow();
            wf.OnOperatorClick += wf_OnOperatorClick;
            flowLayoutPanel1.Controls.Add(wf);
            SetLayoutOrdial();
        }

        void wf_OnOperatorClick(object sender)
        {
            XtraWorkFlow wf = sender as XtraWorkFlow;
            //int oldIndex = flowLayoutPanel1.Controls.IndexOf(wf);//获取操作项的索引值  
            flowLayoutPanel1.Controls.Remove(wf);
            SetLayoutOrdial();
        }

        void SetLayoutOrdial()
        {
            for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
            {
                XtraWorkFlow wf = (XtraWorkFlow)flowLayoutPanel1.Controls[i];
                wf.SetLabelTextOnOrdial(i + 1);
            }
        }

        private void WorkFlow_Config_Load(object sender, EventArgs e)
        {
            LoadWorkFlows();
            LoadRolesForWorkFlow();
        }

        void LoadWorkFlows()
        {
            string sql = "SELECT * FROM t_config_dataflow ORDER BY order_id ASC";
            DataTable dt = new DbHelper().Fill(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                XtraWorkFlow wf = new XtraWorkFlow();
                wf.OnOperatorClick += wf_OnOperatorClick;
                wf.LoadComboBoxItems();
                wf.SetSelectedItem(dt.Rows[i]["role_id"].ToString());
                flowLayoutPanel1.Controls.Add(wf);
            }
            SetLayoutOrdial();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel1.Controls.Count == 0)
            {
                MessageBox.Show("流程中还未添加流程节点！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
            {
                XtraWorkFlow wf = (XtraWorkFlow)flowLayoutPanel1.Controls[i];
                int index = wf.GetSelectedRoleIndex();
                if (index == -1)
                {
                    MessageBox.Show("还有流程节点未选择角色！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            DataTable dtToDb = new DataTable();
            dtToDb.TableName = "t_config_dataflow";
            dtToDb.Columns.Add("role_id", typeof(string));
            dtToDb.Columns.Add("order_id", typeof(int));
            for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
            {
                XtraWorkFlow wf = (XtraWorkFlow)flowLayoutPanel1.Controls[i];
                ComboboxItem item = wf.GetSelectedItem();
                DataRow dr = dtToDb.NewRow();
                dr["role_id"] = item.Value;
                dr["order_id"] = i;
                dtToDb.Rows.Add(dr);
            }

            string sql = "DELETE FROM t_config_dataflow";
            new DbHelper().ExecuteNonQuery(sql);
            new SqlHelper().SqlBulkCopyData(dtToDb);
            dtToDb.Dispose();
            MessageBox.Show("保存成功！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl_config_roleForWorkFlow.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_config_roleForWorkFlow.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    checkedListBoxControl_roleForWorkFlow_selected.Items.Add(item);
                    item.CheckState = CheckState.Unchecked;
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBoxControl_config_roleForWorkFlow.Items.Remove(list[i]);
            }
            list = null;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl_roleForWorkFlow_selected.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_roleForWorkFlow_selected.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    checkedListBoxControl_config_roleForWorkFlow.Items.Add(item);
                    item.CheckState = CheckState.Unchecked;
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBoxControl_roleForWorkFlow_selected.Items.Remove(list[i]);
            }
            list = null;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string sql = "SELECT role_name,Unique_code FROM t_config_role ORDER BY Unique_code ASC";
            DataTable dtFromDB = new DbHelper().Fill(sql);
            checkedListBoxControl_config_roleForWorkFlow.Items.Clear();
            checkedListBoxControl_roleForWorkFlow_selected.Items.Clear();
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["Unique_code"], dtFromDB.Rows[i]["role_name"].ToString());
                checkedListBoxControl_config_roleForWorkFlow.Items.Add(item);
            }
            dtFromDB.Dispose();
            checkedListBoxControl_roleForWorkFlow_selected.Items.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("RoleForWorkFlowConfig");
            doc.AppendChild(root);
            for (int i = 0; i < checkedListBoxControl_roleForWorkFlow_selected.Items.Count; i++)
            {
                XmlElement element = doc.CreateElement("ConfigItem");
                element.SetAttribute("RoleId", checkedListBoxControl_roleForWorkFlow_selected.Items[i].Value.ToString());
                root.AppendChild(element);
            }
            string sql = "UPDATE t_config_dataflow SET super_access_role=@super_access_role";
            DbHelper helper = new DbHelper();
            DbParameter para1 = helper.MakeInParam("super_access_role", doc.OuterXml);
            DbParameter[] param = new DbParameter[] { para1 };
            helper.ExecuteNonQuery(sql, param);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}