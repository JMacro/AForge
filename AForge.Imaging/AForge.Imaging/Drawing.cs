using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public static class Drawing
	{
		public static void FillRectangle(BitmapData imageData, Rectangle rectangle, Color color)
		{
			FillRectangle(new UnmanagedImage(imageData), rectangle, color);
		}

		public unsafe static void FillRectangle(UnmanagedImage image, Rectangle rectangle, Color color)
		{
			CheckPixelFormat(image.PixelFormat);
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int width = image.Width;
			int height = image.Height;
			int stride = image.Stride;
			int x = rectangle.X;
			int y = rectangle.Y;
			int num2 = rectangle.X + rectangle.Width - 1;
			int num3 = rectangle.Y + rectangle.Height - 1;
			if (x >= width || y >= height || num2 < 0 || num3 < 0)
			{
				return;
			}
			int num4 = System.Math.Max(0, x);
			int num5 = System.Math.Min(width - 1, num2);
			int num6 = System.Math.Max(0, y);
			int num7 = System.Math.Min(height - 1, num3);
			byte* ptr = (byte*)image.ImageData.ToPointer() + (long)num6 * (long)stride + (long)num4 * (long)num;
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				byte filler = (byte)(0.2125 * (double)(int)color.R + 0.7154 * (double)(int)color.G + 0.0721 * (double)(int)color.B);
				int count = num5 - num4 + 1;
				for (int i = num6; i <= num7; i++)
				{
					SystemTools.SetUnmanagedMemory(ptr, filler, count);
					ptr += stride;
				}
				return;
			}
			if (image.PixelFormat == PixelFormat.Format32bppArgb)
			{
				double num8 = (double)(int)color.A / 255.0;
				double num9 = 1.0 - num8;
				double num10 = num8 * (double)(int)color.R;
				double num11 = num8 * (double)(int)color.G;
				double num12 = num8 * (double)(int)color.B;
				int num13 = stride - (num5 - num4 + 1) * 4;
				for (int j = num6; j <= num7; j++)
				{
					int num14 = num4;
					while (num14 <= num5)
					{
						double num15 = (double)(int)ptr[3] / 255.0 * num9;
						ptr[2] = (byte)(num10 + num15 * (double)(int)ptr[2]);
						ptr[1] = (byte)(num11 + num15 * (double)(int)ptr[1]);
						*ptr = (byte)(num12 + num15 * (double)(int)(*ptr));
						ptr[3] = (byte)(255.0 * (num8 + num15));
						num14++;
						ptr += 4;
					}
					ptr += num13;
				}
				return;
			}
			byte r = color.R;
			byte g = color.G;
			byte b = color.B;
			int num16 = stride - (num5 - num4 + 1) * num;
			if (color.A == byte.MaxValue)
			{
				for (int k = num6; k <= num7; k++)
				{
					int num17 = num4;
					while (num17 <= num5)
					{
						ptr[2] = r;
						ptr[1] = g;
						*ptr = b;
						num17++;
						ptr += num;
					}
					ptr += num16;
				}
				return;
			}
			int a = color.A;
			int num18 = 255 - a;
			int num19 = a * color.R;
			int num20 = a * color.G;
			int num21 = a * color.B;
			for (int l = num6; l <= num7; l++)
			{
				int num22 = num4;
				while (num22 <= num5)
				{
					ptr[2] = (byte)((num19 + num18 * ptr[2]) / 255);
					ptr[1] = (byte)((num20 + num18 * ptr[1]) / 255);
					*ptr = (byte)((num21 + num18 * *ptr) / 255);
					num22++;
					ptr += num;
				}
				ptr += num16;
			}
		}

		public static void Rectangle(BitmapData imageData, Rectangle rectangle, Color color)
		{
			Rectangle(new UnmanagedImage(imageData), rectangle, color);
		}

		public static void Rectangle(UnmanagedImage image, Rectangle rectangle, Color color)
		{
			CheckPixelFormat(image.PixelFormat);
			_ = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int width = image.Width;
			int height = image.Height;
			_ = image.Stride;
			int x = rectangle.X;
			int y = rectangle.Y;
			int num = rectangle.X + rectangle.Width - 1;
			int num2 = rectangle.Y + rectangle.Height - 1;
			if (x < width && y < height && num >= 0 && num2 >= 0)
			{
				Line(image, new IntPoint(x, y), new IntPoint(num, y), color);
				Line(image, new IntPoint(num, num2), new IntPoint(x, num2), color);
				Line(image, new IntPoint(num, y + 1), new IntPoint(num, num2 - 1), color);
				Line(image, new IntPoint(x, num2 - 1), new IntPoint(x, y + 1), color);
			}
		}

		public static void Line(BitmapData imageData, IntPoint point1, IntPoint point2, Color color)
		{
			Line(new UnmanagedImage(imageData), point1, point2, color);
		}

		public unsafe static void Line(UnmanagedImage image, IntPoint point1, IntPoint point2, Color color)
		{
			CheckPixelFormat(image.PixelFormat);
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int width = image.Width;
			int height = image.Height;
			int stride = image.Stride;
			if ((point1.X < 0 && point2.X < 0) || (point1.Y < 0 && point2.Y < 0) || (point1.X >= width && point2.X >= width) || (point1.Y >= height && point2.Y >= height))
			{
				return;
			}
			CheckEndPoint(width, height, point1, ref point2);
			CheckEndPoint(width, height, point2, ref point1);
			if ((point1.X < 0 && point2.X < 0) || (point1.Y < 0 && point2.Y < 0) || (point1.X >= width && point2.X >= width) || (point1.Y >= height && point2.Y >= height))
			{
				return;
			}
			int x = point1.X;
			int y = point1.Y;
			int x2 = point2.X;
			int y2 = point2.Y;
			byte b = 0;
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				b = (byte)(0.2125 * (double)(int)color.R + 0.7154 * (double)(int)color.G + 0.0721 * (double)(int)color.B);
			}
			double num2 = (double)(int)color.A / 255.0;
			double num3 = 1.0 - num2;
			double num4 = num2 * (double)(int)color.R;
			double num5 = num2 * (double)(int)color.G;
			double num6 = num2 * (double)(int)color.B;
			int num7 = 255 - color.A;
			int num8 = color.A * color.R;
			int num9 = color.A * color.G;
			int num10 = color.A * color.B;
			int num11 = x2 - x;
			int num12 = y2 - y;
			if (System.Math.Abs(num11) >= System.Math.Abs(num12))
			{
				float num13 = (num11 != 0) ? ((float)num12 / (float)num11) : 0f;
				int num14 = (num11 > 0) ? 1 : (-1);
				num11 += num14;
				if (image.PixelFormat == PixelFormat.Format8bppIndexed)
				{
					for (int i = 0; i != num11; i += num14)
					{
						int num15 = x + i;
						int num16 = (int)((float)y + num13 * (float)i);
						byte* ptr = (byte*)image.ImageData.ToPointer() + (long)num16 * (long)stride + num15;
						*ptr = b;
					}
				}
				else if (image.PixelFormat == PixelFormat.Format32bppArgb)
				{
					for (int j = 0; j != num11; j += num14)
					{
						int num17 = x + j;
						int num18 = (int)((float)y + num13 * (float)j);
						byte* ptr2 = (byte*)image.ImageData.ToPointer() + (long)num18 * (long)stride + (long)num17 * 4L;
						double num19 = (double)(int)ptr2[3] / 255.0 * num3;
						ptr2[2] = (byte)(num4 + num19 * (double)(int)ptr2[2]);
						ptr2[1] = (byte)(num5 + num19 * (double)(int)ptr2[1]);
						*ptr2 = (byte)(num6 + num19 * (double)(int)(*ptr2));
						ptr2[3] = (byte)(255.0 * (num2 + num19));
					}
				}
				else if (color.A == byte.MaxValue)
				{
					for (int k = 0; k != num11; k += num14)
					{
						int num20 = x + k;
						int num21 = (int)((float)y + num13 * (float)k);
						byte* ptr3 = (byte*)image.ImageData.ToPointer() + (long)num21 * (long)stride + (long)num20 * (long)num;
						ptr3[2] = color.R;
						ptr3[1] = color.G;
						*ptr3 = color.B;
					}
				}
				else
				{
					for (int l = 0; l != num11; l += num14)
					{
						int num22 = x + l;
						int num23 = (int)((float)y + num13 * (float)l);
						byte* ptr4 = (byte*)image.ImageData.ToPointer() + (long)num23 * (long)stride + (long)num22 * (long)num;
						ptr4[2] = (byte)((num8 + num7 * ptr4[2]) / 255);
						ptr4[1] = (byte)((num9 + num7 * ptr4[1]) / 255);
						*ptr4 = (byte)((num10 + num7 * *ptr4) / 255);
					}
				}
				return;
			}
			float num24 = (num12 != 0) ? ((float)num11 / (float)num12) : 0f;
			int num25 = (num12 > 0) ? 1 : (-1);
			num12 += num25;
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				for (int m = 0; m != num12; m += num25)
				{
					int num26 = (int)((float)x + num24 * (float)m);
					int num27 = y + m;
					byte* ptr5 = (byte*)image.ImageData.ToPointer() + (long)num27 * (long)stride + num26;
					*ptr5 = b;
				}
			}
			else if (image.PixelFormat == PixelFormat.Format32bppArgb)
			{
				for (int n = 0; n != num12; n += num25)
				{
					int num28 = (int)((float)x + num24 * (float)n);
					int num29 = y + n;
					byte* ptr6 = (byte*)image.ImageData.ToPointer() + (long)num29 * (long)stride + (long)num28 * 4L;
					double num30 = (double)(int)ptr6[3] / 255.0 * num3;
					ptr6[2] = (byte)(num4 + num30 * (double)(int)ptr6[2]);
					ptr6[1] = (byte)(num5 + num30 * (double)(int)ptr6[1]);
					*ptr6 = (byte)(num6 + num30 * (double)(int)(*ptr6));
					ptr6[3] = (byte)(255.0 * (num2 + num30));
				}
			}
			else if (color.A == byte.MaxValue)
			{
				for (int num31 = 0; num31 != num12; num31 += num25)
				{
					int num32 = (int)((float)x + num24 * (float)num31);
					int num33 = y + num31;
					byte* ptr7 = (byte*)image.ImageData.ToPointer() + (long)num33 * (long)stride + (long)num32 * (long)num;
					ptr7[2] = color.R;
					ptr7[1] = color.G;
					*ptr7 = color.B;
				}
			}
			else
			{
				for (int num34 = 0; num34 != num12; num34 += num25)
				{
					int num35 = (int)((float)x + num24 * (float)num34);
					int num36 = y + num34;
					byte* ptr8 = (byte*)image.ImageData.ToPointer() + (long)num36 * (long)stride + (long)num35 * (long)num;
					ptr8[2] = (byte)((num8 + num7 * ptr8[2]) / 255);
					ptr8[1] = (byte)((num9 + num7 * ptr8[1]) / 255);
					*ptr8 = (byte)((num10 + num7 * *ptr8) / 255);
				}
			}
		}

		public static void Polygon(BitmapData imageData, List<IntPoint> points, Color color)
		{
			Polygon(new UnmanagedImage(imageData), points, color);
		}

		public static void Polygon(UnmanagedImage image, List<IntPoint> points, Color color)
		{
			int i = 1;
			for (int count = points.Count; i < count; i++)
			{
				Line(image, points[i - 1], points[i], color);
			}
			Line(image, points[points.Count - 1], points[0], color);
		}

		public static void Polyline(BitmapData imageData, List<IntPoint> points, Color color)
		{
			Polyline(new UnmanagedImage(imageData), points, color);
		}

		public static void Polyline(UnmanagedImage image, List<IntPoint> points, Color color)
		{
			int i = 1;
			for (int count = points.Count; i < count; i++)
			{
				Line(image, points[i - 1], points[i], color);
			}
		}

		private static void CheckPixelFormat(PixelFormat format)
		{
			if (format != PixelFormat.Format24bppRgb && format != PixelFormat.Format8bppIndexed && format != PixelFormat.Format32bppArgb && format != PixelFormat.Format32bppRgb)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
		}

		private static void CheckEndPoint(int width, int height, IntPoint start, ref IntPoint end)
		{
			if (end.X >= width)
			{
				int num = width - 1;
				double num2 = (double)(num - start.X) / (double)(end.X - start.X);
				end.Y = (int)((double)start.Y + num2 * (double)(end.Y - start.Y));
				end.X = num;
			}
			if (end.Y >= height)
			{
				int num3 = height - 1;
				double num4 = (double)(num3 - start.Y) / (double)(end.Y - start.Y);
				end.X = (int)((double)start.X + num4 * (double)(end.X - start.X));
				end.Y = num3;
			}
			if (end.X < 0)
			{
				double num5 = (double)(-start.X) / (double)(end.X - start.X);
				end.Y = (int)((double)start.Y + num5 * (double)(end.Y - start.Y));
				end.X = 0;
			}
			if (end.Y < 0)
			{
				double num6 = (double)(-start.Y) / (double)(end.Y - start.Y);
				end.X = (int)((double)start.X + num6 * (double)(end.X - start.X));
				end.Y = 0;
			}
		}
	}
}
