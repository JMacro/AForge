using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class WaterWave : BaseFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		private int xWavesCount = 5;

		private int yWavesCount = 5;

		private int xWavesAmplitude = 10;

		private int yWavesAmplitude = 10;

		public int HorizontalWavesCount
		{
			get
			{
				return xWavesCount;
			}
			set
			{
				xWavesCount = System.Math.Max(1, System.Math.Min(10000, value));
			}
		}

		public int VerticalWavesCount
		{
			get
			{
				return yWavesCount;
			}
			set
			{
				yWavesCount = System.Math.Max(1, System.Math.Min(10000, value));
			}
		}

		public int HorizontalWavesAmplitude
		{
			get
			{
				return xWavesAmplitude;
			}
			set
			{
				xWavesAmplitude = System.Math.Max(0, System.Math.Min(10000, value));
			}
		}

		public int VerticalWavesAmplitude
		{
			get
			{
				return yWavesAmplitude;
			}
			set
			{
				yWavesAmplitude = System.Math.Max(0, System.Math.Min(10000, value));
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public WaterWave()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage source, UnmanagedImage destination)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(source.PixelFormat) / 8;
			int width = source.Width;
			int height = source.Height;
			int stride = source.Stride;
			int stride2 = destination.Stride;
			int num2 = stride2 - width * num;
			int num3 = height - 1;
			int num4 = width - 1;
			byte* ptr = (byte*)source.ImageData.ToPointer();
			byte* ptr2 = (byte*)destination.ImageData.ToPointer();
			double num5 = System.Math.PI * 2.0 * (double)xWavesCount / (double)width;
			double num6 = System.Math.PI * 2.0 * (double)yWavesCount / (double)height;
			for (int i = 0; i < height; i++)
			{
				double num7 = System.Math.Sin(num6 * (double)i) * (double)yWavesAmplitude;
				for (int j = 0; j < width; j++)
				{
					double num8 = (double)j + num7;
					double num9 = (double)i + System.Math.Cos(num5 * (double)j) * (double)xWavesAmplitude;
					if (num8 >= 0.0 && num9 >= 0.0 && num8 < (double)width && num9 < (double)height)
					{
						int num10 = (int)num9;
						int num11 = (num10 == num3) ? num10 : (num10 + 1);
						double num12 = num9 - (double)num10;
						double num13 = 1.0 - num12;
						int num14 = (int)num8;
						int num15 = (num14 == num4) ? num14 : (num14 + 1);
						double num16 = num8 - (double)num14;
						double num17 = 1.0 - num16;
						byte* ptr3 = ptr + (long)num10 * (long)stride + (long)num14 * (long)num;
						byte* ptr4 = ptr + (long)num10 * (long)stride + (long)num15 * (long)num;
						byte* ptr5 = ptr + (long)num11 * (long)stride + (long)num14 * (long)num;
						byte* ptr6 = ptr + (long)num11 * (long)stride + (long)num15 * (long)num;
						int num18 = 0;
						while (num18 < num)
						{
							*ptr2 = (byte)(num13 * (num17 * (double)(int)(*ptr3) + num16 * (double)(int)(*ptr4)) + num12 * (num17 * (double)(int)(*ptr5) + num16 * (double)(int)(*ptr6)));
							num18++;
							ptr2++;
							ptr3++;
							ptr4++;
							ptr5++;
							ptr6++;
						}
					}
					else
					{
						int num19 = 0;
						while (num19 < num)
						{
							*ptr2 = 0;
							num19++;
							ptr2++;
						}
					}
				}
				ptr2 += num2;
			}
		}
	}
}
