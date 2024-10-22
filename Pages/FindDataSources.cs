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
using System.Data.Sql;
using System.Data.SqlClient;
using DotNet.DbUtilities;
using System.Configuration;

namespace Prj_FileManageNCheckApp
{
    public partial class FindDataSources : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }
        public UserEntity UserLoggedIn { get; set; }
        public FindDataSources()
        {
            InitializeComponent();
        }

        public FindDataSources(string fileCodeName, UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            if (paras != null)
                this.FileCodeName = paras.FileCodeName;
            PageControlLocation.MakeControlCenter(groupControl1.Parent, groupControl1);
            //LoadServers();
            LoadDbServerInfos();
        }

        void LoadServers()
        {
            //DataTable dataSources = SqlClientFactory.Instance.CreateDataSourceEnumerator().GetDataSources();
            SqlDataSourceEnumerator sqlserver = SqlDataSourceEnumerator.Instance;
            DataTable db = sqlserver.GetDataSources();
        }

        void LoadDbServerInfos()
        {
            string businessString = ConfigurationManager.AppSettings["BusinessDbConnection"];
            string[] strs = businessString.Split(';');
            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i].Contains("Data Source="))
                {
                    comboBoxEdit1.Text = strs[i].Substring(strs[i].IndexOf("=") + 1);
                }
                if (strs[i].Contains("Initial Catalog="))
                {
                    comboBoxEdit2.Text = strs[i].Substring(strs[i].IndexOf("=") + 1);
                }
                if (strs[i].Contains("User ID="))
                {
                    textEdit1.Text = strs[i].Substring(strs[i].IndexOf("=") + 1);
                }
                if (strs[i].Contains("pwd="))
                {
                    textEdit2.Text = strs[i].Substring(strs[i].IndexOf("=") + 1);
                }
            }
        }

        bool TestConnection()
        {
            string dbType = ConfigurationManager.AppSettings["DataBaseType"];
            string businessString = ConfigurationManager.AppSettings["BusinessDbConnection"];
            if (!businessString.Contains("Data Source="))
            {
                MessageBox.Show("连接数据库的字符串缺少“数据库服务器名称（Data Source）”项！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!businessString.Contains("Initial Catalog="))
            {
                MessageBox.Show("连接数据库的字符串缺少“数据库名（Initial Catalog）”项！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!businessString.Contains("User ID="))
            {
                MessageBox.Show("连接数据库的字符串缺少“登录名（User ID）”项！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!businessString.Contains("pwd="))
            {
                MessageBox.Show("连接数据库的字符串缺少“登录密码（pwd）”项！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            string[] strs = businessString.Split(';');
            string dataSource = string.Empty;
            string database = string.Empty;
            string user = string.Empty;
            string pwd = string.Empty;
            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i].Contains("Data Source="))
                {
                    dataSource = strs[i].Substring(strs[i].IndexOf("=") + 1);
                }
                if (strs[i].Contains("Initial Catalog="))
                {
                    database = strs[i].Substring(strs[i].IndexOf("=") + 1);
                }
                if (strs[i].Contains("User ID="))
                {
                    user = strs[i].Substring(strs[i].IndexOf("=") + 1);
                }
                if (strs[i].Contains("pwd="))
                {
                    pwd = strs[i].Substring(strs[i].IndexOf("=") + 1);
                }
            }
            bool flag = new DbHelper().TestConn(DotNet.Utilities.DataBaseType.Sqlserver, database, user, pwd, dataSource, false);
            return flag;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBoxEdit1.Text))
            {
                MessageBox.Show("数据库服务器名称不能为空！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(comboBoxEdit2.Text))
            {
                MessageBox.Show("数据库名不能为空！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(textEdit1.Text))
            {
                MessageBox.Show("登录名不能为空！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(textEdit2.Text))
            {
                MessageBox.Show("登录密码不能为空！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string connstr = "Data Source=" + comboBoxEdit1.Text + ";Initial Catalog=" + comboBoxEdit2.Text + ";User ID=" + textEdit1.Text + ";pwd=" + textEdit2.Text + ";MultipleActiveResultSets=true;";
            AppConfigModify.ModifyItem("BusinessDbConnection", connstr);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}