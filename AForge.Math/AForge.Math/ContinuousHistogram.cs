using System;

namespace AForge.Math
{
	[Serializable]
	public class ContinuousHistogram
	{
		private int[] values;

		private Range range;

		private float mean;

		private float stdDev;

		private float median;

		private float min;

		private float max;

		private int total;

		public int[] Values => values;

		public Range Range => range;

		public float Mean => mean;

		public float StdDev => stdDev;

		public float Median => median;

		public float Min => min;

		public float Max => max;

		public ContinuousHistogram(int[] values, Range range)
		{
			this.values = values;
			this.range = range;
			Update();
		}

		public Range GetRange(float percent)
		{
			int num = (int)((float)total * (percent + (1f - percent) / 2f));
			int num2 = values.Length;
			int num3 = num2 - 1;
			int i = 0;
			int num4 = total;
			for (; i < num2; i++)
			{
				num4 -= values[i];
				if (num4 < num)
				{
					break;
				}
			}
			int num5 = num3;
			num4 = total;
			while (num5 >= 0)
			{
				num4 -= values[num5];
				if (num4 < num)
				{
					break;
				}
				num5--;
			}
			return new Range((float)i / (float)num3 * range.Length + range.Min, (float)num5 / (float)num3 * range.Length + range.Min);
		}

		public void Update()
		{
			int num = values.Length;
			int num2 = num - 1;
			float length = range.Length;
			float num3 = range.Min;
			max = 0f;
			min = num;
			mean = 0f;
			stdDev = 0f;
			total = 0;
			double num4 = 0.0;
			int num5;
			for (int i = 0; i < num; i++)
			{
				num5 = values[i];
				if (num5 != 0)
				{
					if ((float)i > max)
					{
						max = i;
					}
					if ((float)i < min)
					{
						min = i;
					}
				}
				total += num5;
				num4 += ((double)i / (double)num2 * (double)length + (double)num3) * (double)num5;
			}
			if (total != 0)
			{
				mean = (float)(num4 / (double)total);
			}
			min = min / (float)num2 * length + num3;
			max = max / (float)num2 * length + num3;
			num4 = 0.0;
			for (int i = 0; i < num; i++)
			{
				num5 = values[i];
				double num6 = (double)i / (double)num2 * (double)length + (double)num3 - (double)mean;
				num4 += num6 * num6 * (double)num5;
			}
			if (total != 0)
			{
				stdDev = (float)System.Math.Sqrt(num4 / (double)total);
			}
			int num7 = total / 2;
			int j = 0;
			num5 = 0;
			for (; j < num; j++)
			{
				num5 += values[j];
				if (num5 >= num7)
				{
					break;
				}
			}
			median = (float)j / (float)num2 * length + num3;
		}
	}
}
