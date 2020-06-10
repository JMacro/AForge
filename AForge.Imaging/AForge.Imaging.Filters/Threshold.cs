using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Threshold : BaseInPlacePartialFilter
	{
		protected int threshold = 128;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public int ThresholdValue
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

		public Threshold()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
		}

		public Threshold(int threshold)
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
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				int num3 = image.Stride - rect.Width;
				byte* ptr = (byte*)image.ImageData.ToPointer();
				ptr += top * image.Stride + left;
				for (int i = top; i < num2; i++)
				{
					int num4 = left;
					while (num4 < num)
					{
						*ptr = (byte)((*ptr >= threshold) ? 255 : 0);
						num4++;
						ptr++;
					}
					ptr += num3;
				}
				return;
			}
			byte* ptr2 = (byte*)image.ImageData.ToPointer() + (long)left * 2L;
			int stride = image.Stride;
			for (int j = top; j < num2; j++)
			{
				ushort* ptr3 = (ushort*)(ptr2 + (long)stride * (long)j);
				int num5 = left;
				while (num5 < num)
				{
					*ptr3 = (ushort)((*ptr3 >= threshold) ? 65535 : 0);
					num5++;
					ptr3++;
				}
			}
		}
	}
}
