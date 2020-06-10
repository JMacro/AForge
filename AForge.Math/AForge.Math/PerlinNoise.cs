using System;

namespace AForge.Math
{
	public class PerlinNoise
	{
		private double initFrequency = 1.0;

		private double initAmplitude = 1.0;

		private double persistence = 0.65;

		private int octaves = 4;

		public double InitFrequency
		{
			get
			{
				return initFrequency;
			}
			set
			{
				initFrequency = value;
			}
		}

		public double InitAmplitude
		{
			get
			{
				return initAmplitude;
			}
			set
			{
				initAmplitude = value;
			}
		}

		public double Persistence
		{
			get
			{
				return persistence;
			}
			set
			{
				persistence = value;
			}
		}

		public int Octaves
		{
			get
			{
				return octaves;
			}
			set
			{
				octaves = System.Math.Max(1, System.Math.Min(32, value));
			}
		}

		public PerlinNoise()
		{
		}

		public PerlinNoise(int octaves, double persistence)
		{
			this.octaves = octaves;
			this.persistence = persistence;
		}

		public PerlinNoise(int octaves, double persistence, double initFrequency, double initAmplitude)
		{
			this.octaves = octaves;
			this.persistence = persistence;
			this.initFrequency = initFrequency;
			this.initAmplitude = initAmplitude;
		}

		public double Function(double x)
		{
			double num = initFrequency;
			double num2 = initAmplitude;
			double num3 = 0.0;
			for (int i = 0; i < octaves; i++)
			{
				num3 += SmoothedNoise(x * num) * num2;
				num *= 2.0;
				num2 *= persistence;
			}
			return num3;
		}

		public double Function2D(double x, double y)
		{
			double num = initFrequency;
			double num2 = initAmplitude;
			double num3 = 0.0;
			for (int i = 0; i < octaves; i++)
			{
				num3 += SmoothedNoise(x * num, y * num) * num2;
				num *= 2.0;
				num2 *= persistence;
			}
			return num3;
		}

		private double Noise(int x)
		{
			int num = (x << 13) ^ x;
			return 1.0 - (double)((num * (num * num * 15731 + 789221) + 1376312589) & int.MaxValue) / 1073741824.0;
		}

		private double Noise(int x, int y)
		{
			int num = x + y * 57;
			num = ((num << 13) ^ num);
			return 1.0 - (double)((num * (num * num * 15731 + 789221) + 1376312589) & int.MaxValue) / 1073741824.0;
		}

		private double SmoothedNoise(double x)
		{
			int num = (int)x;
			double a = x - (double)num;
			return CosineInterpolate(Noise(num), Noise(num + 1), a);
		}

		private double SmoothedNoise(double x, double y)
		{
			int num = (int)x;
			int num2 = (int)y;
			double a = x - (double)num;
			double a2 = y - (double)num2;
			double x2 = Noise(num, num2);
			double x3 = Noise(num + 1, num2);
			double x4 = Noise(num, num2 + 1);
			double x5 = Noise(num + 1, num2 + 1);
			double x6 = CosineInterpolate(x2, x3, a);
			double x7 = CosineInterpolate(x4, x5, a);
			return CosineInterpolate(x6, x7, a2);
		}

		private double CosineInterpolate(double x1, double x2, double a)
		{
			double num = (1.0 - System.Math.Cos(a * System.Math.PI)) * 0.5;
			return x1 * (1.0 - num) + x2 * num;
		}
	}
}
