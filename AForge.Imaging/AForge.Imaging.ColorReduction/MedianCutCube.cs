using System.Collections.Generic;
using System.Drawing;

namespace AForge.Imaging.ColorReduction
{
	internal class MedianCutCube
	{
		private class RedComparer : IComparer<Color>
		{
			public int Compare(Color c1, Color c2)
			{
				return c1.R.CompareTo(c2.R);
			}
		}

		private class GreenComparer : IComparer<Color>
		{
			public int Compare(Color c1, Color c2)
			{
				return c1.G.CompareTo(c2.G);
			}
		}

		private class BlueComparer : IComparer<Color>
		{
			public int Compare(Color c1, Color c2)
			{
				return c1.B.CompareTo(c2.B);
			}
		}

		private List<Color> colors;

		private readonly byte minR;

		private readonly byte maxR;

		private readonly byte minG;

		private readonly byte maxG;

		private readonly byte minB;

		private readonly byte maxB;

		private Color? cubeColor = null;

		public int RedSize => maxR - minR;

		public int GreenSize => maxG - minG;

		public int BlueSize => maxB - minB;

		public Color Color
		{
			get
			{
				if (!cubeColor.HasValue)
				{
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					foreach (Color color in colors)
					{
						num += color.R;
						num2 += color.G;
						num3 += color.B;
					}
					int count = colors.Count;
					if (count != 0)
					{
						num /= count;
						num2 /= count;
						num3 /= count;
					}
					cubeColor = Color.FromArgb(num, num2, num3);
				}
				return cubeColor.Value;
			}
		}

		public MedianCutCube(List<Color> colors)
		{
			this.colors = colors;
			minR = (minG = (minB = byte.MaxValue));
			maxR = (maxG = (maxB = 0));
			foreach (Color color in colors)
			{
				if (color.R < minR)
				{
					minR = color.R;
				}
				if (color.R > maxR)
				{
					maxR = color.R;
				}
				if (color.G < minG)
				{
					minG = color.G;
				}
				if (color.G > maxG)
				{
					maxG = color.G;
				}
				if (color.B < minB)
				{
					minB = color.B;
				}
				if (color.B > maxB)
				{
					maxB = color.B;
				}
			}
		}

		public void SplitAtMedian(int rgbComponent, out MedianCutCube cube1, out MedianCutCube cube2)
		{
			switch (rgbComponent)
			{
			case 2:
				colors.Sort(new RedComparer());
				break;
			case 1:
				colors.Sort(new GreenComparer());
				break;
			case 0:
				colors.Sort(new BlueComparer());
				break;
			}
			int num = colors.Count / 2;
			cube1 = new MedianCutCube(colors.GetRange(0, num));
			cube2 = new MedianCutCube(colors.GetRange(num, colors.Count - num));
		}
	}
}
