using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;


namespace nhandangkitu
{
    class catbienso
    {
        public Bitmap bm, bmBienSo;
        public int k, w, h, w1, h1;
        public int[,] free;
        public int[] vung;
        public int[] Diemcutxy;
        public int[] NuaTren;
        public int[] NuaDuoi;
        public int sovung = 0;

        public catbienso(Bitmap a)
        {
            bm = a;
            h = a.Height;
            w = a.Width;
        }
       
        public int[,] MatrixImage(Bitmap bm)
        {
            BitmapData bdata = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int nOffset = bdata.Stride - 3 * bm.Width;
            int[,] matran = new int[bm.Height, bm.Width];

            unsafe
            {
                byte* p = (byte*)bdata.Scan0;
                for (int x = 0; x < bm.Height; x++)
                {
                    for (int y = 0; y < bm.Width; y++)
                    {
                        matran[x, y] = (byte)(p[0] * .33 + p[1] * .33 + p[2] * .33);
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            bm.UnlockBits(bdata);
            return matran;
        }

        public Bitmap MatrixToImage(int[,] m)
        {
            int mw = m.GetLength(1);
            int mh = m.GetLength(0);

            Bitmap bmOut = new Bitmap(mw, mh);
            BitmapData bdata = bmOut.LockBits(new Rectangle(0, 0, bmOut.Width, bmOut.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int nOffset = bdata.Stride - 3 * bmOut.Width;

            unsafe
            {
                byte* p = (byte*)bdata.Scan0;
                for (int x = 0; x < bmOut.Height; x++)
                {
                    for (int y = 0; y < bmOut.Width; y++)
                    {
                        p[0] = p[1] = p[2] = (byte)m[x, y];
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            bmOut.UnlockBits(bdata);
            return bmOut;

        }

       
      


        public  Bitmap Histogram(Bitmap bm)
        {
            int[,] mAnh = MatrixImage(bm);
            int[] hHis = new int[256];

            for (int x = 0; x < bm.Height; x++)
                for (int y = 0; y < bm.Width; y++)
                    hHis[mAnh[x, y]]++;

            int max = hHis[0];
            for (int i = 1; i < 256; i++)
                if (max < hHis[i]) max = hHis[i];
            for (int i = 1; i < 256; i++)
                hHis[i] = hHis[i] * 100 / max;

            Bitmap bmHis = new Bitmap(256, 100);
            BitmapData bdata = bmHis.LockBits(new Rectangle(0, 0, bmHis.Width, bmHis.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int nOffset = bdata.Stride - 3 * bmHis.Width;

            unsafe
            {
                byte* p = (byte*)bdata.Scan0;
                for (int x = 0; x < 100; x++)
                {
                    for (int y = 0; y < 256; y++)
                    {
                        if (x < 100 - hHis[y])
                            p[0] = p[1] = p[2] = 255;
                        else
                            p[0] = p[1] = p[2] = 0;
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            bmHis.UnlockBits(bdata);
            return bmHis;
        }

     
        public  Bitmap VeHistogramY(Bitmap bm)
        {
            int[,] mAnh = MatrixImage(bm);
            int[] hHis = new int[h];
            int[] hHis1 = new int[h];
            hHis1[0] = hHis1[h - 1] = 0;
            int max = 0;

            for (int i = 0; i < h; i++)
            {
                hHis[i] = 0;
                for (int j = 0; j < w; j++)
                {
                    hHis[i] += mAnh[i, j];
                }
                //if (max < hHis[i]) max = hHis[i];
            }

            for (int i = 1; i < h - 1; i++)
            {
                hHis1[i] = hHis[i + 1] - hHis[i - 1];
                if (max < hHis1[i]) max = hHis1[i];
            }

            for (int i = 0; i < h; i++)
            {
                hHis1[i] = hHis1[i] * 100 / max;
                hHis[i] = hHis1[i];
            }
            int[,] mHis = new int[h, 100];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < 100; j++)
                {
                    if (j <= hHis[i])
                        mHis[i, j] = 0;
                    else
                        mHis[i, j] = 255;
                }

            Bitmap bmHis = new Bitmap(100, h);
            BitmapData bdata = bmHis.LockBits(new Rectangle(0, 0, bmHis.Width, bmHis.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int nOffset = bdata.Stride - 3 * bmHis.Width;

            unsafe
            {
                byte* p = (byte*)bdata.Scan0;
                for (int x = 0; x < bmHis.Height; x++)
                {
                    for (int y = 0; y < bmHis.Width; y++)
                    {
                        p[0] = p[1] = p[2] = (byte)mHis[x, y];
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            bmHis.UnlockBits(bdata);
            return bmHis;
        }

        public  Bitmap CutAnh(Bitmap bm, int fromX, int toX, int fromY, int toY)
        {
            int[,] mAnh = MatrixImage(bm);
            int w2 = toX - fromX + 1;
            int h2 = toY - fromY + 1;
            int[,] mCut = new int[h2, w2];

            for (int i = 0; i < h2; i++)
            {
                for (int j = 0; j < w2; j++)
                {
                    mCut[i, j] = mAnh[fromY + i, fromX + j];
                }
            }

            Bitmap bmCut = MatrixToImage(mCut);
            return bmCut;
        }

        public Bitmap CatNgang(Bitmap bm)
        {
            int[,] mAnh = MatrixImage(bm);
            int[] hHis = new int[h];
            int[] hHis1 = new int[h];
            hHis1[0] = hHis1[h - 1] = 0;
            int max = 0;

            for (int i = 0; i < h; i++)
            {
                hHis[i] = 0;
                for (int j = 0; j < w; j++)
                {
                    hHis[i] += mAnh[i, j];
                }
                //if (max < hHis[i]) max = hHis[i];
            }

            for (int i = 1; i < h - 1; i++)
            {
                hHis1[i] = hHis[i + 1] - hHis[i - 1];
                if (max < hHis1[i]) max = hHis1[i];
            }

            for (int i = 0; i < h; i++)
                hHis[i] = hHis1[i];

            int max1 = 0, max2 = 0;
            int imax1 = 0, imax2 = 0;
            for (int i = 0; i < h; i++)
                if (max1 < hHis[i])
                {
                    max1 = hHis[i];
                    imax1 = i;
                }

            int il = 0, ir = h;
            for (int i = 0; i <= imax1; i++)
                if ((hHis[i] < 0.7 * max1) && (i > il))
                    il = i;
            for (int i = imax1; i < h; i++)
                if ((hHis[i] < 0.7 * max1) && (i < ir))
                    ir = i;
            for (int i = il; i <= ir; i++)
                hHis[i] = 0;

            for (int i = 0; i < h; i++)
                if (max2 < hHis[i])
                {
                    max2 = hHis[i];
                    imax2 = i;
                }

            if (imax1 > imax2)
            {
                int tg = imax2; imax2 = imax1; imax1 = tg;
            }

            Bitmap bmOut = CutAnh(bm, 0, bm.Width - 1, imax1, imax2);
            return bmOut;

        }

        public Bitmap VeHistogramX(Bitmap bm)
        {
            Bitmap bmY = CatNgang(bm);
            int w = bmY.Width;
            int h = bmY.Height;
            int[,] mY = MatrixImage(bmY);
            int[] hHis = new int[w];
            int[] hHis1 = new int[w];
            hHis1[0] = hHis1[w - 1] = 0;
            int max = 0;

            for (int i = 0; i < w; i++)
            {
                hHis[i] = 0;
                for (int j = 0; j < h; j++)
                    hHis[i] += mY[j, i];
            }

            for (int i = 1; i < w - 1; i++)
            {
                hHis1[i] = hHis[i + 1] - hHis[i - 1];
                if (max < hHis1[i]) max = hHis1[i];
            }
            for (int i = 0; i < w; i++)
            {
                hHis1[i] = hHis1[i] * 100 / max;
                hHis[i] = hHis1[i];
            }

            int[,] mHis = new int[100, w];
            for (int j = 0; j < w; j++)
                for (int i = 0; i < 100; i++)
                {
                    if (i < 100 - hHis[j])
                        mHis[i, j] = 255;
                    else
                        mHis[i, j] = 0;
                }
            Bitmap bmOut = MatrixToImage(mHis);
            return bmOut;
        }

        public Bitmap CatDoc(Bitmap bm)
        {
            Bitmap bmY = CatNgang(bm);
            int w = bmY.Width;
            int h = bmY.Height;
            int[,] mY = MatrixImage(bmY);
            int[] hHis = new int[w];
            int[] hHis1 = new int[w];
            hHis1[0] = hHis1[w - 1] = 0;
            int max1 = 0, max2 = 0;
            int imax1 = 0, imax2 = 0;

            for (int i = 0; i < w; i++)
            {
                hHis[i] = 0;
                for (int j = 0; j < h; j++)
                    hHis[i] += mY[j, i];
            }

            for (int i = 1; i < w - 1; i++)
            {
                hHis1[i] = hHis[i + 1] - hHis[i - 1];
                if (max1 < hHis1[i])
                {
                    max1 = hHis1[i];
                    imax1 = i;
                }
            }
            for (int i = 0; i < w; i++)
                hHis[i] = hHis1[i];

            int ir = 0, il = w;
            for (int i = 0; i <= imax1; i++)
                if ((hHis[i] < 0.7 * imax1) && (i < il))
                    il = i;
            for (int i = imax1; i < w; i++)
                if ((hHis[i] < 0.7 * imax1) && (i > ir))
                    ir = i;
            for (int i = imax1 - 5; i <= imax1 + 5; i++)
                hHis[i] = 0;

            for (int i = 0; i < w; i++)
                if (max2 < hHis[i])
                {
                    max2 = hHis[i];
                    imax2 = i;
                }
            if (imax1 > imax2)
            {
                int tg = imax2; imax2 = imax1; imax1 = tg;
            }

            Bitmap bmOut = CutAnh(bmY, imax1, imax2, 0, bmY.Height - 1);
            return bmOut;

        }

        //private void button2_Click(object sender, EventArgs e)
        //{

        //    pictureBox2.Image = Laplacian(bm);
        //    pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
        //}

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    //Bitmap bmlap = Laplacian(bm);
        //    pictureBox3.Image = VeHistogramY(bm);
        //    pictureBox3.SizeMode = PictureBoxSizeMode.AutoSize;
        //}


        //private void button5_Click(object sender, EventArgs e)
        //{
        //    pictureBox2.Image = CatNgang(bm);
        //    pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
        //}

        //private void button4_Click_1(object sender, EventArgs e)
        //{
        //    pictureBox4.Image = VeHistogramX(bm);
        //    pictureBox4.SizeMode = PictureBoxSizeMode.AutoSize;
        //}

        //private void button6_Click(object sender, EventArgs e)
        //{
        //    bmBienSo = CatDoc(bm);
        //    pictureBox5.Image = bmBienSo;
        //    pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;

            


        //}
    }
}
