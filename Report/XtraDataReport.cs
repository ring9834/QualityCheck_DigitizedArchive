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
    public partial class XtraDataReport : DevExpress.XtraReports.UI.XtraReport
    {
        private ReportParams RP { get; set; }
        public XtraDataReport()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="onlyCount">true表示只统计总数</param>
        /// <param name="paperDirection">true表示报表纸张横向</param>
        public XtraDataReport(ReportParams param)
        {
            InitializeComponent();
            this.RP = param;
            InitDetailsBasedonXRTable();
            this.ShowPreviewDialog();
        }

        public XtraDataReport(ReportParams param,string filePath)
        {
            InitializeComponent();
            this.RP = param;
            InitDetailsBasedonXRTable();
            string[] ss = filePath.Split('.');
            string format = ss[ss.Length - 1];
            if (format.ToLower().Equals("pdf"))
            {
                this.ExportToPdf(filePath);
            }
            else if (format.ToLower().Equals("xls"))
            {
                this.ExportToXls(filePath);
            }
            else if (format.ToLower().Equals("xlsx"))
            {
                this.ExportToXlsx(filePath);
            }
        }

        private void InitDetailsBasedonXRTable()
        {
            string sql = string.Empty;
            DataSet ds = new DataSet();
            int q = 0;
            foreach (KeyValuePair<string, StatisticsCondition> kvp in this.RP.CollectionBoxList)
            {
                string codeName = kvp.Key;
                string where = "1=1 ";
                string fieldStr = string.Empty;
                string groupStr = string.Empty;
                List<object> paraList = new List<object>();
                int ordial = 0;

                List<ReportSearchConditionEntity> scList = kvp.Value.ReportConditionList;
                for (int j = 0; j < scList.Count; j++)
                {
                    ReportSearchConditionEntity rce = scList[j];
                    if (rce.SearchCondition.ToLower().ToLower().Contains("like"))//基本检索1
                        where += " AND " + rce.FieldName + " " + rce.SearchCondition + " '%'+" + "@param" + ordial + "+'%' ";
                    else
                        where += " AND " + rce.FieldName + " " + rce.SearchCondition + "@param" + ordial;
                    paraList.Add(rce.Value);
                    ordial++;
                }

                List<string> fieldList = kvp.Value.StatisticsFieldsList;
                for (int m = 0; m < fieldList.Count; m++)
                {
                    if (m == 0)
                        fieldStr = fieldList[m];
                    else
                        fieldStr += "+'-'+" + fieldList[m];
                }

                List<string> groupFieldList = kvp.Value.GroupFieldsList;
                for (int m = 0; m < groupFieldList.Count; m++)
                {
                    if (m == 0)
                        groupStr = groupFieldList[m];
                    else
                        groupStr += "+'-'+" + groupFieldList[m];
                }

                if (this.RP.OnlyCount)
                    sql = "SELECT COUNT(DISTINCT " + fieldStr + ") AS ct," + groupStr + " AS grp FROM " + codeName + " WHERE " + where + " GROUP BY " + groupStr + " ORDER BY ct ASC";
                else
                    sql = "SELECT DISTINCT " + fieldStr + " AS ct," + groupStr + " AS grp FROM " + codeName + " WHERE " + where + " ORDER BY ct ASC";
                DbParameter[] param = new DbParameter[paraList.Count];
                DbHelper helper = new DbHelper();
                for (int k = 0; k < paraList.Count; k++)
                    param[k] = helper.MakeInParam("param" + k, paraList[k]);
                DataTable dt = new DbHelper().Fill(sql, param);
                dt.TableName = "table_" + q;
                ds.Tables.Add(dt);
                q++;
            }

            //this.DataSource = ds;
            this.Landscape = RP.LandScape;//纸张方向
            this.Detail.HeightF = 0.1F;
            int columnCount = int.Parse(RP.ColumnCount);
            string title = RP.ReportTitle;
            float titleFont = float.Parse(RP.TitleFontSize);
            float contentFont = float.Parse(RP.ContentFontSize);

            PageHeaderBand phb = new PageHeaderBand();
            phb.HeightF = 30;

            XRLabel lb_Title = new XRLabel();
            lb_Title.WidthF = this.PageWidth - this.Margins.Left - this.Margins.Right;
            lb_Title.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            lb_Title.Font = new System.Drawing.Font("宋体", titleFont, System.Drawing.FontStyle.Bold);
            lb_Title.Text = title;
            phb.Controls.Add(lb_Title);
            this.Bands.Add(phb);

            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DetailReportBand drb = new DetailReportBand();
                drb.DataSource = ds;
                drb.DataMember = ds.Tables[i].TableName;
                GroupHeaderBand ghb = new GroupHeaderBand();
                ghb.HeightF = 12;

                GroupField gf = new GroupField("grp", XRColumnSortOrder.Ascending);//设置GroupHeader分组字段 
                XRLabel lb_group = new XRLabel();
                lb_group.WidthF = this.PageWidth - this.Margins.Left - this.Margins.Right;
                lb_group.BackColor = Color.LightGray;
                lb_group.DataBindings.Add("Text", ds.Tables[i], "grp");
                lb_group.Font = new System.Drawing.Font("宋体", contentFont, System.Drawing.FontStyle.Bold);
                lb_group.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                ghb.Controls.Add(lb_group);
                ghb.GroupFields.Add(gf);
                drb.Bands.Add(ghb);

                DetailBand db = new DetailBand();
                db.Name = "detail_" + i;

                XRLabel lb_content = new XRLabel();
                if (this.RP.ShowBorder)
                {
                    lb_content.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top;
                }
                lb_content.DataBindings.Add("Text", ds.Tables[i], "ct");
                lb_content.WidthF = (this.PageWidth - (this.Margins.Left + this.Margins.Right)) / columnCount;
                lb_content.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                db.HeightF = lb_content.HeightF;
                db.Controls.Add(lb_content);
                db.MultiColumn.Mode = MultiColumnMode.UseColumnCount;
                db.MultiColumn.Layout = DevExpress.XtraPrinting.ColumnLayout.AcrossThenDown;
                db.MultiColumn.ColumnCount = columnCount;
                drb.Bands.Add(db);

                this.Bands.Add(drb);
            }

            if (this.RP.ShowDateTime || this.RP.ShowPageNumber)
            {
                PageFooterBand pfb = new PageFooterBand();
                pfb.HeightF = 30F;
                XRTable tbFooter = new XRTable();
                tbFooter.WidthF = this.PageWidth - (this.Margins.Left + this.Margins.Right);
                XRTableRow tbRowFooter = new XRTableRow();
                XRTableCell tbCellFooter1 = new XRTableCell();
                XRTableCell tbCellFooter2 = new XRTableCell();
                XRTableCell tbCellFooter3 = new XRTableCell();
                tbCellFooter1.WidthF = tbFooter.WidthF / 3;
                tbCellFooter2.WidthF = tbFooter.WidthF / 3;
                tbCellFooter3.WidthF = tbFooter.WidthF / 3;
                tbRowFooter.Cells.AddRange(new XRTableCell[] { tbCellFooter1, tbCellFooter2, tbCellFooter3 });
                if (this.RP.ShowPageNumber)
                {
                    XRPageInfo xrPage1 = new XRPageInfo();
                    xrPage1.Font = new System.Drawing.Font("宋体", 8, System.Drawing.FontStyle.Bold);
                    xrPage1.PageInfo = DevExpress.XtraPrinting.PageInfo.NumberOfTotal;
                    tbCellFooter2.Controls.Add(xrPage1);
                    tbCellFooter2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                }
                if (this.RP.ShowDateTime)
                {
                    XRPageInfo xrPage2 = new XRPageInfo();
                    xrPage2.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
                    xrPage2.Font = new System.Drawing.Font("宋体", 8, System.Drawing.FontStyle.Bold);
                    //xrPage2.Format = "yyyy-MM-dd";
                    tbCellFooter3.Controls.Add(xrPage2);
                    tbCellFooter3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
                }

                tbFooter.Rows.Add(tbRowFooter);
                pfb.Controls.Add(tbFooter);
                this.Bands.Add(pfb);
            }
        }
    }
}
