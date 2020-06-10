using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class OtsuThreshold : BaseInPlacePartialFilter
	{
		private Threshold thresholdFilter = new Threshold();

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public int ThresholdValue => thresholdFilter.ThresholdValue;

		public OtsuThreshold()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
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
			if (image.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				throw new UnsupportedImageFormatException("Source pixel format is not supported by the routine.");
			}
			int result = 0;
			int left = rect.Left;
			int top = rect.Top;
			int num = left + rect.Width;
			int num2 = top + rect.Height;
			int num3 = image.Stride - rect.Width;
			int[] array = new int[256];
			double[] array2 = new double[256];
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + left;
			for (int i = top; i < num2; i++)
			{
				int num4 = left;
				while (num4 < num)
				{
					array[*ptr]++;
					num4++;
					ptr++;
				}
				ptr += num3;
			}
			int num5 = (num - left) * (num2 - top);
			double num6 = 0.0;
			for (int j = 0; j < 256; j++)
			{
				array2[j] = (double)array[j] / (double)num5;
				num6 += array2[j] * (double)j;
			}
			double num7 = double.MinValue;
			double num8 = 0.0;
			double num9 = 1.0;
			double num10 = 0.0;
			for (int k = 0; k < 256; k++)
			{
				if (!(num9 > 0.0))
				{
					break;
				}
				double num11 = num10;
				double num12 = (num6 - num11 * num8) / num9;
				double num13 = num8 * (1.0 - num8) * System.Math.Pow(num11 - num12, 2.0);
				if (num13 > num7)
				{
					num7 = num13;
					result = k;
				}
				num10 *= num8;
				num8 += array2[k];
				num9 -= array2[k];
				num10 += (double)k * array2[k];
				if (num8 != 0.0)
				{
					num10 /= num8;
				}
			}
			return result;
		}

		protected override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			thresholdFilter.ThresholdValue = CalculateThreshold(image, rect);
			thresholdFilter.ApplyInPlace(image, rect);
		}
	}
}
