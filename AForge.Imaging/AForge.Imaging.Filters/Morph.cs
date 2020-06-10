using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Morph : BaseInPlaceFilter2
	{
		private double sourcePercent = 0.5;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public double SourcePercent
		{
			get
			{
				return sourcePercent;
			}
			set
			{
				sourcePercent = System.Math.Max(0.0, System.Math.Min(1.0, value));
			}
		}

		public Morph()
		{
			InitFormatTranslations();
		}

		public Morph(Bitmap overlayImage)
			: base(overlayImage)
		{
			InitFormatTranslations();
		}

		public Morph(UnmanagedImage unmanagedOverlayImage)
			: base(unmanagedOverlayImage)
		{
			InitFormatTranslations();
		}

		private void InitFormatTranslations()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, UnmanagedImage overlay)
		{
			int width = image.Width;
			int height = image.Height;
			int num = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int num2 = width * num;
			int num3 = image.Stride - num2;
			int num4 = overlay.Stride - num2;
			double num5 = 1.0 - sourcePercent;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			byte* ptr2 = (byte*)overlay.ImageData.ToPointer();
			for (int i = 0; i < height; i++)
			{
				int num6 = 0;
				while (num6 < num2)
				{
					*ptr = (byte)(sourcePercent * (double)(int)(*ptr) + num5 * (double)(int)(*ptr2));
					num6++;
					ptr++;
					ptr2++;
				}
				ptr += num3;
				ptr2 += num4;
			}
		}
	}
}
