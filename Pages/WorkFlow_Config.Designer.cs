namespace Prj_FileManageNCheckApp
{
    partial class WorkFlow_Config
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkFlow_Config));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.checkedListBoxControl_roleForWorkFlow_selected = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.checkedListBoxControl_config_roleForWorkFlow = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkedListBoxControl_roleForWorkFlow_selected)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkedListBoxControl_config_roleForWorkFlow)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(5, 44);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(902, 204);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.groupControl1);
            this.panelControl1.Controls.Add(this.simpleButton2);
            this.panelControl1.Controls.Add(this.simpleButton1);
            this.panelControl1.Controls.Add(this.flowLayoutPanel1);
            this.panelControl1.Location = new System.Drawing.Point(226, 49);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(912, 557);
            this.panelControl1.TabIndex = 1;
            // 
            // simpleButton2
            // 
            this.simpleButton2.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton2.Image")));
            this.simpleButton2.Location = new System.Drawing.Point(116, 15);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(81, 23);
            this.simpleButton2.TabIndex = 2;
            this.simpleButton2.Text = "保存流程";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // simpleButton1
            // 
            this.simpleButton1.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton1.Image")));
            this.simpleButton1.Location = new System.Drawing.Point(8, 15);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(102, 23);
            this.simpleButton1.TabIndex = 1;
            this.simpleButton1.Text = "增加流程节点";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // groupControl1
            // 
            this.groupControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.groupControl1.Appearance.Options.UseBackColor = true;
            this.groupControl1.Controls.Add(this.label2);
            this.groupControl1.Controls.Add(this.label1);
            this.groupControl1.Controls.Add(this.checkedListBoxControl_roleForWorkFlow_selected);
            this.groupControl1.Controls.Add(this.checkedListBoxControl_config_roleForWorkFlow);
            this.groupControl1.Controls.Add(this.button6);
            this.groupControl1.Controls.Add(this.button5);
            this.groupControl1.Controls.Add(this.button2);
            this.groupControl1.Controls.Add(this.button4);
            this.groupControl1.Location = new System.Drawing.Point(6, 254);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(901, 289);
            this.groupControl1.TabIndex = 62;
            this.groupControl1.Text = "配置哪些角色可以控制整个流程";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(451, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 14);
            this.label2.TabIndex = 61;
            this.label2.Text = "已选择的角色";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 14);
            this.label1.TabIndex = 60;
            this.label1.Text = "供选择的角色";
            // 
            // checkedListBoxControl_roleForWorkFlow_selected
            // 
            this.checkedListBoxControl_roleForWorkFlow_selected.AllowDrop = true;
            this.checkedListBoxControl_roleForWorkFlow_selected.CheckOnClick = true;
            this.checkedListBoxControl_roleForWorkFlow_selected.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.checkedListBoxControl_roleForWorkFlow_selected.Location = new System.Drawing.Point(454, 51);
            this.checkedListBoxControl_roleForWorkFlow_selected.Margin = new System.Windows.Forms.Padding(2);
            this.checkedListBoxControl_roleForWorkFlow_selected.Name = "checkedListBoxControl_roleForWorkFlow_selected";
            this.checkedListBoxControl_roleForWorkFlow_selected.Size = new System.Drawing.Size(261, 217);
            this.checkedListBoxControl_roleForWorkFlow_selected.TabIndex = 52;
            // 
            // checkedListBoxControl_config_roleForWorkFlow
            // 
            this.checkedListBoxControl_config_roleForWorkFlow.AllowDrop = true;
            this.checkedListBoxControl_config_roleForWorkFlow.CheckOnClick = true;
            this.checkedListBoxControl_config_roleForWorkFlow.Location = new System.Drawing.Point(20, 51);
            this.checkedListBoxControl_config_roleForWorkFlow.Margin = new System.Windows.Forms.Padding(2);
            this.checkedListBoxControl_config_roleForWorkFlow.Name = "checkedListBoxControl_config_roleForWorkFlow";
            this.checkedListBoxControl_config_roleForWorkFlow.Size = new System.Drawing.Size(262, 217);
            this.checkedListBoxControl_config_roleForWorkFlow.TabIndex = 51;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(334, 164);
            this.button6.Margin = new System.Windows.Forms.Padding(2);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(63, 23);
            this.button6.TabIndex = 55;
            this.button6.Text = "全部复位";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(334, 128);
            this.button5.Margin = new System.Windows.Forms.Padding(2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(63, 23);
            this.button5.TabIndex = 54;
            this.button5.Text = "<<复位";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(334, 204);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(63, 23);
            this.button2.TabIndex = 57;
            this.button2.Text = "保存";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(334, 92);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(63, 23);
            this.button4.TabIndex = 53;
            this.button4.Text = "选择>>";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // WorkFlow_Config
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1369, 666);
            this.Controls.Add(this.panelControl1);
            this.Name = "WorkFlow_Config";
            this.Text = "数字化数据提交流程";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.WorkFlow_Config_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkedListBoxControl_roleForWorkFlow_selected)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkedListBoxControl_config_roleForWorkFlow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.CheckedListBoxControl checkedListBoxControl_roleForWorkFlow_selected;
        private DevExpress.XtraEditors.CheckedListBoxControl checkedListBoxControl_config_roleForWorkFlow;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;

    }
}