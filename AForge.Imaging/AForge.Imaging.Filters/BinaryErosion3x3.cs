using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class BinaryErosion3x3 : BaseUsingCopyPartialFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public BinaryErosion3x3()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData, Rectangle rect)
		{
			if (rect.Width < 3 || rect.Height < 3)
			{
				throw new InvalidImagePropertiesException("Processing rectangle mast be at least 3x3 in size.");
			}
			int num = rect.Left + 1;
			int num2 = rect.Top + 1;
			int num3 = rect.Right - 1;
			int num4 = rect.Bottom - 1;
			int stride = destinationData.Stride;
			int stride2 = sourceData.Stride;
			int num5 = stride - rect.Width + 1;
			int num6 = stride2 - rect.Width + 1;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			ptr += num - 1 + (num2 - 1) * stride2;
			ptr2 += num - 1 + (num2 - 1) * stride;
			int num7 = num - 1;
			while (num7 < num3)
			{
				*ptr2 = 0;
				num7++;
				ptr++;
				ptr2++;
			}
			*ptr2 = 0;
			ptr += num6;
			ptr2 += num5;
			for (int i = num2; i < num4; i++)
			{
				*ptr2 = 0;
				ptr++;
				ptr2++;
				int num8 = num;
				while (num8 < num3)
				{
					*ptr2 = (byte)(*ptr & ptr[-1] & ptr[1] & ptr[-stride2] & ptr[-stride2 - 1] & ptr[-stride2 + 1] & ptr[stride2] & ptr[stride2 - 1] & ptr[stride2 + 1]);
					num8++;
					ptr++;
					ptr2++;
				}
				*ptr2 = 0;
				ptr += num6;
				ptr2 += num5;
			}
			int num9 = num - 1;
			while (num9 < num3)
			{
				*ptr2 = 0;
				num9++;
				ptr++;
				ptr2++;
			}
			*ptr2 = 0;
		}
	}
}
