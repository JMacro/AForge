namespace AForge.Imaging.Filters
{
	public sealed class Edges : Convolution
	{
		public Edges()
			: base(new int[3, 3]
			{
				{
					0,
					-1,
					0
				},
				{
					-1,
					4,
					-1
				},
				{
					0,
					-1,
					0
				}
			})
		{
		}
	}
}
