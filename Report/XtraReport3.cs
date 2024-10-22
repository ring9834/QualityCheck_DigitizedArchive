using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;

namespace Prj_FileManageNCheckApp
{
    public partial class XtraReport3 : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraReport3()
        {
            InitializeComponent();
        }

        public XtraReport3(DataTable dt)
        {
            InitializeComponent();
            this.DataSource = dt;
            this.xrTableCell1.DataBindings.Add("Text", dt, "name");
            GroupField gf = new GroupField("name", XRColumnSortOrder.Ascending);//设置GroupHeader分组字段 
            ((GroupHeaderBand)(this.FindControl("GroupHeader1", true))).GroupFields.Add(gf);//把分组字段添加进GroupHeader1 
            this.xrLabel1.DataBindings.Add("Text", dt, "cnt"); 
        }
    }
}
