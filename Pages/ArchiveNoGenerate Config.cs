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
using System.Xml;
using System.Text.RegularExpressions;

namespace Prj_FileManageNCheckApp
{
    public partial class ArchiveNoGenerate_Config : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        public UserEntity UserLoggedIn { get; set; }
        public ArchiveNoGenerate_Config()
        {
            InitializeComponent();
        }

        public ArchiveNoGenerate_Config(string fileCodeName,UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        private void ArchiveNoGenerate_Config_Load(object sender, EventArgs e)
        {
            Combobox1_BindData();            
        }

        protected void Combobox1_BindData()
        {
            ComboboxItem item0 = new ComboboxItem("一段式", "1");
            ComboboxItem item1 = new ComboboxItem("二段式", "2");
            ComboboxItem item2 = new ComboboxItem("三段式", "3");
            ComboboxItem item3 = new ComboboxItem("四段式", "4");
            ComboboxItem item4 = new ComboboxItem("五段式", "5");
            ComboboxItem item5 = new ComboboxItem("六段式", "6");
            comboBox1.Items.Add(item0);
            comboBox1.Items.Add(item1);
            comboBox1.Items.Add(item2);
            comboBox1.Items.Add(item3);
            comboBox1.Items.Add(item4);
            comboBox1.Items.Add(item5);
            comboBox1.DisplayMember = "Display";
            comboBox1.ValueMember = "Value";
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;         
            PageControlLocation.MakeControlCenter(groupControl1.Parent, groupControl1);

            ArchiveNoDAO and = new ArchiveNoDAO();
            DataTable dtmakeup = and.GetArchiveNoDataByCodeName(this.FileCodeName);
            if (dtmakeup.Rows.Count > 0)
            {
                DataRow dr = dtmakeup.Rows[0];
                if (dr != null)
                {
                    string archive_num_parts_amount = dr["archive_num_parts_amount"].ToString();
                    comboBox1.SelectedIndex = int.Parse(archive_num_parts_amount) - 1;
                    panel2.Controls.Clear();
                    DataDictionaryDAO cdd = new DataDictionaryDAO();
                    DataTable dt = cdd.GetColNameShowNameByCodeName(this.FileCodeName);
                    string xml = dr["archive_body"].ToString();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    XmlNodeList nodeList = doc.SelectNodes(@"ArchiveNumMakeup/MakeupItem");
                    int num = int.Parse(archive_num_parts_amount);
                    for (int i = 0; i < num; i++)
                    {
                        System.Windows.Forms.ComboBox cb = new System.Windows.Forms.ComboBox();
                        cb.Name = "cb_archiveNumSection" + i;
                        cb.DataSource = dt.Copy();
                        cb.DisplayMember = "show_name";
                        cb.ValueMember = "col_name";
                        panel2.Controls.Add(cb);
                        cb.DropDownStyle = ComboBoxStyle.DropDownList;
                        cb.Width = 60;
                        cb.Top = 5;
                        cb.Left = 70 * i + 5;
                        string value = nodeList[i].Attributes["value"].Value;
                        int index = GetComboboxIndex(cb, value);
                        cb.SelectedIndex = index;
                    }

                    comboBox3.DataSource = dt.Copy();
                    comboBox3.DisplayMember = "show_name";
                    comboBox3.ValueMember = "col_name";
                    int index2 = GetComboboxIndex(comboBox3, dr["archive_num_field"].ToString());
                    comboBox3.SelectedIndex = index2;
                    textBox1.Text = dr["archive_prefix"].ToString();
                    textBox2.Text = dr["connect_char"].ToString();
                }
            }
            else
            {
                panel2.Controls.Clear();
                comboBox1.SelectedIndex = -1;
                DataDictionaryDAO cdd = new DataDictionaryDAO();
                DataTable dt = cdd.GetColNameShowNameByCodeName(this.FileCodeName);
                comboBox3.DataSource = dt;
                comboBox3.DisplayMember = "show_name";
                comboBox3.ValueMember = "col_name";
                comboBox3.SelectedIndex = -1;
            }
        }

        protected int GetComboboxIndex(System.Windows.Forms.ComboBox box, string value)
        {
            for (int i = 0; i < box.Items.Count; i++)
            {
                if (((DataRowView)box.Items[i]).Row.ItemArray[0].ToString() == value)
                {
                    return i;
                }
            }
            return -1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox1.Text))
            {
                panel2.Controls.Clear();
                DataDictionaryDAO cdd = new DataDictionaryDAO();
                DataTable dt = cdd.GetColNameShowNameByCodeName(this.FileCodeName);

                string selectedValue = ((ComboboxItem)comboBox1.SelectedItem).Value;
                int num = int.Parse(selectedValue);
                for (int i = 0; i < num; i++)
                {
                    System.Windows.Forms.ComboBox cb = new System.Windows.Forms.ComboBox();
                    cb.Name = "cb_archiveNumSection" + i;
                    cb.DataSource = dt.Copy();
                    cb.DisplayMember = "show_name";
                    cb.ValueMember = "col_name";
                    panel2.Controls.Add(cb);
                    cb.DropDownStyle = ComboBoxStyle.DropDownList;
                    cb.Width = 60;
                    cb.Top = 5;
                    cb.Left = 70 * i + 5;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("请选择档号规则，即，几段式的档号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                string s = "[0-9A-Za-z]{1,10}";
                Regex reg = new Regex(s);
                if (!reg.IsMatch(textBox1.Text))
                {
                    MessageBox.Show("前缀只能为数字或字母的组合！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (comboBox3.SelectedIndex < 0)
            {
                MessageBox.Show("档号对应的字段不能为空，请选择！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string archive_num_parts_amount = ((ComboboxItem)comboBox1.SelectedItem).Value;
            string archive_prefix = textBox1.Text;
            string connect_char = textBox2.Text;
            string archive_num_field = ((DataRowView)comboBox3.SelectedItem).Row.ItemArray[0].ToString();
            string archive_body = GetConfigXML();
            ArchiveNoDAO and = new ArchiveNoDAO();
            int result = and.InsertOrUpdateArchiveNoInfo(this.FileCodeName, archive_num_parts_amount, archive_prefix, archive_body, connect_char, archive_num_field);
            if (result > 0)
            {
                MessageBox.Show("档号设置成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected string GetConfigXML()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("ArchiveNumMakeup");
            doc.AppendChild(root);
            for (int i = 0; i < panel2.Controls.Count; i++)
            {
                XmlElement element = doc.CreateElement("MakeupItem");
                DataRowView cbItem = (DataRowView)(((System.Windows.Forms.ComboBox)panel2.Controls[i]).SelectedItem);
                element.SetAttribute("Name", cbItem.Row.ItemArray[1].ToString());
                element.SetAttribute("value", cbItem.Row.ItemArray[0].ToString());
                root.AppendChild(element);
            }
            return doc.OuterXml;
        }
    }
}