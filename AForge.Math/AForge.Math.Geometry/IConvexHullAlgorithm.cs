using System.Collections.Generic;

namespace AForge.Math.Geometry
{
	public interface IConvexHullAlgorithm
	{
		List<IntPoint> FindHull(List<IntPoint> points);
	}
}
