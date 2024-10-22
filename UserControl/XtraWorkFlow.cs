using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DotNet.DbUtilities;

namespace Prj_FileManageNCheckApp
{
    public partial class XtraWorkFlow : DevExpress.XtraEditors.XtraUserControl
    {
        public XtraWorkFlow()
        {
            InitializeComponent();
        }

        //声明事件委托  
        public delegate void OperatorEventHandler(object sender);
        //定义事件  
        public event OperatorEventHandler OnOperatorClick;

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format("确定要移除该项吗？"), "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                OnOperatorClick(this);
            }
        }
        /// <summary>
        /// 设置流程节点的顺序显示
        /// </summary>
        public void SetLabelTextOnOrdial(int ordial)
        {
            labelControl1.Text = "流程节点" + ordial;
        }

        private void XtraWorkFlow_Load(object sender, EventArgs e)
        {
            if (comboBoxEdit1.Properties.Items.Count == 0)
            {
                LoadComboBoxItems();
            }
        }

        public void LoadComboBoxItems()
        {
            string sql = "SELECT role_name,Unique_code FROM t_config_role";
            DataTable dt = new DbHelper().Fill(sql);
            comboBoxEdit1.Properties.Items.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ComboboxItem item = new ComboboxItem(dt.Rows[i]["role_name"].ToString(), dt.Rows[i]["Unique_code"].ToString());
                comboBoxEdit1.Properties.Items.Add(item);
            }
            comboBoxEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        }

        public int GetSelectedRoleIndex()
        {
            return comboBoxEdit1.SelectedIndex;
        }

        public ComboboxItem GetSelectedItem()
        {
            return (ComboboxItem)comboBoxEdit1.SelectedItem;
        }

        public void SetSelectedItem(string roleId)
        {
            foreach (ComboboxItem item in comboBoxEdit1.Properties.Items)
            {
                if (item.Value.Equals(roleId))
                {
                    comboBoxEdit1.SelectedItem = item;
                    return;
                }
            }
        }
    }
}
