using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class BilateralSmoothing : BaseUsingCopyPartialFilter
	{
		private const int maxKernelSize = 255;

		private const int colorsCount = 256;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		private int kernelSize = 9;

		private double spatialFactor = 10.0;

		private double spatialPower = 2.0;

		private double colorFactor = 50.0;

		private double colorPower = 2.0;

		private bool spatialPropertiesChanged = true;

		private bool colorPropertiesChanged = true;

		private bool limitKernelSize = true;

		private bool enableParallelProcessing;

		private double[,] spatialFunc;

		private double[,] colorFunc;

		public bool LimitKernelSize
		{
			get
			{
				return limitKernelSize;
			}
			set
			{
				limitKernelSize = value;
			}
		}

		public bool EnableParallelProcessing
		{
			get
			{
				return enableParallelProcessing;
			}
			set
			{
				enableParallelProcessing = value;
			}
		}

		public int KernelSize
		{
			get
			{
				return kernelSize;
			}
			set
			{
				if (value > 255)
				{
					throw new ArgumentOutOfRangeException("Maximum allowed value of KernelSize property is " + 255.ToString());
				}
				if (limitKernelSize && value > 25)
				{
					throw new ArgumentOutOfRangeException("KernerlSize is larger then 25. Time for applying is significant and may lead to application freezing. In order to use any KernelSize value set property 'LimitKernelSize' to false.");
				}
				if (value < 3)
				{
					throw new ArgumentOutOfRangeException("KernelSize must be greater than 3");
				}
				if (value % 2 == 0)
				{
					throw new ArgumentException("KernerlSize must be an odd integer.");
				}
				kernelSize = value;
			}
		}

		public double SpatialFactor
		{
			get
			{
				return spatialFactor;
			}
			set
			{
				spatialFactor = System.Math.Max(1.0, value);
				spatialPropertiesChanged = true;
			}
		}

		public double SpatialPower
		{
			get
			{
				return spatialPower;
			}
			set
			{
				spatialPower = System.Math.Max(1.0, value);
				spatialPropertiesChanged = true;
			}
		}

		public double ColorFactor
		{
			get
			{
				return colorFactor;
			}
			set
			{
				colorFactor = System.Math.Max(1.0, value);
				colorPropertiesChanged = true;
			}
		}

		public double ColorPower
		{
			get
			{
				return colorPower;
			}
			set
			{
				colorPower = System.Math.Max(1.0, value);
				colorPropertiesChanged = true;
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public BilateralSmoothing()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		private void InitSpatialFunc()
		{
			if (spatialFunc != null && spatialFunc.Length == kernelSize * kernelSize && !spatialPropertiesChanged)
			{
				return;
			}
			if (spatialFunc == null || spatialFunc.Length != kernelSize * kernelSize)
			{
				spatialFunc = new double[kernelSize, kernelSize];
			}
			int num = kernelSize / 2;
			for (int i = 0; i < kernelSize; i++)
			{
				int num2 = i - num;
				int num3 = num2 * num2;
				for (int j = 0; j < kernelSize; j++)
				{
					int num4 = j - num;
					int num5 = num4 * num4;
					spatialFunc[i, j] = System.Math.Exp(-0.5 * System.Math.Pow(System.Math.Sqrt((double)(num3 + num5) / spatialFactor), spatialPower));
				}
			}
			spatialPropertiesChanged = false;
		}

		private void InitColorFunc()
		{
			if (colorFunc != null && !colorPropertiesChanged)
			{
				return;
			}
			if (colorFunc == null)
			{
				colorFunc = new double[256, 256];
			}
			for (int i = 0; i < 256; i++)
			{
				for (int j = 0; j < 256; j++)
				{
					colorFunc[i, j] = System.Math.Exp(-0.5 * System.Math.Pow((double)System.Math.Abs(i - j) / colorFactor, colorPower));
				}
			}
			colorPropertiesChanged = false;
		}

		private void InitFilter()
		{
			InitSpatialFunc();
			InitColorFunc();
		}

		protected override void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
		{
			int num = kernelSize / 2;
			InitFilter();
			if (rect.Width <= kernelSize || rect.Height <= kernelSize)
			{
				ProcessWithEdgeChecks(source, destination, rect);
				return;
			}
			Rectangle rect2 = rect;
			rect2.Inflate(-num, -num);
			if (Environment.ProcessorCount > 1 && enableParallelProcessing)
			{
				ProcessWithoutChecksParallel(source, destination, rect2);
			}
			else
			{
				ProcessWithoutChecks(source, destination, rect2);
			}
			ProcessWithEdgeChecks(source, destination, new Rectangle(rect.Left, rect.Top, rect.Width, num));
			ProcessWithEdgeChecks(source, destination, new Rectangle(rect.Left, rect.Bottom - num, rect.Width, num));
			ProcessWithEdgeChecks(source, destination, new Rectangle(rect.Left, rect.Top + num, num, rect.Height - num * 2));
			ProcessWithEdgeChecks(source, destination, new Rectangle(rect.Right - num, rect.Top + num, num, rect.Height - num * 2));
		}

		private unsafe void ProcessWithoutChecksParallel(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
		{
			int startX = rect.Left;
			int top = rect.Top;
			int stopX = rect.Right;
			int bottom = rect.Bottom;
			int pixelSize = System.Drawing.Image.GetPixelFormatSize(source.PixelFormat) / 8;
			int num = kernelSize / 2;
			int num2 = kernelSize * pixelSize;
			int srcStride = source.Stride;
			int dstStride = destination.Stride;
			_ = srcStride;
			_ = rect.Width;
			_ = pixelSize;
			_ = dstStride;
			_ = rect.Width;
			_ = pixelSize;
			int srcKernelFistPixelOffset = num * (srcStride + pixelSize);
			int srcKernelOffset = srcStride - num2;
			byte* srcBase = (byte*)source.ImageData.ToPointer();
			byte* dstBase = (byte*)destination.ImageData.ToPointer();
			srcBase += (long)(IntPtr)(void*)((long)startX * (long)pixelSize);
			dstBase += (long)(IntPtr)(void*)((long)startX * (long)pixelSize);
			if (pixelSize > 1)
			{
				Parallel.For(top, bottom, delegate(int y)
				{
					byte* ptr4 = srcBase + (long)y * (long)srcStride;
					byte* ptr5 = dstBase + (long)y * (long)dstStride;
					int num9 = startX;
					while (num9 < stopX)
					{
						byte* ptr6 = ptr4 + srcKernelFistPixelOffset;
						double num10 = 0.0;
						double num11 = 0.0;
						double num12 = 0.0;
						double num13 = 0.0;
						double num14 = 0.0;
						double num15 = 0.0;
						byte b3 = ptr4[2];
						byte b4 = ptr4[1];
						byte b5 = *ptr4;
						int num16 = kernelSize;
						while (num16 != 0)
						{
							num16--;
							int num17 = kernelSize;
							while (num17 != 0)
							{
								num17--;
								byte b6 = ptr6[2];
								byte b7 = ptr6[1];
								byte b8 = *ptr6;
								double num18 = spatialFunc[num17, num16] * colorFunc[b6, b3];
								double num19 = spatialFunc[num17, num16] * colorFunc[b7, b4];
								double num20 = spatialFunc[num17, num16] * colorFunc[b8, b5];
								num10 += num18;
								num11 += num19;
								num12 += num20;
								num13 += num18 * (double)(int)b6;
								num14 += num19 * (double)(int)b7;
								num15 += num20 * (double)(int)b8;
								ptr6 -= pixelSize;
							}
							ptr6 -= srcKernelOffset;
						}
						ptr5[2] = (byte)(num13 / num10);
						ptr5[1] = (byte)(num14 / num11);
						*ptr5 = (byte)(num15 / num12);
						num9++;
						ptr4 += pixelSize;
						ptr5 += pixelSize;
					}
				});
			}
			else
			{
				Parallel.For(top, bottom, delegate(int y)
				{
					byte* ptr = srcBase + (long)y * (long)srcStride;
					byte* ptr2 = dstBase + (long)y * (long)dstStride;
					int num3 = startX;
					while (num3 < stopX)
					{
						byte* ptr3 = ptr + srcKernelFistPixelOffset;
						double num4 = 0.0;
						double num5 = 0.0;
						byte b = *ptr;
						int num6 = kernelSize;
						while (num6 != 0)
						{
							num6--;
							int num7 = kernelSize;
							while (num7 != 0)
							{
								num7--;
								byte b2 = *ptr3;
								double num8 = spatialFunc[num7, num6] * colorFunc[b2, b];
								num4 += num8;
								num5 += num8 * (double)(int)b2;
								ptr3 -= pixelSize;
							}
							ptr3 -= srcKernelOffset;
						}
						*ptr2 = (byte)(num5 / num4);
						num3++;
						ptr++;
						ptr2++;
					}
				});
			}
		}

		private unsafe void ProcessWithoutChecks(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
		{
			int left = rect.Left;
			int top = rect.Top;
			int right = rect.Right;
			int bottom = rect.Bottom;
			int num = System.Drawing.Image.GetPixelFormatSize(source.PixelFormat) / 8;
			int num2 = kernelSize / 2;
			int num3 = kernelSize * num;
			int stride = source.Stride;
			int stride2 = destination.Stride;
			int num4 = stride - rect.Width * num;
			int num5 = stride2 - rect.Width * num;
			int num6 = num2 * (stride + num);
			int num7 = stride - num3;
			byte* ptr = (byte*)source.ImageData.ToPointer();
			byte* ptr2 = (byte*)destination.ImageData.ToPointer();
			ptr += top * stride + left * num;
			ptr2 += top * stride2 + left * num;
			if (num > 1)
			{
				for (int i = top; i < bottom; i++)
				{
					int num8 = left;
					while (num8 < right)
					{
						byte* ptr3 = ptr + num6;
						double num9 = 0.0;
						double num10 = 0.0;
						double num11 = 0.0;
						double num12 = 0.0;
						double num13 = 0.0;
						double num14 = 0.0;
						byte b = ptr[2];
						byte b2 = ptr[1];
						byte b3 = *ptr;
						int num15 = kernelSize;
						while (num15 != 0)
						{
							num15--;
							int num16 = kernelSize;
							while (num16 != 0)
							{
								num16--;
								byte b4 = ptr3[2];
								byte b5 = ptr3[1];
								byte b6 = *ptr3;
								double num17 = spatialFunc[num16, num15] * colorFunc[b4, b];
								double num18 = spatialFunc[num16, num15] * colorFunc[b5, b2];
								double num19 = spatialFunc[num16, num15] * colorFunc[b6, b3];
								num9 += num17;
								num10 += num18;
								num11 += num19;
								num12 += num17 * (double)(int)b4;
								num13 += num18 * (double)(int)b5;
								num14 += num19 * (double)(int)b6;
								ptr3 -= num;
							}
							ptr3 -= num7;
						}
						ptr2[2] = (byte)(num12 / num9);
						ptr2[1] = (byte)(num13 / num10);
						*ptr2 = (byte)(num14 / num11);
						num8++;
						ptr += num;
						ptr2 += num;
					}
					ptr += num4;
					ptr2 += num5;
				}
				return;
			}
			for (int j = top; j < bottom; j++)
			{
				int num20 = left;
				while (num20 < right)
				{
					byte* ptr4 = ptr + num6;
					double num21 = 0.0;
					double num22 = 0.0;
					byte b7 = *ptr;
					int num15 = kernelSize;
					while (num15 != 0)
					{
						num15--;
						int num16 = kernelSize;
						while (num16 != 0)
						{
							num16--;
							byte b8 = *ptr4;
							double num23 = spatialFunc[num16, num15] * colorFunc[b8, b7];
							num21 += num23;
							num22 += num23 * (double)(int)b8;
							ptr4 -= num;
						}
						ptr4 -= num7;
					}
					*ptr2 = (byte)(num22 / num21);
					num20++;
					ptr++;
					ptr2++;
				}
				ptr += num4;
				ptr2 += num5;
			}
		}

		private unsafe void ProcessWithEdgeChecks(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
		{
			int width = source.Width;
			int height = source.Height;
			int left = rect.Left;
			int top = rect.Top;
			int right = rect.Right;
			int bottom = rect.Bottom;
			int num = System.Drawing.Image.GetPixelFormatSize(source.PixelFormat) / 8;
			int num2 = kernelSize / 2;
			int num3 = kernelSize * num;
			int stride = source.Stride;
			int stride2 = destination.Stride;
			int num4 = stride - rect.Width * num;
			int num5 = stride2 - rect.Width * num;
			int num6 = num2 * (stride + num);
			int num7 = stride - num3;
			byte* ptr = (byte*)source.ImageData.ToPointer();
			byte* ptr2 = (byte*)destination.ImageData.ToPointer();
			ptr += top * stride + left * num;
			ptr2 += top * stride2 + left * num;
			if (num > 1)
			{
				for (int i = top; i < bottom; i++)
				{
					int num8 = left;
					while (num8 < right)
					{
						byte* ptr3 = ptr + num6;
						double num9 = 0.0;
						double num10 = 0.0;
						double num11 = 0.0;
						double num12 = 0.0;
						double num13 = 0.0;
						double num14 = 0.0;
						byte b = ptr[2];
						byte b2 = ptr[1];
						byte b3 = *ptr;
						int num15 = kernelSize;
						while (num15 != 0)
						{
							num15--;
							int num16 = num15 - num2;
							if (num16 + i >= height || num16 + i < 0)
							{
								ptr3 -= stride;
								continue;
							}
							int num17 = kernelSize;
							while (num17 != 0)
							{
								num17--;
								int num18 = num17 - num2;
								if (num18 + num8 >= width || num18 + num8 < 0)
								{
									ptr3 -= num;
									continue;
								}
								byte b4 = ptr3[2];
								byte b5 = ptr3[1];
								byte b6 = *ptr3;
								double num19 = spatialFunc[num17, num15] * colorFunc[b4, b];
								double num20 = spatialFunc[num17, num15] * colorFunc[b5, b2];
								double num21 = spatialFunc[num17, num15] * colorFunc[b6, b3];
								num9 += num19;
								num10 += num20;
								num11 += num21;
								num12 += num19 * (double)(int)b4;
								num13 += num20 * (double)(int)b5;
								num14 += num21 * (double)(int)b6;
								ptr3 -= num;
							}
							ptr3 -= num7;
						}
						ptr2[2] = (byte)(num12 / num9);
						ptr2[1] = (byte)(num13 / num10);
						*ptr2 = (byte)(num14 / num11);
						num8++;
						ptr += num;
						ptr2 += num;
					}
					ptr += num4;
					ptr2 += num5;
				}
				return;
			}
			for (int j = top; j < bottom; j++)
			{
				int num22 = left;
				while (num22 < right)
				{
					byte* ptr4 = ptr + num6;
					double num23 = 0.0;
					double num24 = 0.0;
					byte b7 = *ptr;
					int num15 = kernelSize;
					while (num15 != 0)
					{
						num15--;
						int num16 = num15 - num2;
						if (num16 + j >= height || num16 + j < 0)
						{
							ptr4 -= stride;
							continue;
						}
						int num17 = kernelSize;
						while (num17 != 0)
						{
							num17--;
							int num18 = num17 - num2;
							if (num18 + num22 >= source.Width || num18 + num22 < 0)
							{
								ptr4 -= num;
								continue;
							}
							byte b8 = *ptr4;
							double num25 = spatialFunc[num17, num15] * colorFunc[b8, b7];
							num23 += num25;
							num24 += num25 * (double)(int)b8;
							ptr4 -= num;
						}
						ptr4 -= num7;
					}
					*ptr2 = (byte)(num24 / num23);
					num22++;
					ptr++;
					ptr2++;
				}
				ptr += num4;
				ptr2 += num5;
			}
		}
	}
}
