using System;

namespace AForge.Math
{
	public static class Statistics
	{
		public static double Mean(int[] values)
		{
			long num = 0L;
			double num2 = 0.0;
			int i = 0;
			for (int num3 = values.Length; i < num3; i++)
			{
				int num4 = values[i];
				num2 += (double)(i * num4);
				num += num4;
			}
			if (num != 0)
			{
				return num2 / (double)num;
			}
			return 0.0;
		}

		public static double StdDev(int[] values)
		{
			return StdDev(values, Mean(values));
		}

		public static double StdDev(int[] values, double mean)
		{
			double num = 0.0;
			int num2 = 0;
			int i = 0;
			for (int num3 = values.Length; i < num3; i++)
			{
				int num4 = values[i];
				double num5 = (double)i - mean;
				num += num5 * num5 * (double)num4;
				num2 += num4;
			}
			if (num2 != 0)
			{
				return System.Math.Sqrt(num / (double)num2);
			}
			return 0.0;
		}

		public static int Median(int[] values)
		{
			int num = 0;
			int num2 = values.Length;
			for (int i = 0; i < num2; i++)
			{
				num += values[i];
			}
			int num3 = num / 2;
			int j = 0;
			int num4 = 0;
			for (; j < num2; j++)
			{
				num4 += values[j];
				if (num4 >= num3)
				{
					break;
				}
			}
			return j;
		}

		public static IntRange GetRange(int[] values, double percent)
		{
			int num = 0;
			int num2 = values.Length;
			for (int i = 0; i < num2; i++)
			{
				num += values[i];
			}
			int num3 = (int)((double)num * (percent + (1.0 - percent) / 2.0));
			int j = 0;
			int num4 = num;
			for (; j < num2; j++)
			{
				num4 -= values[j];
				if (num4 < num3)
				{
					break;
				}
			}
			int num5 = num2 - 1;
			num4 = num;
			while (num5 >= 0)
			{
				num4 -= values[num5];
				if (num4 < num3)
				{
					break;
				}
				num5--;
			}
			return new IntRange(j, num5);
		}

		public static double Entropy(int[] values)
		{
			int num = values.Length;
			int num2 = 0;
			double num3 = 0.0;
			for (int i = 0; i < num; i++)
			{
				num2 += values[i];
			}
			if (num2 != 0)
			{
				for (int j = 0; j < num; j++)
				{
					double num4 = (double)values[j] / (double)num2;
					if (num4 != 0.0)
					{
						num3 += (0.0 - num4) * System.Math.Log(num4, 2.0);
					}
				}
			}
			return num3;
		}

		public static int Mode(int[] values)
		{
			int result = 0;
			int num = 0;
			int i = 0;
			for (int num2 = values.Length; i < num2; i++)
			{
				if (values[i] > num)
				{
					num = values[i];
					result = i;
				}
			}
			return result;
		}
	}
}
