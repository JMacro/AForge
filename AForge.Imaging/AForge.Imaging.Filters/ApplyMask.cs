using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ApplyMask : BaseInPlacePartialFilter
	{
		private Bitmap maskImage;

		private UnmanagedImage unmanagedMaskImage;

		private byte[,] mask;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public Bitmap MaskImage
		{
			get
			{
				return maskImage;
			}
			set
			{
				if (maskImage != null && maskImage.PixelFormat != PixelFormat.Format8bppIndexed)
				{
					throw new ArgumentException("The mask image must be 8 bpp grayscale image.");
				}
				maskImage = value;
				unmanagedMaskImage = null;
				mask = null;
			}
		}

		public UnmanagedImage UnmanagedMaskImage
		{
			get
			{
				return unmanagedMaskImage;
			}
			set
			{
				if (unmanagedMaskImage != null && unmanagedMaskImage.PixelFormat != PixelFormat.Format8bppIndexed)
				{
					throw new ArgumentException("The mask image must be 8 bpp grayscale image.");
				}
				unmanagedMaskImage = value;
				maskImage = null;
				mask = null;
			}
		}

		public byte[,] Mask
		{
			get
			{
				return mask;
			}
			set
			{
				mask = value;
				maskImage = null;
				unmanagedMaskImage = null;
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		private ApplyMask()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
			formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
			formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
			formatTranslations[PixelFormat.Format64bppPArgb] = PixelFormat.Format64bppPArgb;
		}

		public ApplyMask(Bitmap maskImage)
			: this()
		{
			MaskImage = maskImage;
		}

		public ApplyMask(UnmanagedImage unmanagedMaskImage)
			: this()
		{
			UnmanagedMaskImage = unmanagedMaskImage;
		}

		public ApplyMask(byte[,] mask)
			: this()
		{
			Mask = mask;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			if (mask != null)
			{
				if (image.Width != mask.GetLength(1) || image.Height != mask.GetLength(0))
				{
					throw new ArgumentException("Invalid size of mask array. Its size must be the same as the size of the image to mask.");
				}
				byte[,] array;
				reference = ref (array = mask) != null && array.Length != 0 ? ref array[0, 0] : ref *(byte*)null;
				ProcessImage(image, rect, &reference, mask.GetLength(1));
				reference = 0u;
				return;
			}
			if (unmanagedMaskImage != null)
			{
				if (image.Width != unmanagedMaskImage.Width || image.Height != unmanagedMaskImage.Height)
				{
					throw new ArgumentException("Invalid size of unmanaged mask image. Its size must be the same as the size of the image to mask.");
				}
				ProcessImage(image, rect, (byte*)unmanagedMaskImage.ImageData.ToPointer(), unmanagedMaskImage.Stride);
				return;
			}
			if (maskImage != null)
			{
				if (image.Width != maskImage.Width || image.Height != maskImage.Height)
				{
					throw new ArgumentException("Invalid size of mask image. Its size must be the same as the size of the image to mask.");
				}
				BitmapData bitmapData = maskImage.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
				try
				{
					ProcessImage(image, rect, (byte*)bitmapData.Scan0.ToPointer(), bitmapData.Stride);
				}
				finally
				{
					maskImage.UnlockBits(bitmapData);
				}
				return;
			}
			throw new NullReferenceException("None of the possible mask properties were set. Need to provide mask before applying the filter.");
		}

		private unsafe void ProcessImage(UnmanagedImage image, Rectangle rect, byte* mask, int maskLineSize)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int top = rect.Top;
			int num2 = top + rect.Height;
			int left = rect.Left;
			int num3 = left + rect.Width;
			int stride = image.Stride;
			int num4 = maskLineSize - rect.Width;
			mask += maskLineSize * top + left;
			if (num <= 4 && num != 2)
			{
				byte* ptr = (byte*)image.ImageData.ToPointer() + (long)stride * (long)top + (long)num * (long)left;
				int num5 = stride - rect.Width * num;
				switch (num)
				{
				case 2:
					break;
				case 1:
				{
					for (int j = top; j < num2; j++)
					{
						int num7 = left;
						while (num7 < num3)
						{
							if (*mask == 0)
							{
								*ptr = 0;
							}
							num7++;
							ptr++;
							mask++;
						}
						ptr += num5;
						mask += num4;
					}
					break;
				}
				case 3:
				{
					for (int k = top; k < num2; k++)
					{
						int num8 = left;
						while (num8 < num3)
						{
							if (*mask == 0)
							{
								ptr[2] = 0;
								ptr[1] = 0;
								*ptr = 0;
							}
							num8++;
							ptr += 3;
							mask++;
						}
						ptr += num5;
						mask += num4;
					}
					break;
				}
				case 4:
				{
					for (int i = top; i < num2; i++)
					{
						int num6 = left;
						while (num6 < num3)
						{
							if (*mask == 0)
							{
								ptr[2] = 0;
								ptr[1] = 0;
								*ptr = 0;
								ptr[3] = 0;
							}
							num6++;
							ptr += 4;
							mask++;
						}
						ptr += num5;
						mask += num4;
					}
					break;
				}
				}
				return;
			}
			byte* ptr2 = (byte*)image.ImageData.ToPointer() + (long)stride * (long)top + (long)num * (long)left;
			switch (num)
			{
			case 2:
			{
				for (int m = top; m < num2; m++)
				{
					ushort* ptr4 = (ushort*)ptr2;
					int num10 = left;
					while (num10 < num3)
					{
						if (*mask == 0)
						{
							*ptr4 = 0;
						}
						num10++;
						ptr4++;
						mask++;
					}
					ptr2 += stride;
					mask += num4;
				}
				break;
			}
			case 6:
			{
				for (int n = top; n < num2; n++)
				{
					ushort* ptr5 = (ushort*)ptr2;
					int num11 = left;
					while (num11 < num3)
					{
						if (*mask == 0)
						{
							ptr5[2] = 0;
							ptr5[1] = 0;
							*ptr5 = 0;
						}
						num11++;
						ptr5 += 3;
						mask++;
					}
					ptr2 += stride;
					mask += num4;
				}
				break;
			}
			case 8:
			{
				for (int l = top; l < num2; l++)
				{
					ushort* ptr3 = (ushort*)ptr2;
					int num9 = left;
					while (num9 < num3)
					{
						if (*mask == 0)
						{
							ptr3[2] = 0;
							ptr3[1] = 0;
							*ptr3 = 0;
							ptr3[3] = 0;
						}
						num9++;
						ptr3 += 4;
						mask++;
					}
					ptr2 += stride;
					mask += num4;
				}
				break;
			}
			}
		}
	}
}
