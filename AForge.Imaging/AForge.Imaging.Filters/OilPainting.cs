using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class OilPainting : BaseUsingCopyPartialFilter
	{
		private int brushSize = 5;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public int BrushSize
		{
			get
			{
				return brushSize;
			}
			set
			{
				brushSize = System.Math.Max(3, System.Math.Min(21, value | 1));
			}
		}

		public OilPainting()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		public OilPainting(int brushSize)
			: this()
		{
			BrushSize = brushSize;
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
			int num5 = stride - rect.Width * num;
			int num6 = brushSize >> 1;
			int[] array = new int[256];
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
						Array.Clear(array, 0, 256);
						int k;
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
							for (k = -num6; k <= num6; k++)
							{
								num8 = num7 + k;
								if (num8 >= left && num8 < num2)
								{
									byte b = ptr[j * stride + k];
									array[b]++;
								}
							}
						}
						byte b2 = 0;
						k = 0;
						for (int j = 0; j < 256; j++)
						{
							if (array[j] > k)
							{
								b2 = (byte)j;
								k = array[j];
							}
						}
						*ptr2 = b2;
						num7++;
						ptr++;
						ptr2++;
					}
					ptr += num4;
					ptr2 += num5;
				}
				return;
			}
			int[] array2 = new int[256];
			int[] array3 = new int[256];
			int[] array4 = new int[256];
			for (int l = top; l < num3; l++)
			{
				int num9 = left;
				while (num9 < num2)
				{
					Array.Clear(array, 0, 256);
					Array.Clear(array2, 0, 256);
					Array.Clear(array3, 0, 256);
					Array.Clear(array4, 0, 256);
					int k;
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
						for (k = -num6; k <= num6; k++)
						{
							num8 = num9 + k;
							if (num8 >= left && num8 < num2)
							{
								byte* ptr3 = ptr + (j * stride + k * num);
								byte b = (byte)(0.2125 * (double)(int)ptr3[2] + 0.7154 * (double)(int)ptr3[1] + 0.0721 * (double)(int)(*ptr3));
								array[b]++;
								array2[b] += ptr3[2];
								array3[b] += ptr3[1];
								array4[b] += *ptr3;
							}
						}
					}
					byte b2 = 0;
					k = 0;
					for (int j = 0; j < 256; j++)
					{
						if (array[j] > k)
						{
							b2 = (byte)j;
							k = array[j];
						}
					}
					ptr2[2] = (byte)(array2[b2] / array[b2]);
					ptr2[1] = (byte)(array3[b2] / array[b2]);
					*ptr2 = (byte)(array4[b2] / array[b2]);
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
