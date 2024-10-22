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
using System.Data.Common;

namespace Prj_FileManageNCheckApp
{
    public partial class UserConfig_Role : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }
        private DataTable RoleDatatable { get; set; }
        public bool IsNodeEditing { get; set; }
        public NodeEditingType NodeEditType { get; set; }
        public UserConfig_Role()
        {
            InitializeComponent();
        }

        public UserConfig_Role(string fileCodeName)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            if (paras != null)
            {
                this.FileCodeName = paras.FileCodeName;
            }
        }

        private void UserConfig_Role_Load(object sender, EventArgs e)
        {
            LoadRoles();
            ShowAddLinkButton();
        }

        void LoadRoles()
        {
            string fieldName = "role_name,Unique_code";
            UserDAO ud = new UserDAO();
            RoleDatatable = ud.LoadRoels();
            gridControl_role_config.DataSource = RoleDatatable;
            RoleDatatable.Columns.Add(" ", typeof(bool));
            gridView_role_config.PopulateColumns();
            gridView_role_config.Columns[" "].VisibleIndex = 0;
            gridView_role_config.Columns[" "].OptionsColumn.AllowEdit = false;
            gridView_role_config.Columns["Unique_code"].Visible = false;
            gridView_role_config.Tag = fieldName;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
            this.IsNodeEditing = true;
            this.NodeEditType = NodeEditingType.Add;
            DataRow dr = MakeADataRow();
            this.RoleDatatable.Rows.Add(dr);
            gridView_role_config.MoveLast();
            int rowHandle = gridView_role_config.FocusedRowHandle;
            gridView_role_config.SetRowCellValue(rowHandle, gridView_role_config.Columns[" "], true);
        }

        DataRow MakeADataRow()
        {
            DataRow dr = this.RoleDatatable.NewRow();
            dr["Unique_code"] = -1;
            dr["角色名"] = string.Empty;
            return dr;
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
            Col1.VisibleIndex = gridView_role_config.Columns.Count;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_role_config.RepositoryItems.Add(riButtonEdit);
            gridView_role_config.Columns.Add(Col1);
            gridView_role_config.BestFitColumns();
        }

        void riButtonEdit_DealWithNode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            if (e.Button == editor.Properties.Buttons[0])
            {
                if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
                this.IsNodeEditing = true;
                this.NodeEditType = NodeEditingType.Update;
                int rowHandle = gridView_role_config.FocusedRowHandle;
                gridView_role_config.SetRowCellValue(rowHandle, gridView_role_config.Columns[" "], true);
                gridView_role_config.CloseEditor();
            }
            else if (e.Button == editor.Properties.Buttons[1])
            {
                if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
                this.IsNodeEditing = true;
                this.NodeEditType = NodeEditingType.Delete;
                int rowHandle = gridView_role_config.FocusedRowHandle;
                gridView_role_config.SetRowCellValue(rowHandle, gridView_role_config.Columns[" "], true);
                if (MessageBox.Show("确定删除选中的记录吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    int index = gridView_role_config.GetDataSourceRowIndex(gridView_role_config.FocusedRowHandle);
                    object uniqueCode = this.RoleDatatable.Rows[index]["Unique_code"];
                    UserDAO ud = new UserDAO();
                    ud.DeleteRole(uniqueCode);
                    this.RoleDatatable.Rows.RemoveAt(index);
                }
                else
                {
                    gridView_role_config.SetRowCellValue(rowHandle, gridView_role_config.Columns[" "], false);
                }
                this.IsNodeEditing = false;
                this.NodeEditType = NodeEditingType.Nothing;
            }
        }

        private void gridView_role_config_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName.Equals("dealingNode"))
            {
                object val = gridView_role_config.GetRowCellValue(e.RowHandle, gridView_role_config.Columns[" "]);
                if (val != DBNull.Value && val != null)
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

        void RowSave_Click(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            if (e.Button == editor.Properties.Buttons[0])
            {
                int index = gridView_role_config.GetDataSourceRowIndex(gridView_role_config.FocusedRowHandle);
                object colVal = this.RoleDatatable.Rows[index]["角色名"];
                if (string.IsNullOrEmpty(colVal.ToString()))
                {
                    MessageBox.Show("角色名不能为空！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (this.NodeEditType == NodeEditingType.Add)
                {
                    UserDAO ud = new UserDAO();
                    object lastUniqueCode = ud.AddRole(colVal);
                    this.RoleDatatable.Rows[index]["Unique_code"] = lastUniqueCode;//更新DATATABLE中的Unique_code
                }
                else if (this.NodeEditType == NodeEditingType.Update)
                {
                    object uniqueCode = this.RoleDatatable.Rows[index]["Unique_code"];
                    UserDAO ud = new UserDAO();
                    ud.UpdateRole(colVal, uniqueCode);
                }
                this.RoleDatatable.Rows[index][" "] = false;
                gridView_role_config.CloseEditor();
                this.IsNodeEditing = false;
                this.NodeEditType = NodeEditingType.Nothing;
                MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (e.Button == editor.Properties.Buttons[1])
            {
                if (this.NodeEditType == NodeEditingType.Add)
                {
                    int dr_deleting = this.RoleDatatable.Rows.Count - 1;//取消刚才添加的一条，此时未保存到数据库
                    this.RoleDatatable.Rows.RemoveAt(dr_deleting);
                }
                if (this.NodeEditType == NodeEditingType.Update)
                {
                    int rowHandle = gridView_role_config.FocusedRowHandle;
                    gridView_role_config.SetRowCellValue(rowHandle, gridView_role_config.Columns[" "], false);
                    gridView_role_config.CloseEditor();
                }
                this.IsNodeEditing = false;
                this.NodeEditType = NodeEditingType.Nothing;
            }
        }
    }
}