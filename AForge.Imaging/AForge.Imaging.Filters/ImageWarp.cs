using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ImageWarp : BaseFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		private IntPoint[,] warpMap;

		public IntPoint[,] WarpMap
		{
			get
			{
				return warpMap;
			}
			set
			{
				if (value == null)
				{
					throw new NullReferenceException("Warp map can not be set to null.");
				}
				warpMap = value;
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public ImageWarp(IntPoint[,] warpMap)
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			WarpMap = warpMap;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage source, UnmanagedImage destination)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(source.PixelFormat) / 8;
			int width = source.Width;
			int height = source.Height;
			int num2 = System.Math.Min(width, warpMap.GetLength(1));
			int num3 = System.Math.Min(height, warpMap.GetLength(0));
			int stride = source.Stride;
			int stride2 = destination.Stride;
			int num4 = stride2 - num2 * num;
			byte* ptr = (byte*)source.ImageData.ToPointer();
			byte* ptr2 = (byte*)destination.ImageData.ToPointer();
			for (int i = 0; i < num3; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					int num5 = j + warpMap[i, j].X;
					int num6 = i + warpMap[i, j].Y;
					if (num5 >= 0 && num6 >= 0 && num5 < width && num6 < height)
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
						int num8 = 0;
						while (num8 < num)
						{
							*ptr2 = 0;
							num8++;
							ptr2++;
						}
					}
				}
				if (width != num2)
				{
					SystemTools.CopyUnmanagedMemory(ptr2, ptr + (long)i * (long)stride + (long)num2 * (long)num, (width - num2) * num);
				}
				ptr2 += num4;
			}
			int num9 = num3;
			while (num9 < height)
			{
				SystemTools.CopyUnmanagedMemory(ptr2, ptr + (long)num9 * (long)stride, width * num);
				num9++;
				ptr2 += stride2;
			}
		}
	}
}
