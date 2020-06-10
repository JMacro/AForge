using AForge.Math;
using System;

namespace AForge.Imaging.Filters
{
	public class GaussianSharpen : Convolution
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

		public GaussianSharpen()
		{
			CreateFilter();
		}

		public GaussianSharpen(double sigma)
		{
			Sigma = sigma;
		}

		public GaussianSharpen(double sigma, int size)
		{
			Sigma = sigma;
			Size = size;
		}

		private void CreateFilter()
		{
			Gaussian gaussian = new Gaussian(sigma);
			double[,] array = gaussian.Kernel2D(size);
			double num = array[0, 0];
			int[,] array2 = new int[size, size];
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					double num4 = array[i, j] / num;
					if (num4 > 65535.0)
					{
						num4 = 65535.0;
					}
					array2[i, j] = (int)num4;
					num2 += array2[i, j];
				}
			}
			int num5 = size >> 1;
			for (int k = 0; k < size; k++)
			{
				for (int l = 0; l < size; l++)
				{
					if (k == num5 && l == num5)
					{
						array2[k, l] = 2 * num2 - array2[k, l];
					}
					else
					{
						array2[k, l] = -array2[k, l];
					}
					num3 += array2[k, l];
				}
			}
			base.Kernel = array2;
			base.Divisor = num3;
		}
	}
}
