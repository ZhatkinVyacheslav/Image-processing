using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        Bitmap bm;
        List <Region> regions;
        private Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
            regions = new List <Region>();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreatingRegionsBrightness();
            GetMoments();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreatingRegionsColor();
            GetMoments();
        }

        private void CreatingRegionsBrightness()
        {
            bm = new Bitmap(pictureBox1.Image);
            float ErrorRate = 0.09f;
            Region NewRegion0 = new Region(0, 0, bm);
            regions.Add(NewRegion0);

            for (int x = 0; x < pictureBox1.Image.Width; x++)
            {
                for (int y = 0; y < pictureBox1.Image.Height; y++)
                {
                    bool flag = false;

                    for (int  k = 0; k < regions.Count(); k++)
                    {
                        if (Math.Abs(regions[k].brightness - bm.GetPixel(x, y).GetBrightness()) < ErrorRate)
                        {
                            regions[k].AddPointBrightness(x, y, bm);
                            flag = true;
                            k = regions.Count();
                        }
                    }

                    if(!flag)
                    {
                        Region NewRegion = new Region(x, y, bm);
                        regions.Add(NewRegion);
                    }
                }
            }

            //for (int i = 0; i < regions.Count(); i++)
            //{
            //    bm = regions[i].HighlightBorders(bm);
            //}

            for (int i = 0; i < regions.Count(); i++)
            {
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                bm = regions[i].HighlightRegion(bm, randomColor);
            }

            pictureBox2.Image = bm;
        }

        void GetMoments()
        {
            string result = "";

            for (int i = 0; i < regions.Count(); i++)
            {
                result += "Region" + (1 + i).ToString() + " - " + regions[i].CalculateMoment(bm) + "\n";
            }

            label1.Text = result;
        }

        private void CreatingRegionsColor()
        {
            bm = new Bitmap(pictureBox1.Image);
            int ErrorRate = 90;
            Region NewRegion0 = new Region(0, 0, bm);
            regions.Add(NewRegion0);


            for (int x = 0; x < pictureBox1.Image.Width; x++)
            {
                for (int y = 0; y < pictureBox1.Image.Height; y++)
                {
                    bool flag = false;

                    for (int k = 0; k < regions.Count(); k++)
                    {
                        if ((Math.Abs(regions[k].R - (int)bm.GetPixel(x, y).R) < ErrorRate) &&
                            (Math.Abs(regions[k].G - (int)bm.GetPixel(x, y).G) < ErrorRate) &&
                            (Math.Abs(regions[k].B - (int)bm.GetPixel(x, y).B) < ErrorRate))
                        {
                            regions[k].AddPointColor(x, y, bm);
                            flag = true;
                            k = regions.Count();
                        }
                    }

                    if (!flag)
                    {
                        int r = (int)bm.GetPixel(x, y).R;
                        int g = (int)bm.GetPixel(x, y).G;
                        int b = (int)bm.GetPixel(x, y).B;
                        Region NewRegion = new Region(x, y,r, g, b, bm);
                        regions.Add(NewRegion);
                    }
                }
            }

            for (int i = 0; i < regions.Count(); i++)
            {
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                bm = regions[i].HighlightRegion(bm, randomColor);
            }

            pictureBox2.Image = bm;

        }
    }
}
