using System;
using System.Collections.Generic;

namespace AForge.Math.Geometry
{
	public class LineStraighteningOptimizer : IShapeOptimizer
	{
		private float maxDistanceToRemove = 5f;

		public float MaxDistanceToRemove
		{
			get
			{
				return maxDistanceToRemove;
			}
			set
			{
				maxDistanceToRemove = System.Math.Max(0f, value);
			}
		}

		public LineStraighteningOptimizer()
		{
		}

		public LineStraighteningOptimizer(float maxDistanceToRemove)
		{
			this.maxDistanceToRemove = maxDistanceToRemove;
		}

		public List<IntPoint> OptimizeShape(List<IntPoint> shape)
		{
			List<IntPoint> list = new List<IntPoint>();
			List<IntPoint> list2 = new List<IntPoint>();
			if (shape.Count <= 3)
			{
				list.AddRange(shape);
			}
			else
			{
				float distance = 0f;
				list.Add(shape[0]);
				list.Add(shape[1]);
				int num = 2;
				int i = 2;
				for (int count = shape.Count; i < count; i++)
				{
					list.Add(shape[i]);
					num++;
					list2.Add(list[num - 2]);
					PointsCloud.GetFurthestPointFromLine(list2, list[num - 3], list[num - 1], out distance);
					if (distance <= maxDistanceToRemove && (num > 3 || i < count - 1))
					{
						list.RemoveAt(num - 2);
						num--;
					}
					else
					{
						list2.Clear();
					}
				}
				if (num > 3)
				{
					list2.Add(list[num - 1]);
					PointsCloud.GetFurthestPointFromLine(list2, list[num - 2], list[0], out distance);
					if (distance <= maxDistanceToRemove)
					{
						list.RemoveAt(num - 1);
						num--;
					}
					else
					{
						list2.Clear();
					}
					if (num > 3)
					{
						list2.Add(list[0]);
						PointsCloud.GetFurthestPointFromLine(list2, list[num - 1], list[1], out distance);
						if (distance <= maxDistanceToRemove)
						{
							list.RemoveAt(0);
						}
					}
				}
			}
			return list;
		}
	}
}
