using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static internal class MedianFilter
    {
        public static Bitmap Median(this Bitmap image)
        {
            Bitmap original = image;
            int width = original.Width;
            int height = original.Height;
            Bitmap edited = new Bitmap(width, height);
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    Color[] window = new Color[9];
                    int[] windowR = new int[9];
                    int[] windowG = new int[9];
                    int[] windowB = new int[9];
                    int count = 0;
                    for (int i = x - 1; i < x + 2; i++)
                    {
                        for (int j = y - 1; j < y + 2; j++)
                        {
                            window[count] = original.GetPixel(i, j);
                            windowR[count] = window[count].R;
                            windowG[count] = window[count].G;
                            windowB[count] = window[count].B;
                            count++;
                        }
                    }
                    Array.Sort(windowR);
                    Array.Sort(windowG);
                    Array.Sort(windowB);
                    int r = windowR[9 / 2];
                    int g = windowG[9 / 2];
                    int b = windowB[9 / 2];
                    Color color = Color.FromArgb(r, g, b);
                    edited.SetPixel(x, y, color);
                }
            }
            return edited;
        }
    }
}
