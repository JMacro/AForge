using System;

namespace AForge.Math.Metrics
{
	public sealed class EuclideanDistance : IDistance
	{
		public double GetDistance(double[] p, double[] q)
		{
			double num = 0.0;
			double num2 = 0.0;
			if (p.Length != q.Length)
			{
				throw new ArgumentException("Input vectors must be of the same dimension.");
			}
			int i = 0;
			for (int num3 = p.Length; i < num3; i++)
			{
				num2 = p[i] - q[i];
				num += num2 * num2;
			}
			return System.Math.Sqrt(num);
		}
	}
}
