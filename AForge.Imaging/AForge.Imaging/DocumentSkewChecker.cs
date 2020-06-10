using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class DocumentSkewChecker
	{
		private int stepsPerDegree;

		private int houghHeight;

		private double thetaStep;

		private double maxSkewToDetect;

		private double[] sinMap;

		private double[] cosMap;

		private bool needToInitialize = true;

		private short[,] houghMap;

		private short maxMapIntensity;

		private int localPeakRadius = 4;

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
				needToInitialize = true;
			}
		}

		public double MaxSkewToDetect
		{
			get
			{
				return maxSkewToDetect;
			}
			set
			{
				maxSkewToDetect = System.Math.Max(0.0, System.Math.Min(45.0, value));
				needToInitialize = true;
			}
		}

		[Obsolete("The property is deprecated and setting it has not any effect. Use MaxSkewToDetect property instead.")]
		public double MinBeta
		{
			get
			{
				return 0.0 - maxSkewToDetect;
			}
			set
			{
			}
		}

		[Obsolete("The property is deprecated and setting it has not any effect. Use MaxSkewToDetect property instead.")]
		public double MaxBeta
		{
			get
			{
				return maxSkewToDetect;
			}
			set
			{
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

		public DocumentSkewChecker()
		{
			StepsPerDegree = 10;
			MaxSkewToDetect = 30.0;
		}

		public double GetSkewAngle(Bitmap image)
		{
			return GetSkewAngle(image, new Rectangle(0, 0, image.Width, image.Height));
		}

		public double GetSkewAngle(Bitmap image, Rectangle rect)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
			try
			{
				return GetSkewAngle(new UnmanagedImage(bitmapData), rect);
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public double GetSkewAngle(BitmapData imageData)
		{
			return GetSkewAngle(new UnmanagedImage(imageData), new Rectangle(0, 0, imageData.Width, imageData.Height));
		}

		public double GetSkewAngle(BitmapData imageData, Rectangle rect)
		{
			return GetSkewAngle(new UnmanagedImage(imageData), rect);
		}

		public double GetSkewAngle(UnmanagedImage image)
		{
			return GetSkewAngle(image, new Rectangle(0, 0, image.Width, image.Height));
		}

		public unsafe double GetSkewAngle(UnmanagedImage image, Rectangle rect)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			InitHoughMap();
			int width = image.Width;
			int height = image.Height;
			int num = width / 2;
			int num2 = height / 2;
			rect.Intersect(new Rectangle(0, 0, width, height));
			int num3 = -num + rect.Left;
			int num4 = -num2 + rect.Top;
			int num5 = width - num - (width - rect.Right);
			int num6 = height - num2 - (height - rect.Bottom) - 1;
			int num7 = image.Stride - rect.Width;
			int num8 = (int)System.Math.Sqrt(num * num + num2 * num2);
			int num9 = num8 * 2;
			houghMap = new short[houghHeight, num9];
			byte* ptr = (byte*)image.ImageData.ToPointer() + (long)rect.Top * (long)image.Stride + rect.Left;
			byte* ptr2 = ptr + image.Stride;
			for (int i = num4; i < num6; i++)
			{
				int num10 = num3;
				while (num10 < num5)
				{
					if (*ptr < 128 && *ptr2 >= 128)
					{
						for (int j = 0; j < houghHeight; j++)
						{
							int num11 = (int)(cosMap[j] * (double)num10 - sinMap[j] * (double)i) + num8;
							if (num11 >= 0 && num11 < num9)
							{
								houghMap[j, num11]++;
							}
						}
					}
					num10++;
					ptr++;
					ptr2++;
				}
				ptr += num7;
				ptr2 += num7;
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
			CollectLines((short)(width / 10));
			HoughLine[] mostIntensiveLines = GetMostIntensiveLines(5);
			double num12 = 0.0;
			double num13 = 0.0;
			HoughLine[] array = mostIntensiveLines;
			foreach (HoughLine houghLine in array)
			{
				if (houghLine.RelativeIntensity > 0.5)
				{
					num12 += houghLine.Theta * houghLine.RelativeIntensity;
					num13 += houghLine.RelativeIntensity;
				}
			}
			if (mostIntensiveLines.Length > 0)
			{
				num12 /= num13;
			}
			return num12 - 90.0;
		}

		private HoughLine[] GetMostIntensiveLines(int count)
		{
			int num = System.Math.Min(count, lines.Count);
			HoughLine[] array = new HoughLine[num];
			lines.CopyTo(0, array, 0, num);
			return array;
		}

		private void CollectLines(short minLineIntensity)
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
						if (k < 0)
						{
							continue;
						}
						if (k >= length || flag)
						{
							break;
						}
						int l = j - localPeakRadius;
						for (int num4 = j + localPeakRadius; l < num4; l++)
						{
							if (l >= 0)
							{
								if (l >= length2)
								{
									break;
								}
								if (houghMap[k, l] > num2)
								{
									flag = true;
									break;
								}
							}
						}
					}
					if (!flag)
					{
						lines.Add(new HoughLine(90.0 - maxSkewToDetect + (double)i / (double)stepsPerDegree, (short)(j - num), num2, (double)num2 / (double)maxMapIntensity));
					}
				}
			}
			lines.Sort();
		}

		private void InitHoughMap()
		{
			if (needToInitialize)
			{
				needToInitialize = false;
				houghHeight = (int)(2.0 * maxSkewToDetect * (double)stepsPerDegree);
				thetaStep = 2.0 * maxSkewToDetect * System.Math.PI / 180.0 / (double)houghHeight;
				sinMap = new double[houghHeight];
				cosMap = new double[houghHeight];
				double num = 90.0 - maxSkewToDetect;
				for (int i = 0; i < houghHeight; i++)
				{
					sinMap[i] = System.Math.Sin(num * System.Math.PI / 180.0 + (double)i * thetaStep);
					cosMap[i] = System.Math.Cos(num * System.Math.PI / 180.0 + (double)i * thetaStep);
				}
			}
		}
	}
}
