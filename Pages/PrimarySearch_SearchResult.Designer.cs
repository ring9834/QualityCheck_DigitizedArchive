namespace Prj_FileManageNCheckApp
{
    partial class PrimarySearch_SearchResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrimarySearch_SearchResult));
            this.xtraTabControl_archiveSearchManage = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTab_primarySearchResult = new DevExpress.XtraTab.XtraTabPage();
            this.groupControl_HintInfo = new DevExpress.XtraEditors.GroupControl();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.xtraTab_primarysearchRecord = new DevExpress.XtraTab.XtraTabPage();
            this.xtraPagedNavigation1 = new Prj_FileManageNCheckApp.XtraPagedNavigation();
            this.gridControl_primarysearch_records = new DevExpress.XtraGrid.GridControl();
            this.gridView_primarysearch_records = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.xtraGoFrontBack1 = new Prj_FileManageNCheckApp.XtraGoFrontBack();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl_archiveSearchManage)).BeginInit();
            this.xtraTabControl_archiveSearchManage.SuspendLayout();
            this.xtraTab_primarySearchResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl_HintInfo)).BeginInit();
            this.groupControl_HintInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            this.xtraTab_primarysearchRecord.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl_primarysearch_records)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_primarysearch_records)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // xtraTabControl_archiveSearchManage
            // 
            this.xtraTabControl_archiveSearchManage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.xtraTabControl_archiveSearchManage.Location = new System.Drawing.Point(3, 35);
            this.xtraTabControl_archiveSearchManage.Name = "xtraTabControl_archiveSearchManage";
            this.xtraTabControl_archiveSearchManage.SelectedTabPage = this.xtraTab_primarySearchResult;
            this.xtraTabControl_archiveSearchManage.Size = new System.Drawing.Size(1385, 621);
            this.xtraTabControl_archiveSearchManage.TabIndex = 1;
            this.xtraTabControl_archiveSearchManage.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTab_primarySearchResult,
            this.xtraTab_primarysearchRecord});
            this.xtraTabControl_archiveSearchManage.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl_archiveSearchManage_SelectedPageChanged);
            // 
            // xtraTab_primarySearchResult
            // 
            this.xtraTab_primarySearchResult.Controls.Add(this.groupControl_HintInfo);
            this.xtraTab_primarySearchResult.Name = "xtraTab_primarySearchResult";
            this.xtraTab_primarySearchResult.Size = new System.Drawing.Size(1379, 592);
            this.xtraTab_primarySearchResult.Text = "xtraTabPage1";
            // 
            // groupControl_HintInfo
            // 
            this.groupControl_HintInfo.Controls.Add(this.pictureEdit1);
            this.groupControl_HintInfo.Controls.Add(this.labelControl1);
            this.groupControl_HintInfo.Location = new System.Drawing.Point(436, 221);
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
            // xtraTab_primarysearchRecord
            // 
            this.xtraTab_primarysearchRecord.Controls.Add(this.xtraPagedNavigation1);
            this.xtraTab_primarysearchRecord.Controls.Add(this.gridControl_primarysearch_records);
            this.xtraTab_primarysearchRecord.Name = "xtraTab_primarysearchRecord";
            this.xtraTab_primarysearchRecord.Size = new System.Drawing.Size(1379, 592);
            this.xtraTab_primarysearchRecord.Text = "xtraTabPage3";
            // 
            // xtraPagedNavigation1
            // 
            this.xtraPagedNavigation1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.xtraPagedNavigation1.ExportAlltoFileSqlString = null;
            this.xtraPagedNavigation1.FieldString = null;
            this.xtraPagedNavigation1.InvisibleFields = ((System.Collections.Generic.List<string>)(resources.GetObject("xtraPagedNavigation1.InvisibleFields")));
            this.xtraPagedNavigation1.Location = new System.Drawing.Point(0, 563);
            this.xtraPagedNavigation1.Name = "xtraPagedNavigation1";
            this.xtraPagedNavigation1.PageCount = 0;
            this.xtraPagedNavigation1.PagedEventHandler = null;
            this.xtraPagedNavigation1.PagedGridView = null;
            this.xtraPagedNavigation1.PageIndex = 0;
            this.xtraPagedNavigation1.PageSize = 0;
            this.xtraPagedNavigation1.Size = new System.Drawing.Size(1379, 29);
            this.xtraPagedNavigation1.SortString = null;
            this.xtraPagedNavigation1.TabIndex = 4;
            this.xtraPagedNavigation1.TableString = null;
            this.xtraPagedNavigation1.WhereFieldArray = null;
            this.xtraPagedNavigation1.WhereFieldValueArray = null;
            this.xtraPagedNavigation1.WhereString = null;
            // 
            // gridControl_primarysearch_records
            // 
            this.gridControl_primarysearch_records.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl_primarysearch_records.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
            this.gridControl_primarysearch_records.Location = new System.Drawing.Point(-1, 2);
            this.gridControl_primarysearch_records.MainView = this.gridView_primarysearch_records;
            this.gridControl_primarysearch_records.Margin = new System.Windows.Forms.Padding(2);
            this.gridControl_primarysearch_records.Name = "gridControl_primarysearch_records";
            this.gridControl_primarysearch_records.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gridControl_primarysearch_records.Size = new System.Drawing.Size(1378, 556);
            this.gridControl_primarysearch_records.TabIndex = 3;
            this.gridControl_primarysearch_records.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView_primarysearch_records});
            // 
            // gridView_primarysearch_records
            // 
            this.gridView_primarysearch_records.GridControl = this.gridControl_primarysearch_records;
            this.gridView_primarysearch_records.Name = "gridView_primarysearch_records";
            this.gridView_primarysearch_records.OptionsView.ShowGroupPanel = false;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
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
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.xtraGoFrontBack1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1386, 31);
            this.panelControl1.TabIndex = 2;
            // 
            // PrimarySearch_SearchResult
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1386, 657);
            this.Controls.Add(this.xtraTabControl_archiveSearchManage);
            this.Controls.Add(this.panelControl1);
            this.Name = "PrimarySearch_SearchResult";
            this.Text = "数据模糊搜索";
            this.Load += new System.EventHandler(this.PrimarySearch_SearchResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl_archiveSearchManage)).EndInit();
            this.xtraTabControl_archiveSearchManage.ResumeLayout(false);
            this.xtraTab_primarySearchResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl_HintInfo)).EndInit();
            this.groupControl_HintInfo.ResumeLayout(false);
            this.groupControl_HintInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            this.xtraTab_primarysearchRecord.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl_primarysearch_records)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_primarysearch_records)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl xtraTabControl_archiveSearchManage;
        private DevExpress.XtraTab.XtraTabPage xtraTab_primarySearchResult;
        private DevExpress.XtraTab.XtraTabPage xtraTab_primarysearchRecord;
        private DevExpress.XtraGrid.GridControl gridControl_primarysearch_records;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView_primarysearch_records;
        private XtraPagedNavigation xtraPagedNavigation1;
        private XtraGoFrontBack xtraGoFrontBack1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.GroupControl groupControl_HintInfo;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}