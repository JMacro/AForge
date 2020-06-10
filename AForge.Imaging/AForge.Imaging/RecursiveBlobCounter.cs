using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class RecursiveBlobCounter : BlobCounterBase
	{
		private int[] tempLabels;

		private int stride;

		private int pixelSize;

		private byte backgroundThresholdR;

		private byte backgroundThresholdG;

		private byte backgroundThresholdB;

		public Color BackgroundThreshold
		{
			get
			{
				return Color.FromArgb(backgroundThresholdR, backgroundThresholdG, backgroundThresholdB);
			}
			set
			{
				backgroundThresholdR = value.R;
				backgroundThresholdG = value.G;
				backgroundThresholdB = value.B;
			}
		}

		public RecursiveBlobCounter()
		{
		}

		public RecursiveBlobCounter(Bitmap image)
			: base(image)
		{
		}

		public RecursiveBlobCounter(BitmapData imageData)
			: base(imageData)
		{
		}

		public RecursiveBlobCounter(UnmanagedImage image)
			: base(image)
		{
		}

		protected unsafe override void BuildObjectsMap(UnmanagedImage image)
		{
			stride = image.Stride;
			if (image.PixelFormat != PixelFormat.Format8bppIndexed && image.PixelFormat != PixelFormat.Format24bppRgb && image.PixelFormat != PixelFormat.Format32bppRgb && image.PixelFormat != PixelFormat.Format32bppArgb && image.PixelFormat != PixelFormat.Format32bppPArgb)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			tempLabels = new int[(imageWidth + 2) * (imageHeight + 2)];
			int i = 0;
			for (int num = imageWidth + 2; i < num; i++)
			{
				tempLabels[i] = -1;
				tempLabels[i + (imageHeight + 1) * (imageWidth + 2)] = -1;
			}
			int j = 0;
			for (int num2 = imageHeight + 2; j < num2; j++)
			{
				tempLabels[j * (imageWidth + 2)] = -1;
				tempLabels[j * (imageWidth + 2) + imageWidth + 1] = -1;
			}
			objectsCount = 0;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			int num3 = imageWidth + 2 + 1;
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				int num4 = stride - imageWidth;
				for (int k = 0; k < imageHeight; k++)
				{
					int num5 = 0;
					while (num5 < imageWidth)
					{
						if (*ptr > backgroundThresholdG && tempLabels[num3] == 0)
						{
							objectsCount++;
							LabelPixel(ptr, num3);
						}
						num5++;
						ptr++;
						num3++;
					}
					ptr += num4;
					num3 += 2;
				}
			}
			else
			{
				pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
				int num6 = stride - imageWidth * pixelSize;
				for (int l = 0; l < imageHeight; l++)
				{
					int num7 = 0;
					while (num7 < imageWidth)
					{
						if ((ptr[2] > backgroundThresholdR || ptr[1] > backgroundThresholdG || *ptr > backgroundThresholdB) && tempLabels[num3] == 0)
						{
							objectsCount++;
							LabelColorPixel(ptr, num3);
						}
						num7++;
						ptr += pixelSize;
						num3++;
					}
					ptr += num6;
					num3 += 2;
				}
			}
			objectLabels = new int[imageWidth * imageHeight];
			for (int m = 0; m < imageHeight; m++)
			{
				Array.Copy(tempLabels, (m + 1) * (imageWidth + 2) + 1, objectLabels, m * imageWidth, imageWidth);
			}
		}

		private unsafe void LabelPixel(byte* pixel, int labelPointer)
		{
			if (tempLabels[labelPointer] == 0 && *pixel > backgroundThresholdG)
			{
				tempLabels[labelPointer] = objectsCount;
				LabelPixel(pixel + 1, labelPointer + 1);
				LabelPixel(pixel + 1 + stride, labelPointer + 1 + 2 + imageWidth);
				LabelPixel(pixel + stride, labelPointer + 2 + imageWidth);
				LabelPixel(pixel - 1 + stride, labelPointer - 1 + 2 + imageWidth);
				LabelPixel(pixel - 1, labelPointer - 1);
				LabelPixel(pixel - 1 - stride, labelPointer - 1 - 2 - imageWidth);
				LabelPixel(pixel - stride, labelPointer - 2 - imageWidth);
				LabelPixel(pixel + 1 - stride, labelPointer + 1 - 2 - imageWidth);
			}
		}

		private unsafe void LabelColorPixel(byte* pixel, int labelPointer)
		{
			if (tempLabels[labelPointer] == 0 && (pixel[2] > backgroundThresholdR || pixel[1] > backgroundThresholdG || *pixel > backgroundThresholdB))
			{
				tempLabels[labelPointer] = objectsCount;
				LabelColorPixel(pixel + pixelSize, labelPointer + 1);
				LabelColorPixel(pixel + pixelSize + stride, labelPointer + 1 + 2 + imageWidth);
				LabelColorPixel(pixel + stride, labelPointer + 2 + imageWidth);
				LabelColorPixel(pixel - pixelSize + stride, labelPointer - 1 + 2 + imageWidth);
				LabelColorPixel(pixel - pixelSize, labelPointer - 1);
				LabelColorPixel(pixel - pixelSize - stride, labelPointer - 1 - 2 - imageWidth);
				LabelColorPixel(pixel - stride, labelPointer - 2 - imageWidth);
				LabelColorPixel(pixel + pixelSize - stride, labelPointer + 1 - 2 - imageWidth);
			}
		}
	}
}
