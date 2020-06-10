using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class HoughCircleTransformation
	{
		private int radiusToDetect;

		private short[,] houghMap;

		private short maxMapIntensity;

		private int width;

		private int height;

		private int localPeakRadius = 4;

		private short minCircleIntensity = 10;

		private ArrayList circles = new ArrayList();

		public short MinCircleIntensity
		{
			get
			{
				return minCircleIntensity;
			}
			set
			{
				minCircleIntensity = value;
			}
		}

		public int LocalPeakRadius
		{
			get
			{
				return localPeakRadius;
			}
			set
			{
				localPeakRadius = System.Math.Max(1, System.Math.Min(10, value));
			}
		}

		public short MaxIntensity => maxMapIntensity;

		public int CirclesCount => circles.Count;

		public HoughCircleTransformation(int radiusToDetect)
		{
			this.radiusToDetect = radiusToDetect;
		}

		public void ProcessImage(Bitmap image)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
			try
			{
				ProcessImage(new UnmanagedImage(bitmapData));
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public void ProcessImage(BitmapData imageData)
		{
			ProcessImage(new UnmanagedImage(imageData));
		}

		public unsafe void ProcessImage(UnmanagedImage image)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			width = image.Width;
			height = image.Height;
			int num = image.Stride - width;
			houghMap = new short[height, width];
			byte* ptr = (byte*)image.ImageData.ToPointer();
			for (int i = 0; i < height; i++)
			{
				int num2 = 0;
				while (num2 < width)
				{
					if (*ptr != 0)
					{
						DrawHoughCircle(num2, i);
					}
					num2++;
					ptr++;
				}
				ptr += num;
			}
			maxMapIntensity = 0;
			for (int j = 0; j < height; j++)
			{
				for (int k = 0; k < width; k++)
				{
					if (houghMap[j, k] > maxMapIntensity)
					{
						maxMapIntensity = houghMap[j, k];
					}
				}
			}
			CollectCircles();
		}

		public unsafe Bitmap ToBitmap()
		{
			if (houghMap == null)
			{
				throw new ApplicationException("Hough transformation was not done yet.");
			}
			int length = houghMap.GetLength(1);
			int length2 = houghMap.GetLength(0);
			Bitmap bitmap = Image.CreateGrayscaleImage(length, length2);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, length, length2), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
			int num = bitmapData.Stride - length;
			float num2 = 255f / (float)maxMapIntensity;
			byte* ptr = (byte*)bitmapData.Scan0.ToPointer();
			for (int i = 0; i < length2; i++)
			{
				int num3 = 0;
				while (num3 < length)
				{
					*ptr = (byte)System.Math.Min(255, (int)(num2 * (float)houghMap[i, num3]));
					num3++;
					ptr++;
				}
				ptr += num;
			}
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		public HoughCircle[] GetMostIntensiveCircles(int count)
		{
			int num = System.Math.Min(count, circles.Count);
			HoughCircle[] array = new HoughCircle[num];
			circles.CopyTo(0, array, 0, num);
			return array;
		}

		public HoughCircle[] GetCirclesByRelativeIntensity(double minRelativeIntensity)
		{
			int i = 0;
			for (int count = circles.Count; i < count && ((HoughCircle)circles[i]).RelativeIntensity >= minRelativeIntensity; i++)
			{
			}
			return GetMostIntensiveCircles(i);
		}

		private void CollectCircles()
		{
			circles.Clear();
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					short num = houghMap[i, j];
					if (num < minCircleIntensity)
					{
						continue;
					}
					bool flag = false;
					int k = i - localPeakRadius;
					for (int num2 = i + localPeakRadius; k < num2; k++)
					{
						if (k < 0)
						{
							continue;
						}
						if (flag || k >= height)
						{
							break;
						}
						int l = j - localPeakRadius;
						for (int num3 = j + localPeakRadius; l < num3; l++)
						{
							if (l >= 0)
							{
								if (l >= width)
								{
									break;
								}
								if (houghMap[k, l] > num)
								{
									flag = true;
									break;
								}
							}
						}
					}
					if (!flag)
					{
						circles.Add(new HoughCircle(j, i, radiusToDetect, num, (double)num / (double)maxMapIntensity));
					}
				}
			}
			circles.Sort();
		}

		private void DrawHoughCircle(int xCenter, int yCenter)
		{
			int num = 0;
			int num2 = radiusToDetect;
			int num3 = (5 - radiusToDetect * 4) / 4;
			SetHoughCirclePoints(xCenter, yCenter, num, num2);
			while (num < num2)
			{
				num++;
				if (num3 < 0)
				{
					num3 += 2 * num + 1;
				}
				else
				{
					num2--;
					num3 += 2 * (num - num2) + 1;
				}
				SetHoughCirclePoints(xCenter, yCenter, num, num2);
			}
		}

		private void SetHoughCirclePoints(int cx, int cy, int x, int y)
		{
			if (x == 0)
			{
				SetHoughPoint(cx, cy + y);
				SetHoughPoint(cx, cy - y);
				SetHoughPoint(cx + y, cy);
				SetHoughPoint(cx - y, cy);
			}
			else if (x == y)
			{
				SetHoughPoint(cx + x, cy + y);
				SetHoughPoint(cx - x, cy + y);
				SetHoughPoint(cx + x, cy - y);
				SetHoughPoint(cx - x, cy - y);
			}
			else if (x < y)
			{
				SetHoughPoint(cx + x, cy + y);
				SetHoughPoint(cx - x, cy + y);
				SetHoughPoint(cx + x, cy - y);
				SetHoughPoint(cx - x, cy - y);
				SetHoughPoint(cx + y, cy + x);
				SetHoughPoint(cx - y, cy + x);
				SetHoughPoint(cx + y, cy - x);
				SetHoughPoint(cx - y, cy - x);
			}
		}

		private void SetHoughPoint(int x, int y)
		{
			if (x >= 0 && y >= 0 && x < width && y < height)
			{
				houghMap[y, x]++;
			}
		}
	}
}
