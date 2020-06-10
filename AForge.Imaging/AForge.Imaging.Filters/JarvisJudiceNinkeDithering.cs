namespace AForge.Imaging.Filters
{
	public sealed class JarvisJudiceNinkeDithering : ErrorDiffusionToAdjacentNeighbors
	{
		public JarvisJudiceNinkeDithering()
			: base(new int[3][]
			{
				new int[2]
				{
					7,
					5
				},
				new int[5]
				{
					3,
					5,
					7,
					5,
					3
				},
				new int[5]
				{
					1,
					3,
					5,
					3,
					1
				}
			})
		{
		}
	}
}
