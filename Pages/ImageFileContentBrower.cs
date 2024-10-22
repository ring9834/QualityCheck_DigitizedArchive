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
using System.IO;

namespace Prj_FileManageNCheckApp
{
    public partial class ImageFileContentBrower : DevExpress.XtraEditors.XtraForm
    {
        private List<KeyValuePair<string, string>> ImageList { get; set; }
        private string Dh { get; set; }
        //private int mouseX;
        //private int mouseY;
        //private int picX;
        //private int picY;  
        Point pck;
        bool dragFlag = false;
        Point location;
        public ImageFileContentBrower()
        {
            InitializeComponent();
        }

        public ImageFileContentBrower(string dh, List<KeyValuePair<string, string>> imageList)
        {
            InitializeComponent();
            ImageList = imageList;
            this.Dh = dh;
            this.pictureBox1.MouseWheel += pictureBox1_MouseWheel;
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }

        void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            //this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            if (e.Delta >= 0)
            {
                pictureBox1.Width = (int)(pictureBox1.Width * 1.1);//因为Widthh和Height都是int类型，所以要强制转换一下-_-||  
                pictureBox1.Height = (int)(pictureBox1.Height * 1.1);
            }
            else
            {
                pictureBox1.Width = (int)(pictureBox1.Width * 0.9);
                pictureBox1.Height = (int)(pictureBox1.Height * 0.9);
            }
        }

        private void ImageFileContentBrower_Load(object sender, EventArgs e)
        {
            this.pictureBox1.Cursor = Cursors.SizeAll;
            PageControlLocation.MakeControlHoritionalCenter(panel4.Parent, panel4);
            groupControl1.Text = "档号：" + this.Dh;
            for (int i = 0; i < this.ImageList.Count; i++)
            {
                this.imageListBoxControl1.Items.Add(this.ImageList[i].Value);
            }
            if (this.ImageList.Count > 0)
            {
                string ywRootPath = ConfigurationManager.AppSettings["ContentFileRootPath"];
                string path = ywRootPath + ImageList[0].Key.ToString();
                if (File.Exists(path))
                {
                    pictureBox1.Image = Image.FromFile(path);
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        private void imageListBoxControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = imageListBoxControl1.SelectedIndex;
            string ywRootPath = ConfigurationManager.AppSettings["ContentFileRootPath"];
            string path = ywRootPath + ImageList[index].Key.ToString();
            if (File.Exists(path))
            {
                pictureBox1.Image = Image.FromFile(path);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //pck = new Point(e.X, e.X);
            //dragFlag = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //if (dragFlag)
            //{
            //    Point pend = this.PointToClient(pictureBox1.PointToScreen(new Point(e.X, e.Y)));
            //    pend.Offset(pck.X * -1, pck.Y * -1);
            //    pictureBox1.Location = pend;
            //}
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //dragFlag = false;
        }

        private void ImageFileContentBrower_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                this.imageListBoxControl1.Focus();
            }
        }

        private void ImageFileContentBrower_Resize(object sender, EventArgs e)
        {
            pictureBox1.Width = panel2.Width;
            pictureBox1.Height = panel2.Height;
            PageControlLocation.MakeControlHoritionalCenter(panel4.Parent, panel4);
        }

        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            pictureBox1.Width = (int)(pictureBox1.Width * 0.9);
            pictureBox1.Height = (int)(pictureBox1.Height * 0.9);
        }

        private void simpleButton2_Click_1(object sender, EventArgs e)
        {
            pictureBox1.Width = (int)(pictureBox1.Width * 1.1);//因为Widthh和Height都是int类型，所以要强制转换一下-_-||  
            pictureBox1.Height = (int)(pictureBox1.Height * 1.1);
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            pictureBox1.Width = panel2.Width;
            pictureBox1.Height = panel2.Height;
        }

    }
}