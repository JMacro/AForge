using System;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class RotateBilinear : BaseRotateFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public RotateBilinear(double angle)
			: this(angle, keepSize: false)
		{
		}

		public RotateBilinear(double angle, bool keepSize)
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
					double num12 = num7 * num11 + num;
					double num13 = num6 * num11 + num2;
					double num14 = 0.0 - num3;
					int num15 = 0;
					while (num15 < width2)
					{
						double num16 = num12 + num6 * num14;
						double num17 = num13 - num7 * num14;
						int num18 = (int)num16;
						int num19 = (int)num17;
						if (num18 < 0 || num19 < 0 || num18 >= width || num19 >= height)
						{
							*ptr2 = g;
						}
						else
						{
							int num20 = (num18 == num10) ? num18 : (num18 + 1);
							int num21 = (num19 == num9) ? num19 : (num19 + 1);
							double num22;
							if ((num22 = num16 - (double)num18) < 0.0)
							{
								num22 = 0.0;
							}
							double num23 = 1.0 - num22;
							double num24;
							if ((num24 = num17 - (double)num19) < 0.0)
							{
								num24 = 0.0;
							}
							double num25 = 1.0 - num24;
							byte* ptr3 = ptr + (long)num19 * (long)stride;
							byte* ptr4 = ptr + (long)num21 * (long)stride;
							*ptr2 = (byte)(num25 * (num23 * (double)(int)ptr3[num18] + num22 * (double)(int)ptr3[num20]) + num24 * (num23 * (double)(int)ptr4[num18] + num22 * (double)(int)ptr4[num20]));
						}
						num14 += 1.0;
						num15++;
						ptr2++;
					}
					num11 += 1.0;
					ptr2 += num8;
				}
				return;
			}
			num11 = 0.0 - num4;
			for (int j = 0; j < height2; j++)
			{
				double num12 = num7 * num11 + num;
				double num13 = num6 * num11 + num2;
				double num14 = 0.0 - num3;
				int num26 = 0;
				while (num26 < width2)
				{
					double num16 = num12 + num6 * num14;
					double num17 = num13 - num7 * num14;
					int num18 = (int)num16;
					int num19 = (int)num17;
					if (num18 < 0 || num19 < 0 || num18 >= width || num19 >= height)
					{
						ptr2[2] = r;
						ptr2[1] = g;
						*ptr2 = b;
					}
					else
					{
						int num20 = (num18 == num10) ? num18 : (num18 + 1);
						int num21 = (num19 == num9) ? num19 : (num19 + 1);
						double num22;
						if ((num22 = num16 - (double)(float)num18) < 0.0)
						{
							num22 = 0.0;
						}
						double num23 = 1.0 - num22;
						double num24;
						if ((num24 = num17 - (double)(float)num19) < 0.0)
						{
							num24 = 0.0;
						}
						double num25 = 1.0 - num24;
						byte* ptr4;
						byte* ptr3 = ptr4 = ptr + (long)num19 * (long)stride;
						ptr3 += (long)num18 * 3L;
						ptr4 += (long)num20 * 3L;
						byte* ptr5;
						byte* ptr6 = ptr5 = ptr + (long)num21 * (long)stride;
						ptr6 += (long)num18 * 3L;
						ptr5 += (long)num20 * 3L;
						ptr2[2] = (byte)(num25 * (num23 * (double)(int)ptr3[2] + num22 * (double)(int)ptr4[2]) + num24 * (num23 * (double)(int)ptr6[2] + num22 * (double)(int)ptr5[2]));
						ptr2[1] = (byte)(num25 * (num23 * (double)(int)ptr3[1] + num22 * (double)(int)ptr4[1]) + num24 * (num23 * (double)(int)ptr6[1] + num22 * (double)(int)ptr5[1]));
						*ptr2 = (byte)(num25 * (num23 * (double)(int)(*ptr3) + num22 * (double)(int)(*ptr4)) + num24 * (num23 * (double)(int)(*ptr6) + num22 * (double)(int)(*ptr5)));
					}
					num14 += 1.0;
					num26++;
					ptr2 += 3;
				}
				num11 += 1.0;
				ptr2 += num8;
			}
		}
	}
}
