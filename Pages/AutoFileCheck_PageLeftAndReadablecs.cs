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
using System.Configuration;
using DotNet.DbUtilities;
using System.IO;
using iTextSharp.text.pdf;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using System.Xml;

namespace Prj_FileManageNCheckApp
{
    public delegate void ShowGridViewForPageNumberError(DataTable dt);
    public partial class AutoFileCheck_PageLeftAndReadablecs : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        public string ContentFormat { get; set; }//原文格式
        private DataTable DTForAll;
        private int count1, count2, count3 = 0;
        private bool CanRecurrenceHanppen = true;//递归是否执行
        private bool IsChecking { get; set; }//是否正在执行

        private string[] FieldClasses { get; set; }//检测所按分类数组
        private List<ComboBoxEdit> ComboBoxList { get; set; }
        private DataTable DTForPageLeft { get; set; }
        private DataTable DTForPageNumberError { get; set; }
        private object YsField { get; set; }
        private object QyhField { get; set; }
        private object ZyhField { get; set; }
        private object XcsjField { get; set; }
        private object NdField { get; set; }
        private XmlNodeList NodeListForCompareRecords { get; set; }
        public ShowGridViewForPageNumberError ShowGridViewForPageNumberErrorFunc { get; set; }
        public UserEntity UserLoggedIn { get; set; }
        public string ArchiveNoFieldName { get; set; }
        public string SecretClassFieldName { get; set; }
        public AutoFileCheck_PageLeftAndReadablecs()
        {
            InitializeComponent();
        }

        public AutoFileCheck_PageLeftAndReadablecs(string fileCodeName, UserEntity user)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
            this.UserLoggedIn = user;
        }

        public void InitiateDatas(ParametersForChildForms paras)
        {
            this.FileCodeName = paras.FileCodeName;
            this.ContentFormat = paras.ContentFormat;
            this.Name = "自动检测：原文内页漏扫、页码错误、时间信息有误等(" + this.ContentFormat + ")";
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
                widthOfFlowLayout += lb.Width + cbe.Width + 4;

                flowLayoutPanel1.Controls.Add(lb);
                flowLayoutPanel1.Controls.Add(cbe);
            }
            flowLayoutPanel1.Width = widthOfFlowLayout;
            panel2.Left = flowLayoutPanel1.Left + flowLayoutPanel1.Width + 4;
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
            if (this.IsChecking)
            {
                MessageBox.Show(this, "当前正在数据检测，不能重新开始！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.count1 = this.count2 = this.count3 = 0;
            this.textBox1.Clear(); this.textBox4.Clear();
            if (this.DTForPageLeft != null)
            {
                this.DTForPageLeft = null;
                gridControl_pageLeft.DataSource = null;
                gridView_pageLeft.PopulateColumns();
            }
            if (this.DTForPageNumberError != null)
            {
                this.DTForPageNumberError = null;
                gridControl_pageNumberError.DataSource = null;
                gridView_pageNumberError.PopulateColumns();
            }

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
            //this.QZH = ((ComboBoxEdit)ctrls[0]).Text;//检测所选原文文件是否符合已选的分类项，如，是否在同一个全宗。这里的this.QZH表示最大的那个分类，如全宗号，但其他类型的档案最大那个分类有可能不是全宗号

            //if (string.IsNullOrEmpty(textEdit1.Text))
            //{
            //    MessageBox.Show(this, "请指定所要检测原文（如，PDF文件）的目录！\r\n" + "建议：路径中尽量不要包括您所需检测分类之外的文件夹！", "警告！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            string sql = "SELECT archive_num_field FROM t_config_archive_num_makeup WHERE code_name='" + this.FileCodeName + "'";
            DataTable dt = new DbHelper().Fill(sql);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show(this, "档号还未配置，请到配置页面操作后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.CanRecurrenceHanppen = false;
                return;
            }
            if (dt.Rows[0]["archive_num_field"] == DBNull.Value || dt.Rows[0]["archive_num_field"] == null)
            {
                MessageBox.Show(this, "档号对应字段还未配置，请到配置页面操作后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.CanRecurrenceHanppen = false;
                return;
            }
            this.ArchiveNoFieldName = dt.Rows[0]["archive_num_field"].ToString();

            sql = "SELECT secret_field FROM t_config_field_for_autocheck WHERE table_code='" + this.FileCodeName + "'";
            dt = new DbHelper().Fill(sql);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show(this, "用于自动质检的密级字段还未配置，请配置后再继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.SecretClassFieldName = dt.Rows[0]["secret_field"].ToString();
            dt.Dispose();

            sql = "SELECT ys_field,qyh_field,zyh_field,nd_field,xcsj_field FROM t_config_field_for_autocheck WHERE table_code='" + this.FileCodeName + "'";
            DataTable dTForFields = new DbHelper().Fill(sql);
            if (dTForFields.Rows.Count == 0)
            {
                MessageBox.Show(this, "用于自动检测的页数、年度等字段还未配置，请配置后继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.CanRecurrenceHanppen = false;
                return;
            }
            if (dTForFields.Rows[0]["ys_field"] == DBNull.Value && dTForFields.Rows[0]["qyh_field"] == DBNull.Value && dTForFields.Rows[0]["zyh_field"] == DBNull.Value)
            {
                MessageBox.Show(this, "用于自动检测的页数、年度等字段还未配置，请配置后继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.CanRecurrenceHanppen = false;
                return;
            }

            DataTable dtYw = GetTableDataWithYW();
            if (dtYw.Rows.Count == 0)
            {
                MessageBox.Show(this, "原文还未挂接，请挂接后继续！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.CanRecurrenceHanppen = false;
                return;
            }
            dtYw.Dispose();

            this.CanRecurrenceHanppen = true;
            DataRow dr_fields = dTForFields.Rows[0];
            this.YsField = dr_fields["ys_field"];
            this.QyhField = dr_fields["qyh_field"];
            this.ZyhField = dr_fields["zyh_field"];
            this.NdField = dr_fields["nd_field"];
            this.XcsjField = dr_fields["xcsj_field"];

            sql = "SELECT archive_body,archive_num_field,archive_prefix,connect_char FROM t_config_archive_num_makeup WHERE code_name='" + this.FileCodeName + "'";
            DataTable dtForCompareRecords = new DbHelper().Fill(sql);
            string xmlfile = dtForCompareRecords.Rows[0]["archive_body"].ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlfile.ToString());
            this.NodeListForCompareRecords = doc.SelectNodes(@"ArchiveNumMakeup/MakeupItem");

            this.DTForAll = this.GetNeededTableData();
            this.IsChecking = true;
            //GetPDFFilesDirectory(textEdit1.Text);
            CheckPageLeftAndReadable();
            dTForFields.Dispose();
            dtForCompareRecords.Dispose();
            this.DTForAll.Dispose();
            this.IsChecking = false;
            MessageBox.Show(this, "检测完毕！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GetPDFFilesDirectory(string path)
        {
            if (this.CanRecurrenceHanppen)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] fiA = di.GetFiles("*.pdf");
                for (int j = 0; j < fiA.Length; j++)
                {
                    if (this.CanRecurrenceHanppen)
                    {
                        string name1 = fiA[j].FullName;
                        string name2 = fiA[j].Name;
                        string page1 = GetPDFPageNumber(name1);
                        string dh = name2.Substring(0, name2.ToLower().LastIndexOf(".pdf"));
                        label4.Text = "共 " + ++count1 + " 个档号";
                        textBox1.Text += name2 + "  " + page1 + "页\r\n";
                        textBox1.SelectionStart = textBox1.Text.Length;
                        if (textBox1.Text.Length > 10000)
                        {
                            textBox1.Clear();
                        }
                        textBox1.ScrollToCaret();

                        int page2 = int.Parse(page1);
                        FindArchiveNoWithFalsePageNumber(dh, page2);
                        Application.DoEvents();
                    }
                    else
                        break;
                }

                DirectoryInfo[] diA = di.GetDirectories();
                for (int i = 0; i < diA.Length; i++)
                {
                    if (this.CanRecurrenceHanppen)
                    {
                        GetPDFFilesDirectory(diA[i].FullName);
                    }
                    else
                        break;
                }
            }
        }

        void CheckPageLeftAndReadable()
        {
            for (int i = 0; i < this.DTForAll.Rows.Count; i++)
            {
                if (this.CanRecurrenceHanppen)
                {
                    string dh = this.DTForAll.Rows[i][this.ArchiveNoFieldName].ToString();
                    object ywObj = this.DTForAll.Rows[i]["yw"];
                    if (ywObj != DBNull.Value && !string.IsNullOrEmpty(ywObj.ToString()))
                    {
                        string ywRootPath = ConfigurationManager.AppSettings["ContentFileRootPath"];//原文根路径
                        string path = ywRootPath + ywObj.ToString();
                        string pageNumber = GetPDFPageNumber(path);
                        label4.Text = "共 " + ++count1 + " 个档号";
                        textBox1.Text += dh + "  " + pageNumber + "页\r\n";
                        textBox1.SelectionStart = textBox1.Text.Length;
                        if (textBox1.Text.Length > 10000)
                        {
                            textBox1.Clear();
                        }
                        textBox1.ScrollToCaret();

                        FindArchiveNoWithFalsePageNumber(dh, int.Parse(pageNumber));
                        Application.DoEvents();
                    }
                }
                else
                    break;
            }
        }

        private string GetPDFPageNumber(string path)
        {
            try
            {
                PdfReader reader = new PdfReader(path);
                int iPageNum = reader.NumberOfPages;
                reader.Close(); //不关闭会一直占用pdf资源，对接下来的操作会有影响
                return iPageNum.ToString();
            }
            catch
            {
                //MessageBox.Show(path + "  " + e.Message);
                textBox4.Text += path + "\r\n";
                label6.Text = "共 " + ++count2 + " 个PDF文件";
                textBox4.SelectionStart = textBox4.Text.Length;
                textBox4.ScrollToCaret();
                //count2 = 0;
            }
            return "-1";
        }

        private void FindArchiveNoWithFalsePageNumber(string archiveNo, int pdfpageNumber)
        {
            string noEndPageNumber_SplitChar = ConfigurationManager.AppSettings["NoEndPageNumber_SplitChar"];//如果有 止页号 字段，最后一件的起页和止页间用得什么分隔符
            DataRow[] rows = DTForAll.Select(this.ArchiveNoFieldName + "='" + archiveNo + "'");
            if (rows.Length > 0)
            {
                this.CanRecurrenceHanppen = true;
                string startPageNo = (this.QyhField == DBNull.Value || this.QyhField == null) ? null : this.QyhField.ToString();// 起页号 字段
                string endPageNo = (this.ZyhField == DBNull.Value || this.ZyhField == null) ? null : this.ZyhField.ToString();// 止页号 字段
                string pageNumber = (this.YsField == DBNull.Value || this.YsField == null) ? null : this.YsField.ToString(); //-- 页数 字段
                string theYearFieldName = (this.NdField == DBNull.Value || this.NdField == null) ? null : this.NdField.ToString();// 年度 字段名
                string createdTimeFieldName = (this.XcsjField == DBNull.Value || this.XcsjField == null) ? null : this.XcsjField.ToString();// 形成时间 字段名

                bool endPageNumberBool = string.IsNullOrEmpty(endPageNo) ? false : true;
                bool startPageNumberBool = string.IsNullOrEmpty(startPageNo) ? false : true;
                bool pageNumberBool = string.IsNullOrEmpty(pageNumber) ? false : true;
                bool useTheYearFieldBool = string.IsNullOrEmpty(theYearFieldName) ? false : true;
                bool useCreatedTimeFieldBool = string.IsNullOrEmpty(createdTimeFieldName) ? false : true;

                DataRow dr0 = rows[0];
                string mjResult = dr0[this.SecretClassFieldName] == DBNull.Value || dr0[this.SecretClassFieldName] == null ? string.Empty : dr0[this.SecretClassFieldName].ToString();
                if (endPageNumberBool && startPageNumberBool) //起页号和止页号均被使用的情况下
                {
                    if (!pageNumberBool)//若页数字段没被使用
                    {
                        int qyh = 0; int zyh = 0;
                        try
                        {
                            qyh = int.Parse(dr0[startPageNo].ToString());
                            zyh = int.Parse(dr0[endPageNo].ToString());
                            int total = zyh - qyh + 1;
                            if (total > pdfpageNumber)//可能漏页，也可能草稿没扫等
                            {
                                label5.Text = "共 " + ++count3 + " 个档号";
                                AddResultDataToGridViewForPageLeft(archiveNo, startPageNumberBool, pageNumberBool, total.ToString(), string.Empty, pdfpageNumber.ToString());
                            }
                            if (total < 0 || (total == 0 && pdfpageNumber > 1))//起止页或页数或前处理编页有问题的
                            {
                                AddResultDataToGridViewForPageNumberError(archiveNo, mjResult);
                            }
                        }
                        catch
                        {
                            AddResultDataToGridViewForPageNumberError(archiveNo, mjResult);
                        }
                    }
                    else //若页数字段被使用
                    {
                        int qyh = 0; int zyh = 0; int ys = 0;
                        try
                        {
                            qyh = int.Parse(dr0[startPageNo].ToString());
                            zyh = int.Parse(dr0[endPageNo].ToString());
                            ys = int.Parse(dr0[pageNumber].ToString());

                            int total = zyh - qyh + 1;
                            if (total > pdfpageNumber || ys > pdfpageNumber)//可能漏页，也可能草稿没扫等
                            {
                                label5.Text = "共 " + ++count3 + " 个档号";
                                AddResultDataToGridViewForPageLeft(archiveNo, startPageNumberBool, pageNumberBool, total.ToString(), ys.ToString(), pdfpageNumber.ToString());
                            }
                            if (total < 0 || (total == 0 && pdfpageNumber > 1) || total != ys)//起止页或页数或前处理编页有问题的
                            {
                                AddResultDataToGridViewForPageNumberError(archiveNo, mjResult);
                            }
                        }
                        catch
                        {
                            AddResultDataToGridViewForPageNumberError(archiveNo, mjResult);
                        }
                    }
                }
                if (!endPageNumberBool && startPageNumberBool) //仅使用起页号而没使用止页号的情况下
                {
                    int index1 = this.DTForAll.Rows.IndexOf(dr0);
                    int index2 = index1 + 1;
                    if (index2 < this.DTForAll.Rows.Count)
                    {
                        string condtion = GetFieldsGroupWithoutLastClass(dr0);
                        string condtion_next = GetFieldsGroupWithoutLastClass(DTForAll.Rows[index2]);
                        int qyh = 0, qzh2 = 0, total = 0;
                        if (condtion.Equals(condtion_next))//如果在案卷内并且属于同一目录号
                        {
                            try
                            {
                                qyh = int.Parse(dr0[startPageNo].ToString());
                                string qyhstr2 = DTForAll.Rows[index2][startPageNo].ToString();
                                int charIndex = qyhstr2.IndexOf(noEndPageNumber_SplitChar);
                                if (charIndex < 0)
                                {
                                    qzh2 = int.Parse(qyhstr2);
                                }
                                if (charIndex == 0)
                                {
                                    qzh2 = 0;
                                }
                                if (charIndex > 0)
                                {
                                    string[] qyhSplitStrs = qyhstr2.Split(noEndPageNumber_SplitChar.ToCharArray());
                                    qzh2 = int.Parse(qyhSplitStrs[0]);
                                }
                                total = qzh2 - qyh;
                            }
                            catch
                            {
                                AddResultDataToGridViewForPageNumberError(archiveNo, mjResult);
                            }
                        }
                        else if (!condtion.Equals(condtion_next))
                        {
                            try
                            {
                                string qyhStr = dr0[startPageNo].ToString();
                                int charIndex = qyhStr.IndexOf(noEndPageNumber_SplitChar);
                                if (charIndex < 0)//不包含起页数和止页数的分隔符
                                {
                                    qyh = int.Parse(qyhStr);
                                    qzh2 = 0;
                                }
                                else if (charIndex == 0)
                                {
                                    qyh = 0;
                                    qzh2 = int.Parse(qyhStr.Substring(charIndex));
                                }
                                else if (charIndex > 0)
                                {
                                    string[] qyhSplitStrs = qyhStr.Split(noEndPageNumber_SplitChar.ToCharArray());
                                    qyh = int.Parse(qyhSplitStrs[0]);
                                    if (qyhSplitStrs.Length == 1)
                                    {
                                        qzh2 = 0;
                                    }
                                    if (qyhSplitStrs.Length > 1)
                                    {
                                        qzh2 = int.Parse(qyhSplitStrs[1]) + 1;
                                    }
                                }
                                total = qzh2 - qyh;
                            }
                            catch
                            {
                                AddResultDataToGridViewForPageNumberError(archiveNo, mjResult);
                            }
                        }
                        if (!pageNumberBool)//若页数字段没被使用
                        {
                            try
                            {
                                if (total > pdfpageNumber)//可能漏页，也可能草稿没扫等
                                {
                                    label5.Text = "共 " + ++count3 + " 个档号";
                                    AddResultDataToGridViewForPageLeft(archiveNo, startPageNumberBool, pageNumberBool, total.ToString(), string.Empty, pdfpageNumber.ToString());
                                }
                                if (total < 0 || (total == 0 && pdfpageNumber > 1))//起止页或页数或前处理编页有问题的
                                {
                                    AddResultDataToGridViewForPageNumberError(archiveNo, mjResult);
                                }
                            }
                            catch
                            {
                                AddResultDataToGridViewForPageNumberError(archiveNo, mjResult);
                            }
                        }
                        else //若页数字段被使用
                        {
                            try
                            {
                                int ys = 0;
                                ys = int.Parse(dr0[pageNumber].ToString());
                                if (total > pdfpageNumber || ys > pdfpageNumber)//可能漏页，也可能草稿没扫等
                                {
                                    label5.Text = "共 " + ++count3 + " 个档号";
                                    AddResultDataToGridViewForPageLeft(archiveNo, startPageNumberBool, pageNumberBool, total.ToString(), ys.ToString(), pdfpageNumber.ToString());
                                }
                                if (total < 0 || (total == 0 && pdfpageNumber > 1) || total != ys)//起止页或页数或前处理编页有问题的
                                {
                                    AddResultDataToGridViewForPageNumberError(archiveNo, mjResult);
                                }
                            }
                            catch
                            {
                                AddResultDataToGridViewForPageNumberError(archiveNo, mjResult);
                            }
                        }
                    }
                }
                if (!endPageNumberBool && !startPageNumberBool && pageNumberBool) //只使用了页数字段，没有使用起页号和止页号字段
                {
                    int ys = 0;
                    try
                    {
                        ys = int.Parse(dr0[pageNumber].ToString());
                        if (ys > pdfpageNumber)
                        {
                            label5.Text = "共 " + ++count3 + " 个档号";
                            AddResultDataToGridViewForPageLeft(archiveNo, startPageNumberBool, pageNumberBool, string.Empty, ys.ToString(), pdfpageNumber.ToString());
                        }
                        if (ys < 0 || (ys == 0 && pdfpageNumber > 1))
                        {
                            AddResultDataToGridViewForPageNumberError(archiveNo, mjResult);
                        }
                    }
                    catch
                    {
                        AddResultDataToGridViewForPageNumberError(archiveNo, mjResult);
                    }
                }
                CheckYearAndPrimaryTime(rows, archiveNo, useTheYearFieldBool, theYearFieldName, useCreatedTimeFieldBool, createdTimeFieldName);//检查年度或形成时间的逻辑正误
            }
        }

        string GetFieldsGroupWithoutLastClass(DataRow dr)//档号组成中没有最小的分类的其余部分组成的字符串
        {
            string dhstr = string.Empty;
            if (NodeListForCompareRecords != null)
            {
                for (int i = 0; i < NodeListForCompareRecords.Count - 1; i++)
                {

                    if (i == NodeListForCompareRecords.Count - 1)
                    {
                        dhstr += dr[NodeListForCompareRecords[i].Attributes["value"].Value].ToString();
                    }
                    else
                    {
                        dhstr += dr[NodeListForCompareRecords[i].Attributes["value"].Value].ToString() + "-";
                    }
                }
            }
            return dhstr;
        }

        void AddResultDataToGridViewForPageLeft(string dh, bool startPageNoBool, bool pageNumberBool, string countedPageNumber, string shownPageNumber, string realPageNumber)
        {
            string colName1 = "档号";
            string colName2 = "PDF实际页数";
            string colName3 = string.Empty, colName4 = string.Empty;
            if (startPageNoBool)
                colName3 = "起页号推算页数";
            if (pageNumberBool)
                colName4 = "“页数”字段显示页数";

            if (this.DTForPageLeft == null)
            {
                this.DTForPageLeft = new DataTable();
                this.DTForPageLeft.Columns.Add(colName1);
                this.DTForPageLeft.Columns.Add(colName2);
                if (startPageNoBool)
                    this.DTForPageLeft.Columns.Add(colName3);
                if (pageNumberBool)
                    this.DTForPageLeft.Columns.Add(colName4);
            }
            DataRow dr = this.DTForPageLeft.NewRow();
            dr[colName1] = dh;
            dr[colName2] = realPageNumber;
            if (startPageNoBool)
                dr[colName3] = countedPageNumber;
            if (pageNumberBool)
                dr[colName4] = shownPageNumber;
            this.DTForPageLeft.Rows.Add(dr);
            gridControl_pageLeft.DataSource = this.DTForPageLeft;
            gridView_pageLeft.PopulateColumns();
            ShowOpenYwLinkButton();
        }

        void AddResultDataToGridViewForPageNumberError(string dh, string mj)
        {
            string colName1 = "档号", colName2 = "密级";
            if (this.DTForPageNumberError == null)
            {
                this.DTForPageNumberError = new DataTable();
                this.DTForPageNumberError.Columns.Add(colName1);
                this.DTForPageNumberError.Columns.Add(colName2);
            }
            DataRow dr = this.DTForPageNumberError.NewRow();
            dr[colName1] = dh;
            dr[colName2] = mj;
            this.DTForPageNumberError.Rows.Add(dr);
            gridControl_pageNumberError.DataSource = this.DTForPageNumberError;
            gridView_pageNumberError.PopulateColumns();
            ShowOpenSpecialAjLinkButton();
        }

        void ShowOpenYwLinkButton()
        {
            RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();
            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.ButtonClick += ywButtonEdit_ButtonClick;
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
            Col1.VisibleIndex = gridView_pageLeft.Columns.Count;
            Col1.UnboundType = DevExpress.Data.UnboundColumnType.String;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_pageLeft.RepositoryItems.Add(riButtonEdit);
            gridView_pageLeft.Columns.Add(Col1);
            gridView_pageLeft.BestFitColumns();
        }

        void ShowOpenSpecialAjLinkButton()
        {
            RepositoryItemButtonEdit riButtonEdit = new RepositoryItemButtonEdit();
            riButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            riButtonEdit.ButtonClick += paButtonEdit_ButtonClick;
            riButtonEdit.Buttons.Clear();
            EditorButton bt0 = new EditorButton();
            bt0.Kind = ButtonPredefines.Glyph;
            bt0.Caption = "核对";
            bt0.ToolTip = "点击查看本档号前后的记录，以对照差错";
            bt0.Appearance.ForeColor = Color.Blue;
            bt0.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            riButtonEdit.Buttons.Add(bt0);
            GridColumn Col1 = new GridColumn();
            Col1.FieldName = "viewPARecord";
            Col1.Caption = "前后对照";
            Col1.Visible = false;
            Col1.VisibleIndex = gridView_pageLeft.Columns.Count;
            Col1.UnboundType = DevExpress.Data.UnboundColumnType.String;
            Col1.ColumnEdit = riButtonEdit;
            gridControl_pageNumberError.RepositoryItems.Add(riButtonEdit);
            gridView_pageNumberError.Columns.Add(Col1);
            gridView_pageNumberError.BestFitColumns();
        }

        void ywButtonEdit_ButtonClick(object sender, EventArgs e)
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
            dt.Dispose();

            int rowhandle = gridView_pageLeft.FocusedRowHandle;
            DataRow dr = gridView_pageLeft.GetDataRow(rowhandle);
            string dh = dr[0].ToString();
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

        void paButtonEdit_ButtonClick(object sender, EventArgs e)
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
            dt.Dispose();

            int rowhandle = gridView_pageNumberError.FocusedRowHandle;
            DataRow dr = gridView_pageNumberError.GetDataRow(rowhandle);
            string dh = dr[0].ToString();
            sql = "SELECT connect_char FROM t_config_archive_num_makeup WHERE code_name='" + this.FileCodeName + "'";
            object connectCharObj = new DbHelper().ExecuteScalar(sql);
            int lastIndex = dh.LastIndexOf(connectCharObj.ToString());
            string dhWithoutOrdialNumber = dh.Substring(0, lastIndex);

            string selectFieldStr = "Unique_code";
            sql = "SELECT t1.col_name,t1.show_name FROM t_config_col_dict t1 \r\n";
            sql += "INNER JOIN  t_config_field_show_list t2 ON t1.Unique_code=t2.selected_code \r\n";
            sql += "WHERE t1.code='" + this.FileCodeName + "'\r\n";
            sql += " ORDER BY CAST(t2.order_number AS INT) ASC";
            DataTable fieldDt = new DbHelper().Fill(sql);
            for (int i = 0; i < fieldDt.Rows.Count; i++)
            {
                selectFieldStr += "," + fieldDt.Rows[i]["col_name"].ToString() + " AS " + fieldDt.Rows[i]["show_name"].ToString();
            }
            fieldDt.Dispose();
            sql = "SELECT " + selectFieldStr + " FROM " + this.FileCodeName + " WHERE " + archiveNoFieldName + " LIKE '" + dhWithoutOrdialNumber + "%'";
            DataTable dtForCheckPageNumber = new DbHelper().Fill(sql);
            ShowGridViewForPageNumberErrorFunc(dtForCheckPageNumber);
        }

        private DataTable GetNeededTableData()
        {
            string where = GetWhereString();
            string sql = "SELECT * FROM " + this.FileCodeName + " WHERE " + where;
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
        }

        private DataTable GetTableDataWithYW()
        {
            string where = GetWhereString();
            string sql = "SELECT * FROM " + this.FileCodeName + " WHERE " + where + " AND yw IS NOT NULL";
            DataTable dt = new DbHelper().Fill(sql);
            return dt;
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

        private void CheckYearAndPrimaryTime_Box(DataRow[] rows, string archiveNo, bool useTheYearField_BoxBool, string theYearFieldName_Box, bool useCreatedTimeField_BoxBool, string createdTimeFieldName_Box)
        {
            if (useTheYearField_BoxBool)
            {
                string nd = string.Empty;
                try
                {
                    nd = rows[0][theYearFieldName_Box].ToString();
                    DateTime myDateTime = DateTime.ParseExact(nd, "yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    textBox6.Text += archiveNo + " *位置-->年度： " + nd + "\r\n";
                    textBox6.SelectionStart = textBox6.Text.Length;
                    textBox6.ScrollToCaret();
                }
            }
            if (useCreatedTimeField_BoxBool)
            {
                string xcsj = string.Empty;
                try
                {
                    xcsj = rows[0][createdTimeFieldName_Box].ToString();
                    DateTime myDateTime2 = DateTime.ParseExact(xcsj, "yyyyMMdd", CultureInfo.InvariantCulture);
                }
                catch
                {
                    if (string.IsNullOrEmpty(xcsj) || !CheckIndexofTwoZero(xcsj))
                    {
                        textBox6.Text += archiveNo + " *位置-->形成（成文）时间：" + xcsj + " \r\n";
                        textBox6.SelectionStart = textBox6.Text.Length;
                        textBox6.ScrollToCaret();
                    }
                }
            }
        }

        private void CheckYearAndPrimaryTime(DataRow[] rows, string archiveNo, bool useTheYearFieldBool, string theYearFieldName, bool useCreatedTimeFieldBool, string createdTimeFieldName)
        {
            if (useTheYearFieldBool)
            {
                string nd = string.Empty;
                try
                {
                    nd = rows[0][theYearFieldName].ToString();
                    DateTime myDateTime = DateTime.ParseExact(nd, "yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    textBox6.Text += archiveNo + " *位置-->年度： " + nd + "\r\n";
                    textBox6.SelectionStart = textBox6.Text.Length;
                    textBox6.ScrollToCaret();
                }
            }
            if (useCreatedTimeFieldBool)
            {
                string xcsj = string.Empty;
                try
                {
                    xcsj = rows[0][createdTimeFieldName].ToString();
                    DateTime myDateTime2 = DateTime.ParseExact(xcsj, "yyyyMMdd", CultureInfo.InvariantCulture);
                }
                catch
                {
                    if (string.IsNullOrEmpty(xcsj) || !CheckIndexofTwoZero(xcsj))
                    {
                        textBox6.Text += archiveNo + " *位置-->形成（成文）时间：" + xcsj + " \r\n";
                        textBox6.SelectionStart = textBox6.Text.Length;
                        textBox6.ScrollToCaret();
                    }
                }
            }
        }

        private bool CheckIndexofTwoZero(string timeStr)
        {
            string charMadeUp = ConfigurationManager.AppSettings["CharMadeUpInCreatedTime"];
            if (string.IsNullOrEmpty(charMadeUp))
                charMadeUp = "00";

            int index = timeStr.IndexOf(charMadeUp);
            if (index >= 1 && index <= 6)
            {
                return true;
            }
            return false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Clipboard.SetDataObject(textBox3.Text);
            //MessageBox.Show(this, "已将数据拷贝到粘贴板，请粘贴到文本文件、word或excel文件中！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
            saveFileDialog1.Filter = "Excel文件(*.xls)|*.xls|Excel文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
                DataTable dt = gridControl_pageLeft.DataSource as DataTable;
                ExcelHelper.GetExcelByDataTable(dt, fileName);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Clipboard.SetDataObject(textBox5.Text);
            //MessageBox.Show(this, "已将数据拷贝到粘贴板，请粘贴到文本文件、word或excel文件中！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
            saveFileDialog1.Filter = "Excel文件(*.xls)|*.xls|Excel文件(*.xlsx)|*.xlsx|所有文件(*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
                DataTable dt = gridControl_pageNumberError.DataSource as DataTable;
                ExcelHelper.GetExcelByDataTable(dt, fileName);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(textBox6.Text);
            MessageBox.Show(this, "已将数据拷贝到粘贴板，请粘贴到文本文件、word或excel文件中！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(textBox4.Text);
            MessageBox.Show(this, "已将数据拷贝到粘贴板，请粘贴到文本文件、word或excel文件中！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void AutoFileCheck_PageLeftAndReadablecs_FormClosed()
        {
            this.CanRecurrenceHanppen = false;//通知正在进行的自动检测停止工作
            if (this.DTForPageLeft != null) this.DTForPageLeft.Dispose();
            if (this.DTForPageNumberError != null) this.DTForPageNumberError.Dispose();
        }

        private void textEdit1_Click(object sender, EventArgs e)
        {
            //if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    textEdit1.Text = folderBrowserDialog1.SelectedPath;
            //}
        }
    }
}