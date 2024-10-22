using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prj_FileManageNCheckApp
{
    public class PageControlLocation
    {
        public static void MakeControlHoritionalCenter(Control parent, Control control)
        {
            //int parentHeight = parent.Height;
            int parentWidth = parent.Width;
            //int controlHeight = control.Height;
            int controlWidth = control.Width;
            //int parentTop = parent.Top;
            int parentLeft = parent.Left;
            //control.Top = parentTop + (parentHeight - controlHeight) / 2;
            control.Left = parentLeft + (parentWidth - controlWidth) / 2;
        }

        public static void MakeControlCenter(Control parent, Control control)
        {
            int parentHeight = parent.Height;
            int parentWidth = parent.Width;
            int controlHeight = control.Height;
            int controlWidth = control.Width;
            int parentTop = parent.Top;
            int parentLeft = parent.Left;
            control.Top = parentTop + (parentHeight - controlHeight) / 2;
            control.Left = parentLeft + (parentWidth - controlWidth) / 2;
        }

        public static void MakeControlVerticalCenter(Control parent, Control control)
        {
            int parentHeight = parent.Height;
            //int parentWidth = parent.Width;
            int controlHeight = control.Height;
            //int controlWidth = control.Width;
            int parentTop = parent.Top;
            //int parentLeft = parent.Left;
            control.Top = parentTop + (parentHeight - controlHeight) / 2;
            //control.Left = parentLeft + (parentWidth - controlWidth) / 2;
        }

        public static void MakeControlHoritionalCenterNextToAnotherControl_Downward(Control control, Control another)
        {
            int parentWidth = another.Parent.Width;
            int controlWidth = another.Width;
            int parentLeft = another.Parent.Left;
            another.Left = parentLeft + (parentWidth - controlWidth) / 2;
            another.Top = control.Top + control.Height;
        }
    }
}
