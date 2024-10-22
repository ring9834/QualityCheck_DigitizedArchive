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
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace Prj_FileManageNCheckApp
{
    public partial class AutoFileCheck_ImageEgdeNLean : DevExpress.XtraEditors.XtraForm
    {
        public string FileCodeName { get; set; }//标识档案类型的代码，比如是案卷档案、卷内档案，还是归档文件等
        int Counter = 0;
        private double binaryzationThreshold;
        protected double BinaryzationThreshold//阈值
        {
            get
            {
                string tempValue = ConfigurationManager.AppSettings["BinaryzationThreshold"];
                if (string.IsNullOrEmpty(tempValue))
                {
                    binaryzationThreshold = 0.4;
                    return binaryzationThreshold;
                }
                else
                {
                    binaryzationThreshold = double.Parse(tempValue);
                    return binaryzationThreshold;
                }
            }
        }

        private int sideLengthVlue;
        protected int SideLengthVlue//取图片四个角落区域的正方形的边长（单位为像素）
        {
            get
            {
                string tempValue = ConfigurationManager.AppSettings["SideLengthVlue"];
                if (string.IsNullOrEmpty(tempValue))
                {
                    sideLengthVlue = 50;
                    return sideLengthVlue;
                }
                else
                {
                    sideLengthVlue = int.Parse(tempValue);
                    return sideLengthVlue;
                }
            }
        }

        public AutoFileCheck_ImageEgdeNLean()
        {
            InitializeComponent();
        }

        public AutoFileCheck_ImageEgdeNLean(string fileCodeName)
        {
            InitializeComponent();
            this.FileCodeName = fileCodeName;
        }

        private void textEdit2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textEdit2.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Counter = 0;
            textBox3.Clear(); textBox2.Clear();
            ExamineRawPics(textEdit2.Text);
            textBox3.Text += "裁边检测执行完毕！";
            textBox2.Text += "歪斜度检测执行完毕！";
            //button1.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(textBox3.Text);
            MessageBox.Show(this, "已将数据拷贝到粘贴板，请粘贴到文本文件、word或excel文件中！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(textBox2.Text);
            MessageBox.Show(this, "已将数据拷贝到粘贴板，请粘贴到文本文件、word或excel文件中！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExamineRawPics(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] fi0 = di.GetFiles("*.JPG");
            FileInfo[] fi1 = di.GetFiles("*.JPEG");
            IEnumerable<FileInfo> fi3 = fi0.Union(fi1);
            //FileInfo[] fi2 = di.GetFiles("*.TIFF");
            //IEnumerable<FileInfo> fi3 = fi0.Union(fi1).Union(fi2);

            for (int n = 0; n < fi3.Count(); n++)
            {
                string file = fi3.ElementAt(n).FullName;
                this.Counter++;
                label3.Text = "（从" + this.Counter + "幅图片中检测）正在检测：" + file;

                try
                {
                    Bitmap bmp = new Bitmap(file);

                    bool flag = true;
                    if (checkEdit_makeupSize.Checked)//是否补过边
                    {
                        Bitmap bmp_small = ConvertTo1Bpp2(bmp, true);//图像二值化,二值化的区域比较小
                        flag = GetWhiteEage(bmp_small);
                        bmp_small.Dispose();
                    }
                    else
                    {
                        Bitmap bmp_small = ConvertTo1Bpp2(bmp, false);
                        bmp_small.Save(@"e:\111.jpg");
                        double ratio = GetWhitePixesPecentByAverage(bmp_small);//四个角落的正方形取的值
                        double ratio2 = GetWhitePixesPecentByAverage2(bmp_small);//往里推进一定长度后四个正方形取的值
                        double banlance = ratio2 - ratio;
                        if (ratio2 > 0.5)//纸张颜色较深
                        {
                            if (banlance < 0 && banlance >= 0.2)
                                flag = false;//有黑边
                            if (banlance > 0 && banlance >= 0.2)
                                flag = false;//有黑边
                        }
                        else
                        {
                            if (banlance < 0 && Math.Abs(banlance) >= 0.001)
                                flag = false;//有黑边
                        }
                        bmp_small.Dispose();
                    }
                    if (!flag)
                    {
                        textBox3.Text += file + "\r\n";
                        textBox3.SelectionStart = textBox3.Text.Length;
                        textBox3.ScrollToCaret();
                        Application.DoEvents();
                    }
                    if (checkEdit1.Checked)//检测图片的歪斜度
                    {
                        double angle = DeskewImage(bmp, 190);
                        if (Math.Abs(angle) > 20) //超过20度
                        {
                            textBox2.Text += file + "\r\n";
                            textBox2.SelectionStart = textBox2.Text.Length;
                            textBox2.ScrollToCaret();
                            Application.DoEvents();
                        }
                    }

                    bmp.Dispose();
                }
                catch//图片有可能不可读
                {
                    textBox3.Text += file + "图片可能损坏，不可读！\r\n";
                    textBox3.SelectionStart = textBox3.Text.Length;
                    textBox3.ScrollToCaret();
                    Application.DoEvents();
                }
            }

            DirectoryInfo[] diA = di.GetDirectories();
            for (int m = 0; m < diA.Length; m++)
            {
                string name3 = diA[m].FullName;
                ExamineRawPics(name3);//递归
            }
        }

        //二值化四个角
        public Bitmap ConvertTo1Bpp2(Bitmap img, bool edgeMadeupToA4A3)
        {
            int aConstant = SideLengthVlue;//图片四个角各取一个正方形，它就是边长。也就是说对四个角的区域进行二值化
            int w = img.Width;
            int h = img.Height;
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);

            for (int y = 0; y < h; y++)
            {
                Application.DoEvents();
                if (y == aConstant)//跳过中间的大片区域，节省时间
                {
                    y = h - aConstant;
                }

                byte[] scan = new byte[(w + 7) / 8];

                for (int x = 0; x < w; x++)
                {
                    if (x == aConstant)//跳过中间的大片区域，节省时间
                    {
                        x = w - aConstant;
                    }
                    Color c = img.GetPixel(x, y);
                    if (c.GetBrightness() >= BinaryzationThreshold)//二值化的阈值，0至1.0,默认0.4
                        scan[x / 8] |= (byte)(0x80 >> (x % 8));
                }
                Marshal.Copy(scan, 0, (IntPtr)((int)data.Scan0 + data.Stride * y), scan.Length);
            }
            if (!edgeMadeupToA4A3)//如果补过边成了A4A3等纸张的大小，就不执行一下的循环
            {
                int insideWidth = w / 4;//取四个正方形区域往里推进的长度
                int insideHeight = h / 4;

                //往里推进一个长度，取四个正方形进行二值化
                for (int y = insideHeight; y < h - insideHeight; y++)
                {
                    Application.DoEvents();
                    if (y == aConstant + insideHeight)//跳过中间的大片区域，节省时间
                    {
                        y = h - aConstant - insideHeight;
                    }

                    byte[] scan = new byte[(w + 7) / 8];

                    for (int x = insideWidth; x < w - insideWidth; x++)
                    {
                        if (x == aConstant + insideWidth)//跳过中间的大片区域，节省时间
                        {
                            x = w - aConstant - insideWidth;
                        }
                        Color c = img.GetPixel(x, y);
                        if (c.GetBrightness() >= BinaryzationThreshold)//二值化的阈值，0至1.0,默认0.4
                            scan[x / 8] |= (byte)(0x80 >> (x % 8));
                    }
                    Marshal.Copy(scan, 0, (IntPtr)((int)data.Scan0 + data.Stride * y), scan.Length);
                }
            }
            bmp.UnlockBits(data);
            return bmp;
        }

        private bool GetWhiteEage(Bitmap p_BMP)
        {
            Bitmap tmpPic = p_BMP;// new Bitmap(p_FileName);
            bool tmpB = false, tmpB2 = false;
            int[] tmpCounts, tmpCounts2;

            tmpCounts = new int[tmpPic.Width];
            tmpCounts2 = new int[tmpPic.Width];

            //图像左边缘区域
            for (int x = 0; x < tmpPic.Width; x++)
            {
                tmpCounts[x] = 0;
                for (int y = 0; y < tmpPic.Height; y++)
                {
                    Color color = tmpPic.GetPixel(x, y);
                    int gray = (int)(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);
                    if (gray == 255)//如果是白色
                    {
                        tmpCounts[x] = tmpCounts[x] + 1;
                        if (tmpCounts[x] > 30 && tmpCounts[x] == y + 1)//连续白点超过多少个
                        {
                            tmpB = true;
                            break;
                        }
                    }
                    else if (gray != 255)
                    {
                        tmpCounts2[x] = tmpCounts2[x] + 1;
                        if (tmpCounts2[x] > 30)
                        {
                            tmpB2 = true;
                            break;
                        }
                    }
                }

                if (tmpB | tmpB2)
                {
                    break;
                }
            }

            if (!tmpB2)
            {
                tmpCounts = new int[tmpPic.Height];
                tmpCounts2 = new int[tmpPic.Height];

                //图像下边缘区域
                for (int y = 0; y < tmpPic.Height; y++)
                {
                    for (int x = 0; x < tmpPic.Width; x++)
                    {
                        Color color = tmpPic.GetPixel(x, y);
                        int gray = (int)(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);
                        if (gray == 255)
                        {
                            tmpCounts[y] = tmpCounts[y] + 1;
                            if (tmpCounts[y] > 30 && tmpCounts[y] == x + 1)//连续白点超过多少个
                            {
                                tmpB = true;
                                break;
                            }
                        }
                        else if (gray != 255)
                        {
                            tmpCounts2[y] = tmpCounts2[y] + 1;
                            if (tmpCounts2[y] > 30)
                            {
                                tmpB2 = true;
                                break;
                            }
                        }
                    }
                    if (tmpB | tmpB2)
                    {
                        break;
                    }
                }

                if (!tmpB2)
                {
                    tmpCounts = new int[tmpPic.Height];
                    tmpCounts2 = new int[tmpPic.Height];

                    //图像上边缘区域
                    for (int y = tmpPic.Height - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < tmpPic.Width; x++)
                        {
                            Color color = tmpPic.GetPixel(x, y);
                            int gray = (int)(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);
                            if (gray == 255)
                            {
                                tmpCounts[y] = tmpCounts[y] + 1;
                                if (tmpCounts[y] > 30 && tmpCounts[y] == x + 1)//连续白点超过多少个
                                {
                                    tmpB = true;
                                    break;
                                }
                            }
                            else if (gray != 255)
                            {
                                tmpCounts2[y] = tmpCounts2[y] + 1;
                                if (tmpCounts2[y] > 30)
                                {
                                    tmpB2 = true;
                                    break;
                                }
                            }
                        }
                        if (tmpB | tmpB2)
                        {
                            break;
                        }
                    }

                    if (!tmpB2)
                    {
                        tmpCounts = new int[tmpPic.Width];
                        tmpCounts2 = new int[tmpPic.Width];

                        //图像右边缘区域
                        for (int x = tmpPic.Width - 1; x >= 0; x--)
                        {
                            for (int y = 0; y < tmpPic.Height; y++)
                            {
                                Color color = tmpPic.GetPixel(x, y);
                                int gray = (int)(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);
                                if (gray == 255)
                                {
                                    tmpCounts[x] = tmpCounts[x] + 1;
                                    if (tmpCounts[x] > 30 && tmpCounts[x] == y + 1)//连续白点超过多少个
                                    {
                                        tmpB = true;
                                        break;
                                    }
                                }
                                else if (gray != 255)
                                {
                                    tmpCounts2[x] = tmpCounts2[x] + 1;
                                    if (tmpCounts2[x] > 30)
                                    {
                                        tmpB2 = true;
                                        break;
                                    }
                                }
                            }
                            if (tmpB | tmpB2)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            if (tmpB2)//一旦图片四个角落中的一个角落出现黑点，就返回false
            {
                tmpB = false;
            }
            return tmpB;
        }

        private double GetWhitePixesPecentByAverage(Bitmap p_BMP)
        {
            int aConstant = this.SideLengthVlue;
            int pileup = 0;//累积值
            double ratio = 0.0;//比值
            int w = p_BMP.Width;
            int h = p_BMP.Height;

            for (int y = 0; y < h; y++)
            {
                Application.DoEvents();
                if (y == aConstant)//跳过中间的大片区域，节省时间
                {
                    y = h - aConstant;
                }
                for (int x = 0; x < w; x++)
                {
                    if (x == aConstant)//跳过中间的大片区域，节省时间
                    {
                        x = w - aConstant;
                    }
                    Color color = p_BMP.GetPixel(x, y);
                    int gray = (int)(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);
                    if (gray != 255)
                    {
                        pileup = pileup + 1;
                    }
                }
            }
            ratio = pileup / (double)(aConstant * aConstant * 4);
            return ratio;//黑像素占比
        }

        private double GetWhitePixesPecentByAverage2(Bitmap p_BMP)
        {
            int aConstant = this.SideLengthVlue;
            int pileup = 0;//累积值
            double ratio = 0.0;//比值
            int w = p_BMP.Width;
            int h = p_BMP.Height;

            int insideWidth = w / 4;//取四个正方形区域往里推进的长度
            int insideHeight = h / 4;

            for (int y = insideHeight; y < h - insideHeight; y++)
            {
                Application.DoEvents();
                if (y == aConstant + insideHeight)//跳过中间的大片区域，节省时间
                {
                    y = h - aConstant - insideHeight;
                }
                for (int x = insideWidth; x < w - insideWidth; x++)
                {
                    if (x == aConstant + insideWidth)//跳过中间的大片区域，节省时间
                    {
                        x = w - aConstant - insideWidth;
                    }
                    Color color = p_BMP.GetPixel(x, y);
                    int gray = (int)(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);
                    if (gray != 255)
                    {
                        pileup = pileup + 1;
                    }
                }
            }
            ratio = pileup / (double)(aConstant * aConstant * 4);
            return ratio;//黑像素占比
        }

        public static double DeskewImage(Bitmap bmp, byte binarizeThreshold)
        {
            RoteImageClass ins = new RoteImageClass();
            Bitmap tempBmp = CropImage(bmp, bmp.Width / 4, bmp.Height / 4, bmp.Width / 2, bmp.Height / 2);
            ins._internalBmp = BinarizeImage(tempBmp, binarizeThreshold);
            double angle = ins.GetSkewAngle();
            return angle;
        }

        private static Bitmap CropImage(Bitmap bmp, int StartX, int StartY, int w, int h)
        {
            try
            {
                Bitmap bmpOut = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmpOut))
                {
                    g.DrawImage(bmp, new Rectangle(0, 0, w, h), new Rectangle(StartX, StartY, w, h), GraphicsUnit.Pixel);
                    g.Dispose();
                    return bmpOut;
                }
            }
            catch
            {
                return null;
            }
        }

        //图像二值化
        private static Bitmap BinarizeImage(Bitmap b, byte threshold)
        {
            int width = b.Width;
            int height = b.Height;
            BitmapData data = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            unsafe
            {
                byte* p = (byte*)data.Scan0;
                int offset = data.Stride - width * 4;
                byte R, G, B, gray;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        R = p[2];
                        G = p[1];
                        B = p[0];
                        gray = (byte)((R * 19595 + G * 38469 + B * 7472) >> 16);
                        if (gray >= threshold)
                        {
                            p[0] = p[1] = p[2] = 255;
                        }
                        else
                        {
                            p[0] = p[1] = p[2] = 0;
                        }
                        p += 4;
                    }
                    p += offset;
                }
                b.UnlockBits(data);
                return b;
            }
        }
    }
}