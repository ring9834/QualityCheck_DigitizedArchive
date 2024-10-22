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
using DevExpress.XtraTab;

namespace Prj_FileManageNCheckApp
{
    public delegate void UpdateBackButtonState();

    public partial class XtraGoFrontBack : DevExpress.XtraEditors.XtraUserControl
    {
        private int PageIndex { get; set; }
        private int PageCount { get; set; }

        public XtraTabControl TabControl { get; set; }
        
        public UpdateBackButtonState UpdateBackButtonStateFun { get; set; }

        public XtraGoFrontBack()
        {
            InitializeComponent();
            //UpdateBackButtonStateFun = new UpdateBackButtonState(UpdateControls);
            //if (TabControl != null)
            //{
            //    this.PageCount = TabControl.TabPages.Count;
            //    this.PageIndex = TabControl.SelectedTabPageIndex;
            //    VerifyButtonState();
            //}
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {;
            if (TabControl != null)
            {
                this.PageIndex = this.PageIndex - 1;
                TabControl.SelectedTabPageIndex = this.PageIndex;
                VerifyButtonState();
            }
        }

        private void VerifyButtonState()
        {
            if (this.PageIndex > 0 && this.PageIndex <= this.PageCount - 1)
            {
                simpleButton1.Enabled = true;
            }
            else
            {
                simpleButton1.Enabled = false;
            }
        }

        public void UpdateControls()
        {
            this.PageCount = TabControl.TabPages.Count;
            this.PageIndex = TabControl.SelectedTabPageIndex;
            VerifyButtonState();
        }
    }
}
