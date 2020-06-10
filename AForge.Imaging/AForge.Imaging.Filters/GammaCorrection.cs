using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class GammaCorrection : BaseInPlacePartialFilter
	{
		private double gamma;

		private byte[] table = new byte[256];

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public double Gamma
		{
			get
			{
				return gamma;
			}
			set
			{
				gamma = System.Math.Max(0.1, System.Math.Min(5.0, value));
				double y = 1.0 / gamma;
				for (int i = 0; i < 256; i++)
				{
					table[i] = (byte)System.Math.Min(255, (int)(System.Math.Pow((double)i / 255.0, y) * 255.0 + 0.5));
				}
			}
		}

		public GammaCorrection()
			: this(2.2)
		{
		}

		public GammaCorrection(double gamma)
		{
			Gamma = gamma;
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int num2 = rect.Left * num;
			int top = rect.Top;
			int num3 = num2 + rect.Width * num;
			int num4 = top + rect.Height;
			int num5 = image.Stride - rect.Width * num;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + num2;
			for (int i = top; i < num4; i++)
			{
				int num6 = num2;
				while (num6 < num3)
				{
					*ptr = table[*ptr];
					num6++;
					ptr++;
				}
				ptr += num5;
			}
		}
	}
}
