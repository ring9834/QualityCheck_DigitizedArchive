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
using System.Configuration;
using DotNet.DbUtilities;
using System.Data.Common;

namespace Prj_FileManageNCheckApp
{
    public delegate void ShowLinkButtonsInGridViewColumns();
    public partial class XtraPagedNavigation : DevExpress.XtraEditors.XtraUserControl
    {
        public XtraPagedNavigation()
        {
            InitializeComponent();
            if (this.InvisibleFields == null)
                this.InvisibleFields = new List<string>();
        }

        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public string TableString { get; set; }
        public string FieldString { get; set; }
        public string WhereString { get; set; }
        public List<string> WhereFieldArray { get; set; }
        public List<string> WhereFieldValueArray { get; set; }
        public string SortString { get; set; }
        public DevExpress.XtraGrid.Views.Grid.GridView PagedGridView { get; set; }
        public ShowLinkButtonsInGridViewColumns[] PagedEventHandler { get; set; }
        public string ExportAlltoFileSqlString { get; set; }
        private DataTable DT { get; set; }
        public List<string> InvisibleFields { get; set; }//设置一个在GRIDVIEW中不显示的字段，可以多个。

        private DataTable BindPagedData(string tableStr, string fieldStr, string whereStr, string sortStr)
        {
            int pageCount = 0;
            int recordCount = 0;
            DataTable dt = null;
            if (WhereFieldArray != null && WhereFieldArray.Count > 0)
                dt = PagerUtils.GetPagedDataTable(tableStr, fieldStr, whereStr, sortStr, this.WhereFieldArray, this.WhereFieldValueArray, this.PageIndex, this.PageSize, ref pageCount, ref recordCount);
            else
                dt = PagerUtils.GetPagedDataTable(tableStr, fieldStr, whereStr, sortStr, this.PageIndex, this.PageSize, ref pageCount, ref recordCount);
            label_info.Text = " " + this.PageSize + "条/页 ; 当前页： " + this.PageIndex.ToString() + " ; 总页数：" + pageCount.ToString() + " ; 共 " + recordCount.ToString() + " 条记录";
            this.PageCount = pageCount;
            if (this.PageIndex == pageCount)
            {
                followingPageBt.Enabled = false;
                lastPageBt.Enabled = false;
            }
            else
            {
                followingPageBt.Enabled = true;
                lastPageBt.Enabled = true;
            }
            if (this.PageIndex == 1)
            {
                prePageBt.Enabled = false;
                firstPageBt.Enabled = false;
            }
            else
            {
                prePageBt.Enabled = true;
                firstPageBt.Enabled = true;
            }
            return dt;
        }

        public void LoadDataToGridView()
        {
            this.PageIndex = 1;
            this.PageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            this.DT = BindPagedData(this.TableString, this.FieldString, this.WhereString, this.SortString);
            PagedGridView.GridControl.DataSource = this.DT;
            PagedGridView.PopulateColumns();
            PagedGridView.BestFitColumns();
            if (this.PagedEventHandler != null)
            {
                for (int i = 0; i < PagedEventHandler.Length; i++)
                {
                    PagedEventHandler[i]();
                }
            }
            SetColumnVisible();
        }

        public void RefreshDataToGridView()
        {
            this.DT = BindPagedData(this.TableString, this.FieldString, this.WhereString, this.SortString);
            PagedGridView.GridControl.DataSource = this.DT;
            PagedGridView.PopulateColumns();
            PagedGridView.BestFitColumns();
            if (this.PagedEventHandler != null)
            {
                for (int i = 0; i < PagedEventHandler.Length; i++)
                {
                    PagedEventHandler[i]();
                }
            }
            SetColumnVisible();
        }

        void SetColumnVisible()
        {
            if (this.InvisibleFields != null)
            {
                for (int i = 0; i < this.InvisibleFields.Count; i++)
                {
                    if (PagedGridView.Columns.Contains(PagedGridView.Columns[this.InvisibleFields[i]]))
                        PagedGridView.Columns[this.InvisibleFields[i]].Visible = false;
                }
            }
        }

        public void FreshDataInGridView()
        {
            this.PageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            this.DT = BindPagedData(this.TableString, this.FieldString, this.WhereString, this.SortString);
            PagedGridView.GridControl.DataSource = this.DT;
            PagedGridView.PopulateColumns();
            PagedGridView.BestFitColumns();
            if (this.PagedEventHandler != null)
            {
                for (int i = 0; i < PagedEventHandler.Length; i++)
                {
                    PagedEventHandler[i]();
                }
            }
            SetColumnVisible();
        }

        private void firstPageBt_Click(object sender, EventArgs e)
        {
            this.PageIndex = 1;
            this.PageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            this.DT = BindPagedData(this.TableString, this.FieldString, this.WhereString, this.SortString);
            PagedGridView.GridControl.DataSource = this.DT;
            PagedGridView.PopulateColumns();
            PagedGridView.BestFitColumns();
            if (this.PagedEventHandler != null)
            {
                for (int i = 0; i < PagedEventHandler.Length; i++)
                {
                    PagedEventHandler[i]();
                }
            }
            SetColumnVisible();
        }

        private void prePageBt_Click(object sender, EventArgs e)
        {
            this.PageIndex = this.PageIndex - 1;
            this.PageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            this.DT = BindPagedData(this.TableString, this.FieldString, this.WhereString, this.SortString);
            PagedGridView.GridControl.DataSource = this.DT;
            PagedGridView.PopulateColumns();
            PagedGridView.BestFitColumns();
            if (this.PagedEventHandler != null)
            {
                for (int i = 0; i < PagedEventHandler.Length; i++)
                {
                    PagedEventHandler[i]();
                }
            }
            SetColumnVisible();
        }

        private void followingPageBt_Click(object sender, EventArgs e)
        {
            this.PageIndex = this.PageIndex + 1;
            this.PageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            this.DT = BindPagedData(this.TableString, this.FieldString, this.WhereString, this.SortString);
            PagedGridView.GridControl.DataSource = this.DT;
            PagedGridView.PopulateColumns();
            PagedGridView.BestFitColumns();
            if (this.PagedEventHandler != null)
            {
                for (int i = 0; i < PagedEventHandler.Length; i++)
                {
                    PagedEventHandler[i]();
                }
            }
            SetColumnVisible();
        }

        private void lastPageBt_Click(object sender, EventArgs e)
        {
            this.PageIndex = this.PageCount;
            this.PageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            this.DT = BindPagedData(this.TableString, this.FieldString, this.WhereString, this.SortString);
            PagedGridView.GridControl.DataSource = this.DT;
            PagedGridView.PopulateColumns();
            PagedGridView.BestFitColumns();
            if (this.PagedEventHandler != null)
            {
                for (int i = 0; i < PagedEventHandler.Length; i++)
                {
                    PagedEventHandler[i]();
                }
            }
            SetColumnVisible();
        }

        private void txEdit_Jump_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(txEdit_Jump.Text))
                {
                    int jumpto = int.Parse(txEdit_Jump.Text);
                    if (jumpto <= 0)
                    {
                        txEdit_Jump.Text = "1";
                    }
                    if (jumpto > this.PageCount)
                    {
                        txEdit_Jump.Text = this.PageCount.ToString();
                    }
                }
                this.PageIndex = int.Parse(txEdit_Jump.Text);
                this.PageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
                this.DT = BindPagedData(this.TableString, this.FieldString, this.WhereString, this.SortString);
                PagedGridView.GridControl.DataSource = this.DT;
                PagedGridView.PopulateColumns();
                PagedGridView.BestFitColumns();
                txEdit_Jump.Text = string.Empty;
                if (this.PagedEventHandler != null)
                {
                    for (int i = 0; i < PagedEventHandler.Length; i++)
                    {
                        PagedEventHandler[i]();
                    }
                }
                SetColumnVisible();
            }
        }

        private void simpleButton_export_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Excel文件(*.xls)|*.xls|Excel文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
                DataTable dt = PagedGridView.GridControl.DataSource as DataTable;
                ExcelHelper.GetExcelByDataTable(dt, fileName);
            }
        }

        private void simpleButton_exportall_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Excel文件(*.xls)|*.xls|Excel文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DbHelper helper = new DbHelper();
                DbParameter[] param = new DbParameter[WhereFieldArray.Count];
                for (int i = 0; i < WhereFieldArray.Count; i++)
                {
                    param[i] = helper.MakeInParam(WhereFieldArray[i], WhereFieldValueArray[i]);
                }
                string sql = ExportAlltoFileSqlString;
                DataTable dt = helper.Fill(sql, param);
                string fileName = saveFileDialog1.FileName;
                ExcelHelper.GetExcelByDataTable(dt, fileName);
                dt.Dispose();
            }
        }

        private void txEdit_Jump_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键  
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字  
                {
                    e.Handled = true;
                }
            }
        }
    }
}
