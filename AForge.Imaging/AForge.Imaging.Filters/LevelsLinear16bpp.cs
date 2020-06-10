using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class LevelsLinear16bpp : BaseInPlacePartialFilter
	{
		private IntRange inRed = new IntRange(0, 65535);

		private IntRange inGreen = new IntRange(0, 65535);

		private IntRange inBlue = new IntRange(0, 65535);

		private IntRange outRed = new IntRange(0, 65535);

		private IntRange outGreen = new IntRange(0, 65535);

		private IntRange outBlue = new IntRange(0, 65535);

		private ushort[] mapRed = new ushort[65536];

		private ushort[] mapGreen = new ushort[65536];

		private ushort[] mapBlue = new ushort[65536];

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public IntRange InRed
		{
			get
			{
				return inRed;
			}
			set
			{
				inRed = value;
				CalculateMap(inRed, outRed, mapRed);
			}
		}

		public IntRange InGreen
		{
			get
			{
				return inGreen;
			}
			set
			{
				inGreen = value;
				CalculateMap(inGreen, outGreen, mapGreen);
			}
		}

		public IntRange InBlue
		{
			get
			{
				return inBlue;
			}
			set
			{
				inBlue = value;
				CalculateMap(inBlue, outBlue, mapBlue);
			}
		}

		public IntRange InGray
		{
			get
			{
				return inGreen;
			}
			set
			{
				inGreen = value;
				CalculateMap(inGreen, outGreen, mapGreen);
			}
		}

		public IntRange Input
		{
			set
			{
				inRed = (inGreen = (inBlue = value));
				CalculateMap(inRed, outRed, mapRed);
				CalculateMap(inGreen, outGreen, mapGreen);
				CalculateMap(inBlue, outBlue, mapBlue);
			}
		}

		public IntRange OutRed
		{
			get
			{
				return outRed;
			}
			set
			{
				outRed = value;
				CalculateMap(inRed, outRed, mapRed);
			}
		}

		public IntRange OutGreen
		{
			get
			{
				return outGreen;
			}
			set
			{
				outGreen = value;
				CalculateMap(inGreen, outGreen, mapGreen);
			}
		}

		public IntRange OutBlue
		{
			get
			{
				return outBlue;
			}
			set
			{
				outBlue = value;
				CalculateMap(inBlue, outBlue, mapBlue);
			}
		}

		public IntRange OutGray
		{
			get
			{
				return outGreen;
			}
			set
			{
				outGreen = value;
				CalculateMap(inGreen, outGreen, mapGreen);
			}
		}

		public IntRange Output
		{
			set
			{
				outRed = (outGreen = (outBlue = value));
				CalculateMap(inRed, outRed, mapRed);
				CalculateMap(inGreen, outGreen, mapGreen);
				CalculateMap(inBlue, outBlue, mapBlue);
			}
		}

		public LevelsLinear16bpp()
		{
			CalculateMap(inRed, outRed, mapRed);
			CalculateMap(inGreen, outGreen, mapGreen);
			CalculateMap(inBlue, outBlue, mapBlue);
			formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
			formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
			formatTranslations[PixelFormat.Format64bppPArgb] = PixelFormat.Format64bppPArgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 16;
			int left = rect.Left;
			int top = rect.Top;
			int num2 = left + rect.Width;
			int num3 = top + rect.Height;
			_ = image.Stride;
			_ = rect.Width;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			if (image.PixelFormat == PixelFormat.Format16bppGrayScale)
			{
				for (int i = top; i < num3; i++)
				{
					ushort* ptr2 = (ushort*)(ptr + (long)i * (long)image.Stride + (long)left * 2L);
					int num4 = left;
					while (num4 < num2)
					{
						*ptr2 = mapGreen[*ptr2];
						num4++;
						ptr2++;
					}
				}
				return;
			}
			for (int j = top; j < num3; j++)
			{
				ushort* ptr3 = (ushort*)(ptr + (long)j * (long)image.Stride + (long)(left * num) * 2L);
				int num5 = left;
				while (num5 < num2)
				{
					ptr3[2] = mapRed[ptr3[2]];
					ptr3[1] = mapGreen[ptr3[1]];
					*ptr3 = mapBlue[*ptr3];
					num5++;
					ptr3 += num;
				}
			}
		}

		private void CalculateMap(IntRange inRange, IntRange outRange, ushort[] map)
		{
			double num = 0.0;
			double num2 = 0.0;
			if (inRange.Max != inRange.Min)
			{
				num = (double)(outRange.Max - outRange.Min) / (double)(inRange.Max - inRange.Min);
				num2 = (double)outRange.Min - num * (double)inRange.Min;
			}
			for (int i = 0; i < 65536; i++)
			{
				ushort num3 = (ushort)i;
				num3 = (map[i] = ((num3 < inRange.Max) ? ((num3 > inRange.Min) ? ((ushort)(num * (double)(int)num3 + num2)) : ((ushort)outRange.Min)) : ((ushort)outRange.Max)));
			}
		}
	}
}
