using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prj_FileManageNCheckApp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new DataCountReport());
            LoginForm login = new LoginForm();
            login.ShowDialog();
            if (login.DialogResult == DialogResult.OK)
            {
                Application.Run(new XtraForm_FileExamine(login.UserLoggedIn));//判断登陆成功时主进程显示主窗口;把登录成功的用户传递到主窗体
                login.Close();
                login.Dispose();
            }
            else return;  
        }
    }
}
