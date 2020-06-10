namespace AForge.Imaging.Filters
{
	public sealed class Sharpen : Convolution
	{
		public Sharpen()
			: base(new int[3, 3]
			{
				{
					0,
					-1,
					0
				},
				{
					-1,
					5,
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
