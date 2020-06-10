using AForge.Math;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class VerticalIntensityStatistics
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

		public VerticalIntensityStatistics(Bitmap image)
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

		public VerticalIntensityStatistics(BitmapData imageData)
			: this(new UnmanagedImage(imageData))
		{
		}

		public VerticalIntensityStatistics(UnmanagedImage image)
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
				int num6 = image.Stride - width;
				int[] array4 = new int[height];
				for (int j = 0; j < height; j++)
				{
					int num7 = 0;
					int num8 = 0;
					while (num8 < width)
					{
						num7 += *ptr3;
						num8++;
						ptr3++;
					}
					array4[j] = num7;
					ptr3 += num6;
				}
				gray = new Histogram(array4);
				break;
			}
			case PixelFormat.Format16bppGrayScale:
			{
				byte* ptr5 = (byte*)image.ImageData.ToPointer();
				int stride2 = image.Stride;
				int[] array8 = new int[height];
				for (int l = 0; l < height; l++)
				{
					ushort* ptr6 = (ushort*)(ptr5 + (long)stride2 * (long)l);
					int num15 = 0;
					int num16 = 0;
					while (num16 < width)
					{
						num15 += *ptr6;
						num16++;
						ptr6++;
					}
					array8[l] = num15;
				}
				gray = new Histogram(array8);
				break;
			}
			case PixelFormat.Format24bppRgb:
			case PixelFormat.Format32bppRgb:
			case PixelFormat.Format32bppArgb:
			{
				byte* ptr4 = (byte*)image.ImageData.ToPointer();
				int num9 = (pixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
				int num10 = image.Stride - width * num9;
				int[] array5 = new int[height];
				int[] array6 = new int[height];
				int[] array7 = new int[height];
				for (int k = 0; k < height; k++)
				{
					int num11 = 0;
					int num12 = 0;
					int num13 = 0;
					int num14 = 0;
					while (num14 < width)
					{
						num11 += ptr4[2];
						num12 += ptr4[1];
						num13 += *ptr4;
						num14++;
						ptr4 += num9;
					}
					array5[k] = num11;
					array6[k] = num12;
					array7[k] = num13;
					ptr4 += num10;
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
				int[] array = new int[height];
				int[] array2 = new int[height];
				int[] array3 = new int[height];
				for (int i = 0; i < height; i++)
				{
					ushort* ptr2 = (ushort*)(ptr + (long)stride * (long)i);
					int num2 = 0;
					int num3 = 0;
					int num4 = 0;
					int num5 = 0;
					while (num5 < width)
					{
						num2 += ptr2[2];
						num3 += ptr2[1];
						num4 += *ptr2;
						num5++;
						ptr2 += num;
					}
					array[i] = num2;
					array2[i] = num3;
					array3[i] = num4;
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
