using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class AdaptiveSmoothing : BaseUsingCopyPartialFilter
	{
		private double factor = 3.0;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public double Factor
		{
			get
			{
				return factor;
			}
			set
			{
				factor = value;
			}
		}

		public AdaptiveSmoothing()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		public AdaptiveSmoothing(double factor)
			: this()
		{
			this.factor = factor;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(source.PixelFormat) / 8;
			int num2 = num * 2;
			int left = rect.Left;
			int top = rect.Top;
			int num3 = left + rect.Width;
			int num4 = top + rect.Height;
			int num5 = left + 2;
			int num6 = top + 2;
			int num7 = num3 - 2;
			int num8 = num4 - 2;
			int stride = source.Stride;
			int stride2 = destination.Stride;
			int num9 = stride - rect.Width * num;
			int num10 = stride2 - rect.Width * num;
			double num11 = -8.0 * factor * factor;
			byte* ptr = (byte*)source.ImageData.ToPointer() + (long)stride * 2L;
			byte* ptr2 = (byte*)destination.ImageData.ToPointer() + (long)stride2 * 2L;
			ptr += top * stride + left * num;
			ptr2 += top * stride2 + left * num;
			for (int i = num6; i < num8; i++)
			{
				ptr += num2;
				ptr2 += num2;
				for (int j = num5; j < num7; j++)
				{
					int num12 = 0;
					while (num12 < num)
					{
						double num13 = 0.0;
						double num14 = 0.0;
						double num15 = ptr[-stride] - ptr[-num2 - stride];
						double num16 = ptr[-num] - ptr[-num - 2 * stride];
						double num17 = System.Math.Exp((num15 * num15 + num16 * num16) / num11);
						num14 += num17 * (double)(int)ptr[-num - stride];
						num13 += num17;
						num15 = ptr[num - stride] - ptr[-num - stride];
						num16 = *ptr - ptr[-2L * (long)stride];
						num17 = System.Math.Exp((num15 * num15 + num16 * num16) / num11);
						num14 += num17 * (double)(int)ptr[-stride];
						num13 += num17;
						num15 = ptr[num2 - stride] - ptr[-stride];
						num16 = ptr[num] - ptr[num - 2 * stride];
						num17 = System.Math.Exp((num15 * num15 + num16 * num16) / num11);
						num14 += num17 * (double)(int)ptr[num - stride];
						num13 += num17;
						num15 = *ptr - ptr[-num2];
						num16 = ptr[-num + stride] - ptr[-num - stride];
						num17 = System.Math.Exp((num15 * num15 + num16 * num16) / num11);
						num14 += num17 * (double)(int)ptr[-num];
						num13 += num17;
						num15 = ptr[num] - ptr[-num];
						num16 = ptr[stride] - ptr[-stride];
						num17 = System.Math.Exp((num15 * num15 + num16 * num16) / num11);
						num14 += num17 * (double)(int)(*ptr);
						num13 += num17;
						num15 = ptr[num2] - *ptr;
						num16 = ptr[num + stride] - ptr[num - stride];
						num17 = System.Math.Exp((num15 * num15 + num16 * num16) / num11);
						num14 += num17 * (double)(int)ptr[num];
						num13 += num17;
						num15 = ptr[stride] - ptr[-num2 + stride];
						num16 = ptr[-num + 2 * stride] - ptr[-num];
						num17 = System.Math.Exp((num15 * num15 + num16 * num16) / num11);
						num14 += num17 * (double)(int)ptr[-num + stride];
						num13 += num17;
						num15 = ptr[num + stride] - ptr[-num + stride];
						num16 = ptr[2L * (long)stride] - *ptr;
						num17 = System.Math.Exp((num15 * num15 + num16 * num16) / num11);
						num14 += num17 * (double)(int)ptr[stride];
						num13 += num17;
						num15 = ptr[num2 + stride] - ptr[stride];
						num16 = ptr[num + 2 * stride] - ptr[num];
						num17 = System.Math.Exp((num15 * num15 + num16 * num16) / num11);
						num14 += num17 * (double)(int)ptr[num + stride];
						num13 += num17;
						*ptr2 = ((num13 == 0.0) ? (*ptr) : ((byte)System.Math.Min(num14 / num13, 255.0)));
						num12++;
						ptr++;
						ptr2++;
					}
				}
				ptr += num9 + num2;
				ptr2 += num10 + num2;
			}
		}
	}
}
