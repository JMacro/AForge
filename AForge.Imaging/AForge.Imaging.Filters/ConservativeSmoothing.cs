using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ConservativeSmoothing : BaseUsingCopyPartialFilter
	{
		private int size = 3;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public int KernelSize
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

		public ConservativeSmoothing()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		public ConservativeSmoothing(int size)
			: this()
		{
			KernelSize = size;
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
						byte b = byte.MaxValue;
						byte b2 = 0;
						byte b3;
						for (int j = -num6; j <= num6; j++)
						{
							int num8 = i + j;
							if (num8 < top)
							{
								continue;
							}
							if (num8 >= num3)
							{
								break;
							}
							for (int k = -num6; k <= num6; k++)
							{
								num8 = num7 + k;
								if (num8 >= left && j != k && num8 < num2)
								{
									b3 = ptr[j * stride + k];
									if (b3 < b)
									{
										b = b3;
									}
									if (b3 > b2)
									{
										b2 = b3;
									}
								}
							}
						}
						b3 = *ptr;
						*ptr2 = ((b3 > b2) ? b2 : ((b3 < b) ? b : b3));
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
				int num9 = left;
				while (num9 < num2)
				{
					byte b;
					byte b4;
					byte b5 = b = (b4 = byte.MaxValue);
					byte b2;
					byte b6;
					byte b7 = b2 = (b6 = 0);
					byte b3;
					for (int j = -num6; j <= num6; j++)
					{
						int num8 = l + j;
						if (num8 < top)
						{
							continue;
						}
						if (num8 >= num3)
						{
							break;
						}
						for (int k = -num6; k <= num6; k++)
						{
							num8 = num9 + k;
							if (num8 >= left && j != k && num8 < num2)
							{
								byte* ptr3 = ptr + (j * stride + k * num);
								b3 = ptr3[2];
								if (b3 < b5)
								{
									b5 = b3;
								}
								if (b3 > b7)
								{
									b7 = b3;
								}
								b3 = ptr3[1];
								if (b3 < b)
								{
									b = b3;
								}
								if (b3 > b2)
								{
									b2 = b3;
								}
								b3 = *ptr3;
								if (b3 < b4)
								{
									b4 = b3;
								}
								if (b3 > b6)
								{
									b6 = b3;
								}
							}
						}
					}
					b3 = ptr[2];
					ptr2[2] = ((b3 > b7) ? b7 : ((b3 < b5) ? b5 : b3));
					b3 = ptr[1];
					ptr2[1] = ((b3 > b2) ? b2 : ((b3 < b) ? b : b3));
					b3 = *ptr;
					*ptr2 = ((b3 > b6) ? b6 : ((b3 < b4) ? b4 : b3));
					num9++;
					ptr += num;
					ptr2 += num;
				}
				ptr += num4;
				ptr2 += num5;
			}
		}
	}
}
