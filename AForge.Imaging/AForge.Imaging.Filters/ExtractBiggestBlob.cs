using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ExtractBiggestBlob : IFilter, IFilterInformation
	{
		private Bitmap originalImage;

		private IntPoint blobPosition;

		public IntPoint BlobPosition => blobPosition;

		public Dictionary<PixelFormat, PixelFormat> FormatTranslations
		{
			get
			{
				Dictionary<PixelFormat, PixelFormat> dictionary = new Dictionary<PixelFormat, PixelFormat>();
				if (originalImage == null)
				{
					dictionary[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
					dictionary[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
					dictionary[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
					dictionary[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
					dictionary[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
				}
				else
				{
					dictionary[PixelFormat.Format8bppIndexed] = originalImage.PixelFormat;
					dictionary[PixelFormat.Format24bppRgb] = originalImage.PixelFormat;
					dictionary[PixelFormat.Format32bppArgb] = originalImage.PixelFormat;
					dictionary[PixelFormat.Format32bppRgb] = originalImage.PixelFormat;
					dictionary[PixelFormat.Format32bppPArgb] = originalImage.PixelFormat;
				}
				return dictionary;
			}
		}

		public Bitmap OriginalImage
		{
			get
			{
				return originalImage;
			}
			set
			{
				originalImage = value;
			}
		}

		public Bitmap Apply(Bitmap image)
		{
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			Bitmap bitmap = null;
			try
			{
				return Apply(bitmapData);
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public Bitmap Apply(BitmapData imageData)
		{
			if (!FormatTranslations.ContainsKey(imageData.PixelFormat))
			{
				throw new UnsupportedImageFormatException("Source pixel format is not supported by the filter.");
			}
			BlobCounter blobCounter = new BlobCounter(imageData);
			Blob[] objectsInformation = blobCounter.GetObjectsInformation();
			int num = 0;
			Blob blob = null;
			int i = 0;
			for (int num2 = objectsInformation.Length; i < num2; i++)
			{
				int num3 = objectsInformation[i].Rectangle.Width * objectsInformation[i].Rectangle.Height;
				if (num3 > num)
				{
					num = num3;
					blob = objectsInformation[i];
				}
			}
			if (blob == null)
			{
				throw new ArgumentException("The source image does not contain any blobs.");
			}
			blobPosition = new IntPoint(blob.Rectangle.Left, blob.Rectangle.Top);
			if (originalImage == null)
			{
				blobCounter.ExtractBlobsImage(new UnmanagedImage(imageData), blob, extractInOriginalSize: false);
			}
			else
			{
				if (originalImage.PixelFormat != PixelFormat.Format24bppRgb && originalImage.PixelFormat != PixelFormat.Format32bppArgb && originalImage.PixelFormat != PixelFormat.Format32bppRgb && originalImage.PixelFormat != PixelFormat.Format32bppPArgb && originalImage.PixelFormat != PixelFormat.Format8bppIndexed)
				{
					throw new UnsupportedImageFormatException("Original image may be grayscale (8bpp indexed) or color (24/32bpp) image only.");
				}
				if (originalImage.Width != imageData.Width || originalImage.Height != imageData.Height)
				{
					throw new InvalidImagePropertiesException("Original image must have the same size as passed source image.");
				}
				blobCounter.ExtractBlobsImage(originalImage, blob, extractInOriginalSize: false);
			}
			Bitmap result = blob.Image.ToManagedImage();
			blob.Image.Dispose();
			return result;
		}

		public UnmanagedImage Apply(UnmanagedImage image)
		{
			throw new NotImplementedException("The method is not implemented for the filter.");
		}

		public void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
		{
			throw new NotImplementedException("The method is not implemented filter.");
		}
	}
}
