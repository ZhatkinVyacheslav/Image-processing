using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Bitmap bm1;
        Bitmap bm2;
        Bitmap bm3;
        public Form1()
        {
            InitializeComponent();
            chart1.Series[0].Points.Clear();
            bm1 = new Bitmap(pictureBox1.Image);
            bm3 = new Bitmap(pictureBox3.Image);
        }

        void NoiseReyleigh1()
        {
            var image = new Bitmap(pictureBox1.Image);
            int w = image.Width;
            int h = image.Height;

            BitmapData image_data = image.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            int bytes = image_data.Stride * image_data.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
            image.UnlockBits(image_data);

            byte[] noise = new byte[bytes];
            double[] rayleigh = new double[256];

            double a = 0;
            double b = 0.4;

            Random rnd = new Random();
            double sum = 0;

            for (int i = 0; i < 256; i++)
            {
                double step = (double)i * 0.01;
                if (step >= a)
                {
                    rayleigh[i] = (double)((2 / b) * (step - a) * Math.Exp(-Math.Pow(step - a, 2) / b));
                }
                else
                {
                    rayleigh[i] = 0;
                }
                sum += rayleigh[i];
            }

            for (int i = 0; i < 256; i++)
            {
                rayleigh[i] /= sum;
                rayleigh[i] *= bytes;
                rayleigh[i] = (int)Math.Floor(rayleigh[i]);
                chart1.Series[0].Points.AddXY(i, rayleigh[i]);
            }

            int count = 0;
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < (int)rayleigh[i]; j++)
                {
                    noise[j + count] = (byte)i;
                }
                count += (int)rayleigh[i];
            }

            for (int i = 0; i < bytes - count; i++)
            {
                noise[count + i] = 0;
            }

            noise = noise.OrderBy(x => rnd.Next()).ToArray();

            for (int i = 0; i < bytes; i++)
            {
                result[i] = (byte)(buffer[i] + noise[i]);
            }

            Bitmap result_image = new Bitmap(w, h);
            BitmapData result_data = result_image.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
            Marshal.Copy(result, 0, result_data.Scan0, bytes);
            result_image.UnlockBits(result_data);
            pictureBox2.Image = result_image;
        }

        private void шумРайлиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NoiseReyleigh1();
            bm2 = new Bitmap(pictureBox2.Image);
        }

        private void контрагармоническоеСреднее1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = Contraharmonic.ContraharmonicMean(bm1, 1);
            bm2 = new Bitmap(pictureBox2.Image);
        }

        private void контрагармоническоеСреднее1ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = Contraharmonic.ContraharmonicMean(bm1, -1);
            bm2 = new Bitmap(pictureBox2.Image);

            pictureBox2.Image = Contraharmonic.ContraharmonicMean(bm2, 1);
            bm2 = new Bitmap(pictureBox2.Image);

            pictureBox2.Image = Contraharmonic.ContraharmonicMean(bm2, 1);
            bm2 = new Bitmap(pictureBox2.Image);

            Compare();
        }

        private void среднеарифметическоеУсреднениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = MedianFilter.Median(bm1);
            bm2 = new Bitmap(pictureBox2.Image);
            Compare();
        }

        private void Compare()
        {
            float res = MSE.imageCompareMSE(bm2, bm3);
            label1.Text = res.ToString();

            float res2 = UIQ.imageCompareUIQ(bm2, bm3);
            label2.Text = res2.ToString();
        }
    }
}
