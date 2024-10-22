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

namespace Prj_FileManageNCheckApp
{
    public delegate void RefreshGridViewData();
    public partial class XtraUpdateButtonForGrid : DevExpress.XtraEditors.XtraUserControl
    {
        public RefreshGridViewData RefreshGridViewDataFunc { get; set; }
        public bool EnableButton
        {
            get { return this.simpleButton1.Enabled; }
            set { this.simpleButton1.Enabled = (bool)value; }
        }

        public XtraUpdateButtonForGrid()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            RefreshGridViewDataFunc();
        }
    }
}
