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
using System.Configuration;
using DotNet.DbUtilities;
using DevExpress.XtraEditors.Controls;

namespace Prj_FileManageNCheckApp
{
    public partial class AutoFileCheck_Config : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }
        public DataTable DtForAll { get; set; }
        public AutoFileCheck_Config()
        {
            InitializeComponent();
        }

        public AutoFileCheck_Config(string fileCodeName)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            if (xtraTabControl1.SelectedTabPage == xtraTabPage7)//按哪些字段进行分类检测，如，按全宗号，或全宗号和目录号等
            {
                PageControlLocation.MakeControlCenter(groupControl7.Parent, groupControl7);
                LoadInitialData();
            }
            if (xtraTabControl1.SelectedTabPage == xtraTabPage2)//字段组合for自动质检
                LoadInitialData_FieldGroup();
            if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
                LoadInitialData_PageLeft();
        }

        private void AutoFileCheck_Config_Load(object sender, EventArgs e)
        {
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (xtraTabControl1.SelectedTabPage == xtraTabPage2)
            {
                PageControlLocation.MakeControlCenter(groupControl8.Parent, groupControl8);
                LoadInitialData_FieldGroup();
            }
            if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
            {
                PageControlLocation.MakeControlCenter(groupControl1.Parent, groupControl1);
                LoadInitialData_PageLeft();
            }
            if (xtraTabControl1.SelectedTabPage == xtraTabPage5)
            {
                PageControlLocation.MakeControlCenter(groupControl6.Parent, groupControl6);
                string makeUpWhiteSideInPaper = ConfigurationManager.AppSettings["MakeUpWhiteSideInPaper"];
                string sideLengthVlue = ConfigurationManager.AppSettings["SideLengthVlue"];
                string binaryzationThreshold = ConfigurationManager.AppSettings["BinaryzationThreshold"];
                string imageLeanDegree = ConfigurationManager.AppSettings["ImageLeanDegree"];

                if (!string.IsNullOrEmpty(makeUpWhiteSideInPaper))
                {
                    bool makeUpWhiteSideInPaperBool = Boolean.Parse(makeUpWhiteSideInPaper);
                    if (makeUpWhiteSideInPaperBool)
                        checkEdit2.Checked = true;
                    else
                        checkEdit2.Checked = false;
                }
                else
                {
                    checkEdit2.Checked = false;
                }

                textEdit3.Text = sideLengthVlue;
                textEdit4.Text = binaryzationThreshold;
                textEdit6.Text = imageLeanDegree;
            }
            if (xtraTabControl1.SelectedTabPage == xtraTabPage7)
            {
                PageControlLocation.MakeControlCenter(groupControl7.Parent, groupControl7);
                LoadInitialData();
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textEdit1.Text))
            {
                MessageBox.Show("形成时间不确定则所补的字符不能为空！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (checkedListBox1.GetItemChecked(0))
                AppConfigModify.ModifyItem("UseTheYearField", "true");
            else
                AppConfigModify.ModifyItem("UseTheYearField", "false");
            if (checkedListBox1.GetItemChecked(1))
                AppConfigModify.ModifyItem("UseCreatedTimeField", "true");
            else
                AppConfigModify.ModifyItem("UseCreatedTimeField", "false");

            AppConfigModify.ModifyItem("CharMadeUpInCreatedTime", textEdit1.Text);
            MessageBox.Show("配置成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            if (checkEdit3.Checked)
            {
                if (string.IsNullOrEmpty(textEdit2.Text))
                {
                    MessageBox.Show("起页号、止页号之间的分隔符不能为空！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                AppConfigModify.ModifyItem("NoEndPageNumber_SplitChar", textEdit2.Text);
                AppConfigModify.ModifyItem("UseNoEndPageNumber_SplitChar", "true");
            }
            else
            {
                AppConfigModify.ModifyItem("UseNoEndPageNumber_SplitChar", "false");
            }
            if (checkedListBox4.GetItemChecked(0))
                AppConfigModify.ModifyItem("NoPageNumber", "false");
            else
                AppConfigModify.ModifyItem("NoPageNumber", "true");
            if (checkedListBox4.GetItemChecked(1))
                AppConfigModify.ModifyItem("NoStartPageNumber", "false");
            else
                AppConfigModify.ModifyItem("NoStartPageNumber", "true");
            if (checkedListBox4.GetItemChecked(2))
                AppConfigModify.ModifyItem("NoEndPageNumber", "false");
            else
                AppConfigModify.ModifyItem("NoEndPageNumber", "true");
            MessageBox.Show("配置成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textEdit3.Text))
            {
                MessageBox.Show("图像二值化时取样区域的边长不能为空！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
                AppConfigModify.ModifyItem("SideLengthVlue", textEdit3.Text);

            if (string.IsNullOrEmpty(textEdit4.Text))
            {
                MessageBox.Show("图像二值化阈值不能为空！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
                AppConfigModify.ModifyItem("BinaryzationThreshold", textEdit4.Text);

            if (string.IsNullOrEmpty(textEdit6.Text))
            {
                MessageBox.Show("图像允许的最大倾斜度不能为空！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
                AppConfigModify.ModifyItem("ImageLeanDegree", textEdit6.Text);

            if (checkEdit2.Checked)
                AppConfigModify.ModifyItem("MakeUpWhiteSideInPaper", "true");
            else
                AppConfigModify.ModifyItem("MakeUpWhiteSideInPaper", "false");
            MessageBox.Show("配置成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textEdit3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }
        }

        private void textEdit6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }
        }

        private void textEdit4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键  
            {
                if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == '.')
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void LoadInitialData()
        {
            string sql = "SELECT Unique_code,col_name,show_name FROM t_config_col_dict WHERE code='" + this.FileCodeName + "'";
            DataTable dtFromDB = new DbHelper().Fill(sql);
            checkedListBoxControl_config_autoCheckClassify_forSelect.Items.Clear();
            checkedListBoxControl_autoCheckClassify_selected.Items.Clear();
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBoxControl_config_autoCheckClassify_forSelect.Items.Add(item);
            }
            dtFromDB.Dispose();

            sql = "SELECT t2.Unique_code,t2.col_name,t2.show_name FROM t_config_auto_check_classify AS t1 INNER JOIN t_config_col_dict AS t2 ON t1.selected_code=t2.Unique_code WHERE t2.code='" + this.FileCodeName + "' ORDER BY CAST(order_number AS INT) ASC";
            dtFromDB = new DbHelper().Fill(sql);
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBoxControl_autoCheckClassify_selected.Items.Add(item);
                CheckedListBoxItem item_delete = checkedListBoxControl_config_autoCheckClassify_forSelect.Items.SingleOrDefault(t => t.Tag.ToString().Equals(item.Tag.ToString()));
                checkedListBoxControl_config_autoCheckClassify_forSelect.Items.Remove(item_delete);
            }
            dtFromDB.Dispose();
        }

        private void LoadInitialData_FieldGroup()
        {
            string sql = "SELECT Unique_code,col_name,show_name FROM t_config_col_dict WHERE code='" + this.FileCodeName + "'";
            DataTable dtFromDB = new DbHelper().Fill(sql);
            checkedListBoxControl_fieldGroup_for_select.Items.Clear();
            checkedListBoxControl_fieldGroup_selected.Items.Clear();
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBoxControl_fieldGroup_for_select.Items.Add(item);
            }
            dtFromDB.Dispose();

            sql = "SELECT t2.Unique_code,t2.col_name,t2.show_name FROM t_config_fields_group_for_autocheck AS t1 INNER JOIN t_config_col_dict AS t2 ON t1.selected_code=t2.Unique_code WHERE t2.code='" + this.FileCodeName + "' ORDER BY CAST(order_number AS INT) ASC";
            dtFromDB = new DbHelper().Fill(sql);
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBoxControl_fieldGroup_selected.Items.Add(item);
                CheckedListBoxItem item_delete = checkedListBoxControl_fieldGroup_for_select.Items.SingleOrDefault(t => t.Tag.ToString().Equals(item.Tag.ToString()));
                checkedListBoxControl_fieldGroup_for_select.Items.Remove(item_delete);
            }
            dtFromDB.Dispose();

            sql = "SELECT b.name colName FROM sysobjects a INNER JOIN syscolumns b \r\n";
            sql += "ON a.id=b.id AND a.xtype='U' \r\n";
            sql += "WHERE a.name='" + this.FileCodeName + "'";
            dtFromDB = new DbHelper().Fill(sql);
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                ComboboxItem item = new ComboboxItem(dtFromDB.Rows[i]["colName"].ToString(), null);
                comboBoxEdit_secretField.Properties.Items.Add(item);
            }
            dtFromDB.Dispose();

            sql = "SELECT secret_field,yw_catalog_identical FROM t_config_field_for_autocheck WHERE table_code='" + this.FileCodeName + "'";
            dtFromDB = new DbHelper().Fill(sql);
            if (dtFromDB.Rows.Count > 0)
            {
                string secreteField = dtFromDB.Rows[0]["secret_field"].ToString();
                comboBoxEdit_secretField.SelectedIndex = -1;
                for (int i = 0; i < comboBoxEdit_secretField.Properties.Items.Count; i++)
                {
                    if (((ComboboxItem)(comboBoxEdit_secretField.Properties.Items[i])).Display.Equals(secreteField))
                    {
                        comboBoxEdit_secretField.SelectedIndex = i;
                        break;
                    }
                }
                object identicalObj = dtFromDB.Rows[0]["yw_catalog_identical"];
                if (identicalObj == DBNull.Value || !Boolean.Parse(identicalObj.ToString()))
                    checkEdit4.Checked = false;
                else
                    checkEdit4.Checked = true;

            }
            dtFromDB.Dispose();
        }

        private void LoadInitialData_PageLeft()
        {
            string sql = "SELECT b.name colName FROM sysobjects a INNER JOIN syscolumns b \r\n";
            sql += "ON a.id=b.id AND a.xtype='U' \r\n";
            sql += "WHERE a.name='" + this.FileCodeName + "'";
            DataTable dtFromDB = new DbHelper().Fill(sql);
            comboBoxEdit_ys.Properties.Items.Clear();
            comboBoxEdit_qyh.Properties.Items.Clear();
            comboBoxEdit_zyh.Properties.Items.Clear();
            comboBoxEdit_xcsj.Properties.Items.Clear();
            comboBoxEdit_nd.Properties.Items.Clear();
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                ComboboxItem item = new ComboboxItem(dtFromDB.Rows[i]["colName"].ToString(), null);
                comboBoxEdit_ys.Properties.Items.Add(item);
            }
            comboBoxEdit_qyh.Properties.Items.AddRange(comboBoxEdit_ys.Properties.Items);
            comboBoxEdit_zyh.Properties.Items.AddRange(comboBoxEdit_ys.Properties.Items);
            comboBoxEdit_xcsj.Properties.Items.AddRange(comboBoxEdit_ys.Properties.Items);
            comboBoxEdit_nd.Properties.Items.AddRange(comboBoxEdit_ys.Properties.Items);
            comboBoxEdit_ys.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            comboBoxEdit_qyh.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            comboBoxEdit_zyh.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            comboBoxEdit_xcsj.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            comboBoxEdit_nd.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            dtFromDB.Dispose();

            sql = "SELECT ys_field,qyh_field,zyh_field,nd_field,xcsj_field FROM t_config_field_for_autocheck WHERE table_code='" + this.FileCodeName + "'";
            dtFromDB = new DbHelper().Fill(sql);
            if (dtFromDB.Rows.Count > 0)
            {
                DataRow dr = dtFromDB.Rows[0];
                object ysField = dr["ys_field"];
                object qyhField = dr["qyh_field"];
                object zyhField = dr["zyh_field"];
                object ndField = dr["nd_field"];
                object xcsjField = dr["xcsj_field"];
                if (ysField == DBNull.Value || string.IsNullOrEmpty(ysField.ToString()))
                {
                    checkedListBox4.SetItemChecked(0, false);
                    comboBoxEdit_ys.Enabled = false;
                }
                else
                {
                    checkedListBox4.SetItemChecked(0, true);
                    comboBoxEdit_ys.Enabled = true;
                    for (int i = 0; i < comboBoxEdit_ys.Properties.Items.Count; i++)
                    {
                        if (((ComboboxItem)comboBoxEdit_ys.Properties.Items[i]).Display.Equals(ysField.ToString()))
                            comboBoxEdit_ys.SelectedIndex = i;
                    }
                }
                if (qyhField == DBNull.Value || string.IsNullOrEmpty(qyhField.ToString()))
                {
                    checkedListBox4.SetItemChecked(1, false);
                    comboBoxEdit_qyh.Enabled = false;
                }
                else
                {
                    checkedListBox4.SetItemChecked(1, true);
                    comboBoxEdit_qyh.Enabled = true;
                    for (int i = 0; i < comboBoxEdit_qyh.Properties.Items.Count; i++)
                    {
                        if (((ComboboxItem)comboBoxEdit_qyh.Properties.Items[i]).Display.Equals(qyhField.ToString()))
                            comboBoxEdit_qyh.SelectedIndex = i;
                    }
                }
                if (zyhField == DBNull.Value || string.IsNullOrEmpty(zyhField.ToString()))
                {
                    checkedListBox4.SetItemChecked(2, false);
                    comboBoxEdit_zyh.Enabled = false;
                }
                else
                {
                    checkedListBox4.SetItemChecked(2, true);
                    comboBoxEdit_zyh.Enabled = true;
                    for (int i = 0; i < comboBoxEdit_zyh.Properties.Items.Count; i++)
                    {
                        if (((ComboboxItem)comboBoxEdit_zyh.Properties.Items[i]).Display.Equals(zyhField.ToString()))
                            comboBoxEdit_zyh.SelectedIndex = i;
                    }
                }
                if (ndField == DBNull.Value || string.IsNullOrEmpty(ndField.ToString()))
                {
                    checkedListBox1.SetItemChecked(0, false);
                    comboBoxEdit_nd.Enabled = false;
                }
                else
                {
                    checkedListBox1.SetItemChecked(0, true);
                    comboBoxEdit_nd.Enabled = true;
                    for (int i = 0; i < comboBoxEdit_nd.Properties.Items.Count; i++)
                    {
                        if (((ComboboxItem)comboBoxEdit_nd.Properties.Items[i]).Display.Equals(ndField.ToString()))
                            comboBoxEdit_nd.SelectedIndex = i;
                    }
                }
                if (xcsjField == DBNull.Value || string.IsNullOrEmpty(xcsjField.ToString()))
                {
                    checkedListBox1.SetItemChecked(1, false);
                    comboBoxEdit_xcsj.Enabled = false;
                }
                else
                {
                    checkedListBox1.SetItemChecked(1, true);
                    comboBoxEdit_xcsj.Enabled = true;
                    for (int i = 0; i < comboBoxEdit_xcsj.Properties.Items.Count; i++)
                    {
                        if (((ComboboxItem)comboBoxEdit_xcsj.Properties.Items[i]).Display.Equals(xcsjField.ToString()))
                            comboBoxEdit_xcsj.SelectedIndex = i;
                    }
                }
                string usePageNumber_SplitChar = ConfigurationManager.AppSettings["UseNoEndPageNumber_SplitChar"];
                string noEndPageNumber_SplitChar = ConfigurationManager.AppSettings["NoEndPageNumber_SplitChar"];
                if (string.IsNullOrEmpty(usePageNumber_SplitChar) || !Boolean.Parse(usePageNumber_SplitChar))
                    checkEdit3.Checked = false;
                else
                {
                    checkEdit3.Checked = true;
                    textEdit2.Text = noEndPageNumber_SplitChar;
                }
                textEdit1.Text = ConfigurationManager.AppSettings["CharMadeUpInCreatedTime"];
            }
            else
            {
                checkedListBox4.SetItemChecked(0, false);
                checkedListBox4.SetItemChecked(1, false);
                checkedListBox4.SetItemChecked(2, false);
                checkedListBox1.SetItemChecked(0, false);
                checkedListBox1.SetItemChecked(1, false);
                comboBoxEdit_ys.Enabled = false;
                comboBoxEdit_qyh.Enabled = false;
                comboBoxEdit_zyh.Enabled = false;
                comboBoxEdit_nd.Enabled = false;
                comboBoxEdit_xcsj.Enabled = false;
                checkEdit3.Checked = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl_config_autoCheckClassify_forSelect.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_config_autoCheckClassify_forSelect.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    checkedListBoxControl_autoCheckClassify_selected.Items.Add(item);
                    item.CheckState = CheckState.Unchecked;
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBoxControl_config_autoCheckClassify_forSelect.Items.Remove(list[i]);
            }
            list = null;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl_autoCheckClassify_selected.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_autoCheckClassify_selected.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    checkedListBoxControl_config_autoCheckClassify_forSelect.Items.Add(item);
                    item.CheckState = CheckState.Unchecked;
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBoxControl_autoCheckClassify_selected.Items.Remove(list[i]);
            }
            list = null;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string sql = "SELECT Unique_code,col_name,show_name FROM t_config_col_dict WHERE code='" + this.FileCodeName + "'";
            DataTable dtFromDB = new DbHelper().Fill(sql);
            checkedListBoxControl_config_autoCheckClassify_forSelect.Items.Clear();
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBoxControl_config_autoCheckClassify_forSelect.Items.Add(item);
            }
            dtFromDB.Dispose();
            checkedListBoxControl_autoCheckClassify_selected.Items.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sql = string.Empty;
            string codeStr = string.Empty;
            for (int i = 0; i < checkedListBoxControl_autoCheckClassify_selected.Items.Count; i++)
            {
                string selectedcode = checkedListBoxControl_autoCheckClassify_selected.Items[i].Tag.ToString();
                sql = "DECLARE @colName nvarchar(50) \r\n";
                sql += "SELECT @colName = (SELECT col_name FROM t_config_col_dict WHERE Unique_code IN(SELECT selected_code FROM t_config_auto_check_classify WHERE selected_code=" + selectedcode + "))\r\n";
                sql += "IF @colName IS NULL\r\n";
                sql += "INSERT t_config_auto_check_classify(selected_code,order_number) VALUES(" + selectedcode + ",'" + i.ToString() + "')\r\n";
                sql += "ELSE \r\n";
                sql += "UPDATE t_config_auto_check_classify SET order_number='" + i.ToString() + "' WHERE selected_code=" + selectedcode + "\r\n";
                new DbHelper().ExecuteNonQuery(sql);
                if (i == 0)
                    codeStr += selectedcode;
                else
                    codeStr += "," + selectedcode;
            }
            sql = "DELETE FROM t_config_auto_check_classify WHERE selected_code NOT IN(" + codeStr + ") AND selected_code IN (SELECT Unique_code FROM t_config_col_dict WHERE code='" + this.FileCodeName + "')";
            new DbHelper().ExecuteNonQuery(sql);
            MessageBox.Show("保存完毕！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void simpleButton12_Click(object sender, EventArgs e)
        {
            int index = checkedListBoxControl_autoCheckClassify_selected.SelectedIndex;
            if (index - 1 >= 0)
            {
                CheckedListBoxItem item1 = checkedListBoxControl_autoCheckClassify_selected.Items[index];
                checkedListBoxControl_autoCheckClassify_selected.Items.Insert(index - 1, item1);
                checkedListBoxControl_autoCheckClassify_selected.Items.RemoveAt(index + 1);
                checkedListBoxControl_autoCheckClassify_selected.SelectedIndex = index - 1;
                checkedListBoxControl_autoCheckClassify_selected.Items[index - 1].CheckState = CheckState.Checked;
            }
        }

        private void simpleButton7_Click(object sender, EventArgs e)
        {
            int index = checkedListBoxControl_autoCheckClassify_selected.SelectedIndex;
            if (index + 2 <= checkedListBoxControl_autoCheckClassify_selected.Items.Count)
            {
                CheckedListBoxItem item1 = checkedListBoxControl_autoCheckClassify_selected.Items[index];
                checkedListBoxControl_autoCheckClassify_selected.Items.Insert(index + 2, item1);
                checkedListBoxControl_autoCheckClassify_selected.Items.RemoveAt(index);
                checkedListBoxControl_autoCheckClassify_selected.SelectedIndex = index + 1;
                checkedListBoxControl_autoCheckClassify_selected.Items[index + 1].CheckState = CheckState.Checked;
            }
        }

        private void checkedListBoxControl_autoCheckClassify_selected_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = checkedListBoxControl_autoCheckClassify_selected.SelectedIndex;
            for (int i = 0; i < checkedListBoxControl_autoCheckClassify_selected.Items.Count; i++)
            {
                if (i != index)
                {
                    checkedListBoxControl_autoCheckClassify_selected.Items[i].CheckState = CheckState.Unchecked;
                }
            }
        }

        private void checkedListBoxControl_fieldGroup_selected_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = checkedListBoxControl_fieldGroup_selected.SelectedIndex;
            for (int i = 0; i < checkedListBoxControl_fieldGroup_selected.Items.Count; i++)
            {
                if (i != index)
                {
                    checkedListBoxControl_fieldGroup_selected.Items[i].CheckState = CheckState.Unchecked;
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl_fieldGroup_for_select.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_fieldGroup_for_select.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    checkedListBoxControl_fieldGroup_selected.Items.Add(item);
                    item.CheckState = CheckState.Unchecked;
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBoxControl_fieldGroup_for_select.Items.Remove(list[i]);
            }
            list = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl_fieldGroup_selected.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_fieldGroup_selected.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    checkedListBoxControl_fieldGroup_for_select.Items.Add(item);
                    item.CheckState = CheckState.Unchecked;
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBoxControl_fieldGroup_selected.Items.Remove(list[i]);
            }
            list = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = "SELECT Unique_code,col_name,show_name FROM t_config_col_dict WHERE code='" + this.FileCodeName + "'";
            DataTable dtFromDB = new DbHelper().Fill(sql);
            checkedListBoxControl_fieldGroup_for_select.Items.Clear();
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBoxControl_fieldGroup_for_select.Items.Add(item);
            }
            dtFromDB.Dispose();
            checkedListBoxControl_fieldGroup_selected.Items.Clear();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string sql = string.Empty;
            string codeStr = string.Empty;
            for (int i = 0; i < checkedListBoxControl_fieldGroup_selected.Items.Count; i++)
            {
                string selectedcode = checkedListBoxControl_fieldGroup_selected.Items[i].Tag.ToString();
                sql = "DECLARE @colName nvarchar(50) \r\n";
                sql += "SELECT @colName = (SELECT col_name FROM t_config_col_dict WHERE Unique_code IN(SELECT selected_code FROM t_config_fields_group_for_autocheck WHERE selected_code=" + selectedcode + "))\r\n";
                sql += "IF @colName IS NULL\r\n";
                sql += "INSERT t_config_fields_group_for_autocheck(selected_code,order_number) VALUES(" + selectedcode + ",'" + i.ToString() + "')\r\n";
                sql += "ELSE \r\n";
                sql += "UPDATE t_config_fields_group_for_autocheck SET order_number='" + i.ToString() + "' WHERE selected_code=" + selectedcode + "\r\n";
                new DbHelper().ExecuteNonQuery(sql);
                if (i == 0)
                    codeStr += selectedcode;
                else
                    codeStr += "," + selectedcode;
            }
            sql = "DELETE FROM t_config_fields_group_for_autocheck WHERE selected_code NOT IN(" + codeStr + ") AND selected_code IN (SELECT Unique_code FROM t_config_col_dict WHERE code='" + this.FileCodeName + "')";
            new DbHelper().ExecuteNonQuery(sql);
            MessageBox.Show("保存完毕！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void simpleButton13_Click(object sender, EventArgs e)
        {
            int index = checkedListBoxControl_fieldGroup_selected.SelectedIndex;
            if (index - 1 >= 0)
            {
                CheckedListBoxItem item1 = checkedListBoxControl_fieldGroup_selected.Items[index];
                checkedListBoxControl_fieldGroup_selected.Items.Insert(index - 1, item1);
                checkedListBoxControl_fieldGroup_selected.Items.RemoveAt(index + 1);
                checkedListBoxControl_fieldGroup_selected.SelectedIndex = index - 1;
                checkedListBoxControl_fieldGroup_selected.Items[index - 1].CheckState = CheckState.Checked;
            }
        }

        private void simpleButton4_Click_1(object sender, EventArgs e)
        {
            int index = checkedListBoxControl_fieldGroup_selected.SelectedIndex;
            if (index + 2 <= checkedListBoxControl_fieldGroup_selected.Items.Count)
            {
                CheckedListBoxItem item1 = checkedListBoxControl_fieldGroup_selected.Items[index];
                checkedListBoxControl_fieldGroup_selected.Items.Insert(index + 2, item1);
                checkedListBoxControl_fieldGroup_selected.Items.RemoveAt(index);
                checkedListBoxControl_fieldGroup_selected.SelectedIndex = index + 1;
                checkedListBoxControl_fieldGroup_selected.Items[index + 1].CheckState = CheckState.Checked;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (comboBoxEdit_secretField.SelectedIndex == -1)
            {
                MessageBox.Show("请在列表中选择代表密级的字段名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int identical = checkEdit4.Checked ? 1 : 0;
            string sql = "SELECT secret_field FROM t_config_field_for_autocheck WHERE table_code='" + this.FileCodeName + "' \r\n";
            sql += "IF @@Rowcount = 0 \r\n";
            sql += "    BEGIN \r\n";
            sql += "      INSERT INTO t_config_field_for_autocheck(table_code,secret_field,yw_catalog_identical) VALUES('" + this.FileCodeName + "','" + comboBoxEdit_secretField.Text + "'," + identical + ") \r\n";
            sql += "    END \r\n";
            sql += "ELSE \r\n";
            sql += "    BEGIN \r\n";
            sql += "      UPDATE t_config_field_for_autocheck SET secret_field='" + comboBoxEdit_secretField.Text + "',yw_catalog_identical=" + identical + " WHERE table_code='" + this.FileCodeName + "'\r\n";
            sql += "    END \r\n";
            new DbHelper().ExecuteNonQuery(sql);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void simpleButton5_Click_1(object sender, EventArgs e)
        {
            if (!checkedListBox4.GetItemChecked(0) && !checkedListBox4.GetItemChecked(1) && !checkedListBox4.GetItemChecked(2))
            {
                MessageBox.Show("页数、起页号、止页号三者至少需要选择一个！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(textEdit2.Text))
            {
                MessageBox.Show("起页号、止页号之间的分隔符不能为空！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(textEdit1.Text))
            {
                MessageBox.Show("形成时间不确定则所补的字符不能为空！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string ysField = null, qyhField = null, zyhField = null, xcsjField = null, ndField = null;
            if (checkedListBox4.GetItemChecked(0)) ysField = comboBoxEdit_ys.Text;
            if (checkedListBox4.GetItemChecked(0) && string.IsNullOrEmpty(ysField))
            {
                MessageBox.Show("请选择页数字段！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (checkedListBox4.GetItemChecked(1)) qyhField = comboBoxEdit_qyh.Text;
            if (checkedListBox4.GetItemChecked(1) && string.IsNullOrEmpty(qyhField))
            {
                MessageBox.Show("请选择起页号字段！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (checkedListBox4.GetItemChecked(2)) zyhField = comboBoxEdit_zyh.Text;
            if (checkedListBox4.GetItemChecked(2) && string.IsNullOrEmpty(zyhField))
            {
                MessageBox.Show("请选择止页号字段！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (checkedListBox1.GetItemChecked(0)) xcsjField = comboBoxEdit_xcsj.Text;
            if (checkedListBox1.GetItemChecked(0) && string.IsNullOrEmpty(xcsjField))
            {
                MessageBox.Show("请选择形成时间字段！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (checkedListBox1.GetItemChecked(1)) ndField = comboBoxEdit_nd.Text;
            if (checkedListBox1.GetItemChecked(1) && string.IsNullOrEmpty(ndField))
            {
                MessageBox.Show("请选择年度字段！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sql = "SELECT secret_field FROM t_config_field_for_autocheck WHERE table_code='" + this.FileCodeName + "' \r\n";
            sql += "IF @@Rowcount = 0 \r\n";
            sql += "    BEGIN \r\n";
            sql += "      INSERT INTO t_config_field_for_autocheck(table_code,ys_field,qyh_field,zyh_field,nd_field,xcsj_field) VALUES('" + this.FileCodeName + "','" + ysField + "','" + qyhField + "','" + zyhField + "','" + ndField + "','" + xcsjField + "') \r\n";
            sql += "    END \r\n";
            sql += "ELSE \r\n";
            sql += "    BEGIN \r\n";
            sql += "      UPDATE t_config_field_for_autocheck SET ys_field='" + ysField + "',qyh_field='" + qyhField + "',zyh_field='" + zyhField + "',nd_field='" + ndField + "',xcsj_field='" + xcsjField + "' WHERE table_code='" + this.FileCodeName + "'\r\n";
            sql += "    END \r\n";
            new DbHelper().ExecuteNonQuery(sql);

            AppConfigModify.ModifyItem("NoEndPageNumber_SplitChar", textEdit2.Text);
            AppConfigModify.ModifyItem("CharMadeUpInCreatedTime", textEdit1.Text);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //void SetComboBoxState()
        //{
        //    comboBoxEdit_ys.Enabled = checkedListBox4.GetItemChecked(0);
        //    comboBoxEdit_qyh.Enabled = checkedListBox4.GetItemChecked(1);
        //    comboBoxEdit_zyh.Enabled = checkedListBox4.GetItemChecked(2);
        //    comboBoxEdit_xcsj.Enabled = checkedListBox1.GetItemChecked(0);
        //    comboBoxEdit_nd.Enabled = checkedListBox1.GetItemChecked(1);
        //}

        private void checkedListBox4_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if (e.Index ==0)
            {
                comboBoxEdit_ys.Enabled = e.CurrentValue == CheckState.Unchecked ? true : false;
            }
            if (e.Index == 1)
            {
                comboBoxEdit_qyh.Enabled = e.CurrentValue == CheckState.Unchecked ? true : false;
            }
            if (e.Index == 2)
            {
                comboBoxEdit_zyh.Enabled = e.CurrentValue == CheckState.Unchecked ? true : false;
            }
        }

        private void checkedListBox1_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if (e.Index == 0)
            {
                comboBoxEdit_xcsj.Enabled = e.CurrentValue == CheckState.Unchecked ? true : false;
            }
            if (e.Index == 1)
            {
                comboBoxEdit_nd.Enabled = e.CurrentValue == CheckState.Unchecked ? true : false;
            }
        }

    }
}