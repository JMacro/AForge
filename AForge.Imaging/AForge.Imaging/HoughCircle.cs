using System;

namespace AForge.Imaging
{
	public class HoughCircle : IComparable
	{
		public readonly int X;

		public readonly int Y;

		public readonly int Radius;

		public readonly short Intensity;

		public readonly double RelativeIntensity;

		public HoughCircle(int x, int y, int radius, short intensity, double relativeIntensity)
		{
			X = x;
			Y = y;
			Radius = radius;
			Intensity = intensity;
			RelativeIntensity = relativeIntensity;
		}

		public int CompareTo(object value)
		{
			return -Intensity.CompareTo(((HoughCircle)value).Intensity);
		}
	}
}
