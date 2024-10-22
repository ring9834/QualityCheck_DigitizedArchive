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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Xml.Serialization;
using System.IO;

namespace Prj_FileManageNCheckApp
{
    public partial class DataCountReport : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }
        public DataTable SearchDatatable { get; set; }
        public DataSet DS { get; set; }
        private RepositoryItemComboBox RepositoryItemComboBox1 { get; set; }
        private SerializableDictionary<string, StatisticsCondition> ConditionList { get; set; }
        private System.Windows.Forms.Timer TimerForControl { get; set; }
        private ReportParams RP { get; set; }
        public DataCountReport()
        {
            InitializeComponent();
        }

        public DataCountReport(string fileCodeName)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            TimerForControl = new System.Windows.Forms.Timer();
            TimerForControl.Interval = 100;
            TimerForControl.Enabled = true;
            TimerForControl.Tick += timer_Tick;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            PageControlLocation.MakeControlCenter(groupControl1.Parent, groupControl1);
            if (paras != null)
            {
                this.FileCodeName = paras.FileCodeName;
                this.RP = paras.RP;
            }
            if (treeListLookUpEdit1.Properties.DataSource != null)
            {
                LoadReportTemplateData();//模板切换时，加载模板数据
            }
        }

        private void DataCountReport_Load(object sender, EventArgs e)
        {
            this.ConditionList = new SerializableDictionary<string, StatisticsCondition>();
            this.DS = new DataSet();

            LoadFormDataDelegate ldd = new LoadFormDataDelegate(LoadTreeList);
            ldd.BeginInvoke(new AsyncCallback(LoadReportControlsDataCallback), null);//异步加载数据，加快窗口显示速度
            Thread.Sleep(100);
            LoadFormDataDelegate ldd2 = new LoadFormDataDelegate(LoadRepositoryComboBoxData);
            ldd2.BeginInvoke(new AsyncCallback(LoadReportControlsDataCallback2), null);
        }

        private delegate DataTable LoadFormDataDelegate();

        void LoadReportControlsDataCallback(IAsyncResult result)
        {
            LoadFormDataDelegate lddCallback = (LoadFormDataDelegate)((AsyncResult)result).AsyncDelegate;
            DataTable dt = lddCallback.EndInvoke(result);
            this.DS.Tables.Add(dt);
        }

        void LoadReportControlsDataCallback2(IAsyncResult result)
        {
            LoadFormDataDelegate lddCallback = (LoadFormDataDelegate)((AsyncResult)result).AsyncDelegate;
            DataTable dt = lddCallback.EndInvoke(result);
            this.DS.Tables.Add(dt);
        }

        private DataTable LoadTreeList()
        {
            StorehouseKindDAO shkd = new StorehouseKindDAO();
            DataTable dt = shkd.LoadStoreHouseTree2();
            dt.TableName = "ForTree";
            return dt;
        }

        private DataTable LoadRepositoryComboBoxData()
        {
            this.RepositoryItemComboBox1 = new RepositoryItemComboBox();
            ConfigCodeDAO ccd = new ConfigCodeDAO();
            DataTable dtcondtion = ccd.GetCodes("jstj");
            dtcondtion.TableName = "ForRepositive";
            return dtcondtion;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (this.DS.Tables.Count == 2)
            {
                treeListLookUpEdit1.Properties.DataSource = this.DS.Tables["ForTree"];
                //treeListLookUpEdit1.Properties.DisplayMemberKeyFieldName = "Unique_code";
                treeListLookUpEdit1.Properties.DisplayMember = "name";
                treeListLookUpEdit1.Properties.ValueMember = "code";
                treeListLookUpEdit1.Properties.TreeList.KeyFieldName = "Unique_code";
                treeListLookUpEdit1.Properties.TreeList.ParentFieldName = "super_id";

                DataTable dtcondtion = this.DS.Tables["ForRepositive"];
                for (int i = 0; i < dtcondtion.Rows.Count; i++)
                {
                    string name = dtcondtion.Rows[i]["code_name"].ToString();
                    string value = dtcondtion.Rows[i]["code_value"].ToString();
                    int uniquecode = int.Parse(dtcondtion.Rows[i]["Unique_code"].ToString());
                    ComboBoxItem combItem = new ComboBoxItem(new SearchConditionEntity(name, value, uniquecode));
                    RepositoryItemComboBox1.Items.Add(combItem);
                }
                dtcondtion.Dispose();
                RepositoryItemComboBox1.TextEditStyle = TextEditStyles.DisableTextEditor;
                RepositoryItemComboBox1.SelectedIndexChanged += itemBox_SelectedIndexChanged;

                TimerForControl.Enabled = false;
                TimerForControl.Dispose();

                LoadReportTemplateData();
                ShowReportTypes();
            }
        }

        void LoadReportTemplateData()
        {
            if (this.RP != null)
            {
                this.ConditionList = RP.CollectionBoxList;
                SerializableDictionary<string, StatisticsCondition> sd = this.RP.CollectionBoxList;
                if (sd != null)
                {
                    checkedListBoxControl_groupedCondition.Items.Clear();
                    for (int i = 0; i < sd.Count; i++)
                    {
                        string codeName = sd.Keys.ElementAt(i);
                        DataTable dt = treeListLookUpEdit1.Properties.DataSource as DataTable;
                        DataRow dr = null;
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            if (dt.Rows[j]["code"].ToString().Equals(codeName))
                            {
                                dr = dt.Rows[j];
                                break;
                            }
                        }
                        if (dr != null)
                        {
                            string showName = dr["name"].ToString() + "报表条件";
                            CheckedListBoxItem item = new CheckedListBoxItem(codeName, showName);//添加至报表条件收集箱
                            checkedListBoxControl_groupedCondition.Items.Add(item);
                        }
                    }
                    if (checkedListBoxControl_groupedCondition.Items.Count > 0)
                    {
                        checkedListBoxControl_groupedCondition.SelectedIndex = 0;
                    }
                    radioGroup1.SelectedIndex = RP.OnlyCount ? 1 : 0;
                    radioGroup2.SelectedIndex = RP.LandScape ? 1 : 0;
                    comboBoxEdit1.Text = RP.ContentFontSize;
                    comboBoxEdit3.Text = RP.TitleFontSize;
                    comboBoxEdit2.Text = RP.ColumnCount;
                    textEdit1.Text = RP.ReportTitle;
                    checkEdit1.Checked = RP.ShowPageNumber;
                    checkEdit2.Checked = RP.ShowBorder;
                    checkEdit3.Checked = RP.ShowDateTime;
                }
            }
        }

        void ShowReportTypes()
        {
            //显示报表类型类表，如数量统计、类型统计等
            if (comboBoxEdit_TJLX.Properties.Items.Count == 0)
            {
                ConfigCodeDAO ccd = new ConfigCodeDAO();
                DataTable dt_tjlx = ccd.GetCodes("TJLX");
                comboBoxEdit_TJLX.Properties.Items.Clear();
                for (int i = 0; i < dt_tjlx.Rows.Count; i++)
                {
                    DataRow dr = dt_tjlx.Rows[i];
                    AttachedCodeClass fst = new AttachedCodeClass(dr["code_name"].ToString(), dr["code_value"].ToString(), dr["Unique_code"].ToString());
                    comboBoxEdit_TJLX.Properties.Items.Add(fst);
                }
                dt_tjlx.Dispose();
            }
        }

        private void treeListLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            DataRowView dr = (DataRowView)treeListLookUpEdit1.GetSelectedDataRow();
            if (dr != null)
            {
                string codeName = dr["code"].ToString();
                LoadInitialDataToCheckListBox(codeName);
            }
            if (this.SearchDatatable == null)
            {
                this.SearchDatatable = new DataTable();
                this.SearchDatatable.TableName = "SearchCondtionTable";//Datatable序列化必须要设置表名
                this.SearchDatatable.Columns.Add("Unique_code", typeof(string));
                this.SearchDatatable.Columns.Add("col_name", typeof(string));
                this.SearchDatatable.Columns.Add("已选字段", typeof(string));
                this.SearchDatatable.Columns.Add("SearchCondition", typeof(SearchConditionEntity));
                this.SearchDatatable.Columns.Add("值", typeof(object));
            }
            this.SearchDatatable.Rows.Clear();
            checkedListBoxControl_fieldsSelected.Items.Clear();
            checkedListBoxControl_groupFields.Items.Clear();
            this.FileCodeName = string.Empty;
        }

        private void LoadInitialDataToCheckListBox(string codeName)
        {
            DataDictionaryDAO ddd = new DataDictionaryDAO();
            DataTable dtFromDB = ddd.GetColNameShowNameByCodeName(codeName);
            checkedListBoxControl_fieldForSelect.Items.Clear();
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBoxControl_fieldForSelect.Items.Add(item);
            }
            dtFromDB.Dispose();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (checkedListBoxControl_fieldForSelect.Items.Count == 0)
            {
                MessageBox.Show("没有可供选择的字段！请确认是否选择了档案类型库。", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //if (radioButton1.Checked)
            //{
            //List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl_fieldForSelect.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_fieldForSelect.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    DataRow row = this.SearchDatatable.NewRow();
                    row["Unique_code"] = item.Tag;
                    row["col_name"] = item.Value;
                    row["已选字段"] = item.Description;
                    this.SearchDatatable.Rows.Add(row);
                    item.CheckState = CheckState.Unchecked;
                    //list.Add(item);
                }
            }
            //for (int i = 0; i < list.Count; i++)
            //{
            //    checkedListBoxControl1.Items.Remove(list[i]);
            //}
            //list = null;

            BindGridControl();
            //}
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (gridView1.DataRowCount == 0)
                return;
            //if (!radioButton1.Checked && !radioButton2.Checked && !radioButton3.Checked)
            //{
            //    MessageBox.Show("请选择“配置搜索项”，或“配置被统计字段”，或“配置分组字段”！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            //if (radioButton1.Checked)
            //{
            int index = gridView1.GetDataSourceRowIndex(gridView1.FocusedRowHandle);
            //CheckedListBoxItem item = new CheckedListBoxItem();
            //item.Description = this.SearchDatatable.Rows[index]["已选字段"].ToString();
            //item.Value = this.SearchDatatable.Rows[index]["col_name"];
            //item.Tag = this.SearchDatatable.Rows[index]["Unique_code"];
            //checkedListBoxControl1.Items.Add(item);
            this.SearchDatatable.Rows.RemoveAt(index);
            BindGridControl();
            //}
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            if (this.SearchDatatable == null)
            {
                MessageBox.Show("请配置好“搜索项”、“被统计字段”和“分组字段”后继续！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (this.SearchDatatable.Rows.Count == 0)
            {
                MessageBox.Show("请至少选择一个统计字段，然后继续！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataRowView drv = (DataRowView)treeListLookUpEdit1.GetSelectedDataRow();
            string codeName = drv["code"].ToString();
            string showName = drv["name"].ToString() + "报表条件";
            if (string.IsNullOrEmpty(this.FileCodeName))
                this.FileCodeName = codeName;

            //报表搜索条件
            List<ReportSearchConditionEntity> rcList = new List<ReportSearchConditionEntity>();
            for (int i = 0; i < this.SearchDatatable.Rows.Count; i++)
            {
                DataRow dr = this.SearchDatatable.Rows[i];
                ReportSearchConditionEntity rce = new ReportSearchConditionEntity();
                rce.FieldName = dr["col_name"].ToString();
                object condtion = dr["SearchCondition"];
                object value = dr["值"];
                if (condtion == DBNull.Value || condtion == null)
                {
                    MessageBox.Show("有搜索项的搜索条件未配置，请配置后继续！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (value == DBNull.Value || value == null)
                {
                    MessageBox.Show("有搜索项的搜索“值”未配置，请配置后继续！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                rce.SearchCondition = ((SearchConditionEntity)condtion).Value.ToString();
                rce.Value = value.ToString();
                rcList.Add(rce);
            }

            //被统计字段
            if (this.checkedListBoxControl_fieldsSelected.Items.Count == 0)
            {
                MessageBox.Show("还没有选择被统计字段，请选择后继续！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            List<string> sfs = new List<string>();
            for (int i = 0; i < checkedListBoxControl_fieldsSelected.Items.Count; i++)
                sfs.Add(checkedListBoxControl_fieldsSelected.Items[i].Value.ToString());

            //分组字段
            if (this.checkedListBoxControl_groupFields.Items.Count == 0)
            {
                MessageBox.Show("还没有选择分组字段，请选择后继续！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            List<string> gfs = new List<string>();
            for (int i = 0; i < checkedListBoxControl_groupFields.Items.Count; i++)
                gfs.Add(checkedListBoxControl_groupFields.Items[i].Value.ToString());

            //某档案类型库的报表条件，在下面会被加到报表搜集箱中
            StatisticsCondition sc = new StatisticsCondition();
            sc.ReportConditionList = rcList;
            sc.SearchConditionTable = this.SearchDatatable.Copy();
            sc.StatisticsFieldsList = sfs;
            sc.GroupFieldsList = gfs;

            IEnumerable<KeyValuePair<string, StatisticsCondition>> results = this.ConditionList.Where(r => r.Key.Equals(this.FileCodeName));
            if (results.Count() > 0)
            {
                if (MessageBox.Show("“" + showName + "”的报表条件已在收集箱！需要更新吗？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    KeyValuePair<string, StatisticsCondition> result = results.ElementAt(0);
                    this.ConditionList.Remove(result.Key);//若收集箱中已存在，则更新其值
                    this.ConditionList.Add(this.FileCodeName, sc);
                }
            }
            else
            {
                CheckedListBoxItem item = new CheckedListBoxItem(codeName, showName);//添加至报表条件收集箱
                checkedListBoxControl_groupedCondition.Items.Add(item);
                this.ConditionList.Add(codeName, sc);
            }

            //供设置报表的一些参数如字体大小、列数等
            if (comboBoxEdit1.Properties.Items.Count == 0)
            {
                for (int i = 6; i <= 30; i++)
                {
                    comboBoxEdit1.Properties.Items.Add(new ComboboxItem(i.ToString(), i.ToString()));//内容字体大小
                    comboBoxEdit3.Properties.Items.Add(new ComboboxItem(i.ToString(), i.ToString()));//报表标题字体大小
                    i++;
                }
                for (int i = 1; i <= 30; i++)
                {
                    comboBoxEdit2.Properties.Items.Add(new ComboboxItem(i.ToString(), i.ToString()));
                }
            }
            comboBoxEdit1.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            comboBoxEdit2.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            comboBoxEdit3.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        }

        private void BindGridControl()
        {
            if (SearchDatatable != null)
            {
                gridControl1.DataSource = SearchDatatable;
                gridView1.PopulateColumns();
                gridView1.Columns["Unique_code"].Visible = false;
                gridView1.Columns["col_name"].Visible = false;
                gridView1.Columns["SearchCondition"].Visible = false;

                GridColumn Col2 = new GridColumn();
                Col2.FieldName = "SearchCondition";
                Col2.Caption = "搜索条件";
                Col2.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                Col2.ColumnEdit = RepositoryItemComboBox1;
                gridView1.Columns.Add(Col2);
                gridView1.Columns["已选字段"].OptionsColumn.AllowEdit = false;
                Col2.VisibleIndex = 1;
                gridView1.Columns["值"].VisibleIndex = 2;
            }
        }

        void itemBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchConditionEntity obj = (SearchConditionEntity)((ComboBoxEdit)sender).SelectedItem;
            int index = gridView1.GetDataSourceRowIndex(gridView1.FocusedRowHandle);
            this.SearchDatatable.Rows[index]["SearchCondition"] = obj;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (checkedListBoxControl_groupedCondition.Items.Count == 0)
            {
                MessageBox.Show("报表条件收集箱为空，报表生成失败！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ReportParams rp = new ReportParams();
            rp.OnlyCount = radioGroup1.SelectedIndex == 1;
            rp.LandScape = radioGroup2.SelectedIndex == 1;
            rp.CollectionBoxList = this.ConditionList;
            rp.ColumnCount = string.IsNullOrEmpty(comboBoxEdit2.Text) ? "5" : comboBoxEdit2.Text;
            rp.ContentFontSize = string.IsNullOrEmpty(comboBoxEdit1.Text) ? "12" : comboBoxEdit1.Text;
            rp.TitleFontSize = string.IsNullOrEmpty(comboBoxEdit3.Text) ? "18" : comboBoxEdit3.Text;
            rp.ReportTitle = string.IsNullOrEmpty(textEdit1.Text) ? "统计报表" : textEdit1.Text;
            rp.ShowBorder = checkEdit2.Checked;
            rp.ShowDateTime = checkEdit3.Checked;
            rp.ShowPageNumber = checkEdit1.Checked;

            //记录当前收集箱中的报表条件到数据库，以后供用户作为模板使用
            if (checkEdit_tjlx.Checked)
            {
                if (comboBoxEdit_TJLX.SelectedIndex >= 0)
                {
                    string reportType = ((AttachedCodeClass)comboBoxEdit_TJLX.SelectedItem).CodeValue;
                    string xml = SerializeObjectToXml(rp);
                    ReportDAO rd = new ReportDAO();
                    rd.SaveReportTemplate(reportType, xml);//保存为报表模板
                }
            }

            new XtraDataReport(rp);//显示报表预览
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl_groupedCondition.Items.Count; i++)
            {
                if (checkedListBoxControl_groupedCondition.Items[i].CheckState == CheckState.Checked)
                {
                    CheckedListBoxItem item = (CheckedListBoxItem)checkedListBoxControl_groupedCondition.Items[i];
                    this.ConditionList.Remove(item.Value.ToString());
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBoxControl_groupedCondition.Items.Remove(list[i]);
            }
            list = null;
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            if (checkedListBoxControl_fieldForSelect.Items.Count == 0)
            {
                MessageBox.Show("没有可供选择的字段！请确认是否选择了档案类型库。", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //if (radioButton2.Checked)
            //{
            for (int i = 0; i < checkedListBoxControl_fieldForSelect.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_fieldForSelect.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    if (checkedListBoxControl_fieldsSelected.Items.Contains(item))//不能重复选择被统计字段
                        return;
                    checkedListBoxControl_fieldsSelected.Items.Add(item);
                    item.CheckState = CheckState.Unchecked;
                }
            }
            //}
        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {
            if (checkedListBoxControl_fieldForSelect.Items.Count == 0)
            {
                MessageBox.Show("没有可供选择的字段！请确认是否选择了档案类型库。", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //if (radioButton3.Checked)
            //{
            for (int i = 0; i < checkedListBoxControl_fieldForSelect.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_fieldForSelect.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    if (checkedListBoxControl_groupFields.Items.Contains(item))//不能重复选择被统计字段
                        return;
                    checkedListBoxControl_groupFields.Items.Add(item);
                    item.CheckState = CheckState.Unchecked;
                }
            }
            //}
        }

        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl_fieldsSelected.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_fieldsSelected.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    item.CheckState = CheckState.Unchecked;
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBoxControl_fieldsSelected.Items.Remove(list[i]);
            }
            list = null;
        }

        private void simpleButton7_Click(object sender, EventArgs e)
        {
            List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl_groupFields.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBoxControl_groupFields.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    item.CheckState = CheckState.Unchecked;
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBoxControl_groupFields.Items.Remove(list[i]);
            }
            list = null;
        }

        private string SerializeObjectToXml(ReportParams obj)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ReportParams));
            MemoryStream ms = new MemoryStream();
            ser.Serialize(ms, obj);
            string xmlString = Encoding.UTF8.GetString(ms.ToArray());
            return xmlString;
        }

        private ReportParams DeSerializeXmlToObject(string xmlString)
        {
            XmlSerializer dser = new XmlSerializer(typeof(ReportParams));
            //xmlString是你从数据库获取的字符串
            Stream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));
            ReportParams rp = dser.Deserialize(xmlStream) as ReportParams;
            return rp;

        }

        private void comboBoxEdit_TJLX_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit_TJLX.SelectedIndex >= 0)
                checkEdit_tjlx.CheckState = CheckState.Checked;
            else
                checkEdit_tjlx.CheckState = CheckState.Unchecked;
        }

        private void checkedListBoxControl_groupedCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckedListBoxItem item = (CheckedListBoxItem)checkedListBoxControl_groupedCondition.SelectedItem;
            if (item != null)
            {
                string codeName = item.Value.ToString();
                this.FileCodeName = codeName;

                if (this.ConditionList != null)
                {
                    IEnumerable<KeyValuePair<string, StatisticsCondition>> kvps = this.ConditionList.Where(r => r.Key.Equals(codeName));
                    if (kvps.Count() > 0)
                    {
                        //在treeListLookUpEdit1中选择对应的项以显示出了
                        treeListLookUpEdit1.DataBindings.Clear();
                        DataTable dtFromTree = treeListLookUpEdit1.Properties.DataSource as DataTable;
                        DataRow dr = null;
                        for (int i = 0; i < dtFromTree.Rows.Count; i++)
                        {
                            string code = dtFromTree.Rows[i]["code"].ToString();
                            if (this.FileCodeName.Equals(code))
                            {
                                dr = dtFromTree.Rows[i];
                                break;
                            }
                        }
                        if (dr != null)
                        {
                            DataTable dtForBinding = new DataTable();
                            dtForBinding.PrimaryKey = new DataColumn[] { dtForBinding.Columns["code"] };
                            dtForBinding.Columns.Add("code");
                            dtForBinding.Columns.Add("name");
                            dtForBinding.Rows.Add(dr["code"], dr["name"]);
                            treeListLookUpEdit1.DataBindings.Add(new Binding("EditValue", dtForBinding, "code"));
                        }

                        //加载搜索条件
                        KeyValuePair<string, StatisticsCondition> kvp = kvps.ElementAt(0);
                        StatisticsCondition sc = kvp.Value;
                        this.SearchDatatable = sc.SearchConditionTable.Copy();
                        BindGridControl();

                        //LoadInitialDataToCheckListBox(codeName);//加载数据到checkedListBoxControl_fieldForSelect
                        //加载被统计字段
                        checkedListBoxControl_fieldsSelected.Items.Clear();
                        for (int i = 0; i < sc.StatisticsFieldsList.Count; i++)
                        {
                            IEnumerable<CheckedListBoxItem> lbs = checkedListBoxControl_fieldForSelect.Items.Where(t => t.Value.ToString().Equals(sc.StatisticsFieldsList[i]));
                            if (lbs.Count() > 0)
                                checkedListBoxControl_fieldsSelected.Items.Add(lbs.ElementAt(0));
                        }

                        //加载分组字段
                        checkedListBoxControl_groupFields.Items.Clear();
                        for (int i = 0; i < sc.GroupFieldsList.Count; i++)
                        {
                            IEnumerable<CheckedListBoxItem> lbs = checkedListBoxControl_fieldForSelect.Items.Where(t => t.Value.ToString().Equals(sc.GroupFieldsList[i]));
                            if (lbs.Count() > 0)
                                checkedListBoxControl_groupFields.Items.Add(lbs.ElementAt(0));
                        }
                    }
                }
            }
        }

        private void simpleButton9_Click(object sender, EventArgs e)
        {
            if (checkedListBoxControl_groupedCondition.Items.Count == 0)
            {
                MessageBox.Show("报表条件收集箱为空，报表生成失败！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ReportParams rp = new ReportParams();
            rp.OnlyCount = radioGroup1.SelectedIndex == 1;
            rp.LandScape = radioGroup2.SelectedIndex == 1;
            rp.CollectionBoxList = this.ConditionList;
            rp.ColumnCount = string.IsNullOrEmpty(comboBoxEdit2.Text) ? "5" : comboBoxEdit2.Text;
            rp.ContentFontSize = string.IsNullOrEmpty(comboBoxEdit1.Text) ? "12" : comboBoxEdit1.Text;
            rp.TitleFontSize = string.IsNullOrEmpty(comboBoxEdit3.Text) ? "18" : comboBoxEdit3.Text;
            rp.ReportTitle = string.IsNullOrEmpty(textEdit1.Text) ? "统计报表" : textEdit1.Text;
            rp.ShowBorder = checkEdit2.Checked;
            rp.ShowDateTime = checkEdit3.Checked;
            rp.ShowPageNumber = checkEdit1.Checked;

            //记录当前收集箱中的报表条件到数据库，以后供用户作为模板使用
            if (checkEdit_tjlx.Checked)
            {
                if (comboBoxEdit_TJLX.SelectedIndex >= 0)
                {
                    string reportType = ((AttachedCodeClass)comboBoxEdit_TJLX.SelectedItem).CodeValue;
                    string xml = SerializeObjectToXml(rp);
                    ReportDAO rd = new ReportDAO();
                    rd.SaveReportTemplate(reportType, xml);
                }
            }
            saveFileDialog1.Filter = "PDF文件(*.pdf)|*.pdf|Excel文件(*.xls)|*.xls|Excel文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
                new XtraDataReport(rp, fileName);//显示报表预览
            }
        }
    }
}