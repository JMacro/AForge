namespace AForge.Imaging.Filters
{
	public sealed class StuckiDithering : ErrorDiffusionToAdjacentNeighbors
	{
		public StuckiDithering()
			: base(new int[3][]
			{
				new int[2]
				{
					8,
					4
				},
				new int[5]
				{
					2,
					4,
					8,
					4,
					2
				},
				new int[5]
				{
					1,
					2,
					4,
					2,
					1
				}
			})
		{
		}
	}
}
