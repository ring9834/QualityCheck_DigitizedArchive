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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils;
using System.Data.Common;

namespace Prj_FileManageNCheckApp
{
    public partial class UserManagement : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }
        private DataTable UserDatatable { get; set; }
        public bool IsNodeEditing { get; set; }
        public NodeEditingType NodeEditType { get; set; }

        public UserManagement()
        {
            InitializeComponent();
        }

        public UserManagement(string fileCodeName)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            if (paras != null)
                this.FileCodeName = paras.FileCodeName;
        }

        private void UserManagement_Load(object sender, EventArgs e)
        {
            LoadUsers();
            ShowAddLinkButton();
        }

        void LoadUsers()
        {
            string fieldName = "user_name,nick_name,work_place,tel,role_id,password,Unique_code";
            UserDAO ud = new UserDAO();
            UserDatatable = ud.LoadUsers();
            gridControl_user_config.DataSource = UserDatatable;
            UserDatatable.Columns.Add(" ", typeof(bool));
            UserDatatable.Columns.Add("roleField", typeof(AttachedCodeClass));
            gridView_user_config.PopulateColumns();
            gridView_user_config.Columns[" "].VisibleIndex = 0;
            gridView_user_config.Columns[" "].OptionsColumn.AllowEdit = false;
            gridView_user_config.Columns["role_id"].Visible = false;
            gridView_user_config.Columns["Unique_code"].Visible = false;
            gridView_user_config.Columns["roleField"].Visible = false;
            gridView_user_config.Tag = fieldName;

            GridColumn roleTypeCol = new GridColumn();
            roleTypeCol.Caption = "所属角色";
            roleTypeCol.Visible = true;
            roleTypeCol.FieldName = "roleField";
            roleTypeCol.VisibleIndex = gridView_user_config.Columns.Count;

            DataTable dt_role = ud.LoadRoles2();
            roleComboBox.Items.Clear();
            for (int i = 0; i < dt_role.Rows.Count; i++)
            {
                DataRow dr = dt_role.Rows[i];
                AttachedCodeClass acc = new AttachedCodeClass(dr["role_name"].ToString(), null, dr["Unique_code"].ToString());
                roleComboBox.Items.Add(acc);
            }

            dt_role.Dispose();
            roleComboBox.TextEditStyle = TextEditStyles.DisableTextEditor;
            roleComboBox.SelectedIndexChanged += itemBox_SelectedIndexChanged;
            roleTypeCol.ColumnEdit = roleComboBox;
            gridView_user_config.Columns.Add(roleTypeCol);
            BindDataFromDb();
        }

        void BindDataFromDb()
        {
            for (int i = 0; i < this.UserDatatable.Rows.Count; i++)
            {
                int rowhandle = gridView_user_config.GetRowHandle(i);
                DataRow dr0 = gridView_user_config.GetDataRow(rowhandle);
                if (dr0["role_id"] != DBNull.Value)
                {
                    object obj = dr0["role_id"];
                    for (int j = 0; j < roleComboBox.Items.Count; j++)
                    {
                        AttachedCodeClass acc = (AttachedCodeClass)roleComboBox.Items[j];
                        if (acc.UniqueCode.Equals(obj.ToString()))
                        {
                            gridView_user_config.SetRowCellValue(rowhandle, gridView_user_config.Columns["roleField"], acc);
                        }
                    }
                }
            }
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
            bt2.ToolTip = "修改人员信息";
            bt2.Appearance.ForeColor = Color.Blue;
            bt2.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt2);

            EditorButton bt3 = new EditorButton();
            bt3.Kind = ButtonPredefines.Glyph;
            bt3.Caption = "删除";
            bt3.ToolTip = "删除人员信息";
            bt3.Appearance.ForeColor = Color.Blue;
            bt3.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt3);

            GridColumn Col1 = new GridColumn();
            Col1.FieldName = "dealingNode";
            Col1.Caption = "操作";
            Col1.Visible = false;
            Col1.VisibleIndex = gridView_user_config.Columns.Count;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_user_config.RepositoryItems.Add(riButtonEdit);
            gridView_user_config.Columns.Add(Col1);
            gridView_user_config.BestFitColumns();
        }

        void itemBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AttachedCodeClass obj = (AttachedCodeClass)((ComboBoxEdit)sender).SelectedItem;
            int index = gridView_user_config.GetDataSourceRowIndex(gridView_user_config.FocusedRowHandle);
            this.UserDatatable.Rows[index]["roleField"] = obj;
        }

        void riButtonEdit_DealWithNode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            if (e.Button == editor.Properties.Buttons[0])
            {
                if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
                this.IsNodeEditing = true;
                this.NodeEditType = NodeEditingType.Update;
                int rowHandle = gridView_user_config.FocusedRowHandle;
                gridView_user_config.SetRowCellValue(rowHandle, gridView_user_config.Columns[" "], true);
                gridView_user_config.CloseEditor();
            }
            else if (e.Button == editor.Properties.Buttons[1])
            {
                if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
                this.IsNodeEditing = true;
                this.NodeEditType = NodeEditingType.Delete;
                int rowHandle = gridView_user_config.FocusedRowHandle;
                gridView_user_config.SetRowCellValue(rowHandle, gridView_user_config.Columns[" "], true);
                if (MessageBox.Show("确定删除选中的记录吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    int index = gridView_user_config.GetDataSourceRowIndex(gridView_user_config.FocusedRowHandle);
                    object uniqueCode = this.UserDatatable.Rows[index]["Unique_code"];
                    UserDAO ud = new UserDAO();
                    ud.DeleteUser(uniqueCode);
                    this.UserDatatable.Rows.RemoveAt(index);
                }
                else
                {
                    gridView_user_config.SetRowCellValue(rowHandle, gridView_user_config.Columns[" "], false);
                }
                this.IsNodeEditing = false;
                this.NodeEditType = NodeEditingType.Nothing;
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
            this.IsNodeEditing = true;
            this.NodeEditType = NodeEditingType.Add;
            DataRow dr = MakeADataRow();
            this.UserDatatable.Rows.Add(dr);
            gridView_user_config.MoveLast();
            int rowHandle = gridView_user_config.FocusedRowHandle;
            gridView_user_config.SetRowCellValue(rowHandle, gridView_user_config.Columns[" "], true);
        }

        DataRow MakeADataRow()
        {
            DataRow dr = this.UserDatatable.NewRow();
            dr["Unique_code"] = -1;
            dr["用户名"] = string.Empty;
            dr["昵称"] = string.Empty;
            dr["工作单位"] = string.Empty;
            dr["电话"] = string.Empty;
            dr["密码"] = string.Empty;
            dr["role_id"] = string.Empty;
            return dr;
        }

        private void gridView_user_config_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName.Equals("dealingNode"))
            {
                object val = gridView_user_config.GetRowCellValue(e.RowHandle, gridView_user_config.Columns[" "]);
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
                int index = gridView_user_config.GetDataSourceRowIndex(gridView_user_config.FocusedRowHandle);
                object userName = this.UserDatatable.Rows[index]["用户名"];
                object roleObj = this.UserDatatable.Rows[index]["roleField"];
                object nickName = this.UserDatatable.Rows[index]["昵称"];
                object workPlace = this.UserDatatable.Rows[index]["工作单位"];
                object tel = this.UserDatatable.Rows[index]["电话"];
                object pwd = this.UserDatatable.Rows[index]["密码"];
                if (string.IsNullOrEmpty(userName.ToString()))
                {
                    MessageBox.Show("用户名不能为空！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (roleObj == null)
                {
                    MessageBox.Show("用户角色未选择！", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (this.NodeEditType == NodeEditingType.Add)
                {
                    UserDAO ud = new UserDAO();
                    object lastUniqueCode = ud.AddUser(userName, nickName, pwd, workPlace, tel, roleObj);
                    this.UserDatatable.Rows[index]["Unique_code"] = lastUniqueCode;//更新DATATABLE中的Unique_code
                }
                else if (this.NodeEditType == NodeEditingType.Update)
                {
                    object uniqueCode = this.UserDatatable.Rows[index]["Unique_code"];
                    UserDAO ud = new UserDAO();
                    ud.UpdateUser(userName, nickName, pwd, workPlace, tel, roleObj, uniqueCode);
                }
                this.UserDatatable.Rows[index][" "] = false;
                gridView_user_config.CloseEditor();
                this.IsNodeEditing = false;
                this.NodeEditType = NodeEditingType.Nothing;
                MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (e.Button == editor.Properties.Buttons[1])
            {
                if (this.NodeEditType == NodeEditingType.Add)
                {
                    int dr_deleting = this.UserDatatable.Rows.Count - 1;//取消刚才添加的一条，此时未保存到数据库
                    this.UserDatatable.Rows.RemoveAt(dr_deleting);
                }
                if (this.NodeEditType == NodeEditingType.Update)
                {
                    int rowHandle = gridView_user_config.FocusedRowHandle;
                    gridView_user_config.SetRowCellValue(rowHandle, gridView_user_config.Columns[" "], false);
                    gridView_user_config.CloseEditor();
                }
                this.IsNodeEditing = false;
                this.NodeEditType = NodeEditingType.Nothing;
            }
        }
    }
}