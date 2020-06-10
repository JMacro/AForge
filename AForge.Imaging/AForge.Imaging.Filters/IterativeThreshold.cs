using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class IterativeThreshold : Threshold
	{
		private int minError;

		public int MinimumError
		{
			get
			{
				return minError;
			}
			set
			{
				minError = value;
			}
		}

		public IterativeThreshold()
		{
		}

		public IterativeThreshold(int minError)
		{
			this.minError = minError;
		}

		public IterativeThreshold(int minError, int threshold)
		{
			this.minError = minError;
			base.threshold = threshold;
		}

		public int CalculateThreshold(Bitmap image, Rectangle rect)
		{
			int num = 0;
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			try
			{
				return CalculateThreshold(bitmapData, rect);
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public int CalculateThreshold(BitmapData image, Rectangle rect)
		{
			return CalculateThreshold(new UnmanagedImage(image), rect);
		}

		public unsafe int CalculateThreshold(UnmanagedImage image, Rectangle rect)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed && image.PixelFormat != PixelFormat.Format16bppGrayScale)
			{
				throw new UnsupportedImageFormatException("Source pixel format is not supported by the routine.");
			}
			int num = threshold;
			int left = rect.Left;
			int top = rect.Top;
			int num2 = left + rect.Width;
			int num3 = top + rect.Height;
			int[] array = null;
			int num4 = 0;
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				array = new int[256];
				num4 = 256;
				byte* ptr = (byte*)image.ImageData.ToPointer();
				int num5 = image.Stride - rect.Width;
				ptr += top * image.Stride + left;
				for (int i = top; i < num3; i++)
				{
					int num6 = left;
					while (num6 < num2)
					{
						array[*ptr]++;
						num6++;
						ptr++;
					}
					ptr += num5;
				}
			}
			else
			{
				array = new int[65536];
				num4 = 65536;
				byte* ptr2 = (byte*)image.ImageData.ToPointer() + (long)left * 2L;
				int stride = image.Stride;
				for (int j = top; j < num3; j++)
				{
					ushort* ptr3 = (ushort*)(ptr2 + (long)j * (long)stride);
					int num7 = left;
					while (num7 < num2)
					{
						array[*ptr3]++;
						num7++;
						ptr3++;
					}
				}
			}
			int num8 = 0;
			do
			{
				num8 = num;
				double num9 = 0.0;
				int num10 = 0;
				double num11 = 0.0;
				int num12 = 0;
				for (int k = 0; k < num; k++)
				{
					num11 += (double)k * (double)array[k];
					num12 += array[k];
				}
				for (int l = num; l < num4; l++)
				{
					num9 += (double)l * (double)array[l];
					num10 += array[l];
				}
				num11 /= (double)num12;
				num9 /= (double)num10;
				num = ((num12 == 0) ? ((int)num9) : ((num10 != 0) ? ((int)((num11 + num9) / 2.0)) : ((int)num11)));
			}
			while (System.Math.Abs(num8 - num) > minError);
			return num;
		}

		protected override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			threshold = CalculateThreshold(image, rect);
			base.ProcessFilter(image, rect);
		}
	}
}
