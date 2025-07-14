# Check Quality of Digitized Archives or Documents in Batch

This system requires that the data which need to be checked for quality should be organized or curated according to some standards like the ones from Archives or other organizations.

## Features of this System
### Flexible Configrations
These configurations covers the basic ones supporting this system to spin, which I would call it System Infrastructure Configuration, for example, user configuration, permission configuration, archive stock configuration; the system also includes business configurations like basic and precise configuration for searching, full index searching configuration. 

More importantly, it also supports configurating both automatic quality check and manual quality check. A lot of properties of archives can be checked automaticlly in batch, which is the biggest advantage of this system for highly improving checking efficiency. The manual manner of quality check can perfectly complement the automatic one when some properties of archives have to be checked manually.

### Multiple Checking Options
Here, the options refer to the ones reated to automatic quality check, each of which corresponds to a property of an archive or document, such as (1)the lack of an archive catelog with its scanned images existing;(2)the lack of archive images with its catalog existing; (3)the scanned images being unadequate in pages compared to the original one; (4)the page number cateloged being not matched with the actual one; (5) the format of cateloged date being wrong; (6) the fond No format being wrong; (7) the length of font No being not correct; (8) confidential documents was scanned; (9) scanned images with unadequate resolution; (10) scanned images with big angle of inclination, etc.

### PDF and Image check
Quality check suports files with PDF or Image(jpg, jpeg, bmp, png, tif, tiff) formats.

### Check Report with Error Details
Detailed report with detailed error results, which directly correlated to the specific archives with the errors, will be generated in the end and can be exported and downloaded as an excel file.

As follows is a snipit of code for checking whether an image having white edges.
```sh
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
```