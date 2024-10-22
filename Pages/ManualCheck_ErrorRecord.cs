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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DotNet.DbUtilities;
using System.Xml;

namespace Prj_FileManageNCheckApp
{
    public partial class ManualCheck_ErrorRecord : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        public UserEntity UserLoggedIn { get; set; }
        public TableLayoutPanel TableLayoutPanel1;
        private string SearchWhere { get; set; }
        public ManualCheck_ErrorRecord()
        {
            InitializeComponent();
        }

        public ManualCheck_ErrorRecord(string fileCodeName, UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            LoadData();
            LoadSearchControls();
        }

        public void RefreshDatas()
        {
            LoadData();
        }

        void LoadData()
        {
            xtraPagedNavigation_manualCheck_resultList.TableString = "t_archive_manual_check_rec";
            xtraPagedNavigation_manualCheck_resultList.FieldString = "Unique_code,dh as '档号',error_type AS '问题类型',description AS '问题描述',user_id AS 检查人,record_time AS '记录时间',correct_user_id AS '修正人',corrected_time AS '修正时间',is_corrected";
            xtraPagedNavigation_manualCheck_resultList.WhereString = "code_name='" + this.FileCodeName + "'";
            xtraPagedNavigation_manualCheck_resultList.SortString = "Unique_code DESC ";
            xtraPagedNavigation_manualCheck_resultList.PagedGridView = gridView_manualCheck_resultList;
            xtraPagedNavigation_manualCheck_resultList.PagedEventHandler = new ShowLinkButtonsInGridViewColumns[1];//委托数组
            xtraPagedNavigation_manualCheck_resultList.PagedEventHandler[0] = ShowCompleteLinkButton;
            xtraPagedNavigation_manualCheck_resultList.InvisibleFields.Add("Unique_code");
            xtraPagedNavigation_manualCheck_resultList.InvisibleFields.Add("is_corrected");
            xtraPagedNavigation_manualCheck_resultList.LoadDataToGridView();
            xtraPagedNavigation_manualCheck_resultList.ExportAlltoFileSqlString = "SELECT dh as '档号',error_type AS '问题类型',description AS '问题描述',user_id AS 检查人,record_time AS '记录时间',correct_user_id AS '修正人',corrected_time AS '修正时间' FROM t_archive_manual_check_rec WHERE code_name='" + this.FileCodeName + "'";
        }

        void ShowCompleteLinkButton()
        {
            RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();
            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.ButtonClick += riButtonEdit_ButtonClick;
            riButtonEdit.Buttons.Clear();
            EditorButton bt0 = new EditorButton();
            bt0.Kind = ButtonPredefines.Glyph;
            bt0.Caption = "标记";
            bt0.ToolTip = "标识本条记录所描述的问题已修正";
            bt0.Appearance.ForeColor = Color.Blue;
            bt0.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt0);
            GridColumn Col1 = new GridColumn();
            Col1.FieldName = "repairError";
            Col1.Caption = "标记为已修正状态";
            Col1.Visible = false;
            Col1.VisibleIndex = gridView_manualCheck_resultList.Columns.Count;
            Col1.UnboundType = DevExpress.Data.UnboundColumnType.String;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_manualCheck_resultList.RepositoryItems.Add(riButtonEdit);
            gridView_manualCheck_resultList.Columns.Add(Col1);

        }

        void riButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (MessageBox.Show("确定要标记为“已修正”状态吗？", "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                int rowhandle = gridView_manualCheck_resultList.FocusedRowHandle;
                DataRow dr = gridView_manualCheck_resultList.GetDataRow(rowhandle);
                string uniquecode = dr["Unique_code"].ToString();
                ManualCheckDAO mcd = new ManualCheckDAO();
                mcd.UpdateCorrectedStatus(this.UserLoggedIn.UserName, uniquecode);//错误修正后，更新修正者信息
                //RefreshSearchResult();
                xtraPagedNavigation_manualCheck_resultList.RefreshDataToGridView();
            }
        }

        private void ManualCheck_ErrorRecord_Load(object sender, EventArgs e)
        {
            xtraDeleteButtonForGrid1.Parent = panelControl1;
            xtraDeleteButtonForGrid1.TargetGrid = gridView_manualCheck_resultList;
            xtraDeleteButtonForGrid1.IsOneRowSelectedFunc = new IsOneRowSelected(IsOneRowSelectedInGridFunc);
            xtraDeleteButtonForGrid1.RefreshDataInGridViewFunc = new RefreshDataInGridView(RefreshDatas);
            xtraDeleteButtonForGrid1.EnableDeleteButton = true;
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
            string sql = "DELETE FROM t_archive_manual_check_rec WHERE Unique_code=" + dr["Unique_code"].ToString();
            xtraDeleteButtonForGrid1.SQL = sql;
            xtraDeleteButtonForGrid1.AlertInformation = "确认要删除吗？";
            return true;
        }

        private void gridView_manualCheck_resultList_RowStyle(object sender, RowStyleEventArgs e)
        {
            int hand = e.RowHandle;//行号
            if (hand < 0)
                return;

            DataRow dr = gridView_manualCheck_resultList.GetDataRow(hand);
            if (dr == null)
                return;

            object isCorrected = dr["is_corrected"];
            if (isCorrected.ToString().Equals("1"))
            {
                e.Appearance.BackColor = Color.LightGreen;
            }
        }

        void LoadSearchControls()
        {
            splitContainerControl1.Panel2.Controls.Clear();
            TableLayoutPanel1 = new TableLayoutPanel();
            splitContainerControl1.Panel2.Controls.Add(TableLayoutPanel1);
            TableLayoutPanel1.ColumnStyles.Clear();
            TableLayoutPanel1.RowStyles.Clear();
            TableLayoutPanel1.BackColor = Color.White;
            TableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            TableLayoutPanel1.ColumnCount = 8;
            TableLayoutPanel1.Top = 20;
            TableLayoutPanel1.Height = 10;
            ManualCheckDAO mcd = new ManualCheckDAO();
            DataTable dt = mcd.GetConfiguredFieldsForManualCheck(this.FileCodeName);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                Label lab = new Label();
                lab.Name = "Lbl_" + row["col_name"].ToString();
                lab.Text = row["show_name"].ToString();
                //lab.Tag = row["code_value"];
                lab.AutoSize = true;
                lab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));//通过Anchor 设置Label 列中居中
                TableLayoutPanel1.Controls.Add(lab);

                TextEdit txtObj = new TextEdit();
                txtObj.Name = "Txt_" + row["col_name"].ToString();
                txtObj.Tag = row["col_name"];
                txtObj.Width = 120;
                txtObj.Properties.NullValuePromptShowForEmptyValue = true;
                txtObj.Properties.NullValuePrompt = "等于";
                TableLayoutPanel1.Controls.Add(txtObj);
            }
            TableLayoutPanel1.AutoSize = true;

            PageControlLocation.MakeControlHoritionalCenter(TableLayoutPanel1.Parent, TableLayoutPanel1);
            Panel buttonPanel = new Panel();
            Button searchButton = new Button();
            searchButton.Text = "搜索";
            searchButton.Click += searchButton_Click;
            Button cancelButton = new Button();
            cancelButton.Text = "重置";
            cancelButton.Click += cancelButton_Click;
            splitContainerControl1.Panel2.Controls.Add(buttonPanel);
            buttonPanel.Top = TableLayoutPanel1.Top + TableLayoutPanel1.Height + 15;
            buttonPanel.Width = splitContainerControl1.Panel2.Width;
            buttonPanel.Controls.Add(searchButton);
            buttonPanel.Controls.Add(cancelButton);
            PageControlLocation.MakeControlHoritionalCenter(searchButton.Parent, searchButton);
            searchButton.Left = searchButton.Left - searchButton.Width / 2;
            cancelButton.Left = searchButton.Left + searchButton.Width + 15;
        }

        void searchButton_Click(object sender, EventArgs e)
        {
            string connchar = string.Empty;
            ArchiveNoDAO and = new ArchiveNoDAO();
            DataTable dt = and.GetArchiveNoDataByCodeName(this.FileCodeName);
            connchar = dt.Rows[0]["connect_char"].ToString();
            dt.Dispose();

            string valuePart = string.Empty;
            for (int i = 0; i < this.TableLayoutPanel1.Controls.Count; i++)
            {
                Type controlType = this.TableLayoutPanel1.Controls[i].GetType();
                if (controlType == typeof(TextEdit))
                {
                    TextEdit txtBox = (TextEdit)this.TableLayoutPanel1.Controls[i];
                    if (!string.IsNullOrEmpty(txtBox.Text))
                    {
                        if (string.IsNullOrEmpty(valuePart))
                            valuePart += txtBox.Text;
                        else
                            valuePart += connchar + txtBox.Text;
                    }
                }
            }

            if (valuePart.ToLower().Contains("delete") || valuePart.ToLower().Contains("update") || valuePart.ToLower().Contains("insert") || valuePart.ToLower().Contains("create") || valuePart.ToLower().Contains("drop"))
                return;//安全起见

            string where = "CHARINDEX('" + valuePart + "',dh)>0";
            if (string.IsNullOrEmpty(valuePart))
                where = " 1=1 ";
            this.SearchWhere = where;
            RefreshSearchResult();
        }

        void RefreshSearchResult()
        {
            xtraPagedNavigation_manualCheck_resultList.TableString = "t_archive_manual_check_rec";
            xtraPagedNavigation_manualCheck_resultList.FieldString = "Unique_code,dh as '档号',error_type AS '问题类型',description AS '问题描述',user_id AS 检查人,record_time AS '记录时间',correct_user_id AS '修正人',corrected_time AS '修正时间',is_corrected";
            xtraPagedNavigation_manualCheck_resultList.WhereString = SearchWhere;
            xtraPagedNavigation_manualCheck_resultList.SortString = "Unique_code ASC ";
            xtraPagedNavigation_manualCheck_resultList.PagedGridView = gridView_manualCheck_resultList;
            xtraPagedNavigation_manualCheck_resultList.PagedEventHandler = new ShowLinkButtonsInGridViewColumns[1];//委托数组
            xtraPagedNavigation_manualCheck_resultList.PagedEventHandler[0] = ShowCompleteLinkButton;
            xtraPagedNavigation_manualCheck_resultList.InvisibleFields.Add("Unique_code");
            xtraPagedNavigation_manualCheck_resultList.InvisibleFields.Add("is_corrected");
            xtraPagedNavigation_manualCheck_resultList.LoadDataToGridView();
            xtraPagedNavigation_manualCheck_resultList.ExportAlltoFileSqlString = "SELECT dh as '档号',error_type AS '问题类型',description AS '问题描述',user_id AS 检查人,record_time AS '记录时间',correct_user_id AS '修正人',corrected_time AS '修正时间' FROM " + this.FileCodeName + " WHERE " + this.SearchWhere + " ORDER BY Unique_code ASC ";
        }

        void cancelButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < TableLayoutPanel1.Controls.Count; i++)
            {
                Control control = TableLayoutPanel1.Controls[i];
                if (control.GetType() == typeof(TextEdit))
                {
                    TextEdit txtBox = (TextEdit)control;
                    txtBox.ResetText();
                }
            }
        }
    }
}