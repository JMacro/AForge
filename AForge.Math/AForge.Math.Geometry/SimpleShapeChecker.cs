using System;
using System.Collections.Generic;

namespace AForge.Math.Geometry
{
	public class SimpleShapeChecker
	{
		private FlatAnglesOptimizer shapeOptimizer = new FlatAnglesOptimizer(160f);

		private float minAcceptableDistortion = 0.5f;

		private float relativeDistortionLimit = 0.03f;

		private float angleError = 7f;

		private float lengthError = 0.1f;

		public float MinAcceptableDistortion
		{
			get
			{
				return minAcceptableDistortion;
			}
			set
			{
				minAcceptableDistortion = System.Math.Max(0f, value);
			}
		}

		public float RelativeDistortionLimit
		{
			get
			{
				return relativeDistortionLimit;
			}
			set
			{
				relativeDistortionLimit = System.Math.Max(0f, System.Math.Min(1f, value));
			}
		}

		public float AngleError
		{
			get
			{
				return angleError;
			}
			set
			{
				angleError = System.Math.Max(0f, System.Math.Min(20f, value));
			}
		}

		public float LengthError
		{
			get
			{
				return lengthError;
			}
			set
			{
				lengthError = System.Math.Max(0f, System.Math.Min(1f, value));
			}
		}

		public ShapeType CheckShapeType(List<IntPoint> edgePoints)
		{
			if (IsCircle(edgePoints))
			{
				return ShapeType.Circle;
			}
			if (IsConvexPolygon(edgePoints, out List<IntPoint> corners))
			{
				if (corners.Count != 4)
				{
					return ShapeType.Triangle;
				}
				return ShapeType.Quadrilateral;
			}
			return ShapeType.Unknown;
		}

		public bool IsCircle(List<IntPoint> edgePoints)
		{
			Point center;
			float radius;
			return IsCircle(edgePoints, out center, out radius);
		}

		public bool IsCircle(List<IntPoint> edgePoints, out Point center, out float radius)
		{
			if (edgePoints.Count < 8)
			{
				center = new Point(0f, 0f);
				radius = 0f;
				return false;
			}
			PointsCloud.GetBoundingRectangle(edgePoints, out IntPoint minXY, out IntPoint maxXY);
			IntPoint point = maxXY - minXY;
			center = minXY + (Point)point / 2f;
			radius = ((float)point.X + (float)point.Y) / 4f;
			float num = 0f;
			int i = 0;
			for (int count = edgePoints.Count; i < count; i++)
			{
				num += System.Math.Abs(center.DistanceTo(edgePoints[i]) - radius);
			}
			num /= (float)edgePoints.Count;
			float num2 = System.Math.Max(minAcceptableDistortion, ((float)point.X + (float)point.Y) / 2f * relativeDistortionLimit);
			return num <= num2;
		}

		public bool IsQuadrilateral(List<IntPoint> edgePoints)
		{
			List<IntPoint> corners;
			return IsQuadrilateral(edgePoints, out corners);
		}

		public bool IsQuadrilateral(List<IntPoint> edgePoints, out List<IntPoint> corners)
		{
			corners = GetShapeCorners(edgePoints);
			if (corners.Count != 4)
			{
				return false;
			}
			return CheckIfPointsFitShape(edgePoints, corners);
		}

		public bool IsTriangle(List<IntPoint> edgePoints)
		{
			List<IntPoint> corners;
			return IsTriangle(edgePoints, out corners);
		}

		public bool IsTriangle(List<IntPoint> edgePoints, out List<IntPoint> corners)
		{
			corners = GetShapeCorners(edgePoints);
			if (corners.Count != 3)
			{
				return false;
			}
			return CheckIfPointsFitShape(edgePoints, corners);
		}

		public bool IsConvexPolygon(List<IntPoint> edgePoints, out List<IntPoint> corners)
		{
			corners = GetShapeCorners(edgePoints);
			return CheckIfPointsFitShape(edgePoints, corners);
		}

		public PolygonSubType CheckPolygonSubType(List<IntPoint> corners)
		{
			PolygonSubType polygonSubType = PolygonSubType.Unknown;
			PointsCloud.GetBoundingRectangle(corners, out IntPoint minXY, out IntPoint maxXY);
			IntPoint intPoint = maxXY - minXY;
			float num = lengthError * (float)(intPoint.X + intPoint.Y) / 2f;
			if (corners.Count == 3)
			{
				float angleBetweenVectors = GeometryTools.GetAngleBetweenVectors(corners[0], corners[1], corners[2]);
				float angleBetweenVectors2 = GeometryTools.GetAngleBetweenVectors(corners[1], corners[2], corners[0]);
				float angleBetweenVectors3 = GeometryTools.GetAngleBetweenVectors(corners[2], corners[0], corners[1]);
				if (System.Math.Abs(angleBetweenVectors - 60f) <= angleError && System.Math.Abs(angleBetweenVectors2 - 60f) <= angleError && System.Math.Abs(angleBetweenVectors3 - 60f) <= angleError)
				{
					polygonSubType = PolygonSubType.EquilateralTriangle;
				}
				else
				{
					if (System.Math.Abs(angleBetweenVectors - angleBetweenVectors2) <= angleError || System.Math.Abs(angleBetweenVectors2 - angleBetweenVectors3) <= angleError || System.Math.Abs(angleBetweenVectors3 - angleBetweenVectors) <= angleError)
					{
						polygonSubType = PolygonSubType.IsoscelesTriangle;
					}
					if (System.Math.Abs(angleBetweenVectors - 90f) <= angleError || System.Math.Abs(angleBetweenVectors2 - 90f) <= angleError || System.Math.Abs(angleBetweenVectors3 - 90f) <= angleError)
					{
						polygonSubType = ((polygonSubType == PolygonSubType.IsoscelesTriangle) ? PolygonSubType.RectangledIsoscelesTriangle : PolygonSubType.RectangledTriangle);
					}
				}
			}
			else if (corners.Count == 4)
			{
				float angleBetweenLines = GeometryTools.GetAngleBetweenLines(corners[0], corners[1], corners[2], corners[3]);
				float angleBetweenLines2 = GeometryTools.GetAngleBetweenLines(corners[1], corners[2], corners[3], corners[0]);
				if (angleBetweenLines <= angleError)
				{
					polygonSubType = PolygonSubType.Trapezoid;
					if (angleBetweenLines2 <= angleError)
					{
						polygonSubType = PolygonSubType.Parallelogram;
						if (System.Math.Abs(GeometryTools.GetAngleBetweenVectors(corners[1], corners[0], corners[2]) - 90f) <= angleError)
						{
							polygonSubType = PolygonSubType.Rectangle;
						}
						float num2 = corners[0].DistanceTo(corners[1]);
						float num3 = corners[0].DistanceTo(corners[3]);
						if (System.Math.Abs(num2 - num3) <= num)
						{
							polygonSubType = ((polygonSubType == PolygonSubType.Parallelogram) ? PolygonSubType.Rhombus : PolygonSubType.Square);
						}
					}
				}
				else if (angleBetweenLines2 <= angleError)
				{
					polygonSubType = PolygonSubType.Trapezoid;
				}
			}
			return polygonSubType;
		}

		public bool CheckIfPointsFitShape(List<IntPoint> edgePoints, List<IntPoint> corners)
		{
			int count = corners.Count;
			float[] array = new float[count];
			float[] array2 = new float[count];
			float[] array3 = new float[count];
			bool[] array4 = new bool[count];
			for (int i = 0; i < count; i++)
			{
				IntPoint intPoint = corners[i];
				IntPoint intPoint2 = (i + 1 == count) ? corners[0] : corners[i + 1];
				if (!(array4[i] = (intPoint2.X == intPoint.X)))
				{
					array[i] = (float)(intPoint2.Y - intPoint.Y) / (float)(intPoint2.X - intPoint.X);
					array2[i] = (float)intPoint.Y - array[i] * (float)intPoint.X;
					array3[i] = (float)System.Math.Sqrt(array[i] * array[i] + 1f);
				}
			}
			float num = 0f;
			int j = 0;
			for (int count2 = edgePoints.Count; j < count2; j++)
			{
				float num2 = float.MaxValue;
				for (int k = 0; k < count; k++)
				{
					float num3 = 0f;
					num3 = (array4[k] ? ((float)System.Math.Abs(edgePoints[j].X - corners[k].X)) : System.Math.Abs((array[k] * (float)edgePoints[j].X + array2[k] - (float)edgePoints[j].Y) / array3[k]));
					if (num3 < num2)
					{
						num2 = num3;
					}
				}
				num += num2;
			}
			num /= (float)edgePoints.Count;
			PointsCloud.GetBoundingRectangle(corners, out IntPoint minXY, out IntPoint maxXY);
			IntPoint intPoint3 = maxXY - minXY;
			float num4 = System.Math.Max(minAcceptableDistortion, ((float)intPoint3.X + (float)intPoint3.Y) / 2f * relativeDistortionLimit);
			return num <= num4;
		}

		private List<IntPoint> GetShapeCorners(List<IntPoint> edgePoints)
		{
			return shapeOptimizer.OptimizeShape(PointsCloud.FindQuadrilateralCorners(edgePoints));
		}
	}
}
