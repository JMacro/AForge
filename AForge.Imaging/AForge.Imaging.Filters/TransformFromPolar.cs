using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class TransformFromPolar : BaseTransformationFilter
	{
		private const double Pi2 = System.Math.PI * 2.0;

		private const double PiHalf = System.Math.PI / 2.0;

		private double circleDepth = 1.0;

		private double offsetAngle;

		private bool mapBackwards;

		private bool mapFromTop = true;

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

		public TransformFromPolar()
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
			int num4 = height2 - 1;
			double num5 = 1.0 - circleDepth;
			double num6 = (double)num2 / 2.0;
			double num7 = (double)num3 / 2.0;
			double num8 = (num6 < num7) ? num6 : num7;
			num8 -= num8 * num5;
			double num9 = System.Math.Atan2(num7, num6);
			double num10 = (mapBackwards ? offsetAngle : (0.0 - offsetAngle)) / 180.0 * System.Math.PI + System.Math.PI / 2.0;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			int stride = sourceData.Stride;
			int num11 = destinationData.Stride - width2 * num;
			double[] array = new double[width2];
			double[] array2 = new double[width2];
			double[] array3 = new double[width2];
			for (int i = 0; i < width2; i++)
			{
				double num12 = System.Math.PI * -2.0 * (double)i / (double)width2 + num10;
				array[i] = System.Math.Cos(num12);
				array2[i] = System.Math.Sin(num12);
				double num13 = ((num12 > 0.0) ? num12 : (0.0 - num12)) % System.Math.PI;
				if (num13 > System.Math.PI / 2.0)
				{
					num13 = System.Math.PI - num13;
				}
				array3[i] = num5 * ((num13 > num9) ? (num7 / System.Math.Sin(num13)) : (num6 / System.Math.Cos(num13)));
			}
			for (int j = 0; j < height2; j++)
			{
				double num14 = (double)j / (double)num4;
				if (!mapFromTop)
				{
					num14 = 1.0 - num14;
				}
				for (int k = 0; k < width2; k++)
				{
					double num15 = num8 + array3[k];
					double num16 = num14 * num15;
					double num17 = num6 + num16 * (mapBackwards ? (0.0 - array[k]) : array[k]);
					double num18 = num7 - num16 * array2[k];
					int num19 = (int)num17;
					int num20 = (int)num18;
					int num21 = (num19 == num2) ? num19 : (num19 + 1);
					double num22 = num17 - (double)num19;
					double num23 = 1.0 - num22;
					int num24 = (num20 == num3) ? num20 : (num20 + 1);
					double num25 = num18 - (double)num20;
					double num26 = 1.0 - num25;
					byte* ptr3;
					byte* ptr4 = ptr3 = ptr + (long)num20 * (long)stride;
					ptr4 += (long)num19 * (long)num;
					ptr3 += (long)num21 * (long)num;
					byte* ptr5;
					byte* ptr6 = ptr5 = ptr + (long)num24 * (long)stride;
					ptr6 += (long)num19 * (long)num;
					ptr5 += (long)num21 * (long)num;
					int num27 = 0;
					while (num27 < num)
					{
						*ptr2 = (byte)(num26 * (num23 * (double)(int)(*ptr4) + num22 * (double)(int)(*ptr3)) + num25 * (num23 * (double)(int)(*ptr6) + num22 * (double)(int)(*ptr5)));
						num27++;
						ptr2++;
						ptr4++;
						ptr3++;
						ptr6++;
						ptr5++;
					}
				}
				ptr2 += num11;
			}
		}
	}
}
