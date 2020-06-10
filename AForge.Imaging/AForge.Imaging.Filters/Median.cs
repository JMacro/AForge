using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Median : BaseUsingCopyPartialFilter
	{
		private int size = 3;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public int Size
		{
			get
			{
				return size;
			}
			set
			{
				size = System.Math.Max(3, System.Math.Min(25, value | 1));
			}
		}

		public Median()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		public Median(int size)
			: this()
		{
			Size = size;
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
			int num4 = stride - rect.Width * num;
			int num5 = stride2 - rect.Width * num;
			int num6 = size >> 1;
			byte[] array = new byte[size * size];
			byte[] array2 = new byte[size * size];
			byte[] array3 = new byte[size * size];
			byte* ptr = (byte*)source.ImageData.ToPointer();
			byte* ptr2 = (byte*)destination.ImageData.ToPointer();
			ptr += top * stride + left * num;
			ptr2 += top * stride2 + left * num;
			if (destination.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				for (int i = top; i < num3; i++)
				{
					int num7 = left;
					while (num7 < num2)
					{
						int num8 = 0;
						for (int j = -num6; j <= num6; j++)
						{
							int num9 = i + j;
							if (num9 < top)
							{
								continue;
							}
							if (num9 >= num3)
							{
								break;
							}
							for (int k = -num6; k <= num6; k++)
							{
								num9 = num7 + k;
								if (num9 >= left && num9 < num2)
								{
									array2[num8++] = ptr[j * stride + k];
								}
							}
						}
						Array.Sort(array2, 0, num8);
						*ptr2 = array2[num8 >> 1];
						num7++;
						ptr++;
						ptr2++;
					}
					ptr += num4;
					ptr2 += num5;
				}
				return;
			}
			for (int l = top; l < num3; l++)
			{
				int num10 = left;
				while (num10 < num2)
				{
					int num8 = 0;
					int num9;
					for (int j = -num6; j <= num6; j++)
					{
						num9 = l + j;
						if (num9 < top)
						{
							continue;
						}
						if (num9 >= num3)
						{
							break;
						}
						for (int k = -num6; k <= num6; k++)
						{
							num9 = num10 + k;
							if (num9 >= left && num9 < num2)
							{
								byte* ptr3 = ptr + (j * stride + k * num);
								array[num8] = ptr3[2];
								array2[num8] = ptr3[1];
								array3[num8] = *ptr3;
								num8++;
							}
						}
					}
					Array.Sort(array, 0, num8);
					Array.Sort(array2, 0, num8);
					Array.Sort(array3, 0, num8);
					num9 = num8 >> 1;
					ptr2[2] = array[num9];
					ptr2[1] = array2[num9];
					*ptr2 = array3[num9];
					num10++;
					ptr += num;
					ptr2 += num;
				}
				ptr += num4;
				ptr2 += num5;
			}
		}
	}
}
