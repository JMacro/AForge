using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class LevelsLinear : BaseInPlacePartialFilter
	{
		private IntRange inRed = new IntRange(0, 255);

		private IntRange inGreen = new IntRange(0, 255);

		private IntRange inBlue = new IntRange(0, 255);

		private IntRange outRed = new IntRange(0, 255);

		private IntRange outGreen = new IntRange(0, 255);

		private IntRange outBlue = new IntRange(0, 255);

		private byte[] mapRed = new byte[256];

		private byte[] mapGreen = new byte[256];

		private byte[] mapBlue = new byte[256];

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

		public LevelsLinear()
		{
			CalculateMap(inRed, outRed, mapRed);
			CalculateMap(inGreen, outGreen, mapGreen);
			CalculateMap(inBlue, outBlue, mapBlue);
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
			int num4 = image.Stride - rect.Width * num;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + left * num;
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				for (int i = top; i < num3; i++)
				{
					int num5 = left;
					while (num5 < num2)
					{
						*ptr = mapGreen[*ptr];
						num5++;
						ptr++;
					}
					ptr += num4;
				}
				return;
			}
			for (int j = top; j < num3; j++)
			{
				int num6 = left;
				while (num6 < num2)
				{
					ptr[2] = mapRed[ptr[2]];
					ptr[1] = mapGreen[ptr[1]];
					*ptr = mapBlue[*ptr];
					num6++;
					ptr += num;
				}
				ptr += num4;
			}
		}

		private void CalculateMap(IntRange inRange, IntRange outRange, byte[] map)
		{
			double num = 0.0;
			double num2 = 0.0;
			if (inRange.Max != inRange.Min)
			{
				num = (double)(outRange.Max - outRange.Min) / (double)(inRange.Max - inRange.Min);
				num2 = (double)outRange.Min - num * (double)inRange.Min;
			}
			for (int i = 0; i < 256; i++)
			{
				byte b = (byte)i;
				b = (map[i] = ((b < inRange.Max) ? ((b > inRange.Min) ? ((byte)(num * (double)(int)b + num2)) : ((byte)outRange.Min)) : ((byte)outRange.Max)));
			}
		}
	}
}
