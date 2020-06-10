namespace AForge.Math.Random
{
	public class UniformOneGenerator : IRandomNumberGenerator
	{
		private ThreadSafeRandom rand;

		public float Mean => 0.5f;

		public float Variance => 0.0833333358f;

		public UniformOneGenerator()
		{
			rand = new ThreadSafeRandom(0);
		}

		public UniformOneGenerator(int seed)
		{
			rand = new ThreadSafeRandom(seed);
		}

		public float Next()
		{
			return (float)rand.NextDouble();
		}

		public void SetSeed(int seed)
		{
			rand = new ThreadSafeRandom(seed);
		}
	}
}
