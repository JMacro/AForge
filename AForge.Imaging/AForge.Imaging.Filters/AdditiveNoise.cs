using AForge.Math.Random;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class AdditiveNoise : BaseInPlacePartialFilter
	{
		private IRandomNumberGenerator generator = new UniformGenerator(new Range(-10f, 10f));

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public IRandomNumberGenerator Generator
		{
			get
			{
				return generator;
			}
			set
			{
				generator = value;
			}
		}

		public AdditiveNoise()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		public AdditiveNoise(IRandomNumberGenerator generator)
			: this()
		{
			this.generator = generator;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int top = rect.Top;
			int num2 = top + rect.Height;
			int num3 = rect.Left * num;
			int num4 = num3 + rect.Width * num;
			int num5 = image.Stride - (num4 - num3);
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + rect.Left * num;
			for (int i = top; i < num2; i++)
			{
				int num6 = num3;
				while (num6 < num4)
				{
					*ptr = (byte)System.Math.Max(0f, System.Math.Min(255f, (float)(int)(*ptr) + generator.Next()));
					num6++;
					ptr++;
				}
				ptr += num5;
			}
		}
	}
}
