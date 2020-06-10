using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public abstract class BaseTransformationFilter : IFilter, IFilterInformation
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
			PixelFormat pixelFormat = FormatTranslations[imageData.PixelFormat];
			Size size = CalculateNewImageSize(new UnmanagedImage(imageData));
			Bitmap bitmap = (pixelFormat == PixelFormat.Format8bppIndexed) ? Image.CreateGrayscaleImage(size.Width, size.Height) : new Bitmap(size.Width, size.Height, pixelFormat);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, size.Width, size.Height), ImageLockMode.ReadWrite, pixelFormat);
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
			Size size = CalculateNewImageSize(image);
			UnmanagedImage unmanagedImage = UnmanagedImage.Create(size.Width, size.Height, FormatTranslations[image.PixelFormat]);
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
			Size size = CalculateNewImageSize(sourceImage);
			if (destinationImage.Width != size.Width || destinationImage.Height != size.Height)
			{
				throw new InvalidImagePropertiesException("Destination image must have the size expected by the filter.");
			}
			ProcessFilter(sourceImage, destinationImage);
		}

		protected abstract Size CalculateNewImageSize(UnmanagedImage sourceData);

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
