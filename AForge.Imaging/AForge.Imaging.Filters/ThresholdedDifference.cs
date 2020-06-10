using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ThresholdedDifference : BaseFilter2
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		private int threshold = 15;

		private int whitePixelsCount;

		public int Threshold
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

		public int WhitePixelsCount => whitePixelsCount;

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public ThresholdedDifference()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format8bppIndexed;
		}

		public ThresholdedDifference(int threshold)
			: this()
		{
			this.threshold = threshold;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage overlay, UnmanagedImage destinationData)
		{
			whitePixelsCount = 0;
			int width = sourceData.Width;
			int height = sourceData.Height;
			int num = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)overlay.ImageData.ToPointer();
			byte* ptr3 = (byte*)destinationData.ImageData.ToPointer();
			if (num == 1)
			{
				int num2 = sourceData.Stride - width;
				int num3 = overlay.Stride - width;
				int num4 = destinationData.Stride - width;
				for (int i = 0; i < height; i++)
				{
					int num5 = 0;
					while (num5 < width)
					{
						int num6 = *ptr - *ptr2;
						if (num6 < 0)
						{
							num6 = -num6;
						}
						if (num6 > threshold)
						{
							*ptr3 = byte.MaxValue;
							whitePixelsCount++;
						}
						else
						{
							*ptr3 = 0;
						}
						num5++;
						ptr++;
						ptr2++;
						ptr3++;
					}
					ptr += num2;
					ptr2 += num3;
					ptr3 += num4;
				}
				return;
			}
			int num7 = sourceData.Stride - num * width;
			int num8 = overlay.Stride - num * width;
			int num9 = destinationData.Stride - width;
			for (int j = 0; j < height; j++)
			{
				int num10 = 0;
				while (num10 < width)
				{
					int num11 = ptr[2] - ptr2[2];
					int num12 = ptr[1] - ptr2[1];
					int num13 = *ptr - *ptr2;
					if (num11 < 0)
					{
						num11 = -num11;
					}
					if (num12 < 0)
					{
						num12 = -num12;
					}
					if (num13 < 0)
					{
						num13 = -num13;
					}
					if (num11 + num12 + num13 > threshold)
					{
						*ptr3 = byte.MaxValue;
						whitePixelsCount++;
					}
					else
					{
						*ptr3 = 0;
					}
					num10++;
					ptr += num;
					ptr2 += num;
					ptr3++;
				}
				ptr += num7;
				ptr2 += num8;
				ptr3 += num9;
			}
		}
	}
}
