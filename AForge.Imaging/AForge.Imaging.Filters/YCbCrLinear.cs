using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class YCbCrLinear : BaseInPlacePartialFilter
	{
		private Range inY = new Range(0f, 1f);

		private Range inCb = new Range(-0.5f, 0.5f);

		private Range inCr = new Range(-0.5f, 0.5f);

		private Range outY = new Range(0f, 1f);

		private Range outCb = new Range(-0.5f, 0.5f);

		private Range outCr = new Range(-0.5f, 0.5f);

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public Range InY
		{
			get
			{
				return inY;
			}
			set
			{
				inY = value;
			}
		}

		public Range InCb
		{
			get
			{
				return inCb;
			}
			set
			{
				inCb = value;
			}
		}

		public Range InCr
		{
			get
			{
				return inCr;
			}
			set
			{
				inCr = value;
			}
		}

		public Range OutY
		{
			get
			{
				return outY;
			}
			set
			{
				outY = value;
			}
		}

		public Range OutCb
		{
			get
			{
				return outCb;
			}
			set
			{
				outCb = value;
			}
		}

		public Range OutCr
		{
			get
			{
				return outCr;
			}
			set
			{
				outCr = value;
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public YCbCrLinear()
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
			YCbCr yCbCr = new YCbCr();
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			float num10 = 0f;
			if (inY.Max != inY.Min)
			{
				num5 = (outY.Max - outY.Min) / (inY.Max - inY.Min);
				num6 = outY.Min - num5 * inY.Min;
			}
			if (inCb.Max != inCb.Min)
			{
				num7 = (outCb.Max - outCb.Min) / (inCb.Max - inCb.Min);
				num8 = outCb.Min - num7 * inCb.Min;
			}
			if (inCr.Max != inCr.Min)
			{
				num9 = (outCr.Max - outCr.Min) / (inCr.Max - inCr.Min);
				num10 = outCr.Min - num9 * inCr.Min;
			}
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + left * num;
			for (int i = top; i < num3; i++)
			{
				int num11 = left;
				while (num11 < num2)
				{
					rGB.Red = ptr[2];
					rGB.Green = ptr[1];
					rGB.Blue = *ptr;
					YCbCr.FromRGB(rGB, yCbCr);
					if (yCbCr.Y >= inY.Max)
					{
						yCbCr.Y = outY.Max;
					}
					else if (yCbCr.Y <= inY.Min)
					{
						yCbCr.Y = outY.Min;
					}
					else
					{
						yCbCr.Y = num5 * yCbCr.Y + num6;
					}
					if (yCbCr.Cb >= inCb.Max)
					{
						yCbCr.Cb = outCb.Max;
					}
					else if (yCbCr.Cb <= inCb.Min)
					{
						yCbCr.Cb = outCb.Min;
					}
					else
					{
						yCbCr.Cb = num7 * yCbCr.Cb + num8;
					}
					if (yCbCr.Cr >= inCr.Max)
					{
						yCbCr.Cr = outCr.Max;
					}
					else if (yCbCr.Cr <= inCr.Min)
					{
						yCbCr.Cr = outCr.Min;
					}
					else
					{
						yCbCr.Cr = num9 * yCbCr.Cr + num10;
					}
					YCbCr.ToRGB(yCbCr, rGB);
					ptr[2] = rGB.Red;
					ptr[1] = rGB.Green;
					*ptr = rGB.Blue;
					num11++;
					ptr += num;
				}
				ptr += num4;
			}
		}
	}
}
