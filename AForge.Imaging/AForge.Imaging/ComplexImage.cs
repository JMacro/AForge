using AForge.Math;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class ComplexImage : ICloneable
	{
		private Complex[,] data;

		private int width;

		private int height;

		private bool fourierTransformed;

		public int Width => width;

		public int Height => height;

		public bool FourierTransformed => fourierTransformed;

		public Complex[,] Data => data;

		protected ComplexImage(int width, int height)
		{
			this.width = width;
			this.height = height;
			data = new Complex[height, width];
			fourierTransformed = false;
		}

		public object Clone()
		{
			ComplexImage complexImage = new ComplexImage(width, height);
			Complex[,] array = complexImage.data;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					array[i, j] = data[i, j];
				}
			}
			complexImage.fourierTransformed = fourierTransformed;
			return complexImage;
		}

		public static ComplexImage FromBitmap(Bitmap image)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				throw new UnsupportedImageFormatException("Source image can be graysclae (8bpp indexed) image only.");
			}
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
			try
			{
				return FromBitmap(bitmapData);
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public unsafe static ComplexImage FromBitmap(BitmapData imageData)
		{
			if (imageData.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				throw new UnsupportedImageFormatException("Source image can be graysclae (8bpp indexed) image only.");
			}
			int num = imageData.Width;
			int num2 = imageData.Height;
			int num3 = imageData.Stride - num;
			if (!Tools.IsPowerOf2(num) || !Tools.IsPowerOf2(num2))
			{
				throw new InvalidImagePropertiesException("Image width and height should be power of 2.");
			}
			ComplexImage complexImage = new ComplexImage(num, num2);
			Complex[,] array = complexImage.data;
			byte* ptr = (byte*)imageData.Scan0.ToPointer();
			for (int i = 0; i < num2; i++)
			{
				int num4 = 0;
				while (num4 < num)
				{
					array[i, num4].Re = (float)(int)(*ptr) / 255f;
					num4++;
					ptr++;
				}
				ptr += num3;
			}
			return complexImage;
		}

		public unsafe Bitmap ToBitmap()
		{
			Bitmap bitmap = Image.CreateGrayscaleImage(width, height);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
			int num = bitmapData.Stride - width;
			double num2 = fourierTransformed ? System.Math.Sqrt(width * height) : 1.0;
			byte* ptr = (byte*)bitmapData.Scan0.ToPointer();
			for (int i = 0; i < height; i++)
			{
				int num3 = 0;
				while (num3 < width)
				{
					*ptr = (byte)System.Math.Max(0.0, System.Math.Min(255.0, data[i, num3].Magnitude * num2 * 255.0));
					num3++;
					ptr++;
				}
				ptr += num;
			}
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		public void ForwardFourierTransform()
		{
			if (fourierTransformed)
			{
				return;
			}
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					if (((j + i) & 1) != 0)
					{
						data[i, j].Re *= -1.0;
						data[i, j].Im *= -1.0;
					}
				}
			}
			FourierTransform.FFT2(data, FourierTransform.Direction.Forward);
			fourierTransformed = true;
		}

		public void BackwardFourierTransform()
		{
			if (!fourierTransformed)
			{
				return;
			}
			FourierTransform.FFT2(data, FourierTransform.Direction.Backward);
			fourierTransformed = false;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					if (((j + i) & 1) != 0)
					{
						data[i, j].Re *= -1.0;
						data[i, j].Im *= -1.0;
					}
				}
			}
		}
	}
}
