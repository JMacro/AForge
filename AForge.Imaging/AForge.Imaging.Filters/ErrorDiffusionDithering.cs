using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public abstract class ErrorDiffusionDithering : BaseInPlacePartialFilter
	{
		private byte threshold = 128;

		protected int x;

		protected int y;

		protected int startX;

		protected int startY;

		protected int stopX;

		protected int stopY;

		protected int stride;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public byte ThresholdValue
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

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		protected ErrorDiffusionDithering()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
		}

		protected unsafe abstract void Diffuse(int error, byte* ptr);

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			startX = rect.Left;
			startY = rect.Top;
			stopX = startX + rect.Width;
			stopY = startY + rect.Height;
			stride = image.Stride;
			int num = stride - rect.Width;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += startY * stride + startX;
			for (y = startY; y < stopY; y++)
			{
				x = startX;
				while (x < stopX)
				{
					int num2 = *ptr;
					int error;
					if (num2 >= threshold)
					{
						*ptr = byte.MaxValue;
						error = num2 - 255;
					}
					else
					{
						*ptr = 0;
						error = num2;
					}
					Diffuse(error, ptr);
					x++;
					ptr++;
				}
				ptr += num;
			}
		}
	}
}
