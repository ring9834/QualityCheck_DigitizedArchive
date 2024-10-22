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

namespace Prj_FileManageNCheckApp
{
    public partial class ManualCheck_Config : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        private string SearchCondtionForManualCheck { get; set; }//用于手动质检的搜索条件
        public UserEntity UserLoggedIn { get; set; }
        public ManualCheck_Config()
        {
            InitializeComponent();
        }

        public ManualCheck_Config(string fileCodeName,UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        private void XtraForm1_Load(object sender, EventArgs e)
        {
            
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            PageControlLocation.MakeControlCenter(groupControl1.Parent, groupControl1);

            DataDictionaryDAO ddd = new DataDictionaryDAO();
            DataTable dtFromDB = ddd.GetColNameShowNameByCodeName(this.FileCodeName);
            checkedListBoxControl_config_manualCheck_forSelect.Items.Clear();
            checkedListBoxControl_manualCheck_selected.Items.Clear();
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBoxControl_config_manualCheck_forSelect.Items.Add(item);
            }
            dtFromDB.Dispose();

            ManualCheckDAO mcd = new ManualCheckDAO();
            dtFromDB = mcd.GetConfiguredFieldsForManualCheck(this.FileCodeName);//获取已经配置好的手动质检所按分类字段，如，全宗号和目录号，或全宗号和年度

            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBoxControl_manualCheck_selected.Items.Add(item);
                CheckedListBoxItem item_delete = checkedListBoxControl_config_manualCheck_forSelect.Items.SingleOrDefault(t => t.Tag.ToString().Equals(item.Tag.ToString()));
                checkedListBoxControl_config_manualCheck_forSelect.Items.Remove(item_delete);
            }
            dtFromDB.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string codeStr = string.Empty;
            ManualCheckDAO mcd = new ManualCheckDAO();
            for (int i = 0; i < checkedListBoxControl_manualCheck_selected.Items.Count; i++)
            {
                string selectedcode = checkedListBoxControl_manualCheck_selected.Items[i].Tag.ToString();
                mcd.SaveManualCheckConfiguration(selectedcode, i);//根据所选档案库类型，对其进行手动质检分类方案配置保存
                if (i == 0)
                    codeStr += selectedcode;
                else
                    codeStr += "," + selectedcode;
            }
            mcd.DeleteConfigurationItemsEarlyExisted(this.FileCodeName, codeStr);//删除原来在配置方案中，但现在又不再其中的配置项
            MessageBox.Show("保存完毕！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl_config_manualCheck_forSelect.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_config_manualCheck_forSelect.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    checkedListBoxControl_manualCheck_selected.Items.Add(item);
                    item.CheckState = CheckState.Unchecked;
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBoxControl_config_manualCheck_forSelect.Items.Remove(list[i]);
            }
            list = null;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl_manualCheck_selected.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_manualCheck_selected.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    checkedListBoxControl_config_manualCheck_forSelect.Items.Add(item);
                    item.CheckState = CheckState.Unchecked;
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBoxControl_manualCheck_selected.Items.Remove(list[i]);
            }
            list = null;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataDictionaryDAO ddd = new DataDictionaryDAO();
            DataTable dtFromDB = ddd.GetColNameShowNameByCodeName(this.FileCodeName);

            checkedListBoxControl_config_manualCheck_forSelect.Items.Clear();
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBoxControl_config_manualCheck_forSelect.Items.Add(item);
            }
            dtFromDB.Dispose();
            checkedListBoxControl_manualCheck_selected.Items.Clear();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            int index = checkedListBoxControl_manualCheck_selected.SelectedIndex;
            if (index - 1 >= 0)
            {
                CheckedListBoxItem item1 = checkedListBoxControl_manualCheck_selected.Items[index];
                checkedListBoxControl_manualCheck_selected.Items.Insert(index - 1, item1);
                checkedListBoxControl_manualCheck_selected.Items.RemoveAt(index + 1);
                checkedListBoxControl_manualCheck_selected.SelectedIndex = index - 1;
                checkedListBoxControl_manualCheck_selected.Items[index - 1].CheckState = CheckState.Checked;
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            int index = checkedListBoxControl_manualCheck_selected.SelectedIndex;
            if (index + 2 <= checkedListBoxControl_manualCheck_selected.Items.Count)
            {
                CheckedListBoxItem item1 = checkedListBoxControl_manualCheck_selected.Items[index];
                checkedListBoxControl_manualCheck_selected.Items.Insert(index + 2, item1);
                checkedListBoxControl_manualCheck_selected.Items.RemoveAt(index);
                checkedListBoxControl_manualCheck_selected.SelectedIndex = index + 1;
                checkedListBoxControl_manualCheck_selected.Items[index + 1].CheckState = CheckState.Checked;
            }
        }

        private void checkedListBoxControl_manualCheck_selected_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = checkedListBoxControl_manualCheck_selected.SelectedIndex;
            for (int i = 0; i < checkedListBoxControl_manualCheck_selected.Items.Count; i++)
            {
                if (i != index)
                {
                    checkedListBoxControl_manualCheck_selected.Items[i].CheckState = CheckState.Unchecked;
                }
            }
        }
    }
}