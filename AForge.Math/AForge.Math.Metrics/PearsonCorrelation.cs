using System;

namespace AForge.Math.Metrics
{
	public sealed class PearsonCorrelation : ISimilarity
	{
		public double GetSimilarityScore(double[] p, double[] q)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			int num6 = p.Length;
			if (num6 != q.Length)
			{
				throw new ArgumentException("Input vectors must be of the same dimension.");
			}
			for (int i = 0; i < num6; i++)
			{
				double num7 = p[i];
				double num8 = q[i];
				num += num7;
				num2 += num8;
				num3 += num7 * num7;
				num4 += num8 * num8;
				num5 += num7 * num8;
			}
			double num9 = num5 - num * num2 / (double)num6;
			double num10 = System.Math.Sqrt((num3 - num * num / (double)num6) * (num4 - num2 * num2 / (double)num6));
			if (num10 != 0.0)
			{
				return num9 / num10;
			}
			return 0.0;
		}
	}
}
