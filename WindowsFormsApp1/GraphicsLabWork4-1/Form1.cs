using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GraphicsLabWork4_1
{

    public partial class Form1 : Form
    {

        readonly ImageProcessor imageHandler = new ImageProcessor();
        readonly CannyDetector detector = new CannyDetector();
        public Form1()
        {
            InitializeComponent();
            double lower = 0.02, upper = 0.08;
            detector.LowerTreshold = lower;
            detector.UpperTreshold = upper;
        } 

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            if (imageHandler.CurrentBitmap != null) imageHandler.CurrentBitmap.Dispose();
            if (imageHandler.OriginalBitmap != null) imageHandler.OriginalBitmap.Dispose();

            imageHandler.CurrentBitmap = (Bitmap)pictureBox1.Image;
            imageHandler.OriginalBitmap = (Bitmap)pictureBox1.Image;

            imageHandler.SetGrayscale();

            try
            {
                int Width = pictureBox1.Image.Width;
                var n = imageHandler.GetNormalizedMatrix();
                var image = detector.Detection(n, 50, Width);

                imageHandler.DenormalizeCurrent(image);

                n = null;
                image = null;

                detector.CleanUp();
                GC.Collect();

                pictureBox2.Image = imageHandler.CurrentBitmap;
            }
            catch (OutOfMemoryException)
            {
                pictureBox2.Image = null; pictureBox2.Dispose();
                pictureBox1.Image = null; pictureBox1.Dispose();
                imageHandler.CleanUp();
                detector.CleanUp();

                MessageBox.Show("The image you choose is too big. Please choose a smaller image and try again.");
            }
            Cursor = Cursors.Default;
        }
    }
}
