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
using System.Configuration;
using System.IO;
using System.Runtime.Remoting.Messaging;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using Prj_FileManageNCheckApp.Properties;

namespace Prj_FileManageNCheckApp
{
    public partial class FileConnectWithCataloge : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        public string ContentFormat { get; set; }//原文格式
        public UserEntity UserLoggedIn { get; set; }

        public FileConnectWithCataloge()
        {
            InitializeComponent();
        }

        public FileConnectWithCataloge(string fileCodeName, string contentFormat, UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.ContentFormat = contentFormat;
            this.UserLoggedIn = user;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            this.ContentFormat = paras.ContentFormat;
            labelControl1.Text = "此档案库支持挂接的原文格式：" + this.ContentFormat;
            textBox_content_browser.Enabled = string.IsNullOrEmpty(this.ContentFormat) ? false : true;
            PageControlLocation.MakeControlCenter(panelControl1.Parent, panelControl1);
            imageList1.Images.Add("img1", Resources.doing);
            imageList1.Images.Add("img2", Resources.finish);
            imageList1.Images.Add("img3", Resources.haveerror);
            imageList1.TransparentColor = Color.Transparent;
        }

        private void button_content_next_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ContentFormat))
            {
                MessageBox.Show("此档案类型库没有原文，不能挂接！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(textBox_content_browser.Text))
            {
                MessageBox.Show("请选择要挂接的原文所在的文件夹！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string contentFileRootPath = ConfigurationManager.AppSettings["ContentFileRootPath"];
            if (MessageBox.Show("当前的原文根路径设置在“" + contentFileRootPath + "”，是否继续？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            //创建新任务记录
            string datafrom = folderBrowserDialog1.SelectedPath.Substring(folderBrowserDialog1.SelectedPath.LastIndexOf(@"\") + 1);
            ContentFileDAO cfd = new ContentFileDAO();
            DataTable uniquecodeDt = cfd.AddArchiveLinkRecord(this.FileCodeName, datafrom);
            object uniquecode = uniquecodeDt.Rows[0][0].ToString();
            uniquecodeDt.Dispose();

            //动态创建临时表，此临时表用于批量挂接
            string tempTableName = this.FileCodeName + "_temp_" + uniquecode.ToString();
            cfd.CreateTempTableForArchiveLink(this.ContentFormat, tempTableName);

            //显示挂接任务记录
            ShowGjPagedRecords();
            xtraTab_content_acceptRecord.Show();

            //挂接
            int total = 0;
            string uniquenumber = uniquecode.ToString();
            if (this.ContentFormat.ToLower().Contains("pdf"))
            {
                GJTaskDelegate gjTask = new GJTaskDelegate(ConnectCatalogAndContentFile);
                gjTask.BeginInvoke(tempTableName, ref total, ref uniquenumber, new AsyncCallback(LinkContentCallBack), null);
                //success = gjTask.EndInvoke(ref total, result);//会阻塞线程,所以尽量不要在这里用，而是再BeginInvoke中使用 new AsyncCallback
            }
            else
            {
                GJTaskDelegate gjTask = new GJTaskDelegate(ConnectCatalogAndImageContentFile);
                gjTask.BeginInvoke(tempTableName, ref total, ref uniquenumber, new AsyncCallback(LinkContentCallBack), null);
            }
        }

        private delegate int GJTaskDelegate(string tempTableName, ref int total, ref string uniquenumber);

        private void ShowGjPagedRecords()
        {
            xtraPagedNavigation1.TableString = "t_archive_link ";
            xtraPagedNavigation1.FieldString = "Unique_code AS '批次号',data_from AS '数据来源',start_time AS '挂接开始时间',end_time AS '挂接结束时间',link_status AS '挂接状态',total_count AS '原文文件数（个）',success_count AS '成功挂接文件数（个）'";
            xtraPagedNavigation1.WhereString = "code_name='" + this.FileCodeName + "' ";
            xtraPagedNavigation1.SortString = "Unique_code DESC ";
            xtraPagedNavigation1.PagedGridView = gridView_content_acceptRecord;
            xtraPagedNavigation1.PagedEventHandler = new ShowLinkButtonsInGridViewColumns[1];//委托数组
            xtraPagedNavigation1.PagedEventHandler[0] = ShowGJStatusInColumn;
            xtraPagedNavigation1.LoadDataToGridView();
            xtraPagedNavigation1.ExportAlltoFileSqlString = "SELECT Unique_code AS '批次号',data_from AS '数据来源',start_time AS '挂接开始时间',end_time AS '挂接结束时间',link_status AS '挂接状态',total_count AS '原文文件数（个）',success_count AS '成功挂接文件数（个）' FROM t_archive_link WHERE code_name='" + this.FileCodeName + "' ORDER BY Unique_code DESC ";
        }

        private int ConnectCatalogAndContentFile(string tempTableName, ref int totalCount, ref string uniquenumber)
        {
            //执行挂接1：在数据库中创建临时表
            string contentFileRootPath = ConfigurationManager.AppSettings["ContentFileRootPath"];
            //if (this.TempDataTables == null)
            //    this.TempDataTables = new Dictionary<string, DataTable>();
            //bool boolOjb = this.TempDataTables.Select(t => t.Key.Equals(tempTableName)).First();
            //if (!boolOjb)
            //    this.TempDataTables.Remove(tempTableName);

            ArchiveNoDAO and = new ArchiveNoDAO();
            object archiveNoFieldName = and.GetArchiveNoDataByCodeName2(this.FileCodeName);
            if (archiveNoFieldName == null)
            {
                MessageBox.Show("请为此类型库配置恰当的档号字段，再实施原文挂接！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0;
            }

            DataTable dtTemp = new DataTable(tempTableName);
            dtTemp.Columns.Add("DH", typeof(string));
            dtTemp.Columns.Add("FilePath", typeof(string));
            //this.TempDataTables.Add(tempTableName, dtTemp);
            ExtractArchiveNosToDataTable(folderBrowserDialog1.SelectedPath, dtTemp);//提取临时表
            new SqlHelper().SqlBulkCopyData(dtTemp);//效率非常高的批量插入数据表
            totalCount = dtTemp.Rows.Count;
            string amedialparam = uniquenumber;//只是为了传递到EndInvode中用，取巧
            uniquenumber = amedialparam;

            //执行挂接2：挂接            
            //sql = "DECLARE @yw nvarchar(800),@dh nvarchar(50),@dh2 nvarchar(50) \r\n";
            //sql += "DECLARE @success int \r\n";
            //sql += "SET @success=0 \r\n";
            //sql += "DECLARE mycursor CURSOR FOR \r\n";
            //sql += "(SELECT DH,FilePath FROM " + tempTableName + ") \r\n";
            //sql += "OPEN mycursor \r\n";
            //sql += "FETCH NEXT FROM mycursor INTO @dh,@yw \r\n";
            //sql += "WHILE @@FETCH_STATUS = 0 \r\n";
            //sql += "BEGIN \r\n";
            //sql += "SELECT @dh2=(SELECT " + archiveNoFieldName + " FROM " + this.FileCodeName + " WHERE " + archiveNoFieldName + "=@dh) \r\n";
            //sql += "IF @dh2 IS NOT NULL \r\n";
            //sql += "    BEGIN \r\n";
            //sql += "    UPDATE " + this.FileCodeName + " SET " + contentFileFieldName + "= @yw WHERE " + archiveNoFieldName + "=@dh \r\n";
            //sql += "    SET @success = @success + 1 \r\n";
            //sql += "    END \r\n";
            //sql += "FETCH NEXT FROM mycursor INTO @dh,@yw \r\n";
            //sql += "END \r\n";
            //sql += "DEALLOCATE mycursor \r\n";
            //sql += "DROP TABLE " + tempTableName + " \r\n";
            //sql += "SELECT @success";

            ContentFileDAO cfd = new ContentFileDAO();
            DataTable successcountDt = cfd.AddYWPath(this.FileCodeName, tempTableName, archiveNoFieldName.ToString());
            int successcount = int.Parse(successcountDt.Rows[0][0].ToString());
            successcountDt.Dispose();
            //object successcount = new DbHelper().ExecuteScalar(sql);
            return successcount;
        }

        private int ConnectCatalogAndImageContentFile(string tempTableName, ref int totalCount, ref string uniquenumber)
        {
            //执行挂接1：在数据库中创建临时表
            string contentFileRootPath = ConfigurationManager.AppSettings["ContentFileRootPath"];
            ArchiveNoDAO and = new ArchiveNoDAO();
            DataTable dt = and.GetArchiveNoDataByCodeName(this.FileCodeName);

            if (dt.Rows.Count == 0 || dt.Rows[0]["archive_num_field"] == DBNull.Value || string.IsNullOrEmpty(dt.Rows[0]["archive_num_field"].ToString()))
            {
                MessageBox.Show("请为此类型库配置恰当的档号字段，再实施原文挂接！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0;
            }
            string archiveNoFieldName = dt.Rows[0]["archive_num_field"].ToString();
            int archiveNoComposedPart = int.Parse(dt.Rows[0]["archive_num_parts_amount"].ToString());
            string splitChar = dt.Rows[0]["connect_char"].ToString();
            dt.Dispose();

            DataTable dtTemp = new DataTable(tempTableName);
            dtTemp.Columns.Add("DH", typeof(string));
            dtTemp.Columns.Add("FilePath", typeof(string));
            ExtractArchiveNosFromFileNameToDataTable(folderBrowserDialog1.SelectedPath, dtTemp, archiveNoComposedPart, splitChar);//提取临时表
            new SqlHelper().SqlBulkCopyData(dtTemp);//效率非常高的批量插入数据表
            totalCount = dtTemp.Rows.Count;
            string amedialparam = uniquenumber;//只是为了传递到EndInvode中用，取巧
            uniquenumber = amedialparam;

            //执行挂接2：挂接         
            ContentFileDAO cfd = new ContentFileDAO();
            DataTable successcountDt = cfd.AddImageYWPath(this.FileCodeName, tempTableName, archiveNoFieldName);
            int successcount = int.Parse(successcountDt.Rows[0][0].ToString());
            successcountDt.Dispose();
            return successcount;
        }

        void LinkContentCallBack(IAsyncResult result)
        {
            GJTaskDelegate gdk = (GJTaskDelegate)((AsyncResult)result).AsyncDelegate;
            int success = 0, total = 0;
            string uniquecode = string.Empty;
            success = gdk.EndInvoke(ref total, ref uniquecode, result);
            //挂接完后处理
            string endtime = DateTime.Now.ToString();
            ContentFileDAO cfd = new ContentFileDAO();
            cfd.UpdateArchiveLinkRecord(total, success, endtime, uniquecode);

            //告诉Timer更新gridview
            ConfigurationManager.AppSettings["LinkContentUpdated"] = "true";
        }

        private void ExtractArchiveNosToDataTable(string folderPath, DataTable dt)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);
            FileInfo[] fiA = di.GetFiles("*.pdf");
            for (int j = 0; j < fiA.Length; j++)
            {
                string name2 = fiA[j].Name;
                string dh = name2.Substring(0, name2.LastIndexOf(".pdf"));
                string contentFileRootPath = ConfigurationManager.AppSettings["ContentFileRootPath"];
                string pdfPathAll = fiA[j].FullName;
                string pdfPath = pdfPathAll.Substring(contentFileRootPath.Length);

                DataRow r = dt.NewRow();
                r.SetField<string>("DH", dh);
                r.SetField<string>("FilePath", pdfPath);
                dt.Rows.Add(r);

                //textBox2.Text += "正在从文件提取档号：【档号】" + dh + " ---- 时间：" + DateTime.Now.ToString() + "\r\n";
                //textBox2.SelectionStart = textBox1.Text.Length;
                //textBox2.ScrollToCaret();
                //Application.DoEvents();
            }

            DirectoryInfo[] diA = di.GetDirectories();
            for (int i = 0; i < diA.Length; i++)
            {
                ExtractArchiveNosToDataTable(diA[i].FullName, dt);
            }
        }

        /// <summary>
        /// 把图片所在路径存到XML中
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="dt"></param>
        /// <param name="archiveNoComposedPart"></param>
        /// <param name="splitChar"></param>
        private void ExtractArchiveNosFromFileNameToDataTable(string folderPath, DataTable dt, int archiveNoComposedPart, string splitChar)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);
            string[] patterns = this.ContentFormat.Split(';');
            FileInfo[] tempFileInfo;
            FileInfo[] fiA = null;
            for (int i = 0; i < patterns.Length; i++)
            {
                tempFileInfo = di.GetFiles(patterns[i]);
                if (i == 0)
                    fiA = tempFileInfo;
                else
                    fiA = (fiA.Concat((FileInfo[])tempFileInfo.Clone())).ToArray<FileInfo>();
            }

            if (fiA.Length > 0)
            {
                string folderName = fiA[0].Directory.Name;
                int num = folderName.Split(splitChar.ToCharArray()).Length;
                if (num == archiveNoComposedPart)
                {
                    DataRow r = dt.NewRow();
                    r.SetField<string>("DH", folderName);
                    string contentFileRootPath = ConfigurationManager.AppSettings["ContentFileRootPath"];
                    string xml = "<YW>";
                    for (int i = 0; i < fiA.Length; i++)
                    {
                        string fullName = fiA[i].FullName;
                        string relativeName = fullName.Substring(contentFileRootPath.Length);
                        xml += "<ywPath>" + relativeName + "</ywPath>";
                    }
                    xml += "</YW>";
                    r.SetField<string>("FilePath", xml);
                    dt.Rows.Add(r);
                }
            }

            DirectoryInfo[] diA = di.GetDirectories();
            for (int i = 0; i < diA.Length; i++)
            {
                ExtractArchiveNosFromFileNameToDataTable(diA[i].FullName, dt, archiveNoComposedPart, splitChar);
            }
        }

        private void ShowGJStatusInColumn()
        {
            DataTable dt = gridControl_content_acceptRecord.DataSource as DataTable;
            dt.Columns.Add("gjStatus", typeof(ImageComboBoxItem));
            GridColumn Col2 = new GridColumn();
            Col2.FieldName = "gjStatus";
            Col2.Caption = "挂接状态显示";
            Col2.VisibleIndex = gridView_content_acceptRecord.Columns.Count;
            Col2.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            //Col2.ColumnEdit = repositoryItemImageComboBox1;
            gridView_content_acceptRecord.Columns.Add(Col2);
            gridView_content_acceptRecord.Columns["挂接状态"].Visible = false;
            foreach (GridColumn c in gridView_content_acceptRecord.Columns)
            {
                c.OptionsColumn.AllowEdit = false;
            }
        }

        private void gridView_content_acceptRecord_CustomRowCellEdit_1(object sender, CustomRowCellEditEventArgs e)
        {
            //研究了好久才研究出来的方法
            if (e.Column.FieldName == "gjStatus" && e.RowHandle >= 0)
            {
                foreach (ImageComboBoxItem item in this.repositoryItemImageComboBox1.Items)
                {
                    string statusStr = gridView_content_acceptRecord.GetRowCellValue(e.RowHandle, "挂接状态").ToString();
                    int status = int.Parse(statusStr);
                    if (((GJStatus)status).ToString().Equals(item.Value.ToString()))
                    {
                        gridView_content_acceptRecord.SetRowCellValue(e.RowHandle, gridView_content_acceptRecord.Columns["gjStatus"], item);
                    }
                }
            }
        }

        private void simpleButton_BeginGJ_Click(object sender, EventArgs e)
        {
            xtraTab_content_browser.Show();
        }

        private void simpleButton_showContentAccepted_Click(object sender, EventArgs e)
        {
            xtraTab_content_acceptRecord.Show();
            ShowGjPagedRecords();
        }

        private void FileConnectWithCataloge_Load(object sender, EventArgs e)
        {
            xtraTabControl_linkContent.ShowTabHeader = DefaultBoolean.False;
            xtraDeleteButtonForGrid1.Parent = standaloneBarDockControl2;
            xtraDeleteButtonForGrid1.TargetGrid = gridView_content_acceptRecord;
            xtraDeleteButtonForGrid1.IsOneRowSelectedFunc = new IsOneRowSelected(IsOneRowSelectedInGridFunc);
            xtraDeleteButtonForGrid1.RefreshDataInGridViewFunc = new RefreshDataInGridView(ShowGjPagedRecords);
            VerifyPageAccess.VerifyButtonsAccessOnPage(this, this.UserLoggedIn.RoleId);//按钮权限判断
        }

        private bool IsOneRowSelectedInGridFunc(GridView grid)
        {
            if (grid.SelectedRowsCount == 0)
            {
                MessageBox.Show("请选择要删除的记录行!", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            int rowhandle = grid.FocusedRowHandle;
            DataRow dr = grid.GetDataRow(rowhandle);
            string sql = "DELETE FROM t_archive_link WHERE Unique_code=" + dr["批次号"].ToString() + "\r\n";
            xtraDeleteButtonForGrid1.SQL = sql;
            xtraDeleteButtonForGrid1.AlertInformation = "本条挂接记录（不包含挂接的原文）将被删除，确认吗？";
            return true;
        }

        private void timer_linkcontent_Tick(object sender, EventArgs e)
        {
            bool updated = Boolean.Parse(ConfigurationManager.AppSettings["LinkContentUpdated"]);
            if (updated)
            {
                ShowGjPagedRecords();
                ConfigurationManager.AppSettings["LinkContentUpdated"] = "False";
            }
        }

        private void xtraTabControl_linkContent_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            xtraDeleteButtonForGrid1.EnableDeleteButton = xtraTabControl_linkContent.SelectedTabPageIndex == 1 ? true : false;
        }

        private void textBox_content_browser_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_content_browser.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void gridView_content_acceptRecord_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName.Equals("gjStatus"))
            {
                if (e.CellValue != null)
                {
                    ImageComboBoxItem item = (ImageComboBoxItem)e.CellValue;
                    if (item.Value.ToString().Equals(GJStatus.Doing.ToString()))
                    {
                        e.Appearance.BackColor = Color.Orange;
                    }
                    if (item.Value.ToString().Equals(GJStatus.Finish.ToString()))
                    {
                        e.Appearance.BackColor = Color.LightGreen;
                    }
                    if (item.Value.ToString().Equals(GJStatus.HaveError.ToString()))
                    {
                        e.Appearance.BackColor = Color.Red;
                    }
                }
            }
        }

    }
}