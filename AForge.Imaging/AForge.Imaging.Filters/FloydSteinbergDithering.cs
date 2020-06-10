namespace AForge.Imaging.Filters
{
	public sealed class FloydSteinbergDithering : ErrorDiffusionToAdjacentNeighbors
	{
		public FloydSteinbergDithering()
			: base(new int[2][]
			{
				new int[1]
				{
					7
				},
				new int[3]
				{
					3,
					5,
					1
				}
			})
		{
		}
	}
}
