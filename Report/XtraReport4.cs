using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;

namespace Prj_FileManageNCheckApp
{
    public partial class XtraReport4 : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraReport4()
        {
            InitializeComponent();
            while (k < 5)
            {
                PrintDocument();
            }
        }

        public XtraReport4(DataTable dt)
        {
            //InitializeComponent();
            //this.DataSource = dt;
            //this.xrTableCell1.DataBindings.Add("Text", dt, "name");
            //GroupField gf = new GroupField("name", XRColumnSortOrder.Ascending);//设置GroupHeader分组字段 
            //((GroupHeaderBand)(this.FindControl("GroupHeader1", true))).GroupFields.Add(gf);//把分组字段添加进GroupHeader1 
            //this.xrLabel1.DataBindings.Add("Text", dt, "cnt"); 
        }

        int k = 0;

        public void PrintDocument()
        {
            int EveryTabeNum = 2;
            int TabPortaitSpan = 25;

            XRTable xt = new XRTable();
            xt.CanGrow = true;
            xt.Borders = DevExpress.XtraPrinting.BorderSide.All;
            xt.BorderWidth = 0.5f;
            xt.BeginInit();
            XRTableRow xrow_header = new XRTableRow();
            XRTableCell xc_header = new XRTableCell();
            xc_header.Text = "DevExpress Page Break Usage -- Table" + (k + 1).ToString();
            xc_header.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            xrow_header.Cells.Add(xc_header);
            xt.Rows.Add(xrow_header);
            for (int i = 0; i < 6; i++)
            {
                XRTableRow xrow = new XRTableRow();
                xrow.CanGrow = true;
                xrow.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                for (int j = 0; j < 3; j++)
                {
                    XRTableCell xc = new XRTableCell();
                    xc.HeightF = 25f;
                    if (i % 2 == 0)
                    {
                        if (i / 10 >= 1)
                        {
                            xc.Text = "ABCD";
                        }
                        else
                        {
                            xc.Text = DateTime.Now.ToShortDateString();
                        }
                    }
                    else
                    {
                        xc.CanGrow = true;
                        xc.Text = DateTime.Now.ToLocalTime().ToShortTimeString();
                    }
                    xrow.Cells.Add(xc);
                }
                if (i % 2 == 0)
                {
                    xrow.BackColor = Color.LightGray;
                }
                xt.Rows.Add(xrow);
            }
            xt.AdjustSize();
            xt.EndInit();
            xt.Font = new System.Drawing.Font(Font.FontFamily, 8f, FontStyle.Regular);
            //Setting the table position.
            xt.LocationF = new PointF(0, TabPortaitSpan * k + xt.HeightF * k);
            Detail.Controls.Add(xt);
            xt.WidthF = this.PageWidth - this.Margins.Left - this.Margins.Right - 50;

            //Add page break for every EveryTabeNum tables.
            if ((k + 1) % EveryTabeNum == 0)
            {
                XRPageBreak pb = new XRPageBreak();
                pb.LocationF = new PointF(0, TabPortaitSpan * k + xt.HeightF * (k + 1));
                Detail.Controls.Add(pb);
            }
            k++;
        }
    }
}
