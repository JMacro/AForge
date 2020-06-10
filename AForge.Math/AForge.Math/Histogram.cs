using System;

namespace AForge.Math
{
	[Serializable]
	public class Histogram
	{
		private int[] values;

		private double mean;

		private double stdDev;

		private int median;

		private int min;

		private int max;

		private long total;

		public int[] Values => values;

		public double Mean => mean;

		public double StdDev => stdDev;

		public int Median => median;

		public int Min => min;

		public int Max => max;

		public long TotalCount => total;

		public Histogram(int[] values)
		{
			this.values = values;
			Update();
		}

		public IntRange GetRange(double percent)
		{
			return Statistics.GetRange(values, percent);
		}

		public void Update()
		{
			int num = values.Length;
			max = 0;
			min = num;
			total = 0L;
			for (int i = 0; i < num; i++)
			{
				if (values[i] != 0)
				{
					if (i > max)
					{
						max = i;
					}
					if (i < min)
					{
						min = i;
					}
					total += values[i];
				}
			}
			mean = Statistics.Mean(values);
			stdDev = Statistics.StdDev(values, mean);
			median = Statistics.Median(values);
		}
	}
}
