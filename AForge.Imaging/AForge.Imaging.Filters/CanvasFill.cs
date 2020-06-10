using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class CanvasFill : BaseInPlaceFilter
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

		private CanvasFill()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
		}

		public CanvasFill(Rectangle region)
			: this()
		{
			this.region = region;
		}

		public CanvasFill(Rectangle region, Color fillColorRGB)
			: this()
		{
			this.region = region;
			fillRed = fillColorRGB.R;
			fillGreen = fillColorRGB.G;
			fillBlue = fillColorRGB.B;
		}

		public CanvasFill(Rectangle region, byte fillColorGray)
			: this()
		{
			this.region = region;
			fillGray = fillColorGray;
		}

		public CanvasFill(Rectangle region, Color fillColorRGB, byte fillColorGray)
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
			int num2 = System.Math.Max(0, region.X);
			int num3 = System.Math.Max(0, region.Y);
			if (num2 >= width || num3 >= height)
			{
				return;
			}
			int num4 = System.Math.Min(width, region.Right);
			int num5 = System.Math.Min(height, region.Bottom);
			if (num4 <= num2 || num5 <= num3)
			{
				return;
			}
			int stride = image.Stride;
			byte* ptr = (byte*)image.ImageData.ToPointer() + (long)num3 * (long)stride + (long)num2 * (long)num;
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				int count = num4 - num2;
				for (int i = num3; i < num5; i++)
				{
					SystemTools.SetUnmanagedMemory(ptr, fillGray, count);
					ptr += stride;
				}
				return;
			}
			int num6 = stride - (num4 - num2) * num;
			for (int j = num3; j < num5; j++)
			{
				int num7 = num2;
				while (num7 < num4)
				{
					ptr[2] = fillRed;
					ptr[1] = fillGreen;
					*ptr = fillBlue;
					num7++;
					ptr += num;
				}
				ptr += num6;
			}
		}
	}
}
