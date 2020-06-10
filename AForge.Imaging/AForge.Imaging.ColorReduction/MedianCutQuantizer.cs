using System.Collections.Generic;
using System.Drawing;

namespace AForge.Imaging.ColorReduction
{
	public class MedianCutQuantizer : IColorQuantizer
	{
		private List<Color> colors = new List<Color>();

		public void AddColor(Color color)
		{
			colors.Add(color);
		}

		public Color[] GetPalette(int colorCount)
		{
			List<MedianCutCube> list = new List<MedianCutCube>();
			list.Add(new MedianCutCube(colors));
			SplitCubes(list, colorCount);
			Color[] array = new Color[colorCount];
			for (int i = 0; i < colorCount; i++)
			{
				array[i] = list[i].Color;
			}
			return array;
		}

		public void Clear()
		{
			colors.Clear();
		}

		private void SplitCubes(List<MedianCutCube> cubes, int count)
		{
			int num = cubes.Count - 1;
			while (cubes.Count < count)
			{
				MedianCutCube medianCutCube = cubes[num];
				MedianCutCube cube;
				MedianCutCube cube2;
				if (medianCutCube.RedSize >= medianCutCube.GreenSize && medianCutCube.RedSize >= medianCutCube.BlueSize)
				{
					medianCutCube.SplitAtMedian(2, out cube, out cube2);
				}
				else if (medianCutCube.GreenSize >= medianCutCube.BlueSize)
				{
					medianCutCube.SplitAtMedian(1, out cube, out cube2);
				}
				else
				{
					medianCutCube.SplitAtMedian(0, out cube, out cube2);
				}
				cubes.RemoveAt(num);
				cubes.Insert(num, cube);
				cubes.Insert(num, cube2);
				if (--num < 0)
				{
					num = cubes.Count - 1;
				}
			}
		}
	}
}
