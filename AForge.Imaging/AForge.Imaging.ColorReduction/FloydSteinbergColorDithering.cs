namespace AForge.Imaging.ColorReduction
{
	public sealed class FloydSteinbergColorDithering : ColorErrorDiffusionToAdjacentNeighbors
	{
		public FloydSteinbergColorDithering()
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
