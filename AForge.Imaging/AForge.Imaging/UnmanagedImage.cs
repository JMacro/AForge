using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AForge.Imaging
{
	public class UnmanagedImage : IDisposable
	{
		private IntPtr imageData;

		private int width;

		private int height;

		private int stride;

		private PixelFormat pixelFormat;

		private bool mustBeDisposed;

		public IntPtr ImageData => imageData;

		public int Width => width;

		public int Height => height;

		public int Stride => stride;

		public PixelFormat PixelFormat => pixelFormat;

		public UnmanagedImage(IntPtr imageData, int width, int height, int stride, PixelFormat pixelFormat)
		{
			this.imageData = imageData;
			this.width = width;
			this.height = height;
			this.stride = stride;
			this.pixelFormat = pixelFormat;
		}

		public UnmanagedImage(BitmapData bitmapData)
		{
			imageData = bitmapData.Scan0;
			width = bitmapData.Width;
			height = bitmapData.Height;
			stride = bitmapData.Stride;
			pixelFormat = bitmapData.PixelFormat;
		}

		~UnmanagedImage()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (mustBeDisposed && imageData != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(imageData);
				GC.RemoveMemoryPressure(stride * height);
				imageData = IntPtr.Zero;
			}
		}

		public UnmanagedImage Clone()
		{
			IntPtr dst = Marshal.AllocHGlobal(stride * height);
			GC.AddMemoryPressure(stride * height);
			UnmanagedImage unmanagedImage = new UnmanagedImage(dst, width, height, stride, pixelFormat);
			unmanagedImage.mustBeDisposed = true;
			SystemTools.CopyUnmanagedMemory(dst, imageData, stride * height);
			return unmanagedImage;
		}

		public unsafe void Copy(UnmanagedImage destImage)
		{
			if (width != destImage.width || height != destImage.height || pixelFormat != destImage.pixelFormat)
			{
				throw new InvalidImagePropertiesException("Destination image has different size or pixel format.");
			}
			if (stride == destImage.stride)
			{
				SystemTools.CopyUnmanagedMemory(destImage.imageData, imageData, stride * height);
				return;
			}
			int num = destImage.stride;
			int count = (stride < num) ? stride : num;
			byte* ptr = (byte*)imageData.ToPointer();
			byte* ptr2 = (byte*)destImage.imageData.ToPointer();
			for (int i = 0; i < height; i++)
			{
				SystemTools.CopyUnmanagedMemory(ptr2, ptr, count);
				ptr2 += num;
				ptr += stride;
			}
		}

		public static UnmanagedImage Create(int width, int height, PixelFormat pixelFormat)
		{
			int num = 0;
			switch (pixelFormat)
			{
			case PixelFormat.Format8bppIndexed:
				num = 1;
				break;
			case PixelFormat.Format16bppGrayScale:
				num = 2;
				break;
			case PixelFormat.Format24bppRgb:
				num = 3;
				break;
			case PixelFormat.Format32bppRgb:
			case PixelFormat.Format32bppPArgb:
			case PixelFormat.Format32bppArgb:
				num = 4;
				break;
			case PixelFormat.Format48bppRgb:
				num = 6;
				break;
			case PixelFormat.Format64bppPArgb:
			case PixelFormat.Format64bppArgb:
				num = 8;
				break;
			default:
				throw new UnsupportedImageFormatException("Can not create image with specified pixel format.");
			}
			if (width <= 0 || height <= 0)
			{
				throw new InvalidImagePropertiesException("Invalid image size specified.");
			}
			int num2 = width * num;
			if (num2 % 4 != 0)
			{
				num2 += 4 - num2 % 4;
			}
			IntPtr dst = Marshal.AllocHGlobal(num2 * height);
			SystemTools.SetUnmanagedMemory(dst, 0, num2 * height);
			GC.AddMemoryPressure(num2 * height);
			UnmanagedImage unmanagedImage = new UnmanagedImage(dst, width, height, num2, pixelFormat);
			unmanagedImage.mustBeDisposed = true;
			return unmanagedImage;
		}

		public Bitmap ToManagedImage()
		{
			return ToManagedImage(makeCopy: true);
		}

		public unsafe Bitmap ToManagedImage(bool makeCopy)
		{
			Bitmap bitmap = null;
			try
			{
				if (!makeCopy)
				{
					bitmap = new Bitmap(width, height, stride, pixelFormat, imageData);
					if (pixelFormat == PixelFormat.Format8bppIndexed)
					{
						Image.SetGrayscalePalette(bitmap);
					}
				}
				else
				{
					bitmap = ((pixelFormat == PixelFormat.Format8bppIndexed) ? Image.CreateGrayscaleImage(width, height) : new Bitmap(width, height, pixelFormat));
					BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, pixelFormat);
					int num = bitmapData.Stride;
					int count = System.Math.Min(stride, num);
					byte* ptr = (byte*)bitmapData.Scan0.ToPointer();
					byte* ptr2 = (byte*)imageData.ToPointer();
					if (stride != num)
					{
						for (int i = 0; i < height; i++)
						{
							SystemTools.CopyUnmanagedMemory(ptr, ptr2, count);
							ptr += num;
							ptr2 += stride;
						}
					}
					else
					{
						SystemTools.CopyUnmanagedMemory(ptr, ptr2, stride * height);
					}
					bitmap.UnlockBits(bitmapData);
				}
				return bitmap;
			}
			catch (Exception)
			{
				bitmap?.Dispose();
				throw new InvalidImagePropertiesException("The unmanaged image has some invalid properties, which results in failure of converting it to managed image.");
			}
		}

		public static UnmanagedImage FromManagedImage(Bitmap image)
		{
			UnmanagedImage unmanagedImage = null;
			BitmapData bitmapdata = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			try
			{
				return FromManagedImage(bitmapdata);
			}
			finally
			{
				image.UnlockBits(bitmapdata);
			}
		}

		public static UnmanagedImage FromManagedImage(BitmapData imageData)
		{
			PixelFormat pixelFormat = imageData.PixelFormat;
			if (pixelFormat != PixelFormat.Format8bppIndexed && pixelFormat != PixelFormat.Format16bppGrayScale && pixelFormat != PixelFormat.Format24bppRgb && pixelFormat != PixelFormat.Format32bppRgb && pixelFormat != PixelFormat.Format32bppArgb && pixelFormat != PixelFormat.Format32bppPArgb && pixelFormat != PixelFormat.Format48bppRgb && pixelFormat != PixelFormat.Format64bppArgb && pixelFormat != PixelFormat.Format64bppPArgb)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			IntPtr dst = Marshal.AllocHGlobal(imageData.Stride * imageData.Height);
			GC.AddMemoryPressure(imageData.Stride * imageData.Height);
			UnmanagedImage unmanagedImage = new UnmanagedImage(dst, imageData.Width, imageData.Height, imageData.Stride, pixelFormat);
			SystemTools.CopyUnmanagedMemory(dst, imageData.Scan0, imageData.Stride * imageData.Height);
			unmanagedImage.mustBeDisposed = true;
			return unmanagedImage;
		}

		public unsafe byte[] Collect8bppPixelValues(List<IntPoint> points)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(pixelFormat) / 8;
			if (pixelFormat == PixelFormat.Format16bppGrayScale || num > 4)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image. Use Collect16bppPixelValues() method for it.");
			}
			byte[] array = new byte[points.Count * ((pixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3)];
			byte* ptr = (byte*)imageData.ToPointer();
			if (pixelFormat == PixelFormat.Format8bppIndexed)
			{
				int num2 = 0;
				{
					foreach (IntPoint point in points)
					{
						byte* ptr2 = ptr + (long)stride * (long)point.Y + point.X;
						array[num2++] = *ptr2;
					}
					return array;
				}
			}
			int num3 = 0;
			foreach (IntPoint point2 in points)
			{
				byte* ptr2 = ptr + (long)stride * (long)point2.Y + (long)point2.X * (long)num;
				array[num3++] = ptr2[2];
				array[num3++] = ptr2[1];
				array[num3++] = *ptr2;
			}
			return array;
		}

		public List<IntPoint> CollectActivePixels()
		{
			return CollectActivePixels(new Rectangle(0, 0, width, height));
		}

		public unsafe List<IntPoint> CollectActivePixels(Rectangle rect)
		{
			List<IntPoint> list = new List<IntPoint>();
			int num = System.Drawing.Image.GetPixelFormatSize(pixelFormat) / 8;
			rect.Intersect(new Rectangle(0, 0, width, height));
			int x = rect.X;
			int y = rect.Y;
			int right = rect.Right;
			int bottom = rect.Bottom;
			byte* ptr = (byte*)imageData.ToPointer();
			if (pixelFormat == PixelFormat.Format16bppGrayScale || num > 4)
			{
				int num2 = num >> 1;
				for (int i = y; i < bottom; i++)
				{
					ushort* ptr2 = (ushort*)(ptr + (long)i * (long)stride + (long)x * (long)num);
					if (num2 == 1)
					{
						int num3 = x;
						while (num3 < right)
						{
							if (*ptr2 != 0)
							{
								list.Add(new IntPoint(num3, i));
							}
							num3++;
							ptr2++;
						}
						continue;
					}
					int num4 = x;
					while (num4 < right)
					{
						if (ptr2[2] != 0 || ptr2[1] != 0 || *ptr2 != 0)
						{
							list.Add(new IntPoint(num4, i));
						}
						num4++;
						ptr2 += num2;
					}
				}
			}
			else
			{
				for (int j = y; j < bottom; j++)
				{
					byte* ptr3 = ptr + (long)j * (long)stride + (long)x * (long)num;
					if (num == 1)
					{
						int num5 = x;
						while (num5 < right)
						{
							if (*ptr3 != 0)
							{
								list.Add(new IntPoint(num5, j));
							}
							num5++;
							ptr3++;
						}
						continue;
					}
					int num6 = x;
					while (num6 < right)
					{
						if (ptr3[2] != 0 || ptr3[1] != 0 || *ptr3 != 0)
						{
							list.Add(new IntPoint(num6, j));
						}
						num6++;
						ptr3 += num;
					}
				}
			}
			return list;
		}

		public unsafe void SetPixels(List<IntPoint> coordinates, Color color)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(pixelFormat) / 8;
			byte* ptr = (byte*)imageData.ToPointer();
			byte r = color.R;
			byte g = color.G;
			byte b = color.B;
			byte a = color.A;
			switch (pixelFormat)
			{
			case PixelFormat.Format8bppIndexed:
			{
				byte b2 = (byte)(0.2125 * (double)(int)r + 0.7154 * (double)(int)g + 0.0721 * (double)(int)b);
				foreach (IntPoint coordinate in coordinates)
				{
					if (coordinate.X >= 0 && coordinate.Y >= 0 && coordinate.X < width && coordinate.Y < height)
					{
						byte* ptr7 = ptr + (long)coordinate.Y * (long)stride + coordinate.X;
						*ptr7 = b2;
					}
				}
				break;
			}
			case PixelFormat.Format24bppRgb:
			case PixelFormat.Format32bppRgb:
				foreach (IntPoint coordinate2 in coordinates)
				{
					if (coordinate2.X >= 0 && coordinate2.Y >= 0 && coordinate2.X < width && coordinate2.Y < height)
					{
						byte* ptr6 = ptr + (long)coordinate2.Y * (long)stride + (long)coordinate2.X * (long)num;
						ptr6[2] = r;
						ptr6[1] = g;
						*ptr6 = b;
					}
				}
				break;
			case PixelFormat.Format32bppArgb:
				foreach (IntPoint coordinate3 in coordinates)
				{
					if (coordinate3.X >= 0 && coordinate3.Y >= 0 && coordinate3.X < width && coordinate3.Y < height)
					{
						byte* ptr5 = ptr + (long)coordinate3.Y * (long)stride + (long)coordinate3.X * (long)num;
						ptr5[2] = r;
						ptr5[1] = g;
						*ptr5 = b;
						ptr5[3] = a;
					}
				}
				break;
			case PixelFormat.Format16bppGrayScale:
			{
				ushort num9 = (ushort)((ushort)(0.2125 * (double)(int)r + 0.7154 * (double)(int)g + 0.0721 * (double)(int)b) << 8);
				foreach (IntPoint coordinate4 in coordinates)
				{
					if (coordinate4.X >= 0 && coordinate4.Y >= 0 && coordinate4.X < width && coordinate4.Y < height)
					{
						ushort* ptr4 = (ushort*)(ptr + (long)coordinate4.Y * (long)stride + (long)coordinate4.X * 2L);
						*ptr4 = num9;
					}
				}
				break;
			}
			case PixelFormat.Format48bppRgb:
			{
				ushort num6 = (ushort)(r << 8);
				ushort num7 = (ushort)(g << 8);
				ushort num8 = (ushort)(b << 8);
				foreach (IntPoint coordinate5 in coordinates)
				{
					if (coordinate5.X >= 0 && coordinate5.Y >= 0 && coordinate5.X < width && coordinate5.Y < height)
					{
						ushort* ptr3 = (ushort*)(ptr + (long)coordinate5.Y * (long)stride + (long)coordinate5.X * (long)num);
						ptr3[2] = num6;
						ptr3[1] = num7;
						*ptr3 = num8;
					}
				}
				break;
			}
			case PixelFormat.Format64bppArgb:
			{
				ushort num2 = (ushort)(r << 8);
				ushort num3 = (ushort)(g << 8);
				ushort num4 = (ushort)(b << 8);
				ushort num5 = (ushort)(a << 8);
				foreach (IntPoint coordinate6 in coordinates)
				{
					if (coordinate6.X >= 0 && coordinate6.Y >= 0 && coordinate6.X < width && coordinate6.Y < height)
					{
						ushort* ptr2 = (ushort*)(ptr + (long)coordinate6.Y * (long)stride + (long)coordinate6.X * (long)num);
						ptr2[2] = num2;
						ptr2[1] = num3;
						*ptr2 = num4;
						ptr2[3] = num5;
					}
				}
				break;
			}
			default:
				throw new UnsupportedImageFormatException("The pixel format is not supported: " + pixelFormat);
			}
		}

		public void SetPixel(IntPoint point, Color color)
		{
			SetPixel(point.X, point.Y, color);
		}

		public void SetPixel(int x, int y, Color color)
		{
			SetPixel(x, y, color.R, color.G, color.B, color.A);
		}

		public void SetPixel(int x, int y, byte value)
		{
			SetPixel(x, y, value, value, value, byte.MaxValue);
		}

		private unsafe void SetPixel(int x, int y, byte r, byte g, byte b, byte a)
		{
			if (x >= 0 && y >= 0 && x < width && y < height)
			{
				int num = System.Drawing.Image.GetPixelFormatSize(pixelFormat) / 8;
				byte* ptr = (byte*)imageData.ToPointer() + (long)y * (long)stride + (long)x * (long)num;
				ushort* ptr2 = (ushort*)ptr;
				switch (pixelFormat)
				{
				case PixelFormat.Format8bppIndexed:
					*ptr = (byte)(0.2125 * (double)(int)r + 0.7154 * (double)(int)g + 0.0721 * (double)(int)b);
					break;
				case PixelFormat.Format24bppRgb:
				case PixelFormat.Format32bppRgb:
					ptr[2] = r;
					ptr[1] = g;
					*ptr = b;
					break;
				case PixelFormat.Format32bppArgb:
					ptr[2] = r;
					ptr[1] = g;
					*ptr = b;
					ptr[3] = a;
					break;
				case PixelFormat.Format16bppGrayScale:
					*ptr2 = (ushort)((ushort)(0.2125 * (double)(int)r + 0.7154 * (double)(int)g + 0.0721 * (double)(int)b) << 8);
					break;
				case PixelFormat.Format48bppRgb:
					ptr2[2] = (ushort)(r << 8);
					ptr2[1] = (ushort)(g << 8);
					*ptr2 = (ushort)(b << 8);
					break;
				case PixelFormat.Format64bppArgb:
					ptr2[2] = (ushort)(r << 8);
					ptr2[1] = (ushort)(g << 8);
					*ptr2 = (ushort)(b << 8);
					ptr2[3] = (ushort)(a << 8);
					break;
				default:
					throw new UnsupportedImageFormatException("The pixel format is not supported: " + pixelFormat);
				}
			}
		}

		public Color GetPixel(IntPoint point)
		{
			return GetPixel(point.X, point.Y);
		}

		public unsafe Color GetPixel(int x, int y)
		{
			if (x < 0 || y < 0)
			{
				throw new ArgumentOutOfRangeException("x", "The specified pixel coordinate is out of image's bounds.");
			}
			if (x >= width || y >= height)
			{
				throw new ArgumentOutOfRangeException("y", "The specified pixel coordinate is out of image's bounds.");
			}
			Color color = default(Color);
			int num = System.Drawing.Image.GetPixelFormatSize(pixelFormat) / 8;
			byte* ptr = (byte*)imageData.ToPointer() + (long)y * (long)stride + (long)x * (long)num;
			switch (pixelFormat)
			{
			case PixelFormat.Format8bppIndexed:
				return Color.FromArgb(*ptr, *ptr, *ptr);
			case PixelFormat.Format24bppRgb:
			case PixelFormat.Format32bppRgb:
				return Color.FromArgb(ptr[2], ptr[1], *ptr);
			case PixelFormat.Format32bppArgb:
				return Color.FromArgb(ptr[3], ptr[2], ptr[1], *ptr);
			default:
				throw new UnsupportedImageFormatException("The pixel format is not supported: " + pixelFormat);
			}
		}

		public unsafe ushort[] Collect16bppPixelValues(List<IntPoint> points)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(pixelFormat) / 8;
			if (pixelFormat == PixelFormat.Format8bppIndexed || num == 3 || num == 4)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image. Use Collect8bppPixelValues() method for it.");
			}
			ushort[] array = new ushort[points.Count * ((pixelFormat == PixelFormat.Format16bppGrayScale) ? 1 : 3)];
			byte* ptr = (byte*)imageData.ToPointer();
			if (pixelFormat == PixelFormat.Format16bppGrayScale)
			{
				int num2 = 0;
				{
					foreach (IntPoint point in points)
					{
						ushort* ptr2 = (ushort*)(ptr + (long)stride * (long)point.Y + (long)point.X * (long)num);
						array[num2++] = *ptr2;
					}
					return array;
				}
			}
			int num3 = 0;
			foreach (IntPoint point2 in points)
			{
				ushort* ptr2 = (ushort*)(ptr + (long)stride * (long)point2.Y + (long)point2.X * (long)num);
				array[num3++] = ptr2[2];
				array[num3++] = ptr2[1];
				array[num3++] = *ptr2;
			}
			return array;
		}
	}
}
