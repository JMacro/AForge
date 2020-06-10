using System.Collections.Generic;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class BayerFilterOptimized : BaseFilter
	{
		private BayerPattern bayerPattern;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public BayerPattern Pattern
		{
			get
			{
				return bayerPattern;
			}
			set
			{
				bayerPattern = value;
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public BayerFilterOptimized()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format24bppRgb;
		}

		protected override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			if ((width & 1) == 1 || (height & 1) == 1 || width < 2 || height < 2)
			{
				throw new InvalidImagePropertiesException("Source image must have even width and height. Width and height can not be smaller than 2.");
			}
			switch (bayerPattern)
			{
			case BayerPattern.GRBG:
				ApplyGRBG(sourceData, destinationData);
				break;
			case BayerPattern.BGGR:
				ApplyBGGR(sourceData, destinationData);
				break;
			}
		}

		private unsafe void ApplyGRBG(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			int num = width - 1;
			int num2 = height - 1;
			int stride = sourceData.Stride;
			int stride2 = destinationData.Stride;
			int num3 = stride + 1;
			int num4 = stride - 1;
			int num5 = -stride;
			int num6 = num5 + 1;
			int num7 = num5 - 1;
			int num8 = stride - width;
			int num9 = stride2 - width * 3;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			ptr2[2] = ptr[1];
			ptr2[1] = (byte)(*ptr + ptr[num3] >> 1);
			*ptr2 = ptr[stride];
			ptr++;
			ptr2 += 3;
			for (int i = 1; i < num; i += 2)
			{
				ptr2[2] = *ptr;
				ptr2[1] = (byte)((ptr[stride] + ptr[-1] + ptr[1]) / 3);
				*ptr2 = (byte)(ptr[num4] + ptr[num3] >> 1);
				ptr++;
				ptr2 += 3;
				ptr2[2] = (byte)(ptr[-1] + ptr[1] >> 1);
				ptr2[1] = (byte)((*ptr + ptr[num4] + ptr[num3]) / 3);
				*ptr2 = ptr[stride];
				ptr++;
				ptr2 += 3;
			}
			ptr2[2] = *ptr;
			ptr2[1] = (byte)(ptr[-1] + ptr[stride] >> 1);
			*ptr2 = ptr[num4];
			ptr += num8 + 1;
			ptr2 += num9 + 3;
			for (int j = 1; j < num2; j += 2)
			{
				ptr2[2] = (byte)(ptr[num6] + ptr[num3] >> 1);
				ptr2[1] = (byte)((ptr[num5] + ptr[stride] + ptr[1]) / 3);
				*ptr2 = *ptr;
				ptr2 += stride2;
				ptr += stride;
				ptr2[2] = ptr[1];
				ptr2[1] = (byte)((*ptr + ptr[num6] + ptr[num3]) / 3);
				*ptr2 = (byte)(ptr[num5] + ptr[stride] >> 1);
				ptr2 -= stride2;
				ptr -= stride;
				ptr++;
				ptr2 += 3;
				for (int k = 1; k < num; k += 2)
				{
					ptr2[2] = (byte)(ptr[num5] + ptr[stride] >> 1);
					ptr2[1] = (byte)((*ptr + ptr[num7] + ptr[num6] + ptr[num4] + ptr[num3]) / 5);
					*ptr2 = (byte)(ptr[-1] + ptr[1] >> 1);
					ptr2 += stride2;
					ptr += stride;
					ptr2[2] = *ptr;
					ptr2[1] = (byte)(ptr[num5] + ptr[stride] + ptr[-1] + ptr[1] >> 2);
					*ptr2 = (byte)(ptr[num7] + ptr[num6] + ptr[num4] + ptr[num3] >> 2);
					ptr2 += 3;
					ptr++;
					ptr2[2] = (byte)(ptr[-1] + ptr[1] >> 1);
					ptr2[1] = (byte)((*ptr + ptr[num7] + ptr[num6] + ptr[num4] + ptr[num3]) / 5);
					*ptr2 = (byte)(ptr[num5] + ptr[stride] >> 1);
					ptr2 -= stride2;
					ptr -= stride;
					ptr2[2] = (byte)(ptr[num7] + ptr[num6] + ptr[num4] + ptr[num3] >> 2);
					ptr2[1] = (byte)(ptr[num5] + ptr[stride] + ptr[-1] + ptr[1] >> 2);
					*ptr2 = *ptr;
					ptr2 += 3;
					ptr++;
				}
				ptr2[2] = (byte)(ptr[num5] + ptr[stride] >> 1);
				ptr2[1] = (byte)((*ptr + ptr[num7] + ptr[num4]) / 3);
				*ptr2 = ptr[-1];
				ptr += stride;
				ptr2 += stride2;
				ptr2[2] = *ptr;
				ptr2[1] = (byte)((ptr[num5] + ptr[stride] + ptr[-1]) / 3);
				*ptr2 = (byte)(ptr[num7] + ptr[num4] >> 1);
				ptr += num8 + 1;
				ptr2 += num9 + 3;
			}
			ptr2[2] = ptr[num6];
			ptr2[1] = (byte)(ptr[num5] + ptr[1] >> 1);
			*ptr2 = *ptr;
			ptr++;
			ptr2 += 3;
			for (int l = 1; l < num; l += 2)
			{
				ptr2[2] = ptr[num5];
				ptr2[1] = (byte)((ptr[num7] + ptr[num6] + *ptr) / 3);
				*ptr2 = (byte)(ptr[-1] + ptr[1] >> 1);
				ptr++;
				ptr2 += 3;
				ptr2[2] = (byte)(ptr[num7] + ptr[num6] >> 1);
				ptr2[1] = (byte)((ptr[num5] + ptr[-1] + ptr[1]) / 3);
				*ptr2 = *ptr;
				ptr++;
				ptr2 += 3;
			}
			ptr2[2] = ptr[num5];
			ptr2[1] = (byte)(*ptr + ptr[num7] >> 1);
			*ptr2 = ptr[-1];
		}

		private unsafe void ApplyBGGR(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			int num = width - 1;
			int num2 = height - 1;
			int stride = sourceData.Stride;
			int stride2 = destinationData.Stride;
			int num3 = stride + 1;
			int num4 = stride - 1;
			int num5 = -stride;
			int num6 = num5 + 1;
			int num7 = num5 - 1;
			int num8 = stride - width;
			int num9 = stride2 - width * 3;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			ptr2[2] = ptr[num3];
			ptr2[1] = (byte)(ptr[1] + ptr[stride] >> 1);
			*ptr2 = *ptr;
			ptr++;
			ptr2 += 3;
			for (int i = 1; i < num; i += 2)
			{
				ptr2[2] = ptr[stride];
				ptr2[1] = (byte)((*ptr + ptr[num4] + ptr[num3]) / 3);
				*ptr2 = (byte)(ptr[-1] + ptr[1] >> 1);
				ptr++;
				ptr2 += 3;
				ptr2[2] = (byte)(ptr[num4] + ptr[num3] >> 1);
				ptr2[1] = (byte)((ptr[-1] + ptr[stride] + ptr[1]) / 3);
				*ptr2 = *ptr;
				ptr++;
				ptr2 += 3;
			}
			ptr2[2] = ptr[stride];
			ptr2[1] = (byte)(*ptr + ptr[num4] >> 1);
			*ptr2 = ptr[-1];
			ptr += num8 + 1;
			ptr2 += num9 + 3;
			for (int j = 1; j < num2; j += 2)
			{
				ptr2[2] = ptr[1];
				ptr2[1] = (byte)((ptr[num6] + ptr[num3] + *ptr) / 3);
				*ptr2 = (byte)(ptr[num5] + ptr[stride] >> 1);
				ptr2 += stride2;
				ptr += stride;
				ptr2[2] = (byte)(ptr[num6] + ptr[num3] >> 1);
				ptr2[1] = (byte)((ptr[1] + ptr[num5] + ptr[stride]) / 3);
				*ptr2 = *ptr;
				ptr2 -= stride2;
				ptr -= stride;
				ptr++;
				ptr2 += 3;
				for (int k = 1; k < num; k += 2)
				{
					ptr2[2] = *ptr;
					ptr2[1] = (byte)(ptr[num5] + ptr[stride] + ptr[-1] + ptr[1] >> 2);
					*ptr2 = (byte)(ptr[num7] + ptr[num6] + ptr[num4] + ptr[num3] >> 2);
					ptr2 += stride2;
					ptr += stride;
					ptr2[2] = (byte)(ptr[num5] + ptr[stride] >> 1);
					ptr2[1] = (byte)((*ptr + ptr[num7] + ptr[num6] + ptr[num4] + ptr[num3]) / 5);
					*ptr2 = (byte)(ptr[-1] + ptr[1] >> 1);
					ptr2 += 3;
					ptr++;
					ptr2[2] = (byte)(ptr[num7] + ptr[num6] + ptr[num4] + ptr[num3] >> 2);
					ptr2[1] = (byte)(ptr[num5] + ptr[stride] + ptr[-1] + ptr[1] >> 2);
					*ptr2 = *ptr;
					ptr2 -= stride2;
					ptr -= stride;
					ptr2[2] = (byte)(ptr[-1] + ptr[1] >> 1);
					ptr2[1] = (byte)((ptr[num7] + ptr[num6] + ptr[num4] + ptr[num3] + *ptr) / 5);
					*ptr2 = (byte)(ptr[num5] + ptr[stride] >> 1);
					ptr2 += 3;
					ptr++;
				}
				ptr2[2] = *ptr;
				ptr2[1] = (byte)((ptr[num5] + ptr[stride] + ptr[-1]) / 3);
				*ptr2 = (byte)(ptr[num7] + ptr[num4] >> 1);
				ptr += stride;
				ptr2 += stride2;
				ptr2[2] = (byte)(ptr[num5] + ptr[stride] >> 1);
				ptr2[1] = (byte)((ptr[num7] + ptr[num4] + *ptr) / 3);
				*ptr2 = ptr[-1];
				ptr += num8 + 1;
				ptr2 += num9 + 3;
			}
			ptr2[2] = ptr[1];
			ptr2[1] = (byte)(ptr[num6] + *ptr >> 1);
			*ptr2 = ptr[num5];
			ptr++;
			ptr2 += 3;
			for (int l = 1; l < num; l += 2)
			{
				ptr2[2] = *ptr;
				ptr2[1] = (byte)((ptr[-1] + ptr[1] + ptr[num5]) / 3);
				*ptr2 = (byte)(ptr[num7] + ptr[num6] >> 1);
				ptr++;
				ptr2 += 3;
				ptr2[2] = (byte)(ptr[-1] + ptr[1] >> 1);
				ptr2[1] = (byte)((*ptr + ptr[num7] + ptr[num6]) / 3);
				*ptr2 = ptr[num5];
				ptr++;
				ptr2 += 3;
			}
			ptr2[2] = *ptr;
			ptr2[1] = (byte)(ptr[num5] + ptr[-1] >> 1);
			*ptr2 = ptr[num7];
		}
	}
}
