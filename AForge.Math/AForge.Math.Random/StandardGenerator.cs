using System;

namespace AForge.Math.Random
{
	public class StandardGenerator : IRandomNumberGenerator
	{
		private UniformOneGenerator rand;

		private float secondValue;

		private bool useSecond;

		public float Mean => 0f;

		public float Variance => 1f;

		public StandardGenerator()
		{
			rand = new UniformOneGenerator();
		}

		public StandardGenerator(int seed)
		{
			rand = new UniformOneGenerator(seed);
		}

		public float Next()
		{
			if (useSecond)
			{
				useSecond = false;
				return secondValue;
			}
			float num;
			float num2;
			float num3;
			do
			{
				num = rand.Next() * 2f - 1f;
				num2 = rand.Next() * 2f - 1f;
				num3 = num * num + num2 * num2;
			}
			while (num3 >= 1f);
			num3 = (float)System.Math.Sqrt(-2.0 * System.Math.Log(num3) / (double)num3);
			float result = num * num3;
			secondValue = num2 * num3;
			useSecond = true;
			return result;
		}

		public void SetSeed(int seed)
		{
			rand = new UniformOneGenerator(seed);
			useSecond = false;
		}
	}
}
