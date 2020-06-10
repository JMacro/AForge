using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class SaltAndPepperNoise : BaseInPlacePartialFilter
	{
		private double noiseAmount = 10.0;

		private Random rand = new Random();

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public double NoiseAmount
		{
			get
			{
				return noiseAmount;
			}
			set
			{
				noiseAmount = System.Math.Max(0.0, System.Math.Min(100.0, value));
			}
		}

		public SaltAndPepperNoise()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		public SaltAndPepperNoise(double noiseAmount)
			: this()
		{
			this.noiseAmount = noiseAmount;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int left = rect.Left;
			int top = rect.Top;
			int width = rect.Width;
			int height = rect.Height;
			int stride = image.Stride;
			int num = (int)((double)(width * height) * noiseAmount / 100.0);
			byte[] array = new byte[2]
			{
				0,
				255
			};
			byte* ptr = (byte*)image.ImageData.ToPointer();
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				for (int i = 0; i < num; i++)
				{
					int num2 = left + rand.Next(width);
					int num3 = top + rand.Next(height);
					ptr[num3 * stride + num2] = array[rand.Next(2)];
				}
				return;
			}
			int num4 = (image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
			for (int j = 0; j < num; j++)
			{
				int num5 = left + rand.Next(width);
				int num6 = top + rand.Next(height);
				int num7 = rand.Next(3);
				ptr[num6 * stride + num5 * num4 + num7] = array[rand.Next(2)];
			}
		}
	}
}
