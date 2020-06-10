using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public sealed class Add : BaseInPlaceFilter2
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public Add()
		{
			InitFormatTranslations();
		}

		public Add(Bitmap overlayImage)
			: base(overlayImage)
		{
			InitFormatTranslations();
		}

		public Add(UnmanagedImage unmanagedOverlayImage)
			: base(unmanagedOverlayImage)
		{
			InitFormatTranslations();
		}

		private void InitFormatTranslations()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
			formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, UnmanagedImage overlay)
		{
			PixelFormat pixelFormat = image.PixelFormat;
			int width = image.Width;
			int height = image.Height;
			int num;
			switch (pixelFormat)
			{
			case PixelFormat.Format24bppRgb:
			case PixelFormat.Format32bppRgb:
			case PixelFormat.Format8bppIndexed:
			case PixelFormat.Format32bppArgb:
			{
				int num2;
				switch (pixelFormat)
				{
				default:
					num2 = 4;
					break;
				case PixelFormat.Format24bppRgb:
					num2 = 3;
					break;
				case PixelFormat.Format8bppIndexed:
					num2 = 1;
					break;
				}
				int num3 = num2;
				int num4 = width * num3;
				int num5 = image.Stride - num4;
				int num6 = overlay.Stride - num4;
				byte* ptr = (byte*)image.ImageData.ToPointer();
				byte* ptr2 = (byte*)overlay.ImageData.ToPointer();
				for (int i = 0; i < height; i++)
				{
					int num7 = 0;
					while (num7 < num4)
					{
						int num8 = *ptr + *ptr2;
						*ptr = ((num8 > 255) ? byte.MaxValue : ((byte)num8));
						num7++;
						ptr++;
						ptr2++;
					}
					ptr += num5;
					ptr2 += num6;
				}
				return;
			}
			default:
				num = 4;
				break;
			case PixelFormat.Format48bppRgb:
				num = 3;
				break;
			case PixelFormat.Format16bppGrayScale:
				num = 1;
				break;
			}
			int num9 = num;
			int num10 = width * num9;
			int stride = image.Stride;
			int stride2 = overlay.Stride;
			byte* ptr3 = (byte*)image.ImageData.ToPointer();
			byte* ptr4 = (byte*)overlay.ImageData.ToPointer();
			for (int j = 0; j < height; j++)
			{
				ushort* ptr5 = (ushort*)(ptr3 + (long)j * (long)stride);
				ushort* ptr6 = (ushort*)(ptr4 + (long)j * (long)stride2);
				int num11 = 0;
				while (num11 < num10)
				{
					int num12 = *ptr5 + *ptr6;
					*ptr5 = ((num12 > 65535) ? ushort.MaxValue : ((ushort)num12));
					num11++;
					ptr5++;
					ptr6++;
				}
			}
		}
	}
}
