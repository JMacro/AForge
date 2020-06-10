using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class CanvasCrop : BaseInPlaceFilter
	{
		private byte fillRed = byte.MaxValue;

		private byte fillGreen = byte.MaxValue;

		private byte fillBlue = byte.MaxValue;

		private byte fillGray = byte.MaxValue;

		private Rectangle region;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public Color FillColorRGB
		{
			get
			{
				return Color.FromArgb(fillRed, fillGreen, fillBlue);
			}
			set
			{
				fillRed = value.R;
				fillGreen = value.G;
				fillBlue = value.B;
			}
		}

		public byte FillColorGray
		{
			get
			{
				return fillGray;
			}
			set
			{
				fillGray = value;
			}
		}

		public Rectangle Region
		{
			get
			{
				return region;
			}
			set
			{
				region = value;
			}
		}

		private CanvasCrop()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
		}

		public CanvasCrop(Rectangle region)
			: this()
		{
			this.region = region;
		}

		public CanvasCrop(Rectangle region, Color fillColorRGB)
			: this()
		{
			this.region = region;
			fillRed = fillColorRGB.R;
			fillGreen = fillColorRGB.G;
			fillBlue = fillColorRGB.B;
		}

		public CanvasCrop(Rectangle region, byte fillColorGray)
			: this()
		{
			this.region = region;
			fillGray = fillColorGray;
		}

		public CanvasCrop(Rectangle region, Color fillColorRGB, byte fillColorGray)
			: this()
		{
			this.region = region;
			fillRed = fillColorRGB.R;
			fillGreen = fillColorRGB.G;
			fillBlue = fillColorRGB.B;
			fillGray = fillColorGray;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int width = image.Width;
			int height = image.Height;
			int num2 = image.Stride - width * num;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				for (int i = 0; i < height; i++)
				{
					int num3 = 0;
					while (num3 < width)
					{
						if (!region.Contains(num3, i))
						{
							*ptr = fillGray;
						}
						num3++;
						ptr++;
					}
					ptr += num2;
				}
				return;
			}
			for (int j = 0; j < height; j++)
			{
				int num4 = 0;
				while (num4 < width)
				{
					if (!region.Contains(num4, j))
					{
						ptr[2] = fillRed;
						ptr[1] = fillGreen;
						*ptr = fillBlue;
					}
					num4++;
					ptr += num;
				}
				ptr += num2;
			}
		}
	}
}
