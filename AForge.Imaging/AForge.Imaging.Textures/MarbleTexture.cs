using AForge.Math;
using System;

namespace AForge.Imaging.Textures
{
	public class MarbleTexture : ITextureGenerator
	{
		private PerlinNoise noise = new PerlinNoise(2, 0.65, 0.03125, 1.0);

		private Random rand = new Random();

		private int r;

		private double xPeriod = 5.0;

		private double yPeriod = 10.0;

		public double XPeriod
		{
			get
			{
				return xPeriod;
			}
			set
			{
				xPeriod = System.Math.Max(2.0, value);
			}
		}

		public double YPeriod
		{
			get
			{
				return yPeriod;
			}
			set
			{
				yPeriod = System.Math.Max(2.0, value);
			}
		}

		public MarbleTexture()
		{
			Reset();
		}

		public MarbleTexture(double xPeriod, double yPeriod)
		{
			this.xPeriod = xPeriod;
			this.yPeriod = yPeriod;
			Reset();
		}

		public float[,] Generate(int width, int height)
		{
			float[,] array = new float[height, width];
			double num = xPeriod / (double)width;
			double num2 = yPeriod / (double)height;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					array[i, j] = System.Math.Min(1f, (float)System.Math.Abs(System.Math.Sin(((double)j * num + (double)i * num2 + noise.Function2D(j + r, i + r)) * System.Math.PI)));
				}
			}
			return array;
		}

		public void Reset()
		{
			r = rand.Next(5000);
		}
	}
}
