using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class RotateNearestNeighbor : BaseRotateFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public RotateNearestNeighbor(double angle)
			: this(angle, keepSize: false)
		{
		}

		public RotateNearestNeighbor(double angle, bool keepSize)
			: base(angle, keepSize)
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
		}

		protected override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			switch (System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8)
			{
			case 4:
			case 5:
				break;
			case 1:
			case 3:
				ProcessFilter8bpc(sourceData, destinationData);
				break;
			case 2:
			case 6:
				ProcessFilter16bpc(sourceData, destinationData);
				break;
			}
		}

		private unsafe void ProcessFilter8bpc(UnmanagedImage sourceData, UnmanagedImage destinationData)
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
			double num9;
			if (destinationData.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				num9 = 0.0 - num4;
				for (int i = 0; i < height2; i++)
				{
					double num10 = 0.0 - num3;
					int num11 = 0;
					while (num11 < width2)
					{
						int num12 = (int)(num6 * num10 + num7 * num9 + num);
						int num13 = (int)((0.0 - num7) * num10 + num6 * num9 + num2);
						if (num12 < 0 || num13 < 0 || num12 >= width || num13 >= height)
						{
							*ptr2 = g;
						}
						else
						{
							*ptr2 = ptr[num13 * stride + num12];
						}
						num10 += 1.0;
						num11++;
						ptr2++;
					}
					num9 += 1.0;
					ptr2 += num8;
				}
				return;
			}
			num9 = 0.0 - num4;
			for (int j = 0; j < height2; j++)
			{
				double num10 = 0.0 - num3;
				int num14 = 0;
				while (num14 < width2)
				{
					int num12 = (int)(num6 * num10 + num7 * num9 + num);
					int num13 = (int)((0.0 - num7) * num10 + num6 * num9 + num2);
					if (num12 < 0 || num13 < 0 || num12 >= width || num13 >= height)
					{
						ptr2[2] = r;
						ptr2[1] = g;
						*ptr2 = b;
					}
					else
					{
						byte* ptr3 = ptr + (long)num13 * (long)stride + (long)num12 * 3L;
						ptr2[2] = ptr3[2];
						ptr2[1] = ptr3[1];
						*ptr2 = *ptr3;
					}
					num10 += 1.0;
					num14++;
					ptr2 += 3;
				}
				num9 += 1.0;
				ptr2 += num8;
			}
		}

		private unsafe void ProcessFilter16bpc(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			double num = (double)width / 2.0;
			double num2 = (double)height / 2.0;
			int width2 = destinationData.Width;
			int height2 = destinationData.Height;
			double num3 = (double)width2 / 2.0;
			double num4 = (double)height2 / 2.0;
			double num5 = (0.0 - angle) * System.Math.PI / 180.0;
			double num6 = System.Math.Cos(num5);
			double num7 = System.Math.Sin(num5);
			int stride = sourceData.Stride;
			int stride2 = destinationData.Stride;
			ushort num8 = (ushort)(fillColor.R << 8);
			ushort num9 = (ushort)(fillColor.G << 8);
			ushort num10 = (ushort)(fillColor.B << 8);
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			double num11;
			if (destinationData.PixelFormat == PixelFormat.Format16bppGrayScale)
			{
				num11 = 0.0 - num4;
				for (int i = 0; i < height2; i++)
				{
					ushort* ptr3 = (ushort*)(ptr2 + (long)i * (long)stride2);
					double num12 = 0.0 - num3;
					int num13 = 0;
					while (num13 < width2)
					{
						int num14 = (int)(num6 * num12 + num7 * num11 + num);
						int num15 = (int)((0.0 - num7) * num12 + num6 * num11 + num2);
						if (num14 < 0 || num15 < 0 || num14 >= width || num15 >= height)
						{
							*ptr3 = num9;
						}
						else
						{
							ushort* ptr4 = (ushort*)(ptr + (long)num15 * (long)stride + (long)num14 * 2L);
							*ptr3 = *ptr4;
						}
						num12 += 1.0;
						num13++;
						ptr3++;
					}
					num11 += 1.0;
				}
				return;
			}
			num11 = 0.0 - num4;
			for (int j = 0; j < height2; j++)
			{
				ushort* ptr5 = (ushort*)(ptr2 + (long)j * (long)stride2);
				double num12 = 0.0 - num3;
				int num16 = 0;
				while (num16 < width2)
				{
					int num14 = (int)(num6 * num12 + num7 * num11 + num);
					int num15 = (int)((0.0 - num7) * num12 + num6 * num11 + num2);
					if (num14 < 0 || num15 < 0 || num14 >= width || num15 >= height)
					{
						ptr5[2] = num8;
						ptr5[1] = num9;
						*ptr5 = num10;
					}
					else
					{
						ushort* ptr4 = (ushort*)(ptr + (long)num15 * (long)stride + (long)num14 * 6L);
						ptr5[2] = ptr4[2];
						ptr5[1] = ptr4[1];
						*ptr5 = *ptr4;
					}
					num12 += 1.0;
					num16++;
					ptr5 += 3;
				}
				num11 += 1.0;
			}
		}
	}
}
