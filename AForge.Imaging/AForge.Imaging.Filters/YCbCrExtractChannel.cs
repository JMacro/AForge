using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class YCbCrExtractChannel : BaseFilter
	{
		private short channel;

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
				if (value != 0 && value != 1 && value != 2)
				{
					throw new ArgumentException("Invalid channel was specified.");
				}
				channel = value;
			}
		}

		public YCbCrExtractChannel()
		{
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
		}

		public YCbCrExtractChannel(short channel)
			: this()
		{
			Channel = channel;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
			int width = sourceData.Width;
			int height = sourceData.Height;
			int num2 = sourceData.Stride - width * num;
			int num3 = destinationData.Stride - width;
			RGB rGB = new RGB();
			YCbCr yCbCr = new YCbCr();
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			byte b = 0;
			for (int i = 0; i < height; i++)
			{
				int num4 = 0;
				while (num4 < width)
				{
					rGB.Red = ptr[2];
					rGB.Green = ptr[1];
					rGB.Blue = *ptr;
					YCbCr.FromRGB(rGB, yCbCr);
					switch (channel)
					{
					case 0:
						b = (byte)(yCbCr.Y * 255f);
						break;
					case 1:
						b = (byte)(((double)yCbCr.Cb + 0.5) * 255.0);
						break;
					case 2:
						b = (byte)(((double)yCbCr.Cr + 0.5) * 255.0);
						break;
					}
					*ptr2 = b;
					num4++;
					ptr += num;
					ptr2++;
				}
				ptr += num2;
				ptr2 += num3;
			}
		}
	}
}
