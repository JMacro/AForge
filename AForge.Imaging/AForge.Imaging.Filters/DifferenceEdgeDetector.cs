using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class DifferenceEdgeDetector : BaseUsingCopyPartialFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public DifferenceEdgeDetector()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
		{
			int num = rect.Left + 1;
			int num2 = rect.Top + 1;
			int num3 = num + rect.Width - 2;
			int num4 = num2 + rect.Height - 2;
			int stride = destination.Stride;
			int stride2 = source.Stride;
			int num5 = stride - rect.Width + 2;
			int num6 = stride2 - rect.Width + 2;
			byte* ptr = (byte*)source.ImageData.ToPointer();
			byte* ptr2 = (byte*)destination.ImageData.ToPointer();
			ptr += stride2 * num2 + num;
			ptr2 += stride * num2 + num;
			for (int i = num2; i < num4; i++)
			{
				int num7 = num;
				while (num7 < num3)
				{
					int num8 = ptr[-stride2 - 1] - ptr[stride2 + 1];
					if (num8 < 0)
					{
						num8 = -num8;
					}
					int num9 = ptr[-stride2 + 1] - ptr[stride2 - 1];
					if (num9 < 0)
					{
						num9 = -num9;
					}
					if (num9 > num8)
					{
						num8 = num9;
					}
					num9 = ptr[-stride2] - ptr[stride2];
					if (num9 < 0)
					{
						num9 = -num9;
					}
					if (num9 > num8)
					{
						num8 = num9;
					}
					num9 = ptr[-1] - ptr[1];
					if (num9 < 0)
					{
						num9 = -num9;
					}
					if (num9 > num8)
					{
						num8 = num9;
					}
					*ptr2 = (byte)num8;
					num7++;
					ptr++;
					ptr2++;
				}
				ptr += num6;
				ptr2 += num5;
			}
			Drawing.Rectangle(destination, rect, Color.Black);
		}
	}
}
