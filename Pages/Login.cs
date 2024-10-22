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
using System.Data.Common;
using System.Configuration;

namespace Prj_FileManageNCheckApp
{
    public partial class LoginForm : DevExpress.XtraEditors.XtraForm
    {
        public UserEntity UserLoggedIn { get; set; }
        public LoginForm()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            PageControlLocation.MakeControlVerticalCenter(panelControl1.Parent, panelControl1);
            PageControlLocation.MakeControlHoritionalCenter(panelControl2.Parent, panelControl2);
            PageControlLocation.MakeControlHoritionalCenterNextToAnotherControl_Downward(panelControl1, panelControl3);
            simpleButton1.Focus();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (!TestConnection())
            {
                MessageBox.Show("数据库连接有误，请联系系统管理员进行正确配置！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(UserNameTxt.Text))
            {
                MessageBox.Show("用户名不能为空！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(PasswordTxt.Text))
            {
                MessageBox.Show("密码不能为空！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserDAO ud = new UserDAO();
            DataTable dt = ud.GetUserByNameAndPwd(UserNameTxt.Text, PasswordTxt.Text);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("用户名或密码错误，请重试！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            UserEntity user = new UserEntity();
            user.UniqueCode = dt.Rows[0]["Unique_code"].ToString();
            user.UserName = UserNameTxt.Text;
            user.RoleId = dt.Rows[0]["role_id"].ToString();
            UserLoggedIn = user;
            this.DialogResult = DialogResult.OK;//关键:设置登陆成功状态   
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            UserNameTxt.ResetText();
            PasswordTxt.ResetText();
        }

        bool TestConnection()
        {
            string dbType = ConfigurationManager.AppSettings["DataBaseType"];
            string businessString = ConfigurationManager.AppSettings["BusinessDbConnection"];
            if (!businessString.Contains("Data Source="))
            {
                MessageBox.Show("连接数据库的字符串缺少“数据库服务器名称（Data Source）”项！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!businessString.Contains("Initial Catalog="))
            {
                MessageBox.Show("连接数据库的字符串缺少“数据库名（Initial Catalog）”项！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!businessString.Contains("User ID="))
            {
                MessageBox.Show("连接数据库的字符串缺少“登录名（User ID）”项！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!businessString.Contains("pwd="))
            {
                MessageBox.Show("连接数据库的字符串缺少“登录密码（pwd）”项！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
    }
}