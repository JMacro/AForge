namespace AForge.Imaging.ColorReduction
{
	public class ColorErrorDiffusionToAdjacentNeighbors : ErrorDiffusionColorDithering
	{
		private int[][] coefficients;

		private int coefficientsSum;

		public int[][] Coefficients
		{
			get
			{
				return coefficients;
			}
			set
			{
				coefficients = value;
				CalculateCoefficientsSum();
			}
		}

		public ColorErrorDiffusionToAdjacentNeighbors(int[][] coefficients)
		{
			this.coefficients = coefficients;
			CalculateCoefficientsSum();
		}

		protected unsafe override void Diffuse(int rError, int gError, int bError, byte* ptr)
		{
			int[] array = coefficients[0];
			int num = 1;
			int num2 = pixelSize;
			int num3 = 0;
			int num4 = array.Length;
			while (num3 < num4 && x + num < width)
			{
				int num5 = ptr[num2 + 2] + rError * array[num3] / coefficientsSum;
				num5 = ((num5 >= 0) ? ((num5 > 255) ? 255 : num5) : 0);
				ptr[num2 + 2] = (byte)num5;
				int num6 = ptr[num2 + 1] + gError * array[num3] / coefficientsSum;
				num6 = ((num6 >= 0) ? ((num6 > 255) ? 255 : num6) : 0);
				ptr[num2 + 1] = (byte)num6;
				int num7 = ptr[num2] + bError * array[num3] / coefficientsSum;
				num7 = ((num7 >= 0) ? ((num7 > 255) ? 255 : num7) : 0);
				ptr[num2] = (byte)num7;
				num++;
				num3++;
				num2 += pixelSize;
			}
			int i = 1;
			for (int num8 = coefficients.Length; i < num8 && y + i < height; i++)
			{
				ptr += stride;
				array = coefficients[i];
				int num9 = 0;
				int num10 = array.Length;
				int num11 = -(num10 >> 1);
				int num12 = -(num10 >> 1) * pixelSize;
				while (num9 < num10 && x + num11 < width)
				{
					if (x + num11 >= 0)
					{
						int num5 = ptr[num12 + 2] + rError * array[num9] / coefficientsSum;
						num5 = ((num5 >= 0) ? ((num5 > 255) ? 255 : num5) : 0);
						ptr[num12 + 2] = (byte)num5;
						int num6 = ptr[num12 + 1] + gError * array[num9] / coefficientsSum;
						num6 = ((num6 >= 0) ? ((num6 > 255) ? 255 : num6) : 0);
						ptr[num12 + 1] = (byte)num6;
						int num7 = ptr[num12] + bError * array[num9] / coefficientsSum;
						num7 = ((num7 >= 0) ? ((num7 > 255) ? 255 : num7) : 0);
						ptr[num12] = (byte)num7;
					}
					num11++;
					num9++;
					num12 += pixelSize;
				}
			}
		}

		private void CalculateCoefficientsSum()
		{
			coefficientsSum = 0;
			int i = 0;
			for (int num = coefficients.Length; i < num; i++)
			{
				int[] array = coefficients[i];
				int j = 0;
				for (int num2 = array.Length; j < num2; j++)
				{
					coefficientsSum += array[j];
				}
			}
		}
	}
}
