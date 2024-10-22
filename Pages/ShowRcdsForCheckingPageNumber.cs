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

namespace Prj_FileManageNCheckApp
{
    public partial class ShowRcdsForCheckingPageNumber : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }
        public DataTable DT { get; set; }
        public ShowRcdsForCheckingPageNumber()
        {
            InitializeComponent();
        }

        public ShowRcdsForCheckingPageNumber(DataTable dt)
        {
            InitializeComponent();
            this.DT = dt;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            gridControl_checkingPageNumber.DataSource = this.DT;
            gridView_checkingPageNumber.PopulateColumns();
            gridView_checkingPageNumber.BestFitColumns();
        }

        public void UpdateDataTable(DataTable dt)
        {
            this.DT = dt;
        }

        private void ShowRcdsForCheckingPageNumber_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.DT != null)
            {
                this.DT.Dispose();
            }
        }
    }
}