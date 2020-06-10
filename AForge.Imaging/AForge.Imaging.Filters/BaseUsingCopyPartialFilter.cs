using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public abstract class BaseUsingCopyPartialFilter : IFilter, IInPlaceFilter, IInPlacePartialFilter, IFilterInformation
	{
		public abstract Dictionary<PixelFormat, PixelFormat> FormatTranslations
		{
			get;
		}

		public Bitmap Apply(Bitmap image)
		{
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			Bitmap bitmap = null;
			try
			{
				bitmap = Apply(bitmapData);
				if (!(image.HorizontalResolution > 0f))
				{
					return bitmap;
				}
				if (!(image.VerticalResolution > 0f))
				{
					return bitmap;
				}
				bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
				return bitmap;
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public Bitmap Apply(BitmapData imageData)
		{
			CheckSourceFormat(imageData.PixelFormat);
			int width = imageData.Width;
			int height = imageData.Height;
			PixelFormat pixelFormat = FormatTranslations[imageData.PixelFormat];
			Bitmap bitmap = (pixelFormat == PixelFormat.Format8bppIndexed) ? Image.CreateGrayscaleImage(width, height) : new Bitmap(width, height, pixelFormat);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, pixelFormat);
			try
			{
				ProcessFilter(new UnmanagedImage(imageData), new UnmanagedImage(bitmapData), new Rectangle(0, 0, width, height));
				return bitmap;
			}
			finally
			{
				bitmap.UnlockBits(bitmapData);
			}
		}

		public UnmanagedImage Apply(UnmanagedImage image)
		{
			CheckSourceFormat(image.PixelFormat);
			UnmanagedImage unmanagedImage = UnmanagedImage.Create(image.Width, image.Height, FormatTranslations[image.PixelFormat]);
			ProcessFilter(image, unmanagedImage, new Rectangle(0, 0, image.Width, image.Height));
			return unmanagedImage;
		}

		public void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
		{
			CheckSourceFormat(sourceImage.PixelFormat);
			if (destinationImage.PixelFormat != FormatTranslations[sourceImage.PixelFormat])
			{
				throw new InvalidImagePropertiesException("Destination pixel format is specified incorrectly.");
			}
			if (destinationImage.Width != sourceImage.Width || destinationImage.Height != sourceImage.Height)
			{
				throw new InvalidImagePropertiesException("Destination image must have the same width and height as source image.");
			}
			ProcessFilter(sourceImage, destinationImage, new Rectangle(0, 0, sourceImage.Width, sourceImage.Height));
		}

		public void ApplyInPlace(Bitmap image)
		{
			ApplyInPlace(image, new Rectangle(0, 0, image.Width, image.Height));
		}

		public void ApplyInPlace(BitmapData imageData)
		{
			ApplyInPlace(new UnmanagedImage(imageData), new Rectangle(0, 0, imageData.Width, imageData.Height));
		}

		public void ApplyInPlace(UnmanagedImage image)
		{
			ApplyInPlace(image, new Rectangle(0, 0, image.Width, image.Height));
		}

		public void ApplyInPlace(Bitmap image, Rectangle rect)
		{
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
			try
			{
				ApplyInPlace(new UnmanagedImage(bitmapData), rect);
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public void ApplyInPlace(BitmapData imageData, Rectangle rect)
		{
			ApplyInPlace(new UnmanagedImage(imageData), rect);
		}

		public void ApplyInPlace(UnmanagedImage image, Rectangle rect)
		{
			CheckSourceFormat(image.PixelFormat);
			rect.Intersect(new Rectangle(0, 0, image.Width, image.Height));
			if ((rect.Width | rect.Height) != 0)
			{
				int num = image.Stride * image.Height;
				IntPtr intPtr = MemoryManager.Alloc(num);
				SystemTools.CopyUnmanagedMemory(intPtr, image.ImageData, num);
				ProcessFilter(new UnmanagedImage(intPtr, image.Width, image.Height, image.Stride, image.PixelFormat), image, rect);
				MemoryManager.Free(intPtr);
			}
		}

		protected abstract void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData, Rectangle rect);

		private void CheckSourceFormat(PixelFormat pixelFormat)
		{
			if (!FormatTranslations.ContainsKey(pixelFormat))
			{
				throw new UnsupportedImageFormatException("Source pixel format is not supported by the filter.");
			}
		}
	}
}
