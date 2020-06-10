using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Convolution : BaseUsingCopyPartialFilter
	{
		private int[,] kernel;

		private int divisor = 1;

		private int threshold;

		private int size;

		private bool dynamicDivisorForEdges = true;

		private bool processAlpha;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public int[,] Kernel
		{
			get
			{
				return kernel;
			}
			set
			{
				int length = value.GetLength(0);
				if (length != value.GetLength(1) || length < 3 || length > 99 || length % 2 == 0)
				{
					throw new ArgumentException("Invalid kernel size.");
				}
				kernel = value;
				size = length;
			}
		}

		public int Divisor
		{
			get
			{
				return divisor;
			}
			set
			{
				if (value == 0)
				{
					throw new ArgumentException("Divisor can not be equal to zero.");
				}
				divisor = value;
			}
		}

		public int Threshold
		{
			get
			{
				return threshold;
			}
			set
			{
				threshold = value;
			}
		}

		public bool DynamicDivisorForEdges
		{
			get
			{
				return dynamicDivisorForEdges;
			}
			set
			{
				dynamicDivisorForEdges = value;
			}
		}

		public bool ProcessAlpha
		{
			get
			{
				return processAlpha;
			}
			set
			{
				processAlpha = value;
			}
		}

		protected Convolution()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
			formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
		}

		public Convolution(int[,] kernel)
			: this()
		{
			Kernel = kernel;
			divisor = 0;
			int i = 0;
			for (int length = kernel.GetLength(0); i < length; i++)
			{
				int j = 0;
				for (int length2 = kernel.GetLength(1); j < length2; j++)
				{
					divisor += kernel[i, j];
				}
			}
			if (divisor == 0)
			{
				divisor = 1;
			}
		}

		public Convolution(int[,] kernel, int divisor)
			: this()
		{
			Kernel = kernel;
			Divisor = divisor;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(source.PixelFormat) / 8;
			int left = rect.Left;
			int top = rect.Top;
			int stopX = left + rect.Width;
			int stopY = top + rect.Height;
			if (num <= 4 && num != 2)
			{
				int stride = source.Stride;
				int stride2 = destination.Stride;
				int srcOffset = stride - rect.Width * num;
				int dstOffset = stride2 - rect.Width * num;
				byte* ptr = (byte*)source.ImageData.ToPointer();
				byte* ptr2 = (byte*)destination.ImageData.ToPointer();
				ptr += top * stride + left * num;
				ptr2 += top * stride2 + left * num;
				if (destination.PixelFormat == PixelFormat.Format8bppIndexed)
				{
					Process8bppImage(ptr, ptr2, stride, stride2, srcOffset, dstOffset, left, top, stopX, stopY);
				}
				else if (num == 3 || !processAlpha)
				{
					Process24bppImage(ptr, ptr2, stride, stride2, srcOffset, dstOffset, left, top, stopX, stopY, num);
				}
				else
				{
					Process32bppImage(ptr, ptr2, stride, stride2, srcOffset, dstOffset, left, top, stopX, stopY);
				}
			}
			else
			{
				num /= 2;
				int dstStride = destination.Stride / 2;
				int srcStride = source.Stride / 2;
				ushort* ptr3 = (ushort*)source.ImageData.ToPointer();
				ushort* ptr4 = (ushort*)destination.ImageData.ToPointer();
				ptr3 += left * num;
				ptr4 += left * num;
				if (source.PixelFormat == PixelFormat.Format16bppGrayScale)
				{
					Process16bppImage(ptr3, ptr4, srcStride, dstStride, left, top, stopX, stopY);
				}
				else if (num == 3 || !processAlpha)
				{
					Process48bppImage(ptr3, ptr4, srcStride, dstStride, left, top, stopX, stopY, num);
				}
				else
				{
					Process64bppImage(ptr3, ptr4, srcStride, dstStride, left, top, stopX, stopY);
				}
			}
		}

		private unsafe void Process8bppImage(byte* src, byte* dst, int srcStride, int dstStride, int srcOffset, int dstOffset, int startX, int startY, int stopX, int stopY)
		{
			int num = size >> 1;
			int num2 = size * size;
			for (int i = startY; i < stopY; i++)
			{
				int num3 = startX;
				while (num3 < stopX)
				{
					long num5;
					int num4;
					long num6 = num5 = (num4 = 0);
					for (int j = 0; j < size; j++)
					{
						int num7 = j - num;
						int num8 = i + num7;
						if (num8 < startY)
						{
							continue;
						}
						if (num8 >= stopY)
						{
							break;
						}
						for (int k = 0; k < size; k++)
						{
							int num9 = k - num;
							num8 = num3 + num9;
							if (num8 >= startX && num8 < stopX)
							{
								int num10 = kernel[j, k];
								num5 += num10;
								num6 += num10 * src[num7 * srcStride + num9];
								num4++;
							}
						}
					}
					if (num4 == num2)
					{
						num5 = divisor;
					}
					else if (!dynamicDivisorForEdges)
					{
						num5 = divisor;
					}
					if (num5 != 0)
					{
						num6 /= num5;
					}
					num6 += threshold;
					*dst = (byte)((num6 > 255) ? 255 : ((num6 < 0) ? 0 : num6));
					num3++;
					src++;
					dst++;
				}
				src += srcOffset;
				dst += dstOffset;
			}
		}

		private unsafe void Process24bppImage(byte* src, byte* dst, int srcStride, int dstStride, int srcOffset, int dstOffset, int startX, int startY, int stopX, int stopY, int pixelSize)
		{
			int num = size >> 1;
			int num2 = size * size;
			for (int i = startY; i < stopY; i++)
			{
				int num3 = startX;
				while (num3 < stopX)
				{
					long num5;
					int num4;
					long num7;
					long num6;
					long num8 = num7 = (num6 = (num5 = (num4 = 0)));
					for (int j = 0; j < size; j++)
					{
						int num9 = j - num;
						int num10 = i + num9;
						if (num10 < startY)
						{
							continue;
						}
						if (num10 >= stopY)
						{
							break;
						}
						for (int k = 0; k < size; k++)
						{
							int num11 = k - num;
							num10 = num3 + num11;
							if (num10 >= startX && num10 < stopX)
							{
								int num12 = kernel[j, k];
								byte* ptr = src + (num9 * srcStride + num11 * pixelSize);
								num5 += num12;
								num8 += num12 * ptr[2];
								num7 += num12 * ptr[1];
								num6 += num12 * *ptr;
								num4++;
							}
						}
					}
					if (num4 == num2)
					{
						num5 = divisor;
					}
					else if (!dynamicDivisorForEdges)
					{
						num5 = divisor;
					}
					if (num5 != 0)
					{
						num8 /= num5;
						num7 /= num5;
						num6 /= num5;
					}
					num8 += threshold;
					num7 += threshold;
					num6 += threshold;
					dst[2] = (byte)((num8 > 255) ? 255 : ((num8 < 0) ? 0 : num8));
					dst[1] = (byte)((num7 > 255) ? 255 : ((num7 < 0) ? 0 : num7));
					*dst = (byte)((num6 > 255) ? 255 : ((num6 < 0) ? 0 : num6));
					if (pixelSize == 4)
					{
						dst[3] = src[3];
					}
					num3++;
					src += pixelSize;
					dst += pixelSize;
				}
				src += srcOffset;
				dst += dstOffset;
			}
		}

		private unsafe void Process32bppImage(byte* src, byte* dst, int srcStride, int dstStride, int srcOffset, int dstOffset, int startX, int startY, int stopX, int stopY)
		{
			int num = size >> 1;
			int num2 = size * size;
			for (int i = startY; i < stopY; i++)
			{
				int num3 = startX;
				while (num3 < stopX)
				{
					long num5;
					int num4;
					long num8;
					long num7;
					long num6;
					long num9 = num8 = (num7 = (num6 = (num5 = (num4 = 0))));
					for (int j = 0; j < size; j++)
					{
						int num10 = j - num;
						int num11 = i + num10;
						if (num11 < startY)
						{
							continue;
						}
						if (num11 >= stopY)
						{
							break;
						}
						for (int k = 0; k < size; k++)
						{
							int num12 = k - num;
							num11 = num3 + num12;
							if (num11 >= startX && num11 < stopX)
							{
								int num13 = kernel[j, k];
								byte* ptr = src + (num10 * srcStride + num12 * 4);
								num5 += num13;
								num9 += num13 * ptr[2];
								num8 += num13 * ptr[1];
								num7 += num13 * *ptr;
								num6 += num13 * ptr[3];
								num4++;
							}
						}
					}
					if (num4 == num2)
					{
						num5 = divisor;
					}
					else if (!dynamicDivisorForEdges)
					{
						num5 = divisor;
					}
					if (num5 != 0)
					{
						num9 /= num5;
						num8 /= num5;
						num7 /= num5;
						num6 /= num5;
					}
					num9 += threshold;
					num8 += threshold;
					num7 += threshold;
					num6 += threshold;
					dst[2] = (byte)((num9 > 255) ? 255 : ((num9 < 0) ? 0 : num9));
					dst[1] = (byte)((num8 > 255) ? 255 : ((num8 < 0) ? 0 : num8));
					*dst = (byte)((num7 > 255) ? 255 : ((num7 < 0) ? 0 : num7));
					dst[3] = (byte)((num6 > 255) ? 255 : ((num6 < 0) ? 0 : num6));
					num3++;
					src += 4;
					dst += 4;
				}
				src += srcOffset;
				dst += dstOffset;
			}
		}

		private unsafe void Process16bppImage(ushort* baseSrc, ushort* baseDst, int srcStride, int dstStride, int startX, int startY, int stopX, int stopY)
		{
			int num = size >> 1;
			int num2 = size * size;
			for (int i = startY; i < stopY; i++)
			{
				ushort* ptr = baseSrc + i * srcStride;
				ushort* ptr2 = baseDst + i * dstStride;
				int num3 = startX;
				while (num3 < stopX)
				{
					long num5;
					int num4;
					long num6 = num5 = (num4 = 0);
					for (int j = 0; j < size; j++)
					{
						int num7 = j - num;
						int num8 = i + num7;
						if (num8 < startY)
						{
							continue;
						}
						if (num8 >= stopY)
						{
							break;
						}
						for (int k = 0; k < size; k++)
						{
							int num9 = k - num;
							num8 = num3 + num9;
							if (num8 >= startX && num8 < stopX)
							{
								int num10 = kernel[j, k];
								num5 += num10;
								num6 += num10 * ptr[num7 * srcStride + num9];
								num4++;
							}
						}
					}
					if (num4 == num2)
					{
						num5 = divisor;
					}
					else if (!dynamicDivisorForEdges)
					{
						num5 = divisor;
					}
					if (num5 != 0)
					{
						num6 /= num5;
					}
					num6 += threshold;
					*ptr2 = (ushort)((num6 > 65535) ? 65535 : ((num6 < 0) ? 0 : num6));
					num3++;
					ptr++;
					ptr2++;
				}
			}
		}

		private unsafe void Process48bppImage(ushort* baseSrc, ushort* baseDst, int srcStride, int dstStride, int startX, int startY, int stopX, int stopY, int pixelSize)
		{
			int num = size >> 1;
			int num2 = size * size;
			for (int i = startY; i < stopY; i++)
			{
				ushort* ptr = baseSrc + i * srcStride;
				ushort* ptr2 = baseDst + i * dstStride;
				int num3 = startX;
				while (num3 < stopX)
				{
					long num5;
					int num4;
					long num7;
					long num6;
					long num8 = num7 = (num6 = (num5 = (num4 = 0)));
					for (int j = 0; j < size; j++)
					{
						int num9 = j - num;
						int num10 = i + num9;
						if (num10 < startY)
						{
							continue;
						}
						if (num10 >= stopY)
						{
							break;
						}
						for (int k = 0; k < size; k++)
						{
							int num11 = k - num;
							num10 = num3 + num11;
							if (num10 >= startX && num10 < stopX)
							{
								int num12 = kernel[j, k];
								ushort* ptr3 = ptr + (num9 * srcStride + num11 * pixelSize);
								num5 += num12;
								num8 += num12 * ptr3[2];
								num7 += num12 * ptr3[1];
								num6 += num12 * *ptr3;
								num4++;
							}
						}
					}
					if (num4 == num2)
					{
						num5 = divisor;
					}
					else if (!dynamicDivisorForEdges)
					{
						num5 = divisor;
					}
					if (num5 != 0)
					{
						num8 /= num5;
						num7 /= num5;
						num6 /= num5;
					}
					num8 += threshold;
					num7 += threshold;
					num6 += threshold;
					ptr2[2] = (ushort)((num8 > 65535) ? 65535 : ((num8 < 0) ? 0 : num8));
					ptr2[1] = (ushort)((num7 > 65535) ? 65535 : ((num7 < 0) ? 0 : num7));
					*ptr2 = (ushort)((num6 > 65535) ? 65535 : ((num6 < 0) ? 0 : num6));
					if (pixelSize == 4)
					{
						ptr2[3] = ptr[3];
					}
					num3++;
					ptr += pixelSize;
					ptr2 += pixelSize;
				}
			}
		}

		private unsafe void Process64bppImage(ushort* baseSrc, ushort* baseDst, int srcStride, int dstStride, int startX, int startY, int stopX, int stopY)
		{
			int num = size >> 1;
			int num2 = size * size;
			for (int i = startY; i < stopY; i++)
			{
				ushort* ptr = baseSrc + i * srcStride;
				ushort* ptr2 = baseDst + i * dstStride;
				int num3 = startX;
				while (num3 < stopX)
				{
					long num5;
					int num4;
					long num8;
					long num7;
					long num6;
					long num9 = num8 = (num7 = (num6 = (num5 = (num4 = 0))));
					for (int j = 0; j < size; j++)
					{
						int num10 = j - num;
						int num11 = i + num10;
						if (num11 < startY)
						{
							continue;
						}
						if (num11 >= stopY)
						{
							break;
						}
						for (int k = 0; k < size; k++)
						{
							int num12 = k - num;
							num11 = num3 + num12;
							if (num11 >= startX && num11 < stopX)
							{
								int num13 = kernel[j, k];
								ushort* ptr3 = ptr + (num10 * srcStride + num12 * 4);
								num5 += num13;
								num9 += num13 * ptr3[2];
								num8 += num13 * ptr3[1];
								num7 += num13 * *ptr3;
								num6 += num13 * ptr3[3];
								num4++;
							}
						}
					}
					if (num4 == num2)
					{
						num5 = divisor;
					}
					else if (!dynamicDivisorForEdges)
					{
						num5 = divisor;
					}
					if (num5 != 0)
					{
						num9 /= num5;
						num8 /= num5;
						num7 /= num5;
						num6 /= num5;
					}
					num9 += threshold;
					num8 += threshold;
					num7 += threshold;
					num6 += threshold;
					ptr2[2] = (ushort)((num9 > 65535) ? 65535 : ((num9 < 0) ? 0 : num9));
					ptr2[1] = (ushort)((num8 > 65535) ? 65535 : ((num8 < 0) ? 0 : num8));
					*ptr2 = (ushort)((num7 > 65535) ? 65535 : ((num7 < 0) ? 0 : num7));
					ptr2[3] = (ushort)((num6 > 65535) ? 65535 : ((num6 < 0) ? 0 : num6));
					num3++;
					ptr += 4;
					ptr2 += 4;
				}
			}
		}
	}
}
