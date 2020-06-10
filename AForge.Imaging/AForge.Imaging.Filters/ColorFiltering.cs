using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ColorFiltering : BaseInPlacePartialFilter
	{
		private IntRange red = new IntRange(0, 255);

		private IntRange green = new IntRange(0, 255);

		private IntRange blue = new IntRange(0, 255);

		private byte fillR;

		private byte fillG;

		private byte fillB;

		private bool fillOutsideRange = true;

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
			}
		}

		public RGB FillColor
		{
			get
			{
				return new RGB(fillR, fillG, fillB);
			}
			set
			{
				fillR = value.Red;
				fillG = value.Green;
				fillB = value.Blue;
			}
		}

		public bool FillOutsideRange
		{
			get
			{
				return fillOutsideRange;
			}
			set
			{
				fillOutsideRange = value;
			}
		}

		public ColorFiltering()
		{
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		public ColorFiltering(IntRange red, IntRange green, IntRange blue)
			: this()
		{
			this.red = red;
			this.green = green;
			this.blue = blue;
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
					byte b = ptr[2];
					byte b2 = ptr[1];
					byte b3 = *ptr;
					if (b >= red.Min && b <= red.Max && b2 >= green.Min && b2 <= green.Max && b3 >= blue.Min && b3 <= blue.Max)
					{
						if (!fillOutsideRange)
						{
							ptr[2] = fillR;
							ptr[1] = fillG;
							*ptr = fillB;
						}
					}
					else if (fillOutsideRange)
					{
						ptr[2] = fillR;
						ptr[1] = fillG;
						*ptr = fillB;
					}
					num5++;
					ptr += num;
				}
				ptr += num4;
			}
		}
	}
}
