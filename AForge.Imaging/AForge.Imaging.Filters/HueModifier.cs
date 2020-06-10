using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class HueModifier : BaseInPlacePartialFilter
	{
		private int hue;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public int Hue
		{
			get
			{
				return hue;
			}
			set
			{
				hue = System.Math.Max(0, System.Math.Min(359, value));
			}
		}

		public HueModifier()
		{
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
		}

		public HueModifier(int hue)
			: this()
		{
			this.hue = hue;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int left = rect.Left;
			int top = rect.Top;
			int num2 = left + rect.Width;
			int num3 = top + rect.Height;
			int num4 = image.Stride - rect.Width * num;
			RGB rGB = new RGB();
			HSL hSL = new HSL();
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + left * num;
			for (int i = top; i < num3; i++)
			{
				int num5 = left;
				while (num5 < num2)
				{
					rGB.Red = ptr[2];
					rGB.Green = ptr[1];
					rGB.Blue = *ptr;
					HSL.FromRGB(rGB, hSL);
					hSL.Hue = hue;
					HSL.ToRGB(hSL, rGB);
					ptr[2] = rGB.Red;
					ptr[1] = rGB.Green;
					*ptr = rGB.Blue;
					num5++;
					ptr += num;
				}
				ptr += num4;
			}
		}
	}
}
