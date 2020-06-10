using System;
using System.Collections.Generic;

namespace AForge.Math.Geometry
{
	public class FlatAnglesOptimizer : IShapeOptimizer
	{
		private float maxAngleToKeep = 160f;

		public float MaxAngleToKeep
		{
			get
			{
				return maxAngleToKeep;
			}
			set
			{
				maxAngleToKeep = System.Math.Min(180f, System.Math.Max(140f, value));
			}
		}

		public FlatAnglesOptimizer()
		{
		}

		public FlatAnglesOptimizer(float maxAngleToKeep)
		{
			this.maxAngleToKeep = maxAngleToKeep;
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
				list.Add(shape[1]);
				int num2 = 2;
				int i = 2;
				for (int count = shape.Count; i < count; i++)
				{
					list.Add(shape[i]);
					num2++;
					num = GeometryTools.GetAngleBetweenVectors(list[num2 - 2], list[num2 - 3], list[num2 - 1]);
					if (num > maxAngleToKeep && (num2 > 3 || i < count - 1))
					{
						list.RemoveAt(num2 - 2);
						num2--;
					}
				}
				if (num2 > 3)
				{
					num = GeometryTools.GetAngleBetweenVectors(list[num2 - 1], list[num2 - 2], list[0]);
					if (num > maxAngleToKeep)
					{
						list.RemoveAt(num2 - 1);
						num2--;
					}
					if (num2 > 3)
					{
						num = GeometryTools.GetAngleBetweenVectors(list[0], list[num2 - 1], list[1]);
						if (num > maxAngleToKeep)
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
