namespace AForge.Imaging.Filters
{
	public sealed class Blur : Convolution
	{
		public Blur()
			: base(new int[5, 5]
			{
				{
					1,
					2,
					3,
					2,
					1
				},
				{
					2,
					4,
					5,
					4,
					2
				},
				{
					3,
					5,
					6,
					5,
					3
				},
				{
					2,
					4,
					5,
					4,
					2
				},
				{
					1,
					2,
					3,
					2,
					1
				}
			})
		{
			base.ProcessAlpha = true;
		}
	}
}
