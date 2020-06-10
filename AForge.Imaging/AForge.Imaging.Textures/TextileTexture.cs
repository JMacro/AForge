using AForge.Math;
using System;

namespace AForge.Imaging.Textures
{
	public class TextileTexture : ITextureGenerator
	{
		private PerlinNoise noise = new PerlinNoise(3, 0.65, 0.125, 1.0);

		private Random rand = new Random();

		private int r;

		public TextileTexture()
		{
			Reset();
		}

		public float[,] Generate(int width, int height)
		{
			float[,] array = new float[height, width];
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					array[i, j] = System.Math.Max(0f, System.Math.Min(1f, ((float)System.Math.Sin((double)j + noise.Function2D(j + r, i + r)) + (float)System.Math.Sin((double)i + noise.Function2D(j + r, i + r))) * 0.25f + 0.5f));
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
