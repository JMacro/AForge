using System.Drawing;

namespace AForge.Imaging
{
	public class RGB
	{
		public const short R = 2;

		public const short G = 1;

		public const short B = 0;

		public const short A = 3;

		public byte Red;

		public byte Green;

		public byte Blue;

		public byte Alpha;

		public Color Color
		{
			get
			{
				return Color.FromArgb(Alpha, Red, Green, Blue);
			}
			set
			{
				Red = value.R;
				Green = value.G;
				Blue = value.B;
				Alpha = value.A;
			}
		}

		public RGB()
		{
			Red = 0;
			Green = 0;
			Blue = 0;
			Alpha = byte.MaxValue;
		}

		public RGB(byte red, byte green, byte blue)
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = byte.MaxValue;
		}

		public RGB(byte red, byte green, byte blue, byte alpha)
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
		}

		public RGB(Color color)
		{
			Red = color.R;
			Green = color.G;
			Blue = color.B;
			Alpha = color.A;
		}
	}
}
