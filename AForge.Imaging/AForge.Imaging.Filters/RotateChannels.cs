using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public sealed class RotateChannels : BaseInPlacePartialFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public RotateChannels()
		{
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = (image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
			int left = rect.Left;
			int top = rect.Top;
			int num2 = left + rect.Width;
			int num3 = top + rect.Height;
			int num4 = image.Stride - rect.Width * num;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + left * num;
			for (int i = top; i < num3; i++)
			{
				int num5 = left;
				while (num5 < num2)
				{
					byte b = ptr[2];
					ptr[2] = ptr[1];
					ptr[1] = *ptr;
					*ptr = b;
					num5++;
					ptr += num;
				}
				ptr += num4;
			}
		}
	}
}
