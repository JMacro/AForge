namespace AForge.Imaging
{
	internal static class Interpolation
	{
		public static double BiCubicKernel(double x)
		{
			if (x < 0.0)
			{
				x = 0.0 - x;
			}
			double result = 0.0;
			if (x <= 1.0)
			{
				result = (1.5 * x - 2.5) * x * x + 1.0;
			}
			else if (x < 2.0)
			{
				result = ((-0.5 * x + 2.5) * x - 4.0) * x + 2.0;
			}
			return result;
		}
	}
}
