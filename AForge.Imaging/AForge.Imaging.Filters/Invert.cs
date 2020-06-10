using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public sealed class Invert : BaseInPlacePartialFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public Invert()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = (image.PixelFormat == PixelFormat.Format8bppIndexed || image.PixelFormat == PixelFormat.Format16bppGrayScale) ? 1 : 3;
			int top = rect.Top;
			int num2 = top + rect.Height;
			int num3 = rect.Left * num;
			int num4 = num3 + rect.Width * num;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			if (image.PixelFormat == PixelFormat.Format8bppIndexed || image.PixelFormat == PixelFormat.Format24bppRgb)
			{
				int num5 = image.Stride - (num4 - num3);
				byte* ptr2 = ptr + (top * image.Stride + rect.Left * num);
				for (int i = top; i < num2; i++)
				{
					int num6 = num3;
					while (num6 < num4)
					{
						*ptr2 = (byte)(255 - *ptr2);
						num6++;
						ptr2++;
					}
					ptr2 += num5;
				}
				return;
			}
			int stride = image.Stride;
			ptr += top * image.Stride + rect.Left * num * 2;
			for (int j = top; j < num2; j++)
			{
				ushort* ptr3 = (ushort*)ptr;
				int num7 = num3;
				while (num7 < num4)
				{
					*ptr3 = (ushort)(65535 - *ptr3);
					num7++;
					ptr3++;
				}
				ptr += stride;
			}
		}
	}
}
