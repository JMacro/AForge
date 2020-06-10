using AForge.Math;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class ImageStatisticsYCbCr
	{
		private ContinuousHistogram yHistogram;

		private ContinuousHistogram cbHistogram;

		private ContinuousHistogram crHistogram;

		private ContinuousHistogram yHistogramWithoutBlack;

		private ContinuousHistogram cbHistogramWithoutBlack;

		private ContinuousHistogram crHistogramWithoutBlack;

		private int pixels;

		private int pixelsWithoutBlack;

		public ContinuousHistogram Y => yHistogram;

		public ContinuousHistogram Cb => cbHistogram;

		public ContinuousHistogram Cr => crHistogram;

		public ContinuousHistogram YWithoutBlack => yHistogramWithoutBlack;

		public ContinuousHistogram CbWithoutBlack => cbHistogramWithoutBlack;

		public ContinuousHistogram CrWithoutBlack => crHistogramWithoutBlack;

		public int PixelsCount => pixels;

		public int PixelsCountWithoutBlack => pixelsWithoutBlack;

		public ImageStatisticsYCbCr(Bitmap image)
		{
			CheckSourceFormat(image.PixelFormat);
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
			try
			{
				ProcessImage(new UnmanagedImage(bitmapData), null, 0);
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public unsafe ImageStatisticsYCbCr(Bitmap image, Bitmap mask)
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

		public unsafe ImageStatisticsYCbCr(Bitmap image, byte[,] mask)
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

		public ImageStatisticsYCbCr(UnmanagedImage image)
		{
			CheckSourceFormat(image.PixelFormat);
			ProcessImage(image, null, 0);
		}

		public unsafe ImageStatisticsYCbCr(UnmanagedImage image, UnmanagedImage mask)
		{
			CheckSourceFormat(image.PixelFormat);
			CheckMaskProperties(mask.PixelFormat, new Size(mask.Width, mask.Height), new Size(image.Width, image.Height));
			ProcessImage(image, (byte*)mask.ImageData.ToPointer(), mask.Stride);
		}

		public unsafe ImageStatisticsYCbCr(UnmanagedImage image, byte[,] mask)
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
			int[] array = new int[256];
			int[] array2 = new int[256];
			int[] array3 = new int[256];
			int[] array4 = new int[256];
			int[] array5 = new int[256];
			int[] array6 = new int[256];
			RGB rGB = new RGB();
			YCbCr yCbCr = new YCbCr();
			int num = (image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
			int num2 = image.Stride - width * num;
			int num3 = maskLineSize - width;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			if (mask == null)
			{
				for (int i = 0; i < height; i++)
				{
					int num4 = 0;
					while (num4 < width)
					{
						rGB.Red = ptr[2];
						rGB.Green = ptr[1];
						rGB.Blue = *ptr;
						YCbCr.FromRGB(rGB, yCbCr);
						array[(int)(yCbCr.Y * 255f)]++;
						array2[(int)(((double)yCbCr.Cb + 0.5) * 255.0)]++;
						array3[(int)(((double)yCbCr.Cr + 0.5) * 255.0)]++;
						pixels++;
						if ((double)yCbCr.Y != 0.0 || (double)yCbCr.Cb != 0.0 || (double)yCbCr.Cr != 0.0)
						{
							array4[(int)(yCbCr.Y * 255f)]++;
							array5[(int)(((double)yCbCr.Cb + 0.5) * 255.0)]++;
							array6[(int)(((double)yCbCr.Cr + 0.5) * 255.0)]++;
							pixelsWithoutBlack++;
						}
						num4++;
						ptr += num;
					}
					ptr += num2;
				}
			}
			else
			{
				for (int j = 0; j < height; j++)
				{
					int num5 = 0;
					while (num5 < width)
					{
						if (*mask != 0)
						{
							rGB.Red = ptr[2];
							rGB.Green = ptr[1];
							rGB.Blue = *ptr;
							YCbCr.FromRGB(rGB, yCbCr);
							array[(int)(yCbCr.Y * 255f)]++;
							array2[(int)(((double)yCbCr.Cb + 0.5) * 255.0)]++;
							array3[(int)(((double)yCbCr.Cr + 0.5) * 255.0)]++;
							pixels++;
							if ((double)yCbCr.Y != 0.0 || (double)yCbCr.Cb != 0.0 || (double)yCbCr.Cr != 0.0)
							{
								array4[(int)(yCbCr.Y * 255f)]++;
								array5[(int)(((double)yCbCr.Cb + 0.5) * 255.0)]++;
								array6[(int)(((double)yCbCr.Cr + 0.5) * 255.0)]++;
								pixelsWithoutBlack++;
							}
						}
						num5++;
						ptr += num;
						mask++;
					}
					ptr += num2;
					mask += num3;
				}
			}
			yHistogram = new ContinuousHistogram(array, new Range(0f, 1f));
			cbHistogram = new ContinuousHistogram(array2, new Range(-0.5f, 0.5f));
			crHistogram = new ContinuousHistogram(array3, new Range(-0.5f, 0.5f));
			yHistogramWithoutBlack = new ContinuousHistogram(array4, new Range(0f, 1f));
			cbHistogramWithoutBlack = new ContinuousHistogram(array5, new Range(-0.5f, 0.5f));
			crHistogramWithoutBlack = new ContinuousHistogram(array6, new Range(-0.5f, 0.5f));
		}

		private void CheckSourceFormat(PixelFormat pixelFormat)
		{
			if (pixelFormat != PixelFormat.Format24bppRgb && pixelFormat != PixelFormat.Format32bppRgb && pixelFormat != PixelFormat.Format32bppArgb)
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
