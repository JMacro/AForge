using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Pixellate : BaseInPlacePartialFilter
	{
		private int pixelWidth = 8;

		private int pixelHeight = 8;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public int PixelWidth
		{
			get
			{
				return pixelWidth;
			}
			set
			{
				pixelWidth = System.Math.Max(2, System.Math.Min(32, value));
			}
		}

		public int PixelHeight
		{
			get
			{
				return pixelHeight;
			}
			set
			{
				pixelHeight = System.Math.Max(2, System.Math.Min(32, value));
			}
		}

		public int PixelSize
		{
			set
			{
				pixelWidth = (pixelHeight = System.Math.Max(2, System.Math.Min(32, value)));
			}
		}

		public Pixellate()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		public Pixellate(int pixelSize)
			: this()
		{
			PixelSize = pixelSize;
		}

		public Pixellate(int pixelWidth, int pixelHeight)
			: this()
		{
			PixelWidth = pixelWidth;
			PixelHeight = pixelHeight;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int top = rect.Top;
			int num2 = top + rect.Height;
			int width = rect.Width;
			int num3 = image.Stride - width * num;
			int num4 = (width - 1) / pixelWidth + 1;
			int num5 = (width - 1) % pixelWidth + 1;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + rect.Left * num;
			byte* ptr2 = ptr;
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				int[] array = new int[num4];
				int num6 = top;
				int num7 = top;
				while (num6 < num2)
				{
					Array.Clear(array, 0, num4);
					int num8 = 0;
					while (num8 < pixelHeight && num6 < num2)
					{
						int num9 = 0;
						while (num9 < width)
						{
							array[num9 / pixelWidth] += *ptr;
							num9++;
							ptr++;
						}
						ptr += num3;
						num8++;
						num6++;
					}
					int num10 = num8 * pixelWidth;
					int num11 = num8 * num5;
					int i;
					for (i = 0; i < num4 - 1; i++)
					{
						array[i] /= num10;
					}
					array[i] /= num11;
					num8 = 0;
					while (num8 < pixelHeight && num7 < num2)
					{
						int num9 = 0;
						while (num9 < width)
						{
							*ptr2 = (byte)array[num9 / pixelWidth];
							num9++;
							ptr2++;
						}
						ptr2 += num3;
						num8++;
						num7++;
					}
				}
				return;
			}
			int[] array2 = new int[num4 * 3];
			int num12 = top;
			int num13 = top;
			while (num12 < num2)
			{
				Array.Clear(array2, 0, num4 * 3);
				int num8 = 0;
				int num14;
				while (num8 < pixelHeight && num12 < num2)
				{
					int num9 = 0;
					while (num9 < width)
					{
						num14 = num9 / pixelWidth * 3;
						array2[num14] += ptr[2];
						array2[num14 + 1] += ptr[1];
						array2[num14 + 2] += *ptr;
						num9++;
						ptr += 3;
					}
					ptr += num3;
					num8++;
					num12++;
				}
				int num10 = num8 * pixelWidth;
				int num11 = num8 * num5;
				int i = 0;
				num14 = 0;
				while (i < num4 - 1)
				{
					array2[num14] /= num10;
					array2[num14 + 1] /= num10;
					array2[num14 + 2] /= num10;
					i++;
					num14 += 3;
				}
				array2[num14] /= num11;
				array2[num14 + 1] /= num11;
				array2[num14 + 2] /= num11;
				num8 = 0;
				while (num8 < pixelHeight && num13 < num2)
				{
					int num9 = 0;
					while (num9 < width)
					{
						num14 = num9 / pixelWidth * 3;
						ptr2[2] = (byte)array2[num14];
						ptr2[1] = (byte)array2[num14 + 1];
						*ptr2 = (byte)array2[num14 + 2];
						num9++;
						ptr2 += 3;
					}
					ptr2 += num3;
					num8++;
					num13++;
				}
			}
		}
	}
}
