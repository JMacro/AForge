using System.Collections.Generic;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Grayscale : BaseFilter
	{
		public static class CommonAlgorithms
		{
			public static readonly Grayscale BT709 = new Grayscale(0.2125, 0.7154, 0.0721);

			public static readonly Grayscale RMY = new Grayscale(0.5, 0.419, 0.081);

			public static readonly Grayscale Y = new Grayscale(0.299, 0.587, 0.114);
		}

		public readonly double RedCoefficient;

		public readonly double GreenCoefficient;

		public readonly double BlueCoefficient;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public Grayscale(double cr, double cg, double cb)
		{
			RedCoefficient = cr;
			GreenCoefficient = cg;
			BlueCoefficient = cb;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format16bppGrayScale;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			PixelFormat pixelFormat = sourceData.PixelFormat;
			int num;
			switch (pixelFormat)
			{
			case PixelFormat.Format24bppRgb:
			case PixelFormat.Format32bppRgb:
			case PixelFormat.Format32bppArgb:
			{
				int num2 = (pixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
				int num3 = sourceData.Stride - width * num2;
				int num4 = destinationData.Stride - width;
				int num5 = (int)(65536.0 * RedCoefficient);
				int num6 = (int)(65536.0 * GreenCoefficient);
				int num7 = (int)(65536.0 * BlueCoefficient);
				byte* ptr = (byte*)sourceData.ImageData.ToPointer();
				byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
				for (int i = 0; i < height; i++)
				{
					int num8 = 0;
					while (num8 < width)
					{
						*ptr2 = (byte)(num5 * ptr[2] + num6 * ptr[1] + num7 * *ptr >> 16);
						num8++;
						ptr += num2;
						ptr2++;
					}
					ptr += num3;
					ptr2 += num4;
				}
				return;
			}
			default:
				num = 4;
				break;
			case PixelFormat.Format48bppRgb:
				num = 3;
				break;
			}
			int num9 = num;
			byte* ptr3 = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr4 = (byte*)destinationData.ImageData.ToPointer();
			int stride = sourceData.Stride;
			int stride2 = destinationData.Stride;
			for (int j = 0; j < height; j++)
			{
				ushort* ptr5 = (ushort*)(ptr3 + (long)j * (long)stride);
				ushort* ptr6 = (ushort*)(ptr4 + (long)j * (long)stride2);
				int num10 = 0;
				while (num10 < width)
				{
					*ptr6 = (ushort)(RedCoefficient * (double)(int)ptr5[2] + GreenCoefficient * (double)(int)ptr5[1] + BlueCoefficient * (double)(int)(*ptr5));
					num10++;
					ptr5 += num9;
					ptr6++;
				}
			}
		}
	}
}
