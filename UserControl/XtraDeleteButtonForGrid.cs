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
    public delegate bool IsOneRowSelected(GridView grid);
    public delegate void RefreshDataInGridView();
    public partial class XtraDeleteButtonForGrid : DevExpress.XtraEditors.XtraUserControl
    {

        public GridView TargetGrid { get; set; }
        public string SQL { get; set; }
        public IsOneRowSelected IsOneRowSelectedFunc { get; set; }
        public RefreshDataInGridView RefreshDataInGridViewFunc { get; set; }
        public string AlertInformation { get; set; }
        public bool EnableDeleteButton
        {
            get { return this.simpleButton1.Enabled; }
            set { this.simpleButton1.Enabled = (bool)value; }
        }

        public XtraDeleteButtonForGrid()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            bool flag = IsOneRowSelectedFunc(TargetGrid);
            if (flag)
            {
                if (MessageBox.Show(AlertInformation, "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    new DbHelper().ExecuteNonQuery(SQL);
                    //刷新Grid
                    RefreshDataInGridViewFunc();
                }
            }
        }
    }
}
