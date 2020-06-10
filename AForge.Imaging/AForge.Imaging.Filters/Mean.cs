namespace AForge.Imaging.Filters
{
	public sealed class Mean : Convolution
	{
		public Mean()
			: base(new int[3, 3]
			{
				{
					1,
					1,
					1
				},
				{
					1,
					1,
					1
				},
				{
					1,
					1,
					1
				}
			})
		{
			base.ProcessAlpha = true;
		}
	}
}
