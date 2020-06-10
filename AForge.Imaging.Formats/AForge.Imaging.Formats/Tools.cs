using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AForge.Imaging.Formats
{
	internal class Tools
	{
		public static Bitmap CreateGrayscaleImage(int width, int height)
		{
			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
			ColorPalette palette = bitmap.Palette;
			for (int i = 0; i < 256; i++)
			{
				palette.Entries[i] = Color.FromArgb(i, i, i);
			}
			bitmap.Palette = palette;
			return bitmap;
		}

		public static int ReadStream(Stream stream, byte[] buffer, int offset, int count)
		{
			int i = 0;
			int num = 0;
			for (; i != count; i += num)
			{
				num = stream.Read(buffer, offset + i, count - i);
				if (num == 0)
				{
					break;
				}
			}
			return i;
		}
	}
}
