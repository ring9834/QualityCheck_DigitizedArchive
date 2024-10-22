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
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTreeList;
using System.Threading;
using System.Data.Common;
using System.Data.SqlClient;

namespace Prj_FileManageNCheckApp
{
    public partial class SystemCode_Config : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }
        public bool IsNodeEditing { get; set; }
        public NodeEditingType NodeEditType { get; set; }
        public UserEntity UserLoggedIn { get; set; }
        public SystemCode_Config()
        {
            InitializeComponent();
        }

        public SystemCode_Config(string fileCodeName, UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        private void SystemCode_Config_Load(object sender, EventArgs e)
        {
            LoadTree2();
            ShowAddLinkButton();
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            if (paras != null)
                this.FileCodeName = paras.FileCodeName;
        }

        private void LoadTree2()
        {
            treeList1.Columns.Clear();
            TreeListColumn tc1 = new TreeListColumn();
            tc1.FieldName = "codename";
            tc1.Caption = "类名";
            tc1.Width = 200;
            tc1.VisibleIndex = 0;//此为必须设置的属性，否则无法显示列

            TreeListColumn tc2 = new TreeListColumn();
            tc2.FieldName = "codekey";
            tc2.Caption = "类值";
            tc2.VisibleIndex = 1;

            TreeListColumn tc3 = new TreeListColumn();
            tc3.Caption = "顺序";
            tc3.FieldName = "OrderId";
            tc3.Width = 30;
            tc3.VisibleIndex = 2;

            TreeListColumn tc4 = new TreeListColumn();
            tc4.Caption = "说明";
            tc4.FieldName = "Comments";
            tc4.VisibleIndex = 3;

            TreeListColumn tc5 = new TreeListColumn();
            tc5.Caption = "唯一标识";
            tc5.FieldName = "UniqueCode";
            tc5.VisibleIndex = 5;
            treeList1.Columns.AddRange(new TreeListColumn[] { tc1, tc2, tc3, tc4, tc5 });

            //清空现有节点
            treeList1.Nodes.Clear();
            TreeListNode node = treeList1.AppendNode(new object[] { "系统编码库", "", "" }, null);
            //根据节点的字段ID名称查找定位节点
            //TreeListNode node = treeList1.FindNodeByFieldValue("Unique_code", "Allblanklist"); //在filedName列中查找所属值得的节点

            ConfigCodeDAO ccd = new ConfigCodeDAO();
            DataTable dt = ccd.GetCodeBases();
            if (dt.Rows.Count > 0)
            {
                DataView dv = new DataView(dt);
                //dv.RowFilter = "ParentId = '-1'";
                foreach (DataRowView dv1 in dv)
                {
                    object[] nodeview = { dv1["base_name"], dv1["code_key"], null, dv1["comments"], dv1["Unique_code"] };
                    TreeListNode tn = treeList1.AppendNode(nodeview, node);
                    DataTable dt2 = ccd.GetCodesByBase(dv1["Unique_code"].ToString());//获得辅助代码几类对应的分类列表
                    if (dt2.Rows.Count > 0)
                    {
                        DataView dv2 = new DataView(dt2);
                        foreach (DataRowView drv in dv2)
                        {
                            object[] nodeview2 = { drv["code_name"], drv["code_value"], drv["order_id"], null, drv["Unique_code"] };
                            TreeListNode tn2 = treeList1.AppendNode(nodeview2, tn);
                        }
                    }
                }
                node.ExpandAll();
            }
            dt.Dispose();
            //tc5.Visible = false;
        }

        private void ShowAddLinkButton()
        {
            RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();
            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.ButtonClick += riButtonEdit_DealWithNode_ButtonClick;
            riButtonEdit.Buttons.Clear();
            EditorButton bt0 = new EditorButton();
            bt0.Kind = ButtonPredefines.Glyph;
            bt0.Caption = "增加";
            bt0.ToolTip = "增加分类";
            bt0.Appearance.ForeColor = Color.Blue;
            bt0.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt0);


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

            TreeListColumn Col1 = new TreeListColumn();
            Col1.FieldName = "dealingNode";
            Col1.Caption = "操作";
            Col1.Visible = false;
            Col1.VisibleIndex = treeList1.Columns.Count;
            Col1.ColumnEdit = riButtonEdit;
            treeList1.RepositoryItems.Add(riButtonEdit);
            treeList1.Columns.Add(Col1);
            //foreach (TreeListColumn c in treeList1.Columns)
            //{
            //    c.OptionsColumn.AllowEdit = c.ColumnEdit is RepositoryItemButtonEdit;
            //}
            treeList1.BestFitColumns();
        }

        void riButtonEdit_DealWithNode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            TreeListNode node = treeList1.FocusedNode;
            if (e.Button == editor.Properties.Buttons[0])
            {
                if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
                this.IsNodeEditing = true;
                this.NodeEditType = NodeEditingType.Add;
                if (node.Level == 0)
                {
                    if (MessageBox.Show("将增加代码基类，确定吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        object[] nodeview = { null, null, null, null, null };
                        TreeListNode n = treeList1.AppendNode(nodeview, node);
                        n.CheckState = CheckState.Checked;
                    }
                    else
                        this.IsNodeEditing = false;
                }
                else if (node.Level == 1)
                {
                    DialogResult result = MessageBox.Show("增加代码基类请按“是”，增加代码分类请按“否”", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        object[] nodeview = { null, null, null, null, null };
                        TreeListNode n = treeList1.AppendNode(nodeview, node.ParentNode);
                        n.CheckState = CheckState.Checked;
                        n.ParentNode.ExpandAll();
                    }
                    else if (result == System.Windows.Forms.DialogResult.No)
                    {
                        object[] nodeview = { null, null, null, null, null };
                        TreeListNode n = treeList1.AppendNode(nodeview, node);
                        n.CheckState = CheckState.Checked;
                        n.ParentNode.ExpandAll();
                    }
                    else
                    {
                        this.IsNodeEditing = false;
                    }
                }
                else if (node.Level == 2)
                {
                    if (MessageBox.Show("将增加代码分类，确定吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        object[] nodeview = { null, null, null, null, null };
                        TreeListNode n = treeList1.AppendNode(nodeview, node.ParentNode);
                        n.CheckState = CheckState.Checked;
                        n.ParentNode.ExpandAll();
                    }
                    else
                        this.IsNodeEditing = false;
                }
            }
            else if (e.Button == editor.Properties.Buttons[1])
            {
                if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
                this.IsNodeEditing = true;
                this.NodeEditType = NodeEditingType.Update;
                node.CheckState = CheckState.Checked;
                treeList1.CloseEditor();

            }
            else if (e.Button == editor.Properties.Buttons[2])
            {
                if (this.IsNodeEditing) return;//正在编辑时，其他节点不能编辑
                this.IsNodeEditing = true;
                this.NodeEditType = NodeEditingType.Delete;
                node.CheckState = CheckState.Checked;
                string alertString = string.Empty;
                if (node.Level == 0)
                {
                    this.IsNodeEditing = false;
                    node.CheckState = CheckState.Unchecked;
                    MessageBox.Show("根节点不允许被删除", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (node.Level == 1)
                    alertString = "基类和分类都将被删除，确定吗？";
                else if (node.Level == 2)
                    alertString = "确定删除选中的类型吗？";

                ConfigCodeDAO ccd = new ConfigCodeDAO();
                if (MessageBox.Show(alertString, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (node.Level == 1)
                    {
                        object uniquecode = node.GetValue(4);
                        ccd.DeleteCode(uniquecode);//删除指定辅助编码和基类编码
                        for (int i = 0; i < node.Nodes.Count; i++)
                        {
                            node.Nodes.RemoveAt(i);
                        }
                    }
                    else if (node.Level == 2)
                    {
                        object uniquecode = node.GetValue(4);
                        ccd.DeleteCode2(uniquecode);//删除指定辅助编码
                    }
                    treeList1.Nodes.Remove(node);
                    this.IsNodeEditing = false;
                }
                else
                {
                    this.IsNodeEditing = false;
                    node.CheckState = CheckState.Unchecked;
                }
            }
        }

        private void treeList1_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
        {
            if (e.Column.FieldName.Equals("dealingNode"))
            {
                if (e.Node.Checked)
                {
                    if (this.IsNodeEditing)
                    {
                        if (this.NodeEditType == NodeEditingType.Add || this.NodeEditType == NodeEditingType.Update)
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
        }

        void NodeSave_Click(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            if (e.Button == editor.Properties.Buttons[0])
            {
                TreeListNode node = treeList1.FocusedNode;
                object v1 = node.GetValue(0);
                object v2 = node.GetValue(1);
                ConfigCodeDAO ccd = new ConfigCodeDAO();
                if (node.Level == 1)
                {
                    if (v1 == null || string.IsNullOrEmpty(v1.ToString()))
                    {
                        MessageBox.Show("基类“类名”不能为空！", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (v2 == null || string.IsNullOrEmpty(v2.ToString()))
                    {
                        MessageBox.Show("基类“类值”不能为空！", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    object commentsValue = node.GetValue(3);
                    string comments = commentsValue == null ? string.Empty : commentsValue.ToString();
                    
                    if (this.NodeEditType == NodeEditingType.Add)
                    {
                        object lastUniqueCode = ccd.AddCodeBase(v2.ToString(), v1.ToString(), comments);
                        node.SetValue(4, lastUniqueCode.ToString());
                        node.CheckState = CheckState.Unchecked;
                        treeList1.CloseEditor();
                    }
                    else if (this.NodeEditType == NodeEditingType.Update)
                    {
                        object uniquecode = node.GetValue(4);
                        ccd.UpdateCodeBase(v1.ToString(), comments, uniquecode);//更新辅助代码基类
                        node.CheckState = CheckState.Unchecked;
                        treeList1.CloseEditor();
                    }
                }
                else if (node.Level == 2)
                {
                    if (v1 == null || string.IsNullOrEmpty(v1.ToString()))
                    {
                        MessageBox.Show("分类“类名”不能为空！", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (v2 == null || string.IsNullOrEmpty(v2.ToString()))
                    {
                        MessageBox.Show("分类“类值”不能为空！", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (this.NodeEditType == NodeEditingType.Add)
                    {
                        object parentCode = node.ParentNode.GetValue(4);
                        object orderId = node.GetValue(2);
                        object order = orderId == null ? node.ParentNode.Nodes.Count.ToString() : orderId.ToString();
                        object lastUniqueCode = ccd.AddCode(parentCode, v1.ToString(), v2.ToString(), order);//增加辅助代码
                        node.SetValue(4, lastUniqueCode.ToString());
                        node.CheckState = CheckState.Unchecked;
                        treeList1.CloseEditor();
                    }
                    else if (this.NodeEditType == NodeEditingType.Update)
                    {
                        object uniquecode = node.GetValue(4);
                        object orderId = node.GetValue(2);
                        object order = orderId == null ? "-1" : orderId.ToString();
                        ccd.UpadteCode(v1.ToString(), v2.ToString(), order, uniquecode);//更新辅助代码
                        node.CheckState = CheckState.Unchecked;
                        treeList1.CloseEditor();
                    }
                }

                this.IsNodeEditing = false;//变成非编辑状态
            }
            else if (e.Button == editor.Properties.Buttons[1])
            {
                this.IsNodeEditing = false;
                if (this.NodeEditType == NodeEditingType.Add)
                    treeList1.Nodes.Remove(treeList1.FocusedNode);
                if (this.NodeEditType == NodeEditingType.Update)
                {
                    treeList1.FocusedNode.CheckState = CheckState.Unchecked;
                    treeList1.CloseEditor();
                }
            }
        }

        private void treeList1_BeforeCheckNode(object sender, CheckNodeEventArgs e)
        {
            if (this.IsNodeEditing)
            {
                e.CanCheck = false;
            }
            else
                e.CanCheck = true;
        }
    }
}