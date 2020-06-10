using System;

namespace AForge.Math.Metrics
{
	public sealed class CosineSimilarity : ISimilarity
	{
		public double GetSimilarityScore(double[] p, double[] q)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			if (p.Length != q.Length)
			{
				throw new ArgumentException("Input vectors must be of the same dimension.");
			}
			int i = 0;
			for (int num5 = p.Length; i < num5; i++)
			{
				double num6 = p[i];
				double num7 = q[i];
				num2 += num6 * num7;
				num3 += num6 * num6;
				num4 += num7 * num7;
			}
			num = System.Math.Sqrt(num3) * System.Math.Sqrt(num4);
			if (num != 0.0)
			{
				return num2 / num;
			}
			return 0.0;
		}
	}
}
