using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ContrastStretch : BaseInPlacePartialFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public ContrastStretch()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int left = rect.Left;
			int top = rect.Top;
			int num2 = left + rect.Width;
			int num3 = top + rect.Height;
			int stride = image.Stride;
			int num4 = stride - rect.Width * num;
			LevelsLinear levelsLinear = new LevelsLinear();
			byte* ptr = (byte*)image.ImageData.ToPointer();
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				ptr += top * stride + left;
				byte b = byte.MaxValue;
				byte b2 = 0;
				for (int i = top; i < num3; i++)
				{
					int num5 = left;
					while (num5 < num2)
					{
						byte b3 = *ptr;
						if (b3 < b)
						{
							b = b3;
						}
						if (b3 > b2)
						{
							b2 = b3;
						}
						num5++;
						ptr++;
					}
					ptr += num4;
				}
				levelsLinear.InGray = new IntRange(b, b2);
			}
			else
			{
				ptr += top * stride + left * num;
				byte b4 = byte.MaxValue;
				byte b5 = byte.MaxValue;
				byte b6 = byte.MaxValue;
				byte b7 = 0;
				byte b8 = 0;
				byte b9 = 0;
				for (int j = top; j < num3; j++)
				{
					int num6 = left;
					while (num6 < num2)
					{
						byte b10 = ptr[2];
						if (b10 < b4)
						{
							b4 = b10;
						}
						if (b10 > b7)
						{
							b7 = b10;
						}
						b10 = ptr[1];
						if (b10 < b5)
						{
							b5 = b10;
						}
						if (b10 > b8)
						{
							b8 = b10;
						}
						b10 = *ptr;
						if (b10 < b6)
						{
							b6 = b10;
						}
						if (b10 > b9)
						{
							b9 = b10;
						}
						num6++;
						ptr += num;
					}
					ptr += num4;
				}
				levelsLinear.InRed = new IntRange(b4, b7);
				levelsLinear.InGreen = new IntRange(b5, b8);
				levelsLinear.InBlue = new IntRange(b6, b9);
			}
			levelsLinear.ApplyInPlace(image, rect);
		}
	}
}
