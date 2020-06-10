using System.Collections.Generic;

namespace AForge.Math.Geometry
{
	public interface IShapeOptimizer
	{
		List<IntPoint> OptimizeShape(List<IntPoint> shape);
	}
}
