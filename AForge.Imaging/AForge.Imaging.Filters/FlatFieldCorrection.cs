using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class FlatFieldCorrection : BaseInPlaceFilter
	{
		private Bitmap backgroundImage;

		private UnmanagedImage unmanagedBackgroundImage;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public Bitmap BackgoundImage
		{
			get
			{
				return backgroundImage;
			}
			set
			{
				backgroundImage = value;
				if (value != null)
				{
					unmanagedBackgroundImage = null;
				}
			}
		}

		public UnmanagedImage UnmanagedBackgoundImage
		{
			get
			{
				return unmanagedBackgroundImage;
			}
			set
			{
				unmanagedBackgroundImage = value;
				if (value != null)
				{
					backgroundImage = null;
				}
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public FlatFieldCorrection()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		public FlatFieldCorrection(Bitmap backgroundImage)
			: this()
		{
			this.backgroundImage = backgroundImage;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image)
		{
			UnmanagedImage unmanagedImage = null;
			BitmapData bitmapData = null;
			int width = image.Width;
			int height = image.Height;
			int num = image.Stride - ((image.PixelFormat == PixelFormat.Format8bppIndexed) ? width : (width * 3));
			if (backgroundImage == null && unmanagedBackgroundImage == null)
			{
				ResizeBicubic resizeBicubic = new ResizeBicubic(width / 3, height / 3);
				UnmanagedImage unmanagedImage2 = resizeBicubic.Apply(image);
				GaussianBlur gaussianBlur = new GaussianBlur(5.0, 21);
				gaussianBlur.ApplyInPlace(unmanagedImage2);
				gaussianBlur.ApplyInPlace(unmanagedImage2);
				gaussianBlur.ApplyInPlace(unmanagedImage2);
				gaussianBlur.ApplyInPlace(unmanagedImage2);
				gaussianBlur.ApplyInPlace(unmanagedImage2);
				resizeBicubic.NewWidth = width;
				resizeBicubic.NewHeight = height;
				unmanagedImage = resizeBicubic.Apply(unmanagedImage2);
				unmanagedImage2.Dispose();
			}
			else if (backgroundImage != null)
			{
				if (width != backgroundImage.Width || height != backgroundImage.Height || image.PixelFormat != backgroundImage.PixelFormat)
				{
					throw new InvalidImagePropertiesException("Source image and background images must have the same size and pixel format");
				}
				bitmapData = backgroundImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, backgroundImage.PixelFormat);
				unmanagedImage = new UnmanagedImage(bitmapData);
			}
			else
			{
				unmanagedImage = unmanagedBackgroundImage;
			}
			ImageStatistics imageStatistics = new ImageStatistics(unmanagedImage);
			byte* ptr = (byte*)image.ImageData.ToPointer();
			byte* ptr2 = (byte*)unmanagedImage.ImageData.ToPointer();
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				double mean = imageStatistics.Gray.Mean;
				for (int i = 0; i < height; i++)
				{
					int num2 = 0;
					while (num2 < width)
					{
						if (*ptr2 != 0)
						{
							*ptr = (byte)System.Math.Min(mean * (double)(int)(*ptr) / (double)(int)(*ptr2), 255.0);
						}
						num2++;
						ptr++;
						ptr2++;
					}
					ptr += num;
					ptr2 += num;
				}
			}
			else
			{
				double mean2 = imageStatistics.Red.Mean;
				double mean3 = imageStatistics.Green.Mean;
				double mean4 = imageStatistics.Blue.Mean;
				for (int j = 0; j < height; j++)
				{
					int num3 = 0;
					while (num3 < width)
					{
						if (ptr2[2] != 0)
						{
							ptr[2] = (byte)System.Math.Min(mean2 * (double)(int)ptr[2] / (double)(int)ptr2[2], 255.0);
						}
						if (ptr2[1] != 0)
						{
							ptr[1] = (byte)System.Math.Min(mean3 * (double)(int)ptr[1] / (double)(int)ptr2[1], 255.0);
						}
						if (*ptr2 != 0)
						{
							*ptr = (byte)System.Math.Min(mean4 * (double)(int)(*ptr) / (double)(int)(*ptr2), 255.0);
						}
						num3++;
						ptr += 3;
						ptr2 += 3;
					}
					ptr += num;
					ptr2 += num;
				}
			}
			if (backgroundImage != null)
			{
				backgroundImage.UnlockBits(bitmapData);
			}
			if (backgroundImage == null && unmanagedBackgroundImage == null)
			{
				unmanagedImage.Dispose();
			}
		}
	}
}
