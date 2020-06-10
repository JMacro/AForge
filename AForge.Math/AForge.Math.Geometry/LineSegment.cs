using System;
using System.Globalization;

namespace AForge.Math.Geometry
{
	public sealed class LineSegment
	{
		private enum ProjectionLocation
		{
			RayA,
			SegmentAB,
			RayB
		}

		private readonly Point start;

		private readonly Point end;

		private readonly Line line;

		public Point Start => start;

		public Point End => end;

		public float Length => start.DistanceTo(end);

		public LineSegment(Point start, Point end)
		{
			line = Line.FromPoints(start, end);
			this.start = start;
			this.end = end;
		}

		public static explicit operator Line(LineSegment segment)
		{
			return segment.line;
		}

		public float DistanceToPoint(Point point)
		{
			switch (LocateProjection(point))
			{
			case ProjectionLocation.RayA:
				return point.DistanceTo(start);
			case ProjectionLocation.RayB:
				return point.DistanceTo(end);
			default:
				return line.DistanceToPoint(point);
			}
		}

		public Point? GetIntersectionWith(LineSegment other)
		{
			Point? result = null;
			if (line.Slope == other.line.Slope || (line.IsVertical && other.line.IsVertical))
			{
				if (line.Intercept == other.line.Intercept)
				{
					ProjectionLocation projectionLocation = LocateProjection(other.start);
					ProjectionLocation projectionLocation2 = LocateProjection(other.end);
					if (projectionLocation != ProjectionLocation.SegmentAB && projectionLocation == projectionLocation2)
					{
						result = null;
					}
					else if ((start == other.start && projectionLocation2 == ProjectionLocation.RayA) || (start == other.end && projectionLocation == ProjectionLocation.RayA))
					{
						result = start;
					}
					else
					{
						if ((!(end == other.start) || projectionLocation2 != ProjectionLocation.RayB) && (!(end == other.end) || projectionLocation != ProjectionLocation.RayB))
						{
							throw new InvalidOperationException("Overlapping segments do not have a single intersection point.");
						}
						result = end;
					}
				}
			}
			else
			{
				result = GetIntersectionWith(other.line);
				if (result.HasValue && other.LocateProjection(result.Value) != ProjectionLocation.SegmentAB)
				{
					result = null;
				}
			}
			return result;
		}

		public Point? GetIntersectionWith(Line other)
		{
			Point? result;
			if (line.Slope == other.Slope || (line.IsVertical && other.IsVertical))
			{
				if (line.Intercept == other.Intercept)
				{
					throw new InvalidOperationException("Segment is a portion of the specified line.");
				}
				result = null;
			}
			else
			{
				result = line.GetIntersectionWith(other);
			}
			if (result.HasValue && LocateProjection(result.Value) != ProjectionLocation.SegmentAB)
			{
				result = null;
			}
			return result;
		}

		private ProjectionLocation LocateProjection(Point point)
		{
			Point point2 = end - start;
			Point point3 = point - start;
			float num = point3.X * point2.X + point3.Y * point2.Y;
			float num2 = point2.X * point2.X + point2.Y * point2.Y;
			return (!(num < 0f)) ? ((!(num > num2)) ? ProjectionLocation.SegmentAB : ProjectionLocation.RayB) : ProjectionLocation.RayA;
		}

		public static bool operator ==(LineSegment line1, LineSegment line2)
		{
			if (object.ReferenceEquals(line1, line2))
			{
				return true;
			}
			if ((object)line1 == null || (object)line2 == null)
			{
				return false;
			}
			if (line1.start == line2.start)
			{
				return line1.end == line2.end;
			}
			return false;
		}

		public static bool operator !=(LineSegment line1, LineSegment line2)
		{
			return !(line1 == line2);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is LineSegment))
			{
				return false;
			}
			return this == (LineSegment)obj;
		}

		public override int GetHashCode()
		{
			return start.GetHashCode() + end.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "({0}) -> ({1})", start, end);
		}
	}
}
