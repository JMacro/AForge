using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ReplaceChannel : BaseInPlacePartialFilter
	{
		private short channel = 2;

		private Bitmap channelImage;

		private UnmanagedImage unmanagedChannelImage;

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

		public Bitmap ChannelImage
		{
			get
			{
				return channelImage;
			}
			set
			{
				if (value == null)
				{
					throw new NullReferenceException("Channel image was not specified.");
				}
				if (value.PixelFormat != PixelFormat.Format8bppIndexed && value.PixelFormat != PixelFormat.Format16bppGrayScale)
				{
					throw new InvalidImagePropertiesException("Channel image should be 8 bpp indexed or 16 bpp grayscale image.");
				}
				channelImage = value;
				unmanagedChannelImage = null;
			}
		}

		public UnmanagedImage UnmanagedChannelImage
		{
			get
			{
				return unmanagedChannelImage;
			}
			set
			{
				if (value == null)
				{
					throw new NullReferenceException("Channel image was not specified.");
				}
				if (value.PixelFormat != PixelFormat.Format8bppIndexed && value.PixelFormat != PixelFormat.Format16bppGrayScale)
				{
					throw new InvalidImagePropertiesException("Channel image should be 8 bpp indexed or 16 bpp grayscale image.");
				}
				channelImage = null;
				unmanagedChannelImage = value;
			}
		}

		private ReplaceChannel()
		{
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format16bppGrayScale;
		}

		public ReplaceChannel(short channel, Bitmap channelImage)
			: this()
		{
			Channel = channel;
			ChannelImage = channelImage;
		}

		public ReplaceChannel(short channel, UnmanagedImage channelImage)
			: this()
		{
			Channel = channel;
			UnmanagedChannelImage = channelImage;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			if (channel == 3 && num != 4 && num != 8)
			{
				throw new InvalidImagePropertiesException("Can not replace alpha channel of none ARGB image.");
			}
			int width = image.Width;
			int height = image.Height;
			int left = rect.Left;
			int top = rect.Top;
			int num2 = left + rect.Width;
			int num3 = top + rect.Height;
			int num4 = image.Stride - rect.Width * num;
			BitmapData bitmapData = null;
			int num5 = 0;
			PixelFormat pixelFormat = PixelFormat.Format16bppGrayScale;
			byte* ptr;
			if (channelImage != null)
			{
				if (width != channelImage.Width || height != channelImage.Height)
				{
					throw new InvalidImagePropertiesException("Channel image size does not match source image size.");
				}
				bitmapData = channelImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, channelImage.PixelFormat);
				ptr = (byte*)bitmapData.Scan0.ToPointer();
				num5 = bitmapData.Stride;
				pixelFormat = bitmapData.PixelFormat;
			}
			else
			{
				if (width != unmanagedChannelImage.Width || height != unmanagedChannelImage.Height)
				{
					throw new InvalidImagePropertiesException("Channel image size does not match source image size.");
				}
				ptr = (byte*)(void*)unmanagedChannelImage.ImageData;
				num5 = unmanagedChannelImage.Stride;
				pixelFormat = unmanagedChannelImage.PixelFormat;
			}
			if (num <= 4)
			{
				if (pixelFormat != PixelFormat.Format8bppIndexed)
				{
					throw new InvalidImagePropertiesException("Channel image's format does not correspond to format of the source image.");
				}
				int num6 = bitmapData.Stride - rect.Width;
				byte* ptr2 = (byte*)image.ImageData.ToPointer();
				ptr2 += top * image.Stride + left * num;
				ptr += top * num5 + left;
				for (int i = top; i < num3; i++)
				{
					int num7 = left;
					while (num7 < num2)
					{
						ptr2[channel] = *ptr;
						num7++;
						ptr2 += num;
						ptr++;
					}
					ptr2 += num4;
					ptr += num6;
				}
			}
			else
			{
				if (pixelFormat != PixelFormat.Format16bppGrayScale)
				{
					throw new InvalidImagePropertiesException("Channel image's format does not correspond to format of the source image.");
				}
				int stride = image.Stride;
				byte* ptr3 = (byte*)image.ImageData.ToPointer();
				ptr3 += (long)left * (long)num;
				ptr += (long)left * 2L;
				num /= 2;
				for (int j = top; j < num3; j++)
				{
					ushort* ptr4 = (ushort*)(ptr3 + (long)j * (long)stride);
					ushort* ptr5 = (ushort*)(ptr + (long)j * (long)num5);
					int num8 = left;
					while (num8 < num2)
					{
						ptr4[channel] = *ptr5;
						num8++;
						ptr4 += num;
						ptr5++;
					}
				}
			}
			if (bitmapData != null)
			{
				channelImage.UnlockBits(bitmapData);
			}
		}
	}
}
