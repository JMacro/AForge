using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class HSLFiltering : BaseInPlacePartialFilter
	{
		private IntRange hue = new IntRange(0, 359);

		private Range saturation = new Range(0f, 1f);

		private Range luminance = new Range(0f, 1f);

		private int fillH;

		private float fillS;

		private float fillL;

		private bool fillOutsideRange = true;

		private bool updateH = true;

		private bool updateS = true;

		private bool updateL = true;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public IntRange Hue
		{
			get
			{
				return hue;
			}
			set
			{
				hue = value;
			}
		}

		public Range Saturation
		{
			get
			{
				return saturation;
			}
			set
			{
				saturation = value;
			}
		}

		public Range Luminance
		{
			get
			{
				return luminance;
			}
			set
			{
				luminance = value;
			}
		}

		public HSL FillColor
		{
			get
			{
				return new HSL(fillH, fillS, fillL);
			}
			set
			{
				fillH = value.Hue;
				fillS = value.Saturation;
				fillL = value.Luminance;
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

		public bool UpdateHue
		{
			get
			{
				return updateH;
			}
			set
			{
				updateH = value;
			}
		}

		public bool UpdateSaturation
		{
			get
			{
				return updateS;
			}
			set
			{
				updateS = value;
			}
		}

		public bool UpdateLuminance
		{
			get
			{
				return updateL;
			}
			set
			{
				updateL = value;
			}
		}

		public HSLFiltering()
		{
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		public HSLFiltering(IntRange hue, Range saturation, Range luminance)
			: this()
		{
			this.hue = hue;
			this.saturation = saturation;
			this.luminance = luminance;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = (image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
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
					bool flag = false;
					rGB.Red = ptr[2];
					rGB.Green = ptr[1];
					rGB.Blue = *ptr;
					HSL.FromRGB(rGB, hSL);
					if (hSL.Saturation >= saturation.Min && hSL.Saturation <= saturation.Max && hSL.Luminance >= luminance.Min && hSL.Luminance <= luminance.Max && ((hue.Min < hue.Max && hSL.Hue >= hue.Min && hSL.Hue <= hue.Max) || (hue.Min > hue.Max && (hSL.Hue >= hue.Min || hSL.Hue <= hue.Max))))
					{
						if (!fillOutsideRange)
						{
							if (updateH)
							{
								hSL.Hue = fillH;
							}
							if (updateS)
							{
								hSL.Saturation = fillS;
							}
							if (updateL)
							{
								hSL.Luminance = fillL;
							}
							flag = true;
						}
					}
					else if (fillOutsideRange)
					{
						if (updateH)
						{
							hSL.Hue = fillH;
						}
						if (updateS)
						{
							hSL.Saturation = fillS;
						}
						if (updateL)
						{
							hSL.Luminance = fillL;
						}
						flag = true;
					}
					if (flag)
					{
						HSL.ToRGB(hSL, rGB);
						ptr[2] = rGB.Red;
						ptr[1] = rGB.Green;
						*ptr = rGB.Blue;
					}
					num5++;
					ptr += num;
				}
				ptr += num4;
			}
		}
	}
}
