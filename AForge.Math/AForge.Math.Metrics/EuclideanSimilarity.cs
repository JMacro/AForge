namespace AForge.Math.Metrics
{
	public sealed class EuclideanSimilarity : ISimilarity
	{
		public double GetSimilarityScore(double[] p, double[] q)
		{
			double num = 0.0;
			EuclideanDistance euclideanDistance = new EuclideanDistance();
			return 1.0 / (1.0 + euclideanDistance.GetDistance(p, q));
		}
	}
}
