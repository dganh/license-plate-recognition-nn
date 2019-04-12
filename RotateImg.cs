﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace nhandangkitu
{
    public sealed class RotateImg
    {
         
            public static Bitmap RotateImage(Image image, double alpha)
            {
                if (image == null)
                    throw new ArgumentNullException("image");
                const double pi2 = Math.PI / 2.0;

                double oldWidth = (double)image.Width;
                double oldHeight = (double)image.Height;

                double locked_theta = alpha;

                while (locked_theta < 0.0)
                    locked_theta += 2 * Math.PI;

                double newWidth, newHeight;
                int nWidth, nHeight; 
                               
                double adjacentTop, oppositeTop;
                double adjacentBottom, oppositeBottom;

                if ((locked_theta >= 0.0 && locked_theta < pi2) ||
                    (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2)))
                {
                    adjacentTop = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
                    oppositeTop = Math.Abs(Math.Sin(locked_theta)) * oldWidth;

                    adjacentBottom = Math.Abs(Math.Cos(locked_theta)) * oldHeight;
                    oppositeBottom = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
                }
                else
                {
                    adjacentTop = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
                    oppositeTop = Math.Abs(Math.Cos(locked_theta)) * oldHeight;

                    adjacentBottom = Math.Abs(Math.Sin(locked_theta)) * oldWidth;
                    oppositeBottom = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
                }

                newWidth = adjacentTop + oppositeBottom;
                newHeight = adjacentBottom + oppositeTop;

                nWidth = (int)Math.Ceiling(newWidth);
                nHeight = (int)Math.Ceiling(newHeight);

                Bitmap rotatedBmp = new Bitmap(nWidth, nHeight);

                using (Graphics g = Graphics.FromImage(rotatedBmp))
                {

                    Point[] points;

                    if (locked_theta >= 0.0 && locked_theta < pi2)
                    {
                        points = new Point[] { 
											 new Point( (int) oppositeBottom, 0 ), 
											 new Point( nWidth, (int) oppositeTop ),
											 new Point( 0, (int) adjacentBottom )
										 };

                    }
                    else if (locked_theta >= pi2 && locked_theta < Math.PI)
                    {
                        points = new Point[] { 
											 new Point( nWidth, (int) oppositeTop ),
											 new Point( (int) adjacentTop, nHeight ),
											 new Point( (int) oppositeBottom, 0 )						 
										 };
                    }
                    else if (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2))
                    {
                        points = new Point[] { 
											 new Point( (int) adjacentTop, nHeight ), 
											 new Point( 0, (int) adjacentBottom ),
											 new Point( nWidth, (int) oppositeTop )
										 };
                    }
                    else
                    {
                        points = new Point[] { 
											 new Point( 0, (int) adjacentBottom ), 
											 new Point( (int) oppositeBottom, 0 ),
											 new Point( (int) adjacentTop, nHeight )		
										 };
                    }

                    g.DrawImage(image, points);
                }

                return rotatedBmp;
            }
        
    }
}
