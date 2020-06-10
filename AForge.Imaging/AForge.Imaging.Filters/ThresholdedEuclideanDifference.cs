using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ThresholdedEuclideanDifference : BaseFilter2
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

		public ThresholdedEuclideanDifference()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format8bppIndexed;
		}

		public ThresholdedEuclideanDifference(int threshold)
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
			int num10 = threshold * threshold;
			for (int j = 0; j < height; j++)
			{
				int num11 = 0;
				while (num11 < width)
				{
					int num12 = ptr[2] - ptr2[2];
					int num13 = ptr[1] - ptr2[1];
					int num14 = *ptr - *ptr2;
					if (num12 * num12 + num13 * num13 + num14 * num14 > num10)
					{
						*ptr3 = byte.MaxValue;
						whitePixelsCount++;
					}
					else
					{
						*ptr3 = 0;
					}
					num11++;
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
