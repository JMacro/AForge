using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class YCbCrReplaceChannel : BaseInPlacePartialFilter
	{
		private short channel;

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
				if (value != 0 && value != 1 && value != 2)
				{
					throw new ArgumentException("Invalid YCbCr channel was specified.");
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
				if (value.PixelFormat != PixelFormat.Format8bppIndexed)
				{
					throw new InvalidImagePropertiesException("Channel image should be 8bpp indexed image (grayscale).");
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
				if (value.PixelFormat != PixelFormat.Format8bppIndexed)
				{
					throw new InvalidImagePropertiesException("Channel image should be 8bpp indexed image (grayscale).");
				}
				channelImage = null;
				unmanagedChannelImage = value;
			}
		}

		private YCbCrReplaceChannel()
		{
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		public YCbCrReplaceChannel(short channel, Bitmap channelImage)
			: this()
		{
			Channel = channel;
			ChannelImage = channelImage;
		}

		public YCbCrReplaceChannel(short channel, UnmanagedImage channelImage)
			: this()
		{
			Channel = channel;
			UnmanagedChannelImage = channelImage;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int width = image.Width;
			int height = image.Height;
			int left = rect.Left;
			int top = rect.Top;
			int num2 = left + rect.Width;
			int num3 = top + rect.Height;
			int num4 = image.Stride - rect.Width * num;
			BitmapData bitmapData = null;
			int num5 = 0;
			byte* ptr;
			if (channelImage != null)
			{
				if (width != channelImage.Width || height != channelImage.Height)
				{
					throw new InvalidImagePropertiesException("Channel image size does not match source image size.");
				}
				bitmapData = channelImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
				ptr = (byte*)bitmapData.Scan0.ToPointer();
				num5 = bitmapData.Stride;
			}
			else
			{
				if (width != unmanagedChannelImage.Width || height != unmanagedChannelImage.Height)
				{
					throw new InvalidImagePropertiesException("Channel image size does not match source image size.");
				}
				ptr = (byte*)(void*)unmanagedChannelImage.ImageData;
				num5 = unmanagedChannelImage.Stride;
			}
			int num6 = num5 - rect.Width;
			RGB rGB = new RGB();
			YCbCr yCbCr = new YCbCr();
			byte* ptr2 = (byte*)image.ImageData.ToPointer();
			ptr2 += top * image.Stride + left * num;
			ptr += top * num5 + left;
			for (int i = top; i < num3; i++)
			{
				int num7 = left;
				while (num7 < num2)
				{
					rGB.Red = ptr2[2];
					rGB.Green = ptr2[1];
					rGB.Blue = *ptr2;
					YCbCr.FromRGB(rGB, yCbCr);
					switch (channel)
					{
					case 0:
						yCbCr.Y = (float)(int)(*ptr) / 255f;
						break;
					case 1:
						yCbCr.Cb = (float)(int)(*ptr) / 255f - 0.5f;
						break;
					case 2:
						yCbCr.Cr = (float)(int)(*ptr) / 255f - 0.5f;
						break;
					}
					YCbCr.ToRGB(yCbCr, rGB);
					ptr2[2] = rGB.Red;
					ptr2[1] = rGB.Green;
					*ptr2 = rGB.Blue;
					num7++;
					ptr2 += num;
					ptr++;
				}
				ptr2 += num4;
				ptr += num6;
			}
			if (bitmapData != null)
			{
				channelImage.UnlockBits(bitmapData);
			}
		}
	}
}
