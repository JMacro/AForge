using System;

namespace AForge.Math
{
	internal class svd
	{
		public static void svdcmp(double[,] a, out double[] w, out double[,] v)
		{
			int length = a.GetLength(0);
			int length2 = a.GetLength(1);
			if (length < length2)
			{
				throw new ArgumentException("Number of rows in A must be greater or equal to number of columns");
			}
			w = new double[length2];
			v = new double[length2, length2];
			int num = 0;
			int num2 = 0;
			double[] array = new double[length2];
			double num3;
			double num4;
			double num5 = num4 = (num3 = 0.0);
			for (int i = 0; i < length2; i++)
			{
				num = i + 1;
				array[i] = num4 * num5;
				double num6;
				num5 = (num6 = (num4 = 0.0));
				if (i < length)
				{
					for (int j = i; j < length; j++)
					{
						num4 += System.Math.Abs(a[j, i]);
					}
					if (num4 != 0.0)
					{
						for (int j = i; j < length; j++)
						{
							a[j, i] /= num4;
							num6 += a[j, i] * a[j, i];
						}
						double num7 = a[i, i];
						num5 = 0.0 - Sign(System.Math.Sqrt(num6), num7);
						double num8 = num7 * num5 - num6;
						a[i, i] = num7 - num5;
						if (i != length2 - 1)
						{
							for (int k = num; k < length2; k++)
							{
								num6 = 0.0;
								for (int j = i; j < length; j++)
								{
									num6 += a[j, i] * a[j, k];
								}
								num7 = num6 / num8;
								for (int j = i; j < length; j++)
								{
									a[j, k] += num7 * a[j, i];
								}
							}
						}
						for (int j = i; j < length; j++)
						{
							a[j, i] *= num4;
						}
					}
				}
				w[i] = num4 * num5;
				num5 = (num6 = (num4 = 0.0));
				if (i < length && i != length2 - 1)
				{
					for (int j = num; j < length2; j++)
					{
						num4 += System.Math.Abs(a[i, j]);
					}
					if (num4 != 0.0)
					{
						for (int j = num; j < length2; j++)
						{
							a[i, j] /= num4;
							num6 += a[i, j] * a[i, j];
						}
						double num7 = a[i, num];
						num5 = 0.0 - Sign(System.Math.Sqrt(num6), num7);
						double num8 = num7 * num5 - num6;
						a[i, num] = num7 - num5;
						for (int j = num; j < length2; j++)
						{
							array[j] = a[i, j] / num8;
						}
						if (i != length - 1)
						{
							for (int k = num; k < length; k++)
							{
								num6 = 0.0;
								for (int j = num; j < length2; j++)
								{
									num6 += a[k, j] * a[i, j];
								}
								for (int j = num; j < length2; j++)
								{
									a[k, j] += num6 * array[j];
								}
							}
						}
						for (int j = num; j < length2; j++)
						{
							a[i, j] *= num4;
						}
					}
				}
				num3 = System.Math.Max(num3, System.Math.Abs(w[i]) + System.Math.Abs(array[i]));
			}
			for (int i = length2 - 1; i >= 0; i--)
			{
				if (i < length2 - 1)
				{
					if (num5 != 0.0)
					{
						for (int k = num; k < length2; k++)
						{
							v[k, i] = a[i, k] / a[i, num] / num5;
						}
						for (int k = num; k < length2; k++)
						{
							double num6 = 0.0;
							for (int j = num; j < length2; j++)
							{
								num6 += a[i, j] * v[j, k];
							}
							for (int j = num; j < length2; j++)
							{
								v[j, k] += num6 * v[j, i];
							}
						}
					}
					for (int k = num; k < length2; k++)
					{
						v[i, k] = (v[k, i] = 0.0);
					}
				}
				v[i, i] = 1.0;
				num5 = array[i];
				num = i;
			}
			for (int i = length2 - 1; i >= 0; i--)
			{
				num = i + 1;
				num5 = w[i];
				if (i < length2 - 1)
				{
					for (int k = num; k < length2; k++)
					{
						a[i, k] = 0.0;
					}
				}
				if (num5 != 0.0)
				{
					num5 = 1.0 / num5;
					if (i != length2 - 1)
					{
						for (int k = num; k < length2; k++)
						{
							double num6 = 0.0;
							for (int j = num; j < length; j++)
							{
								num6 += a[j, i] * a[j, k];
							}
							double num7 = num6 / a[i, i] * num5;
							for (int j = i; j < length; j++)
							{
								a[j, k] += num7 * a[j, i];
							}
						}
					}
					for (int k = i; k < length; k++)
					{
						a[k, i] *= num5;
					}
				}
				else
				{
					for (int k = i; k < length; k++)
					{
						a[k, i] = 0.0;
					}
				}
				a[i, i] += 1.0;
			}
			for (int j = length2 - 1; j >= 0; j--)
			{
				for (int l = 1; l <= 30; l++)
				{
					int num9 = 1;
					for (num = j; num >= 0; num--)
					{
						num2 = num - 1;
						if (System.Math.Abs(array[num]) + num3 == num3)
						{
							num9 = 0;
							break;
						}
						if (System.Math.Abs(w[num2]) + num3 == num3)
						{
							break;
						}
					}
					double num10;
					double num6;
					double num7;
					double num8;
					double num11;
					double num12;
					if (num9 != 0)
					{
						num10 = 0.0;
						num6 = 1.0;
						for (int i = num; i <= j; i++)
						{
							num7 = num6 * array[i];
							if (System.Math.Abs(num7) + num3 != num3)
							{
								num5 = w[i];
								num8 = Pythag(num7, num5);
								w[i] = num8;
								num8 = 1.0 / num8;
								num10 = num5 * num8;
								num6 = (0.0 - num7) * num8;
								for (int k = 1; k <= length; k++)
								{
									num11 = a[k, num2];
									num12 = a[k, i];
									a[k, num2] = num11 * num10 + num12 * num6;
									a[k, i] = num12 * num10 - num11 * num6;
								}
							}
						}
					}
					num12 = w[j];
					if (num == j)
					{
						if (num12 < 0.0)
						{
							w[j] = 0.0 - num12;
							for (int k = 0; k < length2; k++)
							{
								v[k, j] = 0.0 - v[k, j];
							}
						}
						break;
					}
					if (l == 30)
					{
						throw new ApplicationException("No convergence in 30 svdcmp iterations");
					}
					double num13 = w[num];
					num2 = j - 1;
					num11 = w[num2];
					num5 = array[num2];
					num8 = array[j];
					num7 = ((num11 - num12) * (num11 + num12) + (num5 - num8) * (num5 + num8)) / (2.0 * num8 * num11);
					num5 = Pythag(num7, 1.0);
					num7 = ((num13 - num12) * (num13 + num12) + num8 * (num11 / (num7 + Sign(num5, num7)) - num8)) / num13;
					num10 = (num6 = 1.0);
					for (int k = num; k <= num2; k++)
					{
						int i = k + 1;
						num5 = array[i];
						num11 = w[i];
						num8 = num6 * num5;
						num5 = num10 * num5;
						num12 = (array[k] = Pythag(num7, num8));
						num10 = num7 / num12;
						num6 = num8 / num12;
						num7 = num13 * num10 + num5 * num6;
						num5 = num5 * num10 - num13 * num6;
						num8 = num11 * num6;
						num11 *= num10;
						for (int m = 0; m < length2; m++)
						{
							num13 = v[m, k];
							num12 = v[m, i];
							v[m, k] = num13 * num10 + num12 * num6;
							v[m, i] = num12 * num10 - num13 * num6;
						}
						num12 = Pythag(num7, num8);
						w[k] = num12;
						if (num12 != 0.0)
						{
							num12 = 1.0 / num12;
							num10 = num7 * num12;
							num6 = num8 * num12;
						}
						num7 = num10 * num5 + num6 * num11;
						num13 = num10 * num11 - num6 * num5;
						for (int m = 0; m < length; m++)
						{
							num11 = a[m, k];
							num12 = a[m, i];
							a[m, k] = num11 * num10 + num12 * num6;
							a[m, i] = num12 * num10 - num11 * num6;
						}
					}
					array[num] = 0.0;
					array[j] = num7;
					w[j] = num13;
				}
			}
		}

		private static double Sign(double a, double b)
		{
			if (!(b >= 0.0))
			{
				return 0.0 - System.Math.Abs(a);
			}
			return System.Math.Abs(a);
		}

		private static double Pythag(double a, double b)
		{
			double num = System.Math.Abs(a);
			double num2 = System.Math.Abs(b);
			if (num > num2)
			{
				double num3 = num2 / num;
				return num * System.Math.Sqrt(1.0 + num3 * num3);
			}
			if (num2 > 0.0)
			{
				double num3 = num / num2;
				return num2 * System.Math.Sqrt(1.0 + num3 * num3);
			}
			return 0.0;
		}
	}
}
