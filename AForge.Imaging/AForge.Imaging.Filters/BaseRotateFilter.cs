using System;
using System.Drawing;

namespace AForge.Imaging.Filters
{
	public abstract class BaseRotateFilter : BaseTransformationFilter
	{
		protected double angle;

		protected bool keepSize;

		protected Color fillColor = Color.FromArgb(0, 0, 0);

		public double Angle
		{
			get
			{
				return angle;
			}
			set
			{
				angle = value % 360.0;
			}
		}

		public bool KeepSize
		{
			get
			{
				return keepSize;
			}
			set
			{
				keepSize = value;
			}
		}

		public Color FillColor
		{
			get
			{
				return fillColor;
			}
			set
			{
				fillColor = value;
			}
		}

		public BaseRotateFilter(double angle)
		{
			this.angle = angle;
		}

		public BaseRotateFilter(double angle, bool keepSize)
		{
			this.angle = angle;
			this.keepSize = keepSize;
		}

		protected override Size CalculateNewImageSize(UnmanagedImage sourceData)
		{
			if (keepSize)
			{
				return new Size(sourceData.Width, sourceData.Height);
			}
			double num = (0.0 - angle) * System.Math.PI / 180.0;
			double num2 = System.Math.Cos(num);
			double num3 = System.Math.Sin(num);
			double num4 = (double)sourceData.Width / 2.0;
			double num5 = (double)sourceData.Height / 2.0;
			double val = num4 * num2;
			double val2 = num4 * num3;
			double val3 = num4 * num2 - num5 * num3;
			double val4 = num4 * num3 + num5 * num2;
			double val5 = (0.0 - num5) * num3;
			double val6 = num5 * num2;
			double val7 = 0.0;
			double val8 = 0.0;
			num4 = System.Math.Max(System.Math.Max(val, val3), System.Math.Max(val5, val7)) - System.Math.Min(System.Math.Min(val, val3), System.Math.Min(val5, val7));
			num5 = System.Math.Max(System.Math.Max(val2, val4), System.Math.Max(val6, val8)) - System.Math.Min(System.Math.Min(val2, val4), System.Math.Min(val6, val8));
			return new Size((int)(num4 * 2.0 + 0.5), (int)(num5 * 2.0 + 0.5));
		}
	}
}
