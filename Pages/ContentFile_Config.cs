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

namespace Prj_FileManageNCheckApp
{
    public partial class ContentFile_Config : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        public UserEntity UserLoggedIn { get; set; }
        public ContentFile_Config()
        {
            InitializeComponent();
        }

        public ContentFile_Config(string fileCodeName,UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        private void ContentFile_Config_Load(object sender, EventArgs e)
        {
            textEdit1.Text = ConfigurationManager.AppSettings["ContentFileRootPath"];
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textEdit2.Text))
            {
                MessageBox.Show("请选择合适路径作为原文根路径！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AppConfigModify.ModifyItem("ContentFileRootPath", textEdit2.Text);
            textEdit1.Text = ConfigurationManager.AppSettings["ContentFileRootPath"];
            MessageBox.Show("设置成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textEdit2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textEdit2.Text = folderBrowserDialog1.SelectedPath;                
            }
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            PageControlLocation.MakeControlCenter(groupControl1.Parent, groupControl1);
        }

        private void ContentFile_Config_Activated(object sender, EventArgs e)
        {
            simpleButton1.Focus();
        }

    }
}