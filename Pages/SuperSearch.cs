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
    public partial class SuperSearch : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }
        public UserEntity UserLoggedIn { get; set; }
        public string ContentFormat { get; set; }

        public TableLayoutPanel TableLayoutPanel_AddControlForSuperSearch;
        private XtraSuperSearch SuperSearchUserControl { get; set; }
        private DataTable DT { get; set; }
        private List<SuperSearchConditionEntity> SuperSearchConditonList { get; set; }


        public SuperSearch()
        {
            InitializeComponent();
        }

        public SuperSearch(string fileCodeName)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            SuperSearchUserControl = new XtraSuperSearch(true);
            SuperSearchConditonList = new List<SuperSearchConditionEntity>();
        }

        public SuperSearch(string fileCodeName, UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            this.ContentFormat = paras.ContentFormat;
            this.SuperSearchConditonList.Clear();
            xtraTab_superSearchCondition.Controls.Clear();
            TableLayoutPanel_AddControlForSuperSearch = new TableLayoutPanel();
            xtraTab_superSearchCondition.Controls.Add(TableLayoutPanel_AddControlForSuperSearch);
            TableLayoutPanel_AddControlForSuperSearch.ColumnStyles.Clear();
            TableLayoutPanel_AddControlForSuperSearch.RowStyles.Clear();
            TableLayoutPanel_AddControlForSuperSearch.BackColor = Color.White;
            TableLayoutPanel_AddControlForSuperSearch.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            TableLayoutPanel_AddControlForSuperSearch.ColumnCount = 2;
            TableLayoutPanel_AddControlForSuperSearch.Top = 20;
            TableLayoutPanel_AddControlForSuperSearch.AutoSize = true;
            SearchDAO sd = new SearchDAO();
            this.DT = sd.GetFieldsUsedForSearch(this.FileCodeName);//获得已经配置的用于搜索的字段信息
            Dictionary<string, SuperSearchConditionEntity> dcs = new Dictionary<string, SuperSearchConditionEntity>();
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                SuperSearchConditionEntity ssce = new SuperSearchConditionEntity();
                DataRow row = DT.Rows[i];
                Label lab = new Label();
                lab.Name = "Lbl_" + row["col_name"].ToString();
                lab.Text = row["show_name"].ToString();
                lab.Tag = row["col_name"];
                lab.AutoSize = true;
                lab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));//通过Anchor 设置Label 列中居中
                TableLayoutPanel_AddControlForSuperSearch.Controls.Add(lab);
                ssce.Field = lab.Tag;

                FlowLayoutPanel p = new FlowLayoutPanel();
                p.Name = "flp_" + i;
                p.Height = 30;
                p.AutoSize = true;
                p.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;

                XtraSuperSearch txtObj = new XtraSuperSearch();
                txtObj.comboBoxEdit1.SelectedIndexChanged += comboBoxEdit1_SelectedIndexChanged;
                txtObj.comboBoxEdit3.SelectedIndexChanged += comboBoxEdit3_SelectedIndexChanged;
                txtObj.comboBoxEdit2.Leave += comboBoxEdit2_Leave;
                txtObj.Name = "Txt_" + i;
                txtObj.AndOrControlVisible = false;
                txtObj.AndOrItems.AddRange(SuperSearchUserControl.AndOrItems);
                txtObj.ConditionItems.AddRange(SuperSearchUserControl.ConditionItems);
                txtObj.Width = txtObj.Width - txtObj.comboBoxEdit1.Width - 8;
                p.Controls.Add(txtObj);
                ssce.ID = txtObj.Name;
                this.SuperSearchConditonList.Add(ssce);//增加一个用户控件，就对应生成一个SuperSearchConditionEntity，以搜集andor 和 大于等于等条件信息

                SimpleButton sb = new SimpleButton();
                sb.Name = "sb_" + i + "_" + lab.Tag.ToString();
                sb.Text = "增加";
                sb.Click += sb_Click;
                p.Controls.Add(sb);
                TableLayoutPanel_AddControlForSuperSearch.Controls.Add(p);
            }
            TableLayoutPanel_AddControlForSuperSearch.AutoSize = true;
            PageControlLocation.MakeControlHoritionalCenter(TableLayoutPanel_AddControlForSuperSearch.Parent, TableLayoutPanel_AddControlForSuperSearch);
            Panel buttonPanel = new Panel();
            Button searchButton = new Button();
            searchButton.Text = "搜索";
            this.AcceptButton = searchButton;
            searchButton.Click += searchButton_Click;
            Button cancelButton = new Button();
            cancelButton.Text = "重置";
            cancelButton.Click += cancelButton_Click;
            xtraTab_superSearchCondition.Controls.Add(buttonPanel);
            buttonPanel.Top = TableLayoutPanel_AddControlForSuperSearch.Top + TableLayoutPanel_AddControlForSuperSearch.Height + 15;
            buttonPanel.Width = xtraTab_superSearchCondition.Width;
            buttonPanel.Controls.Add(searchButton);
            buttonPanel.Controls.Add(cancelButton);
            PageControlLocation.MakeControlHoritionalCenter(searchButton.Parent, searchButton);
            searchButton.Left = searchButton.Left - searchButton.Width / 2;
            cancelButton.Left = searchButton.Left + searchButton.Width + 15;
            if (DT.Rows.Count == 0)
            {
                xtraTab_superSearchCondition.Controls.Add(groupControl_HintInfo);
                PageControlLocation.MakeControlCenter(groupControl_HintInfo.Parent, groupControl_HintInfo);
            }
            xtraTab_superSearchCondition.Show();
        }

        void sb_Click(object sender, EventArgs e)
        {
            string name = ((SimpleButton)sender).Name;
            string[] ss = name.Split('_');
            string ordial = ss[1];
            FlowLayoutPanel flp = (FlowLayoutPanel)(TableLayoutPanel_AddControlForSuperSearch.Controls.Find("flp_" + ordial, false))[0];
            int controlCount = flp.Controls.Count;

            if (controlCount == 6)
            {
                MessageBox.Show("本字段条件不能超过5个！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SuperSearchConditionEntity ssce = new SuperSearchConditionEntity();
            XtraSuperSearch txtObj = new XtraSuperSearch();
            txtObj.comboBoxEdit1.SelectedIndexChanged += comboBoxEdit1_SelectedIndexChanged;
            txtObj.comboBoxEdit3.SelectedIndexChanged += comboBoxEdit3_SelectedIndexChanged;
            txtObj.comboBoxEdit2.Leave += comboBoxEdit2_Leave;
            txtObj.Name = "Txt_" + ordial + "_" + (controlCount - 1);
            txtObj.AndOrItems.AddRange(SuperSearchUserControl.AndOrItems);
            txtObj.ConditionItems.AddRange(SuperSearchUserControl.ConditionItems);
            ssce.ID = txtObj.Name;
            ssce.Field = ss[2];
            this.SuperSearchConditonList.Add(ssce);//增加一个用户控件，就对应生成一个SuperSearchConditionEntity，以搜集andor 和 大于等于等条件信息

            SimpleButton sbDelte = (SimpleButton)(flp.Controls.Find("sb_" + ordial + "_" + ss[2], true))[0];
            flp.Controls.Remove(sbDelte);
            flp.Controls.Add(txtObj);

            SimpleButton sb = new SimpleButton();
            sb.Name = "sb_" + ordial + "_" + ss[2];
            sb.Text = "增加";
            sb.Click += sb_Click;
            flp.Controls.Add(sb);
            if (flp.Width > (int)(ClientSize.Width * 0.85))
            {
                flp.Height = flp.Height + 30;//向下扩展
            }
            PageControlLocation.MakeControlHoritionalCenter(TableLayoutPanel_AddControlForSuperSearch.Parent, TableLayoutPanel_AddControlForSuperSearch);
        }

        void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxEdit cbe = sender as ComboBoxEdit;
            string id = ((XtraSuperSearch)cbe.Parent.Parent).Name;
            IEnumerable<SuperSearchConditionEntity> ssces = this.SuperSearchConditonList.Where(t => t.ID.Equals(id));
            if (ssces.Count() > 0)
            {
                SuperSearchConditionEntity ssce = ssces.ElementAt(0);
                ssce.AndOr = ((SearchConditionEntity)cbe.SelectedItem).Value;
            }
        }

        void comboBoxEdit2_Leave(object sender, EventArgs e)
        {
            ComboBoxEdit cbe = sender as ComboBoxEdit;
            string id = ((XtraSuperSearch)cbe.Parent.Parent).Name;
            IEnumerable<SuperSearchConditionEntity> ssces = this.SuperSearchConditonList.Where(t => t.ID.Equals(id));
            if (ssces.Count() > 0)
            {
                SuperSearchConditionEntity ssce = ssces.ElementAt(0);
                ssce.Value = cbe.Text;
            }
        }

        void comboBoxEdit3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxEdit cbe = sender as ComboBoxEdit;
            string id = ((XtraSuperSearch)cbe.Parent.Parent).Name;
            IEnumerable<SuperSearchConditionEntity> ssces = this.SuperSearchConditonList.Where(t => t.ID.Equals(id));
            if (ssces.Count() > 0)
            {
                SuperSearchConditionEntity ssce = ssces.ElementAt(0);
                ssce.Condition = ((SearchConditionEntity)cbe.SelectedItem).Value;
            }
        }

        void searchButton_Click(object sender, EventArgs e)
        {
            List<string> whereFieldList = new List<string>();
            List<string> whereFieldValueList = new List<string>();
            int counter = 0;
            string where = "1=1 ";
            for (int i = 0; i < this.DT.Rows.Count; i++)
            {
                string fieldName = this.DT.Rows[i]["col_name"].ToString();
                IEnumerable<SuperSearchConditionEntity> ssces = this.SuperSearchConditonList.Where(t => t.Field.ToString().Equals(fieldName));
                where += "AND (";
                for (int j = 0; j < ssces.Count(); j++)
                {
                    SuperSearchConditionEntity ssce = ssces.ElementAt(j);

                    if (ssce.AndOr != null && (ssce.Condition == null || ssce.Value == null))
                    {
                        MessageBox.Show("尚有比较条件或搜索值未选择或填写，请选择填写后继续！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (j == 0)
                    {
                        if (ssce.Condition != null && ssce.Value != null)
                        {
                            where += " " + fieldName + " " + ssce.Condition + " @" + fieldName + "_" + j + " ";
                            whereFieldList.Add(fieldName + "_" + j);
                            if (ssce.Condition.ToString().ToLower().Contains("like"))
                                whereFieldValueList.Add("%" + ssce.Value.ToString() + "%");
                            else
                                whereFieldValueList.Add(ssce.Value.ToString());
                            counter++;
                        }
                    }
                    else
                    {
                        if (ssce.AndOr != null && ssce.Condition != null && ssce.Value != null)
                        {
                            where += " " + ssce.AndOr + " " + fieldName + " " + ssce.Condition + " @" + fieldName + "_" + j + " ";
                            whereFieldList.Add(fieldName + "_" + j);
                            if (ssce.Condition.ToString().ToLower().Contains("like"))
                                whereFieldValueList.Add("%" + ssce.Value.ToString() + "%");
                            else
                                whereFieldValueList.Add(ssce.Value.ToString());
                            counter++;
                        }
                    }
                    if (counter == 0)
                        where += " 1=1 ";
                    else
                        counter = 0;//初始化，重新开始计数
                }
                where += ")\r\n";
            }
            if (whereFieldList.Count == 0)//搜索所有记录
                where = "1=1 ";

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
            xtraPagedNavigation1.PagedGridView = gridView_superSearch;
            xtraPagedNavigation1.PagedEventHandler = new ShowLinkButtonsInGridViewColumns[1];//委托数组
            xtraPagedNavigation1.PagedEventHandler[0] = ShowScannedFileContentLinkButton;
            xtraPagedNavigation1.InvisibleFields.Add("Unique_code");
            xtraPagedNavigation1.LoadDataToGridView();
            xtraPagedNavigation1.ExportAlltoFileSqlString = "SELECT " + selectFieldStr + " FROM " + this.FileCodeName + " WHERE " + where + " ORDER BY Unique_code ASC ";
            xtraTab_superSearchRecord.Show();
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
            Col1.VisibleIndex = gridView_superSearch.Columns.Count;
            Col1.UnboundType = DevExpress.Data.UnboundColumnType.String;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_SuperSearch.RepositoryItems.Add(riButtonEdit);
            gridView_superSearch.Columns.Add(Col1);
            foreach (GridColumn c in gridView_superSearch.Columns)
            {
                c.OptionsColumn.AllowEdit = c.ColumnEdit is RepositoryItemButtonEdit;
            }
            gridView_superSearch.BestFitColumns();
        }

        void riButtonEdit_PrimarySearch_ButtonClick(object sender, EventArgs e)
        {
            int rowhandle = gridView_superSearch.FocusedRowHandle;
            DataRow dr = gridView_superSearch.GetDataRow(rowhandle);
            string uniquecode = dr["Unique_code"].ToString();
            string ywRootPath = ConfigurationManager.AppSettings["ContentFileRootPath"];
            if (this.ContentFormat.ToLower().Contains("pdf"))
            {
                ContentFileDAO cfd = new ContentFileDAO();
                object ywPathStr = cfd.VerifyYWAlreadyLinked2(uniquecode, this.FileCodeName);//判断原文是否已挂接
                if (ywPathStr == DBNull.Value || string.IsNullOrEmpty(ywPathStr.ToString()))
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
                    ImageFileContentBrower ifcb = new ImageFileContentBrower(dhObj.ToString(), list);
                    ifcb.Show();
                }
            }
        }

        void cancelButton_Click(object sender, EventArgs e)
        {
            this.SuperSearchConditonList.Clear();
            TableLayoutPanel_AddControlForSuperSearch.Controls.Clear();
            Dictionary<string, SuperSearchConditionEntity> dcs = new Dictionary<string, SuperSearchConditionEntity>();
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                SuperSearchConditionEntity ssce = new SuperSearchConditionEntity();
                DataRow row = DT.Rows[i];
                Label lab = new Label();
                lab.Name = "Lbl_" + row["col_name"].ToString();
                lab.Text = row["show_name"].ToString();
                lab.Tag = row["col_name"];
                lab.AutoSize = true;
                lab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));//通过Anchor 设置Label 列中居中
                TableLayoutPanel_AddControlForSuperSearch.Controls.Add(lab);
                ssce.Field = lab.Tag;

                FlowLayoutPanel p = new FlowLayoutPanel();
                p.Name = "flp_" + i;
                p.Height = 30;
                p.AutoSize = true;
                p.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;

                XtraSuperSearch txtObj = new XtraSuperSearch();
                txtObj.comboBoxEdit1.SelectedIndexChanged += comboBoxEdit1_SelectedIndexChanged;
                txtObj.comboBoxEdit3.SelectedIndexChanged += comboBoxEdit3_SelectedIndexChanged;
                txtObj.comboBoxEdit2.Leave += comboBoxEdit2_Leave;
                txtObj.Name = "Txt_" + i;
                txtObj.AndOrControlVisible = false;
                txtObj.AndOrItems.AddRange(SuperSearchUserControl.AndOrItems);
                txtObj.ConditionItems.AddRange(SuperSearchUserControl.ConditionItems);
                txtObj.Width = txtObj.Width - txtObj.comboBoxEdit1.Width - 8;
                p.Controls.Add(txtObj);
                ssce.ID = txtObj.Name;
                this.SuperSearchConditonList.Add(ssce);//增加一个用户控件，就对应生成一个SuperSearchConditionEntity，以搜集andor 和 大于等于等条件信息

                SimpleButton sb = new SimpleButton();
                sb.Name = "sb_" + i + "_" + lab.Tag.ToString();
                sb.Text = "增加";
                sb.Click += sb_Click;
                p.Controls.Add(sb);
                TableLayoutPanel_AddControlForSuperSearch.Controls.Add(p);
            }
        }

        //解决加载用户控件时闪烁的问题
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED 
                return cp;
            }
        }

        private void SuperSearch_Load(object sender, EventArgs e)
        {
            xtraGoFrontBack1.Parent = panelControl1;
            xtraGoFrontBack1.TabControl = xtraTabControl_superSearch;
            xtraGoFrontBack1.UpdateBackButtonStateFun = new UpdateBackButtonState(xtraGoFrontBack1.UpdateControls);
            xtraGoFrontBack1.UpdateBackButtonStateFun();
        }

        private void xtraTabControl_superSearch_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            xtraGoFrontBack1.UpdateBackButtonStateFun();
        }
    }
}