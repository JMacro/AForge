using AForge.Math;
using System;

namespace AForge.Imaging.Textures
{
	public class CloudsTexture : ITextureGenerator
	{
		private PerlinNoise noise = new PerlinNoise(8, 0.5, 0.03125, 1.0);

		private Random rand = new Random();

		private int r;

		public CloudsTexture()
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
					array[i, j] = System.Math.Max(0f, System.Math.Min(1f, (float)noise.Function2D(j + r, i + r) * 0.5f + 0.5f));
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
