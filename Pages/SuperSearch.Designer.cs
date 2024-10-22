namespace Prj_FileManageNCheckApp
{
    partial class SuperSearch
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SuperSearch));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.xtraGoFrontBack1 = new Prj_FileManageNCheckApp.XtraGoFrontBack();
            this.xtraTabControl_superSearch = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTab_superSearchCondition = new DevExpress.XtraTab.XtraTabPage();
            this.groupControl_HintInfo = new DevExpress.XtraEditors.GroupControl();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.xtraTab_superSearchRecord = new DevExpress.XtraTab.XtraTabPage();
            this.xtraPagedNavigation1 = new Prj_FileManageNCheckApp.XtraPagedNavigation();
            this.gridControl_SuperSearch = new DevExpress.XtraGrid.GridControl();
            this.gridView_superSearch = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl_superSearch)).BeginInit();
            this.xtraTabControl_superSearch.SuspendLayout();
            this.xtraTab_superSearchCondition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl_HintInfo)).BeginInit();
            this.groupControl_HintInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            this.xtraTab_superSearchRecord.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl_SuperSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_superSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.xtraGoFrontBack1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1362, 31);
            this.panelControl1.TabIndex = 3;
            // 
            // xtraGoFrontBack1
            // 
            this.xtraGoFrontBack1.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.xtraGoFrontBack1.Appearance.Options.UseBackColor = true;
            this.xtraGoFrontBack1.Location = new System.Drawing.Point(7, 2);
            this.xtraGoFrontBack1.Name = "xtraGoFrontBack1";
            this.xtraGoFrontBack1.Size = new System.Drawing.Size(70, 29);
            this.xtraGoFrontBack1.TabControl = null;
            this.xtraGoFrontBack1.TabIndex = 2;
            this.xtraGoFrontBack1.UpdateBackButtonStateFun = null;
            // 
            // xtraTabControl_superSearch
            // 
            this.xtraTabControl_superSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xtraTabControl_superSearch.Location = new System.Drawing.Point(1, 34);
            this.xtraTabControl_superSearch.Name = "xtraTabControl_superSearch";
            this.xtraTabControl_superSearch.SelectedTabPage = this.xtraTab_superSearchCondition;
            this.xtraTabControl_superSearch.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            this.xtraTabControl_superSearch.Size = new System.Drawing.Size(1361, 667);
            this.xtraTabControl_superSearch.TabIndex = 4;
            this.xtraTabControl_superSearch.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTab_superSearchCondition,
            this.xtraTab_superSearchRecord});
            this.xtraTabControl_superSearch.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl_superSearch_SelectedPageChanged);
            // 
            // xtraTab_superSearchCondition
            // 
            this.xtraTab_superSearchCondition.Controls.Add(this.groupControl_HintInfo);
            this.xtraTab_superSearchCondition.Name = "xtraTab_superSearchCondition";
            this.xtraTab_superSearchCondition.Size = new System.Drawing.Size(1355, 661);
            this.xtraTab_superSearchCondition.Text = "xtraTabPage1";
            // 
            // groupControl_HintInfo
            // 
            this.groupControl_HintInfo.Controls.Add(this.pictureEdit1);
            this.groupControl_HintInfo.Controls.Add(this.labelControl1);
            this.groupControl_HintInfo.Location = new System.Drawing.Point(418, 486);
            this.groupControl_HintInfo.Name = "groupControl_HintInfo";
            this.groupControl_HintInfo.Size = new System.Drawing.Size(466, 125);
            this.groupControl_HintInfo.TabIndex = 2;
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.EditValue = ((object)(resources.GetObject("pictureEdit1.EditValue")));
            this.pictureEdit1.Location = new System.Drawing.Point(5, 28);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.pictureEdit1.Properties.Appearance.Options.UseBackColor = true;
            this.pictureEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit1.Properties.ZoomPercent = 120D;
            this.pictureEdit1.Size = new System.Drawing.Size(57, 54);
            this.pictureEdit1.TabIndex = 2;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(68, 35);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(382, 56);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "此种档案类型库还未进行检索配置，请到“配置管理”页进行配置，步骤：\r\n1、点击页面最上方的“管理配置”；\r\n2、点击“检索配置”；\r\n3、选择可以进行检索的字段。" +
    "";
            // 
            // xtraTab_superSearchRecord
            // 
            this.xtraTab_superSearchRecord.Controls.Add(this.xtraPagedNavigation1);
            this.xtraTab_superSearchRecord.Controls.Add(this.gridControl_SuperSearch);
            this.xtraTab_superSearchRecord.Name = "xtraTab_superSearchRecord";
            this.xtraTab_superSearchRecord.Size = new System.Drawing.Size(1355, 661);
            this.xtraTab_superSearchRecord.Text = "xtraTabPage3";
            // 
            // xtraPagedNavigation1
            // 
            this.xtraPagedNavigation1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.xtraPagedNavigation1.ExportAlltoFileSqlString = null;
            this.xtraPagedNavigation1.FieldString = null;
            this.xtraPagedNavigation1.InvisibleFields = ((System.Collections.Generic.List<string>)(resources.GetObject("xtraPagedNavigation1.InvisibleFields")));
            this.xtraPagedNavigation1.Location = new System.Drawing.Point(0, 632);
            this.xtraPagedNavigation1.Name = "xtraPagedNavigation1";
            this.xtraPagedNavigation1.PageCount = 0;
            this.xtraPagedNavigation1.PagedEventHandler = null;
            this.xtraPagedNavigation1.PagedGridView = null;
            this.xtraPagedNavigation1.PageIndex = 0;
            this.xtraPagedNavigation1.PageSize = 0;
            this.xtraPagedNavigation1.Size = new System.Drawing.Size(1355, 29);
            this.xtraPagedNavigation1.SortString = null;
            this.xtraPagedNavigation1.TabIndex = 4;
            this.xtraPagedNavigation1.TableString = null;
            this.xtraPagedNavigation1.WhereFieldArray = null;
            this.xtraPagedNavigation1.WhereFieldValueArray = null;
            this.xtraPagedNavigation1.WhereString = null;
            // 
            // gridControl_SuperSearch
            // 
            this.gridControl_SuperSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl_SuperSearch.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
            this.gridControl_SuperSearch.Location = new System.Drawing.Point(-1, 1);
            this.gridControl_SuperSearch.MainView = this.gridView_superSearch;
            this.gridControl_SuperSearch.Margin = new System.Windows.Forms.Padding(2);
            this.gridControl_SuperSearch.Name = "gridControl_SuperSearch";
            this.gridControl_SuperSearch.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gridControl_SuperSearch.Size = new System.Drawing.Size(1354, 630);
            this.gridControl_SuperSearch.TabIndex = 3;
            this.gridControl_SuperSearch.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView_superSearch});
            // 
            // gridView_superSearch
            // 
            this.gridView_superSearch.GridControl = this.gridControl_SuperSearch;
            this.gridView_superSearch.Name = "gridView_superSearch";
            this.gridView_superSearch.OptionsView.ShowGroupPanel = false;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // SuperSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1362, 700);
            this.Controls.Add(this.xtraTabControl_superSearch);
            this.Controls.Add(this.panelControl1);
            this.Name = "SuperSearch";
            this.Text = "高级搜索";
            this.Load += new System.EventHandler(this.SuperSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl_superSearch)).EndInit();
            this.xtraTabControl_superSearch.ResumeLayout(false);
            this.xtraTab_superSearchCondition.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl_HintInfo)).EndInit();
            this.groupControl_HintInfo.ResumeLayout(false);
            this.groupControl_HintInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            this.xtraTab_superSearchRecord.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl_SuperSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_superSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private XtraGoFrontBack xtraGoFrontBack1;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl_superSearch;
        private DevExpress.XtraTab.XtraTabPage xtraTab_superSearchCondition;
        private DevExpress.XtraTab.XtraTabPage xtraTab_superSearchRecord;
        private XtraPagedNavigation xtraPagedNavigation1;
        private DevExpress.XtraGrid.GridControl gridControl_SuperSearch;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView_superSearch;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.GroupControl groupControl_HintInfo;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}