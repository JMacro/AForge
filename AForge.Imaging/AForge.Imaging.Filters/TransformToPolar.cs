using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class TransformToPolar : BaseTransformationFilter
	{
		private const double Pi2 = System.Math.PI * 2.0;

		private const double PiHalf = System.Math.PI / 2.0;

		private double circleDepth = 1.0;

		private double offsetAngle;

		private bool mapBackwards;

		private bool mapFromTop = true;

		private Color fillColor = Color.White;

		private Size newSize = new Size(200, 200);

		private bool useOriginalImageSize = true;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public double CirlceDepth
		{
			get
			{
				return circleDepth;
			}
			set
			{
				circleDepth = System.Math.Max(0.0, System.Math.Min(1.0, value));
			}
		}

		public double OffsetAngle
		{
			get
			{
				return offsetAngle;
			}
			set
			{
				offsetAngle = System.Math.Max(-360.0, System.Math.Min(360.0, value));
			}
		}

		public bool MapBackwards
		{
			get
			{
				return mapBackwards;
			}
			set
			{
				mapBackwards = value;
			}
		}

		public bool MapFromTop
		{
			get
			{
				return mapFromTop;
			}
			set
			{
				mapFromTop = value;
			}
		}

		public Color FillColor
		{
			get
			{
				return fillColor;
			}
			set
			{
				fillColor = value;
			}
		}

		public Size NewSize
		{
			get
			{
				return newSize;
			}
			set
			{
				newSize = new Size(System.Math.Max(1, System.Math.Min(10000, value.Width)), System.Math.Max(1, System.Math.Min(10000, value.Height)));
			}
		}

		public bool UseOriginalImageSize
		{
			get
			{
				return useOriginalImageSize;
			}
			set
			{
				useOriginalImageSize = value;
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public TransformToPolar()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
		}

		protected override Size CalculateNewImageSize(UnmanagedImage sourceData)
		{
			if (!useOriginalImageSize)
			{
				return newSize;
			}
			return new Size(sourceData.Width, sourceData.Height);
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(destinationData.PixelFormat) / 8;
			int width = sourceData.Width;
			int height = sourceData.Height;
			int num2 = width - 1;
			int num3 = height - 1;
			int width2 = destinationData.Width;
			int height2 = destinationData.Height;
			double num4 = 1.0 - circleDepth;
			double num5 = (double)(width2 - 1) / 2.0;
			double num6 = (double)(height2 - 1) / 2.0;
			double num7 = (num5 < num6) ? num5 : num6;
			num7 -= num7 * num4;
			double num8 = System.Math.Atan2(num6, num5);
			double num9 = offsetAngle / 180.0 * System.Math.PI;
			if (num9 < 0.0)
			{
				num9 = System.Math.PI * 2.0 - num9;
			}
			num9 += 7.8539816339744828;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			int stride = sourceData.Stride;
			int num10 = destinationData.Stride - width2 * num;
			byte r = fillColor.R;
			byte b = fillColor.B;
			byte g = fillColor.G;
			byte a = fillColor.A;
			byte b2 = (byte)(0.2125 * (double)(int)r + 0.7154 * (double)(int)g + 0.0721 * (double)(int)b);
			for (int i = 0; i < height2; i++)
			{
				double num11 = (double)i - num6;
				double num12 = num11 * num11;
				for (int j = 0; j < width2; j++)
				{
					double num13 = (double)j - num5;
					double num14 = System.Math.Sqrt(num13 * num13 + num12);
					double num15 = System.Math.Atan2(num11, num13);
					double num16 = (num15 > 0.0) ? num15 : (0.0 - num15);
					if (num16 > System.Math.PI / 2.0)
					{
						num16 = System.Math.PI - num16;
					}
					double num17 = (num16 > num8) ? (num6 / System.Math.Sin(num16)) : (num5 / System.Math.Cos(num16));
					double num18 = num7 + num17 * num4;
					if (num14 < num18 + 1.0)
					{
						num15 += num9;
						num15 %= System.Math.PI * 2.0;
						double num19 = num14 / num18 * (double)num3;
						if (num19 > (double)num3)
						{
							num19 = num3;
						}
						if (!mapFromTop)
						{
							num19 = (double)num3 - num19;
						}
						double num20 = num15 / (System.Math.PI * 2.0) * (double)num2;
						if (mapBackwards)
						{
							num20 = (double)num2 - num20;
						}
						int num21 = (int)num20;
						int num22 = (num21 == num2) ? num21 : (num21 + 1);
						double num23 = num20 - (double)num21;
						double num24 = 1.0 - num23;
						int num25 = (int)num19;
						int num26 = (num25 == num3) ? num25 : (num25 + 1);
						double num27 = num19 - (double)num25;
						double num28 = 1.0 - num27;
						byte* ptr3;
						byte* ptr4 = ptr3 = ptr + (long)num25 * (long)stride;
						ptr4 += (long)num21 * (long)num;
						ptr3 += (long)num22 * (long)num;
						byte* ptr5;
						byte* ptr6 = ptr5 = ptr + (long)num26 * (long)stride;
						ptr6 += (long)num21 * (long)num;
						ptr5 += (long)num22 * (long)num;
						int num29 = 0;
						while (num29 < num)
						{
							*ptr2 = (byte)(num28 * (num24 * (double)(int)(*ptr4) + num23 * (double)(int)(*ptr3)) + num27 * (num24 * (double)(int)(*ptr6) + num23 * (double)(int)(*ptr5)));
							num29++;
							ptr2++;
							ptr4++;
							ptr3++;
							ptr6++;
							ptr5++;
						}
						continue;
					}
					if (num == 1)
					{
						*ptr2 = b2;
					}
					else
					{
						ptr2[2] = r;
						ptr2[1] = g;
						*ptr2 = b;
						if (num > 3)
						{
							ptr2[3] = a;
						}
					}
					ptr2 += num;
				}
				ptr2 += num10;
			}
		}
	}
}
