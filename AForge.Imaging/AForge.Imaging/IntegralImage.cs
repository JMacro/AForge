using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class IntegralImage
	{
		protected uint[,] integralImage;

		private int width;

		private int height;

		public int Width => width;

		public int Height => height;

		public uint[,] InternalData => integralImage;

		protected IntegralImage(int width, int height)
		{
			this.width = width;
			this.height = height;
			integralImage = new uint[height + 1, width + 1];
		}

		public static IntegralImage FromBitmap(Bitmap image)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				throw new UnsupportedImageFormatException("Source image can be graysclae (8 bpp indexed) image only.");
			}
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
			IntegralImage result = FromBitmap(bitmapData);
			image.UnlockBits(bitmapData);
			return result;
		}

		public static IntegralImage FromBitmap(BitmapData imageData)
		{
			return FromBitmap(new UnmanagedImage(imageData));
		}

		public unsafe static IntegralImage FromBitmap(UnmanagedImage image)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				throw new ArgumentException("Source image can be graysclae (8 bpp indexed) image only.");
			}
			int num = image.Width;
			int num2 = image.Height;
			int num3 = image.Stride - num;
			IntegralImage integralImage = new IntegralImage(num, num2);
			uint[,] array = integralImage.integralImage;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			for (int i = 1; i <= num2; i++)
			{
				uint num4 = 0u;
				int num5 = 1;
				while (num5 <= num)
				{
					num4 += *ptr;
					array[i, num5] = num4 + array[i - 1, num5];
					num5++;
					ptr++;
				}
				ptr += num3;
			}
			return integralImage;
		}

		public uint GetRectangleSum(int x1, int y1, int x2, int y2)
		{
			if (x2 < 0 || y2 < 0 || x1 >= width || y1 >= height)
			{
				return 0u;
			}
			if (x1 < 0)
			{
				x1 = 0;
			}
			if (y1 < 0)
			{
				y1 = 0;
			}
			x2++;
			y2++;
			if (x2 > width)
			{
				x2 = width;
			}
			if (y2 > height)
			{
				y2 = height;
			}
			return integralImage[y2, x2] + integralImage[y1, x1] - integralImage[y2, x1] - integralImage[y1, x2];
		}

		public int GetHaarXWavelet(int x, int y, int radius)
		{
			int y2 = y - radius;
			int y3 = y + radius - 1;
			uint rectangleSum = GetRectangleSum(x, y2, x + radius - 1, y3);
			uint rectangleSum2 = GetRectangleSum(x - radius, y2, x - 1, y3);
			return (int)(rectangleSum - rectangleSum2);
		}

		public int GetHaarYWavelet(int x, int y, int radius)
		{
			int x2 = x - radius;
			int x3 = x + radius - 1;
			float num = (float)(double)GetRectangleSum(x2, y, x3, y + radius - 1);
			float num2 = (float)(double)GetRectangleSum(x2, y - radius, x3, y - 1);
			return (int)(num - num2);
		}

		public uint GetRectangleSumUnsafe(int x1, int y1, int x2, int y2)
		{
			x2++;
			y2++;
			return integralImage[y2, x2] + integralImage[y1, x1] - integralImage[y2, x1] - integralImage[y1, x2];
		}

		public uint GetRectangleSum(int x, int y, int radius)
		{
			return GetRectangleSum(x - radius, y - radius, x + radius, y + radius);
		}

		public uint GetRectangleSumUnsafe(int x, int y, int radius)
		{
			return GetRectangleSumUnsafe(x - radius, y - radius, x + radius, y + radius);
		}

		public float GetRectangleMean(int x1, int y1, int x2, int y2)
		{
			if (x2 < 0 || y2 < 0 || x1 >= width || y1 >= height)
			{
				return 0f;
			}
			if (x1 < 0)
			{
				x1 = 0;
			}
			if (y1 < 0)
			{
				y1 = 0;
			}
			x2++;
			y2++;
			if (x2 > width)
			{
				x2 = width;
			}
			if (y2 > height)
			{
				y2 = height;
			}
			return (float)((double)(integralImage[y2, x2] + integralImage[y1, x1] - integralImage[y2, x1] - integralImage[y1, x2]) / (double)((x2 - x1) * (y2 - y1)));
		}

		public float GetRectangleMeanUnsafe(int x1, int y1, int x2, int y2)
		{
			x2++;
			y2++;
			return (float)((double)(integralImage[y2, x2] + integralImage[y1, x1] - integralImage[y2, x1] - integralImage[y1, x2]) / (double)((x2 - x1) * (y2 - y1)));
		}

		public float GetRectangleMean(int x, int y, int radius)
		{
			return GetRectangleMean(x - radius, y - radius, x + radius, y + radius);
		}

		public float GetRectangleMeanUnsafe(int x, int y, int radius)
		{
			return GetRectangleMeanUnsafe(x - radius, y - radius, x + radius, y + radius);
		}
	}
}
