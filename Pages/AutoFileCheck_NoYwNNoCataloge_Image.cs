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
using System.Threading;
using System.Configuration;
using DotNet.DbUtilities;
using System.IO;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using System.Data.Common;
using System.Xml;

namespace Prj_FileManageNCheckApp
{
    public partial class AutoFileCheck_NoYwNNoCataloge_Image : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        public string ContentFormat { get; set; }//原文格式
        bool CanRecurrenceHanppen = true;
        bool IsExecuteDInAdvance = false;//是否经过预处理过程
        DataTable DTForAll = new DataTable();
        ArchiveClass AC { get; set; }
        private string CreatedTempTalbeName { get; set; }
        private string[] FieldClasses { get; set; }//检测所按分类数组
        private string QZH { get; set; }
        private List<ComboBoxEdit> ComboBoxList { get; set; }
        public UserEntity UserLoggedIn { get; set; }

        public AutoFileCheck_NoYwNNoCataloge_Image()
        {
            InitializeComponent();
        }

        private void AutoFileCheck_NoYwNNoCataloge_Load(object sender, EventArgs e)
        {
            this.DTForAll.Columns.Add("DH");
        }

        public AutoFileCheck_NoYwNNoCataloge_Image(string fileCodeName, UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            this.ContentFormat = paras.ContentFormat;
            GetAllQzhs();
        }

        private void GetAllQzhs()
        {
            string sql = "SELECT t2.Unique_code,t2.col_name,t2.show_name FROM t_config_auto_check_classify AS t1 INNER JOIN t_config_col_dict AS t2 ON t1.selected_code=t2.Unique_code \r\n";
            sql += "WHERE t2.code='" + this.FileCodeName + "' ORDER BY CAST(order_number AS INT) ASC";
            DataTable dt0 = new DbHelper().Fill(sql);
            if (dt0.Rows.Count == 0)
            {
                MessageBox.Show(this, "自动检测所按分类还未配置，请到自动检测配置页进行配置！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int widthOfFlowLayout = 0;
            flowLayoutPanel1.Controls.Clear();
            this.FieldClasses = new string[dt0.Rows.Count];
            this.ComboBoxList = new List<ComboBoxEdit>();
            for (int i = 0; i < dt0.Rows.Count; i++)
            {
                LabelControl lb = new LabelControl();
                lb.Name = "lb_" + i;
                lb.Text = dt0.Rows[i]["show_name"].ToString();
                lb.Tag = dt0.Rows[i]["col_name"];
                this.FieldClasses[i] = lb.Tag.ToString();
                ComboBoxEdit cbe = new ComboBoxEdit();
                cbe.Name = "cbe_" + i;
                cbe.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
                cbe.SelectedIndexChanged += cbe_SelectedIndexChanged;
                this.ComboBoxList.Add(cbe);

                DbHelper helper = new DbHelper();
                DataTable dt = null;
                if (i == 0)
                {
                    sql = "SELECT DISTINCT " + lb.Tag.ToString() + " FROM " + this.FileCodeName + " WHERE " + lb.Tag.ToString() + " IS NOT NULL ORDER BY " + lb.Tag.ToString() + " ASC";
                    dt = helper.Fill(sql);
                    for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        ComboboxItem item = new ComboboxItem(dt.Rows[k][lb.Tag.ToString()].ToString(), null);
                        cbe.Properties.Items.Add(item);
                    }
                    //cbe.SelectedIndex = cbe.Properties.Items.Count > 0 ? 0 : -1;
                    dt.Dispose();
                }
                widthOfFlowLayout += lb.Width + cbe.Width + 5;
                flowLayoutPanel1.Controls.Add(lb);
                flowLayoutPanel1.Controls.Add(cbe);
            }
            flowLayoutPanel1.Width = widthOfFlowLayout;
            panel2.Left = flowLayoutPanel1.Left + flowLayoutPanel1.Width + 5;
            dt0.Dispose();
        }

        private void cbe_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxEdit cbe = (ComboBoxEdit)sender;
            string selectedText = cbe.Text;
            for (int i = 0; i < this.ComboBoxList.Count; i++)
            {
                if (this.ComboBoxList[i] == cbe)
                {
                    if (i < this.FieldClasses.Length - 1)
                    {
                        string sql = "SELECT DISTINCT " + this.FieldClasses[i + 1] + " FROM " + this.FileCodeName + " WHERE " + this.FieldClasses[i + 1] + " IS NOT NULL AND " + this.FieldClasses[i] + "=@FieldName ORDER BY " + this.FieldClasses[i + 1] + " ASC";
                        DbHelper helper = new DbHelper();
                        DbParameter para1 = helper.MakeInParam("FieldName", this.ComboBoxList[i].Text);
                        DbParameter[] param = new DbParameter[] { para1 };
                        DataTable dt = helper.Fill(sql, param);
                        this.ComboBoxList[i + 1].Properties.Items.Clear();
                        for (int k = 0; k < dt.Rows.Count; k++)
                        {
                            ComboboxItem item = new ComboboxItem(dt.Rows[k][this.FieldClasses[i + 1]].ToString(), null);
                            this.ComboBoxList[i + 1].Properties.Items.Add(item);
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.CanRecurrenceHanppen = true;
            textBox1.Clear();
            gridControl_openFileWithNoPDF.DataSource = null;
            gridView_openFileWithNoPDF.PopulateColumns();
            gridControl_ywWithNoCataloge.DataSource = null;
            gridView_ywWithNoCataloge.PopulateColumns();
            gridControl_secretFileWithPDF.DataSource = null;
            gridView_secretFileWithPDF.PopulateColumns();

            Control[] ctrls = flowLayoutPanel1.Controls.Find("cbe_0", false);
            if (ctrls.Length == 0)
            {
                MessageBox.Show(this, "本档案类型库还未进行按某种分类自动检测的配置！，请配置后再继续。", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
            {
                Type controlType = flowLayoutPanel1.Controls[i].GetType();
                if (controlType == typeof(ComboBoxEdit))
                {
                    ComboBoxEdit cbedit = (ComboBoxEdit)flowLayoutPanel1.Controls[i];
                    if (string.IsNullOrEmpty(cbedit.Text))
                    {
                        MessageBox.Show(this, "请选择自动检测所需的分类项！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            this.QZH = ((ComboBoxEdit)ctrls[0]).Text;//检测所选原文文件是否符合已选的分类项，如，是否在同一个全宗。这里的this.QZH表示最大的那个分类，如全宗号，但其他类型的档案最大那个分类有可能不是全宗号

            if (string.IsNullOrEmpty(textEdit1.Text))
            {
                MessageBox.Show(this, "请指定所要检测原文（如，PDF、JPG文件）的路径！\r\n" + "建议：路径中尽量不要包括您所需检测分类之外的文件夹！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sql = "SELECT archive_num_field,archive_num_parts_amount,connect_char FROM t_config_archive_num_makeup WHERE code_name='" + this.FileCodeName + "'";
            DataTable dt = new DbHelper().Fill(sql);
            string archiveNoFieldName = dt.Rows[0]["archive_num_field"].ToString();
            int archiveNoComposedPart = int.Parse(dt.Rows[0]["archive_num_parts_amount"].ToString());
            string splitChar = dt.Rows[0]["connect_char"].ToString();
            dt.Dispose();

            ExtractArchiveNosToDataTable(textEdit1.Text, archiveNoComposedPart, splitChar);

            if (this.CanRecurrenceHanppen)
            {
                this.CreatedTempTalbeName = this.FileCodeName + "_temp_" + this.QZH + "_" + this.UserLoggedIn.UniqueCode + "_" + DateTime.Now.Millisecond; //防止数据中创表重复
                this.DTForAll.TableName = this.CreatedTempTalbeName;

                sql = "IF OBJECT_ID(N'" + this.CreatedTempTalbeName + "',N'U') IS NOT NULL \r\n";
                sql += "DELETE FROM " + this.CreatedTempTalbeName + " \r\n";
                sql += "ELSE \r\n";
                sql += "CREATE TABLE [dbo].[" + this.CreatedTempTalbeName + "]( \r\n";
                sql += "[DH] [nvarchar](50) NOT NULL \r\n";
                sql += ") ON [PRIMARY]";
                new DbHelper().ExecuteNonQuery(sql);//如果wwj_temp或jhj_temp存在，则删除其中的数据，如果不存在，则创建ajj_temp或jhj_temp。

                new SqlHelper().SqlBulkCopyData(this.DTForAll);//效率非常高的批量插入数据表
                this.DTForAll.Clear();//必须清除，不然会累加。
                textBox1.Text += "预处理结束！\r\n";
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
                this.IsExecuteDInAdvance = true;//标识，进行过预处理
            }
        }

        private void ExtractArchiveNosToDataTable(string path, int archiveNoComposedPart, string splitChar)
        {
            if (this.CanRecurrenceHanppen)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                string[] patterns = this.ContentFormat.Split(';');
                FileInfo[] tempFileInfo;
                FileInfo[] fiA = null;
                for (int i = 0; i < patterns.Length; i++)
                {
                    tempFileInfo = di.GetFiles(patterns[i]);
                    if (i == 0)
                        fiA = tempFileInfo;
                    else
                        fiA = (fiA.Concat((FileInfo[])tempFileInfo.Clone())).ToArray<FileInfo>();
                }

                if (fiA.Length > 0)
                {
                    string folderName = fiA[0].Directory.Name;
                    int num = folderName.Split(splitChar.ToCharArray()).Length;
                    if (num == archiveNoComposedPart)
                    {

                        if (!IsArchiveNoRelevantToPDFFile(folderName))
                        {
                            this.CanRecurrenceHanppen = false;//告诉递归函数不能再继续执行了
                            MessageBox.Show(this, "问题：\r\n可能是1、所选择的检测分类（如，全宗号）与所指定的检测文件夹不匹配！请选择适当的全宗号！\r\n可能是2、【" + folderName + "】对应的扫描原文中检测所按分类设置有问题，请检查并修改！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        DataRow r = this.DTForAll.NewRow();
                        r.SetField<string>("DH", folderName);
                        this.DTForAll.Rows.Add(r);

                        textBox1.Text += folderName + "\r\n";
                        textBox1.SelectionStart = textBox1.Text.Length;
                        if (textBox1.Text.Length > 10000)
                        {
                            textBox1.Clear();//重新加载，防止影响滚动速度
                        }
                        textBox1.ScrollToCaret();
                        Application.DoEvents();
                    }
                }

                DirectoryInfo[] diA = di.GetDirectories();
                for (int i = 0; i < diA.Length; i++)
                {
                    if (this.CanRecurrenceHanppen)
                    {
                        ExtractArchiveNosToDataTable(diA[i].FullName, archiveNoComposedPart, splitChar);
                    }
                    else
                        break;
                }
            }
        }

        private bool IsArchiveNoRelevantToPDFFile(string dh) //所指定的检测文件夹内是否与所选定的全宗号对应。
        {
            string[] strs = dh.Split('-');
            if (!strs[0].Equals(this.QZH))
            {
                return false;
            }
            return true;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (!this.IsExecuteDInAdvance)
            {
                MessageBox.Show(this, "请先进行预处理过程，然后再检测！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            GetAllDhsWithNoPDF();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!this.IsExecuteDInAdvance)
            {
                MessageBox.Show(this, "请先进行预处理过程，然后再检测！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            GetAllDhsWithNoCatalogue();
        }

        private void GetAllDhsWithNoPDF()
        {
            string scannedWithSecretFileStr = string.Empty;//此字符串对搜索结果很重要
            string sql = "SELECT archive_num_field FROM t_config_archive_num_makeup WHERE code_name='" + this.FileCodeName + "'";
            DataTable dt = new DbHelper().Fill(sql);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show(this, "档号还未配置，请到配置页面操作后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dt.Rows[0]["archive_num_field"] == DBNull.Value || dt.Rows[0]["archive_num_field"] == null)
            {
                MessageBox.Show(this, "档号对应字段还未配置，请到配置页面操作后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string archiveNoFieldName = dt.Rows[0]["archive_num_field"].ToString();

            sql = "SELECT t2.Unique_code,t2.col_name,t2.show_name FROM t_config_fields_group_for_autocheck AS t1 INNER JOIN t_config_col_dict AS t2 ON t1.selected_code=t2.Unique_code WHERE t2.code='" + this.FileCodeName + "' ORDER BY CAST(order_number AS INT) ASC";
            dt = new DbHelper().Fill(sql);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show(this, "用于自动质检的字段组合还未配置，请配置后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 0) scannedWithSecretFileStr = dt.Rows[i]["col_name"].ToString();
                else scannedWithSecretFileStr += "+" + dt.Rows[i]["col_name"].ToString();
            }

            sql = "SELECT secret_field FROM t_config_field_for_autocheck WHERE table_code='" + this.FileCodeName + "'";
            dt = new DbHelper().Fill(sql);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show(this, "用于自动质检的密级字段还未配置，请配置后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string secretClassFieldName = dt.Rows[0]["secret_field"].ToString();

            string whereString = GetWhereString();
            if (radioGroup1.SelectedIndex == 0)
            {
                sql = "SELECT " + archiveNoFieldName + " AS '档号'," + secretClassFieldName + " AS '密级' FROM " + this.FileCodeName + " WHERE " + whereString + " \r\n";
                sql += "AND " + scannedWithSecretFileStr + " IN(SELECT  " + scannedWithSecretFileStr + " FROM " + this.FileCodeName + "\r\n";
                sql += "WHERE  CHARINDEX('密'," + secretClassFieldName + ")=0 AND " + whereString + ") \r\n";
                sql += "AND " + archiveNoFieldName + " NOT IN (SELECT dh FROM " + this.CreatedTempTalbeName + ") ORDER BY " + archiveNoFieldName + " ASC";
            }
            else
            {
                sql = "SELECT " + archiveNoFieldName + " AS '档号'," + secretClassFieldName + " AS '密级' FROM " + this.FileCodeName + " WHERE " + whereString + " \r\n";
                sql += "AND " + scannedWithSecretFileStr + " IN(SELECT  " + scannedWithSecretFileStr + " FROM " + this.FileCodeName + "\r\n";
                sql += "WHERE  CHARINDEX('密'," + secretClassFieldName + ")>0 AND " + whereString + ") \r\n";
                sql += "AND " + archiveNoFieldName + " NOT IN (SELECT dh FROM " + this.CreatedTempTalbeName + ") ORDER BY " + archiveNoFieldName + " ASC";
            }

            dt = new DbHelper().Fill(sql);
            if (dt != null)
            {
                gridControl_openFileWithNoPDF.DataSource = dt;
                gridView_openFileWithNoPDF.PopulateColumns();
                ShowOpenYwPathLinkButton();
            }
        }

        private string GetWhereString()
        {
            string result = string.Empty;
            for (int i = 0; i < this.FieldClasses.Length; i++)
            {
                if (i == 0) result = this.FieldClasses[i] + "='" + this.ComboBoxList[i].Text + "'";
                else result += " AND " + this.FieldClasses[i] + "='" + this.ComboBoxList[i].Text + "'";
            }
            return result;
        }

        private string GetWhereString2()
        {
            string result = string.Empty;
            for (int i = 0; i < this.FieldClasses.Length; i++)
            {
                if (i == 0) result = "t1." + this.FieldClasses[i] + "='" + this.ComboBoxList[i].Text + "'";
                else result += " AND t1." + this.FieldClasses[i] + "='" + this.ComboBoxList[i].Text + "'";

            }
            return result;
        }

        void ShowOpenYwPathLinkButton()
        {
            RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();
            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.ButtonClick += riButtonEdit_ButtonClick;
            riButtonEdit.Buttons.Clear();
            EditorButton bt0 = new EditorButton();
            bt0.Kind = ButtonPredefines.Glyph;
            bt0.Caption = "文件目录";
            bt0.ToolTip = "点击查看目录下是否有原文";
            bt0.Appearance.ForeColor = Color.Blue;
            bt0.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt0);
            GridColumn Col1 = new GridColumn();
            Col1.FieldName = "verifyYw";
            Col1.Caption = "查看目录";
            Col1.Visible = false;
            Col1.VisibleIndex = gridView_openFileWithNoPDF.Columns.Count;
            Col1.UnboundType = DevExpress.Data.UnboundColumnType.String;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_openFileWithNoPDF.RepositoryItems.Add(riButtonEdit);
            gridView_openFileWithNoPDF.Columns.Add(Col1);
            //foreach (GridColumn c in gridView_openFileWithNoPDF.Columns)
            //{
            //    c.OptionsColumn.AllowEdit = c.ColumnEdit is RepositoryItemButtonEdit;
            //}
            this.gridView_openFileWithNoPDF.BestFitColumns();
        }

        void riButtonEdit_ButtonClick(object sender, EventArgs e)
        {
            string contentFileRootPath = ConfigurationManager.AppSettings["ContentFileRootPath"];//原文根路径
            if (Directory.Exists(contentFileRootPath))
                System.Diagnostics.Process.Start(contentFileRootPath);
            else
                MessageBox.Show("当前原文根路径设置在“" + contentFileRootPath + "”，请确认是否正确后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void GetAllDhsWithNoCatalogue()
        {
            string sql = "SELECT archive_num_field FROM t_config_archive_num_makeup WHERE code_name='" + this.FileCodeName + "'";
            DataTable dt = new DbHelper().Fill(sql);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show(this, "档号还未配置，请到配置页面操作后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dt.Rows[0]["archive_num_field"] == DBNull.Value || dt.Rows[0]["archive_num_field"] == null)
            {
                MessageBox.Show(this, "档号对应字段还未配置，请到配置页面操作后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string archiveNoFieldName = dt.Rows[0]["archive_num_field"].ToString();

            string whereString = GetWhereString();
            sql = "select DH AS '档号' FROM " + this.CreatedTempTalbeName + " EXCEPT SELECT " + archiveNoFieldName + " FROM " + this.FileCodeName + " WHERE " + whereString;
            dt = new DbHelper().Fill(sql);
            if (dt != null)
            {
                gridControl_ywWithNoCataloge.DataSource = dt;
                gridView_ywWithNoCataloge.PopulateColumns();
                gridView_ywWithNoCataloge.BestFitColumns();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!this.IsExecuteDInAdvance)
            {
                MessageBox.Show("请先进行预处理过程，然后再检测！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            GetAllDhsWithPDFInSecretPackageOrBox();
        }

        private void GetAllDhsWithPDFInSecretPackageOrBox()
        {
            string scannedWithSecretFileStr = string.Empty, scannedWithSecretFileStr2 = string.Empty;//此字符串对搜索结果很重要
            string sql = "SELECT archive_num_field FROM t_config_archive_num_makeup WHERE code_name='" + this.FileCodeName + "'";
            DataTable dt = new DbHelper().Fill(sql);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show(this, "档号还未配置，请到配置页面操作后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dt.Rows[0]["archive_num_field"] == DBNull.Value || dt.Rows[0]["archive_num_field"] == null)
            {
                MessageBox.Show(this, "档号对应字段还未配置，请到配置页面操作后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string archiveNoFieldName = dt.Rows[0]["archive_num_field"].ToString();

            sql = "SELECT t2.Unique_code,t2.col_name,t2.show_name FROM t_config_fields_group_for_autocheck AS t1 INNER JOIN t_config_col_dict AS t2 ON t1.selected_code=t2.Unique_code WHERE t2.code='" + this.FileCodeName + "' ORDER BY CAST(order_number AS INT) ASC";
            dt = new DbHelper().Fill(sql);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show(this, "用于自动质检的字段组合还未配置，请配置后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 0) scannedWithSecretFileStr = dt.Rows[i]["col_name"].ToString();
                else scannedWithSecretFileStr += "+" + dt.Rows[i]["col_name"].ToString();
                if (i == 0) scannedWithSecretFileStr2 = "t1." + dt.Rows[i]["col_name"].ToString();
                else scannedWithSecretFileStr2 += "+" + "t1." + dt.Rows[i]["col_name"].ToString();
            }

            sql = "SELECT secret_field FROM t_config_field_for_autocheck WHERE table_code='" + this.FileCodeName + "'";
            dt = new DbHelper().Fill(sql);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show(this, "用于自动质检的密级字段还未配置，请配置后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string secretClassFieldName = dt.Rows[0]["secret_field"].ToString();

            string whereString = GetWhereString();
            string whereStringT1 = GetWhereString2();
            sql = "SELECT t1." + archiveNoFieldName + " AS '档号'," + secretClassFieldName + " AS '密级' FROM " + this.FileCodeName + " AS t1 \r\n";
            sql += " INNER JOIN " + this.CreatedTempTalbeName + " AS t2 ON t1." + archiveNoFieldName + "=t2.dh \r\n";
            sql += "WHERE " + whereStringT1 + " \r\n";
            sql += "AND " + scannedWithSecretFileStr2 + " IN(SELECT " + scannedWithSecretFileStr + " FROM " + this.FileCodeName + "\r\n";
            sql += "WHERE CHARINDEX('密'," + secretClassFieldName + ")>0 AND " + whereString + ")";

            dt = new DbHelper().Fill(sql);
            if (dt != null)
            {
                gridControl_secretFileWithPDF.DataSource = dt;
                gridView_secretFileWithPDF.PopulateColumns();
                ShowOpenYwLinkButton();
            }
        }

        void ShowOpenYwLinkButton()
        {
            RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();
            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.ButtonClick += secretButtonEdit_ButtonClick;
            riButtonEdit.Buttons.Clear();
            EditorButton bt0 = new EditorButton();
            bt0.Kind = ButtonPredefines.Glyph;
            bt0.Caption = "原文";
            bt0.ToolTip = "点击查看原文";
            bt0.Appearance.ForeColor = Color.Blue;
            bt0.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt0);
            GridColumn Col1 = new GridColumn();
            Col1.FieldName = "viewYw";
            Col1.Caption = "查看原文";
            Col1.Visible = false;
            Col1.VisibleIndex = gridView_secretFileWithPDF.Columns.Count;
            Col1.UnboundType = DevExpress.Data.UnboundColumnType.String;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_secretFileWithPDF.RepositoryItems.Add(riButtonEdit);
            gridView_secretFileWithPDF.Columns.Add(Col1);
            //foreach (GridColumn c in gridView_secretFileWithPDF.Columns)
            //{
            //    c.OptionsColumn.AllowEdit = c.ColumnEdit is RepositoryItemButtonEdit;
            //}
            gridView_secretFileWithPDF.BestFitColumns();
        }

        void secretButtonEdit_ButtonClick(object sender, EventArgs e)
        {
            string sql = "SELECT archive_num_field FROM t_config_archive_num_makeup WHERE code_name='" + this.FileCodeName + "'";
            DataTable dt = new DbHelper().Fill(sql);
            string archiveNoFieldName = dt.Rows[0]["archive_num_field"].ToString();
            int rowhandle = gridView_secretFileWithPDF.FocusedRowHandle;
            DataRow dr = gridView_secretFileWithPDF.GetDataRow(rowhandle);
            string dh = dr[0].ToString();
            if (this.ContentFormat.ToLower().Contains("pdf"))
            {
                sql = "SELECT yw FROM " + this.FileCodeName + " WHERE " + archiveNoFieldName + "='" + dh + "'";
                object ywRelativePath = new DbHelper().ExecuteScalar(sql);
                if (ywRelativePath != DBNull.Value)
                {
                    string ywRootPath = ConfigurationManager.AppSettings["ContentFileRootPath"];//原文根路径
                    string path = ywRootPath + ywRelativePath;
                    try
                    {
                        if (System.IO.File.Exists(path))
                            System.Diagnostics.Process.Start(path);//打开PDF文件的默认浏览器
                        else
                            MessageBox.Show("打不开原文！原因可能是：\r\n1、原文根路径当前设置在“" + ywRootPath + "”，请检查与原文挂接时设置的路径是否一致；\r\n2、请检查访问原文网络（服务器）时是否已经登录。\r\n若不知如何操作，请联系系统管理员或售后人员。", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch
                    {
                        MessageBox.Show("打不开原文！原因可能是：\r\n1、原文根路径当前设置在“" + ywRootPath + "”，请检查与原文挂接时设置的路径是否一致；\r\n2、请检查访问原文网络（服务器）时是否已经登录。\r\n若不知如何操作，请联系系统管理员或售后人员。", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                sql = "SELECT yw_xml FROM " + this.FileCodeName + " WHERE " + archiveNoFieldName + " = '" + dh + "'";
                dt = new DbHelper().Fill(sql);
                object ywXMLStr = dt.Rows[0]["yw_xml"];
                if (ywXMLStr == DBNull.Value || string.IsNullOrEmpty(ywXMLStr.ToString()))
                {
                    MessageBox.Show("本条目录的原文还未挂接，或没有原文！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ywXMLStr.ToString());
                XmlNodeList nodeList = doc.SelectNodes(@"YW/ywPath");
                if (nodeList.Count > 0)
                {
                    List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        FileInfo fi = new FileInfo(nodeList[i].InnerText);
                        string fileName = fi.Name;
                        list.Add(new KeyValuePair<string, string>((nodeList[i].InnerText), fileName));
                    }
                    ImageFileContentBrower ifcb = new ImageFileContentBrower(dh, list);
                    ifcb.Show();
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Excel文件(*.xls)|*.xls|Excel文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
                DataTable dt = gridControl_openFileWithNoPDF.DataSource as DataTable;
                ExcelHelper.GetExcelByDataTable(dt, fileName);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Excel文件(*.xls)|*.xls|Excel文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
                DataTable dt = gridControl_ywWithNoCataloge.DataSource as DataTable;
                ExcelHelper.GetExcelByDataTable(dt, fileName);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Excel文件(*.xls)|*.xls|Excel文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
                DataTable dt = gridControl_secretFileWithPDF.DataSource as DataTable;
                ExcelHelper.GetExcelByDataTable(dt, fileName);
            }
        }

        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioGroup1.SelectedIndex == 0)
            {
                button5.Enabled = true;
                gridControl_secretFileWithPDF.Enabled = true;
                button8.Enabled = true;
            }
            else
            {
                button5.Enabled = false;
                gridControl_secretFileWithPDF.Enabled = false;
                button8.Enabled = false;
            }
        }

        public void AutoFileCheck_NoYwNNoCataloge_FormClosed()
        {
            if (!string.IsNullOrEmpty(this.CreatedTempTalbeName))
            {
                this.CanRecurrenceHanppen = false;//通知自动检测的工作停止
                string sql = "IF OBJECT_ID(N'" + this.CreatedTempTalbeName + "',N'U') IS NOT NULL \r\n";
                sql += "DROP TABLE " + this.CreatedTempTalbeName;
                new DbHelper().ExecuteNonQuery(sql);
            }
        }

        private void textEdit1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textEdit1.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}