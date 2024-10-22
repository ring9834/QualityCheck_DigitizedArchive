namespace Prj_FileManageNCheckApp
{
    partial class XtraPagedNavigation
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XtraPagedNavigation));
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.simpleButton_exportall = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton_export = new DevExpress.XtraEditors.SimpleButton();
            this.label_info = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txEdit_Jump = new DevExpress.XtraEditors.TextEdit();
            this.lastPageBt = new DevExpress.XtraEditors.SimpleButton();
            this.followingPageBt = new DevExpress.XtraEditors.SimpleButton();
            this.prePageBt = new DevExpress.XtraEditors.SimpleButton();
            this.firstPageBt = new DevExpress.XtraEditors.SimpleButton();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.panelControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txEdit_Jump.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl3
            // 
            this.panelControl3.Controls.Add(this.simpleButton_exportall);
            this.panelControl3.Controls.Add(this.simpleButton_export);
            this.panelControl3.Controls.Add(this.label_info);
            this.panelControl3.Controls.Add(this.label8);
            this.panelControl3.Controls.Add(this.label9);
            this.panelControl3.Controls.Add(this.txEdit_Jump);
            this.panelControl3.Controls.Add(this.lastPageBt);
            this.panelControl3.Controls.Add(this.followingPageBt);
            this.panelControl3.Controls.Add(this.prePageBt);
            this.panelControl3.Controls.Add(this.firstPageBt);
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl3.Location = new System.Drawing.Point(0, -1);
            this.panelControl3.Margin = new System.Windows.Forms.Padding(2);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(1325, 30);
            this.panelControl3.TabIndex = 6;
            // 
            // simpleButton_exportall
            // 
            this.simpleButton_exportall.Location = new System.Drawing.Point(706, 4);
            this.simpleButton_exportall.Margin = new System.Windows.Forms.Padding(2);
            this.simpleButton_exportall.Name = "simpleButton_exportall";
            this.simpleButton_exportall.Size = new System.Drawing.Size(71, 22);
            this.simpleButton_exportall.TabIndex = 20;
            this.simpleButton_exportall.Text = "导出所有页";
            this.simpleButton_exportall.Click += new System.EventHandler(this.simpleButton_exportall_Click);
            // 
            // simpleButton_export
            // 
            this.simpleButton_export.Location = new System.Drawing.Point(632, 4);
            this.simpleButton_export.Margin = new System.Windows.Forms.Padding(2);
            this.simpleButton_export.Name = "simpleButton_export";
            this.simpleButton_export.Size = new System.Drawing.Size(71, 22);
            this.simpleButton_export.TabIndex = 19;
            this.simpleButton_export.Text = "导出当前页";
            this.simpleButton_export.Click += new System.EventHandler(this.simpleButton_export_Click);
            // 
            // label_info
            // 
            this.label_info.AutoSize = true;
            this.label_info.Location = new System.Drawing.Point(241, 8);
            this.label_info.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_info.Name = "label_info";
            this.label_info.Size = new System.Drawing.Size(203, 14);
            this.label_info.TabIndex = 18;
            this.label_info.Text = "（当前为第         页，共          页）";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(225, 8);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(19, 14);
            this.label8.TabIndex = 17;
            this.label8.Text = "页";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(141, 8);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 14);
            this.label9.TabIndex = 16;
            this.label9.Text = "跳转到第";
            // 
            // txEdit_Jump
            // 
            this.txEdit_Jump.Location = new System.Drawing.Point(196, 6);
            this.txEdit_Jump.Margin = new System.Windows.Forms.Padding(2);
            this.txEdit_Jump.Name = "txEdit_Jump";
            this.txEdit_Jump.Size = new System.Drawing.Size(26, 20);
            this.txEdit_Jump.TabIndex = 15;
            this.txEdit_Jump.ToolTip = "按ENTER键后开始跳转";
            this.txEdit_Jump.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txEdit_Jump_KeyPress);
            this.txEdit_Jump.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txEdit_Jump_KeyUp);
            // 
            // lastPageBt
            // 
            this.lastPageBt.Image = ((System.Drawing.Image)(resources.GetObject("lastPageBt.Image")));
            this.lastPageBt.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.lastPageBt.Location = new System.Drawing.Point(110, 3);
            this.lastPageBt.Margin = new System.Windows.Forms.Padding(2);
            this.lastPageBt.Name = "lastPageBt";
            this.lastPageBt.Size = new System.Drawing.Size(28, 23);
            this.lastPageBt.TabIndex = 14;
            this.lastPageBt.Click += new System.EventHandler(this.lastPageBt_Click);
            // 
            // followingPageBt
            // 
            this.followingPageBt.Image = ((System.Drawing.Image)(resources.GetObject("followingPageBt.Image")));
            this.followingPageBt.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.followingPageBt.Location = new System.Drawing.Point(77, 3);
            this.followingPageBt.Margin = new System.Windows.Forms.Padding(2);
            this.followingPageBt.Name = "followingPageBt";
            this.followingPageBt.Size = new System.Drawing.Size(28, 23);
            this.followingPageBt.TabIndex = 13;
            this.followingPageBt.Click += new System.EventHandler(this.followingPageBt_Click);
            // 
            // prePageBt
            // 
            this.prePageBt.Image = ((System.Drawing.Image)(resources.GetObject("prePageBt.Image")));
            this.prePageBt.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.prePageBt.Location = new System.Drawing.Point(45, 3);
            this.prePageBt.Margin = new System.Windows.Forms.Padding(2);
            this.prePageBt.Name = "prePageBt";
            this.prePageBt.Size = new System.Drawing.Size(28, 23);
            this.prePageBt.TabIndex = 12;
            this.prePageBt.Click += new System.EventHandler(this.prePageBt_Click);
            // 
            // firstPageBt
            // 
            this.firstPageBt.Image = ((System.Drawing.Image)(resources.GetObject("firstPageBt.Image")));
            this.firstPageBt.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.firstPageBt.Location = new System.Drawing.Point(14, 3);
            this.firstPageBt.Margin = new System.Windows.Forms.Padding(2);
            this.firstPageBt.Name = "firstPageBt";
            this.firstPageBt.Size = new System.Drawing.Size(28, 23);
            this.firstPageBt.TabIndex = 11;
            this.firstPageBt.Click += new System.EventHandler(this.firstPageBt_Click);
            // 
            // XtraPagedNavigation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl3);
            this.Name = "XtraPagedNavigation";
            this.Size = new System.Drawing.Size(1325, 29);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.panelControl3.ResumeLayout(false);
            this.panelControl3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txEdit_Jump.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl3;
        private DevExpress.XtraEditors.SimpleButton simpleButton_exportall;
        private DevExpress.XtraEditors.SimpleButton simpleButton_export;
        private System.Windows.Forms.Label label_info;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private DevExpress.XtraEditors.TextEdit txEdit_Jump;
        private DevExpress.XtraEditors.SimpleButton lastPageBt;
        private DevExpress.XtraEditors.SimpleButton followingPageBt;
        private DevExpress.XtraEditors.SimpleButton prePageBt;
        private DevExpress.XtraEditors.SimpleButton firstPageBt;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}
