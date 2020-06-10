using System;
using System.Collections.Generic;

namespace AForge.Math.Geometry
{
	public static class PointsCloud
	{
		private static float quadrilateralRelativeDistortionLimit = 0.1f;

		public static float QuadrilateralRelativeDistortionLimit
		{
			get
			{
				return quadrilateralRelativeDistortionLimit;
			}
			set
			{
				quadrilateralRelativeDistortionLimit = System.Math.Max(0f, System.Math.Min(0.25f, value));
			}
		}

		public static void Shift(IList<IntPoint> cloud, IntPoint shift)
		{
			int i = 0;
			for (int count = cloud.Count; i < count; i++)
			{
				cloud[i] += shift;
			}
		}

		public static void GetBoundingRectangle(IEnumerable<IntPoint> cloud, out IntPoint minXY, out IntPoint maxXY)
		{
			int num = int.MaxValue;
			int num2 = int.MinValue;
			int num3 = int.MaxValue;
			int num4 = int.MinValue;
			foreach (IntPoint item in cloud)
			{
				int x = item.X;
				int y = item.Y;
				if (x < num)
				{
					num = x;
				}
				if (x > num2)
				{
					num2 = x;
				}
				if (y < num3)
				{
					num3 = y;
				}
				if (y > num4)
				{
					num4 = y;
				}
			}
			if (num > num2)
			{
				throw new ArgumentException("List of points can not be empty.");
			}
			minXY = new IntPoint(num, num3);
			maxXY = new IntPoint(num2, num4);
		}

		public static Point GetCenterOfGravity(IEnumerable<IntPoint> cloud)
		{
			int num = 0;
			float num2 = 0f;
			float num3 = 0f;
			foreach (IntPoint item in cloud)
			{
				num2 += (float)item.X;
				num3 += (float)item.Y;
				num++;
			}
			num2 /= (float)num;
			num3 /= (float)num;
			return new Point(num2, num3);
		}

		public static IntPoint GetFurthestPoint(IEnumerable<IntPoint> cloud, IntPoint referencePoint)
		{
			IntPoint result = referencePoint;
			float num = -1f;
			int x = referencePoint.X;
			int y = referencePoint.Y;
			foreach (IntPoint item in cloud)
			{
				int num2 = x - item.X;
				int num3 = y - item.Y;
				float num4 = num2 * num2 + num3 * num3;
				if (num4 > num)
				{
					num = num4;
					result = item;
				}
			}
			return result;
		}

		public static void GetFurthestPointsFromLine(IEnumerable<IntPoint> cloud, IntPoint linePoint1, IntPoint linePoint2, out IntPoint furthestPoint1, out IntPoint furthestPoint2)
		{
			GetFurthestPointsFromLine(cloud, linePoint1, linePoint2, out furthestPoint1, out float _, out furthestPoint2, out float _);
		}

		public static void GetFurthestPointsFromLine(IEnumerable<IntPoint> cloud, IntPoint linePoint1, IntPoint linePoint2, out IntPoint furthestPoint1, out float distance1, out IntPoint furthestPoint2, out float distance2)
		{
			furthestPoint1 = linePoint1;
			distance1 = 0f;
			furthestPoint2 = linePoint2;
			distance2 = 0f;
			if (linePoint2.X != linePoint1.X)
			{
				float num = (float)(linePoint2.Y - linePoint1.Y) / (float)(linePoint2.X - linePoint1.X);
				float num2 = (float)linePoint1.Y - num * (float)linePoint1.X;
				float num3 = (float)System.Math.Sqrt(num * num + 1f);
				float num4 = 0f;
				foreach (IntPoint item in cloud)
				{
					num4 = (num * (float)item.X + num2 - (float)item.Y) / num3;
					if (num4 > distance1)
					{
						distance1 = num4;
						furthestPoint1 = item;
					}
					if (num4 < distance2)
					{
						distance2 = num4;
						furthestPoint2 = item;
					}
				}
			}
			else
			{
				int x = linePoint1.X;
				float num5 = 0f;
				foreach (IntPoint item2 in cloud)
				{
					num5 = x - item2.X;
					if (num5 > distance1)
					{
						distance1 = num5;
						furthestPoint1 = item2;
					}
					if (num5 < distance2)
					{
						distance2 = num5;
						furthestPoint2 = item2;
					}
				}
			}
			distance2 = 0f - distance2;
		}

		public static IntPoint GetFurthestPointFromLine(IEnumerable<IntPoint> cloud, IntPoint linePoint1, IntPoint linePoint2)
		{
			float distance;
			return GetFurthestPointFromLine(cloud, linePoint1, linePoint2, out distance);
		}

		public static IntPoint GetFurthestPointFromLine(IEnumerable<IntPoint> cloud, IntPoint linePoint1, IntPoint linePoint2, out float distance)
		{
			IntPoint result = linePoint1;
			distance = 0f;
			if (linePoint2.X != linePoint1.X)
			{
				float num = (float)(linePoint2.Y - linePoint1.Y) / (float)(linePoint2.X - linePoint1.X);
				float num2 = (float)linePoint1.Y - num * (float)linePoint1.X;
				float num3 = (float)System.Math.Sqrt(num * num + 1f);
				float num4 = 0f;
				{
					foreach (IntPoint item in cloud)
					{
						num4 = System.Math.Abs((num * (float)item.X + num2 - (float)item.Y) / num3);
						if (num4 > distance)
						{
							distance = num4;
							result = item;
						}
					}
					return result;
				}
			}
			int x = linePoint1.X;
			float num5 = 0f;
			foreach (IntPoint item2 in cloud)
			{
				distance = System.Math.Abs(x - item2.X);
				if (num5 > distance)
				{
					distance = num5;
					result = item2;
				}
			}
			return result;
		}

		public static List<IntPoint> FindQuadrilateralCorners(IEnumerable<IntPoint> cloud)
		{
			List<IntPoint> list = new List<IntPoint>();
			GetBoundingRectangle(cloud, out IntPoint minXY, out IntPoint maxXY);
			IntPoint point = maxXY - minXY;
			IntPoint referencePoint = minXY + point / 2;
			float num = quadrilateralRelativeDistortionLimit * (float)(point.X + point.Y) / 2f;
			IntPoint furthestPoint = GetFurthestPoint(cloud, referencePoint);
			IntPoint furthestPoint2 = GetFurthestPoint(cloud, furthestPoint);
			list.Add(furthestPoint);
			list.Add(furthestPoint2);
			GetFurthestPointsFromLine(cloud, furthestPoint, furthestPoint2, out IntPoint furthestPoint3, out float distance, out IntPoint furthestPoint4, out float distance2);
			if ((distance >= num && distance2 >= num) || (distance < num && distance != 0f && distance2 < num && distance2 != 0f))
			{
				if (!list.Contains(furthestPoint3))
				{
					list.Add(furthestPoint3);
				}
				if (!list.Contains(furthestPoint4))
				{
					list.Add(furthestPoint4);
				}
			}
			else
			{
				IntPoint furthestPoint5 = (distance > distance2) ? furthestPoint3 : furthestPoint4;
				GetFurthestPointsFromLine(cloud, furthestPoint, furthestPoint5, out furthestPoint3, out distance, out furthestPoint4, out distance2);
				bool flag = false;
				if (distance >= num && distance2 >= num)
				{
					if (furthestPoint4.DistanceTo(furthestPoint2) > furthestPoint3.DistanceTo(furthestPoint2))
					{
						furthestPoint3 = furthestPoint4;
					}
					flag = true;
				}
				else
				{
					GetFurthestPointsFromLine(cloud, furthestPoint2, furthestPoint5, out furthestPoint3, out distance, out furthestPoint4, out distance2);
					if (distance >= num && distance2 >= num)
					{
						if (furthestPoint4.DistanceTo(furthestPoint) > furthestPoint3.DistanceTo(furthestPoint))
						{
							furthestPoint3 = furthestPoint4;
						}
						flag = true;
					}
				}
				if (!flag)
				{
					list.Add(furthestPoint5);
				}
				else
				{
					list.Add(furthestPoint3);
					GetFurthestPointsFromLine(cloud, furthestPoint, furthestPoint3, out furthestPoint5, out float distance3, out furthestPoint4, out distance2);
					if (distance2 >= num && distance3 >= num)
					{
						if (furthestPoint5.DistanceTo(furthestPoint2) > furthestPoint4.DistanceTo(furthestPoint2))
						{
							furthestPoint4 = furthestPoint5;
						}
					}
					else
					{
						GetFurthestPointsFromLine(cloud, furthestPoint2, furthestPoint3, out furthestPoint5, out distance3, out furthestPoint4, out distance2);
						if (furthestPoint5.DistanceTo(furthestPoint) > furthestPoint4.DistanceTo(furthestPoint) && furthestPoint5 != furthestPoint2 && furthestPoint5 != furthestPoint3)
						{
							furthestPoint4 = furthestPoint5;
						}
					}
					if (furthestPoint4 != furthestPoint && furthestPoint4 != furthestPoint2 && furthestPoint4 != furthestPoint3)
					{
						list.Add(furthestPoint4);
					}
				}
			}
			int i = 1;
			for (int count = list.Count; i < count; i++)
			{
				if (list[i].X < list[0].X || (list[i].X == list[0].X && list[i].Y < list[0].Y))
				{
					IntPoint value = list[i];
					list[i] = list[0];
					list[0] = value;
				}
			}
			float num2 = (list[1].X != list[0].X) ? ((float)(list[1].Y - list[0].Y) / (float)(list[1].X - list[0].X)) : ((list[1].Y > list[0].Y) ? float.PositiveInfinity : float.NegativeInfinity);
			float num3 = (list[2].X != list[0].X) ? ((float)(list[2].Y - list[0].Y) / (float)(list[2].X - list[0].X)) : ((list[2].Y > list[0].Y) ? float.PositiveInfinity : float.NegativeInfinity);
			if (num3 < num2)
			{
				IntPoint value2 = list[1];
				list[1] = list[2];
				list[2] = value2;
				float num4 = num2;
				num2 = num3;
				num3 = num4;
			}
			if (list.Count == 4)
			{
				float num5 = (list[3].X != list[0].X) ? ((float)(list[3].Y - list[0].Y) / (float)(list[3].X - list[0].X)) : ((list[3].Y > list[0].Y) ? float.PositiveInfinity : float.NegativeInfinity);
				if (num5 < num2)
				{
					IntPoint value3 = list[1];
					list[1] = list[3];
					list[3] = value3;
					float num6 = num2;
					num2 = num5;
					num5 = num6;
				}
				if (num5 < num3)
				{
					IntPoint value4 = list[2];
					list[2] = list[3];
					list[3] = value4;
					float num7 = num3;
					num3 = num5;
					num5 = num7;
				}
			}
			return list;
		}
	}
}
