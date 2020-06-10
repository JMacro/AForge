using System;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class BradleyLocalThresholding : BaseInPlaceFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		private int windowSize = 41;

		private float pixelBrightnessDifferenceLimit = 0.15f;

		public int WindowSize
		{
			get
			{
				return windowSize;
			}
			set
			{
				windowSize = System.Math.Max(3, value | 1);
			}
		}

		public float PixelBrightnessDifferenceLimit
		{
			get
			{
				return pixelBrightnessDifferenceLimit;
			}
			set
			{
				pixelBrightnessDifferenceLimit = System.Math.Max(0f, System.Math.Min(1f, value));
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public BradleyLocalThresholding()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image)
		{
			IntegralImage integralImage = IntegralImage.FromBitmap(image);
			int width = image.Width;
			int height = image.Height;
			int num = width - 1;
			int num2 = height - 1;
			int num3 = image.Stride - width;
			int num4 = windowSize / 2;
			float num5 = 1f - pixelBrightnessDifferenceLimit;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			for (int i = 0; i < height; i++)
			{
				int num6 = i - num4;
				int num7 = i + num4;
				if (num6 < 0)
				{
					num6 = 0;
				}
				if (num7 > num2)
				{
					num7 = num2;
				}
				int num8 = 0;
				while (num8 < width)
				{
					int num9 = num8 - num4;
					int num10 = num8 + num4;
					if (num9 < 0)
					{
						num9 = 0;
					}
					if (num10 > num)
					{
						num10 = num;
					}
					*ptr = (byte)((*ptr >= (int)(integralImage.GetRectangleMeanUnsafe(num9, num6, num10, num7) * num5)) ? 255 : 0);
					num8++;
					ptr++;
				}
				ptr += num3;
			}
		}
	}
}
