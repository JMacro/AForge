using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public abstract class BaseInPlacePartialFilter : IFilter, IInPlaceFilter, IInPlacePartialFilter, IFilterInformation
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
			PixelFormat pixelFormat = imageData.PixelFormat;
			CheckSourceFormat(pixelFormat);
			int width = imageData.Width;
			int height = imageData.Height;
			Bitmap bitmap = (pixelFormat == PixelFormat.Format8bppIndexed) ? Image.CreateGrayscaleImage(width, height) : new Bitmap(width, height, pixelFormat);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, pixelFormat);
			SystemTools.CopyUnmanagedMemory(bitmapData.Scan0, imageData.Scan0, imageData.Stride * height);
			try
			{
				ProcessFilter(new UnmanagedImage(bitmapData), new Rectangle(0, 0, width, height));
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
			UnmanagedImage unmanagedImage = UnmanagedImage.Create(image.Width, image.Height, image.PixelFormat);
			Apply(image, unmanagedImage);
			return unmanagedImage;
		}

		public unsafe void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
		{
			CheckSourceFormat(sourceImage.PixelFormat);
			if (destinationImage.PixelFormat != sourceImage.PixelFormat)
			{
				throw new InvalidImagePropertiesException("Destination pixel format must be the same as pixel format of source image.");
			}
			if (destinationImage.Width != sourceImage.Width || destinationImage.Height != sourceImage.Height)
			{
				throw new InvalidImagePropertiesException("Destination image must have the same width and height as source image.");
			}
			int stride = destinationImage.Stride;
			int stride2 = sourceImage.Stride;
			int count = System.Math.Min(stride2, stride);
			byte* ptr = (byte*)destinationImage.ImageData.ToPointer();
			byte* ptr2 = (byte*)sourceImage.ImageData.ToPointer();
			int i = 0;
			for (int height = sourceImage.Height; i < height; i++)
			{
				SystemTools.CopyUnmanagedMemory(ptr, ptr2, count);
				ptr += stride;
				ptr2 += stride2;
			}
			ProcessFilter(destinationImage, new Rectangle(0, 0, destinationImage.Width, destinationImage.Height));
		}

		public void ApplyInPlace(Bitmap image)
		{
			ApplyInPlace(image, new Rectangle(0, 0, image.Width, image.Height));
		}

		public void ApplyInPlace(BitmapData imageData)
		{
			CheckSourceFormat(imageData.PixelFormat);
			ProcessFilter(new UnmanagedImage(imageData), new Rectangle(0, 0, imageData.Width, imageData.Height));
		}

		public void ApplyInPlace(UnmanagedImage image)
		{
			CheckSourceFormat(image.PixelFormat);
			ProcessFilter(image, new Rectangle(0, 0, image.Width, image.Height));
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
				ProcessFilter(image, rect);
			}
		}

		protected abstract void ProcessFilter(UnmanagedImage image, Rectangle rect);

		private void CheckSourceFormat(PixelFormat pixelFormat)
		{
			if (!FormatTranslations.ContainsKey(pixelFormat))
			{
				throw new UnsupportedImageFormatException("Source pixel format is not supported by the filter.");
			}
		}
	}
}
