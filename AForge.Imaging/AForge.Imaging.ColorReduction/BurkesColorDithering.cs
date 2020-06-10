namespace AForge.Imaging.ColorReduction
{
	public sealed class BurkesColorDithering : ColorErrorDiffusionToAdjacentNeighbors
	{
		public BurkesColorDithering()
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
