namespace Prj_FileManageNCheckApp
{
    partial class UserManagement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserManagement));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.gridControl_user_config = new DevExpress.XtraGrid.GridControl();
            this.gridView_user_config = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.roleComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl_user_config)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_user_config)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.roleComboBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.simpleButton2);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1283, 34);
            this.panelControl1.TabIndex = 9;
            // 
            // simpleButton2
            // 
            this.simpleButton2.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton2.Image")));
            this.simpleButton2.Location = new System.Drawing.Point(11, 5);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(75, 23);
            this.simpleButton2.TabIndex = 0;
            this.simpleButton2.Text = "增加人员";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // gridControl_user_config
            // 
            this.gridControl_user_config.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl_user_config.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
            this.gridControl_user_config.Location = new System.Drawing.Point(1, 35);
            this.gridControl_user_config.MainView = this.gridView_user_config;
            this.gridControl_user_config.Margin = new System.Windows.Forms.Padding(2);
            this.gridControl_user_config.Name = "gridControl_user_config";
            this.gridControl_user_config.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.roleComboBox});
            this.gridControl_user_config.Size = new System.Drawing.Size(1282, 645);
            this.gridControl_user_config.TabIndex = 10;
            this.gridControl_user_config.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView_user_config});
            // 
            // gridView_user_config
            // 
            this.gridView_user_config.GridControl = this.gridControl_user_config;
            this.gridView_user_config.Name = "gridView_user_config";
            this.gridView_user_config.OptionsView.ShowGroupPanel = false;
            this.gridView_user_config.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridView_user_config_CustomRowCellEdit);
            // 
            // roleComboBox
            // 
            this.roleComboBox.AutoHeight = false;
            this.roleComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.roleComboBox.Name = "roleComboBox";
            // 
            // UserManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1283, 682);
            this.Controls.Add(this.gridControl_user_config);
            this.Controls.Add(this.panelControl1);
            this.Name = "UserManagement";
            this.Text = "用户配置";
            this.Load += new System.EventHandler(this.UserManagement_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl_user_config)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_user_config)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.roleComboBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraGrid.GridControl gridControl_user_config;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView_user_config;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox roleComboBox;

    }
}