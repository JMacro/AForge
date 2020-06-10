using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Crop : BaseTransformationFilter
	{
		private Rectangle rect;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public Rectangle Rectangle
		{
			get
			{
				return rect;
			}
			set
			{
				rect = value;
			}
		}

		public Crop(Rectangle rect)
		{
			this.rect = rect;
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
			formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
		}

		protected override Size CalculateNewImageSize(UnmanagedImage sourceData)
		{
			return new Size(rect.Width, rect.Height);
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			Rectangle rectangle = rect;
			rectangle.Intersect(new Rectangle(0, 0, sourceData.Width, sourceData.Height));
			int left = rectangle.Left;
			int top = rectangle.Top;
			int num = rectangle.Bottom - 1;
			int width = rectangle.Width;
			int stride = sourceData.Stride;
			int stride2 = destinationData.Stride;
			int num2 = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
			int count = width * num2;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer() + (long)top * (long)stride + (long)left * (long)num2;
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			if (rect.Top < 0)
			{
				ptr2 -= (long)stride2 * (long)rect.Top;
			}
			if (rect.Left < 0)
			{
				ptr2 -= (long)num2 * (long)rect.Left;
			}
			for (int i = top; i <= num; i++)
			{
				SystemTools.CopyUnmanagedMemory(ptr2, ptr, count);
				ptr += stride;
				ptr2 += stride2;
			}
		}
	}
}
