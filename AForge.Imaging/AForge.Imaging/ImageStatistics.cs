using AForge.Math;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class ImageStatistics
	{
		private Histogram red;

		private Histogram green;

		private Histogram blue;

		private Histogram gray;

		private Histogram redWithoutBlack;

		private Histogram greenWithoutBlack;

		private Histogram blueWithoutBlack;

		private Histogram grayWithoutBlack;

		private int pixels;

		private int pixelsWithoutBlack;

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

		public Histogram RedWithoutBlack
		{
			get
			{
				if (redWithoutBlack == null)
				{
					throw new InvalidImagePropertiesException("Cannot access color histogram since the last processed image was grayscale.");
				}
				return redWithoutBlack;
			}
		}

		public Histogram GreenWithoutBlack
		{
			get
			{
				if (greenWithoutBlack == null)
				{
					throw new InvalidImagePropertiesException("Cannot access color histogram since the last processed image was grayscale.");
				}
				return greenWithoutBlack;
			}
		}

		public Histogram BlueWithoutBlack
		{
			get
			{
				if (blueWithoutBlack == null)
				{
					throw new InvalidImagePropertiesException("Cannot access color histogram since the last processed image was grayscale.");
				}
				return blueWithoutBlack;
			}
		}

		public Histogram GrayWithoutBlack
		{
			get
			{
				if (grayWithoutBlack == null)
				{
					throw new InvalidImagePropertiesException("Cannot access gray histogram since the last processed image was color.");
				}
				return grayWithoutBlack;
			}
		}

		public int PixelsCount => pixels;

		public int PixelsCountWithoutBlack => pixelsWithoutBlack;

		public bool IsGrayscale => gray != null;

		public ImageStatistics(Bitmap image)
		{
			CheckSourceFormat(image.PixelFormat);
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			try
			{
				ProcessImage(new UnmanagedImage(bitmapData), null, 0);
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public unsafe ImageStatistics(Bitmap image, Bitmap mask)
		{
			CheckSourceFormat(image.PixelFormat);
			CheckMaskProperties(mask.PixelFormat, new Size(mask.Width, mask.Height), new Size(image.Width, image.Height));
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			BitmapData bitmapData2 = mask.LockBits(new Rectangle(0, 0, mask.Width, mask.Height), ImageLockMode.ReadOnly, mask.PixelFormat);
			try
			{
				ProcessImage(new UnmanagedImage(bitmapData), (byte*)bitmapData2.Scan0.ToPointer(), bitmapData2.Stride);
			}
			finally
			{
				image.UnlockBits(bitmapData);
				mask.UnlockBits(bitmapData2);
			}
		}

		public unsafe ImageStatistics(Bitmap image, byte[,] mask)
		{
			CheckSourceFormat(image.PixelFormat);
			CheckMaskProperties(PixelFormat.Format8bppIndexed, new Size(mask.GetLength(1), mask.GetLength(0)), new Size(image.Width, image.Height));
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			try
			{
				try
				{
					byte[,] array;
					reference = ref (array = mask) != null && array.Length != 0 ? ref array[0, 0] : ref *(byte*)null;
					ProcessImage(new UnmanagedImage(bitmapData), &reference, mask.GetLength(1));
				}
				finally
				{
					reference = 0u;
				}
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public ImageStatistics(UnmanagedImage image)
		{
			CheckSourceFormat(image.PixelFormat);
			ProcessImage(image, null, 0);
		}

		public unsafe ImageStatistics(UnmanagedImage image, UnmanagedImage mask)
		{
			CheckSourceFormat(image.PixelFormat);
			CheckMaskProperties(mask.PixelFormat, new Size(mask.Width, mask.Height), new Size(image.Width, image.Height));
			ProcessImage(image, (byte*)mask.ImageData.ToPointer(), mask.Stride);
		}

		public unsafe ImageStatistics(UnmanagedImage image, byte[,] mask)
		{
			CheckSourceFormat(image.PixelFormat);
			CheckMaskProperties(PixelFormat.Format8bppIndexed, new Size(mask.GetLength(1), mask.GetLength(0)), new Size(image.Width, image.Height));
			byte[,] array;
			reference = ref (array = mask) != null && array.Length != 0 ? ref array[0, 0] : ref *(byte*)null;
			ProcessImage(image, &reference, mask.GetLength(1));
			reference = 0u;
		}

		private unsafe void ProcessImage(UnmanagedImage image, byte* mask, int maskLineSize)
		{
			int width = image.Width;
			int height = image.Height;
			pixels = (pixelsWithoutBlack = 0);
			red = (green = (blue = (gray = null)));
			redWithoutBlack = (greenWithoutBlack = (blueWithoutBlack = (grayWithoutBlack = null)));
			int num = maskLineSize - width;
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				int[] array = new int[256];
				int[] array2 = new int[256];
				int num2 = image.Stride - width;
				byte* ptr = (byte*)image.ImageData.ToPointer();
				if (mask == null)
				{
					for (int i = 0; i < height; i++)
					{
						int num3 = 0;
						while (num3 < width)
						{
							byte b = *ptr;
							array[b]++;
							pixels++;
							if (b != 0)
							{
								array2[b]++;
								pixelsWithoutBlack++;
							}
							num3++;
							ptr++;
						}
						ptr += num2;
					}
				}
				else
				{
					for (int j = 0; j < height; j++)
					{
						int num4 = 0;
						while (num4 < width)
						{
							if (*mask != 0)
							{
								byte b = *ptr;
								array[b]++;
								pixels++;
								if (b != 0)
								{
									array2[b]++;
									pixelsWithoutBlack++;
								}
							}
							num4++;
							ptr++;
							mask++;
						}
						ptr += num2;
						mask += num;
					}
				}
				gray = new Histogram(array);
				grayWithoutBlack = new Histogram(array2);
				return;
			}
			int[] array3 = new int[256];
			int[] array4 = new int[256];
			int[] array5 = new int[256];
			int[] array6 = new int[256];
			int[] array7 = new int[256];
			int[] array8 = new int[256];
			int num5 = (image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
			int num6 = image.Stride - width * num5;
			byte* ptr2 = (byte*)image.ImageData.ToPointer();
			if (mask == null)
			{
				for (int k = 0; k < height; k++)
				{
					int num7 = 0;
					while (num7 < width)
					{
						byte b2 = ptr2[2];
						byte b3 = ptr2[1];
						byte b4 = *ptr2;
						array3[b2]++;
						array4[b3]++;
						array5[b4]++;
						pixels++;
						if (b2 != 0 || b3 != 0 || b4 != 0)
						{
							array6[b2]++;
							array7[b3]++;
							array8[b4]++;
							pixelsWithoutBlack++;
						}
						num7++;
						ptr2 += num5;
					}
					ptr2 += num6;
				}
			}
			else
			{
				for (int l = 0; l < height; l++)
				{
					int num8 = 0;
					while (num8 < width)
					{
						if (*mask != 0)
						{
							byte b2 = ptr2[2];
							byte b3 = ptr2[1];
							byte b4 = *ptr2;
							array3[b2]++;
							array4[b3]++;
							array5[b4]++;
							pixels++;
							if (b2 != 0 || b3 != 0 || b4 != 0)
							{
								array6[b2]++;
								array7[b3]++;
								array8[b4]++;
								pixelsWithoutBlack++;
							}
						}
						num8++;
						ptr2 += num5;
						mask++;
					}
					ptr2 += num6;
					mask += num;
				}
			}
			red = new Histogram(array3);
			green = new Histogram(array4);
			blue = new Histogram(array5);
			redWithoutBlack = new Histogram(array6);
			greenWithoutBlack = new Histogram(array7);
			blueWithoutBlack = new Histogram(array8);
		}

		private void CheckSourceFormat(PixelFormat pixelFormat)
		{
			if (pixelFormat != PixelFormat.Format8bppIndexed && pixelFormat != PixelFormat.Format24bppRgb && pixelFormat != PixelFormat.Format32bppRgb && pixelFormat != PixelFormat.Format32bppArgb)
			{
				throw new UnsupportedImageFormatException("Source pixel format is not supported.");
			}
		}

		private void CheckMaskProperties(PixelFormat maskFormat, Size maskSize, Size sourceImageSize)
		{
			if (maskFormat != PixelFormat.Format8bppIndexed)
			{
				throw new ArgumentException("Mask image must be 8 bpp grayscale image.");
			}
			if (maskSize.Width != sourceImageSize.Width || maskSize.Height != sourceImageSize.Height)
			{
				throw new ArgumentException("Mask must have the same size as the source image to get statistics for.");
			}
		}
	}
}
