using System.Drawing;

namespace AForge.Imaging.ColorReduction
{
	public interface IColorQuantizer
	{
		void AddColor(Color color);

		Color[] GetPalette(int colorCount);

		void Clear();
	}
}
