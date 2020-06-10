using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public abstract class BaseFilter2 : BaseFilter
	{
		private Bitmap overlayImage;

		private UnmanagedImage unmanagedOverlayImage;

		public Bitmap OverlayImage
		{
			get
			{
				return overlayImage;
			}
			set
			{
				overlayImage = value;
				if (value != null)
				{
					unmanagedOverlayImage = null;
				}
			}
		}

		public UnmanagedImage UnmanagedOverlayImage
		{
			get
			{
				return unmanagedOverlayImage;
			}
			set
			{
				unmanagedOverlayImage = value;
				if (value != null)
				{
					overlayImage = null;
				}
			}
		}

		protected BaseFilter2()
		{
		}

		protected BaseFilter2(Bitmap overlayImage)
		{
			this.overlayImage = overlayImage;
		}

		protected BaseFilter2(UnmanagedImage unmanagedOverlayImage)
		{
			this.unmanagedOverlayImage = unmanagedOverlayImage;
		}

		protected override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			PixelFormat pixelFormat = sourceData.PixelFormat;
			int width = sourceData.Width;
			int height = sourceData.Height;
			if (overlayImage != null)
			{
				if (pixelFormat != overlayImage.PixelFormat)
				{
					throw new InvalidImagePropertiesException("Source and overlay images must have same pixel format.");
				}
				if (width != overlayImage.Width || height != overlayImage.Height)
				{
					throw new InvalidImagePropertiesException("Overlay image size must be equal to source image size.");
				}
				BitmapData bitmapData = overlayImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, pixelFormat);
				try
				{
					ProcessFilter(sourceData, new UnmanagedImage(bitmapData), destinationData);
				}
				finally
				{
					overlayImage.UnlockBits(bitmapData);
				}
				return;
			}
			if (unmanagedOverlayImage != null)
			{
				if (pixelFormat != unmanagedOverlayImage.PixelFormat)
				{
					throw new InvalidImagePropertiesException("Source and overlay images must have same pixel format.");
				}
				if (width != unmanagedOverlayImage.Width || height != unmanagedOverlayImage.Height)
				{
					throw new InvalidImagePropertiesException("Overlay image size must be equal to source image size.");
				}
				ProcessFilter(sourceData, unmanagedOverlayImage, destinationData);
				return;
			}
			throw new NullReferenceException("Overlay image is not set.");
		}

		protected abstract void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage overlay, UnmanagedImage destinationData);
	}
}
