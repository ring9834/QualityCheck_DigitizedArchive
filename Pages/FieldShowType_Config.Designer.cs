namespace Prj_FileManageNCheckApp
{
    partial class FieldShowType_Config
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FieldShowType_Config));
            this.gridControl_fieldShowType_config = new DevExpress.XtraGrid.GridControl();
            this.gridView_fieldShowType_config = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.allowNullCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.attachedCodeComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.FieldShowTypeComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar3 = new DevExpress.XtraBars.Bar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.standaloneBarDockControl2 = new DevExpress.XtraBars.StandaloneBarDockControl();
            this.simpleButton_BeginGJ = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl_fieldShowType_config)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_fieldShowType_config)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.allowNullCheckEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.attachedCodeComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FieldShowTypeComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl_fieldShowType_config
            // 
            this.gridControl_fieldShowType_config.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl_fieldShowType_config.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
            this.gridControl_fieldShowType_config.Location = new System.Drawing.Point(-1, 39);
            this.gridControl_fieldShowType_config.MainView = this.gridView_fieldShowType_config;
            this.gridControl_fieldShowType_config.Margin = new System.Windows.Forms.Padding(2);
            this.gridControl_fieldShowType_config.Name = "gridControl_fieldShowType_config";
            this.gridControl_fieldShowType_config.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.allowNullCheckEdit,
            this.attachedCodeComboBox,
            this.FieldShowTypeComboBox});
            this.gridControl_fieldShowType_config.Size = new System.Drawing.Size(1391, 689);
            this.gridControl_fieldShowType_config.TabIndex = 4;
            this.gridControl_fieldShowType_config.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView_fieldShowType_config});
            // 
            // gridView_fieldShowType_config
            // 
            this.gridView_fieldShowType_config.GridControl = this.gridControl_fieldShowType_config;
            this.gridView_fieldShowType_config.Name = "gridView_fieldShowType_config";
            this.gridView_fieldShowType_config.OptionsView.ShowGroupPanel = false;
            this.gridView_fieldShowType_config.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridView_fieldShowType_config_CustomRowCellEdit);
            this.gridView_fieldShowType_config.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gridView_fieldShowType_config_ShowingEditor);
            // 
            // allowNullCheckEdit
            // 
            this.allowNullCheckEdit.AutoHeight = false;
            this.allowNullCheckEdit.Name = "allowNullCheckEdit";
            // 
            // attachedCodeComboBox
            // 
            this.attachedCodeComboBox.AutoHeight = false;
            this.attachedCodeComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.attachedCodeComboBox.Name = "attachedCodeComboBox";
            // 
            // FieldShowTypeComboBox
            // 
            this.FieldShowTypeComboBox.AutoHeight = false;
            this.FieldShowTypeComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.FieldShowTypeComboBox.Name = "FieldShowTypeComboBox";
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar3});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.DockControls.Add(this.standaloneBarDockControl2);
            this.barManager1.Form = this;
            this.barManager1.MaxItemId = 0;
            this.barManager1.StatusBar = this.bar3;
            // 
            // bar3
            // 
            this.bar3.BarName = "Status bar";
            this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar3.DockCol = 0;
            this.bar3.DockRow = 0;
            this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar3.OptionsBar.AllowQuickCustomization = false;
            this.bar3.OptionsBar.DrawDragBorder = false;
            this.bar3.OptionsBar.UseWholeRow = true;
            this.bar3.Text = "Status bar";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1388, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 707);
            this.barDockControlBottom.Size = new System.Drawing.Size(1388, 23);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 707);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1388, 0);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 707);
            // 
            // standaloneBarDockControl2
            // 
            this.standaloneBarDockControl2.CausesValidation = false;
            this.standaloneBarDockControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.standaloneBarDockControl2.Location = new System.Drawing.Point(0, 0);
            this.standaloneBarDockControl2.Name = "standaloneBarDockControl2";
            this.standaloneBarDockControl2.Size = new System.Drawing.Size(1388, 34);
            this.standaloneBarDockControl2.Text = "standaloneBarDockControl2";
            // 
            // simpleButton_BeginGJ
            // 
            this.simpleButton_BeginGJ.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.simpleButton_BeginGJ.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton_BeginGJ.Image")));
            this.simpleButton_BeginGJ.Location = new System.Drawing.Point(10, 8);
            this.simpleButton_BeginGJ.Name = "simpleButton_BeginGJ";
            this.simpleButton_BeginGJ.Size = new System.Drawing.Size(65, 23);
            this.simpleButton_BeginGJ.TabIndex = 11;
            this.simpleButton_BeginGJ.Text = "保存";
            this.simpleButton_BeginGJ.Click += new System.EventHandler(this.simpleButton_BeginGJ_Click);
            // 
            // FieldShowType_Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1388, 730);
            this.Controls.Add(this.simpleButton_BeginGJ);
            this.Controls.Add(this.standaloneBarDockControl2);
            this.Controls.Add(this.gridControl_fieldShowType_config);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "FieldShowType_Config";
            this.Text = "编辑界面显示配置";
            ((System.ComponentModel.ISupportInitialize)(this.gridControl_fieldShowType_config)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_fieldShowType_config)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.allowNullCheckEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.attachedCodeComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FieldShowTypeComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl_fieldShowType_config;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView_fieldShowType_config;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit allowNullCheckEdit;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar3;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.StandaloneBarDockControl standaloneBarDockControl2;
        private DevExpress.XtraEditors.SimpleButton simpleButton_BeginGJ;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox attachedCodeComboBox;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox FieldShowTypeComboBox;
    }
}