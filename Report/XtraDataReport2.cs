using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;
using DotNet.DbUtilities;
using System.Collections.Generic;
using System.Data.Common;

namespace Prj_FileManageNCheckApp
{
    public partial class XtraDataReport2 : DevExpress.XtraReports.UI.XtraReport
    {
        private bool PaperDirection { get; set; }
        public XtraDataReport2()
        {
            InitializeComponent();
        }

        public XtraDataReport2(DataTable dt)
        {
            InitializeComponent();
            this.DataSource = dt;
            this.xrTableCell1.DataBindings.Add("Text", dt, "name");
            GroupField gf = new GroupField("name", XRColumnSortOrder.Ascending);//设置GroupHeader分组字段 
            ((GroupHeaderBand)(this.FindControl("GroupHeader1", true))).GroupFields.Add(gf);//把分组字段添加进GroupHeader1 
            this.xrLabel2.DataBindings.Add("Text", dt, "ajh"); 
        }
    }
}
