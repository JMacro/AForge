using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class PointedColorFloodFill : BaseInPlacePartialFilter
	{
		private bool[,] checkedPixels;

		private unsafe byte* scan0;

		private int stride;

		private int startX;

		private int stopX;

		private int startY;

		private int stopY;

		private byte minR;

		private byte maxR;

		private byte minG;

		private byte maxG;

		private byte minB;

		private byte maxB;

		private byte fillR;

		private byte fillG;

		private byte fillB;

		private IntPoint startingPoint = new IntPoint(0, 0);

		private Color tolerance = Color.FromArgb(0, 0, 0);

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public Color Tolerance
		{
			get
			{
				return tolerance;
			}
			set
			{
				tolerance = value;
			}
		}

		public Color FillColor
		{
			get
			{
				return Color.FromArgb(fillR, fillG, fillB);
			}
			set
			{
				fillR = value.R;
				fillG = value.G;
				fillB = value.B;
			}
		}

		public IntPoint StartingPoint
		{
			get
			{
				return startingPoint;
			}
			set
			{
				startingPoint = value;
			}
		}

		public PointedColorFloodFill()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		public PointedColorFloodFill(Color fillColor)
			: this()
		{
			FillColor = fillColor;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			if (rect.Contains(startingPoint.X, startingPoint.Y))
			{
				startX = rect.Left;
				startY = rect.Top;
				stopX = rect.Right - 1;
				stopY = rect.Bottom - 1;
				scan0 = (byte*)image.ImageData.ToPointer();
				stride = image.Stride;
				checkedPixels = new bool[image.Height, image.Width];
				if (image.PixelFormat == PixelFormat.Format8bppIndexed)
				{
					byte b = *CoordsToPointerGray(startingPoint.X, startingPoint.Y);
					minG = (byte)System.Math.Max(0, b - tolerance.G);
					maxG = (byte)System.Math.Min(255, b + tolerance.G);
					LinearFloodFill4Gray(startingPoint);
					return;
				}
				byte* ptr = CoordsToPointerRGB(startingPoint.X, startingPoint.Y);
				minR = (byte)System.Math.Max(0, ptr[2] - tolerance.R);
				maxR = (byte)System.Math.Min(255, ptr[2] + tolerance.R);
				minG = (byte)System.Math.Max(0, ptr[1] - tolerance.G);
				maxG = (byte)System.Math.Min(255, ptr[1] + tolerance.G);
				minB = (byte)System.Math.Max(0, *ptr - tolerance.B);
				maxB = (byte)System.Math.Min(255, *ptr + tolerance.B);
				LinearFloodFill4RGB(startingPoint);
			}
		}

		private unsafe void LinearFloodFill4Gray(IntPoint startingPoint)
		{
			Queue<IntPoint> queue = new Queue<IntPoint>();
			queue.Enqueue(startingPoint);
			while (queue.Count > 0)
			{
				IntPoint intPoint = queue.Dequeue();
				int x = intPoint.X;
				int y = intPoint.Y;
				byte* ptr = CoordsToPointerGray(x, y);
				int num = x;
				byte* ptr2 = ptr;
				do
				{
					*ptr2 = fillG;
					checkedPixels[y, num] = true;
					num--;
					ptr2--;
				}
				while (num >= startX && !checkedPixels[y, num] && CheckGrayPixel(*ptr2));
				num++;
				int num2 = x;
				ptr2 = ptr;
				do
				{
					*ptr2 = fillG;
					checkedPixels[y, num2] = true;
					num2++;
					ptr2++;
				}
				while (num2 <= stopX && !checkedPixels[y, num2] && CheckGrayPixel(*ptr2));
				num2--;
				ptr2 = CoordsToPointerGray(num, y);
				bool flag = false;
				bool flag2 = false;
				int y2 = y - 1;
				int y3 = y + 1;
				int num3 = num;
				while (num3 <= num2)
				{
					if (y > startY && !checkedPixels[y - 1, num3] && CheckGrayPixel(*(ptr2 - stride)))
					{
						if (!flag)
						{
							queue.Enqueue(new IntPoint(num3, y2));
							flag = true;
						}
					}
					else
					{
						flag = false;
					}
					if (y < stopY && !checkedPixels[y + 1, num3] && CheckGrayPixel(ptr2[stride]))
					{
						if (!flag2)
						{
							queue.Enqueue(new IntPoint(num3, y3));
							flag2 = true;
						}
					}
					else
					{
						flag2 = false;
					}
					num3++;
					ptr2++;
				}
			}
		}

		private unsafe void LinearFloodFill4RGB(IntPoint startPoint)
		{
			Queue<IntPoint> queue = new Queue<IntPoint>();
			queue.Enqueue(startingPoint);
			while (queue.Count > 0)
			{
				IntPoint intPoint = queue.Dequeue();
				int x = intPoint.X;
				int y = intPoint.Y;
				byte* ptr = CoordsToPointerRGB(x, y);
				int num = x;
				byte* ptr2 = ptr;
				do
				{
					ptr2[2] = fillR;
					ptr2[1] = fillG;
					*ptr2 = fillB;
					checkedPixels[y, num] = true;
					num--;
					ptr2 -= 3;
				}
				while (num >= startX && !checkedPixels[y, num] && CheckRGBPixel(ptr2));
				num++;
				int num2 = x;
				ptr2 = ptr;
				do
				{
					ptr2[2] = fillR;
					ptr2[1] = fillG;
					*ptr2 = fillB;
					checkedPixels[y, num2] = true;
					num2++;
					ptr2 += 3;
				}
				while (num2 <= stopX && !checkedPixels[y, num2] && CheckRGBPixel(ptr2));
				num2--;
				ptr2 = CoordsToPointerRGB(num, y);
				bool flag = false;
				bool flag2 = false;
				int num3 = y - 1;
				int num4 = y + 1;
				int num5 = num;
				while (num5 <= num2)
				{
					if (y > startY && !checkedPixels[num3, num5] && CheckRGBPixel(ptr2 - stride))
					{
						if (!flag)
						{
							queue.Enqueue(new IntPoint(num5, num3));
							flag = true;
						}
					}
					else
					{
						flag = false;
					}
					if (y < stopY && !checkedPixels[num4, num5] && CheckRGBPixel(ptr2 + stride))
					{
						if (!flag2)
						{
							queue.Enqueue(new IntPoint(num5, num4));
							flag2 = true;
						}
					}
					else
					{
						flag2 = false;
					}
					num5++;
					ptr2 += 3;
				}
			}
		}

		private bool CheckGrayPixel(byte pixel)
		{
			if (pixel >= minG)
			{
				return pixel <= maxG;
			}
			return false;
		}

		private unsafe bool CheckRGBPixel(byte* pixel)
		{
			if (pixel[2] >= minR && pixel[2] <= maxR && pixel[1] >= minG && pixel[1] <= maxG && *pixel >= minB)
			{
				return *pixel <= maxB;
			}
			return false;
		}

		private unsafe byte* CoordsToPointerGray(int x, int y)
		{
			return scan0 + (long)stride * (long)y + x;
		}

		private unsafe byte* CoordsToPointerRGB(int x, int y)
		{
			return scan0 + (long)stride * (long)y + (long)x * 3L;
		}
	}
}
