using System.Collections.Generic;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public sealed class GrayscaleToRGB : BaseFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public GrayscaleToRGB()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format24bppRgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			int num = sourceData.Stride - width;
			int num2 = destinationData.Stride - width * 3;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			for (int i = 0; i < height; i++)
			{
				int num3 = 0;
				while (num3 < width)
				{
					byte* intPtr = ptr2 + 2;
					byte* intPtr2 = ptr2 + 1;
					byte b;
					*ptr2 = (b = *ptr);
					byte b2;
					*intPtr2 = (b2 = b);
					*intPtr = b2;
					num3++;
					ptr++;
					ptr2 += 3;
				}
				ptr += num;
				ptr2 += num2;
			}
		}
	}
}
