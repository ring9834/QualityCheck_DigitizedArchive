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
using DevExpress.XtraTreeList.Nodes;
using System.Data.Common;

namespace Prj_FileManageNCheckApp
{
    public delegate DataTable GetMenuDataTable();
    public partial class UserConfig_Access : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }
        public GetMenuDataTable GetMenuDataTableFunc { get; set; }
        public DataTable DtRole { get; set; }
        public bool IsNodeEditing { get; set; }
        public NodeEditingType NodeEditType { get; set; }
        public UserConfig_Access()
        {
            InitializeComponent();
        }

        public UserConfig_Access(string fileCodeName)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            if (paras != null)
                this.FileCodeName = paras.FileCodeName;
            LoadRoles();
        }

        private void UserConfig_Access_Load(object sender, EventArgs e)
        {

        }

        void LoadRoles()
        {
            UserDAO ud = new UserDAO();
            this.DtRole = ud.LoadRoles2();
            treeList1.DataSource = this.DtRole;
            treeList1.KeyFieldName = "Unique_code";
            treeList1.ParentFieldName = "parent_id";
            treeList1.Columns["role_name"].OptionsColumn.AllowEdit = false;
        }

        private void treeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            if (e.Node != null)
            {
                if (GetMenuDataTableFunc != null)
                {
                    string uniqueCode = e.Node.GetValue("Unique_code").ToString();
                    UserDAO ud = new UserDAO();
                    DataTable dt = ud.GetAccessesByRoleId(uniqueCode);
                    if (dt.Rows.Count == 0)
                    {
                        dt = GetMenuDataTableFunc();
                        dt.TableName = "t_config_access";
                        dt.Columns.Add("role_id", typeof(string));
                        for (int i = 0; i < dt.Rows.Count; i++)
                            dt.Rows[i]["role_id"] = uniqueCode;
                        new SqlHelper().SqlBulkCopyData(dt);
                        dt.Dispose();
                    }
                    DataTable dtFromDB = ud.GetAccessesByRoleId2(uniqueCode);
                    treeList2.DataSource = dtFromDB;
                    treeList2.KeyFieldName = "id";
                    treeList2.ParentFieldName = "parent_id";
                    treeList2.ExpandAll();
                    treeList2.Columns["Unique_code"].Visible = false;
                    treeList2.Columns["功能列表"].OptionsColumn.AllowEdit = false;
                    treeList2.Columns["功能触发ID"].OptionsColumn.AllowEdit = false;
                    treeList2.Columns["页面名称"].OptionsColumn.AllowEdit = false;
                    treeList2.Columns["角色"].OptionsColumn.AllowEdit = false;
                    if (treeList2.Columns["dealingNode"] == null)
                        ShowLinkButton();
                }
            }
        }

        private void ShowLinkButton()
        {
            RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();
            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.ButtonClick += riButtonEdit_DealWithNode_ButtonClick;
            riButtonEdit.Buttons.Clear();
            EditorButton bt0 = new EditorButton();
            bt0.Kind = ButtonPredefines.Glyph;
            bt0.Caption = "修改";
            bt0.ToolTip = "配置本行所示权限";
            bt0.Appearance.ForeColor = Color.Blue;
            bt0.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt0);

            TreeListColumn Col1 = new TreeListColumn();
            Col1.FieldName = "dealingNode";
            Col1.Caption = "操作";
            Col1.Visible = false;
            Col1.VisibleIndex = treeList2.Columns.Count;
            Col1.ColumnEdit = riButtonEdit;
            treeList2.RepositoryItems.Add(riButtonEdit);
            treeList2.Columns.Add(Col1);
            treeList2.BestFitColumns();
        }

        private void treeList2_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            if (e.Column.FieldName.Equals("dealingNode"))
            {
                if (e.Node.Checked)
                {
                    if (this.IsNodeEditing)
                    {
                        if (this.NodeEditType == NodeEditingType.Update)
                        {
                            DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit ritem = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
                            ritem.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
                            ritem.ButtonClick += NodeSave_Click;

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
            //if (e.Column.FieldName.Equals("可删除") && e.Column == treeList2.FocusedColumn)
            //{
            //    if (e.Node.Checked)
            //    {
            //        if (this.IsNodeEditing)
            //        {
            //            object valueObj = e.Node.GetValue("可删除");
            //            if (valueObj != DBNull.Value)
            //            {
            //                if (Boolean.Parse(valueObj.ToString()))
            //                {

            //                    e.Node.SetValue("可修改", true);
            //                    treeList2.CloseEditor();

            //                }
            //                else
            //                {
            //                    e.Node.SetValue("可修改", false);
            //                    treeList2.CloseEditor();
            //                }
            //            }
            //        }
            //    }
            //}
            //if (e.Column.FieldName.Equals("可修改") && e.Column == treeList2.FocusedColumn)
            //{
            //    if (e.Node.Checked)
            //    {
            //        if (this.IsNodeEditing)
            //        {
            //            object valueObj = e.Node.GetValue("可修改");
            //            if (valueObj != DBNull.Value)
            //            {
            //                if (Boolean.Parse(valueObj.ToString()))
            //                {
            //                    e.Node.SetValue("可增加", true);
            //                }
            //                else
            //                    e.Node.SetValue("可增加", false);
            //            }
            //        }
            //    }
            //}
            //if (e.Column.FieldName.Equals("可增加") && e.Column == treeList2.FocusedColumn)
            //{
            //    if (e.Node.Checked)
            //    {
            //        if (this.IsNodeEditing)
            //        {
            //            object valueObj = e.Node.GetValue("可增加");
            //            if (valueObj != DBNull.Value)
            //            {
            //                if (Boolean.Parse(valueObj.ToString()))
            //                    e.Node.SetValue("可搜索", true);
            //                else
            //                    e.Node.SetValue("可搜索", false);
            //            }
            //        }
            //    }
            //}
            //if (e.Column.FieldName.Equals("可搜索") && e.Column == treeList2.FocusedColumn)
            //{
            //    if (e.Node.Checked)
            //    {
            //        if (this.IsNodeEditing)
            //        {
            //            object valueObj = e.Node.GetValue("可搜索");
            //            if (valueObj != DBNull.Value)
            //            {
            //                if (Boolean.Parse(valueObj.ToString()))
            //                    e.Node.SetValue("可查看", true);
            //                else
            //                    e.Node.SetValue("可查看", false);
            //            }
            //        }
            //    }
            //}
        }

        void riButtonEdit_DealWithNode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            TreeListNode node = treeList2.FocusedNode;
            if (e.Button == editor.Properties.Buttons[0])
            {
                if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
                this.IsNodeEditing = true;
                this.NodeEditType = NodeEditingType.Update;
                node.CheckState = CheckState.Checked;
                treeList2.CloseEditor();
            }
        }

        void NodeSave_Click(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            if (e.Button == editor.Properties.Buttons[0])
            {
                object valueObj1 = treeList2.FocusedNode.GetValue("可查看");
                object valueObj2 = treeList2.FocusedNode.GetValue("可搜索");
                object valueObj3 = treeList2.FocusedNode.GetValue("可增加");
                object valueObj4 = treeList2.FocusedNode.GetValue("可修改");
                object valueObj5 = treeList2.FocusedNode.GetValue("可删除");
                object valueObj6 = treeList2.FocusedNode.GetValue("Unique_code");

                UserDAO ud = new UserDAO();
                ud.UpdateAccesses(valueObj1, valueObj2, valueObj3, valueObj4, valueObj5, valueObj6);

                MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                treeList2.FocusedNode.CheckState = CheckState.Unchecked;
                treeList2.CloseEditor();
                this.IsNodeEditing = false;
                this.NodeEditType = NodeEditingType.Nothing;

            }
            else if (e.Button == editor.Properties.Buttons[1])
            {
                treeList2.FocusedNode.CheckState = CheckState.Unchecked;
                treeList2.CloseEditor();
                this.IsNodeEditing = false;
                this.NodeEditType = NodeEditingType.Nothing;
            }
        }
    }
}