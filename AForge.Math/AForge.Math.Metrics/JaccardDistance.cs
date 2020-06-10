using System;

namespace AForge.Math.Metrics
{
	public sealed class JaccardDistance : IDistance
	{
		public double GetDistance(double[] p, double[] q)
		{
			double num = 0.0;
			int num2 = 0;
			int num3 = 0;
			if (p.Length != q.Length)
			{
				throw new ArgumentException("Input vectors must be of the same dimension.");
			}
			int i = 0;
			for (int num4 = p.Length; i < num4; i++)
			{
				if (p[i] != 0.0 || q[i] != 0.0)
				{
					if (p[i] == q[i])
					{
						num2++;
					}
					num3++;
				}
			}
			if (num3 != 0)
			{
				return 1.0 - (double)num2 / (double)num3;
			}
			return 0.0;
		}
	}
}
