using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace nhandangkitu
{
    public sealed class processImage
    {
        //public Bitmap iResize(Bitmap img)
        //{

        //    int resizedW = (int)(32);
        //    int resizedH = (int)(32);
        //    Bitmap bmp = new Bitmap(resizedW, resizedH);
        //    Graphics graphic = Graphics.FromImage((Bitmap)bmp);
        //    graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //    graphic.DrawImage(img, 0, 0, resizedW, resizedH);
        //    graphic.Dispose();
        //    return (Bitmap)bmp;
        //}

        public static Bitmap charResize(Bitmap bmp)
        {

            int resizedW = (int)((float)(32) / (float)(bmp.Height) * (float)(bmp.Width));
            int resizedH = 32;
            Bitmap bm = new Bitmap(resizedW, resizedH);
            Graphics graphic = Graphics.FromImage((Bitmap)bm);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.DrawImage(bmp, 0, 0, resizedW, resizedH);
            graphic.Dispose();

            bm = stdImg(bm);
            return bm;
        }
        public static Bitmap plateResize(Bitmap bmp)
        {
            int resizedH = (int)((float)(150) / (float)(bmp.Width) * (float)(bmp.Height));
            int resizedW = 150;
            Bitmap bm = new Bitmap(resizedW, resizedH);
            Graphics graphic = Graphics.FromImage((Bitmap)bm);
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.DrawImage(bmp, 0, 0, resizedW, resizedH);
            graphic.Dispose();
            
            //Bitmap image = new Bitmap(bm.Width, bm.Height);
            //BitmapData imageData = image.LockBits(new Rectangle(0, 0, bm.Width, bm.Height),
            //                         ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            //BitmapData bitmapData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height),
            //                         ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            // unsafe
            // {
            //     byte* imagePointer = (byte*)imageData.Scan0;
            //     byte* bitmapPointer = (byte*)bitmapData.Scan0;
                
            //     for (int i = 0; i < bm.Height; i++)
            //     {
            //         for (int j = 0; j < bm.Width; j++)
            //         {
            //             if (bitmapPointer[0] == 255)
            //                 imagePointer[0] = imagePointer[1] = imagePointer[2] = imagePointer[3] = 200;
            //             else
            //             {
            //                 imagePointer[0] = (byte)bitmapPointer[0];
            //                 imagePointer[1] = (byte)bitmapPointer[1];
            //                 imagePointer[2] = (byte)bitmapPointer[2];
            //                 imagePointer[3] = (byte)255;
            //             }
            //             bitmapPointer += 4;
            //             imagePointer += 4;
            //         }
            //         imagePointer += (imageData.Stride - (imageData.Width * 4));
            //         bitmapPointer += (bitmapData.Stride - (bitmapData.Width * 4));
            //     }
            // }
            // bm.UnlockBits(bitmapData);
            // image.UnlockBits(imageData);

            return bm;
        }
        public static Bitmap stdImg(Bitmap bm)
        {
            int i, j;
            Bitmap image = new Bitmap(16, 32);
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, 16, 32),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bitmapData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {

                byte* imagePointer = (byte*)imageData.Scan0;
                byte* bmpPointer = (byte*)bitmapData.Scan0;

                for (i = 0; i < 32; i++)
                {
                    for (j = 0; j < 16; j++)
                    {
                        if (j < bm.Width)
                        {
                            // write the logic implementation here
                            imagePointer[0] = (byte)bmpPointer[0];
                            imagePointer[1] = (byte)bmpPointer[1];
                            imagePointer[2] = (byte)bmpPointer[2];
                            imagePointer[3] = (byte)255;
                            bmpPointer += 4;
                        }
                        else
                        {
                            imagePointer[0] = 255;
                            imagePointer[1] = 255;
                            imagePointer[2] = 255;
                            imagePointer[3] = 255;

                        }
                        //4 bytes per pixel
                        imagePointer += 4;

                    }//end for j

                    //4 bytes per pixel
                    imagePointer += (imageData.Stride - (imageData.Width * 4));
                    bmpPointer += (bitmapData.Stride - (bm.Width * 4));
                }//end for i
            }//end unsafe
            image.UnlockBits(imageData);
            bm.UnlockBits(bitmapData);
            return image;
        }

        public static Bitmap Median_LockBits(Bitmap bm)
        {
            Byte[] mang = new Byte[9];
            Bitmap bitmap = new Bitmap(bm);
            Rectangle rec = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(rec, System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int w = bmpData.Stride;
            IntPtr ptr = bmpData.Scan0;
            int bytes = bmpData.Stride * bitmap.Height;
            Byte[] rgbValues = new Byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            for (int counter = w; counter < rgbValues.Length - w; counter += 4)
            {
                if ((counter % w != 0) && ((counter + 4) % w != 0))
                {
                    mang[0] = rgbValues[counter];
                    mang[1] = rgbValues[counter - 4];
                    mang[2] = rgbValues[counter + 4];
                    mang[3] = rgbValues[counter - w];
                    mang[4] = rgbValues[counter + w];
                    mang[5] = rgbValues[counter - w - 4];
                    mang[6] = rgbValues[counter - w + 4];
                    mang[7] = rgbValues[counter + w - 4];
                    mang[8] = rgbValues[counter + w + 4];

                    Array.Sort(mang);
                    rgbValues[counter] = mang[4];
                    rgbValues[counter + 1] = mang[4];
                    rgbValues[counter + 2] = mang[4];
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bitmap.UnlockBits(bmpData);
            return bitmap;

        }

        public static Bitmap AdjustContrast(Bitmap Image, float Value)
        {
            Value = (100.0f + Value) / 100.0f;
            Value *= Value;
            Bitmap NewBitmap = new Bitmap (Image);
            BitmapData data = NewBitmap.LockBits(
                new Rectangle(0, 0, NewBitmap.Width, NewBitmap.Height),
                ImageLockMode.ReadWrite,
                NewBitmap.PixelFormat);

            unsafe
            {
                for (int y = 0; y < NewBitmap.Height; ++y)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int columnOffset = 0;
                    for (int x = 0; x < NewBitmap.Width; ++x)
                    {
                        byte B = row[columnOffset];
                        byte G = row[columnOffset + 1];
                        byte R = row[columnOffset + 2];

                        float Red = R / 255.0f;
                        float Green = G / 255.0f;
                        float Blue = B / 255.0f;
                        Red = (((Red - 0.5f) * Value) + 0.5f) * 255.0f;
                        Green = (((Green - 0.5f) * Value) + 0.5f) * 255.0f;
                        Blue = (((Blue - 0.5f) * Value) + 0.5f) * 255.0f;

                        int iR = (int)Red;
                        iR = iR > 255 ? 255 : iR;
                        iR = iR < 0 ? 0 : iR;
                        int iG = (int)Green;
                        iG = iG > 255 ? 255 : iG;
                        iG = iG < 0 ? 0 : iG;
                        int iB = (int)Blue;
                        iB = iB > 255 ? 255 : iB;
                        iB = iB < 0 ? 0 : iB;

                        row[columnOffset] = (byte)iB;
                        row[columnOffset + 1] = (byte)iG;
                        row[columnOffset + 2] = (byte)iR;

                        columnOffset += 4;
                    }
                }
            }

            NewBitmap.UnlockBits(data);

            return NewBitmap;
        }


        public static Bitmap Binarization(Bitmap b, int nNguong)
        {
            byte nVal = 0;
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                int nOffset = stride - b.Width * 4;
                for (int y = 0; y < b.Height; y++)
                {
                    for (int x = 0; x < b.Width; x++)
                    {
                        nVal = (byte)p[1];
                        if (nVal < nNguong) { nVal = 0; }
                        else { nVal = 255; }
                        p[0] = p[1] = p[2] = (byte)nVal;
                        p[3] = 255;
                        p += 4;
                    }
                    p += nOffset;
                }
            }
            b.UnlockBits(bmData);
            return b;
        }
        public static int[] HaarFeature(Bitmap bmp)    //Ảnh tham số phải có kích thước: 2^n * 2^n (32*32)
        {
            int[] F = new int[16];
            Bitmap[] myStack = new Bitmap[64];
            int head = 0;
            int i = 0;
            F[i++] = computeSum(bmp);
            myStack[head++] = bmp;
            Bitmap[] bm = new Bitmap[4];
            while (myStack[0] != null)
            {
                bmp = (myStack[--head]); myStack[head] = null;
                if (bmp.Width > 4)
                {
                    int index = 0;
                    for (int w = 0; w < bmp.Width; w += bmp.Width / 2)
                    {
                        for (int h = 0; h < bmp.Height; h += bmp.Height / 2)
                        {
                            bm[index] = cut4Image(bmp, w, h);
                            myStack[head++] = bm[index++];
                        }
                    }

                    F[i++] = computeSum(bm[0]) + computeSum(bm[1]);
                    F[i++] = computeSum(bm[1]) + computeSum(bm[2]);
                    F[i++] = computeSum(bm[3]);

                }


            }

            return F;

        }
        public static int computeSum(Bitmap bmp)
        {
            int i, j;
            int sumPix = 0;

            BitmapData bitmapData1 = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* imagePointer1 = (byte*)bitmapData1.Scan0;

                for (i = 0; i < bitmapData1.Height; i++)
                {
                    for (j = 0; j < bitmapData1.Width; j++)
                    {
                        sumPix += convert01(imagePointer1[0]);
                        imagePointer1 += 4;

                    }

                    imagePointer1 += bitmapData1.Stride - (bitmapData1.Width * 4);
                }
            }
            bmp.UnlockBits(bitmapData1);
            return (sumPix);
        }

        public static Bitmap cut4Image(Bitmap image, int wStart, int hStart)
        {
            int i, j;
            Bitmap output = new Bitmap(image.Height / 2, image.Width / 2);
            BitmapData bitmapData1 = output.LockBits(new Rectangle(0, 0, output.Width, output.Height),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData imageData1 = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* bitmapPointer1 = (byte*)bitmapData1.Scan0;
                byte* imagePointer1 = (byte*)imageData1.Scan0;
                imagePointer1 += (hStart * image.Width * 4 + wStart * 4 + (imageData1.Stride - (imageData1.Width * 4)) * hStart);
                for (i = hStart; i < (hStart + bitmapData1.Height); i++)
                {
                    for (j = wStart; j < (wStart + bitmapData1.Width); j++)
                    {
                        bitmapPointer1[0] = imagePointer1[0];
                        bitmapPointer1[1] = imagePointer1[1];
                        bitmapPointer1[2] = imagePointer1[2];
                        bitmapPointer1[3] = imagePointer1[3];
                        //4 bytes per pixel
                        bitmapPointer1 += 4;
                        imagePointer1 += 4;
                    }
                    bitmapPointer1 += (bitmapData1.Stride - (bitmapData1.Width * 4));
                    //if (wStart == 0) 
                    imagePointer1 += output.Width * 4 + (imageData1.Stride - (imageData1.Width * 4));
                    //else imagePointer1 += (imageData1.Stride - (imageData1.Width * 4));
                }
            }
            output.UnlockBits(bitmapData1);
            image.UnlockBits(imageData1);
            return output;

        }

        public static Bitmap cut2Image(Bitmap image, int position)
        {
            Bitmap output = new Bitmap(image.Width, image.Height / 2);
            BitmapData bitmapData1 = output.LockBits(new Rectangle(0, 0, output.Width, output.Height),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData imageData1 = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* bitmapPointer1 = (byte*)bitmapData1.Scan0;
                byte* imagePointer1 = (byte*)imageData1.Scan0;
                imagePointer1 += (position * bitmapData1.Height * bitmapData1.Width * 4 + (bitmapData1.Stride - (bitmapData1.Width * 4)) * position * bitmapData1.Height);
                for (int i = 0; i < bitmapData1.Width; i++)
                {
                    for (int j = position * bitmapData1.Height; j < position * bitmapData1.Height + bitmapData1.Height; j++)
                    {
                        bitmapPointer1[0] = imagePointer1[0];
                        bitmapPointer1[1] = imagePointer1[1];
                        bitmapPointer1[2] = imagePointer1[2];
                        bitmapPointer1[3] = imagePointer1[3];
                        bitmapPointer1 += 4;
                        imagePointer1 += 4;
                    }
                    bitmapPointer1 += (bitmapData1.Stride - (bitmapData1.Width * 4));
                    imagePointer1 += (imageData1.Stride - (imageData1.Width * 4));
                }
            }
            output.UnlockBits(bitmapData1);
            image.UnlockBits(imageData1);
            return output;
        }

        public static int convert01(int pixel)
        {
            if (pixel == 255) return 0;
            else return 1;
        }

        public static Bitmap Displayplot(int[] F)
        {
            int i, j;
            Bitmap output = new Bitmap(F.Length, F.Length);
            BitmapData bitmapData1 = output.LockBits(new Rectangle(0, 0, F.Length, F.Length),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* imagePointer1 = (byte*)bitmapData1.Scan0;
                for (i = 0; i < bitmapData1.Height; i++)
                {
                    for (j = 0; j < bitmapData1.Width; j++)
                    {
                        if ((bitmapData1.Height - i) < ((F[j]) / 4))
                        {
                            imagePointer1[0] = 5;
                            imagePointer1[1] = 1;
                            imagePointer1[2] = 90;
                            imagePointer1[3] = 255;
                        }
                        else
                        {
                            imagePointer1[0] = 255;
                            imagePointer1[1] = 255;
                            imagePointer1[2] = 255;
                            imagePointer1[3] = 255;
                        }
                        imagePointer1 += 4;
                    }

                    imagePointer1 += (bitmapData1.Stride - (bitmapData1.Width * 4));
                }
            }
            output.UnlockBits(bitmapData1);
            return output;

        }

        public static void SaveImage(PictureBox picbox)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Bitmap Image|*.bmp";
            saveFileDialog1.Title = "Save an Image File";

            DialogResult gj = saveFileDialog1.ShowDialog();

            if (gj == DialogResult.OK && !saveFileDialog1.FileName.Equals(""))
            {
                string GetExtension = System.IO.Path.GetExtension(saveFileDialog1.FileName);
                picbox.Image.Save(saveFileDialog1.FileName);
            }
        }

        public static int[] ImgGetInput(Bitmap charImg)
        {
            charImg = charResize(charImg);
            charImg = Binarization(charImg, 107);
            charImg = Median_LockBits(charImg); charImg = Median_LockBits(charImg);
            Bitmap a;
            int[] F = new int[32];
            int[] f1, f2;
            a = cut2Image(charImg, 0);
            f1 = HaarFeature(a);
            a = cut2Image(charImg, 1);
            f2 = HaarFeature(a);

            for (int i = 0; i < 16; i++)
            {
                F[i] = f1[i];
            }
            for (int i = 16; i < 32; i++)
            {
                F[i] = f2[i - 16];
            }
            return F;
        }

        public static void SaveImg(PictureBox pictureBox1)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";

            DialogResult gj = saveFileDialog1.ShowDialog();
            if (gj == DialogResult.OK && !saveFileDialog1.FileName.Equals(""))
            {
                string GetExtension = System.IO.Path.GetExtension(saveFileDialog1.FileName);

               if (GetExtension.Equals(""))
                {
                   pictureBox1.Image.Save(saveFileDialog1.FileName);
                }
                else //Nếu người dùng có nhập cả đuôi file thì kiểm tra xem đó có phải là đuôi file ảnh
                {
                    switch (GetExtension)
                    {
                        case ".jpg":
                        case ".bmp":
                        case ".gif": pictureBox1.Image.Save(saveFileDialog1.FileName); break;
                        default: MessageBox.Show("Không phải định dạng ảnh"); break;
                    }
                }

            }
        }

    }
}
