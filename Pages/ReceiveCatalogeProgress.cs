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
using System.Xml;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System.Data.Common;

namespace Prj_FileManageNCheckApp
{
    public partial class ReceiveCatalogeProgress : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        protected Dictionary<string, KeyValuePair<string, string>> MatchedResult { get; set; }
        protected CatalogRecord CR = new CatalogRecord();
        public UserEntity UserLoggedIn { get; set; }
        protected string WhereCondtionForAchiveNumMakeup { get; set; }
        public ReceiveCatalogeProgress()
        {
            InitializeComponent();
        }

        public ReceiveCatalogeProgress(string fileCodeName, UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            if (xtraTabControl_ReceiveData.SelectedTabPageIndex == 0)
            {
                PageControlLocation.MakeControlCenter(panelControl1.Parent, panelControl1);
            }
            if (xtraTabControl_ReceiveData.SelectedTabPageIndex == 1)
            {
                LoadDataToCheckedListBoxs();
            }
            if (xtraTabControl_ReceiveData.SelectedTabPageIndex == 2)
            {
                ShowCatalogeAcceptedRecords();
            }
        }

        private void ReceiveCatalogeProgress_Load(object sender, EventArgs e)
        {
            simpleButton_generate_dh.Enabled = false;
            xtraTabControl_ReceiveData.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            xtraGoFrontBack1.Parent = standaloneBarDockControl1;
            xtraGoFrontBack1.TabControl = xtraTabControl_ReceiveData;
            xtraGoFrontBack1.UpdateBackButtonStateFun = new UpdateBackButtonState(xtraGoFrontBack1.UpdateControls);
            xtraGoFrontBack1.UpdateBackButtonStateFun();

            xtraDeleteButtonForGrid1.Parent = standaloneBarDockControl1;
            xtraDeleteButtonForGrid1.TargetGrid = gridView_cataloge_acceptrecord;
            xtraDeleteButtonForGrid1.IsOneRowSelectedFunc = new IsOneRowSelected(IsOneRowSelectedInGridFunc);
            xtraDeleteButtonForGrid1.RefreshDataInGridViewFunc = new RefreshDataInGridView(ShowCatalogeAcceptedRecords);
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
            string sql = "DELETE FROM t_config_imp_catalogs_rec WHERE Unique_code=" + dr["批次号"].ToString() + "\r\n";
            sql += "DELETE FROM " + this.FileCodeName + " WHERE import_bundle='" + dr["批次号"].ToString() + "'";
            xtraDeleteButtonForGrid1.SQL = sql;
            xtraDeleteButtonForGrid1.AlertInformation = "导入的目录也将被删除，确认吗？";
            return true;
        }

        private void LoadDataToCheckedListBoxs()
        {
            this.MatchedResult = null;
            checkedListBoxControl1.Items.Clear();
            DataTable dt = ExcelHelper.GetDataHeader(textBox_content_browser.Text, 0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                checkedListBoxControl1.Items.Add(dt.Columns[i].ColumnName);
            }
            dt.Dispose();

            //读取this.FileCodeName对应的数据库表
            checkedListBoxControl2.Items.Clear();
            DataDictionaryDAO ddd = new DataDictionaryDAO();
            DataTable dtFromDB = ddd.GetColNameShowNameByCodeName(this.FileCodeName);

            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                checkedListBoxControl2.Items.Add(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString());
            }

            this.CR.Table_Code = this.FileCodeName;
            this.CR.Excel_File_Name = this.openFileDialog1.SafeFileName;

            dtFromDB.Dispose();
            xtraTab_cataloge_match.Show();
            checkedListBoxControl3.Items.Clear();
        }

        private void checkedListBoxControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBoxControl1.Items.Count; i++)
            {
                int index = checkedListBoxControl1.SelectedIndex;
                if (i == index && checkedListBoxControl1.Items[index].CheckState == CheckState.Checked) { }
                else
                {
                    checkedListBoxControl1.Items[i].CheckState = CheckState.Unchecked;
                }
            }
        }

        private void checkedListBoxControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBoxControl2.Items.Count; i++)
            {
                int index = checkedListBoxControl2.SelectedIndex;
                if (i == index && checkedListBoxControl2.Items[index].CheckState == CheckState.Checked) { }
                else
                {
                    checkedListBoxControl2.Items[i].CheckState = CheckState.Unchecked;
                }
            }
        }

        private void button_cataloge_automatch_Click(object sender, EventArgs e)
        {
            if (this.MatchedResult == null)
            {
                this.MatchedResult = new Dictionary<string, KeyValuePair<string, string>>();
            }

            List<DevExpress.XtraEditors.Controls.CheckedListBoxItem> items1 = new List<DevExpress.XtraEditors.Controls.CheckedListBoxItem>();
            List<DevExpress.XtraEditors.Controls.CheckedListBoxItem> items2 = new List<DevExpress.XtraEditors.Controls.CheckedListBoxItem>();

            for (int i = 0; i < checkedListBoxControl1.Items.Count; i++)
            {
                for (int j = 0; j < checkedListBoxControl2.Items.Count; j++)
                {
                    DevExpress.XtraEditors.Controls.CheckedListBoxItem item1 = checkedListBoxControl1.Items[i];
                    DevExpress.XtraEditors.Controls.CheckedListBoxItem item2 = checkedListBoxControl2.Items[j];
                    if (item2.Description.Equals(item1.Value.ToString()))
                    {
                        string matchString = "           " + GetStringWithBlanks(item1.Value.ToString()) + item2.Description;
                        KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(item2.Value.ToString(), item2.Description);
                        this.MatchedResult.Add(item1.Value.ToString(), kvp);
                        DevExpress.XtraEditors.Controls.CheckedListBoxItem item3 = new DevExpress.XtraEditors.Controls.CheckedListBoxItem();
                        item3.Tag = item1.Value.ToString();
                        item3.Value = matchString;
                        checkedListBoxControl3.Items.Add(item3);
                        items1.Add(item1);
                        items2.Add(item2);
                    }
                }
            }

            for (int i = 0; i < items1.Count; i++)
            {
                checkedListBoxControl1.Items.Remove(items1[i]);
            }

            for (int i = 0; i < items2.Count; i++)
            {
                checkedListBoxControl2.Items.Remove(items2[i]);
            }
            items1 = null; items2 = null;
        }

        private string GetStringWithBlanks(string str)
        {
            string result = str;
            for (int j = str.Length; j < 10; j++)
            {
                result = result + "一";
            }
            return result;
        }

        private void button_cataloge_bind_Click(object sender, EventArgs e)
        {
            if (checkedListBoxControl1.CheckedItems.Count == 0)
            {
                MessageBox.Show(this, "请选择一个源字段", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (checkedListBoxControl2.CheckedItems.Count == 0)
            {
                MessageBox.Show(this, "请选择一个目标字段", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (this.MatchedResult == null)
            {
                this.MatchedResult = new Dictionary<string, KeyValuePair<string, string>>();
            }


            DevExpress.XtraEditors.Controls.CheckedListBoxItem item1 = null;
            DevExpress.XtraEditors.Controls.CheckedListBoxItem item2 = null;

            for (int i = 0; i < checkedListBoxControl1.Items.Count; i++)
            {
                if (checkedListBoxControl1.Items[i].CheckState == CheckState.Checked)
                {
                    item1 = checkedListBoxControl1.Items[i];
                }
            }
            for (int i = 0; i < checkedListBoxControl2.Items.Count; i++)
            {
                if (checkedListBoxControl2.Items[i].CheckState == CheckState.Checked)
                {
                    item2 = checkedListBoxControl2.Items[i];
                }
            }
            string matchString = "           " + GetStringWithBlanks(item1.Value.ToString()) + item2.Description;
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(item2.Value.ToString(), item2.Description);
            this.MatchedResult.Add(item1.Value.ToString(), kvp);
            DevExpress.XtraEditors.Controls.CheckedListBoxItem item3 = new DevExpress.XtraEditors.Controls.CheckedListBoxItem();
            item3.Tag = item1.Value.ToString();
            item3.Value = matchString;
            checkedListBoxControl3.Items.Add(item3);
            checkedListBoxControl1.Items.Remove(item1);
            checkedListBoxControl2.Items.Remove(item2);
        }

        private void button_cataloge_restore_Click(object sender, EventArgs e)
        {
            List<DevExpress.XtraEditors.Controls.CheckedListBoxItem> items3 = new List<DevExpress.XtraEditors.Controls.CheckedListBoxItem>();
            for (int i = 0; i < checkedListBoxControl3.Items.Count; i++)
            {
                if (checkedListBoxControl3.Items[i].CheckState == CheckState.Checked)
                {
                    string key = checkedListBoxControl3.Items[i].Tag.ToString();
                    KeyValuePair<string, string> kvp = this.MatchedResult.Where(s => s.Key == key).FirstOrDefault().Value;
                    checkedListBoxControl1.Items.Add(key);
                    checkedListBoxControl2.Items.Add(kvp.Key, kvp.Value);
                    this.MatchedResult.Remove(key);
                    items3.Add(checkedListBoxControl3.Items[i]);
                    //checkedListBoxControl3.Items.RemoveAt(i);
                }
            }

            for (int i = 0; i < items3.Count; i++)
            {
                checkedListBoxControl3.Items.Remove(items3[i]);
            }
            items3 = null;
        }

        private void button_cataloge_allrestore_Click(object sender, EventArgs e)
        {
            checkedListBoxControl3.Items.Clear();
            checkedListBoxControl1.Items.Clear();
            this.MatchedResult.Clear();
            if (ExcelHelper.HaveNullHeader(textBox_content_browser.Text, 0))
            {
                MessageBox.Show("选择的目录文件中有无用的空表头（或空列），请检查并去掉!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataTable dt = ExcelHelper.GetDataHeader(textBox_content_browser.Text, 0);

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                checkedListBoxControl1.Items.Add(dt.Columns[i].ColumnName);
            }
            dt.Dispose();

            //读取this.FileCodeName对应的数据库表

            checkedListBoxControl2.Items.Clear();
            DataDictionaryDAO ddd = new DataDictionaryDAO();
            DataTable dtFromDB = ddd.GetColNameShowNameByCodeName(this.FileCodeName);
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                checkedListBoxControl2.Items.Add(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString());
            }
            dtFromDB.Dispose();
        }

        private void checkBox1_cataloge_config_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1_cataloge_config.Checked)
            {
                textBox_cataloge_config.Visible = true;
                //label_cataloge_config.Visible = true;
            }
            else
            {
                textBox_cataloge_config.Visible = false;
                //label_cataloge_config.Visible = false;
            }
        }

        private void button_cataloge_matchnext_Click(object sender, EventArgs e)
        {
            if (checkBox1_cataloge_config.Checked)
            {
                if (string.IsNullOrEmpty(textBox_cataloge_config.Text))
                {
                    MessageBox.Show("要存储此配置，请输入配置名称！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (checkedListBoxControl1.Items.Count > 0)
            {
                MessageBox.Show("可选字段中还有一个字段未匹配，请匹配后再继续！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("将要把EXCEL文件中的记录全部导入到数据库，确定吗？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                return;

            DataTable tableFromExcel = ExcelHelper.GetDataTable(this.openFileDialog1.FileName, 0);
            if (tableFromExcel != null)
            {
                if (tableFromExcel.Rows.Count == 0)
                {
                    MessageBox.Show("文件中目录条数为0，接收已终止！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                for (int i = 0; i < tableFromExcel.Columns.Count; i++)
                {
                    string colName = tableFromExcel.Columns[i].ColumnName;
                    if (!string.IsNullOrEmpty(this.MatchedResult.Keys.Where(s => s == colName).FirstOrDefault()))//更换列名，与数据库表中的列名对应
                        tableFromExcel.Columns[i].ColumnName = this.MatchedResult.Where(s => s.Key == colName).FirstOrDefault().Value.Key;
                }

                bool flag = VerifyIsCatalogAccedptedOverAgain(tableFromExcel);
                if (flag)//有重复
                {
                    if (MessageBox.Show("相同或相似的数据曾被接收过一批！允许重复接收，但重复接收的数据批次可根据需要在接收记录中手动删除。确认接收吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                        return;
                }

                flag = CheckExcelContentValid(tableFromExcel, progressBarControl1);//检查EXCEL内容合法性
                if (flag)//报错，就返回
                    return;

                this.CR.Import_Time = DateTime.Now.ToString();
                this.CR.Record_Number = (tableFromExcel.Rows.Count - 1).ToString();
                if (checkBox1_cataloge_config.Checked)
                {
                    this.CR.Config_Name = textBox_cataloge_config.Text;
                    this.CR.Config_XML = GetConfigXML();
                }
                ReceiveCatalogeDAO rcd = new ReceiveCatalogeDAO();//保存导入目录的历史记录
                object bundleNumber = rcd.SaveImportCatalogeRecord(this.CR.Table_Code, this.CR.Excel_File_Name, this.CR.Import_Time, this.CR.Import_User, this.CR.Record_Number, this.CR.Config_Name, this.CR.Config_XML);
                tableFromExcel.Columns.Add("import_bundle");
                for (int i = 0; i < tableFromExcel.Rows.Count; i++)//更新列值
                    tableFromExcel.Rows[i]["import_bundle"] = bundleNumber;

                tableFromExcel.TableName = this.FileCodeName;//对应的表名      
                tableFromExcel.Rows.RemoveAt(0);//表头去掉
                new SqlHelper().SqlBulkCopyData(tableFromExcel);//效率非常高，简直秒杀啊！            
                MessageBox.Show(tableFromExcel.Rows.Count + "条目录已接收成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tableFromExcel.Dispose();

                ShowCatalogeAcceptedRecords();
                xtraTab_cataloge_acceptRecord.Show();
                HinttoCreateArchiveNumber(bundleNumber.ToString());//提示生成档号
            }
        }

        bool VerifyIsCatalogAccedptedOverAgain(DataTable dt)
        {
            ReceiveCatalogeDAO rcd = new ReceiveCatalogeDAO();
            object count = rcd.VerifyIsCatalogueAcceptedAgain(dt, this.FileCodeName);
            if (int.Parse(count.ToString()) > 0)
                return true;
            return false;
        }

        private bool CheckExcelContentValid(DataTable tableFromExcel, ProgressBarControl progress)
        {
            //检查个字段是否超长或是否含有“'”字符。
            ReceiveCatalogeDAO rcd = new ReceiveCatalogeDAO();
            DataTable dtCheck = rcd.GetColumnLengthInfoByFileCodeName(this.FileCodeName);//取得某表格各列定义的允许长度
            progress.Visible = true;
            bool flag = false;
            progress.Properties.Minimum = 0;
            progress.Properties.Maximum = 100;
            progress.Properties.Step = 1;
            progress.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
            progress.Position = 0;
            progress.Properties.ShowTitle = true;
            progress.Properties.PercentView = true;
            progress.Text = "正在检查文件中各条信息是否超长...";
            for (int i = 1; i < tableFromExcel.Rows.Count; i++)
            {
                for (int j = 0; j < dtCheck.Rows.Count; j++)
                {
                    for (int k = 0; k < tableFromExcel.Columns.Count; k++)
                    {
                        string colName = tableFromExcel.Columns[k].ColumnName;
                        if (dtCheck.Rows[j]["name"].ToString().Equals(colName))
                        {
                            string fieldType = dtCheck.Rows[j]["fieldType"].ToString();
                            if (!fieldType.Equals("numberic") && !fieldType.Equals("int"))
                            {
                                int realLength = tableFromExcel.Rows[i][colName].ToString().Length;
                                int principleLength = int.Parse(dtCheck.Rows[j]["fieldLenth"].ToString());
                                if (realLength * 2 > principleLength)
                                {
                                    MessageBox.Show(colName + "字段第" + (i + 1) + "行的内容长度超长，请检查修改后再重新接收目录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    flag = true;
                                    break;
                                }
                            }
                            break;
                        }
                        if (tableFromExcel.Rows[i][colName].ToString().Contains(@"'"))
                        {
                            MessageBox.Show(colName + "字段第" + (i + 1) + "行的内容中包含“‘”字符，请检查修改后再重新接收目录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                if (flag)
                {
                    break;
                }
                Application.DoEvents();
                progress.Position += 1;
            }
            dtCheck.Dispose();
            progress.Visible = false;
            if (flag)
            {
                return true;
            }
            return false;
        }

        protected string GetConfigXML()
        {
            XmlDocument doc = new XmlDocument();
            //XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "GB2312", null);
            //doc.AppendChild(dec);
            XmlElement root = doc.CreateElement("ExcelConfig");
            doc.AppendChild(root);
            for (int i = 0; i < this.MatchedResult.Count; i++)
            {
                XmlElement element = doc.CreateElement("ConfigItem");
                element.SetAttribute("Name", this.MatchedResult.Keys.ElementAt(i));
                element.SetAttribute("value1", this.MatchedResult.Values.ElementAt(i).Key);
                element.SetAttribute("description", this.MatchedResult.Values.ElementAt(i).Value);
                root.AppendChild(element);
            }

            return doc.OuterXml;
        }

        private void ShowCatalogeAcceptedRecords()
        {
            xtraPagedNavigation1.TableString = "t_config_imp_catalogs_rec ";
            xtraPagedNavigation1.FieldString = "Unique_code AS '批次号',excel_file_name AS '数据文件',import_time AS '接收时间', record_number AS '接收成功条数' ";
            xtraPagedNavigation1.WhereString = "table_code='" + this.FileCodeName + "' ";
            xtraPagedNavigation1.SortString = "Unique_code DESC ";
            xtraPagedNavigation1.PagedGridView = gridView_cataloge_acceptrecord;
            xtraPagedNavigation1.PagedEventHandler = new ShowLinkButtonsInGridViewColumns[1];//委托数组
            xtraPagedNavigation1.PagedEventHandler[0] = ShowDetailLinkButtonInColumn;
            xtraPagedNavigation1.LoadDataToGridView();
            xtraPagedNavigation1.ExportAlltoFileSqlString = "SELECT * FROM " + this.FileCodeName + "_manual_check WHERE is_completed IS NULL";
        }

        private void ShowDetailLinkButtonInColumn()
        {
            RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();
            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.ButtonClick += riButtonEdit_ButtonClick;
            riButtonEdit.Buttons.Clear();
            EditorButton bt0 = new EditorButton();
            bt0.Kind = ButtonPredefines.Glyph;
            bt0.Caption = "查看详情";
            bt0.ToolTip = "点击查看详情";
            //bt0.Image = global::NPOIExeclHelper.Properties.Resources.blog__2_;
            //bt0.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
            bt0.Appearance.ForeColor = Color.Blue;
            bt0.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt0);
            GridColumn Col1 = new GridColumn();
            Col1.FieldName = "detail";
            Col1.Caption = "接收记录详情";
            Col1.Visible = false;
            Col1.VisibleIndex = gridView_cataloge_acceptrecord.Columns.Count;
            Col1.UnboundType = DevExpress.Data.UnboundColumnType.String;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_cataloge_acceptrecord.RepositoryItems.Add(riButtonEdit);
            gridView_cataloge_acceptrecord.Columns.Add(Col1);
            foreach (GridColumn c in gridView_cataloge_acceptrecord.Columns)
            {
                c.OptionsColumn.AllowEdit = c.ColumnEdit is RepositoryItemButtonEdit;
            }
            this.gridView_cataloge_acceptrecord.BestFitColumns();
        }


        void riButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            int rowhandle = gridView_cataloge_acceptrecord.FocusedRowHandle;
            DataRow dr = gridView_cataloge_acceptrecord.GetDataRow(rowhandle);
            string selectFieldStr = string.Empty;
            FieldShowListDAO fsd = new FieldShowListDAO();
            DataTable fieldDt = fsd.GetFieldsShownInList2(this.FileCodeName);

            if (fieldDt.Rows.Count == 0)
            {
                MessageBox.Show("此档案类型库还未进行显示配置，请配置后继续！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            for (int i = 0; i < fieldDt.Rows.Count; i++)
            {
                if (i == 0)
                    selectFieldStr = fieldDt.Rows[i]["col_name"].ToString() + " AS " + fieldDt.Rows[i]["show_name"].ToString();
                else
                    selectFieldStr += "," + fieldDt.Rows[i]["col_name"].ToString() + " AS " + fieldDt.Rows[i]["show_name"].ToString();
            }
            fieldDt.Dispose();
            WhereCondtionForAchiveNumMakeup = "import_bundle='" + dr["批次号"].ToString() + "'";

            xtraPagedNavigation2.TableString = this.FileCodeName;
            xtraPagedNavigation2.FieldString = selectFieldStr;
            xtraPagedNavigation2.WhereString = WhereCondtionForAchiveNumMakeup;
            xtraPagedNavigation2.SortString = "Unique_code DESC ";
            xtraPagedNavigation2.PagedGridView = gridView_cataloge_detail;
            xtraPagedNavigation2.LoadDataToGridView();
            xtraPagedNavigation2.ExportAlltoFileSqlString = "SELECT " + selectFieldStr + " FROM " + this.FileCodeName + " WHERE import_bundle='" + dr["批次号"].ToString() + "' ORDER BY Unique_code ASC ";
            xtraTabPage_cataloge_detail.Show();

            //HinttoCreateArchiveNumber(dr["批次号"].ToString());//提示未生成档号
        }

        private void HinttoCreateArchiveNumber(string bundleNumber)
        {
            ArchiveNoDAO and = new ArchiveNoDAO();
            object dhFieldName = and.GetArchiveNoDataByCodeName2(this.FileCodeName);
            if (dhFieldName == DBNull.Value || dhFieldName == null)
            {
                MessageBox.Show("此档案类型库的档号字段还未配置，请配置后继续！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ReceiveCatalogeDAO rcd = new ReceiveCatalogeDAO();
            object dhRecord = rcd.GetDHValueByImportBundle(dhFieldName, this.FileCodeName, bundleNumber);//根据目录导入批次得到第一条记录的档号值，用以判断档号是否生成
            if (dhRecord == DBNull.Value || dhRecord == null)
            {
                MessageBox.Show("此批接收的目录数据还未生成档号，请点击“查看详情”-->“生成档号”按钮生成后继续！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void simpleButton_generate_dh_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("本列表所有记录都将生成档号，确定吗？", "问询", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                ArchiveNoDAO and = new ArchiveNoDAO();
                DataTable dt = and.GetArchiveNoDataByCodeName(this.FileCodeName);//取得指定档案库类型对应的档号字段信息
                if (dt.Rows.Count > 0)
                {
                    string xmlfile = dt.Rows[0]["archive_body"].ToString();
                    string dhfield = dt.Rows[0]["archive_num_field"].ToString();
                    string prefix = dt.Rows[0]["archive_prefix"].ToString();
                    string connchar = dt.Rows[0]["connect_char"].ToString();
                    dt.Dispose();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlfile.ToString());
                    XmlNodeList nodeList = doc.SelectNodes(@"ArchiveNumMakeup/MakeupItem");
                    string dhstr = "";
                    //string declareStr = "", fetchInto = "", fetchInto2 = "", selectField = "";
                    dhstr += "'" + prefix + "'+";
                    for (int i = 0; i < nodeList.Count; i++)
                    {

                        if (i == nodeList.Count - 1)
                        {
                            //declareStr += "@" + nodeList[i].Attributes["value"].Value + " nvarchar(50)"; ;
                            dhstr += nodeList[i].Attributes["value"].Value;
                            //fetchInto += "@" + nodeList[i].Attributes["value"].Value;
                            //fetchInto2 += nodeList[i].Attributes["value"].Value;
                            //selectField += nodeList[i].Attributes["value"].Value;
                        }
                        else
                        {
                            //declareStr += "@" + nodeList[i].Attributes["value"].Value + " nvarchar(50),";
                            dhstr += nodeList[i].Attributes["value"].Value + "+'" + connchar + "'+";
                            //fetchInto += "@" + nodeList[i].Attributes["value"].Value + ",";
                            //fetchInto2 += nodeList[i].Attributes["value"].Value + ",";
                            //selectField += nodeList[i].Attributes["value"].Value + ",";
                        }
                    }
                    and.MakeArchiveNumber(this.FileCodeName, dhfield, dhstr, this.WhereCondtionForAchiveNumMakeup);//批量生成档号
                    MessageBox.Show("档号生成成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void checkEdit_cataloge_matched_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit_cataloge_matched.Checked)
            {
                for (int i = 0; i < checkedListBoxControl3.Items.Count; i++)
                {
                    checkedListBoxControl3.Items[i].CheckState = CheckState.Checked;
                }
            }
            else
            {
                for (int i = 0; i < checkedListBoxControl3.Items.Count; i++)
                {
                    checkedListBoxControl3.Items[i].CheckState = CheckState.Unchecked;
                }
            }
        }

        private void simpleButton_BeginRecieveCataloge_Click(object sender, EventArgs e)
        {
            xtraTab_cataloge_browser.Show();
        }

        private void simpleButton_showCatalogeAccepted_Click(object sender, EventArgs e)
        {
            xtraTab_cataloge_acceptRecord.Show();
            ShowCatalogeAcceptedRecords();
        }

        private void xtraTabControl_ReceiveData_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            xtraGoFrontBack1.UpdateBackButtonStateFun();
            xtraDeleteButtonForGrid1.EnableDeleteButton = xtraTabControl_ReceiveData.SelectedTabPageIndex == 2 ? true : false;
            simpleButton_generate_dh.Enabled = xtraTabControl_ReceiveData.SelectedTabPageIndex == 3 ? true : false;
        }

        private void textBox_content_browser_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Excel文件(*.xls)|*.xls|Excel文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox_content_browser.Text = openFileDialog1.FileName;
            }
        }

        private void button_content_next_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_content_browser.Text))
            {
                MessageBox.Show("请选择对应的包含目录数据的EXCEL文件（支持XLS和XLSX）!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (ExcelHelper.HaveNullHeader(textBox_content_browser.Text, 0))
            {
                MessageBox.Show("选择的目录文件中有无用的空表头（或空列），请检查并去掉!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            LoadDataToCheckedListBoxs();
        }
    }
}