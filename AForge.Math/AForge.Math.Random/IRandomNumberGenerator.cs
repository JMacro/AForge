namespace AForge.Math.Random
{
	public interface IRandomNumberGenerator
	{
		float Mean
		{
			get;
		}

		float Variance
		{
			get;
		}

		float Next();

		void SetSeed(int seed);
	}
}
