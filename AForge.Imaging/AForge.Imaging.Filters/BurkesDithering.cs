namespace AForge.Imaging.Filters
{
	public sealed class BurkesDithering : ErrorDiffusionToAdjacentNeighbors
	{
		public BurkesDithering()
			: base(new int[2][]
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
				}
			})
		{
		}
	}
}
