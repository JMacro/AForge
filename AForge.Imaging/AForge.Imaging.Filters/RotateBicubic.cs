using System;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class RotateBicubic : BaseRotateFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public RotateBicubic(double angle)
			: this(angle, keepSize: false)
		{
		}

		public RotateBicubic(double angle, bool keepSize)
			: base(angle, keepSize)
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			double num = (double)(width - 1) / 2.0;
			double num2 = (double)(height - 1) / 2.0;
			int width2 = destinationData.Width;
			int height2 = destinationData.Height;
			double num3 = (double)(width2 - 1) / 2.0;
			double num4 = (double)(height2 - 1) / 2.0;
			double num5 = (0.0 - angle) * System.Math.PI / 180.0;
			double num6 = System.Math.Cos(num5);
			double num7 = System.Math.Sin(num5);
			int stride = sourceData.Stride;
			int num8 = destinationData.Stride - ((destinationData.PixelFormat == PixelFormat.Format8bppIndexed) ? width2 : (width2 * 3));
			byte r = fillColor.R;
			byte g = fillColor.G;
			byte b = fillColor.B;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			int num9 = height - 1;
			int num10 = width - 1;
			double num11;
			if (destinationData.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				num11 = 0.0 - num4;
				for (int i = 0; i < height2; i++)
				{
					double num12 = 0.0 - num3;
					int num13 = 0;
					while (num13 < width2)
					{
						double num14 = num6 * num12 + num7 * num11 + num;
						double num15 = (0.0 - num7) * num12 + num6 * num11 + num2;
						int num16 = (int)num14;
						int num17 = (int)num15;
						if (num16 < 0 || num17 < 0 || num16 >= width || num17 >= height)
						{
							*ptr2 = g;
						}
						else
						{
							double num18 = num14 - (double)num16;
							double num19 = num15 - (double)num17;
							double num20 = 0.0;
							for (int j = -1; j < 3; j++)
							{
								double num21 = Interpolation.BiCubicKernel(num19 - (double)j);
								int num22 = num17 + j;
								if (num22 < 0)
								{
									num22 = 0;
								}
								if (num22 > num9)
								{
									num22 = num9;
								}
								for (int k = -1; k < 3; k++)
								{
									double num23 = num21 * Interpolation.BiCubicKernel((double)k - num18);
									int num24 = num16 + k;
									if (num24 < 0)
									{
										num24 = 0;
									}
									if (num24 > num10)
									{
										num24 = num10;
									}
									num20 += num23 * (double)(int)ptr[num22 * stride + num24];
								}
							}
							*ptr2 = (byte)System.Math.Max(0.0, System.Math.Min(255.0, num20));
						}
						num12 += 1.0;
						num13++;
						ptr2++;
					}
					num11 += 1.0;
					ptr2 += num8;
				}
				return;
			}
			num11 = 0.0 - num4;
			for (int l = 0; l < height2; l++)
			{
				double num12 = 0.0 - num3;
				int num25 = 0;
				while (num25 < width2)
				{
					double num14 = num6 * num12 + num7 * num11 + num;
					double num15 = (0.0 - num7) * num12 + num6 * num11 + num2;
					int num16 = (int)num14;
					int num17 = (int)num15;
					if (num16 < 0 || num17 < 0 || num16 >= width || num17 >= height)
					{
						ptr2[2] = r;
						ptr2[1] = g;
						*ptr2 = b;
					}
					else
					{
						double num18 = num14 - (double)(float)num16;
						double num19 = num15 - (double)(float)num17;
						double num20;
						double num26;
						double num27 = num20 = (num26 = 0.0);
						for (int m = -1; m < 3; m++)
						{
							double num21 = Interpolation.BiCubicKernel(num19 - (double)(float)m);
							int num22 = num17 + m;
							if (num22 < 0)
							{
								num22 = 0;
							}
							if (num22 > num9)
							{
								num22 = num9;
							}
							for (int n = -1; n < 3; n++)
							{
								double num23 = num21 * Interpolation.BiCubicKernel((double)(float)n - num18);
								int num24 = num16 + n;
								if (num24 < 0)
								{
									num24 = 0;
								}
								if (num24 > num10)
								{
									num24 = num10;
								}
								byte* ptr3 = ptr + (long)num22 * (long)stride + (long)num24 * 3L;
								num27 += num23 * (double)(int)ptr3[2];
								num20 += num23 * (double)(int)ptr3[1];
								num26 += num23 * (double)(int)(*ptr3);
							}
						}
						ptr2[2] = (byte)System.Math.Max(0.0, System.Math.Min(255.0, num27));
						ptr2[1] = (byte)System.Math.Max(0.0, System.Math.Min(255.0, num20));
						*ptr2 = (byte)System.Math.Max(0.0, System.Math.Min(255.0, num26));
					}
					num12 += 1.0;
					num25++;
					ptr2 += 3;
				}
				num11 += 1.0;
				ptr2 += num8;
			}
		}
	}
}
