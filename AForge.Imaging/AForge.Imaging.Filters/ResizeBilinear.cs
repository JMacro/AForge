using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ResizeBilinear : BaseResizeFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public ResizeBilinear(int newWidth, int newHeight)
			: base(newWidth, newHeight)
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			int num = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
			int stride = sourceData.Stride;
			int num2 = destinationData.Stride - num * newWidth;
			double num3 = (double)width / (double)newWidth;
			double num4 = (double)height / (double)newHeight;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			int num5 = height - 1;
			int num6 = width - 1;
			for (int i = 0; i < newHeight; i++)
			{
				double num7 = (double)i * num4;
				int num8 = (int)num7;
				int num9 = (num8 == num5) ? num8 : (num8 + 1);
				double num10 = num7 - (double)num8;
				double num11 = 1.0 - num10;
				byte* ptr3 = ptr + (long)num8 * (long)stride;
				byte* ptr4 = ptr + (long)num9 * (long)stride;
				for (int j = 0; j < newWidth; j++)
				{
					double num12 = (double)j * num3;
					int num13 = (int)num12;
					int num14 = (num13 == num6) ? num13 : (num13 + 1);
					double num15 = num12 - (double)num13;
					double num16 = 1.0 - num15;
					byte* ptr5 = ptr3 + (long)num13 * (long)num;
					byte* ptr6 = ptr3 + (long)num14 * (long)num;
					byte* ptr7 = ptr4 + (long)num13 * (long)num;
					byte* ptr8 = ptr4 + (long)num14 * (long)num;
					int num17 = 0;
					while (num17 < num)
					{
						*ptr2 = (byte)(num11 * (num16 * (double)(int)(*ptr5) + num15 * (double)(int)(*ptr6)) + num10 * (num16 * (double)(int)(*ptr7) + num15 * (double)(int)(*ptr8)));
						num17++;
						ptr2++;
						ptr5++;
						ptr6++;
						ptr7++;
						ptr8++;
					}
				}
				ptr2 += num2;
			}
		}
	}
}
