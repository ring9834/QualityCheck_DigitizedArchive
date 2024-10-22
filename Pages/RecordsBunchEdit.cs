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
using System.Data.Common;

namespace Prj_FileManageNCheckApp
{
    public partial class RecordsBunchEdit : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        public TableLayoutPanel TableLayoutPanel_AddControlForPrimarySearch;
        public bool IsNodeEditing { get; set; }
        public NodeEditingType NodeEditType { get; set; }
        public UserEntity UserLoggedIn { get; set; }
        public List<string> WhereFieldList { get; set; }
        public List<string> WhereFieldValueList { get; set; }
        public RecordsBunchEdit()
        {
            InitializeComponent();
        }

        public RecordsBunchEdit(string fileCodeName,UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
            radioButton1.Checked = true;
            splitContainerControl1.Collapsed = true;
            layoutControl1.Enabled = false;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            LoadSearchControls();
            LoadFields();
        }

        void LoadFields()
        {
            DataDictionaryDAO ddd = new DataDictionaryDAO();
            DataTable dtFromDB = ddd.GetColNameShowNameByCodeName(this.FileCodeName);
            comboBox1.DataSource = dtFromDB;
            comboBox1.ValueMember = "col_name";
            comboBox1.DisplayMember = "show_name";
        }

        void LoadSearchControls()
        {
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
            DataTable dt = sd.GetFieldsUsedForSearch(this.FileCodeName);
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
            searchButton.Click += searchButton_Click;
            this.AcceptButton = searchButton;
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
            xtraTab_primarySearchResult.Show();
        }

        void searchButton_Click(object sender, EventArgs e)
        {
            GetSearchResult();
            xtraTab_primarysearchRecord.Show();
        }

        void GetSearchResult()
        {
            string where = "1=1 ";
            this.WhereFieldList = new List<string>();
            this.WhereFieldValueList = new List<string>();
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
                            WhereFieldValueList.Add("%" + txtBox.Text + "%");
                        else if (searchChar.Equals("="))//基本检索2
                            WhereFieldValueList.Add("" + txtBox.Text + "");
                        WhereFieldList.Add(fieldName);
                        //过滤特殊字符
                        //where = where.ToLower().Replace("'", "").Replace("<", "").Replace(">", "").Replace("or", "").Replace("and", "").Replace("=", "").Replace("[", "[[]").Replace("_", "[_]").Replace("%", "[%]");
                    }
                }
            }

            string selectFieldStr = "Unique_code";//所有搜索的语句，头部都加上Unique_code字段，只不过在搜索结果页不让它显示出来。 
            DataDictionaryDAO ddd = new DataDictionaryDAO();
            DataTable fieldDt = ddd.GetColInfoByCodeName3(this.FileCodeName);

            List<ColDictionary> colList = new List<ColDictionary>();
            for (int i = 0; i < fieldDt.Rows.Count; i++)
            {
                DataRow dr = fieldDt.Rows[i];
                selectFieldStr += "," + dr["col_name"].ToString() + " AS " + dr["show_name"].ToString();
                ColDictionary coldict = new ColDictionary(dr["show_name"].ToString(), dr["col_name"].ToString(), false, dr["col_maxlen"].ToString());
                bool isColNull = dr["col_null"] == DBNull.Value ? false : Boolean.Parse(dr["col_null"].ToString());
                coldict.ColNull = isColNull;
                coldict.ColShowValue = dr["col_show_value"] == DBNull.Value ? string.Empty : dr["col_show_value"].ToString();
                colList.Add(coldict);
            }
            fieldDt.Dispose();

            gridView_bunchEdit.Tag = colList;//记录字段各信息，修改记录时用
            xtraPagedNavigation1.TableString = this.FileCodeName;
            xtraPagedNavigation1.FieldString = selectFieldStr;
            xtraPagedNavigation1.WhereString = where;
            xtraPagedNavigation1.WhereFieldArray = WhereFieldList;
            xtraPagedNavigation1.WhereFieldValueArray = WhereFieldValueList;
            xtraPagedNavigation1.SortString = "Unique_code ASC ";
            xtraPagedNavigation1.PagedGridView = gridView_bunchEdit;
            xtraPagedNavigation1.PagedEventHandler = new ShowLinkButtonsInGridViewColumns[1];//委托数组
            xtraPagedNavigation1.PagedEventHandler[0] = ShowLinkButton;
            xtraPagedNavigation1.LoadDataToGridView();
            xtraPagedNavigation1.ExportAlltoFileSqlString = "SELECT " + selectFieldStr + " FROM " + this.FileCodeName + " WHERE " + where + " ORDER BY Unique_code ASC ";
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

        void ShowDealingNodeLinkButton()
        {
            RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();
            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.ButtonClick += riButtonEdit_DealWithNode_ButtonClick;
            riButtonEdit.Buttons.Clear();

            EditorButton bt2 = new EditorButton();
            bt2.Kind = ButtonPredefines.Glyph;
            bt2.Caption = "修改";
            bt2.ToolTip = "修改分类";
            bt2.Appearance.ForeColor = Color.Blue;
            bt2.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt2);

            EditorButton bt3 = new EditorButton();
            bt3.Kind = ButtonPredefines.Glyph;
            bt3.Caption = "删除";
            bt3.ToolTip = "删除分类";
            bt3.Appearance.ForeColor = Color.Blue;
            bt3.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt3);

            GridColumn Col1 = new GridColumn();
            Col1.FieldName = "dealingNode";
            Col1.Caption = "操作";
            Col1.Visible = false;
            Col1.VisibleIndex = gridView_bunchEdit.Columns.Count;
            Col1.ColumnEdit = riButtonEdit;

            gridControl_bunchEdit.RepositoryItems.Add(riButtonEdit);
            gridView_bunchEdit.Columns.Add(Col1);
            gridView_bunchEdit.BestFitColumns();
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

        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowLinkButton();
        }

        void ShowLinkButton()
        {
            gridView_bunchEdit.Columns["Unique_code"].Visible = false;
            if (radioGroup1.SelectedIndex == -1) return;//未选择某项，不进行任何操作

            DataTable dtAll = gridControl_bunchEdit.DataSource as DataTable;
            if (radioGroup1.SelectedIndex == 0)
            {
                splitContainerControl1.Collapsed = false;
                layoutControl1.Enabled = true;
                if (gridView_bunchEdit.Columns["dealingNode"] != null)
                {
                    gridView_bunchEdit.Columns.Remove(gridView_bunchEdit.Columns["dealingNode"]);
                }
                if (dtAll.Columns.Contains(" "))
                {
                    dtAll.Columns.Remove(" ");
                    gridView_bunchEdit.Columns.Remove(gridView_bunchEdit.Columns[" "]);
                }
            }
            else if (radioGroup1.SelectedIndex == 1)
            {
                splitContainerControl1.Collapsed = false;
                layoutControl1.Enabled = false;
                if (gridView_bunchEdit.Columns["dealingNode"] != null)
                {
                    gridView_bunchEdit.Columns.Remove(gridView_bunchEdit.Columns["dealingNode"]);
                }
                if (dtAll.Columns.Contains(" "))
                {
                    dtAll.Columns.Remove(" ");
                    gridView_bunchEdit.Columns.Remove(gridView_bunchEdit.Columns[" "]);
                }
            }
            else
            {
                splitContainerControl1.Collapsed = true;
                layoutControl1.Enabled = false;
                if (!dtAll.Columns.Contains(" "))
                {
                    dtAll.Columns.Add(" ", typeof(bool));
                    gridView_bunchEdit.PopulateColumns();
                    gridView_bunchEdit.Columns[" "].VisibleIndex = 0;
                    gridView_bunchEdit.Columns[" "].OptionsColumn.AllowEdit = false;
                }
                if (gridView_bunchEdit.Columns["dealingNode"] == null)
                {
                    ShowDealingNodeLinkButton();
                }
                gridView_bunchEdit.Columns["Unique_code"].Visible = false;
                ShowRepositeItemsInGridView();
            }
        }

        void ShowRepositeItemsInGridView()
        {
            List<ColDictionary> list = (List<ColDictionary>)gridView_bunchEdit.Tag;
            for (int i = 0; i < list.Count; i++)
            {
                ColDictionary col = list[i];
                if (!string.IsNullOrEmpty(col.ColShowValue))//使用了辅助代码
                {
                    BunchEditDAO bed = new BunchEditDAO();
                    DataTable dt = bed.GetColumnSelectingValuesByColName(this.FileCodeName, col.ColName);//获得一个供选择的值列表，供某字段使用，如密级、保管期限等字段
                    RepositoryItemComboBox box = new RepositoryItemComboBox();
                    box.Items.Clear();
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        DataRow dr = dt.Rows[j];
                        AttachedCodeClass fst = new AttachedCodeClass(dr["code_name"].ToString(), dr["code_value"].ToString(), dr["Unique_code"].ToString());
                        box.Items.Add(fst);
                    }
                    box.ParseEditValue += repositoryItemComboBox1_ParseEditValue;
                    box.TextEditStyle = TextEditStyles.DisableTextEditor;
                    gridView_bunchEdit.Columns[col.ShowName].ColumnEdit = box;
                }
            }
        }

        void repositoryItemComboBox1_ParseEditValue(object sender, ConvertEditValueEventArgs e)
        {
            if (e.Value != null)
            {
                if (!string.IsNullOrEmpty(e.Value.ToString()))
                {
                    if (e.Value.GetType() == typeof(AttachedCodeClass))
                    {
                        AttachedCodeClass acc = (AttachedCodeClass)e.Value;
                        e.Value = acc.CodeName;
                        e.Handled = true;
                    }
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (radioGroup1.SelectedIndex == 0)
            {
                if (((DataTable)gridControl_bunchEdit.DataSource).Rows.Count == 0)
                {
                    MessageBox.Show("可被修改的记录数为零！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("请选择批量修改的字段！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (radioButton2.Checked && string.IsNullOrEmpty(comboBoxEdit1.Text))
                {
                    MessageBox.Show("被替换的内容不能为空！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("所有页的相关数据都将被修改,确认吗？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {

                    BunchEditDAO bed = new BunchEditDAO();
                    if (radioButton1.Checked)
                    {
                        //内容全部替换
                        bed.ModifyFieldValueWholy(WhereFieldList, WhereFieldValueList, this.FileCodeName, comboBox1.SelectedValue.ToString(), comboBoxEdit1.Text, xtraPagedNavigation1.WhereString);
                    }
                    else if (radioButton2.Checked)
                    {
                        //内容部分替换
                        bed.ModifyFieldValuePartly(WhereFieldList, WhereFieldValueList, this.FileCodeName, comboBox1.SelectedValue.ToString(), comboBoxEdit2.Text, comboBoxEdit1.Text, xtraPagedNavigation1.WhereString);
                    }
                    else if (radioButton3.Checked)
                    {
                        //空白内容替换为指定值
                        bed.ModifyNullFieldValue(WhereFieldList, WhereFieldValueList, this.FileCodeName, comboBox1.SelectedValue.ToString(), comboBoxEdit1.Text, xtraPagedNavigation1.WhereString);
                    }
                    gridView_bunchEdit.CloseEditor();
                    xtraPagedNavigation1.RefreshDataToGridView();//刷新列表                
                    MessageBox.Show("修改成功！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            if (radioGroup1.SelectedIndex == 1)
            {
                if (MessageBox.Show("确定要删除查询结果中的所有数据吗？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    BunchEditDAO bed = new BunchEditDAO();
                    bed.DeleteRecordsSelected(this.FileCodeName, xtraPagedNavigation1.WhereString);
                    GetSearchResult();//刷新列表
                }
            }
        }

        void riButtonEdit_DealWithNode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            if (e.Button == editor.Properties.Buttons[0])
            {
                if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
                this.IsNodeEditing = true;
                this.NodeEditType = NodeEditingType.Update;
                int rowHandle = gridView_bunchEdit.FocusedRowHandle;
                gridView_bunchEdit.SetRowCellValue(rowHandle, gridView_bunchEdit.Columns[" "], true);
                gridView_bunchEdit.CloseEditor();

            }
            else if (e.Button == editor.Properties.Buttons[1])
            {
                if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
                this.IsNodeEditing = true;
                this.NodeEditType = NodeEditingType.Delete;
                int rowHandle = gridView_bunchEdit.FocusedRowHandle;
                gridView_bunchEdit.SetRowCellValue(rowHandle, gridView_bunchEdit.Columns[" "], true);
                object colVal = gridView_bunchEdit.GetRowCellValue(rowHandle, gridView_bunchEdit.Columns["列名"]);
                if (MessageBox.Show("确定删除选中的记录吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    int index = gridView_bunchEdit.GetDataSourceRowIndex(rowHandle);
                    DataTable dtAll = gridControl_bunchEdit.DataSource as DataTable;
                    object uniqueCode = dtAll.Rows[index]["Unique_code"];
                    BunchEditDAO bed = new BunchEditDAO();
                    bed.DeleteSpecificRecordFromStoreHouseTable(this.FileCodeName, uniqueCode);//从指定档案库中删除某一条记录
                    dtAll.Rows.RemoveAt(index);
                    this.IsNodeEditing = false;
                    this.NodeEditType = NodeEditingType.Nothing;
                }
                else
                {
                    this.IsNodeEditing = false;
                    this.NodeEditType = NodeEditingType.Nothing;
                    gridView_bunchEdit.SetRowCellValue(rowHandle, gridView_bunchEdit.Columns[" "], false);
                }
            }
        }

        private void gridView_bunchEdit_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName.Equals("dealingNode"))
            {
                object val = gridView_bunchEdit.GetRowCellValue(e.RowHandle, gridView_bunchEdit.Columns[" "]);
                if (val != DBNull.Value && val != null)
                {
                    if (Boolean.Parse(val.ToString()))
                    {
                        if (this.IsNodeEditing)
                        {
                            if (this.NodeEditType == NodeEditingType.Update)
                            {
                                DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit ritem = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
                                ritem.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
                                ritem.ButtonClick += RowSave_Click;

                                ritem.Buttons[0].Appearance.ForeColor = Color.Red;
                                ritem.Buttons[0].Appearance.TextOptions.HAlignment = HorzAlignment.Center;
                                ritem.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;
                                ritem.Buttons[0].Caption = "保存";

                                EditorButton bt2 = new EditorButton();
                                bt2.Kind = ButtonPredefines.Glyph;
                                bt2.Caption = "取消";
                                bt2.ToolTip = "取消编辑结果";
                                bt2.Appearance.ForeColor = Color.Red;
                                bt2.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
                                ritem.Buttons.Add(bt2);

                                e.RepositoryItem = ritem;
                            }
                        }
                    }
                }
            }
        }

        void RowSave_Click(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;

            if (e.Button == editor.Properties.Buttons[0])
            {
                if (this.NodeEditType == NodeEditingType.Update)
                {
                    List<ColDictionary> list = (List<ColDictionary>)gridView_bunchEdit.Tag;
                    DataTable dt = gridControl_bunchEdit.DataSource as DataTable;
                    int rowHandle = gridView_bunchEdit.FocusedRowHandle;
                    int index = gridView_bunchEdit.GetDataSourceRowIndex(rowHandle);
                    DataRow dr = dt.Rows[index];
                    string setStrForUpdate = string.Empty;
                    DbParameter[] param = new DbParameter[list.Count];
                    DbHelper helper = new DbHelper();
                    for (int i = 0; i < list.Count; i++)
                    {
                        ColDictionary col = list[i];
                        object[] objs = dr.ItemArray;
                        for (int j = 0; j < gridView_bunchEdit.Columns.Count; j++)
                        {
                            if (col.ShowName.Equals(gridView_bunchEdit.Columns[j].FieldName))
                            {
                                object colNullVal = dr[j];
                                if (!col.ColNull && (colNullVal == DBNull.Value || colNullVal == null || string.IsNullOrEmpty(colNullVal.ToString())))
                                {
                                    MessageBox.Show(col.ShowName + "不能为空！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                if (int.Parse(col.MaxLength) < dr[j].ToString().Length)
                                {
                                    MessageBox.Show(col.ShowName + "超过允许最大的长度，请检查后再继续！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                if (string.IsNullOrEmpty(setStrForUpdate))
                                {
                                    setStrForUpdate += col.ColName + "=@" + col.ColName;
                                }
                                else
                                {
                                    setStrForUpdate += "," + col.ColName + "=@" + col.ColName;
                                }
                                DbParameter para = helper.MakeInParam(col.ColName, dr[j]);
                                param[i] = para;
                                break;
                            }
                        }
                    }
                    BunchEditDAO bed = new BunchEditDAO();
                    bed.ModifyARecordInStoreHouseTable(this.FileCodeName, setStrForUpdate, dr["Unique_code"].ToString(), param);//修改指定档案库中一条记录

                    gridView_bunchEdit.SetRowCellValue(rowHandle, gridView_bunchEdit.Columns[" "], false);
                    gridView_bunchEdit.CloseEditor();
                    this.IsNodeEditing = false;
                    this.NodeEditType = NodeEditingType.Nothing;
                    MessageBox.Show("修改成功！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else if (e.Button == editor.Properties.Buttons[1])
            {
                if (this.NodeEditType == NodeEditingType.Update)
                {
                    int rowHandle = gridView_bunchEdit.FocusedRowHandle;
                    gridView_bunchEdit.SetRowCellValue(rowHandle, gridView_bunchEdit.Columns[" "], false);
                    gridView_bunchEdit.CloseEditor();
                }
                this.IsNodeEditing = false;
                this.NodeEditType = NodeEditingType.Nothing;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BunchEditDAO bed = new BunchEditDAO();//获得一个供选择的值列表，供某字段使用，如密级、保管期限等字段
            DataTable dt = bed.GetColumnSelectingValuesByColName(this.FileCodeName, comboBox1.SelectedValue.ToString());

            comboBoxEdit1.Properties.Items.Clear();
            comboBoxEdit2.Properties.Items.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                comboBoxEdit1.Properties.Items.Add(new ComboboxItem(dr["code_name"].ToString(), dr["code_value"].ToString()));
                comboBoxEdit2.Properties.Items.Add(new ComboboxItem(dr["code_name"].ToString(), dr["code_value"].ToString()));
            }
            if (dt.Rows.Count > 0)
            {
                comboBoxEdit1.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
                //comboBoxEdit2.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            }
            else
            {
                comboBoxEdit1.Properties.TextEditStyle = TextEditStyles.Standard;
                //comboBoxEdit2.Properties.TextEditStyle = TextEditStyles.Standard;
            }

        }
    }
}