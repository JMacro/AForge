using System;

namespace AForge.Math.Geometry
{
	public static class GeometryTools
	{
		public static float GetAngleBetweenVectors(Point startPoint, Point vector1end, Point vector2end)
		{
			float num = vector1end.X - startPoint.X;
			float num2 = vector1end.Y - startPoint.Y;
			float num3 = vector2end.X - startPoint.X;
			float num4 = vector2end.Y - startPoint.Y;
			return (float)(System.Math.Acos((double)(num * num3 + num2 * num4) / (System.Math.Sqrt(num * num + num2 * num2) * System.Math.Sqrt(num3 * num3 + num4 * num4))) * 180.0 / System.Math.PI);
		}

		public static float GetAngleBetweenLines(Point a1, Point a2, Point b1, Point b2)
		{
			Line line = Line.FromPoints(a1, a2);
			return line.GetAngleBetweenLines(Line.FromPoints(b1, b2));
		}
	}
}
