using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class MSE
    {
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public static float imageCompareMSE(Bitmap sourceImage1, Bitmap sourceImage2)
        {
            int imageWidth = sourceImage1.Width;
            int imageHeight = sourceImage1.Height;
            float res = 0;
            int totalPixelCount = imageHeight * imageWidth;
            int pixelIntensity1, pixelIntensity2;
            for (int i = 0; i < imageWidth; i++)
            {
                for (int j = 0; j < imageHeight; j++)
                {
                    Color pixelColor1 = sourceImage1.GetPixel(i, j);
                    Color pixelColor2 = sourceImage2.GetPixel(i, j);
                    pixelIntensity1 = Clamp((int)(0.36 * pixelColor1.R) + (int)(0.53 * pixelColor1.G) + (int)(0.11 * pixelColor1.B), 0, 255);
                    pixelIntensity2 = Clamp((int)(0.36 * pixelColor2.R) + (int)(0.53 * pixelColor2.G) + (int)(0.11 * pixelColor2.B), 0, 255);
                    res += (pixelIntensity1 - pixelIntensity2) * (pixelIntensity1 - pixelIntensity2);
                }
            }
            res = res / totalPixelCount;
            return res;
        }
    }
}
