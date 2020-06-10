using System;
using System.Collections.Generic;

namespace AForge.Math.Geometry
{
	public class ClosePointsMergingOptimizer : IShapeOptimizer
	{
		private float maxDistanceToMerge = 10f;

		public float MaxDistanceToMerge
		{
			get
			{
				return maxDistanceToMerge;
			}
			set
			{
				maxDistanceToMerge = System.Math.Max(0f, value);
			}
		}

		public ClosePointsMergingOptimizer()
		{
		}

		public ClosePointsMergingOptimizer(float maxDistanceToMerge)
		{
			this.maxDistanceToMerge = maxDistanceToMerge;
		}

		public List<IntPoint> OptimizeShape(List<IntPoint> shape)
		{
			List<IntPoint> list = new List<IntPoint>();
			if (shape.Count <= 3)
			{
				list.AddRange(shape);
			}
			else
			{
				float num = 0f;
				list.Add(shape[0]);
				int num2 = 1;
				int i = 1;
				for (int count = shape.Count; i < count; i++)
				{
					num = list[num2 - 1].DistanceTo(shape[i]);
					if (num <= maxDistanceToMerge && num2 + (count - i) > 3)
					{
						list[num2 - 1] = (list[num2 - 1] + shape[i]) / 2;
						continue;
					}
					list.Add(shape[i]);
					num2++;
				}
				if (num2 > 3)
				{
					num = list[num2 - 1].DistanceTo(list[0]);
					if (num <= maxDistanceToMerge)
					{
						list[0] = (list[num2 - 1] + list[0]) / 2;
						list.RemoveAt(num2 - 1);
					}
				}
			}
			return list;
		}
	}
}
