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
using System.Data.Common;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Controls;

namespace Prj_FileManageNCheckApp
{
    public partial class FieldShowType_Config : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }
        public DataTable ShowTypeDataTable { get; set; }
        public UserEntity UserLoggedIn { get; set; }
        public FieldShowType_Config()
        {
            InitializeComponent();
        }

        public FieldShowType_Config(string fileCodeName,UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            if (this.ShowTypeDataTable != null)
            {
                ShowTypeDataTable.Dispose();
                this.ShowTypeDataTable = null;
            }
            gridView_fieldShowType_config.Columns.Clear();
            DataDictionaryDAO ddd = new DataDictionaryDAO();
            this.ShowTypeDataTable = ddd.GetColInfoByCodeName2(this.FileCodeName);

            gridControl_fieldShowType_config.DataSource = ShowTypeDataTable;
            gridView_fieldShowType_config.PopulateColumns();
            ShowTypeDataTable.Columns.Add("ShowTypeField", typeof(AttachedCodeClass));
            ShowTypeDataTable.Columns.Add("AttachedCodeField", typeof(AttachedCodeClass));

            GridColumn showTypeCol = new GridColumn();
            showTypeCol.Caption = "显示类型";
            showTypeCol.Visible = true;
            showTypeCol.FieldName = "ShowTypeField";

            ConfigCodeDAO ccd = new ConfigCodeDAO();
            DataTable dt_xsfs = ccd.GetCodes("XSFS");

            FieldShowTypeComboBox.Items.Clear();
            for (int i = 0; i < dt_xsfs.Rows.Count; i++)
            {
                DataRow dr = dt_xsfs.Rows[i];
                AttachedCodeClass fst = new AttachedCodeClass(dr["code_name"].ToString(), dr["code_value"].ToString(), dr["Unique_code"].ToString());
                FieldShowTypeComboBox.Items.Add(fst);
            }
            dt_xsfs.Dispose();
            FieldShowTypeComboBox.TextEditStyle = TextEditStyles.DisableTextEditor;
            FieldShowTypeComboBox.SelectedIndexChanged += itemBox_SelectedIndexChanged;
            showTypeCol.ColumnEdit = FieldShowTypeComboBox;
            gridView_fieldShowType_config.Columns.Add(showTypeCol);
            showTypeCol.VisibleIndex = 2;

            GridColumn attachedCodeTypeCol = new GridColumn();
            attachedCodeTypeCol.Caption = "辅助代码";
            attachedCodeTypeCol.Visible = true;
            attachedCodeTypeCol.FieldName = "AttachedCodeField";
            attachedCodeTypeCol.VisibleIndex = 3;
            gridView_fieldShowType_config.Columns.Add(attachedCodeTypeCol);
            gridView_fieldShowType_config.Columns["Unique_code"].Visible = false;
            gridView_fieldShowType_config.Columns["col_show_type"].Visible = false;
            gridView_fieldShowType_config.Columns["col_show_value"].Visible = false;
            gridView_fieldShowType_config.Columns["col_name"].OptionsColumn.AllowEdit = false;
            gridView_fieldShowType_config.Columns["show_name"].OptionsColumn.AllowEdit = false;
            BindDataFromDb();
        }

        void BindDataFromDb()
        {
            for (int i = 0; i < this.ShowTypeDataTable.Rows.Count; i++)
            {
                int rowhandle = gridView_fieldShowType_config.GetRowHandle(i);
                DataRow dr0 = gridView_fieldShowType_config.GetDataRow(rowhandle);
                if (dr0["col_show_type"] != DBNull.Value)
                {
                    object obj = dr0["col_show_type"];
                    for (int j = 0; j < FieldShowTypeComboBox.Items.Count; j++)
                    {
                        AttachedCodeClass acc = (AttachedCodeClass)FieldShowTypeComboBox.Items[j];
                        if (acc.UniqueCode.Equals(obj.ToString()))
                        {
                            gridView_fieldShowType_config.SetRowCellValue(rowhandle, gridView_fieldShowType_config.Columns["ShowTypeField"], acc);
                        }
                    }
                }
                if (dr0["col_show_value"] != DBNull.Value)
                {
                    object obj = dr0["col_show_value"];
                    GetAttachedCodeComboBoxItems();
                    for (int j = 0; j < attachedCodeComboBox.Items.Count; j++)
                    {
                        AttachedCodeClass acc = (AttachedCodeClass)attachedCodeComboBox.Items[j];
                        if (acc.UniqueCode.Equals(obj.ToString()))
                        {
                            gridView_fieldShowType_config.SetRowCellValue(rowhandle, gridView_fieldShowType_config.Columns["AttachedCodeField"], acc);
                        }
                    }

                }
            }
        }

        void itemBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AttachedCodeClass obj = (AttachedCodeClass)((ComboBoxEdit)sender).SelectedItem;
            int index = gridView_fieldShowType_config.GetDataSourceRowIndex(gridView_fieldShowType_config.FocusedRowHandle);
            this.ShowTypeDataTable.Rows[index]["ShowTypeField"] = obj;
        }

        void attchItemBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AttachedCodeClass obj = (AttachedCodeClass)((ComboBoxEdit)sender).SelectedItem;
            int index = gridView_fieldShowType_config.GetDataSourceRowIndex(gridView_fieldShowType_config.FocusedRowHandle);
            this.ShowTypeDataTable.Rows[index]["AttachedCodeField"] = obj;
        }

        private void gridView_fieldShowType_config_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName.Equals("AttachedCodeField"))
            {
                int rowhandle = e.RowHandle;
                DataRow dr0 = gridView_fieldShowType_config.GetDataRow(rowhandle);
                if (dr0["ShowTypeField"] != DBNull.Value)
                {
                    AttachedCodeClass obj = (AttachedCodeClass)dr0["ShowTypeField"];
                    if (obj.CodeValue.Equals("XLK"))
                    {
                        GetAttachedCodeComboBoxItems();
                        e.RepositoryItem = attachedCodeComboBox;
                    }
                }
            }
            if (e.Column.FieldName.Equals("ShowTypeField"))
            {
                int rowhandle = e.RowHandle;
                DataRow dr0 = gridView_fieldShowType_config.GetDataRow(rowhandle);
                if (dr0["ShowTypeField"] != DBNull.Value)
                {
                    AttachedCodeClass obj = (AttachedCodeClass)dr0["ShowTypeField"];
                    if (obj.CodeValue.Equals("XLK"))
                    {
                        object v = gridView_fieldShowType_config.GetRowCellValue(rowhandle, gridView_fieldShowType_config.Columns["AttachedCodeField"]);
                        if (v == DBNull.Value)//已经显示提示信息，就不要重复显示了
                            gridView_fieldShowType_config.SetRowCellValue(rowhandle, gridView_fieldShowType_config.Columns["AttachedCodeField"], new AttachedCodeClass("请点击选择...", null, null));
                    }
                    else
                    {
                        object v = gridView_fieldShowType_config.GetRowCellValue(rowhandle, gridView_fieldShowType_config.Columns["AttachedCodeField"]);
                        if (v != DBNull.Value)
                            gridView_fieldShowType_config.SetRowCellValue(rowhandle, gridView_fieldShowType_config.Columns["AttachedCodeField"], DBNull.Value);
                    }
                }
            }
        }

        void GetAttachedCodeComboBoxItems()
        {
            if (attachedCodeComboBox.Items.Count == 0)
            {
                ConfigCodeDAO ccd = new ConfigCodeDAO();
                DataTable dt_attch = ccd.GetCodeBases();
                for (int i = 0; i < dt_attch.Rows.Count; i++)
                {
                    DataRow dr = dt_attch.Rows[i];
                    AttachedCodeClass fst_attch = new AttachedCodeClass(dr["base_name"].ToString(), dr["code_key"].ToString(), dr["Unique_code"].ToString());
                    attachedCodeComboBox.Items.Add(fst_attch);
                }
                dt_attch.Dispose();
                attachedCodeComboBox.TextEditStyle = TextEditStyles.DisableTextEditor;
                attachedCodeComboBox.SelectedIndexChanged += attchItemBox_SelectedIndexChanged;
            }
        }

        private void simpleButton_BeginGJ_Click(object sender, EventArgs e)
        {
            if (this.ShowTypeDataTable != null)
            {
                for (int i = 0; i < this.ShowTypeDataTable.Rows.Count; i++)
                {
                    object obj1 = this.ShowTypeDataTable.Rows[i]["ShowTypeField"];
                    object obj2 = this.ShowTypeDataTable.Rows[i]["AttachedCodeField"];
                    if (obj1 != DBNull.Value)
                    {
                        AttachedCodeClass obj = (AttachedCodeClass)obj1;
                        if (obj.CodeValue.Equals("XLK"))
                        {
                            AttachedCodeClass acc = (AttachedCodeClass)obj2;
                            if (string.IsNullOrEmpty(acc.UniqueCode))
                            {
                                MessageBox.Show("请确认“显示类型”和“辅助代码”都设置正确！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    object obj3 = this.ShowTypeDataTable.Rows[i]["col_order"];
                    if (string.IsNullOrEmpty(obj3.ToString()))
                    {
                        MessageBox.Show("请确认所有排序是否都已设置正确！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!ValidateUtil.IsInt(obj3.ToString()))
                    {
                        MessageBox.Show("排序中出现非整数的字符！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                for (int i = 0; i < this.ShowTypeDataTable.Rows.Count; i++)
                {
                    object obj1 = this.ShowTypeDataTable.Rows[i]["ShowTypeField"];
                    object obj2 = this.ShowTypeDataTable.Rows[i]["AttachedCodeField"];
                    object obj3 = this.ShowTypeDataTable.Rows[i]["col_null"];
                    object obj4 = this.ShowTypeDataTable.Rows[i]["col_use"];
                    object colorder = this.ShowTypeDataTable.Rows[i]["col_order"];
                    object comments = this.ShowTypeDataTable.Rows[i]["comments"];
                    object uniquecode = this.ShowTypeDataTable.Rows[i]["Unique_code"];
                    object showTypeName = DBNull.Value;
                    if (obj1 != DBNull.Value)
                        showTypeName = ((AttachedCodeClass)obj1).UniqueCode;
                    object showTypeValue = DBNull.Value;
                    if (obj2 != DBNull.Value)
                        showTypeValue = ((AttachedCodeClass)obj2).UniqueCode;

                    bool colNull = Boolean.Parse(obj3.ToString());
                    bool colUse = Boolean.Parse(obj4.ToString());
                    DataDictionaryDAO ddd = new DataDictionaryDAO();
                    ddd.UpdateColInfo(uniquecode, showTypeName, showTypeValue, colNull, colUse, colorder, comments);
                }
                //InitiateDatas(this.FileCodeName);
                MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gridView_fieldShowType_config_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (gridView_fieldShowType_config.FocusedColumn.FieldName.Equals("AttachedCodeField"))
            {
                int rowhandle = gridView_fieldShowType_config.FocusedRowHandle;
                DataRow dr0 = gridView_fieldShowType_config.GetDataRow(rowhandle);
                if (dr0["ShowTypeField"] != DBNull.Value)
                {
                    AttachedCodeClass obj = (AttachedCodeClass)dr0["ShowTypeField"];
                    if (obj.CodeValue.Equals("XLK"))
                    {
                        e.Cancel = false;//允许编辑，查了很多资料才研究出来的
                    }
                    else
                        e.Cancel = true;//不允许编辑
                }
                else
                    e.Cancel = true;
            }
        }

    }
}