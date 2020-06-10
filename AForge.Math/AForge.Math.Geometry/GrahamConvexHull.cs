using System;
using System.Collections.Generic;

namespace AForge.Math.Geometry
{
	public class GrahamConvexHull : IConvexHullAlgorithm
	{
		private class PointToProcess : IComparable
		{
			public int X;

			public int Y;

			public float K;

			public float Distance;

			public PointToProcess(IntPoint point)
			{
				X = point.X;
				Y = point.Y;
				K = 0f;
				Distance = 0f;
			}

			public int CompareTo(object obj)
			{
				PointToProcess pointToProcess = (PointToProcess)obj;
				if (!(K < pointToProcess.K))
				{
					if (!(K > pointToProcess.K))
					{
						if (!(Distance > pointToProcess.Distance))
						{
							if (!(Distance < pointToProcess.Distance))
							{
								return 0;
							}
							return 1;
						}
						return -1;
					}
					return 1;
				}
				return -1;
			}

			public IntPoint ToPoint()
			{
				return new IntPoint(X, Y);
			}
		}

		public List<IntPoint> FindHull(List<IntPoint> points)
		{
			if (points.Count <= 3)
			{
				return new List<IntPoint>(points);
			}
			List<PointToProcess> list = new List<PointToProcess>();
			foreach (IntPoint point in points)
			{
				list.Add(new PointToProcess(point));
			}
			int index = 0;
			PointToProcess pointToProcess = list[0];
			int i = 1;
			for (int count = list.Count; i < count; i++)
			{
				if (list[i].X < pointToProcess.X || (list[i].X == pointToProcess.X && list[i].Y < pointToProcess.Y))
				{
					pointToProcess = list[i];
					index = i;
				}
			}
			list.RemoveAt(index);
			int j = 0;
			for (int count2 = list.Count; j < count2; j++)
			{
				int num = list[j].X - pointToProcess.X;
				int num2 = list[j].Y - pointToProcess.Y;
				list[j].Distance = num * num + num2 * num2;
				list[j].K = ((num == 0) ? float.PositiveInfinity : ((float)num2 / (float)num));
			}
			list.Sort();
			List<PointToProcess> list2 = new List<PointToProcess>();
			list2.Add(pointToProcess);
			list2.Add(list[0]);
			list.RemoveAt(0);
			PointToProcess pointToProcess2 = list2[1];
			PointToProcess pointToProcess3 = list2[0];
			while (list.Count != 0)
			{
				PointToProcess pointToProcess4 = list[0];
				if (pointToProcess4.K == pointToProcess2.K || pointToProcess4.Distance == 0f)
				{
					list.RemoveAt(0);
				}
				else if ((pointToProcess4.X - pointToProcess3.X) * (pointToProcess2.Y - pointToProcess4.Y) - (pointToProcess2.X - pointToProcess4.X) * (pointToProcess4.Y - pointToProcess3.Y) < 0)
				{
					list2.Add(pointToProcess4);
					list.RemoveAt(0);
					pointToProcess3 = pointToProcess2;
					pointToProcess2 = pointToProcess4;
				}
				else
				{
					list2.RemoveAt(list2.Count - 1);
					pointToProcess2 = pointToProcess3;
					pointToProcess3 = list2[list2.Count - 2];
				}
			}
			List<IntPoint> list3 = new List<IntPoint>();
			foreach (PointToProcess item in list2)
			{
				list3.Add(item.ToPoint());
			}
			return list3;
		}
	}
}
