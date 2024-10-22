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
using DevExpress.XtraGrid.Views.Grid;
using DotNet.DbUtilities;

namespace Prj_FileManageNCheckApp
{
    public delegate void RefreshGridViewData();
    public delegate bool VerifyIfARecordSelected(GridView grid);
    public partial class XtraDeleteGridviewRecord : DevExpress.XtraEditors.XtraUserControl
    {
        public GridView TargetGridView { get; set; }
        public string SqlStr { get; set; }
        public string VerifyInformationInAlertWindow { get; set; }
        public RefreshGridViewData RefreshGridViewDataFunc { get; set; }
        public GetParamsForDeleteButton GetParamsForDeleteButtonFunc { get; set; }
        public VerifyIfARecordSelected VerifyIfARecordSelectedFunc { get; set; }
        public bool EnableButton
        {
            get { return this.simpleButton1.Enabled; }
            set { this.simpleButton1.Enabled = (bool)value; }
        }

        public XtraDeleteGridviewRecord()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            bool flag = VerifyIfARecordSelectedFunc(TargetGridView);//判断是否选择了一条记录
            if (!flag)
            {
                MessageBox.Show("请选择一条要删除的记录！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show(VerifyInformationInAlertWindow, "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                GetParamsForDeleteButtonFunc(TargetGridView);//获得自己需要的应有的参数
                new DbHelper().ExecuteNonQuery(SqlStr);
                RefreshGridViewDataFunc();//执行委托，刷新GridView
            }
        }
    }
}
