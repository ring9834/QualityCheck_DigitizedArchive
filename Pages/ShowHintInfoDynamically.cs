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
using DevExpress.XtraTreeList;

namespace Prj_FileManageNCheckApp
{
    public partial class ShowHintInfoDynamically : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }
        public string ButtonText { get; set; }
        public string HintInfo { get; set; }

        public ShowHintInfoDynamically()
        {
            InitializeComponent();
        }

        public ShowHintInfoDynamically(string fileCodeName,string buttonText,string hintInfo)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.ButtonText = buttonText;
            this.HintInfo = hintInfo;
            PageControlLocation.MakeControlCenter(groupControl1.Parent, groupControl1);
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            labelControl1.Text = this.HintInfo;
            if (string.IsNullOrEmpty(this.ButtonText))
            {
                simpleButton1.Visible = false;
            }
            else 
            {
                simpleButton1.Visible = true;
                simpleButton1.Text = this.ButtonText;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            ConfigCodeDAO ccd = new ConfigCodeDAO();
            DataTable dtCodes = ccd.GetCodes("SJLX");
            if (dtCodes.Rows.Count == 0)
            {
                MessageBox.Show("编码配置中还没有“数据类型”方面的配置（字符型（nvarchar）、数据型(numberic)、日期型(datetime)等），请配置后在继续！", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataDictionaryDAO cdd = new DataDictionaryDAO();
            DataTable dtColbase = cdd.GetColDicBase();
            cdd.CreateBusinessDictTable(dtColbase, dtCodes, this.FileCodeName);

            MessageBox.Show("配置成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Form form = this.Parent.FindForm();
            if (form !=  null)
            {
                XtraForm_FileExamine xf = (XtraForm_FileExamine)form;
                xf.ShowHintInfoDynamically();
            }

        }
    }
}