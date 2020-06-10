using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class MoravecCornersDetector : ICornersDetector
	{
		private int windowSize = 3;

		private int threshold = 500;

		private static int[] xDelta = new int[8]
		{
			-1,
			0,
			1,
			1,
			1,
			0,
			-1,
			-1
		};

		private static int[] yDelta = new int[8]
		{
			-1,
			-1,
			-1,
			0,
			1,
			1,
			1,
			0
		};

		public int WindowSize
		{
			get
			{
				return windowSize;
			}
			set
			{
				if ((value & 1) == 0)
				{
					throw new ArgumentException("The value shoule be odd.");
				}
				windowSize = System.Math.Max(3, System.Math.Min(15, value));
			}
		}

		public int Threshold
		{
			get
			{
				return threshold;
			}
			set
			{
				threshold = value;
			}
		}

		public MoravecCornersDetector()
		{
		}

		public MoravecCornersDetector(int threshold)
			: this(threshold, 3)
		{
		}

		public MoravecCornersDetector(int threshold, int windowSize)
		{
			Threshold = threshold;
			WindowSize = windowSize;
		}

		public List<IntPoint> ProcessImage(Bitmap image)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed && image.PixelFormat != PixelFormat.Format24bppRgb && image.PixelFormat != PixelFormat.Format32bppRgb && image.PixelFormat != PixelFormat.Format32bppArgb)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			try
			{
				return ProcessImage(new UnmanagedImage(bitmapData));
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public List<IntPoint> ProcessImage(BitmapData imageData)
		{
			return ProcessImage(new UnmanagedImage(imageData));
		}

		public unsafe List<IntPoint> ProcessImage(UnmanagedImage image)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed && image.PixelFormat != PixelFormat.Format24bppRgb && image.PixelFormat != PixelFormat.Format32bppRgb && image.PixelFormat != PixelFormat.Format32bppArgb)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			int width = image.Width;
			int height = image.Height;
			int stride = image.Stride;
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int num2 = windowSize / 2;
			int num3 = stride - windowSize * num;
			int[,] array = new int[height, width];
			byte* ptr = (byte*)image.ImageData.ToPointer();
			int i = num2;
			for (int num4 = height - num2; i < num4; i++)
			{
				int j = num2;
				for (int num5 = width - num2; j < num5; j++)
				{
					int num6 = int.MaxValue;
					for (int k = 0; k < 8; k++)
					{
						int num7 = i + yDelta[k];
						int num8 = j + xDelta[k];
						if (num7 < num2 || num7 >= num4 || num8 < num2 || num8 >= num5)
						{
							continue;
						}
						int num9 = 0;
						byte* ptr2 = ptr + (long)(i - num2) * (long)stride + (long)(j - num2) * (long)num;
						byte* ptr3 = ptr + (long)(num7 - num2) * (long)stride + (long)(num8 - num2) * (long)num;
						for (int l = 0; l < windowSize; l++)
						{
							int num10 = 0;
							int num11 = windowSize * num;
							while (num10 < num11)
							{
								int num12 = *ptr2 - *ptr3;
								num9 += num12 * num12;
								num10++;
								ptr2++;
								ptr3++;
							}
							ptr2 += num3;
							ptr3 += num3;
						}
						if (num9 < num6)
						{
							num6 = num9;
						}
					}
					if (num6 < threshold)
					{
						num6 = 0;
					}
					array[i, j] = num6;
				}
			}
			List<IntPoint> list = new List<IntPoint>();
			int m = num2;
			for (int num13 = height - num2; m < num13; m++)
			{
				int n = num2;
				for (int num14 = width - num2; n < num14; n++)
				{
					int num15 = array[m, n];
					int num16 = -num2;
					while (num15 != 0 && num16 <= num2)
					{
						for (int num17 = -num2; num17 <= num2; num17++)
						{
							if (array[m + num16, n + num17] > num15)
							{
								num15 = 0;
								break;
							}
						}
						num16++;
					}
					if (num15 != 0)
					{
						list.Add(new IntPoint(n, m));
					}
				}
			}
			return list;
		}
	}
}
