using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ThresholdWithCarry : BaseInPlacePartialFilter
	{
		private byte threshold = 128;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public byte ThresholdValue
		{
			get
			{
				return threshold;
			}
			set
			{
				threshold = value;
			}
		}

		public ThresholdWithCarry()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
		}

		public ThresholdWithCarry(byte threshold)
			: this()
		{
			this.threshold = threshold;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int left = rect.Left;
			int top = rect.Top;
			int num = left + rect.Width;
			int num2 = top + rect.Height;
			int num3 = image.Stride - rect.Width;
			short num4 = 0;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + left;
			for (int i = top; i < num2; i++)
			{
				num4 = 0;
				int num5 = left;
				while (num5 < num)
				{
					num4 = (short)(num4 + *ptr);
					if (num4 >= threshold)
					{
						*ptr = byte.MaxValue;
						num4 = (short)(num4 - 255);
					}
					else
					{
						*ptr = 0;
					}
					num5++;
					ptr++;
				}
				ptr += num3;
			}
		}
	}
}
