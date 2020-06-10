namespace AForge.Imaging.Filters
{
	public class ErrorDiffusionToAdjacentNeighbors : ErrorDiffusionDithering
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

		public ErrorDiffusionToAdjacentNeighbors(int[][] coefficients)
		{
			this.coefficients = coefficients;
			CalculateCoefficientsSum();
		}

		protected unsafe override void Diffuse(int error, byte* ptr)
		{
			int[] array = coefficients[0];
			int num = 1;
			int i = 0;
			for (int num2 = array.Length; i < num2; i++)
			{
				if (x + num >= stopX)
				{
					break;
				}
				int num3 = ptr[num] + error * array[i] / coefficientsSum;
				num3 = ((num3 >= 0) ? ((num3 > 255) ? 255 : num3) : 0);
				ptr[num] = (byte)num3;
				num++;
			}
			int j = 1;
			for (int num4 = coefficients.Length; j < num4 && y + j < stopY; j++)
			{
				ptr += stride;
				array = coefficients[j];
				int k = 0;
				int num5 = array.Length;
				int num6 = -(num5 >> 1);
				for (; k < num5; k++)
				{
					if (x + num6 >= stopX)
					{
						break;
					}
					if (x + num6 >= startX)
					{
						int num3 = ptr[num6] + error * array[k] / coefficientsSum;
						num3 = ((num3 >= 0) ? ((num3 > 255) ? 255 : num3) : 0);
						ptr[num6] = (byte)num3;
					}
					num6++;
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
