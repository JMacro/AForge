using AForge.Math;
using System;

namespace AForge.Imaging.ComplexFilters
{
	public class FrequencyFilter : IComplexFilter
	{
		private IntRange frequencyRange = new IntRange(0, 1024);

		public IntRange FrequencyRange
		{
			get
			{
				return frequencyRange;
			}
			set
			{
				frequencyRange = value;
			}
		}

		public FrequencyFilter()
		{
		}

		public FrequencyFilter(IntRange frequencyRange)
		{
			this.frequencyRange = frequencyRange;
		}

		public void Apply(ComplexImage complexImage)
		{
			if (!complexImage.FourierTransformed)
			{
				throw new ArgumentException("The source complex image should be Fourier transformed.");
			}
			int width = complexImage.Width;
			int height = complexImage.Height;
			int num = width >> 1;
			int num2 = height >> 1;
			int min = frequencyRange.Min;
			int max = frequencyRange.Max;
			Complex[,] data = complexImage.Data;
			for (int i = 0; i < height; i++)
			{
				int num3 = i - num2;
				for (int j = 0; j < width; j++)
				{
					int num4 = j - num;
					int num5 = (int)System.Math.Sqrt(num4 * num4 + num3 * num3);
					if (num5 > max || num5 < min)
					{
						data[i, j].Re = 0.0;
						data[i, j].Im = 0.0;
					}
				}
			}
		}
	}
}
