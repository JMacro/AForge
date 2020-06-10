using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class OrderedDithering : BaseInPlacePartialFilter
	{
		private int rows = 4;

		private int cols = 4;

		private byte[,] matrix = new byte[4, 4]
		{
			{
				15,
				143,
				47,
				175
			},
			{
				207,
				79,
				239,
				111
			},
			{
				63,
				191,
				31,
				159
			},
			{
				255,
				127,
				223,
				95
			}
		};

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public OrderedDithering()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
		}

		public OrderedDithering(byte[,] matrix)
			: this()
		{
			rows = matrix.GetLength(0);
			cols = matrix.GetLength(1);
			this.matrix = matrix;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int left = rect.Left;
			int top = rect.Top;
			int num = left + rect.Width;
			int num2 = top + rect.Height;
			int num3 = image.Stride - rect.Width;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + left;
			for (int i = top; i < num2; i++)
			{
				int num4 = left;
				while (num4 < num)
				{
					*ptr = (byte)((*ptr > matrix[i % rows, num4 % cols]) ? 255 : 0);
					num4++;
					ptr++;
				}
				ptr += num3;
			}
		}
	}
}
