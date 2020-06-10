using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ResizeNearestNeighbor : BaseResizeFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public ResizeNearestNeighbor(int newWidth, int newHeight)
			: base(newWidth, newHeight)
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
			formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			int num = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
			int stride = sourceData.Stride;
			int stride2 = destinationData.Stride;
			double num2 = (double)width / (double)newWidth;
			double num3 = (double)height / (double)newHeight;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			for (int i = 0; i < newHeight; i++)
			{
				byte* ptr3 = ptr2 + (long)stride2 * (long)i;
				byte* ptr4 = ptr + (long)stride * (long)(int)((double)i * num3);
				for (int j = 0; j < newWidth; j++)
				{
					byte* ptr5 = ptr4 + (long)num * (long)(int)((double)j * num2);
					int num4 = 0;
					while (num4 < num)
					{
						*ptr3 = *ptr5;
						num4++;
						ptr3++;
						ptr5++;
					}
				}
			}
		}
	}
}
