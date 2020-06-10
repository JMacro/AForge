using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ExtractChannel : BaseFilter
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
				if (value != 2 && value != 1 && value != 0 && value != 3)
				{
					throw new ArgumentException("Invalid channel is specified.");
				}
				channel = value;
			}
		}

		public ExtractChannel()
		{
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format16bppGrayScale;
		}

		public ExtractChannel(short channel)
			: this()
		{
			Channel = channel;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			int num = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
			if (channel == 3 && num != 4 && num != 8)
			{
				throw new InvalidImagePropertiesException("Can not extract alpha channel from none ARGB image.");
			}
			if (num <= 4)
			{
				int num2 = sourceData.Stride - width * num;
				int num3 = destinationData.Stride - width;
				byte* ptr = (byte*)sourceData.ImageData.ToPointer();
				byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
				ptr += channel;
				for (int i = 0; i < height; i++)
				{
					int num4 = 0;
					while (num4 < width)
					{
						*ptr2 = *ptr;
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
				ptr5 += channel;
				int num5 = 0;
				while (num5 < width)
				{
					*ptr6 = *ptr5;
					num5++;
					ptr5 += num;
					ptr6++;
				}
			}
		}
	}
}
