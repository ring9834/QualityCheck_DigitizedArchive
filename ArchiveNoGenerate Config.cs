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
        public ArchiveNoGenerate_Config()
        {
            InitializeComponent();
        }

        public ArchiveNoGenerate_Config(string fileCodeName)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
        }

        private void ArchiveNoGenerate_Config_Load(object sender, EventArgs e)
        {
            Combobox1_BindData();
            PageControlLocation.MakeControlCenter(panel1.Parent, panel1);
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

        public void InitiateDatas(string fileCodeName)
        {
            this.FileCodeName = fileCodeName;
            //layoutControl1.Visible = true;
            //panel1.Visible = true;
            //button1.Visible = true;
            //label1.Visible = true;

            string sql = "SELECT * FROM t_config_archive_num_makeup WHERE code_name='" + this.FileCodeName + "'";
            DataTable dtmakeup = new DbHelper().Fill(sql);
            if (dtmakeup.Rows.Count > 0)
            {
                DataRow dr = dtmakeup.Rows[0];
                if (dr != null)
                {
                    string archive_num_parts_amount = dr["archive_num_parts_amount"].ToString();
                    comboBox1.SelectedIndex = int.Parse(archive_num_parts_amount) - 1;
                    panel2.Controls.Clear();
                    sql = "SELECT col_name,show_name FROM t_config_col_dict WHERE code='" + this.FileCodeName + "'";
                    DataTable dt = new DbHelper().Fill(sql);

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
                sql = "SELECT col_name,show_name FROM t_config_col_dict WHERE code='" + this.FileCodeName + "'";
                DataTable dt = new DbHelper().Fill(sql);
                comboBox3.DataSource = dt;
                comboBox3.DisplayMember = "show_name";
                comboBox3.ValueMember = "col_name";
            }
            //}
            //else
            //{
            //    layoutControl1.Visible = false;
            //    panel1.Visible = false;
            //    button1.Visible = false;
            //    label1.Visible = false;
            //}
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
                string sql = "SELECT col_name,show_name FROM t_config_col_dict WHERE code='" + this.FileCodeName + "'";
                DataTable dt = new DbHelper().Fill(sql);

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

            string archive_num_parts_amount = ((ComboboxItem)comboBox1.SelectedItem).Value;
            string archive_prefix = textBox1.Text;
            string connect_char = textBox2.Text;
            string archive_num_field = ((DataRowView)comboBox3.SelectedItem).Row.ItemArray[0].ToString();
            string archive_body = GetConfigXML();
            string sql = string.Empty;
            sql += "IF (SELECT COUNT(*) FROM t_config_archive_num_makeup WHERE code_name='" + this.FileCodeName + "') = 0 \r\n";
            sql += "BEGIN \r\n";
            sql += "INSERT t_config_archive_num_makeup(archive_num_parts_amount,archive_prefix,archive_body,connect_char,archive_num_field,code_name)\r\n";
            sql += "VALUES('" + archive_num_parts_amount + "','" + archive_prefix + "','" + archive_body + "','" + connect_char + "','" + archive_num_field + "','" + this.FileCodeName + "') \r\n";
            sql += "END \r\n";
            sql += "ELSE \r\n";
            sql += "BEGIN \r\n";
            sql += "UPDATE  t_config_archive_num_makeup SET archive_num_parts_amount='" + archive_num_parts_amount + "',archive_prefix='" + archive_prefix + "',archive_body='" + archive_body + "',\r\n";
            sql += "connect_char='" + connect_char + "', archive_num_field='" + archive_num_field + "' WHERE code_name='" + this.FileCodeName + "'\r\n";
            sql += "END \r\n";
            int result = new DbHelper().ExecuteNonQuery(sql);
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