namespace AForge.Math.Metrics
{
	public sealed class CosineDistance : IDistance
	{
		public double GetDistance(double[] p, double[] q)
		{
			CosineSimilarity cosineSimilarity = new CosineSimilarity();
			return 1.0 - cosineSimilarity.GetSimilarityScore(p, q);
		}
	}
}
