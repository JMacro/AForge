using System;
using System.Globalization;

namespace AForge.Math.Geometry
{
	public sealed class Line
	{
		private readonly float k;

		private readonly float b;

		public bool IsVertical => float.IsInfinity(k);

		public bool IsHorizontal => k == 0f;

		public float Slope => k;

		public float Intercept => b;

		public static Line FromPoints(Point point1, Point point2)
		{
			return new Line(point1, point2);
		}

		public static Line FromSlopeIntercept(float slope, float intercept)
		{
			return new Line(slope, intercept);
		}

		public static Line FromRTheta(float radius, float theta)
		{
			return new Line(radius, theta, unused: false);
		}

		public static Line FromPointTheta(Point point, float theta)
		{
			return new Line(point, theta);
		}

		private Line(Point start, Point end)
		{
			if (start == end)
			{
				throw new ArgumentException("Start point of the line cannot be the same as its end point.");
			}
			k = (end.Y - start.Y) / (end.X - start.X);
			b = (float.IsInfinity(k) ? start.X : (start.Y - k * start.X));
		}

		private Line(float slope, float intercept)
		{
			k = slope;
			b = intercept;
		}

		private Line(float radius, float theta, bool unused)
		{
			if (radius < 0f)
			{
				throw new ArgumentOutOfRangeException("radius", radius, "Must be non-negative");
			}
			theta *= (float)System.Math.PI / 180f;
			float num = (float)System.Math.Sin(theta);
			float num2 = (float)System.Math.Cos(theta);
			Point point = new Point(radius * num2, radius * num);
			k = (0f - num2) / num;
			if (!float.IsInfinity(k))
			{
				b = point.Y - k * point.X;
			}
			else
			{
				b = System.Math.Abs(radius);
			}
		}

		private Line(Point point, float theta)
		{
			theta *= (float)System.Math.PI / 180f;
			k = (float)(-1.0 / System.Math.Tan(theta));
			if (!float.IsInfinity(k))
			{
				b = point.Y - k * point.X;
			}
			else
			{
				b = point.X;
			}
		}

		public float GetAngleBetweenLines(Line secondLine)
		{
			float num = secondLine.k;
			bool isVertical = IsVertical;
			bool isVertical2 = secondLine.IsVertical;
			if (k == num || (isVertical && isVertical2))
			{
				return 0f;
			}
			float num2 = 0f;
			if (isVertical || isVertical2)
			{
				num2 = ((!isVertical) ? ((float)(System.Math.PI / 2.0 - System.Math.Atan(k) * (double)System.Math.Sign(k))) : ((float)(System.Math.PI / 2.0 - System.Math.Atan(num) * (double)System.Math.Sign(num))));
			}
			else
			{
				float num3 = ((num > k) ? (num - k) : (k - num)) / (1f + k * num);
				num2 = (float)System.Math.Atan(num3);
			}
			num2 *= 57.29578f;
			if (num2 < 0f)
			{
				num2 = 0f - num2;
			}
			return num2;
		}

		public Point? GetIntersectionWith(Line secondLine)
		{
			float num = secondLine.k;
			float num2 = secondLine.b;
			bool isVertical = IsVertical;
			bool isVertical2 = secondLine.IsVertical;
			Point? result = null;
			if (k == num || (isVertical && isVertical2))
			{
				if (b == num2)
				{
					throw new InvalidOperationException("Identical lines do not have an intersection point.");
				}
			}
			else if (isVertical)
			{
				result = new Point(b, num * b + num2);
			}
			else if (isVertical2)
			{
				result = new Point(num2, k * num2 + b);
			}
			else
			{
				float num3 = (num2 - b) / (k - num);
				result = new Point(num3, k * num3 + b);
			}
			return result;
		}

		public Point? GetIntersectionWith(LineSegment other)
		{
			return other.GetIntersectionWith(this);
		}

		public float DistanceToPoint(Point point)
		{
			if (!IsVertical)
			{
				float num = (float)System.Math.Sqrt(k * k + 1f);
				return System.Math.Abs((k * point.X + b - point.Y) / num);
			}
			return System.Math.Abs(b - point.X);
		}

		public static bool operator ==(Line line1, Line line2)
		{
			if (object.ReferenceEquals(line1, line2))
			{
				return true;
			}
			if ((object)line1 == null || (object)line2 == null)
			{
				return false;
			}
			if (line1.k == line2.k)
			{
				return line1.b == line2.b;
			}
			return false;
		}

		public static bool operator !=(Line line1, Line line2)
		{
			return !(line1 == line2);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Line))
			{
				return false;
			}
			return this == (Line)obj;
		}

		public override int GetHashCode()
		{
			return k.GetHashCode() + b.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "k = {0}, b = {1}", k, b);
		}
	}
}
