using System.Collections.Generic;

namespace AForge.Imaging.Filters
{
	internal static class QuadTransformationCalcs
	{
		private const double TOLERANCE = 1E-13;

		private static double Det2(double a, double b, double c, double d)
		{
			return a * d - b * c;
		}

		private static double[,] MultiplyMatrix(double[,] a, double[,] b)
		{
			return new double[3, 3]
			{
				{
					a[0, 0] * b[0, 0] + a[0, 1] * b[1, 0] + a[0, 2] * b[2, 0],
					a[0, 0] * b[0, 1] + a[0, 1] * b[1, 1] + a[0, 2] * b[2, 1],
					a[0, 0] * b[0, 2] + a[0, 1] * b[1, 2] + a[0, 2] * b[2, 2]
				},
				{
					a[1, 0] * b[0, 0] + a[1, 1] * b[1, 0] + a[1, 2] * b[2, 0],
					a[1, 0] * b[0, 1] + a[1, 1] * b[1, 1] + a[1, 2] * b[2, 1],
					a[1, 0] * b[0, 2] + a[1, 1] * b[1, 2] + a[1, 2] * b[2, 2]
				},
				{
					a[2, 0] * b[0, 0] + a[2, 1] * b[1, 0] + a[2, 2] * b[2, 0],
					a[2, 0] * b[0, 1] + a[2, 1] * b[1, 1] + a[2, 2] * b[2, 1],
					a[2, 0] * b[0, 2] + a[2, 1] * b[1, 2] + a[2, 2] * b[2, 2]
				}
			};
		}

		private static double[,] AdjugateMatrix(double[,] a)
		{
			double[,] array = new double[3, 3];
			array[0, 0] = Det2(a[1, 1], a[1, 2], a[2, 1], a[2, 2]);
			array[1, 0] = Det2(a[1, 2], a[1, 0], a[2, 2], a[2, 0]);
			array[2, 0] = Det2(a[1, 0], a[1, 1], a[2, 0], a[2, 1]);
			array[0, 1] = Det2(a[2, 1], a[2, 2], a[0, 1], a[0, 2]);
			array[1, 1] = Det2(a[2, 2], a[2, 0], a[0, 2], a[0, 0]);
			array[2, 1] = Det2(a[2, 0], a[2, 1], a[0, 0], a[0, 1]);
			array[0, 2] = Det2(a[0, 1], a[0, 2], a[1, 1], a[1, 2]);
			array[1, 2] = Det2(a[0, 2], a[0, 0], a[1, 2], a[1, 0]);
			array[2, 2] = Det2(a[0, 0], a[0, 1], a[1, 0], a[1, 1]);
			return array;
		}

		private static double[,] MapSquareToQuad(List<IntPoint> quad)
		{
			double[,] array = new double[3, 3];
			double num = quad[0].X - quad[1].X + quad[2].X - quad[3].X;
			double num2 = quad[0].Y - quad[1].Y + quad[2].Y - quad[3].Y;
			if (num < 1E-13 && num > -1E-13 && num2 < 1E-13 && num2 > -1E-13)
			{
				array[0, 0] = quad[1].X - quad[0].X;
				array[0, 1] = quad[2].X - quad[1].X;
				array[0, 2] = quad[0].X;
				array[1, 0] = quad[1].Y - quad[0].Y;
				array[1, 1] = quad[2].Y - quad[1].Y;
				array[1, 2] = quad[0].Y;
				array[2, 0] = 0.0;
				array[2, 1] = 0.0;
				array[2, 2] = 1.0;
			}
			else
			{
				double a = quad[1].X - quad[2].X;
				double b = quad[3].X - quad[2].X;
				double c = quad[1].Y - quad[2].Y;
				double d = quad[3].Y - quad[2].Y;
				double num3 = Det2(a, b, c, d);
				if (num3 == 0.0)
				{
					return null;
				}
				array[2, 0] = Det2(num, b, num2, d) / num3;
				array[2, 1] = Det2(a, num, c, num2) / num3;
				array[2, 2] = 1.0;
				array[0, 0] = (double)(quad[1].X - quad[0].X) + array[2, 0] * (double)quad[1].X;
				array[0, 1] = (double)(quad[3].X - quad[0].X) + array[2, 1] * (double)quad[3].X;
				array[0, 2] = quad[0].X;
				array[1, 0] = (double)(quad[1].Y - quad[0].Y) + array[2, 0] * (double)quad[1].Y;
				array[1, 1] = (double)(quad[3].Y - quad[0].Y) + array[2, 1] * (double)quad[3].Y;
				array[1, 2] = quad[0].Y;
			}
			return array;
		}

		public static double[,] MapQuadToQuad(List<IntPoint> input, List<IntPoint> output)
		{
			double[,] a = MapSquareToQuad(input);
			double[,] array = MapSquareToQuad(output);
			if (array == null)
			{
				return null;
			}
			return MultiplyMatrix(array, AdjugateMatrix(a));
		}
	}
}
