namespace AForge.Imaging.ColorReduction
{
	public sealed class JarvisJudiceNinkeColorDithering : ColorErrorDiffusionToAdjacentNeighbors
	{
		public JarvisJudiceNinkeColorDithering()
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
