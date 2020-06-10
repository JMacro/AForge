namespace AForge.Math.Random
{
	public class UniformGenerator : IRandomNumberGenerator
	{
		private UniformOneGenerator rand;

		private float min;

		private float length;

		public float Mean => (min + min + length) / 2f;

		public float Variance => length * length / 12f;

		public Range Range => new Range(min, min + length);

		public UniformGenerator(Range range)
			: this(range, 0)
		{
		}

		public UniformGenerator(Range range, int seed)
		{
			rand = new UniformOneGenerator(seed);
			min = range.Min;
			length = range.Length;
		}

		public float Next()
		{
			return rand.Next() * length + min;
		}

		public void SetSeed(int seed)
		{
			rand = new UniformOneGenerator(seed);
		}
	}
}
