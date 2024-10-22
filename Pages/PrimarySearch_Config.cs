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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;

namespace Prj_FileManageNCheckApp
{
    public partial class PrimarySearch_Config : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        public DataTable SearchDatatable { get; set; }
        public UserEntity UserLoggedIn { get; set; }
        public PrimarySearch_Config()
        {
            InitializeComponent();
        }

        public PrimarySearch_Config(string fileCodeName, UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            PageControlLocation.MakeControlCenter(groupControl1.Parent, groupControl1);
            DataDictionaryDAO ddd = new DataDictionaryDAO();
            DataTable dtFromDB = ddd.GetColNameShowNameByCodeName(this.FileCodeName);//取得指定档案库类型对应的字段信息：字段名和显示名
            checkedListBox_primarysearch_canBeSelected.Items.Clear();
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBox_primarysearch_canBeSelected.Items.Add(item);
            }
            dtFromDB.Dispose();
            BindGridControl();
            DeleteAlreadySelectedFields();
        }

        private void BindGridControl()
        {
            SearchDAO sd = new SearchDAO();
            this.SearchDatatable = sd.GetConfigurededFieds(this.FileCodeName);//获得已配置（已选）字段信息

            BindingSource bs = new BindingSource();
            bs.DataSource = this.SearchDatatable;
            gridControl_primarysearch_selected.DataSource = SearchDatatable;
            gridView_primarysearch_selected.PopulateColumns();

            GridColumn Col2 = new GridColumn();
            Col2.FieldName = "SearchCondition";
            Col2.Caption = "搜索条件";
            Col2.VisibleIndex = gridView_primarysearch_selected.Columns.Count;
            Col2.UnboundType = DevExpress.Data.UnboundColumnType.Object;

            RepositoryItemComboBox itemBox = new RepositoryItemComboBox();
            ConfigCodeDAO ccd = new ConfigCodeDAO();
            DataTable dtcondtion = ccd.GetCodes("jstj");//通过key获得辅助代码（编码）
            for (int i = 0; i < dtcondtion.Rows.Count; i++)
            {
                string name = dtcondtion.Rows[i]["code_name"].ToString();
                string value = dtcondtion.Rows[i]["code_value"].ToString();
                int uniquecode = int.Parse(dtcondtion.Rows[i]["Unique_code"].ToString());
                ComboBoxItem combItem = new ComboBoxItem(new SearchConditionEntity(name, value, uniquecode));
                itemBox.Items.Add(combItem);
            }
            dtcondtion.Dispose();

            itemBox.TextEditStyle = TextEditStyles.DisableTextEditor;
            itemBox.SelectedIndexChanged += itemBox_SelectedIndexChanged;
            Col2.ColumnEdit = itemBox;
            gridView_primarysearch_selected.Columns.Add(Col2);
            gridView_primarysearch_selected.Columns["已选字段"].OptionsColumn.AllowEdit = false;
            gridView_primarysearch_selected.Columns["Unique_code"].Visible = false;
            gridView_primarysearch_selected.Columns["col_name"].Visible = false;
            gridView_primarysearch_selected.Columns["search_code"].Visible = false;
            SearchDatatable.Columns.Add("SearchCondition", typeof(SearchConditionEntity));

            for (int i = 0; i < this.SearchDatatable.Rows.Count; i++)
            {
                string searchCode = this.SearchDatatable.Rows[i]["search_code"].ToString();
                for (int j = 0; j < itemBox.Items.Count; j++)
                {
                    SearchConditionEntity entity = (SearchConditionEntity)itemBox.Items[j];
                    if (entity.UniqueCode.ToString().Equals(searchCode))
                    {
                        int rowhandle = gridView_primarysearch_selected.GetRowHandle(i);
                        gridView_primarysearch_selected.SetRowCellValue(rowhandle, gridView_primarysearch_selected.Columns["SearchCondition"], entity);//△△△设置已经选定的搜索条件，也可以利用这种方法设置RepositoryItemComboBox的默认选中项
                        this.SearchDatatable.Rows[i]["SearchCondition"] = entity;
                    }
                }
            }
        }

        void DeleteAlreadySelectedFields()
        {
            for (int i = 0; i < this.SearchDatatable.Rows.Count; i++)
            {
                CheckedListBoxItem item_delete = checkedListBox_primarysearch_canBeSelected.Items.SingleOrDefault(t => t.Tag.ToString().Equals(SearchDatatable.Rows[i]["Unique_code"].ToString()));
                checkedListBox_primarysearch_canBeSelected.Items.Remove(item_delete);
            }
        }

        void itemBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchConditionEntity obj = (SearchConditionEntity)((ComboBoxEdit)sender).SelectedItem;
            int index = gridView_primarysearch_selected.GetDataSourceRowIndex(gridView_primarysearch_selected.FocusedRowHandle);
            this.SearchDatatable.Rows[index]["SearchCondition"] = obj;
        }

        private void button_primarySearchConfig_select_Click(object sender, EventArgs e)
        {
            List<CheckedListBoxItem> list = new List<CheckedListBoxItem>();
            for (int i = 0; i < checkedListBox_primarysearch_canBeSelected.Items.Count; i++)
            {
                CheckedListBoxItem item = checkedListBox_primarysearch_canBeSelected.Items[i];
                if (item.CheckState == CheckState.Checked)
                {
                    DataRow row = this.SearchDatatable.NewRow();
                    row["Unique_code"] = item.Tag;
                    row["col_name"] = item.Value;
                    row["已选字段"] = item.Description;
                    this.SearchDatatable.Rows.Add(row);
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                checkedListBox_primarysearch_canBeSelected.Items.Remove(list[i]);
            }
            list = null;
        }

        private void button_primarySearchConfig_restore_Click(object sender, EventArgs e)
        {
            int index = gridView_primarysearch_selected.GetDataSourceRowIndex(gridView_primarysearch_selected.FocusedRowHandle);
            CheckedListBoxItem item = new CheckedListBoxItem();
            item.Description = this.SearchDatatable.Rows[index]["已选字段"].ToString();
            item.Value = this.SearchDatatable.Rows[index]["col_name"];
            item.Tag = this.SearchDatatable.Rows[index]["Unique_code"];
            checkedListBox_primarysearch_canBeSelected.Items.Add(item);
            this.SearchDatatable.Rows.RemoveAt(index);
        }

        private void button_primarySearchConfig_restoreAll_Click(object sender, EventArgs e)
        {
            DataDictionaryDAO ddd = new DataDictionaryDAO();
            DataTable dtFromDB = ddd.GetColNameShowNameByCodeName(this.FileCodeName);//取得指定档案库类型对应的字段信息：字段名和显示名
            checkedListBox_primarysearch_canBeSelected.Items.Clear();
            for (int i = 0; i < dtFromDB.Rows.Count; i++)
            {
                CheckedListBoxItem item = new CheckedListBoxItem(dtFromDB.Rows[i]["col_name"], dtFromDB.Rows[i]["show_name"].ToString(), dtFromDB.Rows[i]["Unique_code"].ToString());
                checkedListBox_primarysearch_canBeSelected.Items.Add(item);
            }
            dtFromDB.Dispose();
            this.SearchDatatable.Clear();
        }

        private void button_primarySearchConfig_save_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.SearchDatatable.Rows.Count; i++)
            {
                object sCondition = this.SearchDatatable.Rows[i]["SearchCondition"];
                if (sCondition == DBNull.Value)
                {
                    MessageBox.Show("请确认为每个选定的字段确定了搜索条件！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            string sql = string.Empty;
            string codeStr = string.Empty;
            SearchDAO sd = new SearchDAO();
            for (int i = 0; i < this.SearchDatatable.Rows.Count; i++)
            {
                string selectedcode = this.SearchDatatable.Rows[i]["Unique_code"].ToString();
                object searchCondition = this.SearchDatatable.Rows[i]["SearchCondition"];
                SearchConditionEntity entity = (SearchConditionEntity)searchCondition;
                
                sd.SaveConfiguredFieldsForSearch(selectedcode, entity.UniqueCode, i);//保存配置的搜索字段
                if (i == 0)
                    codeStr += selectedcode;
                else
                    codeStr += "," + selectedcode;
            }
            sd.DeleteFieldsNotInConfiguredFieldList(this.FileCodeName, codeStr);//删除原来在现在不在已配置字段列表中的字段
            MessageBox.Show("保存完毕！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_primarySearchConfig_up_Click(object sender, EventArgs e)
        {
            int rowhandle = gridView_primarysearch_selected.FocusedRowHandle;
            int index = gridView_primarysearch_selected.GetDataSourceRowIndex(rowhandle);
            if (index - 1 >= 0)
            {
                DataRow row = this.SearchDatatable.NewRow();
                row.ItemArray = this.SearchDatatable.Rows[index - 1].ItemArray;
                this.SearchDatatable.Rows[index - 1].ItemArray = this.SearchDatatable.Rows[index].ItemArray;
                this.SearchDatatable.Rows[index].ItemArray = row.ItemArray;
                rowhandle = gridView_primarysearch_selected.GetRowHandle(index - 1);
                gridView_primarysearch_selected.FocusedRowHandle = rowhandle;
            }
        }

        private void button_primarySearchConfig_down_Click(object sender, EventArgs e)
        {
            int rowhandle = gridView_primarysearch_selected.FocusedRowHandle;
            int index = gridView_primarysearch_selected.GetDataSourceRowIndex(rowhandle);
            if (index + 1 < this.SearchDatatable.Rows.Count)
            {
                DataRow row = this.SearchDatatable.NewRow();
                row.ItemArray = this.SearchDatatable.Rows[index + 1].ItemArray;
                this.SearchDatatable.Rows[index + 1].ItemArray = this.SearchDatatable.Rows[index].ItemArray;
                this.SearchDatatable.Rows[index].ItemArray = row.ItemArray;
                rowhandle = gridView_primarysearch_selected.GetRowHandle(index + 1);
                gridView_primarysearch_selected.FocusedRowHandle = rowhandle;
            }
        }

        private void PrimarySearch_Config_Load(object sender, EventArgs e)
        {

        }
    }
}