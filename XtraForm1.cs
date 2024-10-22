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
using DotNet.DbUtilities;
using DevExpress.XtraTab;
using System.Xml;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
using System.Runtime.Remoting.Messaging;
using System.Xml.Serialization;
using System.IO;

namespace Prj_FileManageNCheckApp
{
    public delegate void NeededFunction(ParametersForChildForms parameters);
    public partial class XtraForm_FileExamine : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        public string ContentFormat { get; set; }//原文格式
        public ParametersForChildForms Parameters { get; set; }//实时传给各子窗口的参数

        public string PrefixForTabbedPage = "XtraTab_";
        public string GlobalViriableForTabbedPage { get; set; }
        public Dictionary<string, List<NeededFunction>> NeededFunctions { get; set; }//执行各子窗体函数的委托数组
        private DataTable DTForCheckingPageNumberError { get; set; }
        public UserEntity UserLoggedIn { get; set; }
        private DataTable TreeDataTable { get; set; }
        private Timer TimerForTree { get; set; }

        public XtraForm_FileExamine()
        {
            InitializeComponent();
        }

        public XtraForm_FileExamine(UserEntity userLoggedIn)
        {
            InitializeComponent();
            this.UserLoggedIn = userLoggedIn;
            TimerForTree = new Timer();
            TimerForTree.Interval = 100;
            TimerForTree.Enabled = true;
            TimerForTree.Tick += timer_Tick;
        }

        private void XtraForm1_Load(object sender, EventArgs e)
        {
            LoadFormDataDelegate ldd = new LoadFormDataDelegate(LoadFormDataFunction);
            ldd.BeginInvoke(new AsyncCallback(LoadTreeListDataCallback), null);//异步加载数据，加快窗口显示速度
        }

        private delegate DataTable LoadFormDataDelegate();

        private DataTable LoadFormDataFunction()
        {
            this.NeededFunctions = new Dictionary<string, List<NeededFunction>>();
            return LoadTree();
        }

        void LoadTreeListDataCallback(IAsyncResult result)
        {
            LoadFormDataDelegate lddCallback = (LoadFormDataDelegate)((AsyncResult)result).AsyncDelegate;
            TreeDataTable = lddCallback.EndInvoke(result);
        }

        DataTable LoadTree()
        {
            MainFormDAO fmd = new MainFormDAO();
            DataTable overAll = fmd.LoadTree();
            return overAll;
        }

        void BindDataToTreeList()
        {
            treeList1.DataSource = this.TreeDataTable;
            treeList1.KeyFieldName = "Unique_code";
            treeList1.ParentFieldName = "super_id";
            treeList1.ExpandToLevel(0);//仅展开到2级
            treeList1.Columns["code"].Visible = false;
            treeList1.Columns["node_type"].Visible = false;
            treeList1.Columns["code_value"].Visible = false;
            treeList1.Columns["name"].OptionsColumn.AllowEdit = false;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            VerifyMenusVisibilityOnUserLoggedIn();
            BindDataToTreeList();
            if (treeList1.Nodes.Count > 0)
            {
                TimerForTree.Enabled = false;
                TimerForTree.Dispose();
            }
        }

        void VerifyMenusVisibilityOnUserLoggedIn()//根据登录者的权限，设置工具栏各栏目的可见性
        {
            MainFormDAO mfd = new MainFormDAO();
            DataTable dtFromDB = mfd.VerifyMenusVisibilityOnUserLoggedIn(this.UserLoggedIn.RoleId);
            if (ribbonControl1.Pages.Count > 0)
            {
                for (int i = 0; i < ribbonControl1.Pages.Count; i++)
                {
                    RibbonPage page = ribbonControl1.Pages[i];
                    DataRow[] drs_page = dtFromDB.Select("name='" + page.Name + "'");
                    if (drs_page.Length > 0)
                    {
                        object deal_visible = drs_page[0]["deal_visible"];
                        if (Boolean.Parse(deal_visible.ToString()))
                        {
                            page.Visible = true;
                        }
                        else
                        {
                            page.Visible = false;
                        }
                    }
                    for (int j = 0; j < page.Groups.Count; j++)
                    {
                        RibbonPageGroup group = page.Groups[j];
                        for (int k = 0; k < group.ItemLinks.Count; k++)
                        {
                            BarItemLink itemLink = group.ItemLinks[k];
                            DataRow[] drs_Item = dtFromDB.Select("name='" + itemLink.Item.Name + "'");
                            if (drs_Item.Length > 0)
                            {
                                object deal_visible = drs_Item[0]["deal_visible"];
                                if (Boolean.Parse(deal_visible.ToString()))
                                {
                                    itemLink.Visible = true;
                                }
                                else
                                {
                                    itemLink.Visible = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void treeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            if (e.Node != null)
            {
                if (e.Node == treeList1.Nodes[0]) return;//根节点不进行任何操作

                this.FileCodeName = e.Node.GetValue("code").ToString();
                object contentFormat = e.Node.GetValue("code_value");
                this.ContentFormat = (contentFormat == DBNull.Value || contentFormat == null) ? null : contentFormat.ToString();
                this.Parameters = new ParametersForChildForms();
                this.Parameters.FileCodeName = this.FileCodeName;
                this.Parameters.ContentFormat = this.ContentFormat;

                if (string.IsNullOrEmpty(this.GlobalViriableForTabbedPage))
                    this.GlobalViriableForTabbedPage = "PrimarySearch_SearchResult";//默认为基本搜索

                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(e.Node.GetValue("node_type").ToString());
            }
        }

        private void ShowControlsOnCondition(string condition)
        {
            if (condition.ToLower().Equals("true"))
            {
                xtraTabControl_All.Visible = true;
                splitContainerControl1.Collapsed = false;
                if (this.GlobalViriableForTabbedPage == "PrimarySearch_SearchResult")
                {
                    ShowPrimarySearchWindow();
                }
                if (this.GlobalViriableForTabbedPage == "ReceiveCatalogeProgress")
                {
                    ShowReceiveCatalogeWindow();
                }
                if (this.GlobalViriableForTabbedPage == "FileConnectWithCataloge")
                {
                    ShowCatalogeLinkContentWindow();
                }
                if (this.GlobalViriableForTabbedPage == "ManualCheck_Goto")
                {
                    ShowManualCheckGotoWindow();
                }
                if (this.GlobalViriableForTabbedPage == "ManualCheck_ErrorRecord")
                {
                    ShowManualCheckErrorRecordWindow();
                }
                if (this.GlobalViriableForTabbedPage == "PrimarySearch_Config")
                {
                    ShowPrimarySearchConfig();
                }
                if (this.GlobalViriableForTabbedPage == "ManualCheck_Config")
                {
                    ShowManualCheckConfigWindow();
                }
                if (this.GlobalViriableForTabbedPage == "ArchiveNoGenerate_Config")
                {
                    ShowArchiveNoGenerateConfigWindow();
                }
                if (this.GlobalViriableForTabbedPage == "ShowRcdsForCheckingPageNumber")
                {
                    ShowRcdsForCheckingPageNumberWindow();
                }
                if (this.GlobalViriableForTabbedPage == "AutoFileCheck_Config")
                {
                    ShowAutoFileCheckConfigWindow();
                }
                if (this.GlobalViriableForTabbedPage == "FieldShowType_Config")
                {
                    ShowFieldShowTypeConfigWindow();
                }
                if (this.GlobalViriableForTabbedPage == "BusinessDataDictionary")
                {
                    ShowBusinessDataDictionaryWindow();
                }
                if (this.GlobalViriableForTabbedPage == "RecordsBunchEdit")
                {
                    ShowRecordsBunchEditWindow();
                }
                if (this.GlobalViriableForTabbedPage == "FieldShowList_Config")
                {
                    ShowFieldShowListConfigWindow();
                }
                if (this.GlobalViriableForTabbedPage == "AutoFileCheck_NoYwNNoCataloge")
                {
                    ShowNoYwNNoCatalogeCheckWinow();
                }
                if (this.GlobalViriableForTabbedPage == "AutoFileCheck_PageLeftAndReadablecs")
                {
                    ShowPageLeftAndReadablecsCheckWindow();
                }
                if (this.GlobalViriableForTabbedPage == "AutoFileCheck_NoYwNNoCataloge_Image")
                {
                    ShowNoYwNNoCatalogeImageCheckWinow();
                }
                if (this.GlobalViriableForTabbedPage == "AutoFileCheck_PageLeftAndReadablecs_Image")
                {
                    ShowPageLeftAndReadablecsImageCheckWindow();
                }
                if (this.GlobalViriableForTabbedPage == "SuperSearch")
                {
                    ShowSuperSearchWindow();
                }
            }
            else
            {
                xtraTabControl_All.Visible = false;
                splitContainerControl1.Collapsed = false;
            }
            if (this.GlobalViriableForTabbedPage == "AutoFileCheck_ImageEgdeNLean")
            {
                splitContainerControl1.Collapsed = true;
                xtraTabControl_All.Visible = true;
                ShowImageEgdeNLeanCheckWindow();
            }
            if (this.GlobalViriableForTabbedPage == "SystemCode_Config")
            {
                splitContainerControl1.Collapsed = true;
                xtraTabControl_All.Visible = true;
                ShowSystemCodeConfigWindow();
            }
            if (this.GlobalViriableForTabbedPage == "ArchiveKinds_Config")
            {
                splitContainerControl1.Collapsed = true;
                xtraTabControl_All.Visible = true;
                ShowArchiveKindsConfigWindow();
            }
            if (this.GlobalViriableForTabbedPage == "PublicDataDictionary")
            {
                splitContainerControl1.Collapsed = true;
                xtraTabControl_All.Visible = true;
                ShowPublicDataDictionaryWindow();
            }
            if (this.GlobalViriableForTabbedPage == "UserConfig_Role")
            {
                splitContainerControl1.Collapsed = true;
                xtraTabControl_All.Visible = true;
                ShowUserConfigRoleWindow();
            }
            if (this.GlobalViriableForTabbedPage == "UserConfig_Access")
            {
                splitContainerControl1.Collapsed = true;
                xtraTabControl_All.Visible = true;
                ShowUserConfigAccessWindow();
            }
            if (this.GlobalViriableForTabbedPage == "WorkFlow_Config")
            {
                splitContainerControl1.Collapsed = true;
                xtraTabControl_All.Visible = true;
                ShowWorkFlowConfigWindow();
            }
            if (this.GlobalViriableForTabbedPage == "FindDataSources")
            {
                splitContainerControl1.Collapsed = true;
                xtraTabControl_All.Visible = true;
                ShowDataBaseConfigWindow();
            }
            if (this.GlobalViriableForTabbedPage == "ContentFile_Config")
            {
                splitContainerControl1.Collapsed = true;
                xtraTabControl_All.Visible = true;
                ShowContentFileConfigWindow();
            }
            if (this.GlobalViriableForTabbedPage == "DataCountReport")
            {
                splitContainerControl1.Collapsed = true;
                xtraTabControl_All.Visible = true;
                ShowDataCountReportWindow();
            }
            if (this.GlobalViriableForTabbedPage == "UserManagement")
            {
                splitContainerControl1.Collapsed = true;
                xtraTabControl_All.Visible = true;
                ShowUserManagementWindow();
            }

            foreach (XtraTabPage page in xtraTabControl_All.TabPages)//如果已经打开未初始化业务字典的页面，则关闭
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + "ShowHintInfoDynamically"))
                {
                    //this.GlobalViriableForTabbedPage = page.Name.Substring(this.PrefixForTabbedPage.Length);
                    this.NeededFunctions.Remove("ShowHintInfoDynamically");//对应的应执行的委托也必须去除
                    xtraTabControl_All.TabPages.Remove(page);
                    page.Dispose();
                    return;
                }
            }
        }

        private void ExcuteDelegates()//执行委托  
        {
            if (this.NeededFunctions.Count > 0)
            {
                IEnumerable<KeyValuePair<string, List<NeededFunction>>> func = this.NeededFunctions.Where(f => f.Key.Equals(this.GlobalViriableForTabbedPage));
                if (func.Count() > 0)
                {
                    List<NeededFunction> funcs = func.ElementAt(0).Value;
                    for (int i = 0; i < funcs.Count; i++)
                    {
                        funcs[i](this.Parameters);//执行委托,传递参数到子窗口
                    }
                }
            }
        }

        private void AddDelegateFunctionsToArray(NeededFunction myDelegate)
        {
            if (this.NeededFunctions.Count > 0)
            {
                IEnumerable<KeyValuePair<string, List<NeededFunction>>> funcElements = this.NeededFunctions.Where(f => f.Key.Equals(this.GlobalViriableForTabbedPage));
                if (funcElements.Count() == 0)
                {
                    List<NeededFunction> funcs = new List<NeededFunction>();
                    funcs.Add(myDelegate);
                    this.NeededFunctions.Add(this.GlobalViriableForTabbedPage, funcs);
                }
                else
                {
                    funcElements.ElementAt(0).Value.Add(myDelegate);
                }
            }
            else
            {
                List<NeededFunction> funcs = new List<NeededFunction>();
                funcs.Add(myDelegate);
                this.NeededFunctions.Add(this.GlobalViriableForTabbedPage, funcs);
            }
        }

        public bool ShowHintInfoDynamically()
        {
            xtraTabControl_All.Visible = true;
            if (!Boolean.Parse(treeList1.FocusedNode.GetValue("node_type").ToString())) //是空节点
            {
                string info = "请在档案业务节点下操作！\r\n显示档案业务节点的方法：\r\n1、点击导航树上文字左边的黑色三角展开；\r\n2、双节带黑色三角的节点。\r\n然后，点击屏幕上方工具栏内的工具菜单，以显示相应的窗口！";
                string buttonText = string.Empty;
                ShowHintInfoWindow(info, buttonText);
                return true;
            }

            if (!ConfigUtils.VerifyIfBusinessDictionaryConfigured(this.FileCodeName))
            {
                string info = "此种档案类型库还未进行数据库数据结构的初始化，需要初始化后才能后续的操作。\r\n点击下方按钮，开始初始化。初始化工作包括：\r\n1、创建数据表；\r\n2、公共数据字典自动初始化。";
                string buttonText = "开始初始化";
                ShowHintInfoWindow(info, buttonText);
                return true;
            }

            if (Boolean.Parse(treeList1.FocusedNode.GetValue("node_type").ToString())) //业务节点
            {
                if (this.GlobalViriableForTabbedPage.Equals("ShowHintInfoDynamically"))
                {
                    string info = "点击屏幕上方工具栏内的工具菜单，以显示相应的窗口！";
                    string buttonText = string.Empty;
                    ShowHintInfoWindow(info, buttonText);
                    return true;
                }
            }
            return false;
        }

        void ShowHintInfoWindow(string info, string buttonText)//提示窗口
        {
            this.GlobalViriableForTabbedPage = "ShowHintInfoDynamically";
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ShowHintInfoDynamically form = (ShowHintInfoDynamically)page.Controls.Find("ShowHintInfoDynamically", false)[0];
                    form.ButtonText = buttonText;
                    form.HintInfo = info;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            ShowHintInfoDynamically newForm = new ShowHintInfoDynamically(this.FileCodeName, buttonText, info)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };

            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void ShowManualCheckConfigWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            ManualCheck_Config newForm = new ManualCheck_Config(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };

            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem19_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "ManualCheck_Goto";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        private void ShowManualCheckGotoWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            ManualCheck_Goto newForm = new ManualCheck_Goto(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem20_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "ManualCheck_ErrorRecord";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        private void ShowManualCheckErrorRecordWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            ManualCheck_ErrorRecord newForm = new ManualCheck_ErrorRecord(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void xtraTabControl_All_CloseButtonClick(object sender, EventArgs e)
        {
            DevExpress.XtraTab.ViewInfo.ClosePageButtonEventArgs EArg = (DevExpress.XtraTab.ViewInfo.ClosePageButtonEventArgs)e;
            string name = EArg.Page.Text;//得到关闭的选项卡的text  
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)//遍历得到和关闭的选项卡一样的Text  
            {
                if (page.Text == name)
                {
                    string pageName = page.Name;
                    //if (!pageName.Contains("ShowHintInfoDynamically"))
                    this.GlobalViriableForTabbedPage = pageName.Substring(this.PrefixForTabbedPage.Length);
                    DisposeDatabase(page);
                    this.NeededFunctions.Remove(this.GlobalViriableForTabbedPage);//对应的应执行的委托也必须去除
                    xtraTabControl_All.TabPages.Remove(page);
                    page.Dispose();

                    if (xtraTabControl_All.TabPages.Count == 0)
                        xtraTabControl_All.Visible = false;
                    else
                        xtraTabControl_All.Visible = true;
                    return;
                }
            }
        }

        void DisposeDatabase(XtraTabPage page)
        {
            if (this.GlobalViriableForTabbedPage == "AutoFileCheck_NoYwNNoCataloge")
            {
                Control[] ctrls = page.Controls.Find(GlobalViriableForTabbedPage, false);
                if (ctrls.Length > 0)
                {
                    AutoFileCheck_NoYwNNoCataloge form = (AutoFileCheck_NoYwNNoCataloge)ctrls[0];
                    if (form != null)
                    {
                        form.AutoFileCheck_NoYwNNoCataloge_FormClosed();
                    }
                }
            }
            if (this.GlobalViriableForTabbedPage == "AutoFileCheck_NoYwNNoCataloge_Image")
            {
                Control[] ctrls = page.Controls.Find(GlobalViriableForTabbedPage, false);
                if (ctrls.Length > 0)
                {
                    AutoFileCheck_NoYwNNoCataloge_Image form = (AutoFileCheck_NoYwNNoCataloge_Image)ctrls[0];
                    if (form != null)
                    {
                        form.AutoFileCheck_NoYwNNoCataloge_FormClosed();//释放数据库资源
                    }
                }
            }
            if (this.GlobalViriableForTabbedPage == "AutoFileCheck_PageLeftAndReadablecs")
            {
                Control[] ctrls = page.Controls.Find(GlobalViriableForTabbedPage, false);
                if (ctrls.Length > 0)
                {
                    AutoFileCheck_PageLeftAndReadablecs form = (AutoFileCheck_PageLeftAndReadablecs)ctrls[0];
                    if (form != null)
                    {
                        form.AutoFileCheck_PageLeftAndReadablecs_FormClosed();
                    }
                }
            }
            if (this.GlobalViriableForTabbedPage == "AutoFileCheck_PageLeftAndReadablecs_Image")
            {
                Control[] ctrls = page.Controls.Find(GlobalViriableForTabbedPage, false);
                if (ctrls.Length > 0)
                {
                    AutoFileCheck_PageLeftAndReadablecs_Image form = (AutoFileCheck_PageLeftAndReadablecs_Image)ctrls[0];
                    if (form != null)
                    {
                        form.AutoFileCheck_PageLeftAndReadablecs_FormClosed();
                    }
                }
            }
        }

        private void xtraTabControl_All_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            if (e.Page != null)
            {
                string pageName = e.Page.Name;
                //if (!pageName.Contains("ShowHintInfoDynamically"))
                this.GlobalViriableForTabbedPage = pageName.Substring(this.PrefixForTabbedPage.Length);//如果用户手动切换TabPage，则重新赋值this.GlobalViriableForTabbedPage，以执行相应页面对应的委托
            }
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "PrimarySearch_SearchResult";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        private void ShowPrimarySearchWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            PrimarySearch_SearchResult newForm = new PrimarySearch_SearchResult(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "PrimarySearch_Config";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        private void ShowPrimarySearchConfig()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            PrimarySearch_Config newForm = new PrimarySearch_Config(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "ReceiveCatalogeProgress";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        private void ShowReceiveCatalogeWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            ReceiveCatalogeProgress newForm = new ReceiveCatalogeProgress(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "FileConnectWithCataloge";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        private void ShowCatalogeLinkContentWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            FileConnectWithCataloge newForm = new FileConnectWithCataloge(this.FileCodeName, this.ContentFormat, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，传递第一个参数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrEmpty(this.ContentFormat)) return;

            if (this.ContentFormat.ToLower().Contains("pdf"))
                this.GlobalViriableForTabbedPage = "AutoFileCheck_NoYwNNoCataloge";
            else
                this.GlobalViriableForTabbedPage = "AutoFileCheck_NoYwNNoCataloge_Image";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        private void ShowNoYwNNoCatalogeCheckWinow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            AutoFileCheck_NoYwNNoCataloge newForm = new AutoFileCheck_NoYwNNoCataloge(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 
        }

        private void ShowNoYwNNoCatalogeImageCheckWinow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            AutoFileCheck_NoYwNNoCataloge_Image newForm = new AutoFileCheck_NoYwNNoCataloge_Image(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrEmpty(this.ContentFormat)) return;

            if (this.ContentFormat.ToLower().Contains("pdf"))
                this.GlobalViriableForTabbedPage = "AutoFileCheck_PageLeftAndReadablecs";
            else
                this.GlobalViriableForTabbedPage = "AutoFileCheck_PageLeftAndReadablecs_Image";

            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        private void ShowPageLeftAndReadablecsCheckWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            AutoFileCheck_PageLeftAndReadablecs newForm = new AutoFileCheck_PageLeftAndReadablecs(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            newForm.ShowGridViewForPageNumberErrorFunc += new ShowGridViewForPageNumberError(ShowGridViewForPageNumberErrorFunction);//显示核对页码错误的记录页（多项）
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 
        }

        private void ShowPageLeftAndReadablecsImageCheckWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            AutoFileCheck_PageLeftAndReadablecs_Image newForm = new AutoFileCheck_PageLeftAndReadablecs_Image(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            newForm.ShowGridViewForPageNumberErrorFunc += new ShowGridViewForPageNumberError_Image(ShowGridViewForPageNumberErrorFunction);//显示核对页码错误的记录页（多项）
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 
        }

        void ShowGridViewForPageNumberErrorFunction(DataTable dt)
        {
            this.GlobalViriableForTabbedPage = "ShowRcdsForCheckingPageNumber";
            DTForCheckingPageNumberError = dt;
            ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
        }

        void ShowRcdsForCheckingPageNumberWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ShowRcdsForCheckingPageNumber form = (ShowRcdsForCheckingPageNumber)page.Controls.Find("ShowRcdsForCheckingPageNumber", false)[0];
                    form.DT = DTForCheckingPageNumberError;//更新DATATABLE
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }
            ShowRcdsForCheckingPageNumber newForm = new ShowRcdsForCheckingPageNumber(DTForCheckingPageNumberError)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "AutoFileCheck_ImageEgdeNLean";
            ShowControlsOnCondition(string.Empty);
        }

        private void ShowImageEgdeNLeanCheckWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            AutoFileCheck_ImageEgdeNLean newForm = new AutoFileCheck_ImageEgdeNLean(this.FileCodeName)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            //ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 
        }

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "ArchiveNoGenerate_Config";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        private void ShowArchiveNoGenerateConfigWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            ArchiveNoGenerate_Config newForm = new ArchiveNoGenerate_Config(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem18_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "ContentFile_Config";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        private void ShowContentFileConfigWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            ContentFile_Config newForm = new ContentFile_Config(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem16_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barButtonItem29_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "ManualCheck_Config";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        private void barButtonItem28_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "AutoFileCheck_Config";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        void ShowAutoFileCheckConfigWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            AutoFileCheck_Config newForm = new AutoFileCheck_Config(this.FileCodeName)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void XtraForm_FileExamine_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (this.GlobalViriableForTabbedPage == "AutoFileCheck_NoYwNNoCataloge")
                {
                    if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                    {
                        Control[] ctrls = page.Controls.Find(GlobalViriableForTabbedPage, false);
                        if (ctrls.Length > 0)
                        {
                            AutoFileCheck_NoYwNNoCataloge form = (AutoFileCheck_NoYwNNoCataloge)ctrls[0];
                            if (form != null)
                            {
                                form.AutoFileCheck_NoYwNNoCataloge_FormClosed();//释放数据库资源
                                break;
                            }
                        }
                    }
                }
                if (this.GlobalViriableForTabbedPage == "AutoFileCheck_NoYwNNoCataloge_Image")
                {
                    if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                    {
                        Control[] ctrls = page.Controls.Find(GlobalViriableForTabbedPage, false);
                        if (ctrls.Length > 0)
                        {
                            AutoFileCheck_NoYwNNoCataloge_Image form = (AutoFileCheck_NoYwNNoCataloge_Image)ctrls[0];
                            if (form != null)
                            {
                                form.AutoFileCheck_NoYwNNoCataloge_FormClosed();//释放数据库资源
                                break;
                            }
                        }
                    }
                }
                if (this.GlobalViriableForTabbedPage == "AutoFileCheck_PageLeftAndReadablecs")
                {
                    if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                    {
                        Control[] ctrls = page.Controls.Find(GlobalViriableForTabbedPage, false);
                        if (ctrls.Length > 0)
                        {
                            AutoFileCheck_PageLeftAndReadablecs form = (AutoFileCheck_PageLeftAndReadablecs)ctrls[0];
                            if (form != null)
                            {
                                form.AutoFileCheck_PageLeftAndReadablecs_FormClosed();//释放数据库资源
                                break;
                            }
                        }
                    }
                }
                if (this.GlobalViriableForTabbedPage == "AutoFileCheck_PageLeftAndReadablecs_Image")
                {
                    if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                    {
                        Control[] ctrls = page.Controls.Find(GlobalViriableForTabbedPage, false);
                        if (ctrls.Length > 0)
                        {
                            AutoFileCheck_PageLeftAndReadablecs_Image form = (AutoFileCheck_PageLeftAndReadablecs_Image)ctrls[0];
                            if (form != null)
                            {
                                form.AutoFileCheck_PageLeftAndReadablecs_FormClosed();//释放数据库资源
                                break;
                            }
                        }
                    }
                }
            }

        }

        private void barButtonItem16_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "SystemCode_Config";
            ShowControlsOnCondition(string.Empty);
        }

        void ShowSystemCodeConfigWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            SystemCode_Config newForm = new SystemCode_Config(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "FieldShowType_Config";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        void ShowFieldShowTypeConfigWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            FieldShowType_Config newForm = new FieldShowType_Config(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void treeList1_GetSelectImage(object sender, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            if (e.Node != null)
            {
                if (e.Node.Level == 0)
                {
                    e.NodeImageIndex = 0;
                }
                if (e.Node.Level == 1)
                {
                    e.NodeImageIndex = 1;
                }
                if (e.Node.Level == 2)
                {
                    e.NodeImageIndex = 2;
                }
                if (e.FocusedNode)
                {
                    e.NodeImageIndex = 3;
                }
            }
        }

        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "ArchiveKinds_Config";
            ShowControlsOnCondition(string.Empty);
        }

        void ShowArchiveKindsConfigWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            ArchiveKinds_Config newForm = new ArchiveKinds_Config(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            newForm.UpdateTreeListDelegateFunc = RefreshTreeListData;
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        void RefreshTreeListData()
        {
            this.TreeDataTable = LoadTree();
            BindDataToTreeList();
        }

        private void barButtonItem14_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "PublicDataDictionary";
            ShowControlsOnCondition(string.Empty);
        }

        void ShowPublicDataDictionaryWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            PublicDataDictionary newForm = new PublicDataDictionary(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "BusinessDataDictionary";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        void ShowBusinessDataDictionaryWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            BusinessDataDictionary newForm = new BusinessDataDictionary(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barButtonItem15_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "RecordsBunchEdit";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        void ShowRecordsBunchEditWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            RecordsBunchEdit newForm = new RecordsBunchEdit(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "RecordsBunchEdit";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        private void barButtonItem26_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "UserConfig_Role";
            ShowControlsOnCondition(string.Empty);
        }

        void ShowUserConfigRoleWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            UserConfig_Role newForm = new UserConfig_Role(this.FileCodeName)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem27_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "UserConfig_Access";
            ShowControlsOnCondition(string.Empty);
        }

        void ShowUserConfigAccessWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            UserConfig_Access newForm = new UserConfig_Access(this.FileCodeName)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newForm.GetMenuDataTableFunc = GenerateXMLFromMenu;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        DataTable GenerateXMLFromMenu()
        {
            if (ribbonControl1.Pages.Count > 0)
            {
                DataTable doc = new DataTable();
                doc.Columns.Add("id", typeof(int));
                doc.Columns.Add("parent_id", typeof(int));
                doc.Columns.Add("caption", typeof(string));
                doc.Columns.Add("name", typeof(string));
                doc.Columns.Add("page_name_directedto", typeof(string));
                DataRow root = doc.NewRow();
                root["id"] = -1;
                root["parent_id"] = -1;
                root["caption"] = "工具栏权限分配";
                root["name"] = string.Empty;
                root["page_name_directedto"] = string.Empty;
                doc.Rows.Add(root);
                int counter = 0;
                for (int i = 0; i < ribbonControl1.Pages.Count; i++)
                {
                    RibbonPage page = ribbonControl1.Pages[i];
                    DataRow drPage = doc.NewRow();
                    drPage["id"] = i;
                    drPage["parent_id"] = -1;
                    drPage["caption"] = page.Text;
                    drPage["name"] = page.Name;
                    drPage["page_name_directedto"] = string.Empty;
                    doc.Rows.Add(drPage);
                    for (int j = 0; j < page.Groups.Count; j++)
                    {
                        RibbonPageGroup group = page.Groups[j];
                        for (int k = 0; k < group.ItemLinks.Count; k++)
                        {
                            BarItemLink itemLink = group.ItemLinks[k];
                            DataRow drItem = doc.NewRow();
                            drItem["id"] = ribbonControl1.Pages.Count + counter;
                            drItem["parent_id"] = i;
                            drItem["caption"] = itemLink.Caption;
                            drItem["name"] = itemLink.Item.Name;
                            drItem["page_name_directedto"] = itemLink.Item.Tag == null ? string.Empty : itemLink.Item.Tag.ToString();
                            doc.Rows.Add(drItem);
                            counter++;
                        }
                    }
                }
                return doc;
            }
            return null;
        }

        private void barButtonItem22_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void barButtonItem21_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void barButtonItem24_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void barButtonItem17_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "FieldShowList_Config";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        void ShowFieldShowListConfigWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            FieldShowList_Config newForm = new FieldShowList_Config(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem25_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "FindDataSources";
            ShowControlsOnCondition(string.Empty);
        }

        void ShowDataBaseConfigWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            FindDataSources newForm = new FindDataSources(this.FileCodeName, this.UserLoggedIn)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem23_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void barButtonItem30_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "WorkFlow_Config";
            ShowControlsOnCondition(string.Empty);
        }

        void ShowWorkFlowConfigWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            WorkFlow_Config newForm = new WorkFlow_Config(this.FileCodeName)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem35_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "AutoFileCheck_Config";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        private void barButtonItem36_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "ManualCheck_Config";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        private void XtraForm_FileExamine_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("确定要关闭本系统吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                e.Cancel = false;
            else
                e.Cancel = true;
        }

        private void barButtonItem48_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "DataCountReport";
            ShowControlsOnCondition(string.Empty);
        }

        void ShowDataCountReportWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            DataCountReport newForm = new DataCountReport(this.FileCodeName)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void ribbonControl1_SelectedPageChanged(object sender, EventArgs e)
        {
            if (ribbonControl1.SelectedPage.Name.Equals("ribbonPageReports"))
            {
                ReportDAO rd = new ReportDAO();
                DataTable dt_tjlx = rd.GetStatisticTypes();
                pageGroupReports.ItemLinks.Clear();
                for (int i = 0; i < dt_tjlx.Rows.Count; i++)
                {
                    BarButtonItem bbi = new BarButtonItem();
                    bbi.Caption = dt_tjlx.Rows[i]["code_name"].ToString();
                    bbi.Tag = dt_tjlx.Rows[i]["code_value"].ToString();
                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XtraForm_FileExamine));
                    bbi.Glyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem48.Glyph")));
                    bbi.ItemClick += bbi_ItemClick;
                    pageGroupReports.ItemLinks.Add(bbi);
                }
            }
        }

        private ReportParams DeSerializeXmlToObject(string xmlString)
        {
            XmlSerializer dser = new XmlSerializer(typeof(ReportParams));
            //xmlString是你从数据库获取的字符串
            Stream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));
            ReportParams rp = dser.Deserialize(xmlStream) as ReportParams;
            return rp;
        }

        void bbi_ItemClick(object sender, ItemClickEventArgs e)
        {
            string reportType = e.Item.Tag.ToString();
            ReportDAO rd = new ReportDAO();
            object reportCondition = rd.GetReportCondition(reportType);
            if (reportCondition != null)
            {
                ReportParams rp = DeSerializeXmlToObject(reportCondition.ToString());
                if (this.Parameters == null)
                {
                    this.Parameters = new ParametersForChildForms();
                }
                this.Parameters.RP = rp;
                this.GlobalViriableForTabbedPage = "DataCountReport";//打开报表生成窗口
                ShowControlsOnCondition(string.Empty);
            }
        }

        private void barButtonItem38_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "SuperSearch";
            if (treeList1.FocusedNode != null)
            {
                if (ShowHintInfoDynamically()) return;//显示提醒消息
                ShowControlsOnCondition(treeList1.FocusedNode.GetValue("node_type").ToString());
            }
        }

        void ShowSuperSearchWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            SuperSearch newForm = new SuperSearch(this.FileCodeName)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }

        private void barButtonItem39_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void barButtonItem37_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.GlobalViriableForTabbedPage = "UserManagement";
            ShowControlsOnCondition(string.Empty);
        }

        void ShowUserManagementWindow()
        {
            foreach (XtraTabPage page in xtraTabControl_All.TabPages)
            {
                if (page.Name.Equals(this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage))
                {
                    xtraTabControl_All.SelectedTabPage = page;
                    ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数 刷新页面用
                    return;
                }
            }

            UserManagement newForm = new UserManagement(this.FileCodeName)
            {
                Visible = true,
                TopLevel = false,
                Dock = DockStyle.Fill,
                FormBorderStyle = FormBorderStyle.None,
            };
            AddDelegateFunctionsToArray(newForm.InitiateDatas);//委托赋值，用于执行子窗体中的一个函数
            XtraTabPage newPage = new XtraTabPage();
            newPage.Name = this.PrefixForTabbedPage + this.GlobalViriableForTabbedPage;
            newPage.Text = newForm.Text;
            newPage.Controls.Add(newForm);
            xtraTabControl_All.TabPages.Add(newPage);
            xtraTabControl_All.SelectedTabPage = newPage;
            ExcuteDelegates();//执行委托,就是执行进入页面需要执行的函数
        }
    }
}