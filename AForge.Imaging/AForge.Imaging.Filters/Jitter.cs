using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Jitter : BaseUsingCopyPartialFilter
	{
		private int radius = 2;

		private Random rand = new Random();

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public int Radius
		{
			get
			{
				return radius;
			}
			set
			{
				radius = System.Math.Max(1, System.Math.Min(10, value));
			}
		}

		public Jitter()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		public Jitter(int radius)
			: this()
		{
			Radius = radius;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(source.PixelFormat) / 8;
			int left = rect.Left;
			int top = rect.Top;
			int num2 = left + rect.Width;
			int num3 = top + rect.Height;
			int stride = source.Stride;
			int stride2 = destination.Stride;
			int num4 = stride2 - rect.Width * num;
			int maxValue = radius * 2 + 1;
			byte* ptr = (byte*)source.ImageData.ToPointer();
			byte* ptr2 = (byte*)destination.ImageData.ToPointer();
			if (stride == stride2)
			{
				SystemTools.CopyUnmanagedMemory(ptr2, ptr, stride * source.Height);
			}
			else
			{
				int count = source.Width * num;
				int i = 0;
				for (int height = source.Height; i < height; i++)
				{
					SystemTools.CopyUnmanagedMemory(ptr2 + (long)stride2 * (long)i, ptr + (long)stride * (long)i, count);
				}
			}
			ptr2 += top * stride2 + left * num;
			for (int j = top; j < num3; j++)
			{
				for (int k = left; k < num2; k++)
				{
					int num5 = k + rand.Next(maxValue) - radius;
					int num6 = j + rand.Next(maxValue) - radius;
					if (num5 >= left && num6 >= top && num5 < num2 && num6 < num3)
					{
						byte* ptr3 = ptr + (long)num6 * (long)stride + (long)num5 * (long)num;
						int num7 = 0;
						while (num7 < num)
						{
							*ptr2 = *ptr3;
							num7++;
							ptr2++;
							ptr3++;
						}
					}
					else
					{
						ptr2 += num;
					}
				}
				ptr2 += num4;
			}
		}
	}
}
