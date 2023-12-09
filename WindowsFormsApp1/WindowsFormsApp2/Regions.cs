using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Web;



namespace WindowsFormsApp2
{
    internal class Region
    {
        public List<KeyValuePair<int, int>> pixels;
        public float brightness;
        public Color color;
        public int R;
        public int G;
        public int B;


        public Region()
        {
            pixels = new List<KeyValuePair<int, int>>();
        }

        public Region(int x, int y, Bitmap soursebmp)
        {
            pixels = new List<KeyValuePair<int, int>>();
            brightness = soursebmp.GetPixel(x, y).GetBrightness();
            AddPointBrightness(x, y, soursebmp);
        }

        public Region(int x, int y, int r, int g, int b, Bitmap soursebmp)
        {
            pixels = new List<KeyValuePair<int, int>>();
            R = r; G = g; B = b;
            AddPointColor(x, y, soursebmp);
        }

        public void AddPointColor(int x, int y,  Bitmap soursebmp)
        {
            KeyValuePair<int, int> NewPair = new KeyValuePair<int, int>(x, y);
            pixels.Add(NewPair);
            int r = (int)soursebmp.GetPixel(x, y).R;
            int g = (int)soursebmp.GetPixel(x, y).G;
            int b = (int)soursebmp.GetPixel(x, y).B;
            if (CheckColor(r, g, b))
            {
                CalculateRGB(soursebmp);
            }
        }

        public void AddPointBrightness(int x, int y, Bitmap soursebmp)
        {
            KeyValuePair<int, int> NewPair = new KeyValuePair<int, int>(x, y);
            pixels.Add(NewPair);

            if (Math.Abs(brightness - soursebmp.GetPixel(x, y).GetBrightness()) > 0.01f)
            {
                CalculateBrightness(soursebmp);
            }
        }

        public void CalculateBrightness(Bitmap soursebmp)
        {
            float sumBrightness = 0;
            for (int i = 0; i < pixels.Count(); i++)
            {
                sumBrightness += soursebmp.GetPixel(pixels[i].Key, pixels[i].Value).GetBrightness();
            }
            brightness = sumBrightness / pixels.Count();
        }

        public void CalculateRGB(Bitmap soursebmp)
        {
            int sumR = 0;
            int sumG = 0;
            int sumB = 0;
            for (int i = 0; i < pixels.Count(); i++)
            {
                sumR += (int)soursebmp.GetPixel(pixels[i].Key, pixels[i].Value).R;
                sumG += (int)soursebmp.GetPixel(pixels[i].Key, pixels[i].Value).G;
                sumB += (int)soursebmp.GetPixel(pixels[i].Key, pixels[i].Value).B;
            }
            R = sumR / pixels.Count();
            G = sumG / pixels.Count();
            B = sumB / pixels.Count();
        }

        public bool findPoint(int x, int y)
        {
            for (int i = 0; i < pixels.Count(); i++)
            {
                if (pixels[i].Key == x && pixels[i].Value == y) return true;
            }
            return false;
        }

        public Bitmap HighlightBorders(Bitmap soursebmp)
        {
            for (int i = 0; i < pixels.Count(); i++)
            {
                if (!(findPoint(pixels[i].Key + 1, pixels[i].Value) && findPoint(pixels[i].Key, pixels[i].Value + 1)
                    && findPoint(pixels[i].Key - 1, pixels[i].Value) && findPoint(pixels[i].Key, pixels[i].Value - 1)))
                {
                    soursebmp.SetPixel(pixels[i].Key, pixels[i].Value, Color.Red);
                }
            }

            return soursebmp;
        }

        public Bitmap HighlightRegion(Bitmap soursebmp, Color rndcolor)
        {
            for (int i = 0; i < pixels.Count(); i++)
            {
                soursebmp.SetPixel(pixels[i].Key, pixels[i].Value, rndcolor);
            }


            color = rndcolor;
            return soursebmp;
        }

        public double CalculateMoment(Bitmap soursebmp)
        {
            double result = 0.0f;
            int p = 2;
            int q = 2;

            for (int i = 0; i < pixels.Count; i++)
            {
                result += Math.Pow(pixels[i].Key, p) * Math.Pow(pixels[i].Value, q) * soursebmp.GetPixel(pixels[i].Key, pixels[i].Value).GetBrightness();
            }
            return result;
        }

        public bool CheckColor(int r, int g, int b)
        {
            return ((Math.Abs(R - r) > 3) && (Math.Abs(G - g) > 3) && (Math.Abs(B - b) > 3));
        }
    }
}
