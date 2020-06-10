using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Shrink : BaseTransformationFilter
	{
		private Color colorToRemove = Color.FromArgb(0, 0, 0);

		private int minX;

		private int minY;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public Color ColorToRemove
		{
			get
			{
				return colorToRemove;
			}
			set
			{
				colorToRemove = value;
			}
		}

		public Shrink()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		public Shrink(Color colorToRemove)
			: this()
		{
			this.colorToRemove = colorToRemove;
		}

		protected unsafe override Size CalculateNewImageSize(UnmanagedImage sourceData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			int num = sourceData.Stride - ((sourceData.PixelFormat == PixelFormat.Format8bppIndexed) ? width : (width * 3));
			byte r = colorToRemove.R;
			byte g = colorToRemove.G;
			byte b = colorToRemove.B;
			minX = width;
			minY = height;
			int num2 = 0;
			int num3 = 0;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			if (sourceData.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				for (int i = 0; i < height; i++)
				{
					int num4 = 0;
					while (num4 < width)
					{
						if (*ptr != g)
						{
							if (num4 < minX)
							{
								minX = num4;
							}
							if (num4 > num2)
							{
								num2 = num4;
							}
							if (i < minY)
							{
								minY = i;
							}
							if (i > num3)
							{
								num3 = i;
							}
						}
						num4++;
						ptr++;
					}
					ptr += num;
				}
			}
			else
			{
				for (int j = 0; j < height; j++)
				{
					int num5 = 0;
					while (num5 < width)
					{
						if (ptr[2] != r || ptr[1] != g || *ptr != b)
						{
							if (num5 < minX)
							{
								minX = num5;
							}
							if (num5 > num2)
							{
								num2 = num5;
							}
							if (j < minY)
							{
								minY = j;
							}
							if (j > num3)
							{
								num3 = j;
							}
						}
						num5++;
						ptr += 3;
					}
					ptr += num;
				}
			}
			if (minX == width && minY == height && num2 == 0 && num3 == 0)
			{
				minX = (minY = 0);
			}
			return new Size(num2 - minX + 1, num3 - minY + 1);
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = destinationData.Width;
			int height = destinationData.Height;
			int stride = sourceData.Stride;
			int stride2 = destinationData.Stride;
			int num = width;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			ptr += (long)minY * (long)stride;
			if (destinationData.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				ptr += minX;
			}
			else
			{
				ptr += (long)minX * 3L;
				num *= 3;
			}
			for (int i = 0; i < height; i++)
			{
				SystemTools.CopyUnmanagedMemory(ptr2, ptr, num);
				ptr2 += stride2;
				ptr += stride;
			}
		}
	}
}
