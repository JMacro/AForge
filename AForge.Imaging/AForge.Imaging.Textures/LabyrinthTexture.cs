using AForge.Math;
using System;

namespace AForge.Imaging.Textures
{
	public class LabyrinthTexture : ITextureGenerator
	{
		private PerlinNoise noise = new PerlinNoise(1, 0.65, 0.0625, 1.0);

		private Random rand = new Random();

		private int r;

		public LabyrinthTexture()
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
					array[i, j] = System.Math.Min(1f, (float)System.Math.Abs(noise.Function2D(j + r, i + r)));
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
