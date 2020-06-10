using System;

namespace AForge.Math.Metrics
{
	public sealed class ManhattanDistance : IDistance
	{
		public double GetDistance(double[] p, double[] q)
		{
			double num = 0.0;
			if (p.Length != q.Length)
			{
				throw new ArgumentException("Input vectors must be of the same dimension.");
			}
			int i = 0;
			for (int num2 = p.Length; i < num2; i++)
			{
				num += System.Math.Abs(p[i] - q[i]);
			}
			return num;
		}
	}
}
