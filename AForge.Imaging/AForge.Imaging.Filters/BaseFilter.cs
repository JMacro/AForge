using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public abstract class BaseFilter : IFilter, IFilterInformation
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
				ProcessFilter(new UnmanagedImage(imageData), new UnmanagedImage(bitmapData));
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
			ProcessFilter(image, unmanagedImage);
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
			ProcessFilter(sourceImage, destinationImage);
		}

		protected abstract void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData);

		private void CheckSourceFormat(PixelFormat pixelFormat)
		{
			if (!FormatTranslations.ContainsKey(pixelFormat))
			{
				throw new UnsupportedImageFormatException("Source pixel format is not supported by the filter.");
			}
		}
	}
}
