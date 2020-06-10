using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class YCbCrFiltering : BaseInPlacePartialFilter
	{
		private Range yRange = new Range(0f, 1f);

		private Range cbRange = new Range(-0.5f, 0.5f);

		private Range crRange = new Range(-0.5f, 0.5f);

		private float fillY;

		private float fillCb;

		private float fillCr;

		private bool fillOutsideRange = true;

		private bool updateY = true;

		private bool updateCb = true;

		private bool updateCr = true;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public Range Y
		{
			get
			{
				return yRange;
			}
			set
			{
				yRange = value;
			}
		}

		public Range Cb
		{
			get
			{
				return cbRange;
			}
			set
			{
				cbRange = value;
			}
		}

		public Range Cr
		{
			get
			{
				return crRange;
			}
			set
			{
				crRange = value;
			}
		}

		public YCbCr FillColor
		{
			get
			{
				return new YCbCr(fillY, fillCb, fillCr);
			}
			set
			{
				fillY = value.Y;
				fillCb = value.Cb;
				fillCr = value.Cr;
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

		public bool UpdateY
		{
			get
			{
				return updateY;
			}
			set
			{
				updateY = value;
			}
		}

		public bool UpdateCb
		{
			get
			{
				return updateCb;
			}
			set
			{
				updateCb = value;
			}
		}

		public bool UpdateCr
		{
			get
			{
				return updateCr;
			}
			set
			{
				updateCr = value;
			}
		}

		public YCbCrFiltering()
		{
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		public YCbCrFiltering(Range yRange, Range cbRange, Range crRange)
			: this()
		{
			this.yRange = yRange;
			this.cbRange = cbRange;
			this.crRange = crRange;
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
			YCbCr yCbCr = new YCbCr();
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
					YCbCr.FromRGB(rGB, yCbCr);
					if (yCbCr.Y >= yRange.Min && yCbCr.Y <= yRange.Max && yCbCr.Cb >= cbRange.Min && yCbCr.Cb <= cbRange.Max && yCbCr.Cr >= crRange.Min && yCbCr.Cr <= crRange.Max)
					{
						if (!fillOutsideRange)
						{
							if (updateY)
							{
								yCbCr.Y = fillY;
							}
							if (updateCb)
							{
								yCbCr.Cb = fillCb;
							}
							if (updateCr)
							{
								yCbCr.Cr = fillCr;
							}
							flag = true;
						}
					}
					else if (fillOutsideRange)
					{
						if (updateY)
						{
							yCbCr.Y = fillY;
						}
						if (updateCb)
						{
							yCbCr.Cb = fillCb;
						}
						if (updateCr)
						{
							yCbCr.Cr = fillCr;
						}
						flag = true;
					}
					if (flag)
					{
						YCbCr.ToRGB(yCbCr, rGB);
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
