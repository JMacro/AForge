using AForge.Math;
using System;

namespace AForge.Imaging.Filters
{
	public sealed class GaussianBlur : Convolution
	{
		private double sigma = 1.4;

		private int size = 5;

		public double Sigma
		{
			get
			{
				return sigma;
			}
			set
			{
				sigma = System.Math.Max(0.5, System.Math.Min(5.0, value));
				CreateFilter();
			}
		}

		public int Size
		{
			get
			{
				return size;
			}
			set
			{
				size = System.Math.Max(3, System.Math.Min(21, value | 1));
				CreateFilter();
			}
		}

		public GaussianBlur()
		{
			CreateFilter();
			base.ProcessAlpha = true;
		}

		public GaussianBlur(double sigma)
		{
			Sigma = sigma;
			base.ProcessAlpha = true;
		}

		public GaussianBlur(double sigma, int size)
		{
			Sigma = sigma;
			Size = size;
			base.ProcessAlpha = true;
		}

		private void CreateFilter()
		{
			Gaussian gaussian = new Gaussian(sigma);
			double[,] array = gaussian.Kernel2D(size);
			double num = array[0, 0];
			int[,] array2 = new int[size, size];
			int num2 = 0;
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					double num3 = array[i, j] / num;
					if (num3 > 65535.0)
					{
						num3 = 65535.0;
					}
					array2[i, j] = (int)num3;
					num2 += array2[i, j];
				}
			}
			base.Kernel = array2;
			base.Divisor = num2;
		}
	}
}
