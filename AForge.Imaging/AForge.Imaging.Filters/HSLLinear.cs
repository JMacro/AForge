using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class HSLLinear : BaseInPlacePartialFilter
	{
		private Range inLuminance = new Range(0f, 1f);

		private Range inSaturation = new Range(0f, 1f);

		private Range outLuminance = new Range(0f, 1f);

		private Range outSaturation = new Range(0f, 1f);

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public Range InLuminance
		{
			get
			{
				return inLuminance;
			}
			set
			{
				inLuminance = value;
			}
		}

		public Range OutLuminance
		{
			get
			{
				return outLuminance;
			}
			set
			{
				outLuminance = value;
			}
		}

		public Range InSaturation
		{
			get
			{
				return inSaturation;
			}
			set
			{
				inSaturation = value;
			}
		}

		public Range OutSaturation
		{
			get
			{
				return outSaturation;
			}
			set
			{
				outSaturation = value;
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public HSLLinear()
		{
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
			RGB rGB = new RGB();
			HSL hSL = new HSL();
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			if (inLuminance.Max != inLuminance.Min)
			{
				num5 = (outLuminance.Max - outLuminance.Min) / (inLuminance.Max - inLuminance.Min);
				num6 = outLuminance.Min - num5 * inLuminance.Min;
			}
			if (inSaturation.Max != inSaturation.Min)
			{
				num7 = (outSaturation.Max - outSaturation.Min) / (inSaturation.Max - inSaturation.Min);
				num8 = outSaturation.Min - num7 * inSaturation.Min;
			}
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + left * num;
			for (int i = top; i < num3; i++)
			{
				int num9 = left;
				while (num9 < num2)
				{
					rGB.Red = ptr[2];
					rGB.Green = ptr[1];
					rGB.Blue = *ptr;
					HSL.FromRGB(rGB, hSL);
					if (hSL.Luminance >= inLuminance.Max)
					{
						hSL.Luminance = outLuminance.Max;
					}
					else if (hSL.Luminance <= inLuminance.Min)
					{
						hSL.Luminance = outLuminance.Min;
					}
					else
					{
						hSL.Luminance = num5 * hSL.Luminance + num6;
					}
					if (hSL.Saturation >= inSaturation.Max)
					{
						hSL.Saturation = outSaturation.Max;
					}
					else if (hSL.Saturation <= inSaturation.Min)
					{
						hSL.Saturation = outSaturation.Min;
					}
					else
					{
						hSL.Saturation = num7 * hSL.Saturation + num8;
					}
					HSL.ToRGB(hSL, rGB);
					ptr[2] = rGB.Red;
					ptr[1] = rGB.Green;
					*ptr = rGB.Blue;
					num9++;
					ptr += num;
				}
				ptr += num4;
			}
		}
	}
}
