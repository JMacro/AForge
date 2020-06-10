using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class SobelEdgeDetector : BaseUsingCopyPartialFilter
	{
		private bool scaleIntensity = true;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public bool ScaleIntensity
		{
			get
			{
				return scaleIntensity;
			}
			set
			{
				scaleIntensity = value;
			}
		}

		public SobelEdgeDetector()
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
			double num7 = 0.0;
			for (int i = num2; i < num4; i++)
			{
				int num8 = num;
				while (num8 < num3)
				{
					double num9 = System.Math.Min(255, System.Math.Abs(ptr[-stride2 - 1] + ptr[-stride2 + 1] - ptr[stride2 - 1] - ptr[stride2 + 1] + 2 * (ptr[-stride2] - ptr[stride2])) + System.Math.Abs(ptr[-stride2 + 1] + ptr[stride2 + 1] - ptr[-stride2 - 1] - ptr[stride2 - 1] + 2 * (ptr[1] - ptr[-1])));
					if (num9 > num7)
					{
						num7 = num9;
					}
					*ptr2 = (byte)num9;
					num8++;
					ptr++;
					ptr2++;
				}
				ptr += num6;
				ptr2 += num5;
			}
			if (scaleIntensity && num7 != 255.0)
			{
				double num10 = 255.0 / num7;
				ptr2 = (byte*)destination.ImageData.ToPointer();
				ptr2 += stride * num2 + num;
				for (int j = num2; j < num4; j++)
				{
					int num11 = num;
					while (num11 < num3)
					{
						*ptr2 = (byte)(num10 * (double)(int)(*ptr2));
						num11++;
						ptr2++;
					}
					ptr2 += num5;
				}
			}
			Drawing.Rectangle(destination, rect, Color.Black);
		}
	}
}
