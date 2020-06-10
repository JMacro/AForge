using System;

namespace AForge.Math.Random
{
	public class ExponentialGenerator : IRandomNumberGenerator
	{
		private UniformOneGenerator rand;

		private float rate;

		public float Rate => rate;

		public float Mean => 1f / rate;

		public float Variance => 1f / (rate * rate);

		public ExponentialGenerator(float rate)
			: this(rate, 0)
		{
		}

		public ExponentialGenerator(float rate, int seed)
		{
			if (rate <= 0f)
			{
				throw new ArgumentException("Rate value should be greater than zero.");
			}
			rand = new UniformOneGenerator(seed);
			this.rate = rate;
		}

		public float Next()
		{
			return (0f - (float)System.Math.Log(rand.Next())) / rate;
		}

		public void SetSeed(int seed)
		{
			rand = new UniformOneGenerator(seed);
		}
	}
}
