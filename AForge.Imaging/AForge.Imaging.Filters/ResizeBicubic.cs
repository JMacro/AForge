using System;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ResizeBicubic : BaseResizeFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public ResizeBicubic(int newWidth, int newHeight)
			: base(newWidth, newHeight)
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			int num = (sourceData.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int stride = sourceData.Stride;
			int num2 = destinationData.Stride - num * newWidth;
			double num3 = (double)width / (double)newWidth;
			double num4 = (double)height / (double)newHeight;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			int num5 = height - 1;
			int num6 = width - 1;
			if (destinationData.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				for (int i = 0; i < newHeight; i++)
				{
					double num7 = (double)i * num4 - 0.5;
					int num8 = (int)num7;
					double num9 = num7 - (double)num8;
					int num10 = 0;
					while (num10 < newWidth)
					{
						double num11 = (double)num10 * num3 - 0.5;
						int num12 = (int)num11;
						double num13 = num11 - (double)num12;
						double num14 = 0.0;
						for (int j = -1; j < 3; j++)
						{
							double num15 = Interpolation.BiCubicKernel(num9 - (double)j);
							int num16 = num8 + j;
							if (num16 < 0)
							{
								num16 = 0;
							}
							if (num16 > num5)
							{
								num16 = num5;
							}
							for (int k = -1; k < 3; k++)
							{
								double num17 = num15 * Interpolation.BiCubicKernel((double)k - num13);
								int num18 = num12 + k;
								if (num18 < 0)
								{
									num18 = 0;
								}
								if (num18 > num6)
								{
									num18 = num6;
								}
								num14 += num17 * (double)(int)ptr[num16 * stride + num18];
							}
						}
						*ptr2 = (byte)System.Math.Max(0.0, System.Math.Min(255.0, num14));
						num10++;
						ptr2++;
					}
					ptr2 += num2;
				}
				return;
			}
			for (int l = 0; l < newHeight; l++)
			{
				double num7 = (double)l * num4 - 0.5;
				int num8 = (int)num7;
				double num9 = num7 - (double)num8;
				int num19 = 0;
				while (num19 < newWidth)
				{
					double num11 = (double)num19 * num3 - 0.5;
					int num12 = (int)num11;
					double num13 = num11 - (double)num12;
					double num14;
					double num20;
					double num21 = num14 = (num20 = 0.0);
					for (int m = -1; m < 3; m++)
					{
						double num15 = Interpolation.BiCubicKernel(num9 - (double)m);
						int num16 = num8 + m;
						if (num16 < 0)
						{
							num16 = 0;
						}
						if (num16 > num5)
						{
							num16 = num5;
						}
						for (int n = -1; n < 3; n++)
						{
							double num17 = num15 * Interpolation.BiCubicKernel((double)n - num13);
							int num18 = num12 + n;
							if (num18 < 0)
							{
								num18 = 0;
							}
							if (num18 > num6)
							{
								num18 = num6;
							}
							byte* ptr3 = ptr + (long)num16 * (long)stride + (long)num18 * 3L;
							num21 += num17 * (double)(int)ptr3[2];
							num14 += num17 * (double)(int)ptr3[1];
							num20 += num17 * (double)(int)(*ptr3);
						}
					}
					ptr2[2] = (byte)System.Math.Max(0.0, System.Math.Min(255.0, num21));
					ptr2[1] = (byte)System.Math.Max(0.0, System.Math.Min(255.0, num14));
					*ptr2 = (byte)System.Math.Max(0.0, System.Math.Min(255.0, num20));
					num19++;
					ptr2 += 3;
				}
				ptr2 += num2;
			}
		}
	}
}
