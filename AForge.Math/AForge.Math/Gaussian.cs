using System;

namespace AForge.Math
{
	public class Gaussian
	{
		private double sigma = 1.0;

		private double sqrSigma = 1.0;

		public double Sigma
		{
			get
			{
				return sigma;
			}
			set
			{
				sigma = System.Math.Max(1E-08, value);
				sqrSigma = sigma * sigma;
			}
		}

		public Gaussian()
		{
		}

		public Gaussian(double sigma)
		{
			Sigma = sigma;
		}

		public double Function(double x)
		{
			return System.Math.Exp(x * x / (-2.0 * sqrSigma)) / (System.Math.Sqrt(System.Math.PI * 2.0) * sigma);
		}

		public double Function2D(double x, double y)
		{
			return System.Math.Exp((x * x + y * y) / (-2.0 * sqrSigma)) / (System.Math.PI * 2.0 * sqrSigma);
		}

		public double[] Kernel(int size)
		{
			if (size % 2 == 0 || size < 3 || size > 101)
			{
				throw new ArgumentException("Wrong kernal size.");
			}
			int num = size / 2;
			double[] array = new double[size];
			int num2 = -num;
			for (int i = 0; i < size; i++)
			{
				array[i] = Function(num2);
				num2++;
			}
			return array;
		}

		public double[,] Kernel2D(int size)
		{
			if (size % 2 == 0 || size < 3 || size > 101)
			{
				throw new ArgumentException("Wrong kernal size.");
			}
			int num = size / 2;
			double[,] array = new double[size, size];
			int num2 = -num;
			for (int i = 0; i < size; i++)
			{
				int num3 = -num;
				for (int j = 0; j < size; j++)
				{
					array[i, j] = Function2D(num3, num2);
					num3++;
				}
				num2++;
			}
			return array;
		}
	}
}
