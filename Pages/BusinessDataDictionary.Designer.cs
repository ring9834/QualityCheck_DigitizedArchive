namespace Prj_FileManageNCheckApp
{
    partial class BusinessDataDictionary
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BusinessDataDictionary));
            this.gridControl_businessDictionary_config = new DevExpress.XtraGrid.GridControl();
            this.gridView_businessDictionary_config = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dataTypeComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl_businessDictionary_config)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_businessDictionary_config)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTypeComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridControl_businessDictionary_config
            // 
            this.gridControl_businessDictionary_config.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl_businessDictionary_config.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
            this.gridControl_businessDictionary_config.Location = new System.Drawing.Point(1, 34);
            this.gridControl_businessDictionary_config.MainView = this.gridView_businessDictionary_config;
            this.gridControl_businessDictionary_config.Margin = new System.Windows.Forms.Padding(2);
            this.gridControl_businessDictionary_config.Name = "gridControl_businessDictionary_config";
            this.gridControl_businessDictionary_config.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.dataTypeComboBox});
            this.gridControl_businessDictionary_config.Size = new System.Drawing.Size(1222, 570);
            this.gridControl_businessDictionary_config.TabIndex = 5;
            this.gridControl_businessDictionary_config.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView_businessDictionary_config});
            // 
            // gridView_businessDictionary_config
            // 
            this.gridView_businessDictionary_config.GridControl = this.gridControl_businessDictionary_config;
            this.gridView_businessDictionary_config.Name = "gridView_businessDictionary_config";
            this.gridView_businessDictionary_config.OptionsView.ShowGroupPanel = false;
            this.gridView_businessDictionary_config.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridView_publicDictionary_config_CustomRowCellEdit);
            this.gridView_businessDictionary_config.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gridView_publicDictionary_config_ShowingEditor);
            // 
            // dataTypeComboBox
            // 
            this.dataTypeComboBox.AutoHeight = false;
            this.dataTypeComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dataTypeComboBox.Name = "dataTypeComboBox";
            // 
            // bar1
            // 
            this.bar1.BarName = "Custom 1";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.Text = "Custom 1";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.simpleButton1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1223, 34);
            this.panelControl1.TabIndex = 6;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton1.Image")));
            this.simpleButton1.Location = new System.Drawing.Point(11, 5);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "增加记录";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.labelControl1);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl2.Location = new System.Drawing.Point(0, 604);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(1223, 30);
            this.panelControl2.TabIndex = 7;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 8F);
            this.labelControl1.Appearance.ForeColor = System.Drawing.Color.Red;
            this.labelControl1.Location = new System.Drawing.Point(8, 7);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(404, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "说明：操作“字段类型”时，选择，表示是管理字段，不选择，表示业务字段。";
            // 
            // BusinessDataDictionary
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1223, 634);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.gridControl_businessDictionary_config);
            this.Name = "BusinessDataDictionary";
            this.Text = "业务数据字典";
            this.Load += new System.EventHandler(this.PublicDataDictionary_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl_businessDictionary_config)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_businessDictionary_config)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTypeComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl_businessDictionary_config;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView_businessDictionary_config;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox dataTypeComboBox;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}