using System;

namespace AForge.Math
{
	public static class FourierTransform
	{
		public enum Direction
		{
			Forward = 1,
			Backward = -1
		}

		private const int minLength = 2;

		private const int maxLength = 16384;

		private const int minBits = 1;

		private const int maxBits = 14;

		private static int[][] reversedBits = new int[14][];

		private static Complex[,][] complexRotation = new Complex[14, 2][];

		public static void DFT(Complex[] data, Direction direction)
		{
			int num = data.Length;
			Complex[] array = new Complex[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = Complex.Zero;
				double num2 = (double)(0 - direction) * 2.0 * System.Math.PI * (double)i / (double)num;
				for (int j = 0; j < num; j++)
				{
					double num3 = System.Math.Cos((double)j * num2);
					double num4 = System.Math.Sin((double)j * num2);
					array[i].Re += data[j].Re * num3 - data[j].Im * num4;
					array[i].Im += data[j].Re * num4 + data[j].Im * num3;
				}
			}
			if (direction == Direction.Forward)
			{
				for (int k = 0; k < num; k++)
				{
					data[k].Re = array[k].Re / (double)num;
					data[k].Im = array[k].Im / (double)num;
				}
			}
			else
			{
				for (int l = 0; l < num; l++)
				{
					data[l].Re = array[l].Re;
					data[l].Im = array[l].Im;
				}
			}
		}

		public static void DFT2(Complex[,] data, Direction direction)
		{
			int length = data.GetLength(0);
			int length2 = data.GetLength(1);
			Complex[] array = new Complex[System.Math.Max(length, length2)];
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					array[j] = Complex.Zero;
					double num = (double)(0 - direction) * 2.0 * System.Math.PI * (double)j / (double)length2;
					for (int k = 0; k < length2; k++)
					{
						double num2 = System.Math.Cos((double)k * num);
						double num3 = System.Math.Sin((double)k * num);
						array[j].Re += data[i, k].Re * num2 - data[i, k].Im * num3;
						array[j].Im += data[i, k].Re * num3 + data[i, k].Im * num2;
					}
				}
				if (direction == Direction.Forward)
				{
					for (int l = 0; l < length2; l++)
					{
						data[i, l].Re = array[l].Re / (double)length2;
						data[i, l].Im = array[l].Im / (double)length2;
					}
				}
				else
				{
					for (int m = 0; m < length2; m++)
					{
						data[i, m].Re = array[m].Re;
						data[i, m].Im = array[m].Im;
					}
				}
			}
			for (int n = 0; n < length2; n++)
			{
				for (int num4 = 0; num4 < length; num4++)
				{
					array[num4] = Complex.Zero;
					double num = (double)(0 - direction) * 2.0 * System.Math.PI * (double)num4 / (double)length;
					for (int num5 = 0; num5 < length; num5++)
					{
						double num2 = System.Math.Cos((double)num5 * num);
						double num3 = System.Math.Sin((double)num5 * num);
						array[num4].Re += data[num5, n].Re * num2 - data[num5, n].Im * num3;
						array[num4].Im += data[num5, n].Re * num3 + data[num5, n].Im * num2;
					}
				}
				if (direction == Direction.Forward)
				{
					for (int num6 = 0; num6 < length; num6++)
					{
						data[num6, n].Re = array[num6].Re / (double)length;
						data[num6, n].Im = array[num6].Im / (double)length;
					}
				}
				else
				{
					for (int num7 = 0; num7 < length; num7++)
					{
						data[num7, n].Re = array[num7].Re;
						data[num7, n].Im = array[num7].Im;
					}
				}
			}
		}

		public static void FFT(Complex[] data, Direction direction)
		{
			int num = data.Length;
			int num2 = Tools.Log2(num);
			ReorderData(data);
			int num3 = 1;
			for (int i = 1; i <= num2; i++)
			{
				Complex[] array = GetComplexRotation(i, direction);
				int num4 = num3;
				num3 <<= 1;
				for (int j = 0; j < num4; j++)
				{
					Complex complex = array[j];
					for (int k = j; k < num; k += num3)
					{
						int num5 = k + num4;
						Complex complex2 = data[k];
						Complex complex3 = data[num5];
						double num6 = complex3.Re * complex.Re - complex3.Im * complex.Im;
						double num7 = complex3.Re * complex.Im + complex3.Im * complex.Re;
						data[k].Re += num6;
						data[k].Im += num7;
						data[num5].Re = complex2.Re - num6;
						data[num5].Im = complex2.Im - num7;
					}
				}
			}
			if (direction == Direction.Forward)
			{
				for (int l = 0; l < num; l++)
				{
					data[l].Re /= num;
					data[l].Im /= num;
				}
			}
		}

		public static void FFT2(Complex[,] data, Direction direction)
		{
			int length = data.GetLength(0);
			int length2 = data.GetLength(1);
			if (!Tools.IsPowerOf2(length) || !Tools.IsPowerOf2(length2) || length < 2 || length > 16384 || length2 < 2 || length2 > 16384)
			{
				throw new ArgumentException("Incorrect data length.");
			}
			Complex[] array = new Complex[length2];
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					array[j] = data[i, j];
				}
				FFT(array, direction);
				for (int k = 0; k < length2; k++)
				{
					data[i, k] = array[k];
				}
			}
			Complex[] array2 = new Complex[length];
			for (int l = 0; l < length2; l++)
			{
				for (int m = 0; m < length; m++)
				{
					array2[m] = data[m, l];
				}
				FFT(array2, direction);
				for (int n = 0; n < length; n++)
				{
					data[n, l] = array2[n];
				}
			}
		}

		private static int[] GetReversedBits(int numberOfBits)
		{
			if (numberOfBits < 1 || numberOfBits > 14)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (reversedBits[numberOfBits - 1] == null)
			{
				int num = Tools.Pow2(numberOfBits);
				int[] array = new int[num];
				for (int i = 0; i < num; i++)
				{
					int num2 = i;
					int num3 = 0;
					for (int j = 0; j < numberOfBits; j++)
					{
						num3 = ((num3 << 1) | (num2 & 1));
						num2 >>= 1;
					}
					array[i] = num3;
				}
				reversedBits[numberOfBits - 1] = array;
			}
			return reversedBits[numberOfBits - 1];
		}

		private static Complex[] GetComplexRotation(int numberOfBits, Direction direction)
		{
			int num = (direction != Direction.Forward) ? 1 : 0;
			if (complexRotation[numberOfBits - 1, num] == null)
			{
				int num2 = 1 << numberOfBits - 1;
				double num3 = 1.0;
				double num4 = 0.0;
				double num5 = System.Math.PI / (double)num2 * (double)direction;
				double num6 = System.Math.Cos(num5);
				double num7 = System.Math.Sin(num5);
				Complex[] array = new Complex[num2];
				for (int i = 0; i < num2; i++)
				{
					array[i] = new Complex(num3, num4);
					double num8 = num3 * num7 + num4 * num6;
					num3 = num3 * num6 - num4 * num7;
					num4 = num8;
				}
				complexRotation[numberOfBits - 1, num] = array;
			}
			return complexRotation[numberOfBits - 1, num];
		}

		private static void ReorderData(Complex[] data)
		{
			int num = data.Length;
			if (num < 2 || num > 16384 || !Tools.IsPowerOf2(num))
			{
				throw new ArgumentException("Incorrect data length.");
			}
			int[] array = GetReversedBits(Tools.Log2(num));
			for (int i = 0; i < num; i++)
			{
				int num2 = array[i];
				if (num2 > i)
				{
					Complex complex = data[i];
					data[i] = data[num2];
					data[num2] = complex;
				}
			}
		}
	}
}
