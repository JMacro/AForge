using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class SimpleSkeletonization : BaseUsingCopyPartialFilter
	{
		private byte bg;

		private byte fg = byte.MaxValue;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public byte Background
		{
			get
			{
				return bg;
			}
			set
			{
				bg = value;
			}
		}

		public byte Foreground
		{
			get
			{
				return fg;
			}
			set
			{
				fg = value;
			}
		}

		public SimpleSkeletonization()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
		}

		public SimpleSkeletonization(byte bg, byte fg)
			: this()
		{
			this.bg = bg;
			this.fg = fg;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
		{
			int left = rect.Left;
			int top = rect.Top;
			int num = left + rect.Width;
			int num2 = top + rect.Height;
			int stride = source.Stride;
			int stride2 = destination.Stride;
			int num3 = stride - rect.Width;
			byte* ptr = (byte*)source.ImageData.ToPointer();
			byte* ptr2 = (byte*)destination.ImageData.ToPointer();
			byte* ptr3 = ptr;
			byte* ptr4 = ptr2;
			ptr3 += top * stride + left;
			ptr4 += (long)top * (long)stride2;
			for (int i = top; i < num2; i++)
			{
				SystemTools.SetUnmanagedMemory(ptr4 + left, bg, num - left);
				int num4 = -1;
				int num5 = left;
				while (num5 < num)
				{
					if (num4 == -1)
					{
						if (*ptr3 == fg)
						{
							num4 = num5;
						}
					}
					else if (*ptr3 != fg)
					{
						ptr4[num4 + (num5 - num4 >> 1)] = fg;
						num4 = -1;
					}
					num5++;
					ptr3++;
				}
				if (num4 != -1)
				{
					ptr4[num4 + (num - num4 >> 1)] = fg;
				}
				ptr3 += num3;
				ptr4 += stride2;
			}
			ptr += (long)top * (long)stride;
			for (int j = left; j < num; j++)
			{
				ptr3 = ptr + j;
				ptr4 = ptr2 + j;
				int num4 = -1;
				int num6 = top;
				while (num6 < num2)
				{
					if (num4 == -1)
					{
						if (*ptr3 == fg)
						{
							num4 = num6;
						}
					}
					else if (*ptr3 != fg)
					{
						ptr4[(long)stride2 * (long)(num4 + (num6 - num4 >> 1))] = fg;
						num4 = -1;
					}
					num6++;
					ptr3 += stride;
				}
				if (num4 != -1)
				{
					ptr4[(long)stride2 * (long)(num4 + (num2 - num4 >> 1))] = fg;
				}
			}
		}
	}
}
