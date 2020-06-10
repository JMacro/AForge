namespace AForge.Math.Random
{
	public class GaussianGenerator : IRandomNumberGenerator
	{
		private StandardGenerator rand;

		private float mean;

		private float stdDev;

		public float Mean => mean;

		public float Variance => stdDev * stdDev;

		public float StdDev => stdDev;

		public GaussianGenerator(float mean, float stdDev)
			: this(mean, stdDev, 0)
		{
		}

		public GaussianGenerator(float mean, float stdDev, int seed)
		{
			this.mean = mean;
			this.stdDev = stdDev;
			rand = new StandardGenerator(seed);
		}

		public float Next()
		{
			return rand.Next() * stdDev + mean;
		}

		public void SetSeed(int seed)
		{
			rand = new StandardGenerator(seed);
		}
	}
}
