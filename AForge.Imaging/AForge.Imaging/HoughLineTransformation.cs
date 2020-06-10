using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class HoughLineTransformation
	{
		private int stepsPerDegree;

		private int houghHeight;

		private double thetaStep;

		private double[] sinMap;

		private double[] cosMap;

		private short[,] houghMap;

		private short maxMapIntensity;

		private int localPeakRadius = 4;

		private short minLineIntensity = 10;

		private ArrayList lines = new ArrayList();

		public int StepsPerDegree
		{
			get
			{
				return stepsPerDegree;
			}
			set
			{
				stepsPerDegree = System.Math.Max(1, System.Math.Min(10, value));
				houghHeight = 180 * stepsPerDegree;
				thetaStep = System.Math.PI / (double)houghHeight;
				sinMap = new double[houghHeight];
				cosMap = new double[houghHeight];
				for (int i = 0; i < houghHeight; i++)
				{
					sinMap[i] = System.Math.Sin((double)i * thetaStep);
					cosMap[i] = System.Math.Cos((double)i * thetaStep);
				}
			}
		}

		public short MinLineIntensity
		{
			get
			{
				return minLineIntensity;
			}
			set
			{
				minLineIntensity = value;
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

		public int LinesCount => lines.Count;

		public HoughLineTransformation()
		{
			StepsPerDegree = 1;
		}

		public void ProcessImage(Bitmap image)
		{
			ProcessImage(image, new Rectangle(0, 0, image.Width, image.Height));
		}

		public void ProcessImage(Bitmap image, Rectangle rect)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
			try
			{
				ProcessImage(new UnmanagedImage(bitmapData), rect);
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public void ProcessImage(BitmapData imageData)
		{
			ProcessImage(new UnmanagedImage(imageData), new Rectangle(0, 0, imageData.Width, imageData.Height));
		}

		public void ProcessImage(BitmapData imageData, Rectangle rect)
		{
			ProcessImage(new UnmanagedImage(imageData), rect);
		}

		public void ProcessImage(UnmanagedImage image)
		{
			ProcessImage(image, new Rectangle(0, 0, image.Width, image.Height));
		}

		public unsafe void ProcessImage(UnmanagedImage image, Rectangle rect)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			int width = image.Width;
			int height = image.Height;
			int num = width / 2;
			int num2 = height / 2;
			rect.Intersect(new Rectangle(0, 0, width, height));
			int num3 = -num + rect.Left;
			int num4 = -num2 + rect.Top;
			int num5 = width - num - (width - rect.Right);
			int num6 = height - num2 - (height - rect.Bottom);
			int num7 = image.Stride - rect.Width;
			int num8 = (int)System.Math.Sqrt(num * num + num2 * num2);
			int num9 = num8 * 2;
			houghMap = new short[houghHeight, num9];
			byte* ptr = (byte*)image.ImageData.ToPointer() + (long)rect.Top * (long)image.Stride + rect.Left;
			for (int i = num4; i < num6; i++)
			{
				int num10 = num3;
				while (num10 < num5)
				{
					if (*ptr != 0)
					{
						for (int j = 0; j < houghHeight; j++)
						{
							int num11 = (int)System.Math.Round(cosMap[j] * (double)num10 - sinMap[j] * (double)i) + num8;
							if (num11 >= 0 && num11 < num9)
							{
								houghMap[j, num11]++;
							}
						}
					}
					num10++;
					ptr++;
				}
				ptr += num7;
			}
			maxMapIntensity = 0;
			for (int k = 0; k < houghHeight; k++)
			{
				for (int l = 0; l < num9; l++)
				{
					if (houghMap[k, l] > maxMapIntensity)
					{
						maxMapIntensity = houghMap[k, l];
					}
				}
			}
			CollectLines();
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

		public HoughLine[] GetMostIntensiveLines(int count)
		{
			int num = System.Math.Min(count, lines.Count);
			HoughLine[] array = new HoughLine[num];
			lines.CopyTo(0, array, 0, num);
			return array;
		}

		public HoughLine[] GetLinesByRelativeIntensity(double minRelativeIntensity)
		{
			int i = 0;
			for (int count = lines.Count; i < count && ((HoughLine)lines[i]).RelativeIntensity >= minRelativeIntensity; i++)
			{
			}
			return GetMostIntensiveLines(i);
		}

		private void CollectLines()
		{
			int length = houghMap.GetLength(0);
			int length2 = houghMap.GetLength(1);
			int num = length2 >> 1;
			lines.Clear();
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					short num2 = houghMap[i, j];
					if (num2 < minLineIntensity)
					{
						continue;
					}
					bool flag = false;
					int k = i - localPeakRadius;
					for (int num3 = i + localPeakRadius; k < num3; k++)
					{
						if (flag)
						{
							break;
						}
						int num4 = k;
						int num5 = j;
						if (num4 < 0)
						{
							num4 = length + num4;
							num5 = length2 - num5;
						}
						if (num4 >= length)
						{
							num4 -= length;
							num5 = length2 - num5;
						}
						int l = num5 - localPeakRadius;
						for (int num6 = num5 + localPeakRadius; l < num6; l++)
						{
							if (l >= 0)
							{
								if (l >= length2)
								{
									break;
								}
								if (houghMap[num4, l] > num2)
								{
									flag = true;
									break;
								}
							}
						}
					}
					if (!flag)
					{
						lines.Add(new HoughLine((double)i / (double)stepsPerDegree, (short)(j - num), num2, (double)num2 / (double)maxMapIntensity));
					}
				}
			}
			lines.Sort();
		}
	}
}
