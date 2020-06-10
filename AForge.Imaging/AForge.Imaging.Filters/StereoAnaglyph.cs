using System.Collections.Generic;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class StereoAnaglyph : BaseInPlaceFilter2
	{
		public enum Algorithm
		{
			TrueAnaglyph,
			GrayAnaglyph,
			ColorAnaglyph,
			HalfColorAnaglyph,
			OptimizedAnaglyph
		}

		private Algorithm anaglyphAlgorithm = Algorithm.GrayAnaglyph;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public Algorithm AnaglyphAlgorithm
		{
			get
			{
				return anaglyphAlgorithm;
			}
			set
			{
				anaglyphAlgorithm = value;
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public StereoAnaglyph()
		{
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		public StereoAnaglyph(Algorithm anaglyphAlgorithm)
			: this()
		{
			this.anaglyphAlgorithm = anaglyphAlgorithm;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, UnmanagedImage overlay)
		{
			int width = image.Width;
			int height = image.Height;
			int num = image.Stride - width * 3;
			int num2 = overlay.Stride - width * 3;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			byte* ptr2 = (byte*)overlay.ImageData.ToPointer();
			switch (anaglyphAlgorithm)
			{
			case Algorithm.TrueAnaglyph:
			{
				for (int j = 0; j < height; j++)
				{
					int num4 = 0;
					while (num4 < width)
					{
						ptr[2] = (byte)((double)(int)ptr[2] * 0.299 + (double)(int)ptr[1] * 0.587 + (double)(int)(*ptr) * 0.114);
						ptr[1] = 0;
						*ptr = (byte)((double)(int)ptr2[2] * 0.299 + (double)(int)ptr2[1] * 0.587 + (double)(int)(*ptr2) * 0.114);
						num4++;
						ptr += 3;
						ptr2 += 3;
					}
					ptr += num;
					ptr2 += num2;
				}
				break;
			}
			case Algorithm.GrayAnaglyph:
			{
				for (int l = 0; l < height; l++)
				{
					int num6 = 0;
					while (num6 < width)
					{
						ptr[2] = (byte)((double)(int)ptr[2] * 0.299 + (double)(int)ptr[1] * 0.587 + (double)(int)(*ptr) * 0.114);
						ptr[1] = (byte)((double)(int)ptr2[2] * 0.299 + (double)(int)ptr2[1] * 0.587 + (double)(int)(*ptr2) * 0.114);
						*ptr = ptr[1];
						num6++;
						ptr += 3;
						ptr2 += 3;
					}
					ptr += num;
					ptr2 += num2;
				}
				break;
			}
			case Algorithm.ColorAnaglyph:
			{
				for (int m = 0; m < height; m++)
				{
					int num7 = 0;
					while (num7 < width)
					{
						ptr[1] = ptr2[1];
						*ptr = *ptr2;
						num7++;
						ptr += 3;
						ptr2 += 3;
					}
					ptr += num;
					ptr2 += num2;
				}
				break;
			}
			case Algorithm.HalfColorAnaglyph:
			{
				for (int k = 0; k < height; k++)
				{
					int num5 = 0;
					while (num5 < width)
					{
						ptr[2] = (byte)((double)(int)ptr[2] * 0.299 + (double)(int)ptr[1] * 0.587 + (double)(int)(*ptr) * 0.114);
						ptr[1] = ptr2[1];
						*ptr = *ptr2;
						num5++;
						ptr += 3;
						ptr2 += 3;
					}
					ptr += num;
					ptr2 += num2;
				}
				break;
			}
			case Algorithm.OptimizedAnaglyph:
			{
				for (int i = 0; i < height; i++)
				{
					int num3 = 0;
					while (num3 < width)
					{
						ptr[2] = (byte)((double)(int)ptr[1] * 0.7 + (double)(int)(*ptr) * 0.3);
						ptr[1] = ptr2[1];
						*ptr = *ptr2;
						num3++;
						ptr += 3;
						ptr2 += 3;
					}
					ptr += num;
					ptr2 += num2;
				}
				break;
			}
			}
		}
	}
}
