using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class PointedMeanFloodFill : BaseInPlacePartialFilter
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

		private int meanR;

		private int meanG;

		private int meanB;

		private int pixelsCount;

		private IntPoint startingPoint = new IntPoint(0, 0);

		private Color tolerance = Color.FromArgb(16, 16, 16);

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

		public PointedMeanFloodFill()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			if (!rect.Contains(startingPoint.X, startingPoint.Y) || tolerance == Color.Black)
			{
				return;
			}
			startX = rect.Left;
			startY = rect.Top;
			stopX = rect.Right - 1;
			stopY = rect.Bottom - 1;
			scan0 = (byte*)image.ImageData.ToPointer();
			stride = image.Stride;
			checkedPixels = new bool[image.Height, image.Width];
			pixelsCount = (meanR = (meanG = (meanB = 0)));
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				byte b = *CoordsToPointerGray(startingPoint.X, startingPoint.Y);
				minG = (byte)System.Math.Max(0, b - tolerance.G);
				maxG = (byte)System.Math.Min(255, b + tolerance.G);
				LinearFloodFill4Gray(startingPoint.X, startingPoint.Y);
				meanG /= pixelsCount;
				byte b2 = (byte)meanG;
				byte* ptr = (byte*)image.ImageData.ToPointer();
				ptr += startY * stride + startX;
				int num = stride - rect.Width;
				for (int i = startY; i <= stopY; i++)
				{
					int num2 = startX;
					while (num2 <= stopX)
					{
						if (checkedPixels[i, num2])
						{
							*ptr = b2;
						}
						num2++;
						ptr++;
					}
					ptr += num;
				}
				return;
			}
			byte* ptr2 = CoordsToPointerRGB(startingPoint.X, startingPoint.Y);
			minR = (byte)System.Math.Max(0, ptr2[2] - tolerance.R);
			maxR = (byte)System.Math.Min(255, ptr2[2] + tolerance.R);
			minG = (byte)System.Math.Max(0, ptr2[1] - tolerance.G);
			maxG = (byte)System.Math.Min(255, ptr2[1] + tolerance.G);
			minB = (byte)System.Math.Max(0, *ptr2 - tolerance.B);
			maxB = (byte)System.Math.Min(255, *ptr2 + tolerance.B);
			LinearFloodFill4RGB(startingPoint.X, startingPoint.Y);
			meanR /= pixelsCount;
			meanG /= pixelsCount;
			meanB /= pixelsCount;
			byte b3 = (byte)meanR;
			byte b4 = (byte)meanG;
			byte b5 = (byte)meanB;
			byte* ptr3 = (byte*)image.ImageData.ToPointer();
			ptr3 += startY * stride + startX * 3;
			int num3 = stride - rect.Width * 3;
			for (int j = startY; j <= stopY; j++)
			{
				int num4 = startX;
				while (num4 <= stopX)
				{
					if (checkedPixels[j, num4])
					{
						ptr3[2] = b3;
						ptr3[1] = b4;
						*ptr3 = b5;
					}
					num4++;
					ptr3 += 3;
				}
				ptr3 += num3;
			}
		}

		private unsafe void LinearFloodFill4Gray(int x, int y)
		{
			byte* ptr = CoordsToPointerGray(x, y);
			int num = x;
			byte* ptr2 = ptr;
			do
			{
				meanG += *ptr2;
				pixelsCount++;
				checkedPixels[y, num] = true;
				num--;
				ptr2--;
			}
			while (num >= startX && !checkedPixels[y, num] && CheckGrayPixel(*ptr2));
			num++;
			int num2 = x + 1;
			ptr2 = ptr + 1;
			while (num2 <= stopX && !checkedPixels[y, num2] && CheckGrayPixel(*ptr2))
			{
				meanG += *ptr2;
				pixelsCount++;
				checkedPixels[y, num2] = true;
				num2++;
				ptr2++;
			}
			num2--;
			ptr2 = CoordsToPointerGray(num, y);
			int num3 = num;
			while (num3 <= num2)
			{
				if (y > startY && !checkedPixels[y - 1, num3] && CheckGrayPixel(*(ptr2 - stride)))
				{
					LinearFloodFill4Gray(num3, y - 1);
				}
				if (y < stopY && !checkedPixels[y + 1, num3] && CheckGrayPixel(ptr2[stride]))
				{
					LinearFloodFill4Gray(num3, y + 1);
				}
				num3++;
				ptr2++;
			}
		}

		private unsafe void LinearFloodFill4RGB(int x, int y)
		{
			byte* ptr = CoordsToPointerRGB(x, y);
			int num = x;
			byte* ptr2 = ptr;
			do
			{
				meanR += ptr2[2];
				meanG += ptr2[1];
				meanB += *ptr2;
				pixelsCount++;
				checkedPixels[y, num] = true;
				num--;
				ptr2 -= 3;
			}
			while (num >= startX && !checkedPixels[y, num] && CheckRGBPixel(ptr2));
			num++;
			int num2 = x + 1;
			ptr2 = ptr + 3;
			while (num2 <= stopX && !checkedPixels[y, num2] && CheckRGBPixel(ptr2))
			{
				meanR += ptr2[2];
				meanG += ptr2[1];
				meanB += *ptr2;
				pixelsCount++;
				checkedPixels[y, num2] = true;
				num2++;
				ptr2 += 3;
			}
			num2--;
			ptr2 = CoordsToPointerRGB(num, y);
			int num3 = num;
			while (num3 <= num2)
			{
				if (y > startY && !checkedPixels[y - 1, num3] && CheckRGBPixel(ptr2 - stride))
				{
					LinearFloodFill4RGB(num3, y - 1);
				}
				if (y < stopY && !checkedPixels[y + 1, num3] && CheckRGBPixel(ptr2 + stride))
				{
					LinearFloodFill4RGB(num3, y + 1);
				}
				num3++;
				ptr2 += 3;
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
