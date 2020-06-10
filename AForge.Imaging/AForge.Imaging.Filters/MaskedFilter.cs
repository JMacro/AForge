using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class MaskedFilter : BaseInPlacePartialFilter
	{
		private IFilter baseFilter;

		private Bitmap maskImage;

		private UnmanagedImage unmanagedMaskImage;

		private byte[,] mask;

		public IFilter BaseFilter
		{
			get
			{
				return baseFilter;
			}
			private set
			{
				if (value == null)
				{
					throw new NullReferenceException("Base filter can not be set to null.");
				}
				if (!(value is IFilterInformation))
				{
					throw new ArgumentException("The specified base filter must implement IFilterInformation interface.");
				}
				Dictionary<PixelFormat, PixelFormat> formatTranslations = ((IFilterInformation)value).FormatTranslations;
				foreach (KeyValuePair<PixelFormat, PixelFormat> item in formatTranslations)
				{
					if (item.Key != item.Value)
					{
						throw new ArgumentException("The specified filter must never change pixel format.");
					}
				}
				baseFilter = value;
			}
		}

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

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => ((IFilterInformation)baseFilter).FormatTranslations;

		public MaskedFilter(IFilter baseFiler, Bitmap maskImage)
		{
			BaseFilter = baseFiler;
			MaskImage = maskImage;
		}

		public MaskedFilter(IFilter baseFiler, UnmanagedImage unmanagedMaskImage)
		{
			BaseFilter = baseFiler;
			UnmanagedMaskImage = unmanagedMaskImage;
		}

		public MaskedFilter(IFilter baseFiler, byte[,] mask)
		{
			BaseFilter = baseFiler;
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
			UnmanagedImage unmanagedImage = baseFilter.Apply(image);
			if (image.Width != unmanagedImage.Width || image.Height != unmanagedImage.Height)
			{
				throw new ArgumentException("Base filter must not change image size.");
			}
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int top = rect.Top;
			int num2 = top + rect.Height;
			int left = rect.Left;
			int num3 = left + rect.Width;
			int stride = image.Stride;
			int stride2 = unmanagedImage.Stride;
			int num4 = maskLineSize - rect.Width;
			mask += maskLineSize * top + left;
			if (num <= 4 && num != 2)
			{
				byte* ptr = (byte*)image.ImageData.ToPointer() + (long)stride * (long)top + (long)num * (long)left;
				int num5 = stride - rect.Width * num;
				byte* ptr2 = (byte*)unmanagedImage.ImageData.ToPointer() + (long)stride2 * (long)top + (long)num * (long)left;
				int num6 = stride2 - rect.Width * num;
				switch (num)
				{
				case 2:
					break;
				case 1:
				{
					for (int j = top; j < num2; j++)
					{
						int num8 = left;
						while (num8 < num3)
						{
							if (*mask != 0)
							{
								*ptr = *ptr2;
							}
							num8++;
							ptr++;
							ptr2++;
							mask++;
						}
						ptr += num5;
						ptr2 += num6;
						mask += num4;
					}
					break;
				}
				case 3:
				{
					for (int k = top; k < num2; k++)
					{
						int num9 = left;
						while (num9 < num3)
						{
							if (*mask != 0)
							{
								ptr[2] = ptr2[2];
								ptr[1] = ptr2[1];
								*ptr = *ptr2;
							}
							num9++;
							ptr += 3;
							ptr2 += 3;
							mask++;
						}
						ptr += num5;
						ptr2 += num6;
						mask += num4;
					}
					break;
				}
				case 4:
				{
					for (int i = top; i < num2; i++)
					{
						int num7 = left;
						while (num7 < num3)
						{
							if (*mask != 0)
							{
								ptr[2] = ptr2[2];
								ptr[1] = ptr2[1];
								*ptr = *ptr2;
								ptr[3] = ptr2[3];
							}
							num7++;
							ptr += 4;
							ptr2 += 4;
							mask++;
						}
						ptr += num5;
						ptr2 += num6;
						mask += num4;
					}
					break;
				}
				}
				return;
			}
			byte* ptr3 = (byte*)image.ImageData.ToPointer() + (long)stride * (long)top + (long)num * (long)left;
			byte* ptr4 = (byte*)unmanagedImage.ImageData.ToPointer() + (long)stride2 * (long)top + (long)num * (long)left;
			switch (num)
			{
			case 2:
			{
				for (int m = top; m < num2; m++)
				{
					ushort* ptr7 = (ushort*)ptr3;
					ushort* ptr8 = (ushort*)ptr4;
					int num11 = left;
					while (num11 < num3)
					{
						if (*mask != 0)
						{
							*ptr7 = *ptr8;
						}
						num11++;
						ptr7++;
						ptr8++;
						mask++;
					}
					ptr3 += stride;
					ptr4 += stride2;
					mask += num4;
				}
				break;
			}
			case 6:
			{
				for (int n = top; n < num2; n++)
				{
					ushort* ptr9 = (ushort*)ptr3;
					ushort* ptr10 = (ushort*)ptr4;
					int num12 = left;
					while (num12 < num3)
					{
						if (*mask != 0)
						{
							ptr9[2] = ptr10[2];
							ptr9[1] = ptr10[1];
							*ptr9 = *ptr10;
						}
						num12++;
						ptr9 += 3;
						ptr10 += 3;
						mask++;
					}
					ptr3 += stride;
					ptr4 += stride2;
					mask += num4;
				}
				break;
			}
			case 8:
			{
				for (int l = top; l < num2; l++)
				{
					ushort* ptr5 = (ushort*)ptr3;
					ushort* ptr6 = (ushort*)ptr4;
					int num10 = left;
					while (num10 < num3)
					{
						if (*mask != 0)
						{
							ptr5[2] = ptr6[2];
							ptr5[1] = ptr6[1];
							*ptr5 = *ptr6;
							ptr5[3] = ptr6[3];
						}
						num10++;
						ptr5 += 4;
						ptr6 += 4;
						mask++;
					}
					ptr3 += stride;
					ptr4 += stride2;
					mask += num4;
				}
				break;
			}
			}
		}
	}
}
