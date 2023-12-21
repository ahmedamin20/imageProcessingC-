using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using openCV;

namespace Histogram
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        IplImage image1;
        Bitmap bmp;

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.bmp|All|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                image1 = cvlib.CvLoadImage(openFileDialog1.FileName, cvlib.CV_LOAD_IMAGE_COLOR);
                CvSize size = new CvSize(pictureBox1.Width, pictureBox1.Height);
                IplImage resized_image = cvlib.CvCreateImage(size, image1.depth, image1.nChannels);
                cvlib.CvResize(ref image1, ref resized_image, cvlib.CV_INTER_LINEAR);
                pictureBox1.Image = (Image)resized_image;
            }
        }

        private void EqualizedImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpImg = (Bitmap)image1;
            Bitmap newImage = bmpImg;
            int width = bmpImg.Width;
            int hieght = bmpImg.Height;


            //******************* Calculate N(i) **************//

            int[] ni_Red = new int[256];
            int[] ni_Green = new int[256];
            int[] ni_Blue = new int[256];


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hieght; j++)
                {
                    Color pixelColor = bmpImg.GetPixel(i, j);

                    ni_Red[pixelColor.R]++;
                    ni_Green[pixelColor.G]++;
                    ni_Blue[pixelColor.B]++;
                }
            }

            //******************* Calculate P(Ni) **************//
            decimal[] prob_ni_Red = new decimal[256];
            decimal[] prob_ni_Green = new decimal[256];
            decimal[] prob_ni_Blue = new decimal[256];

            for (int i = 0; i < 256; i++)
            {
                prob_ni_Red[i] = (decimal)ni_Red[i] / (decimal)(width * hieght);
                prob_ni_Green[i] = (decimal)ni_Green[i] / (decimal)(width * hieght);
                prob_ni_Blue[i] = (decimal)ni_Blue[i] / (decimal)(width * hieght);
            }

            //******************* Calculate CDF **************//

            decimal[] cdf_Red = new decimal[256];
            decimal[] cdf_Green = new decimal[256];
            decimal[] cdf_Blue = new decimal[256];

            cdf_Red[0] = prob_ni_Red[0];
            cdf_Green[0] = prob_ni_Green[0];
            cdf_Blue[0] = prob_ni_Blue[0];

            for (int i = 1; i < 256; i++)
            {
                cdf_Red[i] = prob_ni_Red[i] + cdf_Red[i - 1];
                cdf_Green[i] = prob_ni_Green[i] + cdf_Green[i - 1];
                cdf_Blue[i] = prob_ni_Blue[i] + cdf_Blue[i - 1];
            }


            //******************* Calculate CDF(L-1) **************//


            int red, green, blue;
            int constant = 255;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hieght; j++)
                {
                    Color pixelColor = bmpImg.GetPixel(i, j);

                    red = (int)Math.Round(cdf_Red[pixelColor.R] * constant);
                    green = (int)Math.Round(cdf_Red[pixelColor.G] * constant);
                    blue = (int)Math.Round(cdf_Red[pixelColor.B] * constant);

                    Color newColor = Color.FromArgb(red, green, blue);
                    newImage.SetPixel(i, j, newColor);

                }
            }

            pictureBox2.Image = (Image)newImage;
        }
       
        /* avg convert*/
        private void GrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmp = (Bitmap)image1;
            int width = bmp.Width;
            int height = bmp.Height;
            Color p;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    p = bmp.GetPixel(x, y);
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;
                    int avg = (r + g + b) / 3;
                    bmp.SetPixel(x, y, Color.FromArgb(a, avg, avg, avg));

                    pictureBox2.Image = (Image)bmp;


                }

            }
        }
        
        /*red convert*/
        private void RedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmp = (Bitmap)image1;
            int width = bmp.Width;
            int height = bmp.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color p = bmp.GetPixel(x, y);

                    Color grayColor = Color.FromArgb(p.R, 0, 0);

                    bmp.SetPixel(x, y, grayColor);
                }
                pictureBox2.Image = (Image)bmp;
            }


        }
       
        /*green convert*/
        private void GreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmp = (Bitmap)image1;
            int width = bmp.Width;
            int height = bmp.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color p = bmp.GetPixel(x, y);
                    Color grayColor = Color.FromArgb(0, p.G, 0);

                    bmp.SetPixel(x, y, grayColor);
                }
                pictureBox2.Image = (Image)bmp;
            }
        }
        
        /*blue convert*/
        private void BlueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bmp = (Bitmap)image1;
            int width = bmp.Width;
            int height = bmp.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color p = bmp.GetPixel(x, y);
                    Color grayColor = Color.FromArgb(0, 0, p.B);

                    bmp.SetPixel(x, y, grayColor);
                }
                pictureBox2.Image = (Image)bmp;
            }
        }
        
        
        /*Histogram*/
        private void HistForOriginalPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.Series["Red"].Points.Clear();
            chart1.Series["Green"].Points.Clear();
            chart1.Series["Blue"].Points.Clear();

            Bitmap bmpImg = (Bitmap)pictureBox1.Image;
            int width = bmpImg.Width;
            int hieght = bmpImg.Height;

            int[] ni_Red = new int[256];
            int[] ni_Green = new int[256];
            int[] ni_Blue = new int[256];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hieght; j++)
                {
                    Color pixelColor = bmpImg.GetPixel(i, j);

                    ni_Red[pixelColor.R]++;
                    ni_Green[pixelColor.G]++;
                    ni_Blue[pixelColor.B]++;

                }
            }

            for (int i = 0; i < 256; i++)
            {
                chart1.Series["Red"].Points.AddY(ni_Red[i]);
                chart1.Series["Green"].Points.AddY(ni_Green[i]);
                chart1.Series["Blue"].Points.AddY(ni_Blue[i]);
            }
        }

        private void HistForModifiedPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart2.Series["Red"].Points.Clear();
            chart2.Series["Green"].Points.Clear();
            chart2.Series["Blue"].Points.Clear();

            Bitmap bmpImg = (Bitmap)pictureBox2.Image;
            int width = bmpImg.Width;
            int hieght = bmpImg.Height;

            int[] ni_Red = new int[256];
            int[] ni_Green = new int[256];
            int[] ni_Blue = new int[256];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hieght; j++)
                {
                    Color pixelColor = bmpImg.GetPixel(i, j);

                    ni_Red[pixelColor.R]++;
                    ni_Green[pixelColor.G]++;
                    ni_Blue[pixelColor.B]++;

                }
            }

            for (int i = 0; i < 256; i++)
            {
                chart2.Series["Red"].Points.AddY(ni_Red[i]);
                chart2.Series["Green"].Points.AddY(ni_Green[i]);
                chart2.Series["Blue"].Points.AddY(ni_Blue[i]);
            }
        }

        /*EqualizedHistogram*/
        private void EqualizedHistForModifiedPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart2.Series["Red"].Points.Clear();
            chart2.Series["Green"].Points.Clear();
            chart2.Series["Blue"].Points.Clear();

            Bitmap bmpImg = (Bitmap)pictureBox2.Image;
            int width = bmpImg.Width;
            int hieght = bmpImg.Height;

            int[] ni_Red = new int[256];
            int[] ni_Green = new int[256];
            int[] ni_Blue = new int[256];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hieght; j++)
                {
                    Color pixelColor = bmpImg.GetPixel(i, j);

                    ni_Red[pixelColor.R]++;
                    ni_Green[pixelColor.G]++;
                    ni_Blue[pixelColor.B]++;

                }
            }

            for (int i = 0; i < 256; i++)
            {
                chart2.Series["Red"].Points.AddY(ni_Red[i]);
                chart2.Series["Green"].Points.AddY(ni_Green[i]);
                chart2.Series["Blue"].Points.AddY(ni_Blue[i]);
            }
        }

        private void EqualizedHistForOriginalPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.Series["Red"].Points.Clear();
            chart1.Series["Green"].Points.Clear();
            chart1.Series["Blue"].Points.Clear();

            Bitmap bmpImg = (Bitmap)pictureBox1.Image;
            int width = bmpImg.Width;
            int hieght = bmpImg.Height;

            int[] ni_Red = new int[256];
            int[] ni_Green = new int[256];
            int[] ni_Blue = new int[256];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hieght; j++)
                {
                    Color pixelColor = bmpImg.GetPixel(i, j);

                    ni_Red[pixelColor.R]++;
                    ni_Green[pixelColor.G]++;
                    ni_Blue[pixelColor.B]++;

                }
            }

            for (int i = 0; i < 256; i++)
            {
                chart1.Series["Red"].Points.AddY(ni_Red[i]);
                chart1.Series["Green"].Points.AddY(ni_Green[i]);
                chart1.Series["Blue"].Points.AddY(ni_Blue[i]);
            }
        }

        /*max Filter*/
        private void MaxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpImg = (Bitmap)pictureBox1.Image;
            if (bmpImg != null)
            {
                // Apply max filter
                Bitmap maxFilteredImage = ApplyMaxFilter(bmpImg);

                // Display the result in pictureBox2
                pictureBox2.Image = (Image)maxFilteredImage.Clone();
            }
        }

        private Bitmap ApplyMaxFilter(Bitmap inputImage)
        {
            // Pad the input image with a 1-pixel border of zeros
            Bitmap paddedImage = new Bitmap(inputImage.Width + 2, inputImage.Height + 2);
            using (Graphics g = Graphics.FromImage(paddedImage))
            {
                g.DrawImage(inputImage, 2, 2);
            }

            // Create a copy of the input image
            Bitmap resultImage = new Bitmap(inputImage.Width, inputImage.Height);

            // Loop through each pixel within the padded image boundaries (excluding padding)
            for (int i = 2; i < paddedImage.Width - 2; i++)
            {
                for (int j = 2; j < paddedImage.Height - 2; j++)
                {
                    int max = 0;
                    for (int k = -2; k <= 2; k++)
                    {
                        for (int l = -2; l <= 2; l++)
                        {
                            Color pixelColor = paddedImage.GetPixel(i + k, j + l);
                            int value = pixelColor.R;
                            if (value > max)
                                max = value;
                        }
                    }
                    resultImage.SetPixel(i - 2, j - 2,Color.FromArgb(max , max , max)); // Adjust offset for original image coordinates
                    
                }
            }

            return resultImage;
        }
        

        /*min filter*/
        private void MinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpImg = (Bitmap)pictureBox1.Image;
            if (bmpImg != null)
            {
                Bitmap minFilteredImage = ApplyminFilter(bmpImg);

                // Display the result in pictureBox2
                pictureBox2.Image = (Image)minFilteredImage.Clone();
            }
        }
        private Bitmap ApplyminFilter(Bitmap inputImage)
        {
            Bitmap paddedImage = new Bitmap(inputImage.Width + 4, inputImage.Height + 4);
            using (Graphics g = Graphics.FromImage(paddedImage))
            {
                g.DrawImage(inputImage, 2, 2);
            }
            // ... padding, creating result image, looping through pixels ...
            // Create a copy of the input image
            Bitmap resultImage = new Bitmap(inputImage.Width, inputImage.Height);
            for (int i = 2; i < paddedImage.Width - 2; i++)
            {
                for (int j = 2; j < paddedImage.Height - 2; j++)
                {
                    int minR = 255; // Initialize to max red value for comparison
                    int minG = 255; // Initialize to max green value for comparison
                    int minB = 255; // Initialize to max blue value for comparison

                    // Loop through the 5x5 window around the current pixel
                    for (int k = -2; k <= 2; k++)
                    {
                        for (int l = -2; l <= 2; l++)
                        {
                            int neighbourX = i + k;
                            int neighbourY = j + l;

                            if (neighbourX >= 0 && neighbourX < paddedImage.Width &&
                                neighbourY >= 0 && neighbourY < paddedImage.Height)
                            {
                                Color neighbourPixelColor = paddedImage.GetPixel(neighbourX, neighbourY);

                                if (neighbourPixelColor.R < minR)
                                    minR = neighbourPixelColor.R;
                                if (neighbourPixelColor.G < minG)
                                    minG = neighbourPixelColor.G;
                                if (neighbourPixelColor.B < minB)
                                    minB = neighbourPixelColor.B;
                            }
                        }
                    }

                    resultImage.SetPixel(i - 2, j - 2, Color.FromArgb(minR, minG, minB));
                }
            }

            return resultImage;
        }
        
        /* midean  filter*/
        private void MideanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpImg = (Bitmap)pictureBox1.Image;
            if (bmpImg != null)
            {
                // Apply max filter
                Bitmap medianFilteredImage = ApplyMedianFilter(bmpImg);

                // Display the result in pictureBox2
                pictureBox2.Image = (Image)medianFilteredImage.Clone();
            }
        }
        private Bitmap ApplyMedianFilter(Bitmap inputImage)
        {
            // Pad the image with a 2-pixel border of zeros for the 5x5 window
            Bitmap paddedImage = new Bitmap(inputImage.Width + 4, inputImage.Height + 4);
            using (Graphics g = Graphics.FromImage(paddedImage))
            {
                g.DrawImage(inputImage, 2, 2);
            }

            // Create a copy of the input image
            Bitmap resultImage = new Bitmap(inputImage.Width, inputImage.Height);

            // Loop through each pixel within the padded image boundaries (excluding padding)
            for (int i = 2; i < paddedImage.Width - 2; i++)
            {
                for (int j = 2; j < paddedImage.Height - 2; j++)
                {
                    List<int> values = new List<int>();

                    // Loop through the 5x5 window around the current pixel
                    for (int k = -2; k <= 2; k++)
                    {
                        for (int l = -2; l <= 2; l++)
                        {
                            int neighbourX = i + k;
                            int neighbourY = j + l;

                            if (neighbourX >= 0 && neighbourX < paddedImage.Width &&
                                neighbourY >= 0 && neighbourY < paddedImage.Height)
                            {
                                Color pixelColor = paddedImage.GetPixel(neighbourX, neighbourY);
                                values.Add(pixelColor.R); // Extract and store red values (adjust for other channels)
                            }
                        }
                    }

                    // Sort the list of intensity values and find the median
                    values.Sort();
                    int median = values[values.Count / 2];

                    // Set the pixel in the result image to the median intensity
                    resultImage.SetPixel(i - 2, j - 2, Color.FromArgb(median, median, median));
                }
            }

            return resultImage;
        }
        
        
        /*avg filter*/
        private void AverageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpImg = (Bitmap)pictureBox1.Image;
            if (bmpImg != null)
            {
                // Apply max filter
                Bitmap averageFilteredImage = ApplyAverageFilter(bmpImg);

                // Display the result in pictureBox2
                pictureBox2.Image = (Image)averageFilteredImage.Clone();
            }
        }

        private Bitmap ApplyAverageFilter(Bitmap inputImage)
        {
            // Pad the image with a 2-pixel border of zeros for the 5x5 window
            Bitmap paddedImage = new Bitmap(inputImage.Width + 4, inputImage.Height + 4);
            using (Graphics g = Graphics.FromImage(paddedImage))
            {
                g.DrawImage(inputImage, 2, 2);
            }

            // Create a copy of the input image
            Bitmap resultImage = new Bitmap(inputImage.Width, inputImage.Height);

            // Loop through each pixel within the padded image boundaries (excluding padding)
            for (int i = 2; i < paddedImage.Width - 2; i++)
            {
                for (int j = 2; j < paddedImage.Height - 2; j++)
                {
                    int sum = 0;
                    int count = 0; // Track valid neighbor count for normalization

                    // Loop through the 5x5 window around the current pixel
                    for (int k = -2; k <= 2; k++)
                    {
                        for (int l = -2; l <= 2; l++)
                        {
                            int neighbourX = i + k;
                            int neighbourY = j + l;

                            if (neighbourX >= 0 && neighbourX < paddedImage.Width &&
                                neighbourY >= 0 && neighbourY < paddedImage.Height)
                            {
                                Color pixelColor = paddedImage.GetPixel(neighbourX, neighbourY);
                                sum += pixelColor.R; // Extract and accumulate red values (adjust for other channels)
                                count++; // Increase count for valid neighbors
                            }
                        }
                    }

                    // Calculate average intensity and set pixel in result image
                    int average = sum / count;
                    resultImage.SetPixel(i - 2, j - 2, Color.FromArgb(average, average, average));
                }
            }

            return resultImage;
        }

        /* gaussian filter*/
        private void GaussianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpImg = (Bitmap)pictureBox1.Image;
            if (bmpImg != null)
            {
                // Apply max filter
                Bitmap averageFilteredImage = ApplyGaussianFilter(bmpImg , 0.5);

                // Display the result in pictureBox2
                pictureBox2.Image = (Image)averageFilteredImage.Clone();
            }
        }
        private Bitmap ApplyGaussianFilter(Bitmap inputImage, double sigma)
        {
            int kernelSize = 3; // Adjust kernel size as needed
            int halfSize = kernelSize / 2;

            double[,] kernel = GenerateGaussianKernel(kernelSize, sigma);

            Bitmap resultImage = new Bitmap(inputImage.Width, inputImage.Height);

            for (int i = halfSize; i < inputImage.Width - halfSize; i++)
            {
                for (int j = halfSize; j < inputImage.Height - halfSize; j++)
                {
                    double sumR = 0, sumG = 0, sumB = 0;

                    for (int k = -halfSize; k <= halfSize; k++)
                    {
                        for (int l = -halfSize; l <= halfSize; l++)
                        {
                            Color pixelColor = inputImage.GetPixel(i + k, j + l);
                            double weight = kernel[k + halfSize, l + halfSize];

                            sumR += pixelColor.R * weight;
                            sumG += pixelColor.G * weight;
                            sumB += pixelColor.B * weight;
                        }
                    }

                    resultImage.SetPixel(i, j, Color.FromArgb((int)sumR, (int)sumG, (int)sumB));
                }
            }

            return resultImage;
        }

        private double[,] GenerateGaussianKernel(int size, double sigma)
        {
            double[,] kernel = new double[size, size];
            double sum = 0;

            int halfSize = size / 2;

            for (int x = -halfSize; x <= halfSize; x++)
            {
                for (int y = -halfSize; y <= halfSize; y++)
                {
                    kernel[x + halfSize, y + halfSize] = Math.Exp(-(x * x + y * y) / (2 * sigma * sigma));
                    sum += kernel[x + halfSize, y + halfSize];
                }
            }

            // Normalize the kernel
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    kernel[i, j] /= sum;
                }
            }

            return kernel;
        }

    }
}
