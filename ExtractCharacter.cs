using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace nhandangkitu
{
    public class ExtractCharacter
    {
        public  Bitmap bm, bm1;
        public  int k = 0, w, h;
        public  int[,] free;
        public  int[] vung;
        public  int[] Diemcutxy;
        public  int[] NuaTren;
        public  int[] NuaDuoi;
        public  int sovung = 0;
        public ExtractCharacter(Bitmap bmp)
        {
                    bm = new Bitmap(bmp);
                    bm1 = bm;
                    w = bm.Width;
                    h = bm.Height;
        }

        public  int[,] MatrixImage()
        {
            BitmapData bdata = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int nOffset = bdata.Stride - 3 * bm.Width;
            int temp;
            int[,] matran = new int[bm.Height, bm.Width];

            unsafe
            {
                byte* p = (byte*)bdata.Scan0;
                for (int x = 0; x < bm.Height; x++)
                {
                    for (int y = 0; y < bm.Width; y++)
                    {
                        temp = (byte)(p[0] * .33 + p[1] * .33 + p[2] * .33);
                        if (temp < 128) matran[x, y] = 0;//diem den chua dc xet
                        else matran[x, y] = 255;//diem trang
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            bm.UnlockBits(bdata);
            return matran;
        }

        public  void Duyet()
        {
            for (int i = 0; i < bm.Height; i++)
                for (int j = 0; j < bm.Width; j++)
                    if (free[i, j] == 0)//diem den  va chua dc xet
                    {
                        k++;
                        visit(i, j);
                    }
            vung = new int[k];
            for (int i = 0; i < k; i++)
            {
                vung[i] = i + 1;
            }
            Diemcutxy = new int[4 * k];
        }

        public  void visit(int i, int j)
        {
            if ((i < 0) || (i >= h) || (j < 0) || (j >= w)) return;
            if (free[i, j] > 0) return;
            free[i, j] = k;
            visit(i - 1, j);
            visit(i + 1, j);
            visit(i, j - 1);
            visit(i, j + 1);
        }
        
        public  void TestVung()
        {
            for (int l = 0; l < k; l++)
            {
                int xmin = 1000, ymin = 1000;
                int ymax = 0, xmax = 0;
                for (int x = 0; x < bm.Height; x++)
                {
                    for (int y = 0; y < bm.Width; y++)
                    {
                        if (free[x, y] == vung[l])
                        {
                            if (xmin > x) xmin = x;
                            if (xmax < x) xmax = x;
                            if (ymin > y) ymin = y;
                            if (ymax < y) ymax = y;
                        }
                    }
                }

                int lw = ymax - ymin + 1;
                int lh = xmax - xmin + 1;
                if ((lh > bm.Height / 2) || (lw > bm.Width / 4) || (lw < bm.Width / 16) || (lh < bm.Height / 4))
                {
                    for (int x = 0; x < bm.Height; x++)
                    {
                        for (int y = 0; y < bm.Width; y++)
                        {
                            if (free[x, y] == vung[l])
                                free[x, y] = 255;
                        }
                    }
                    vung[l] = 0;
                }
                else
                {
                    Diemcutxy[4 * sovung] = xmin;
                    Diemcutxy[4 * sovung + 1] = xmax;
                    Diemcutxy[4 * sovung + 2] = ymin;
                    Diemcutxy[4 * sovung + 3] = ymax;
                    sovung++;
                }

            }

            int i1 = 0, i2 = 0;
            NuaTren = new int[5 * sovung];
            NuaDuoi = new int[5 * sovung];
            for (int i = 0; i < sovung; i++)
            {
                if (Diemcutxy[4 * i] < h/3 )
                {
                    NuaTren[4 * i1] = Diemcutxy[4 * i];
                    NuaTren[4 * i1 + 1] = Diemcutxy[4 * i + 1];
                    NuaTren[4 * i1 + 2] = Diemcutxy[4 * i + 2];
                    NuaTren[4 * i1 + 3] = Diemcutxy[4 * i + 3];
                    i1++;
                }
                else
                {
                    NuaDuoi[4 * i2] = Diemcutxy[4 * i];
                    NuaDuoi[4 * i2 + 1] = Diemcutxy[4 * i + 1];
                    NuaDuoi[4 * i2 + 2] = Diemcutxy[4 * i + 2];
                    NuaDuoi[4 * i2 + 3] = Diemcutxy[4 * i + 3];
                    i2++;
                }
            }

            for (int i = 0; i < i1 - 1; i++)
            {
                for (int j = i + 1; j < i1; j++)
                {
                    if (NuaTren[4 * i + 2] > NuaTren[4 * j + 2])
                    {
                        int tg = NuaTren[4 * j]; NuaTren[4 * j] = NuaTren[4 * i]; NuaTren[4 * i] = tg;
                        tg = NuaTren[4 * j + 1]; NuaTren[4 * j + 1] = NuaTren[4 * i + 1]; NuaTren[4 * i + 1] = tg;
                        tg = NuaTren[4 * j + 2]; NuaTren[4 * j + 2] = NuaTren[4 * i + 2]; NuaTren[4 * i + 2] = tg;
                        tg = NuaTren[4 * j + 3]; NuaTren[4 * j + 3] = NuaTren[4 * i + 3]; NuaTren[4 * i + 3] = tg;
                    }
                }
            }

            for (int i = 0; i < i2 - 1; i++)
            {
                for (int j = i + 1; j < i2; j++)
                {
                    if (NuaDuoi[4 * i + 3] > NuaDuoi[4 * j + 3])
                    {
                        int tg = NuaDuoi[4 * j]; NuaDuoi[4 * j] = NuaDuoi[4 * i]; NuaDuoi[4 * i] = tg;
                        tg = NuaDuoi[4 * j + 1]; NuaDuoi[4 * j + 1] = NuaDuoi[4 * i + 1]; NuaDuoi[4 * i + 1] = tg;
                        tg = NuaDuoi[4 * j + 2]; NuaDuoi[4 * j + 2] = NuaDuoi[4 * i + 2]; NuaDuoi[4 * i + 2] = tg;
                        tg = NuaDuoi[4 * j + 3]; NuaDuoi[4 * j + 3] = NuaDuoi[4 * i + 3]; NuaDuoi[4 * i + 3] = tg;
                    }
                }
            }
        }

        public Bitmap CutAnh(int fromX, int toX, int fromY, int toY)
        {
            BitmapData bdata = bm1.LockBits(new Rectangle(0, 0, bm1.Width, bm1.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int nOffset = bdata.Stride - 3 * bm1.Width;
            int[,] mAnh = new int[bm1.Height, bm1.Width];
            int w_new = toY - fromY + 1;
            int h_new = toX - fromX + 1;
            int[,] mCut = new int[h_new, w_new];

            unsafe
            {
                byte* p = (byte*)bdata.Scan0;
                for (int x = 0; x < bm1.Height; x++)
                {
                    for (int y = 0; y < bm1.Width; y++)
                    {
                        mAnh[x, y] = p[0];
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            bm1.UnlockBits(bdata);

            for (int i = 0; i < h_new; i++)
                for (int j = 0; j < w_new; j++)
                {
                    mCut[i, j] = mAnh[fromX + i, fromY + j];
                }

            Bitmap bmCut = new Bitmap(w_new, h_new);
            BitmapData bmCutdata = bmCut.LockBits(new Rectangle(0, 0, w_new, h_new), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int nOffset1 = bmCutdata.Stride - 3 * w_new;
            unsafe
            {
                byte* p1 = (byte*)bmCutdata.Scan0;
                for (int i = 0; i < h_new; i++)
                {
                    for (int j = 0; j < w_new; j++)
                    {
                        p1[0] = p1[1] = p1[2] = (byte)mCut[i, j];
                        p1 += 3;
                    }
                    p1 += nOffset1;
                }
            }
            bmCut.UnlockBits(bmCutdata);
            return bmCut;
        }

      public void Execute()
      {
          free = MatrixImage();
          Duyet();
          TestVung();
      }
        public double GetAngle()
        {
            double alpha;
            alpha = Math.Atan(Math.Abs((double)(NuaTren[1] - NuaTren[13]) / (double)(NuaTren[3] - NuaTren[15])));

            if (NuaTren[3] < NuaTren[15])
            {
                return (-alpha);
            }
            else
            {
                return alpha;
            }
                       
        }
        public Bitmap CutImg1()
        {
            Bitmap a;
            a = CutAnh(NuaTren[0], NuaTren[1], NuaTren[2], NuaTren[3]);
            return a;
        }
        public Bitmap CutImg2()
        {
            Bitmap a;
            a = CutAnh( NuaTren[4], NuaTren[5], NuaTren[6], NuaTren[7]);
            return a;
        }
        public Bitmap CutImg3()
        {
            Bitmap a;
            a = CutAnh( NuaTren[8], NuaTren[9], NuaTren[10], NuaTren[11]);
            return a;
        }
        public Bitmap CutImg4()
        {
            Bitmap a;
            a = CutAnh( NuaTren[12], NuaTren[13], NuaTren[14], NuaTren[15]);
            return a;
        }
        public Bitmap CutImg5()
        {
            Bitmap a;
            a = CutAnh( NuaDuoi[0], NuaDuoi[1], NuaDuoi[2], NuaDuoi[3]);
            return a;
        }
        public Bitmap CutImg6()
        {
            Bitmap a;
            a = CutAnh(NuaDuoi[4], NuaDuoi[5], NuaDuoi[6], NuaDuoi[7]);
            return a;
        }
        public Bitmap CutImg7()
        {
            Bitmap a;
            a = CutAnh( NuaDuoi[8], NuaDuoi[9], NuaDuoi[10], NuaDuoi[11]);
            return a;
        }
        public Bitmap CutImg8()
        {
            Bitmap a;
            a = CutAnh(NuaDuoi[12], NuaDuoi[13], NuaDuoi[14], NuaDuoi[15]);
            return a;
        }

        
      
    }
}
