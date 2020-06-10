using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ExtractNormalizedRGBChannel : BaseFilter
	{
		private short channel = 2;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public short Channel
		{
			get
			{
				return channel;
			}
			set
			{
				if (value != 2 && value != 1 && value != 0)
				{
					throw new ArgumentException("Invalid channel is specified.");
				}
				channel = value;
			}
		}

		public ExtractNormalizedRGBChannel()
		{
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format16bppGrayScale;
		}

		public ExtractNormalizedRGBChannel(short channel)
			: this()
		{
			Channel = channel;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			int num = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
			if (num <= 4)
			{
				int num2 = sourceData.Stride - width * num;
				int num3 = destinationData.Stride - width;
				byte* ptr = (byte*)sourceData.ImageData.ToPointer();
				byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
				for (int i = 0; i < height; i++)
				{
					int num4 = 0;
					while (num4 < width)
					{
						int num5 = ptr[2] + ptr[1] + *ptr;
						*ptr2 = (byte)((num5 != 0) ? ((byte)(255 * ptr[channel] / num5)) : 0);
						num4++;
						ptr += num;
						ptr2++;
					}
					ptr += num2;
					ptr2 += num3;
				}
				return;
			}
			num /= 2;
			byte* ptr3 = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr4 = (byte*)destinationData.ImageData.ToPointer();
			int stride = sourceData.Stride;
			int stride2 = destinationData.Stride;
			for (int j = 0; j < height; j++)
			{
				ushort* ptr5 = (ushort*)(ptr3 + (long)j * (long)stride);
				ushort* ptr6 = (ushort*)(ptr4 + (long)j * (long)stride2);
				int num6 = 0;
				while (num6 < width)
				{
					int num5 = ptr5[2] + ptr5[1] + *ptr5;
					*ptr6 = (ushort)((num5 != 0) ? ((ushort)(65535 * ptr5[channel] / num5)) : 0);
					num6++;
					ptr5 += num;
					ptr6++;
				}
			}
		}
	}
}
