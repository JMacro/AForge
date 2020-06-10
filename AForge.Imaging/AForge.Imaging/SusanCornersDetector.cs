using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class SusanCornersDetector : ICornersDetector
	{
		private int differenceThreshold = 25;

		private int geometricalThreshold = 18;

		private static int[] rowRadius = new int[7]
		{
			1,
			2,
			3,
			3,
			3,
			2,
			1
		};

		public int DifferenceThreshold
		{
			get
			{
				return differenceThreshold;
			}
			set
			{
				differenceThreshold = value;
			}
		}

		public int GeometricalThreshold
		{
			get
			{
				return geometricalThreshold;
			}
			set
			{
				geometricalThreshold = value;
			}
		}

		public SusanCornersDetector()
		{
		}

		public SusanCornersDetector(int differenceThreshold, int geometricalThreshold)
		{
			this.differenceThreshold = differenceThreshold;
			this.geometricalThreshold = geometricalThreshold;
		}

		public List<IntPoint> ProcessImage(Bitmap image)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed && image.PixelFormat != PixelFormat.Format24bppRgb && image.PixelFormat != PixelFormat.Format32bppRgb && image.PixelFormat != PixelFormat.Format32bppArgb)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			try
			{
				return ProcessImage(new UnmanagedImage(bitmapData));
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public List<IntPoint> ProcessImage(BitmapData imageData)
		{
			return ProcessImage(new UnmanagedImage(imageData));
		}

		public unsafe List<IntPoint> ProcessImage(UnmanagedImage image)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed && image.PixelFormat != PixelFormat.Format24bppRgb && image.PixelFormat != PixelFormat.Format32bppRgb && image.PixelFormat != PixelFormat.Format32bppArgb)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			int width = image.Width;
			int height = image.Height;
			UnmanagedImage unmanagedImage = null;
			unmanagedImage = ((image.PixelFormat != PixelFormat.Format8bppIndexed) ? Grayscale.CommonAlgorithms.BT709.Apply(image) : image);
			int[,] array = new int[height, width];
			int stride = unmanagedImage.Stride;
			int num = stride - width;
			byte* ptr = (byte*)unmanagedImage.ImageData.ToPointer() + (long)stride * 3L + 3;
			int i = 3;
			for (int num2 = height - 3; i < num2; i++)
			{
				int num3 = 3;
				int num4 = width - 3;
				while (num3 < num4)
				{
					byte b = *ptr;
					int num5 = 0;
					int num6 = 0;
					int num7 = 0;
					for (int j = -3; j <= 3; j++)
					{
						int num8 = rowRadius[j + 3];
						byte* ptr2 = ptr + (long)stride * (long)j;
						for (int k = -num8; k <= num8; k++)
						{
							if (System.Math.Abs(b - ptr2[k]) <= differenceThreshold)
							{
								num5++;
								num6 += num3 + k;
								num7 += i + j;
							}
						}
					}
					if (num5 < geometricalThreshold)
					{
						num6 /= num5;
						num7 /= num5;
						num5 = ((num3 != num6 || i != num7) ? (geometricalThreshold - num5) : 0);
					}
					else
					{
						num5 = 0;
					}
					array[i, num3] = num5;
					num3++;
					ptr++;
				}
				ptr += 6 + num;
			}
			if (image.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				unmanagedImage.Dispose();
			}
			List<IntPoint> list = new List<IntPoint>();
			int l = 2;
			for (int num9 = height - 2; l < num9; l++)
			{
				int m = 2;
				for (int num10 = width - 2; m < num10; m++)
				{
					int num11 = array[l, m];
					int num12 = -2;
					while (num11 != 0 && num12 <= 2)
					{
						for (int n = -2; n <= 2; n++)
						{
							if (array[l + num12, m + n] > num11)
							{
								num11 = 0;
								break;
							}
						}
						num12++;
					}
					if (num11 != 0)
					{
						list.Add(new IntPoint(m, l));
					}
				}
			}
			return list;
		}
	}
}
