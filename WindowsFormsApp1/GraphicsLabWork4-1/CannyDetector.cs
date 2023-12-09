using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsLabWork4_1
{
    internal class CannyDetector
    {
        public CannyDetector()
        {
        }

        private bool MaxValue;
        public bool MaxPrecision
        {
            get { return MaxValue; }
            set { MaxValue = value; }
        }

        private double upperTreshold, lowerTreshold;
        public double LowerTreshold
        {
            get { return lowerTreshold; }
            set { lowerTreshold = value; }
        }
        public double UpperTreshold
        {
            get { return upperTreshold; }
            set { upperTreshold = value; }
        }
        private double[,] xExcerpt;
        private double[,] yExcerpt;
        private double[,] valueGradient;
        private double[,] directionGradient;
        private double[,] xMatrix = { { 1, 0, -1 }, { 2, 0, -2 }, { 1, 0, -1 } };
        private double[,] yMatrix = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
        private double[,] gaussMatrix = {
                                            {0.0121, 0.0261, 0.0337, 0.0261, 0.0121},
                                            {0.0261, 0.0561, 0.0724, 0.0561, 0.0261},
                                            {0.0337, 0.0724, 0.0935, 0.0724, 0.0337},
                                            {0.0261, 0.0561, 0.0724, 0.0561, 0.0261},
                                            {0.0121, 0.0261, 0.0337, 0.0261, 0.0121}
                                        };

        private double[,] Convolution(double[,] picture, double[,] kernel, int numeral)
        {
            if (picture == null) return null;

            int yPoz = picture.GetLength(1);
            int xPoz = picture.GetLength(0);

            double[,] pictureOut = new double[xPoz, yPoz];


            Parallel.For(0, xPoz, i => {
                for (int j = 0; j < yPoz; j++)
                {
                    double novelty = 0;
                    for (int innerI = i - numeral; innerI < i + numeral + 1; innerI++)
                        for (int innerJ = j - numeral; innerJ < j + numeral + 1; innerJ++)
                        {
                            int idxX = (innerI + xPoz) % xPoz;
                            int idxY = (innerJ + yPoz) % yPoz;

                            int kernx = innerI - (i - numeral);
                            int kerny = innerJ - (j - numeral);
                            novelty += picture[idxX, idxY] * kernel[kernx, kerny];
                        }

                    pictureOut[i, j] = novelty;
                }
            });


            return pictureOut;
        }

        public double[,] ExpandMatrix(double[,] matrix, int expansion)
        {
            if (matrix == null) return null;

            int x = matrix.GetLength(0), y = matrix.GetLength(1);
            double[,] res = new double[x + 2 * expansion, y + 2 * expansion];
            for (int i = -expansion; i < x + expansion - 1; i++)
                for (int j = -expansion; j < y + expansion - 1; j++)
                {
                    var ii = (i + x) % x;
                    int jj = (j + y) % y;
                    res[i + 2, j + 2] = matrix[ii, jj];
                }
            return res;
        }

        public double[,] Detection(double[,] normPicture, int accuracy, int width)
        {

            if (normPicture == null) return null;
            int blurx, blury;

            double[,] bluray;
            try
            {
                bluray = Convolution(normPicture, gaussMatrix, 2);

                blurx = bluray.GetLength(0); blury = bluray.GetLength(1);

                xExcerpt = Convolution(bluray, xMatrix, 1);
                yExcerpt = Convolution(bluray, yMatrix, 1);
            }
            catch (OutOfMemoryException)
            {
                throw;
            }

            int xExcerptrx = xExcerpt.GetLength(0), xExcerptry = xExcerpt.GetLength(1);
            valueGradient = new double[xExcerptrx, xExcerptry];
            directionGradient = new double[xExcerptrx, xExcerptry];

            for (int x = 0; x < blurx; x++)
            {
                for (int y = 0; y < blury; y++)
                {
                    valueGradient[x, y] = Math.Sqrt(xExcerpt[x, y] * xExcerpt[x, y] + yExcerpt[x, y] * yExcerpt[x, y]);
                    double pom = Math.Atan2(xExcerpt[x, y], yExcerpt[x, y]);
                    if ((pom >= -Math.PI / 8 && pom < Math.PI / 8) || (pom <= -7 * Math.PI / 8 && pom > 7 * Math.PI / 8))
                        directionGradient[x, y] = 0;
                    else if ((pom >= Math.PI / 8 && pom < 3 * Math.PI / 8) || (pom <= -5 * Math.PI / 8 && pom > -7 * Math.PI / 8))
                        directionGradient[x, y] = Math.PI / 4;
                    else if ((pom >= 3 * Math.PI / 8 && pom <= 5 * Math.PI / 8) || (-3 * Math.PI / 8 >= pom && pom > -5 * Math.PI / 8))
                        directionGradient[x, y] = Math.PI / 2;
                    else if ((pom < -Math.PI / 8 && pom >= -3 * Math.PI / 8) || (pom > 5 * Math.PI / 8 && pom <= 7 * Math.PI / 8))
                        directionGradient[x, y] = -Math.PI / 4;
                }
            }

            var max = this.max(valueGradient);
            for (int i = 0; i < xExcerptrx; i++)
            {
                for (int j = 0; j < xExcerptry; j++)
                {
                    valueGradient[i, j] /= max;
                }
            }

            if (upperTreshold == 0 && lowerTreshold == 0) DefineThresholds(blurx, blury);

            for (int i = 0; i < xExcerptrx; i++)
            {
                for (int j = 0; j < xExcerptry; j++)
                {
                    valueGradient[i, j] = valueGradient[i, j] < lowerTreshold ? 0 : valueGradient[i, j];
                }
            }

            for (var x = 1; x < blurx - 1; x++)
            {
                for (var y = 1; y < blury - 1; y++)
                {

                    if (directionGradient[x, y] == 0 && (valueGradient[x, y] <= valueGradient[x - 1, y] || valueGradient[x, y] <= valueGradient[x + 1, y]))

                        valueGradient[x, y] = 0;

                    else if (directionGradient[x, y] == Math.PI / 2 && (valueGradient[x, y] <= valueGradient[x, y - 1] || valueGradient[x, y + 1] >= valueGradient[x, y]))

                        valueGradient[x, y] = 0;

                    else if (directionGradient[x, y] == Math.PI / 4 && (valueGradient[x, y] <= valueGradient[x - 1, y + 1] || valueGradient[x, y] <= valueGradient[x + 1, y - 1]))

                        valueGradient[x, y] = 0;

                    else if (directionGradient[x, y] == -Math.PI / 4 && (valueGradient[x, y] <= valueGradient[x - 1, y - 1] || valueGradient[x, y] <= valueGradient[x + 1, y + 1]))

                        valueGradient[x, y] = 0;
                }
            }

            for (var x = 2; x < blurx - 2; x++)
            {
                for (var y = 2; y < blury - 2; y++)
                {
                    if (directionGradient[x, y] == 0)
                        if (valueGradient[x - 2, y] > valueGradient[x, y] || valueGradient[x + 2, y] > valueGradient[x, y])
                            valueGradient[x, y] = 0;
                    if (directionGradient[x, y] == Math.PI / 2)
                        if (valueGradient[x, y - 2] > valueGradient[x, y] || valueGradient[x, y + 2] > valueGradient[x, y])
                            valueGradient[x, y] = 0;
                    if (directionGradient[x, y] == Math.PI / 4)
                        if (valueGradient[x - 2, y + 2] > valueGradient[x, y] || valueGradient[x + 2, y - 2] > valueGradient[x, y])
                            valueGradient[x, y] = 0;
                    if (directionGradient[x, y] == -Math.PI / 4)
                        if (valueGradient[x + 2, y + 2] > valueGradient[x, y] || valueGradient[x - 2, y - 2] > valueGradient[x, y])
                            valueGradient[x, y] = 0;
                }
            }

            for (var x = 0; x < blurx; x++)
            {
                for (var y = 0; y < blury; y++)
                {
                    if (valueGradient[x, y] > upperTreshold)
                        valueGradient[x, y] = 1;
                }
            }

            int pomH = 0;
            int pomOLd = -1;
            int passage = 0;


            bool resume = true;
            while (resume)
            {
                passage = passage + 1;
                pomOLd = pomH;
                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < xExcerptry - 1; y++)
                    {
                        if (valueGradient[x, y] <= upperTreshold && valueGradient[x, y] >= lowerTreshold)
                        {
                            double pom1 = valueGradient[x - 1, y - 1];
                            double pom2 = valueGradient[x, y - 1];
                            double pom3 = valueGradient[x + 1, y - 1];
                            double pom4 = valueGradient[x - 1, y];
                            double pom5 = valueGradient[x + 1, y];
                            double pom6 = valueGradient[x - 1, y + 1];
                            double pom7 = valueGradient[x, y + 1];
                            double pom8 = valueGradient[x + 1, y + 1];

                            if (pom1 == 1 || pom2 == 1 || pom3 == 1 || pom4 == 1 || pom5 == 1 || pom6 == 1 || pom7 == 1 || pom8 == 1)
                            {
                                valueGradient[x, y] = 1;
                                pomH = pomH + 1;
                            }

                        }
                    }
                }

                if (MaxValue)
                {
                    resume = pomH != pomOLd;
                }
                else
                {
                    resume = passage <= accuracy;
                }
            }

            for (int i = 0; i < xExcerptrx; i++)
            {
                for (int j = 0; j < xExcerptry; j++)
                {
                    if (valueGradient[i, j] <= upperTreshold)
                        valueGradient[i, j] = 0;
                }
            }
            return valueGradient;
        }

        private void DefineThresholds(int dimx, int dimy)
        {
            double sum = 0;
            double number = 0;

            for (var x = 1; x < dimx - 1; x++)
                for (var y = 1; y < dimy - 1; y++)
                {
                    if (valueGradient[x, y] != 0)
                    {
                        sum += valueGradient[x, y];
                        number++;
                    }
                }
            upperTreshold = sum / number;
            lowerTreshold = 0.4 * upperTreshold;

        }
        private double max(double[,] mat)
        {
            double m = -1;

            foreach (var el in mat)
            {
                m = el > m ? el : m;
            }

            return m;
        }

        internal void CleanUp()
        {
            xExcerpt = null;
            yExcerpt = null;
            valueGradient = null;
            directionGradient = null;
            GC.Collect();
        }
    }
}
