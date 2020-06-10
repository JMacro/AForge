using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class SISThreshold : BaseInPlacePartialFilter
	{
		private Threshold thresholdFilter = new Threshold();

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public int ThresholdValue => thresholdFilter.ThresholdValue;

		public SISThreshold()
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
			int left = rect.Left;
			int top = rect.Top;
			int num = left + rect.Width;
			int num2 = top + rect.Height;
			int num3 = num - 1;
			int num4 = num2 - 1;
			int stride = image.Stride;
			int num5 = stride - rect.Width;
			double num6 = 0.0;
			double num7 = 0.0;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + left;
			ptr += stride;
			for (int i = top + 1; i < num4; i++)
			{
				ptr++;
				int num8 = left + 1;
				while (num8 < num3)
				{
					double num9 = System.Math.Abs(ptr[1] - ptr[-1]);
					double num10 = System.Math.Abs(ptr[stride] - ptr[-stride]);
					double num11 = (num9 > num10) ? num9 : num10;
					num6 += num11;
					num7 += num11 * (double)(int)(*ptr);
					num8++;
					ptr++;
				}
				ptr += num5 + 1;
			}
			if (num6 != 0.0)
			{
				return (byte)(num7 / num6);
			}
			return 0;
		}

		protected override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			thresholdFilter.ThresholdValue = CalculateThreshold(image, rect);
			thresholdFilter.ApplyInPlace(image, rect);
		}
	}
}
