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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;

namespace Prj_FileManageNCheckApp
{
    public partial class DataCountReport2 : DevExpress.XtraEditors.XtraForm
    {
        public DataTable SearchDatatable { get; set; }
        private RepositoryItemComboBox RepositoryItemComboBox1 { get; set; }
        private List<KeyValuePair<string, StatisticsCondition>> ConditionList { get; set; }
        public DataCountReport2()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            //string sql = "SELECT distinct qzh+'-'+mlh AS name,count(distinct ajh) AS cnt FROM Jnda group by qzh+'-'+mlh";
            ////string sql = "SELECT distinct qzh+'-'+mlh AS name,ajh FROM Jnda";
            //DataTable dt = new DbHelper().Fill(sql);
            //XtraReport3 rpt = new XtraReport3(dt);
            XtraReport4 rpt = new XtraReport4();
            this.documentViewer1.DocumentSource = rpt;
            rpt.CreateDocument();
        }


    }
}