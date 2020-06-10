namespace AForge.Imaging.ColorReduction
{
	public sealed class SierraColorDithering : ColorErrorDiffusionToAdjacentNeighbors
	{
		public SierraColorDithering()
			: base(new int[3][]
			{
				new int[2]
				{
					5,
					3
				},
				new int[5]
				{
					2,
					4,
					5,
					4,
					2
				},
				new int[3]
				{
					2,
					3,
					2
				}
			})
		{
		}
	}
}
