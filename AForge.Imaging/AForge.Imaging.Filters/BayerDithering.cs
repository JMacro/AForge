namespace AForge.Imaging.Filters
{
	public sealed class BayerDithering : OrderedDithering
	{
		public BayerDithering()
			: base(new byte[4, 4]
			{
				{
					0,
					192,
					48,
					240
				},
				{
					128,
					64,
					176,
					112
				},
				{
					32,
					224,
					16,
					208
				},
				{
					160,
					96,
					144,
					80
				}
			})
		{
		}
	}
}
