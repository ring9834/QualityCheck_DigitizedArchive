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
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using System.Data.Common;

namespace Prj_FileManageNCheckApp
{
    public partial class ArchiveKinds_Config : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }
        public bool IsNodeEditing { get; set; }
        public NodeEditingType NodeEditType { get; set; }
        public DataTable DtOverAll { get; set; }
        public UserEntity UserLoggedIn { get; set; }

        public delegate void UpdateTreeListDelegate();
        public UpdateTreeListDelegate UpdateTreeListDelegateFunc { get; set; }
        public ArchiveKinds_Config()
        {
            InitializeComponent();
        }

        public ArchiveKinds_Config(string fileCodeName, UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        private void ArchiveKinds_Config_Load(object sender, EventArgs e)
        {

        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            //this.FileCodeName = paras.FileCodeName;
            GetContentTypeItems();
            LoadTree();
        }

        void LoadTree()
        {
            StorehouseKindDAO shkd = new StorehouseKindDAO();
            DtOverAll = shkd.LoadStoreHouseTree();
            DataColumn[] clos = new DataColumn[1];
            clos[0] = DtOverAll.Columns[0];
            //DtOverAll.PrimaryKey = clos;//设置DataTable主键
            DtOverAll.Columns.Add("原文存储类型", typeof(AttachedCodeClass));
            treeList1.DataSource = DtOverAll;
            treeList1.KeyFieldName = "Unique_code";
            treeList1.ParentFieldName = "super_id";
            treeList1.ExpandAll();
            treeList1.Columns["原文类型"].Visible = false;
            BindDataFromDb();
            ShowAddLinkButton();
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
            bt0.ToolTip = "增加节点";
            bt0.Appearance.ForeColor = Color.Blue;
            bt0.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt0);

            EditorButton bt2 = new EditorButton();
            bt2.Kind = ButtonPredefines.Glyph;
            bt2.Caption = "修改";
            bt2.ToolTip = "修改节点";
            bt2.Appearance.ForeColor = Color.Blue;
            bt2.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt2);

            EditorButton bt3 = new EditorButton();
            bt3.Kind = ButtonPredefines.Glyph;
            bt3.Caption = "删除";
            bt3.ToolTip = "删除节点";
            bt3.Appearance.ForeColor = Color.Blue;
            bt3.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt3);

            TreeListColumn Col1 = new TreeListColumn();
            Col1.FieldName = "dealingNode";
            Col1.Caption = "操作";
            Col1.Visible = false;
            Col1.VisibleIndex = treeList1.Columns.Count;
            Col1.ColumnEdit = riButtonEdit;

            treeList1.RepositoryItems.Clear();
            treeList1.RepositoryItems.Add(riButtonEdit);
            treeList1.Columns.Add(Col1);
            treeList1.BestFitColumns();
        }

        void BindDataFromDb()
        {
            for (int i = 0; i < this.DtOverAll.Rows.Count; i++)
            {
                DataRow dr0 = this.DtOverAll.Rows[i];
                if (dr0["原文类型"] != DBNull.Value)
                {
                    object obj = dr0["原文类型"];
                    for (int j = 0; j < contentTypeComboBox.Items.Count; j++)
                    {
                        AttachedCodeClass acc = (AttachedCodeClass)contentTypeComboBox.Items[j];
                        if (acc.UniqueCode.Equals(obj.ToString()))
                        {
                            List<TreeListNode> nodes = treeList1.GetNodeList();
                            TreeListNode node = nodes.Single(n => n.GetValue("Unique_code").ToString().Equals(dr0["Unique_code"].ToString()));
                            if (node != null)
                            {
                                node.SetValue("原文存储类型", acc);
                            }
                        }
                    }

                }
            }
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
                    object superIdObj = node.GetValue("Unique_code");
                    int superId = 0;
                    if (superIdObj != null) superId = int.Parse(superIdObj.ToString());
                    DataRow dr = MakeADataRow(superId);
                    this.DtOverAll.Rows.Add(dr);
                    //object[] nodeview = dr.ItemArray;
                    //TreeListNode n = treeList1.AppendNode(nodeview, node);
                    node.LastNode.CheckState = CheckState.Checked;
                    node.ExpandAll();
                }
                else
                {
                    object obj1 = node.GetValue("是否非空节点");
                    bool isNullNode = Boolean.Parse(obj1.ToString());
                    if (!isNullNode)//空节点
                    {
                        DialogResult result = MessageBox.Show("增加兄弟节点请按“是”，增加子节点请按“否”", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            object superIdObj = node.ParentNode.GetValue("Unique_code");
                            int superId = 0;
                            if (superIdObj != null) superId = int.Parse(superIdObj.ToString());
                            DataRow dr = MakeADataRow(superId);
                            this.DtOverAll.Rows.Add(dr);
                            TreeListNode n = node.ParentNode.LastNode;
                            n.CheckState = CheckState.Checked;
                            n.ParentNode.ExpandAll();
                        }
                        else if (result == System.Windows.Forms.DialogResult.No)
                        {
                            object superIdObj = node.GetValue("Unique_code");
                            int superId = 0;
                            if (superIdObj != null) superId = int.Parse(superIdObj.ToString());
                            DataRow dr = MakeADataRow(superId);
                            this.DtOverAll.Rows.Add(dr);
                            TreeListNode n = node.LastNode;
                            n.CheckState = CheckState.Checked;
                            n.ParentNode.ExpandAll();
                        }
                        else
                        {
                            this.IsNodeEditing = false;
                            this.NodeEditType = NodeEditingType.Nothing;
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("非空节点（档案业务节点）不能增加子节点，将增加兄弟节点，确定吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            object superIdObj = node.ParentNode.GetValue("Unique_code");
                            int superId = 0;
                            if (superIdObj != null) superId = int.Parse(superIdObj.ToString());
                            DataRow dr = MakeADataRow(superId);
                            this.DtOverAll.Rows.Add(dr);
                            TreeListNode n = node.ParentNode.LastNode;
                            n.CheckState = CheckState.Checked;
                            n.ParentNode.ExpandAll();
                        }
                        else
                        {
                            this.IsNodeEditing = false;
                            this.NodeEditType = NodeEditingType.Nothing;
                        }
                    }
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
                if (node.Level == 0)
                {
                    this.IsNodeEditing = false;
                    node.CheckState = CheckState.Unchecked;
                    MessageBox.Show("根节点不允许被删除", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string alertString = string.Empty;
                if (node.HasChildren)
                    alertString = "节点和子节点及其数据都将被删除，请谨慎操作，确定吗？";
                else
                    alertString = "确定删除选中的节点吗？";
                if (MessageBox.Show(alertString, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    StorehouseKindDAO shdk = new StorehouseKindDAO();
                    node.CheckState = CheckState.Unchecked;
                    if (node.HasChildren)
                    {
                        List<object> list = new List<object>();
                        for (int i = 0; i < node.Nodes.Count; i++)
                        {
                            object uniquecode = node.Nodes[i].GetValue("Unique_code");
                            string table = node.Nodes[i].GetValue("档案类型值").ToString();
                            shdk.ClearTempDataFromTables(table, uniquecode);//当删除一个档案库类型时，同时删除与之相关的表中数据
                            list.Add(uniquecode);

                        }
                        for (int i = 0; i < list.Count; i++)
                        {
                            RemoveRowFromDataTable(list[i].ToString());
                        }
                    }
                    object uniquecode0 = node.GetValue("Unique_code");
                    string table0 = node.GetValue("档案类型值").ToString();
                    shdk.ClearTempDataFromTables(table0, uniquecode0);
                    RemoveRowFromDataTable(uniquecode0.ToString());
                    shdk.DropTablesWithTempName();//删除名带temp的临时表
                }
                else
                    node.CheckState = CheckState.Unchecked;
                this.IsNodeEditing = false;
                this.NodeEditType = NodeEditingType.Nothing;
                UpdateTreeListDelegateFunc();//更新档案类型库的树
            }
        }

        DataRow MakeADataRow(int superId)
        {
            DataRow dr = this.DtOverAll.NewRow();
            dr["Unique_code"] = -1;
            dr["super_id"] = superId;
            dr["档案类型名"] = DBNull.Value;
            dr["档案类型值"] = DBNull.Value;
            dr["是否非空节点"] = false;
            dr["排序"] = DBNull.Value;
            dr["是否有原文"] = false;
            dr["原文类型"] = DBNull.Value;
            dr["是否可用"] = true;
            return dr;
        }

        void RemoveRowFromDataTable(string uniqueCode)
        {
            for (int i = 0; i < this.DtOverAll.Rows.Count; i++)
            {
                DataRow dr = this.DtOverAll.Rows[i];
                if (dr["Unique_code"].ToString().Equals(uniqueCode))
                {
                    this.DtOverAll.Rows.Remove(dr);
                    return;
                }
            }
        }

        /// <summary>
        /// 使用RepositoryItemCheckEidt时用，绑定数据库时不用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void useFlagCheckEdit_QueryCheckStateByValue(object sender, DevExpress.XtraEditors.Controls.QueryCheckStateByValueEventArgs e)
        {
            string val = e.Value.ToString();
            switch (val)
            {
                case "True":
                case "Yes":
                case "1":
                    e.CheckState = CheckState.Checked;
                    break;
                case "False":
                case "No":
                case "0":
                    e.CheckState = CheckState.Unchecked;
                    break;
                default:
                    e.CheckState = CheckState.Indeterminate;
                    break;
            }
            e.Handled = true;
        }

        /// <summary>
        /// 使用RepositoryItemCheckEidt时用，绑定数据库时不用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void useFlagCheckEdit_QueryValueByCheckState(object sender, DevExpress.XtraEditors.Controls.QueryValueByCheckStateEventArgs e)
        {
            CheckEdit edit = sender as CheckEdit;
            object val = edit.EditValue;
            switch (e.CheckState)
            {
                case CheckState.Checked:
                    if (val is bool) e.Value = true;
                    else if (val is string) e.Value = "Yes";
                    else if (val is int) e.Value = 1;
                    else e.Value = null;
                    break;
                case CheckState.Unchecked:
                    if (val is bool) e.Value = false;
                    else if (val is string) e.Value = "No";
                    else if (val is int) e.Value = 0;
                    else e.Value = null;
                    break;
                default:
                    if (val is bool) e.Value = false;
                    else if (val is string) e.Value = "?";
                    else if (val is int) e.Value = -1;
                    else e.Value = null;
                    break;
            }
            e.Handled = true;
        }

        private void treeList1_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            if (this.IsNodeEditing)
            {
                e.CanCheck = false;
            }
            else
                e.CanCheck = true;
        }

        private void treeList1_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (treeList1.FocusedColumn.FieldName.Equals("原文存储类型"))
            {
                TreeListNode node = treeList1.FocusedNode;
                if (node != null)
                {
                    object obj = node.GetValue("是否有原文");
                    if (obj != DBNull.Value)
                    {
                        if (!Boolean.Parse(obj.ToString()))
                        {
                            e.Cancel = true;//单元格不允许编辑
                        }
                        else
                            e.Cancel = false;
                    }
                    else
                        e.Cancel = true;
                }
            }
            if (treeList1.FocusedColumn.FieldName.Equals("档案类型值"))
            {
                if (this.NodeEditType == NodeEditingType.Add)
                {
                    TreeListNode node = treeList1.FocusedNode;
                    if (node.Checked)
                    {
                        e.Cancel = false;
                    }
                    else
                        e.Cancel = true;
                }
                else
                    e.Cancel = true;
            }
        }

        private void treeList1_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
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

            if (e.Column.FieldName.Equals("原文存储类型"))
            {
                object obj = e.Node.GetValue("是否有原文");
                if (obj != DBNull.Value)
                {
                    if (Boolean.Parse(obj.ToString()))
                    {
                        e.RepositoryItem = contentTypeComboBox;
                    }
                    else
                    {
                        object display = e.Node.GetValue("原文存储类型");
                        if (!string.IsNullOrEmpty(display.ToString()))
                        {
                            e.Node.SetValue("原文存储类型", new AttachedCodeClass("", null, null));
                        }
                    }
                }
            }
        }

        void NodeSave_Click(object sender, ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            TreeListNode node = treeList1.FocusedNode;
            if (e.Button == editor.Properties.Buttons[0])
            {
                DataRow dr_saving = null;
                if (this.NodeEditType == NodeEditingType.Add)
                {
                    dr_saving = this.DtOverAll.Rows[this.DtOverAll.Rows.Count - 1];
                }
                else if (this.NodeEditType == NodeEditingType.Update)
                {
                    object unique_code_updating = node.GetValue("Unique_code");
                    for (int i = 0; i < this.DtOverAll.Rows.Count; i++)
                    {
                        string code_select = this.DtOverAll.Rows[i]["Unique_code"].ToString();
                        if (code_select.Equals(unique_code_updating.ToString()))
                        {
                            dr_saving = this.DtOverAll.Rows[i];
                        }
                    }
                }

                object obj1 = dr_saving["档案类型名"];
                object obj3 = dr_saving["排序"];
                object obj4 = dr_saving["原文存储类型"];

                if (obj1 == DBNull.Value || string.IsNullOrEmpty(obj1.ToString()))
                {
                    MessageBox.Show("档案类型名不能为空！", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (obj3 != DBNull.Value)
                {
                    if (!ValidateUtil.IsInt(obj3.ToString()))
                    {
                        MessageBox.Show("排序须为整数值，请重填！", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                if (this.NodeEditType == NodeEditingType.Add)
                {
                    object obj5 = dr_saving["是否有原文"];
                    object obj2 = dr_saving["档案类型值"];

                    if (obj2 == DBNull.Value || string.IsNullOrEmpty(obj2.ToString()))
                    {
                        MessageBox.Show("档案类型值不能为空！", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (Boolean.Parse(obj5.ToString()))
                    {
                        if (obj4 == DBNull.Value)
                        {
                            MessageBox.Show("原文存储类型不能为空，请选择！", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    object superId = dr_saving["super_id"];
                    object colNull = dr_saving["是否非空节点"];
                    object colUse = dr_saving["是否可用"];
                    object hasContent = dr_saving["是否有原文"];
                    object contentType = DBNull.Value;
                    if (obj4 != DBNull.Value)
                    {
                        AttachedCodeClass acc = (AttachedCodeClass)obj4;
                        contentType = acc.UniqueCode;
                    }
                    StorehouseKindDAO shkd = new StorehouseKindDAO();
                    object newUniqueId = shkd.AddStoreHouseNode(superId, obj1, obj2, colNull, colUse, obj3, hasContent, contentType);
                    int rowIndex = this.DtOverAll.Rows.Count - 1;
                    this.DtOverAll.Rows[rowIndex].SetField("Unique_code", newUniqueId);//更新节点的唯一标识
                    this.IsNodeEditing = false;
                    this.NodeEditType = NodeEditingType.Nothing;
                    node.CheckState = CheckState.Unchecked;
                    MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (this.NodeEditType == NodeEditingType.Update)
                {
                    object contentType = DBNull.Value;
                    if (obj4 != DBNull.Value)
                    {
                        AttachedCodeClass acc = (AttachedCodeClass)obj4;
                        contentType = acc.UniqueCode;
                    }
                    if (MessageBox.Show("只能更新档案类型名、原文类型和排序，确定吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        StorehouseKindDAO shkd = new StorehouseKindDAO();
                        shkd.UpdateStoreHouseNode(node.GetValue("Unique_code"), obj1, obj3, contentType);
                        MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    this.IsNodeEditing = false;
                    this.NodeEditType = NodeEditingType.Nothing;
                    node.CheckState = CheckState.Unchecked;
                }
                UpdateTreeListDelegateFunc();//更新档案类型库的树
            }
            else if (e.Button == editor.Properties.Buttons[1])
            {
                if (this.NodeEditType == NodeEditingType.Add)
                {
                    int dr_deleting = this.DtOverAll.Rows.Count - 1;//取消刚才添加的一条，此时未保存到数据库
                    this.DtOverAll.Rows.RemoveAt(dr_deleting);
                }
                if (this.NodeEditType == NodeEditingType.Update)
                {
                    treeList1.FocusedNode.CheckState = CheckState.Unchecked;
                    treeList1.CloseEditor();
                }
                this.IsNodeEditing = false;
                this.NodeEditType = NodeEditingType.Nothing;
            }
        }

        private void treeList1_CellValueChanging(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName.Equals("是否有原文"))
            {
                TreeListNode node = e.Node;
                object obj1 = node.GetValue("是否非空节点");
                if (obj1 != DBNull.Value)
                {
                    object obj2 = e.Value;
                    if (!Boolean.Parse(obj1.ToString()))
                    {
                        if (Boolean.Parse(obj2.ToString()))
                        {
                            MessageBox.Show("空节点不允许有原文！", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
            }
            else if (e.Column.FieldName.Equals("是否非空节点"))
            {
                TreeListNode node = e.Node;
                object obj = e.Value;
                if (node.Level == 0)
                {
                    if (Boolean.Parse(obj.ToString()))
                    {
                        MessageBox.Show("根节点不能为非空节点（档案业务类型节点）！", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    if (node.Nodes.Count > 0)
                    {
                        MessageBox.Show("有子节点的节点不能为非空节点（档案业务类型节点）！", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        object obj1 = node.GetValue("是否有原文");
                        if (obj1 != DBNull.Value)
                        {
                            if (Boolean.Parse(obj1.ToString()))
                            {
                                if (!Boolean.Parse(obj.ToString()))
                                {
                                    MessageBox.Show("空节点不允许有原文！", "警示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        void GetContentTypeItems()
        {
            if (contentTypeComboBox.Items.Count == 0)
            {
                ConfigCodeDAO ccd = new ConfigCodeDAO();
                DataTable dt = ccd.GetCodes("YWLX");
                contentTypeComboBox.Items.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    AttachedCodeClass fst = new AttachedCodeClass(dr["code_name"].ToString(), dr["code_value"].ToString(), dr["Unique_code"].ToString());
                    contentTypeComboBox.Items.Add(fst);
                }
                contentTypeComboBox.TextEditStyle = TextEditStyles.DisableTextEditor;
                dt.Dispose();
            }
        }

        private void treeList1_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {

        }

    }
}