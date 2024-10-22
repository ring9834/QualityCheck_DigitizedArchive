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
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraGrid.Columns;
using DotNet.Utilities;
using System.Data.Common;

namespace Prj_FileManageNCheckApp
{
    public partial class PublicDataDictionary : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }
        private DataTable PubDicDatatable { get; set; }
        public bool IsNodeEditing { get; set; }
        public NodeEditingType NodeEditType { get; set; }
        public UserEntity UserLoggedIn { get; set; }
        public PublicDataDictionary()
        {
            InitializeComponent();
        }

        public PublicDataDictionary(string fileCodeName, UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;

        }

        private void PublicDataDictionary_Load(object sender, EventArgs e)
        {
            LoadDictionary();
            ShowAddLinkButton();
            BindDataFromDb();

        }

        void LoadDictionary()
        {
            string fieldName = "col_name,show_name,col_datatype,col_maxlen,col_null,col_default,col_use,query_flag,field_type,comments,Unique_code";
            DataDictionaryDAO ddd = new DataDictionaryDAO();
            PubDicDatatable = ddd.GetPublicDictionaryCols();//获得公共数据字典中的字段信息
            gridControl_publicDictionary_config.DataSource = PubDicDatatable;
            PubDicDatatable.Columns.Add(" ", typeof(bool));
            gridView_publicDictionary_config.PopulateColumns();
            gridView_publicDictionary_config.Columns[" "].VisibleIndex = 0;
            gridView_publicDictionary_config.Columns["Unique_code"].Visible = false;
            gridView_publicDictionary_config.Columns["字段类型"].ToolTip = "选择表示“管理字段”；不选择表示“业务字段”";
            gridView_publicDictionary_config.Tag = fieldName;

            ConfigCodeDAO ccd = new ConfigCodeDAO();
            DataTable dt_sjlx = ccd.GetCodes("SJLX");
            dataTypeComboBox.Items.Clear();
            for (int i = 0; i < dt_sjlx.Rows.Count; i++)
            {
                DataRow dr = dt_sjlx.Rows[i];
                AttachedCodeClass fst = new AttachedCodeClass(dr["code_name"].ToString(), dr["code_value"].ToString(), dr["Unique_code"].ToString());
                dataTypeComboBox.Items.Add(fst);
            }
            dt_sjlx.Dispose();
            dataTypeComboBox.TextEditStyle = TextEditStyles.DisableTextEditor;
            dataTypeComboBox.SelectedIndexChanged += itemBox_SelectedIndexChanged;
            dataTypeComboBox.ParseEditValue += dataTypeComboBox_ParseEditValue;
            gridView_publicDictionary_config.Columns["数据类型"].ColumnEdit = dataTypeComboBox;
        }

        void dataTypeComboBox_ParseEditValue(object sender, ConvertEditValueEventArgs e)
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

        void BindDataFromDb()
        {
            for (int i = 0; i < this.PubDicDatatable.Rows.Count; i++)
            {
                int rowhandle = gridView_publicDictionary_config.GetRowHandle(i);
                DataRow dr0 = gridView_publicDictionary_config.GetDataRow(rowhandle);
                if (dr0["数据类型"] != DBNull.Value)
                {
                    object obj = dr0["数据类型"];
                    for (int j = 0; j < dataTypeComboBox.Items.Count; j++)
                    {
                        AttachedCodeClass acc = (AttachedCodeClass)dataTypeComboBox.Items[j];
                        if (acc.UniqueCode.Equals(obj.ToString()))
                        {
                            gridView_publicDictionary_config.SetRowCellValue(rowhandle, gridView_publicDictionary_config.Columns["数据类型"], acc.CodeName);
                            break;
                        }
                    }
                }
            }
        }

        void itemBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //AttachedCodeClass obj = (AttachedCodeClass)((ComboBoxEdit)sender).SelectedItem;
            //int index = gridView_publicDictionary_config.GetDataSourceRowIndex(gridView_publicDictionary_config.FocusedRowHandle);
            //this.PubDicDatatable.Rows[index]["数据类型"] = obj.UniqueCode;
        }

        private void ShowAddLinkButton()
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
            Col1.VisibleIndex = gridView_publicDictionary_config.Columns.Count;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_publicDictionary_config.RepositoryItems.Add(riButtonEdit);
            gridView_publicDictionary_config.Columns.Add(Col1);
            gridView_publicDictionary_config.BestFitColumns();
        }

        void riButtonEdit_DealWithNode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            if (e.Button == editor.Properties.Buttons[0])
            {
                if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
                this.IsNodeEditing = true;
                this.NodeEditType = NodeEditingType.Update;
                int rowHandle = gridView_publicDictionary_config.FocusedRowHandle;
                gridView_publicDictionary_config.SetRowCellValue(rowHandle, gridView_publicDictionary_config.Columns[" "], true);
                gridView_publicDictionary_config.CloseEditor();

            }
            else if (e.Button == editor.Properties.Buttons[1])
            {
                if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
                this.IsNodeEditing = true;
                this.NodeEditType = NodeEditingType.Delete;
                int rowHandle = gridView_publicDictionary_config.FocusedRowHandle;
                gridView_publicDictionary_config.SetRowCellValue(rowHandle, gridView_publicDictionary_config.Columns[" "], true);
                if (MessageBox.Show("确定删除选中的记录吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    int index = gridView_publicDictionary_config.GetDataSourceRowIndex(gridView_publicDictionary_config.FocusedRowHandle);
                    object uniqueCode = this.PubDicDatatable.Rows[index]["Unique_code"];
                    DataDictionaryDAO ddd = new DataDictionaryDAO();
                    ddd.DeleteSpecificDataDictRecord(uniqueCode);//在公共数据字典中删除某字段
                    this.PubDicDatatable.Rows.RemoveAt(index);
                    this.IsNodeEditing = false;
                    this.NodeEditType = NodeEditingType.Nothing;
                }
                else
                {
                    this.IsNodeEditing = false;
                    this.NodeEditType = NodeEditingType.Nothing;
                    gridView_publicDictionary_config.SetRowCellValue(rowHandle, gridView_publicDictionary_config.Columns[" "], false);
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
            this.IsNodeEditing = true;
            this.NodeEditType = NodeEditingType.Add;
            DataRow dr = MakeADataRow();
            this.PubDicDatatable.Rows.Add(dr);
            gridView_publicDictionary_config.MoveLast();
            int rowHandle = gridView_publicDictionary_config.FocusedRowHandle;
            gridView_publicDictionary_config.SetRowCellValue(rowHandle, gridView_publicDictionary_config.Columns[" "], true);
        }

        DataRow MakeADataRow()
        {
            DataRow dr = this.PubDicDatatable.NewRow();
            dr["Unique_code"] = -1;
            dr["列名"] = string.Empty;
            dr["显示名"] = string.Empty;
            dr["数据类型"] = string.Empty;
            dr["最大长度"] = string.Empty;
            dr["可为空?"] = true;
            dr["默认值"] = string.Empty;
            dr["可用?"] = true;
            dr["可查?"] = true;
            dr["字段类型"] = -1;
            dr["备注"] = string.Empty;
            return dr;
        }

        private void gridView_publicDictionary_config_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName.Equals("dealingNode"))
            {
                object val = gridView_publicDictionary_config.GetRowCellValue(e.RowHandle, gridView_publicDictionary_config.Columns[" "]);
                if (val != DBNull.Value)
                {
                    if (Boolean.Parse(val.ToString()))
                    {
                        if (this.IsNodeEditing)
                        {
                            if (this.NodeEditType == NodeEditingType.Add || this.NodeEditType == NodeEditingType.Update)
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

        private void gridView_publicDictionary_config_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (gridView_publicDictionary_config.FocusedColumn == gridView_publicDictionary_config.Columns[" "])
            {
                if (this.IsNodeEditing)
                {
                    e.Cancel = true;
                }
            }
        }

        void RowSave_Click(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            if (e.Button == editor.Properties.Buttons[0])
            {
                int index = gridView_publicDictionary_config.GetDataSourceRowIndex(gridView_publicDictionary_config.FocusedRowHandle);
                object colVal = this.PubDicDatatable.Rows[index]["列名"];
                if (string.IsNullOrEmpty(colVal.ToString()))
                {
                    MessageBox.Show("列名不能为空！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                object showVal = this.PubDicDatatable.Rows[index]["显示名"];
                if (string.IsNullOrEmpty(showVal.ToString()))
                {
                    MessageBox.Show("显示名不能为空！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                object dataType = this.PubDicDatatable.Rows[index]["数据类型"];
                if (string.IsNullOrEmpty(dataType.ToString()))
                {
                    MessageBox.Show("数据类型名不能为空！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                object maxLen = this.PubDicDatatable.Rows[index]["最大长度"];
                if (string.IsNullOrEmpty(maxLen.ToString()))
                {
                    MessageBox.Show("最大长度不能为空！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!ValidateUtil.IsInt(maxLen.ToString()))
                {
                    MessageBox.Show("最大长度中出现非整数的字符！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string dataTypeCode = string.Empty;
                for (int i = 0; i < dataTypeComboBox.Items.Count; i++)
                {
                    AttachedCodeClass acc = (AttachedCodeClass)dataTypeComboBox.Items[i];
                    if (acc.CodeName.Equals(dataType.ToString()))
                        dataTypeCode = acc.UniqueCode;
                }

                object colNull = this.PubDicDatatable.Rows[index]["可为空?"];
                object colUse = this.PubDicDatatable.Rows[index]["可用?"];
                object colQuery = this.PubDicDatatable.Rows[index]["可查?"];
                object fieldType = this.PubDicDatatable.Rows[index]["字段类型"];
                object colDefault = this.PubDicDatatable.Rows[index]["默认值"];
                object comment = this.PubDicDatatable.Rows[index]["备注"];
                DataDictionaryDAO ddd = new DataDictionaryDAO();
                if (this.NodeEditType == NodeEditingType.Add)
                {
                    object lastUniqueCode = ddd.SaveCodeBase(colVal, showVal, dataTypeCode, maxLen, colNull, colUse, colQuery, fieldType, colDefault, comment);//保存公共数据字典记录
                    this.PubDicDatatable.Rows[index]["Unique_code"] = lastUniqueCode;//更新DATATABLE中的Unique_code
                    this.PubDicDatatable.Rows[index][" "] = false;
                    gridView_publicDictionary_config.CloseEditor();
                    this.IsNodeEditing = false;
                    this.NodeEditType = NodeEditingType.Nothing;
                    MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if (this.NodeEditType == NodeEditingType.Update)
                {
                    object uniqueCode = this.PubDicDatatable.Rows[index]["Unique_code"];
                    ddd.UpdateCodeBase(uniqueCode, colVal, showVal, dataTypeCode, maxLen, colNull, colUse, colQuery, fieldType, colDefault, comment);//更新公共数据字典记录
                    this.PubDicDatatable.Rows[index][" "] = false;
                    gridView_publicDictionary_config.CloseEditor();
                    this.IsNodeEditing = false;
                    this.NodeEditType = NodeEditingType.Nothing;
                    MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else if (e.Button == editor.Properties.Buttons[1])
            {
                if (this.NodeEditType == NodeEditingType.Add)
                {
                    int dr_deleting = this.PubDicDatatable.Rows.Count - 1;//取消刚才添加的一条，此时未保存到数据库
                    this.PubDicDatatable.Rows.RemoveAt(dr_deleting);
                }
                if (this.NodeEditType == NodeEditingType.Update)
                {
                    int rowHandle = gridView_publicDictionary_config.FocusedRowHandle;
                    gridView_publicDictionary_config.SetRowCellValue(rowHandle, gridView_publicDictionary_config.Columns[" "], false);
                    gridView_publicDictionary_config.CloseEditor();
                }
                this.IsNodeEditing = false;
                this.NodeEditType = NodeEditingType.Nothing;
            }
        }
    }
}