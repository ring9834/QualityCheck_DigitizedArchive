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
    public partial class FieldShowList_Config : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        private string SearchCondtionForManualCheck { get; set; }//用于手动质检的搜索条件
        public UserEntity UserLoggedIn { get; set; }
        public FieldShowList_Config()
        {
            InitializeComponent();
        }

        public FieldShowList_Config(string fileCodeName, UserEntity user)
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
            checkedListBoxControl_config_fieldList_forSelect.Items.Clear();
            checkedListBoxControl_fieldList_selected.Items.Clear();
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBoxControl_config_fieldList_forSelect.Items.Add(item);
            }
            dtFromDB.Dispose();

            FieldShowListDAO fsd = new FieldShowListDAO();
            dtFromDB = fsd.GetFieldsShownInList(this.FileCodeName);
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBoxControl_fieldList_selected.Items.Add(item);
                CheckedListBoxItem item_delete = checkedListBoxControl_config_fieldList_forSelect.Items.SingleOrDefault(t => t.Tag.ToString().Equals(item.Tag.ToString()));
                checkedListBoxControl_config_fieldList_forSelect.Items.Remove(item_delete);
            }
            dtFromDB.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sql = string.Empty;
            string codeStr = string.Empty;
            FieldShowListDAO fsd = new FieldShowListDAO();
            for (int i = 0; i < checkedListBoxControl_fieldList_selected.Items.Count; i++)
            {
                string selectedcode = checkedListBoxControl_fieldList_selected.Items[i].Tag.ToString();
                fsd.SaveFieldsShowList(selectedcode, i);//保存显示列表，包括显示顺序
                if (i == 0)
                    codeStr += selectedcode;
                else
                    codeStr += "," + selectedcode;
            }

            fsd.DeleteFieldsFromList(this.FileCodeName, codeStr);//删除原来在列表中，但后来不再列表中的列
            MessageBox.Show("保存完毕！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl_config_fieldList_forSelect.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_config_fieldList_forSelect.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    checkedListBoxControl_fieldList_selected.Items.Add(item);
                    item.CheckState = CheckState.Unchecked;
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBoxControl_config_fieldList_forSelect.Items.Remove(list[i]);
            }
            list = null;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl_fieldList_selected.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_fieldList_selected.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    checkedListBoxControl_config_fieldList_forSelect.Items.Add(item);
                    item.CheckState = CheckState.Unchecked;
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBoxControl_fieldList_selected.Items.Remove(list[i]);
            }
            list = null;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataDictionaryDAO ddd = new DataDictionaryDAO();
            DataTable dtFromDB = ddd.GetColNameShowNameByCodeName(this.FileCodeName);
            checkedListBoxControl_config_fieldList_forSelect.Items.Clear();
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBoxControl_config_fieldList_forSelect.Items.Add(item);
            }
            dtFromDB.Dispose();
            checkedListBoxControl_fieldList_selected.Items.Clear();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            int index = checkedListBoxControl_fieldList_selected.SelectedIndex;
            if (index - 1 >= 0)
            {
                CheckedListBoxItem item1 = checkedListBoxControl_fieldList_selected.Items[index];
                checkedListBoxControl_fieldList_selected.Items.Insert(index - 1, item1);
                checkedListBoxControl_fieldList_selected.Items.RemoveAt(index + 1);
                checkedListBoxControl_fieldList_selected.SelectedIndex = index - 1;
                checkedListBoxControl_fieldList_selected.Items[index - 1].CheckState = CheckState.Checked;
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            int index = checkedListBoxControl_fieldList_selected.SelectedIndex;
            if (index + 2 <= checkedListBoxControl_fieldList_selected.Items.Count)
            {
                CheckedListBoxItem item1 = checkedListBoxControl_fieldList_selected.Items[index];
                checkedListBoxControl_fieldList_selected.Items.Insert(index + 2, item1);
                checkedListBoxControl_fieldList_selected.Items.RemoveAt(index);
                checkedListBoxControl_fieldList_selected.SelectedIndex = index + 1;
                checkedListBoxControl_fieldList_selected.Items[index + 1].CheckState = CheckState.Checked;
            }
        }

        private void checkedListBoxControl_fieldList_selected_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = checkedListBoxControl_fieldList_selected.SelectedIndex;
            for (int i = 0; i < checkedListBoxControl_fieldList_selected.Items.Count; i++)
            {
                if (i != index)
                {
                    checkedListBoxControl_fieldList_selected.Items[i].CheckState = CheckState.Unchecked;
                }
            }
        }
    }
}