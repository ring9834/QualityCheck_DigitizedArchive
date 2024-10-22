namespace Prj_FileManageNCheckApp
{
    partial class ShowRcdsForCheckingPageNumber
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
            this.gridControl_checkingPageNumber = new DevExpress.XtraGrid.GridControl();
            this.gridView_checkingPageNumber = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl_checkingPageNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_checkingPageNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl_checkingPageNumber
            // 
            this.gridControl_checkingPageNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl_checkingPageNumber.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
            this.gridControl_checkingPageNumber.Location = new System.Drawing.Point(0, 0);
            this.gridControl_checkingPageNumber.MainView = this.gridView_checkingPageNumber;
            this.gridControl_checkingPageNumber.Margin = new System.Windows.Forms.Padding(2);
            this.gridControl_checkingPageNumber.Name = "gridControl_checkingPageNumber";
            this.gridControl_checkingPageNumber.Size = new System.Drawing.Size(1302, 655);
            this.gridControl_checkingPageNumber.TabIndex = 4;
            this.gridControl_checkingPageNumber.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView_checkingPageNumber});
            // 
            // gridView_checkingPageNumber
            // 
            this.gridView_checkingPageNumber.GridControl = this.gridControl_checkingPageNumber;
            this.gridView_checkingPageNumber.Name = "gridView_checkingPageNumber";
            this.gridView_checkingPageNumber.OptionsView.ShowGroupPanel = false;
            // 
            // ShowRcdsForCheckingPageNumber
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1302, 656);
            this.Controls.Add(this.gridControl_checkingPageNumber);
            this.Name = "ShowRcdsForCheckingPageNumber";
            this.Text = "核对起止页或页数正确与否";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShowRcdsForCheckingPageNumber_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl_checkingPageNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_checkingPageNumber)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl_checkingPageNumber;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView_checkingPageNumber;
    }
}