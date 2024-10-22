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
using DotNet.Utilities;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;
using System.IO;

namespace Prj_FileManageNCheckApp
{
    public partial class ManualCheck_Goto : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        public string ContentFormat { get; set; }
        private string SearchCondtionForManualCheck { get; set; }//用于手动质检的搜索条件
        public UserEntity UserLoggedIn { get; set; }
        public string CreatingTableFields { get; set; }
        public string FieldStrWithoutAlias { get; set; }
        public DataTable DtRole { get; set; }
        private DataTable DtRowStyle { get; set; }
        private string ArchiveNoFieldName { get; set; }
        public ManualCheck_Goto()
        {
            InitializeComponent();
        }

        public ManualCheck_Goto(string fileCodeName, UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        private void ManualCheck_Goto_Load(object sender, EventArgs e)
        {
            xtraTabControl_manualCheck_gotoCheck.ShowTabHeader = DefaultBoolean.False;
            this.WindowState = FormWindowState.Maximized;
            xtraGoFrontBack1.Parent = panelControl1;
            xtraGoFrontBack1.TabControl = xtraTabControl_manualCheck_gotoCheck;
            xtraGoFrontBack1.UpdateBackButtonStateFun = new UpdateBackButtonState(xtraGoFrontBack1.UpdateControls);
            xtraGoFrontBack1.UpdateBackButtonStateFun();
            InitiateErrorCollectPanel();

            xtraUpdateButtonForGrid1.Parent = panelControl1;
            xtraUpdateButtonForGrid1.RefreshGridViewDataFunc = new RefreshGridViewData(RefreshDatas);//当页刷新

            gridView_manualCataloge_shouldCheck.IndicatorWidth = 36;
            gridView_manualCataloge_recordForCheck.IndicatorWidth = 36;

            UserDAO ud = new UserDAO();
            this.DtRole = ud.LoadRoles2();

            ManualCheckDAO mcd = new ManualCheckDAO();
            this.DtRowStyle = mcd.GetManualCheckedRecords(this.FileCodeName);
        }

        private void InitiateErrorCollectPanel()
        {
            PageControlLocation.MakeControlHoritionalCenter(panel_manualCheck_errorCollect.Parent, panel_manualCheck_errorCollect);
            ConfigCodeDAO ccd = new ConfigCodeDAO();
            DataTable dtManualcheck = ccd.GetCodes("mljc");
            comboBoxEdit_errorType.Properties.Items.Clear();
            for (int i = 0; i < dtManualcheck.Rows.Count; i++)
            {
                comboBoxEdit_errorType.Properties.Items.Add(dtManualcheck.Rows[i]["code_name"]);
            }
            //splitContainerControl_manualCheck.SplitterPosition = this.Height - 230;
            //splitContainerControl_manualCheck.SplitterPosition = this.Height - 230;
            splitContainerControl_manualCheck.Collapsed = true;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            this.ContentFormat = paras.ContentFormat;
            RefreshDatas();
        }

        string GetSuperRoleForWorkFlow(out string roleId)
        {
            DataFlowDAO dfd = new DataFlowDAO();
            DataTable dt = dfd.GetDataFlow();
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("数字化加工多方协作工作流（数据递交流程）还未配置，请配置后再继续！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                roleId = string.Empty;
                return string.Empty;
            }
            roleId = dt.Rows[0]["role_id"].ToString();
            string xml = dt.Rows[0]["super_access_role"].ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNodeList nodeList = doc.SelectNodes(@"RoleForWorkFlowConfig/ConfigItem");
            string superRole = string.Empty;
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (i == 0)
                {
                    superRole = "'" + nodeList[i].Attributes["RoleId"].Value + "'";
                }
                else
                {
                    superRole += "," + "'" + nodeList[i].Attributes["RoleId"].Value + "'";
                }
            }
            dt.Dispose();
            return superRole;
        }

        public void RefreshDatas()
        {
            string roleId = string.Empty;//数据递交流程第一个节点负责的角色
            string superRole = GetSuperRoleForWorkFlow(out roleId);//获得工作流全控制角色
            if (string.IsNullOrEmpty(superRole)) return;
            ManualCheckDAO mcd = new ManualCheckDAO();
            DataTable dt = mcd.GetConfiguredFieldsForManualCheck(this.FileCodeName);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("此档案类型库还未进行手动质检（验收）配置，请配置后再继续！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridControl_manualCataloge_shouldCheck.DataSource = null;
                gridView_manualCataloge_shouldCheck.PopulateColumns();
                dt.Dispose();
                return;
            }

            string fieldStr = string.Empty, condtionStr = string.Empty, orderByStr = string.Empty;
            CreatingTableFields = string.Empty;//创建临时表字段，存放需要手工质检的记录汇总信息
            string fieldGroupString = string.Empty;
            this.SearchCondtionForManualCheck = "1=1 ";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 0)
                {
                    fieldStr = dt.Rows[i]["col_name"].ToString() + " AS '" + dt.Rows[i]["show_name"].ToString() + "'";
                    condtionStr = "t2." + dt.Rows[i]["col_name"].ToString() + "=t1." + dt.Rows[i]["col_name"].ToString();
                    orderByStr = dt.Rows[i]["col_name"].ToString();
                    CreatingTableFields = dt.Rows[i]["col_name"].ToString() + " nvarchar(Max)";
                    FieldStrWithoutAlias = dt.Rows[i]["col_name"].ToString();
                    fieldGroupString = dt.Rows[i]["col_name"].ToString();
                }
                else
                {
                    fieldStr += "," + dt.Rows[i]["col_name"].ToString() + " AS '" + dt.Rows[i]["show_name"].ToString() + "'";
                    condtionStr += " AND t2." + dt.Rows[i]["col_name"].ToString() + "=t1." + dt.Rows[i]["col_name"].ToString();
                    orderByStr += "," + dt.Rows[i]["col_name"].ToString();
                    CreatingTableFields += "," + dt.Rows[i]["col_name"].ToString() + " nvarchar(Max)";
                    FieldStrWithoutAlias += "," + dt.Rows[i]["col_name"].ToString();
                    fieldGroupString += "+'-'+" + dt.Rows[i]["col_name"].ToString();
                }
                this.SearchCondtionForManualCheck += " AND " + dt.Rows[i]["col_name"].ToString() + "='{" + i + "}'";
            }

            if (this.UserLoggedIn.RoleId.Equals(roleId) || superRole.Contains(this.UserLoggedIn.RoleId)) //登录的是工作流第一个节点角色,或控制整个流程的角色
            {
                //如果没有临时表，就先创建临时表，后缀加上“_manual_check”
                if (!string.IsNullOrEmpty(CreatingTableFields))
                {
                    mcd.CreateTempTableForManualCheck(CreatingTableFields, this.FileCodeName);
                }
                dt.Dispose();

                //筛选 使用 EXCEPT
                mcd.SelectDataForManualCheck(FieldStrWithoutAlias, this.FileCodeName, fieldGroupString, condtionStr);
                xtraTab_manualCheck_shouldCheck.Show();

                //使用了分页控件，再也不用那么多重复代码了，各分页控件间也不用公用PageIndex,PageCount，TableString等变量了
                string tableStr = this.FileCodeName + "_manual_check";
                string fieldStrParam = fieldStr + ",record_count AS '记录数量（条）',Unique_code";
                string whereStr = " 1=1";
                string sortStr = orderByStr;
                xtraPagedNavigation_shouldCheck.TableString = tableStr;
                xtraPagedNavigation_shouldCheck.FieldString = fieldStrParam;
                xtraPagedNavigation_shouldCheck.WhereString = whereStr;
                xtraPagedNavigation_shouldCheck.SortString = sortStr;
                xtraPagedNavigation_shouldCheck.PagedGridView = gridView_manualCataloge_shouldCheck;
                xtraPagedNavigation_shouldCheck.PagedEventHandler = new ShowLinkButtonsInGridViewColumns[2];//委托数组
                xtraPagedNavigation_shouldCheck.PagedEventHandler[0] = ShowGotoManualCheckLinkButton;
                xtraPagedNavigation_shouldCheck.PagedEventHandler[1] = ShowGotoManualCheckCompleteLinkButton;
                xtraPagedNavigation_shouldCheck.InvisibleFields.Add("Unique_code");
                xtraPagedNavigation_shouldCheck.LoadDataToGridView();
                xtraPagedNavigation_shouldCheck.ExportAlltoFileSqlString = "SELECT * FROM " + this.FileCodeName + "_manual_check ";

            }
            else //其他后续流程上的角色只能看到提交后的数据
            {
                string tableStr = this.FileCodeName + "_workflow_rec";
                string fieldStrParam = fieldStr + ",record_count AS '记录数量（条）',Unique_code";
                string whereStr = " is_completed = 0 AND role_id='" + this.UserLoggedIn.RoleId + "'";
                string sortStr = orderByStr;
                xtraPagedNavigation_shouldCheck.TableString = tableStr;
                xtraPagedNavigation_shouldCheck.FieldString = fieldStrParam;
                xtraPagedNavigation_shouldCheck.WhereString = whereStr;
                xtraPagedNavigation_shouldCheck.SortString = sortStr;
                xtraPagedNavigation_shouldCheck.PagedGridView = gridView_manualCataloge_shouldCheck;
                xtraPagedNavigation_shouldCheck.PagedEventHandler = new ShowLinkButtonsInGridViewColumns[2];//委托数组
                xtraPagedNavigation_shouldCheck.PagedEventHandler[0] = ShowGotoManualCheckLinkButton;
                xtraPagedNavigation_shouldCheck.PagedEventHandler[1] = ShowGotoManualCheckCompleteLinkButton;
                xtraPagedNavigation_shouldCheck.InvisibleFields.Add("Unique_code");
                xtraPagedNavigation_shouldCheck.LoadDataToGridView();
                xtraPagedNavigation_shouldCheck.ExportAlltoFileSqlString = "SELECT * FROM " + this.FileCodeName + "_workflow_rec WHERE is_completed = 0 AND role_id='" + this.UserLoggedIn.RoleId + "'";
            }

            //if (superRole.Contains(this.UserLoggedIn.RoleId))//控制整个流程的角色，可查看所有节点的当前数据
            //{
            //    checkEdit1.Visible = true;
            string tableStr2 = this.FileCodeName + "_workflow_rec";
            string fieldStrParam2 = fieldStr + ",record_count AS '记录数量（条）',role_id AS '当前数据所在位置',is_completed AS '是否验收结束',Unique_code";
            string whereStr2 = " 1=1 ";
            string sortStr2 = orderByStr;
            xtraPagedNavigation_alldata.TableString = tableStr2;
            xtraPagedNavigation_alldata.FieldString = fieldStrParam2;
            xtraPagedNavigation_alldata.WhereString = whereStr2;
            xtraPagedNavigation_alldata.SortString = sortStr2;
            xtraPagedNavigation_alldata.PagedGridView = gridView_allData_inWorkflow;
            xtraPagedNavigation_alldata.InvisibleFields.Add("Unique_code");
            xtraPagedNavigation_alldata.LoadDataToGridView();
            xtraPagedNavigation_alldata.ExportAlltoFileSqlString = "SELECT * FROM " + this.FileCodeName + "_workflow_rec";
            foreach (GridColumn c in gridView_allData_inWorkflow.Columns)
            {
                c.OptionsColumn.AllowEdit = false;
            }
            //}
            //else
            //{
            //    checkEdit1.Visible = false;
            //}
        }

        private void ShowGotoManualCheckLinkButton()
        {
            RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();
            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.ButtonClick += riButtonEdit_ManualCheck_ButtonClick;
            riButtonEdit.Buttons.Clear();
            EditorButton bt0 = new EditorButton();
            bt0.Kind = ButtonPredefines.Glyph;
            bt0.Caption = "去质检（验收）";
            bt0.ToolTip = "点击去手动质检（验收）";
            bt0.Appearance.ForeColor = Color.Blue;
            bt0.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt0);
            GridColumn Col1 = new GridColumn();
            Col1.FieldName = "manualcheck";
            Col1.Caption = "手动质检（验收）链接";
            Col1.Visible = false;
            Col1.VisibleIndex = gridView_manualCataloge_shouldCheck.Columns.Count;
            Col1.UnboundType = DevExpress.Data.UnboundColumnType.String;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_manualCataloge_shouldCheck.RepositoryItems.Add(riButtonEdit);
            gridView_manualCataloge_shouldCheck.Columns.Add(Col1);
        }
        private void ShowGotoManualCheckCompleteLinkButton()
        {
            RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();
            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.ButtonClick += riButtonEdit_ManualCheckComplete_ButtonClick;
            riButtonEdit.Buttons.Clear();
            EditorButton bt0 = new EditorButton();
            bt0.Kind = ButtonPredefines.Glyph;
            bt0.Caption = "提交";
            bt0.ToolTip = "提交给下一流程（比如，监理方。根据配置的流程自动提交给相应人员。）";
            bt0.Appearance.ForeColor = Color.Blue;
            bt0.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt0);
            GridColumn Col1 = new GridColumn();
            Col1.FieldName = "completeThisArea";
            Col1.Caption = "完成后提交";
            Col1.Visible = false;
            Col1.VisibleIndex = gridView_manualCataloge_shouldCheck.Columns.Count;
            Col1.UnboundType = DevExpress.Data.UnboundColumnType.String;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_manualCataloge_shouldCheck.RepositoryItems.Add(riButtonEdit);
            gridView_manualCataloge_shouldCheck.Columns.Add(Col1);
            foreach (GridColumn c in gridView_manualCataloge_shouldCheck.Columns)
            {
                c.OptionsColumn.AllowEdit = c.ColumnEdit is RepositoryItemButtonEdit;
            }
        }

        void riButtonEdit_ManualCheck_ButtonClick(object sender, EventArgs e)
        {
            ArchiveNoDAO and = new ArchiveNoDAO();
            object archiveNum = and.GetArchiveNoDataByCodeName2(this.FileCodeName);
            this.ArchiveNoFieldName = archiveNum.ToString();
            int rowhandle = gridView_manualCataloge_shouldCheck.FocusedRowHandle;
            DataRow dr = gridView_manualCataloge_shouldCheck.GetDataRow(rowhandle);
            string where = string.Format(this.SearchCondtionForManualCheck, dr.ItemArray);//很好的赋值方法
            string selectFieldStr = "Unique_code";//用于打开原文用。
            FieldShowListDAO fsd = new FieldShowListDAO();
            DataTable fieldDt = fsd.GetFieldsShownInList2(this.FileCodeName);//获得指定档案库类型对应的字段显示列表
            for (int i = 0; i < fieldDt.Rows.Count; i++)
            {
                selectFieldStr += "," + fieldDt.Rows[i]["col_name"].ToString() + " AS " + fieldDt.Rows[i]["show_name"].ToString();
            }
            //if (!selectFieldStr.Contains(this.ArchiveNoFieldName))
            selectFieldStr += "," + this.ArchiveNoFieldName;
            fieldDt.Dispose();

            //使用了分页控件，再也不用那么多重复代码了，各分页控件间也不用公用PageIndex,PageCount，TableString等变量了
            xtraPagedNavigation_recordForCheck.TableString = this.FileCodeName;
            xtraPagedNavigation_recordForCheck.FieldString = selectFieldStr;
            xtraPagedNavigation_recordForCheck.WhereString = where;
            xtraPagedNavigation_recordForCheck.SortString = "Unique_code ASC ";
            xtraPagedNavigation_recordForCheck.PagedGridView = gridView_manualCataloge_recordForCheck;
            xtraPagedNavigation_recordForCheck.PagedEventHandler = new ShowLinkButtonsInGridViewColumns[2];//委托数组
            xtraPagedNavigation_recordForCheck.PagedEventHandler[0] = ShowYwForManualCheckLinkButton;
            xtraPagedNavigation_recordForCheck.PagedEventHandler[1] = ShowCollectErrorForManualCheckLinkButton;
            //xtraPagedNavigation_recordForCheck.PagedEventHandler[2] = SetGridCellColorOnCondition;
            xtraPagedNavigation_recordForCheck.InvisibleFields.Add("Unique_code");
            xtraPagedNavigation_recordForCheck.InvisibleFields.Add(this.ArchiveNoFieldName);
            xtraPagedNavigation_recordForCheck.LoadDataToGridView();
            xtraPagedNavigation_recordForCheck.ExportAlltoFileSqlString = "SELECT " + selectFieldStr + " FROM " + this.FileCodeName + " WHERE " + where + " ORDER BY " + xtraPagedNavigation_recordForCheck.SortString;
            xtraTab_manualCheck_checkList.Show();
        }

        void riButtonEdit_ManualCheckComplete_ButtonClick(object sender, EventArgs e)
        {
            DataFlowDAO dfd = new DataFlowDAO();
            DataTable dt0 = dfd.GetDataFlowDataByRole(this.UserLoggedIn.RoleId);//根据角色获得数据流信息
            if (dt0.Rows.Count == 0)
            {
                MessageBox.Show("本登录人员不在多方协作工作流中（一般包括加工方、监理方和验收方等），将不能进行提交操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int rowhandle = gridView_manualCataloge_shouldCheck.FocusedRowHandle;
            DataRow dr = gridView_manualCataloge_shouldCheck.GetDataRow(rowhandle);
            string where = string.Format(this.SearchCondtionForManualCheck, dr.ItemArray);
            ContentFileDAO cfd = new ContentFileDAO();
            object recCount = cfd.VerifyYWAlreadyLinked(this.FileCodeName, where);
            if (int.Parse(recCount.ToString()) == 0)
            {
                MessageBox.Show("还未挂接原文，不能提交！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //sql0 = "IF OBJECT_ID(N'" + this.FileCodeName + "_workflow_rec" + "',N'U') IS NOT NULL \r\n";
            //sql0 += "  SELECT * FROM " + this.FileCodeName + "_workflow_rec WHERE " + where + " \r\n";
            //sql0 += "ELSE \r\n";
            //sql0 += "  SELECT 0";
            //dt0 = new DbHelper().Fill(sql0);
            //if (dt0.Rows.Count > 0)
            //{
            //    MessageBox.Show("之前曾提交过本批数据，不能重复提交！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            if (MessageBox.Show("一旦提交，数据将不能返回（到本流程节点）！ \r\n 确定提交本批数据吗？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                return;

            dt0 = dfd.GetDataFlowRoles();//获得流程中的角色，从流程的前到后
            string nextRoleId = string.Empty;
            int complete = 0;
            for (int i = 0; i < dt0.Rows.Count; i++)
            {
                if (dt0.Rows[i]["role_id"].ToString().Equals(this.UserLoggedIn.RoleId))
                {
                    if (i < dt0.Rows.Count - 1)
                    {
                        nextRoleId = dt0.Rows[i + 1]["role_id"].ToString();
                        break;
                    }
                    else
                    {
                        nextRoleId = this.UserLoggedIn.RoleId;
                        complete = 1;//标识流程本批数据的流程完结
                    }
                }
            }
            ManualCheckDAO mfd = new ManualCheckDAO();
            mfd.SummitWhenCompleteThisCheck(CreatingTableFields, this.FileCodeName, FieldStrWithoutAlias, where, nextRoleId, complete, dr["Unique_code"].ToString());//某个流程节点已经完成质检或验收，点提交按钮，记录工作流状态
            RefreshDatas();//更新显示列表
        }

        private void ShowYwForManualCheckLinkButton()
        {
            RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();
            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.ButtonClick += riButtonEdit_Yw_ManualCheck_ButtonClick;
            riButtonEdit.Buttons.Clear();
            EditorButton bt0 = new EditorButton();
            bt0.Kind = ButtonPredefines.Glyph;
            bt0.Caption = "原文";
            bt0.ToolTip = "点击查看针对此条目录的原文";
            bt0.Appearance.ForeColor = Color.Blue;
            bt0.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt0);
            GridColumn Col1 = new GridColumn();
            Col1.FieldName = "ywForManualCheck";
            Col1.Caption = "原文链接";
            Col1.Visible = false;
            Col1.VisibleIndex = gridView_manualCataloge_recordForCheck.Columns.Count;
            Col1.UnboundType = DevExpress.Data.UnboundColumnType.String;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_manualCataloge_recordForCheck.RepositoryItems.Add(riButtonEdit);
            gridView_manualCataloge_recordForCheck.Columns.Add(Col1);
        }
        private void ShowCollectErrorForManualCheckLinkButton()
        {
            RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();
            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.ButtonClick += riButtonEdit_CollectError_ManualCheck_ButtonClick;
            riButtonEdit.Buttons.Clear();
            EditorButton bt0 = new EditorButton();
            bt0.Kind = ButtonPredefines.Glyph;
            bt0.Caption = "标记为问题项";
            bt0.ToolTip = "标记并搜集本项的错误信息";
            bt0.Appearance.ForeColor = Color.Blue;
            bt0.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt0);
            GridColumn Col1 = new GridColumn();
            Col1.FieldName = "collectErrorForManualCheck";
            Col1.Caption = "问题标记链接";
            Col1.Visible = false;
            Col1.VisibleIndex = gridView_manualCataloge_recordForCheck.Columns.Count;
            Col1.UnboundType = DevExpress.Data.UnboundColumnType.String;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_manualCataloge_recordForCheck.RepositoryItems.Add(riButtonEdit);
            gridView_manualCataloge_recordForCheck.Columns.Add(Col1);
            foreach (GridColumn c in gridView_manualCataloge_recordForCheck.Columns)
            {
                c.OptionsColumn.AllowEdit = c.ColumnEdit is RepositoryItemButtonEdit;
            }
        }

        void riButtonEdit_Yw_ManualCheck_ButtonClick(object sender, EventArgs e)
        {
            int rowhandle = gridView_manualCataloge_recordForCheck.FocusedRowHandle;
            DataRow dr = gridView_manualCataloge_recordForCheck.GetDataRow(rowhandle);
            string uniquecode = dr["Unique_code"].ToString();
            if (this.ContentFormat.ToLower().Contains("pdf"))
            {
                string ywRootPath = ConfigurationManager.AppSettings["ContentFileRootPath"];
                ContentFileDAO cfd = new ContentFileDAO();
                object ywPathStr = cfd.VerifyYWAlreadyLinked2(uniquecode, this.FileCodeName);
                if (string.IsNullOrEmpty(ywPathStr.ToString()))
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
                dt = cfd.VerifyImageYWAlreadyLinked(this.FileCodeName, archiveNoFieldName, uniquecode);
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

        void riButtonEdit_CollectError_ManualCheck_ButtonClick(object sender, EventArgs e)
        {
            int rowhandle = gridView_manualCataloge_recordForCheck.FocusedRowHandle;
            DataRow dr = gridView_manualCataloge_recordForCheck.GetDataRow(rowhandle);
            string uniquecode = dr["Unique_code"].ToString();
            ArchiveNoDAO and = new ArchiveNoDAO();
            object dhfield = and.GetArchiveNoDataByCodeName2(this.FileCodeName);
            object dh = and.GetArchiveNoVale(dhfield, this.FileCodeName, uniquecode);
            textBox_manualCheck_dh.Text = dh.ToString();
            splitContainerControl_manualCheck.Collapsed = false;
        }

        private void button_manualcheck_save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_manualCheck_dh.Text))
            {
                MessageBox.Show("请点击“标记为问题项”选择某条疑似问题项！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(comboBoxEdit_errorType.SelectedItem.ToString()))
            {
                MessageBox.Show("请选择疑似问题类型！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ManualCheckDAO mfd = new ManualCheckDAO();
            //收集问题记录
            mfd.CollectErrorData(textBox_manualCheck_dh.Text, comboBoxEdit_errorType.SelectedItem.ToString(), textBox_manualCheck_errorDescription.Text, this.FileCodeName, DateTime.Now.ToString(), this.UserLoggedIn.UserName);
            textBox_manualCheck_dh.Text = string.Empty;
            textBox_manualCheck_errorDescription.Text = string.Empty;
            comboBoxEdit_errorType.SelectedIndex = -1;
            splitContainerControl_manualCheck.Collapsed = true;
            this.DtRowStyle = mfd.GetManualCheckedRecords(this.FileCodeName);
            //xtraPagedNavigation_recordForCheck.RefreshDataToGridView();
            MessageBox.Show("标记成功！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_manualcheck_cancel_Click(object sender, EventArgs e)
        {
            textBox_manualCheck_dh.Text = string.Empty;
            textBox_manualCheck_errorDescription.Text = string.Empty;
            comboBoxEdit_errorType.SelectedIndex = -1;
            splitContainerControl_manualCheck.Collapsed = true;
        }

        private void xtraTabControl_manualCheck_gotoCheck_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            xtraGoFrontBack1.UpdateBackButtonStateFun();
            xtraUpdateButtonForGrid1.EnableButton = xtraTabControl_manualCheck_gotoCheck.SelectedTabPageIndex == 0 ? true : false;
        }

        private void gridView_manualCataloge_shouldCheck_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView_manualCataloge_recordForCheck_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle > -1)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit1.Checked)
            {
                xtraTab_allData.Show();
            }
            else
            {
                xtraTab_manualCheck_shouldCheck.Show();
            }
        }

        private void gridView_allData_inWorkflow_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName.Equals("当前数据所在位置"))
            {
                DataTable dt = gridControl_allData_inWorkflow.DataSource as DataTable;
                if (dt != null)
                {
                    int index = gridView_allData_inWorkflow.GetDataSourceRowIndex(e.RowHandle);
                    DataRow dr = dt.Rows[index];

                    if (ValidateUtil.IsInt(dr["当前数据所在位置"].ToString()))
                    {
                        DataRow[] drRoles = this.DtRole.Select("Unique_code=" + dr["当前数据所在位置"].ToString());
                        if (drRoles.Length > 0)
                        {
                            DataRow drRole = drRoles[0];
                            gridView_allData_inWorkflow.SetRowCellValue(e.RowHandle, "当前数据所在位置", drRole["role_name"].ToString());
                        }
                    }
                }
            }
        }

        private void gridView_manualCataloge_recordForCheck_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            int hand = e.RowHandle;//行号
            if (hand < 0)
                return;

            DataRow dr = gridView_manualCataloge_recordForCheck.GetDataRow(hand);
            if (dr == null)
                return;

            //if (this.DtRowStyle == null)
            //{
            //string sql = "SELECT dh FROM t_archive_manual_check_rec WHERE code_name='" + this.FileCodeName + "'";
            //this.DtRowStyle = new DbHelper().Fill(sql);
            //}

            object dh = dr[this.ArchiveNoFieldName];
            if (this.DtRowStyle.Rows.Count > 0)
            {
                DataRow[] drs = this.DtRowStyle.Select("dh='" + dh.ToString() + "'");
                if (drs.Length > 0)
                {
                    e.Appearance.BackColor = Color.LightGreen;
                }
            }

        }
    }
}