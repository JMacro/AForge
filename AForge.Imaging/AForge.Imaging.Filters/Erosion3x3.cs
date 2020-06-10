using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Erosion3x3 : BaseUsingCopyPartialFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public Erosion3x3()
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
			byte b = *ptr;
			if (ptr[1] < b)
			{
				b = ptr[1];
			}
			if (ptr[stride2] < b)
			{
				b = ptr[stride2];
			}
			if (ptr[stride2 + 1] < b)
			{
				b = ptr[stride2 + 1];
			}
			*ptr2 = b;
			ptr++;
			ptr2++;
			int num7 = num;
			while (num7 < num3)
			{
				b = *ptr;
				if (ptr[-1] < b)
				{
					b = ptr[-1];
				}
				if (ptr[1] < b)
				{
					b = ptr[1];
				}
				if (ptr[stride2 - 1] < b)
				{
					b = ptr[stride2 - 1];
				}
				if (ptr[stride2] < b)
				{
					b = ptr[stride2];
				}
				if (ptr[stride2 + 1] < b)
				{
					b = ptr[stride2 + 1];
				}
				*ptr2 = b;
				num7++;
				ptr++;
				ptr2++;
			}
			b = *ptr;
			if (ptr[-1] < b)
			{
				b = ptr[-1];
			}
			if (ptr[stride2 - 1] < b)
			{
				b = ptr[stride2 - 1];
			}
			if (ptr[stride2] < b)
			{
				b = ptr[stride2];
			}
			*ptr2 = b;
			ptr += num6;
			ptr2 += num5;
			for (int i = num2; i < num4; i++)
			{
				b = *ptr;
				if (ptr[1] < b)
				{
					b = ptr[1];
				}
				if (ptr[-stride2] < b)
				{
					b = ptr[-stride2];
				}
				if (ptr[-stride2 + 1] < b)
				{
					b = ptr[-stride2 + 1];
				}
				if (ptr[stride2] < b)
				{
					b = ptr[stride2];
				}
				if (ptr[stride2 + 1] < b)
				{
					b = ptr[stride2 + 1];
				}
				*ptr2 = b;
				ptr++;
				ptr2++;
				int num8 = num;
				while (num8 < num3)
				{
					b = *ptr;
					if (ptr[-1] < b)
					{
						b = ptr[-1];
					}
					if (ptr[1] < b)
					{
						b = ptr[1];
					}
					if (ptr[-stride2 - 1] < b)
					{
						b = ptr[-stride2 - 1];
					}
					if (ptr[-stride2] < b)
					{
						b = ptr[-stride2];
					}
					if (ptr[-stride2 + 1] < b)
					{
						b = ptr[-stride2 + 1];
					}
					if (ptr[stride2 - 1] < b)
					{
						b = ptr[stride2 - 1];
					}
					if (ptr[stride2] < b)
					{
						b = ptr[stride2];
					}
					if (ptr[stride2 + 1] < b)
					{
						b = ptr[stride2 + 1];
					}
					*ptr2 = b;
					num8++;
					ptr++;
					ptr2++;
				}
				b = *ptr;
				if (ptr[-1] < b)
				{
					b = ptr[-1];
				}
				if (ptr[-stride2 - 1] < b)
				{
					b = ptr[-stride2 - 1];
				}
				if (ptr[-stride2] < b)
				{
					b = ptr[-stride2];
				}
				if (ptr[stride2 - 1] < b)
				{
					b = ptr[stride2 - 1];
				}
				if (ptr[stride2] < b)
				{
					b = ptr[stride2];
				}
				*ptr2 = b;
				ptr += num6;
				ptr2 += num5;
			}
			*ptr2 = (byte)(*ptr | ptr[1] | ptr[-stride2] | ptr[-stride2 + 1]);
			b = *ptr;
			if (ptr[1] < b)
			{
				b = ptr[1];
			}
			if (ptr[-stride2] < b)
			{
				b = ptr[-stride2];
			}
			if (ptr[-stride2 + 1] < b)
			{
				b = ptr[-stride2 + 1];
			}
			*ptr2 = b;
			ptr++;
			ptr2++;
			int num9 = num;
			while (num9 < num3)
			{
				b = *ptr;
				if (ptr[-1] < b)
				{
					b = ptr[-1];
				}
				if (ptr[1] < b)
				{
					b = ptr[1];
				}
				if (ptr[-stride2 - 1] < b)
				{
					b = ptr[-stride2 - 1];
				}
				if (ptr[-stride2] < b)
				{
					b = ptr[-stride2];
				}
				if (ptr[-stride2 + 1] < b)
				{
					b = ptr[-stride2 + 1];
				}
				*ptr2 = b;
				num9++;
				ptr++;
				ptr2++;
			}
			b = *ptr;
			if (ptr[-1] < b)
			{
				b = ptr[-1];
			}
			if (ptr[-stride2 - 1] < b)
			{
				b = ptr[-stride2 - 1];
			}
			if (ptr[-stride2] < b)
			{
				b = ptr[-stride2];
			}
			*ptr2 = b;
		}
	}
}
