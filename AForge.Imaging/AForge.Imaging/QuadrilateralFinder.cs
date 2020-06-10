using AForge.Math.Geometry;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class QuadrilateralFinder
	{
		public List<IntPoint> ProcessImage(Bitmap image)
		{
			CheckPixelFormat(image.PixelFormat);
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			List<IntPoint> list = null;
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
			CheckPixelFormat(image.PixelFormat);
			int width = image.Width;
			int height = image.Height;
			List<IntPoint> list = new List<IntPoint>();
			byte* ptr = (byte*)image.ImageData.ToPointer();
			int stride = image.Stride;
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				for (int i = 0; i < height; i++)
				{
					bool flag = true;
					for (int j = 0; j < width; j++)
					{
						if (ptr[j] != 0)
						{
							list.Add(new IntPoint(j, i));
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						for (int num = width - 1; num >= 0; num--)
						{
							if (ptr[num] != 0)
							{
								list.Add(new IntPoint(num, i));
								break;
							}
						}
					}
					ptr += stride;
				}
			}
			else
			{
				int num2 = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
				byte* ptr2 = null;
				for (int k = 0; k < height; k++)
				{
					bool flag = true;
					ptr2 = ptr;
					int num3 = 0;
					while (num3 < width)
					{
						if (ptr2[2] != 0 || ptr2[1] != 0 || *ptr2 != 0)
						{
							list.Add(new IntPoint(num3, k));
							flag = false;
							break;
						}
						num3++;
						ptr2 += num2;
					}
					if (!flag)
					{
						ptr2 = ptr + (long)width * (long)num2 - num2;
						int num4 = width - 1;
						while (num4 >= 0)
						{
							if (ptr2[2] != 0 || ptr2[1] != 0 || *ptr2 != 0)
							{
								list.Add(new IntPoint(num4, k));
								break;
							}
							num4--;
							ptr2 -= num2;
						}
					}
					ptr += stride;
				}
			}
			return PointsCloud.FindQuadrilateralCorners(list);
		}

		private void CheckPixelFormat(PixelFormat format)
		{
			if (format != PixelFormat.Format8bppIndexed && format != PixelFormat.Format24bppRgb && format != PixelFormat.Format32bppArgb && format != PixelFormat.Format32bppPArgb)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
		}
	}
}
