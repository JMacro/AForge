using System;

namespace AForge.Imaging.Filters
{
	[Obsolete("Use Grayscale.CommonAlgorithms.RMY object instead")]
	public sealed class GrayscaleRMY : Grayscale
	{
		public GrayscaleRMY()
			: base(0.5, 0.419, 0.081)
		{
		}
	}
}
