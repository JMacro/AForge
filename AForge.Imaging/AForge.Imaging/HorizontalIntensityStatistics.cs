using AForge.Math;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class HorizontalIntensityStatistics
	{
		private Histogram red;

		private Histogram green;

		private Histogram blue;

		private Histogram gray;

		public Histogram Red
		{
			get
			{
				if (red == null)
				{
					throw new InvalidImagePropertiesException("Cannot access color histogram since the last processed image was grayscale.");
				}
				return red;
			}
		}

		public Histogram Green
		{
			get
			{
				if (green == null)
				{
					throw new InvalidImagePropertiesException("Cannot access color histogram since the last processed image was grayscale.");
				}
				return green;
			}
		}

		public Histogram Blue
		{
			get
			{
				if (blue == null)
				{
					throw new InvalidImagePropertiesException("Cannot access color histogram since the last processed image was grayscale.");
				}
				return blue;
			}
		}

		public Histogram Gray
		{
			get
			{
				if (gray == null)
				{
					throw new InvalidImagePropertiesException("Cannot access gray histogram since the last processed image was color.");
				}
				return gray;
			}
		}

		public bool IsGrayscale => gray != null;

		public HorizontalIntensityStatistics(Bitmap image)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed && image.PixelFormat != PixelFormat.Format16bppGrayScale && image.PixelFormat != PixelFormat.Format24bppRgb && image.PixelFormat != PixelFormat.Format32bppRgb && image.PixelFormat != PixelFormat.Format32bppArgb && image.PixelFormat != PixelFormat.Format48bppRgb && image.PixelFormat != PixelFormat.Format64bppArgb)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			try
			{
				ProcessImage(new UnmanagedImage(bitmapData));
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public HorizontalIntensityStatistics(BitmapData imageData)
			: this(new UnmanagedImage(imageData))
		{
		}

		public HorizontalIntensityStatistics(UnmanagedImage image)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed && image.PixelFormat != PixelFormat.Format16bppGrayScale && image.PixelFormat != PixelFormat.Format24bppRgb && image.PixelFormat != PixelFormat.Format32bppRgb && image.PixelFormat != PixelFormat.Format32bppArgb && image.PixelFormat != PixelFormat.Format48bppRgb && image.PixelFormat != PixelFormat.Format64bppArgb)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			ProcessImage(image);
		}

		private unsafe void ProcessImage(UnmanagedImage image)
		{
			PixelFormat pixelFormat = image.PixelFormat;
			int width = image.Width;
			int height = image.Height;
			red = (green = (blue = (gray = null)));
			switch (pixelFormat)
			{
			case PixelFormat.Format8bppIndexed:
			{
				byte* ptr3 = (byte*)image.ImageData.ToPointer();
				int num3 = image.Stride - width;
				int[] array4 = new int[width];
				for (int j = 0; j < height; j++)
				{
					int num4 = 0;
					while (num4 < width)
					{
						array4[num4] += *ptr3;
						num4++;
						ptr3++;
					}
					ptr3 += num3;
				}
				gray = new Histogram(array4);
				break;
			}
			case PixelFormat.Format16bppGrayScale:
			{
				byte* ptr5 = (byte*)image.ImageData.ToPointer();
				int stride2 = image.Stride;
				int[] array8 = new int[width];
				for (int l = 0; l < height; l++)
				{
					ushort* ptr6 = (ushort*)(ptr5 + (long)stride2 * (long)l);
					int num8 = 0;
					while (num8 < width)
					{
						array8[num8] += *ptr6;
						num8++;
						ptr6++;
					}
				}
				gray = new Histogram(array8);
				break;
			}
			case PixelFormat.Format24bppRgb:
			case PixelFormat.Format32bppRgb:
			case PixelFormat.Format32bppArgb:
			{
				byte* ptr4 = (byte*)image.ImageData.ToPointer();
				int num5 = (pixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
				int num6 = image.Stride - width * num5;
				int[] array5 = new int[width];
				int[] array6 = new int[width];
				int[] array7 = new int[width];
				for (int k = 0; k < height; k++)
				{
					int num7 = 0;
					while (num7 < width)
					{
						array5[num7] += ptr4[2];
						array6[num7] += ptr4[1];
						array7[num7] += *ptr4;
						num7++;
						ptr4 += num5;
					}
					ptr4 += num6;
				}
				red = new Histogram(array5);
				green = new Histogram(array6);
				blue = new Histogram(array7);
				break;
			}
			case PixelFormat.Format48bppRgb:
			case PixelFormat.Format64bppArgb:
			{
				byte* ptr = (byte*)image.ImageData.ToPointer();
				int stride = image.Stride;
				int num = (pixelFormat == PixelFormat.Format48bppRgb) ? 3 : 4;
				int[] array = new int[width];
				int[] array2 = new int[width];
				int[] array3 = new int[width];
				for (int i = 0; i < height; i++)
				{
					ushort* ptr2 = (ushort*)(ptr + (long)stride * (long)i);
					int num2 = 0;
					while (num2 < width)
					{
						array[num2] += ptr2[2];
						array2[num2] += ptr2[1];
						array3[num2] += *ptr2;
						num2++;
						ptr2 += num;
					}
				}
				red = new Histogram(array);
				green = new Histogram(array2);
				blue = new Histogram(array3);
				break;
			}
			}
		}
	}
}
