using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ChannelFiltering : BaseInPlacePartialFilter
	{
		private IntRange red = new IntRange(0, 255);

		private IntRange green = new IntRange(0, 255);

		private IntRange blue = new IntRange(0, 255);

		private byte fillR;

		private byte fillG;

		private byte fillB;

		private bool redFillOutsideRange = true;

		private bool greenFillOutsideRange = true;

		private bool blueFillOutsideRange = true;

		private byte[] mapRed = new byte[256];

		private byte[] mapGreen = new byte[256];

		private byte[] mapBlue = new byte[256];

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public IntRange Red
		{
			get
			{
				return red;
			}
			set
			{
				red = value;
				CalculateMap(red, fillR, redFillOutsideRange, mapRed);
			}
		}

		public byte FillRed
		{
			get
			{
				return fillR;
			}
			set
			{
				fillR = value;
				CalculateMap(red, fillR, redFillOutsideRange, mapRed);
			}
		}

		public IntRange Green
		{
			get
			{
				return green;
			}
			set
			{
				green = value;
				CalculateMap(green, fillG, greenFillOutsideRange, mapGreen);
			}
		}

		public byte FillGreen
		{
			get
			{
				return fillG;
			}
			set
			{
				fillG = value;
				CalculateMap(green, fillG, greenFillOutsideRange, mapGreen);
			}
		}

		public IntRange Blue
		{
			get
			{
				return blue;
			}
			set
			{
				blue = value;
				CalculateMap(blue, fillB, blueFillOutsideRange, mapBlue);
			}
		}

		public byte FillBlue
		{
			get
			{
				return fillB;
			}
			set
			{
				fillB = value;
				CalculateMap(blue, fillB, blueFillOutsideRange, mapBlue);
			}
		}

		public bool RedFillOutsideRange
		{
			get
			{
				return redFillOutsideRange;
			}
			set
			{
				redFillOutsideRange = value;
				CalculateMap(red, fillR, redFillOutsideRange, mapRed);
			}
		}

		public bool GreenFillOutsideRange
		{
			get
			{
				return greenFillOutsideRange;
			}
			set
			{
				greenFillOutsideRange = value;
				CalculateMap(green, fillG, greenFillOutsideRange, mapGreen);
			}
		}

		public bool BlueFillOutsideRange
		{
			get
			{
				return blueFillOutsideRange;
			}
			set
			{
				blueFillOutsideRange = value;
				CalculateMap(blue, fillB, blueFillOutsideRange, mapBlue);
			}
		}

		public ChannelFiltering()
			: this(new IntRange(0, 255), new IntRange(0, 255), new IntRange(0, 255))
		{
		}

		public ChannelFiltering(IntRange red, IntRange green, IntRange blue)
		{
			Red = red;
			Green = green;
			Blue = blue;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = (image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
			int left = rect.Left;
			int top = rect.Top;
			int num2 = left + rect.Width;
			int num3 = top + rect.Height;
			int num4 = image.Stride - rect.Width * num;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + left * num;
			for (int i = top; i < num3; i++)
			{
				int num5 = left;
				while (num5 < num2)
				{
					ptr[2] = mapRed[ptr[2]];
					ptr[1] = mapGreen[ptr[1]];
					*ptr = mapBlue[*ptr];
					num5++;
					ptr += num;
				}
				ptr += num4;
			}
		}

		private void CalculateMap(IntRange range, byte fill, bool fillOutsideRange, byte[] map)
		{
			for (int i = 0; i < 256; i++)
			{
				if (i >= range.Min && i <= range.Max)
				{
					map[i] = (fillOutsideRange ? ((byte)i) : fill);
				}
				else
				{
					map[i] = (fillOutsideRange ? fill : ((byte)i));
				}
			}
		}
	}
}
