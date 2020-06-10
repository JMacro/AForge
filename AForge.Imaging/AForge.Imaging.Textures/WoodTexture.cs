using AForge.Math;
using System;

namespace AForge.Imaging.Textures
{
	public class WoodTexture : ITextureGenerator
	{
		private PerlinNoise noise = new PerlinNoise(8, 0.5, 0.03125, 0.05);

		private Random rand = new Random();

		private int r;

		private double rings = 12.0;

		public double Rings
		{
			get
			{
				return rings;
			}
			set
			{
				rings = System.Math.Max(3.0, value);
			}
		}

		public WoodTexture()
		{
			Reset();
		}

		public WoodTexture(double rings)
		{
			this.rings = rings;
			Reset();
		}

		public float[,] Generate(int width, int height)
		{
			float[,] array = new float[height, width];
			int num = width / 2;
			int num2 = height / 2;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					double num3 = (double)(j - num) / (double)width;
					double num4 = (double)(i - num2) / (double)height;
					array[i, j] = System.Math.Min(1f, (float)System.Math.Abs(System.Math.Sin((System.Math.Sqrt(num3 * num3 + num4 * num4) + noise.Function2D(j + r, i + r)) * System.Math.PI * 2.0 * rings)));
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
