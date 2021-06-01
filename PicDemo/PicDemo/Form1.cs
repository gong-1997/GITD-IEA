using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV.ML;
using Emgu.CV.Structure;
namespace PicDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Image<Gray, byte> img = new Image<Gray, byte>("E:\\测试\\c.bmp");
            pictureBox1.Image = img.ToBitmap();
            trackBar1.Maximum = 100;
            textBox1.Text = trackBar1.Value.ToString();
            
        }

        Image img1;
        private void button1_Click(object sender, EventArgs e)
        {
            //OpenFileDialog of = new OpenFileDialog();
            //if (of.ShowDialog() != DialogResult.OK)
            //{ return; }
            Image<Gray, byte> img = new Image<Gray, byte>("E:\\测试\\h.png");


            Mat dst1 = new Mat();
            CvInvoke.Canny(img, dst1, 120, 180);
            pictureBox1.Image = dst1.Bitmap;
            VectorOfVectorOfPoint vvp = new VectorOfVectorOfPoint();
            VectorOfVectorOfPoint use_vvp = new VectorOfVectorOfPoint();
           
            CvInvoke.FindContours(dst1, vvp, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
           
            int number = vvp.ToArrayOfArray().Length;//取得轮廓的数量.
            MessageBox.Show("轮廓数目为" + ":" + number.ToString());
            for (int i = 0; i < number; i++)
            {
                VectorOfPoint vp = vvp[i];
                //double area = CvInvoke.ContourArea(vp);
                double length = CvInvoke.ArcLength(vp,true);
                if (length>500)//可按实际图片修改
                { use_vvp.Push(vp); }


            }
            Mat result = new Mat(img.Size, Emgu.CV.CvEnum.DepthType.Cv8U, 3);
            result.SetTo(new MCvScalar(255, 255, 255));
            CvInvoke.DrawContours(result, use_vvp, -1, new MCvScalar(0, 0, 0));
            
            
            pictureBox1.Image =result.Bitmap;
            MessageBox.Show("去噪成功!");
            //Image bit = pictureBox1.Image;
            //bit.Save("E:\\测试\\yy.png");
            //pictureBox3.Image = img.Bitmap;
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            //OpenFileDialog of = new OpenFileDialog();
            //if (of.ShowDialog() != DialogResult.OK)
            //{ return; }
            Image<Gray, byte> img = new Image<Gray, byte>("E:\\测试\\c.bmp");
             
            int aa=5;
            if (trackBar1.Value % 2 == 1)
            {
                aa =trackBar1.Value;
            
            }
            pictureBox1.Image = img.SmoothMedian(5).ToBitmap();
            Image bit = pictureBox1.Image;
            bit.Save("E:\\测试\\h.png");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Image<Gray, byte> img = new Image<Gray, byte>("E:\\测试\\c.bmp");

            textBox1.Text = trackBar1.Value.ToString();
            int aa = 5;
            if (trackBar1.Value % 2 == 1)
            {
                aa = trackBar1.Value;
                pictureBox1.Image = img.SmoothMedian(aa).ToBitmap();

            }
            
            Image bit = pictureBox1.Image;
            bit.Save("E:\\测试\\h.png");
        }
    }
}
