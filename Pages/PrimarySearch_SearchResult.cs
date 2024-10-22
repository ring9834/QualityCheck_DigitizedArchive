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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using System.Configuration;
using System.Xml;
using System.IO;

namespace Prj_FileManageNCheckApp
{
    public partial class PrimarySearch_SearchResult : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        public string ContentFormat { get; set; }
        public TableLayoutPanel TableLayoutPanel_AddControlForPrimarySearch;
        public UserEntity UserLoggedIn { get; set; }
        protected string SearchWhere { get; set; }
        protected string FieldString { get; set; }
        public PrimarySearch_SearchResult()
        {
            InitializeComponent();
        }

        public PrimarySearch_SearchResult(string fileCodeName, UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            this.ContentFormat = paras.ContentFormat;
            xtraTab_primarySearchResult.Controls.Clear();
            TableLayoutPanel_AddControlForPrimarySearch = new TableLayoutPanel();
            xtraTab_primarySearchResult.Controls.Add(TableLayoutPanel_AddControlForPrimarySearch);
            TableLayoutPanel_AddControlForPrimarySearch.ColumnStyles.Clear();
            TableLayoutPanel_AddControlForPrimarySearch.RowStyles.Clear();
            TableLayoutPanel_AddControlForPrimarySearch.BackColor = Color.White;
            TableLayoutPanel_AddControlForPrimarySearch.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            TableLayoutPanel_AddControlForPrimarySearch.ColumnCount = 8;
            TableLayoutPanel_AddControlForPrimarySearch.Top = 20;
            TableLayoutPanel_AddControlForPrimarySearch.Height = 10;
            SearchDAO sd = new SearchDAO();
            DataTable dt = sd.GetFieldsUsedForSearch(this.FileCodeName);//获得已经配置的用于搜索的字段信息
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                Label lab = new Label();
                lab.Name = "Lbl_" + row["col_name"].ToString();
                lab.Text = row["show_name"].ToString();
                lab.Tag = row["code_value"];
                lab.AutoSize = true;
                lab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));//通过Anchor 设置Label 列中居中
                TableLayoutPanel_AddControlForPrimarySearch.Controls.Add(lab);

                TextEdit txtObj = new TextEdit();
                txtObj.Name = "Txt_" + row["col_name"].ToString();
                txtObj.Tag = row["col_name"];
                txtObj.Width = 120;
                txtObj.Properties.NullValuePromptShowForEmptyValue = true;
                txtObj.Properties.NullValuePrompt = row["code_name"].ToString();
                TableLayoutPanel_AddControlForPrimarySearch.Controls.Add(txtObj);
            }
            TableLayoutPanel_AddControlForPrimarySearch.AutoSize = true;

            PageControlLocation.MakeControlHoritionalCenter(TableLayoutPanel_AddControlForPrimarySearch.Parent, TableLayoutPanel_AddControlForPrimarySearch);
            Panel buttonPanel = new Panel();
            Button searchButton = new Button();
            searchButton.Text = "搜索";
            this.AcceptButton = searchButton;
            searchButton.Click += searchButton_Click;
            Button cancelButton = new Button();
            cancelButton.Text = "重置";
            cancelButton.Click += cancelButton_Click;
            xtraTab_primarySearchResult.Controls.Add(buttonPanel);
            buttonPanel.Top = TableLayoutPanel_AddControlForPrimarySearch.Top + TableLayoutPanel_AddControlForPrimarySearch.Height + 15;
            buttonPanel.Width = xtraTab_primarySearchResult.Width;
            buttonPanel.Controls.Add(searchButton);
            buttonPanel.Controls.Add(cancelButton);
            PageControlLocation.MakeControlHoritionalCenter(searchButton.Parent, searchButton);
            searchButton.Left = searchButton.Left - searchButton.Width / 2;
            cancelButton.Left = searchButton.Left + searchButton.Width + 15;
            if (dt.Rows.Count == 0)
            {
                xtraTab_primarySearchResult.Controls.Add(groupControl_HintInfo);
                PageControlLocation.MakeControlCenter(groupControl_HintInfo.Parent, groupControl_HintInfo);
            }
            xtraTab_primarySearchResult.Show();
            dt.Dispose();
        }

        void searchButton_Click(object sender, EventArgs e)
        {
            string where = "1=1 ";
            List<string> whereFieldList = new List<string>();
            List<string> whereFieldValueList = new List<string>();
            for (int i = 0; i < this.TableLayoutPanel_AddControlForPrimarySearch.Controls.Count; i++)
            {
                Type controlType = this.TableLayoutPanel_AddControlForPrimarySearch.Controls[i].GetType();
                if (controlType == typeof(TextEdit))
                {
                    TextEdit txtBox = (TextEdit)this.TableLayoutPanel_AddControlForPrimarySearch.Controls[i];
                    if (!string.IsNullOrEmpty(txtBox.Text))
                    {
                        Label lbl = (Label)this.TableLayoutPanel_AddControlForPrimarySearch.Controls.Find("Lbl_" + txtBox.Tag.ToString(), false)[0];
                        string searchChar = lbl.Tag.ToString();
                        string fieldName = txtBox.Tag.ToString();
                        where += " AND " + fieldName + " " + searchChar + " @" + fieldName;
                        if (searchChar.ToLower().Contains("like"))//基本检索1
                            whereFieldValueList.Add("%" + txtBox.Text + "%");
                        else if (searchChar.Equals("="))//基本检索2
                            whereFieldValueList.Add("" + txtBox.Text + "");
                        whereFieldList.Add(fieldName);

                        //过滤特殊字符
                        //where = where.ToLower().Replace("'", "").Replace("<", "").Replace(">", "").Replace("or", "").Replace("and", "").Replace("=", "").Replace("[", "[[]").Replace("_", "[_]").Replace("%", "[%]");
                    }
                }
            }

            string selectFieldStr = "Unique_code";//所有搜索的语句，头部都加上Unique_code字段，只不过在搜索结果页不让它显示出来。 
            FieldShowListDAO fsd = new FieldShowListDAO();
            DataTable fieldDt = fsd.GetFieldsShownInList2(this.FileCodeName);//获得指定档案库类型对应的字段显示列表
            for (int i = 0; i < fieldDt.Rows.Count; i++)
            {
                selectFieldStr += "," + fieldDt.Rows[i]["col_name"].ToString() + " AS " + fieldDt.Rows[i]["show_name"].ToString();
            }
            fieldDt.Dispose();

            xtraPagedNavigation1.TableString = this.FileCodeName;
            xtraPagedNavigation1.FieldString = selectFieldStr;
            xtraPagedNavigation1.WhereString = where;
            xtraPagedNavigation1.WhereFieldArray = whereFieldList;
            xtraPagedNavigation1.WhereFieldValueArray = whereFieldValueList;
            xtraPagedNavigation1.SortString = "Unique_code ASC ";
            xtraPagedNavigation1.PagedGridView = gridView_primarysearch_records;
            xtraPagedNavigation1.PagedEventHandler = new ShowLinkButtonsInGridViewColumns[1];//委托数组
            xtraPagedNavigation1.PagedEventHandler[0] = ShowScannedFileContentLinkButton;
            xtraPagedNavigation1.InvisibleFields.Add("Unique_code");
            xtraPagedNavigation1.LoadDataToGridView();
            xtraPagedNavigation1.ExportAlltoFileSqlString = "SELECT " + selectFieldStr + " FROM " + this.FileCodeName + " WHERE " + where + " ORDER BY Unique_code ASC ";
            xtraTab_primarysearchRecord.Show();
        }

        void cancelButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < TableLayoutPanel_AddControlForPrimarySearch.Controls.Count; i++)
            {
                Control control = TableLayoutPanel_AddControlForPrimarySearch.Controls[i];
                if (control.GetType() == typeof(TextEdit))
                {
                    TextEdit txtBox = (TextEdit)control;
                    txtBox.ResetText();
                }
            }
        }

        void ShowScannedFileContentLinkButton()
        {
            RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();
            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.ButtonClick += riButtonEdit_PrimarySearch_ButtonClick;
            riButtonEdit.Buttons.Clear();
            EditorButton bt0 = new EditorButton();
            bt0.Kind = ButtonPredefines.Glyph;
            bt0.Caption = "原文";
            bt0.ToolTip = "点击打开原文";
            bt0.Appearance.ForeColor = Color.Blue;
            bt0.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt0);
            GridColumn Col1 = new GridColumn();
            Col1.FieldName = "openyw";
            Col1.Caption = "原文链接";
            Col1.VisibleIndex = gridView_primarysearch_records.Columns.Count;
            Col1.UnboundType = DevExpress.Data.UnboundColumnType.String;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_primarysearch_records.RepositoryItems.Add(riButtonEdit);
            gridView_primarysearch_records.Columns.Add(Col1);
            foreach (GridColumn c in gridView_primarysearch_records.Columns)
            {
                c.OptionsColumn.AllowEdit = c.ColumnEdit is RepositoryItemButtonEdit;
            }
            gridView_primarysearch_records.BestFitColumns();
        }

        void riButtonEdit_PrimarySearch_ButtonClick(object sender, EventArgs e)
        {
            int rowhandle = gridView_primarysearch_records.FocusedRowHandle;
            DataRow dr = gridView_primarysearch_records.GetDataRow(rowhandle);
            string uniquecode = dr["Unique_code"].ToString();
            string ywRootPath = ConfigurationManager.AppSettings["ContentFileRootPath"];
            if (this.ContentFormat.ToLower().Contains("pdf"))
            {
                ContentFileDAO cfd = new ContentFileDAO();
                object ywPathStr = cfd.VerifyYWAlreadyLinked2(uniquecode, this.FileCodeName);//判断原文是否已挂接
                if (ywPathStr ==DBNull.Value || string.IsNullOrEmpty(ywPathStr.ToString()))
                {
                    MessageBox.Show("本条目录的原文还未挂接，或没有原文！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string path = ywRootPath + ywPathStr;//拼接成绝对路径
                try
                {
                    if (System.IO.File.Exists(path))
                        System.Diagnostics.Process.Start(path);//打开PDF文件的默认浏览器
                    else
                        MessageBox.Show("打不开原文！原因可能是：\r\n1、原文根路径当前设置在“" + ywRootPath + "”，请检查与原文挂接时设置的路径是否一致；\r\n2、请检查访问原文网络（服务器）时是否已经登录。\r\n若不知如何操作，请联系系统管理员或售后人员。", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch
                {
                    MessageBox.Show("打不开原文！原因可能是：\r\n1、原文根路径当前设置在“" + ywRootPath + "”，请检查与原文挂接时设置的路径是否一致；\r\n2、请检查访问原文网络（服务器）时是否已经登录。\r\n若不知如何操作，请联系系统管理员或售后人员。", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                ArchiveNoDAO and = new ArchiveNoDAO();
                DataTable dt = and.GetArchiveNoDataByCodeName(this.FileCodeName);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show(this, "档号还未配置，请到配置页面操作后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (dt.Rows[0]["archive_num_field"] == DBNull.Value || dt.Rows[0]["archive_num_field"] == null)
                {
                    MessageBox.Show(this, "档号对应字段还未配置，请到配置页面操作后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string archiveNoFieldName = dt.Rows[0]["archive_num_field"].ToString();
                ContentFileDAO cfd = new ContentFileDAO();
                dt = cfd.VerifyImageYWAlreadyLinked(this.FileCodeName, archiveNoFieldName, uniquecode);//判断IMAGE原文是否挂接
                object ywXMLStr = dt.Rows[0]["yw_xml"];
                if (ywXMLStr == DBNull.Value || string.IsNullOrEmpty(ywXMLStr.ToString()))
                {
                    MessageBox.Show("本条目录的原文还未挂接，或没有原文！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                object dhObj = dt.Rows[0][archiveNoFieldName];//档号

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ywXMLStr.ToString());
                XmlNodeList nodeList = doc.SelectNodes(@"YW/ywPath");
                if (nodeList.Count > 0)
                {
                    List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        FileInfo fi = new FileInfo(nodeList[i].InnerText);
                        string fileName = fi.Name;
                        list.Add(new KeyValuePair<string, string>((nodeList[i].InnerText), fileName));
                    }
                    ImageFileContentBrower ifcb = new ImageFileContentBrower(dhObj.ToString(),list);
                    ifcb.Show();
                }
            }
        }

        private void PrimarySearch_SearchResult_Load(object sender, EventArgs e)
        {
            xtraTabControl_archiveSearchManage.ShowTabHeader = DefaultBoolean.False;
            xtraGoFrontBack1.Parent = panelControl1;
            xtraGoFrontBack1.TabControl = xtraTabControl_archiveSearchManage;
            xtraGoFrontBack1.UpdateBackButtonStateFun = new UpdateBackButtonState(xtraGoFrontBack1.UpdateControls);
            xtraGoFrontBack1.UpdateBackButtonStateFun();
        }

        private void xtraTabControl_archiveSearchManage_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            xtraGoFrontBack1.UpdateBackButtonStateFun();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED 
                return cp;
            }
        }
    }
}