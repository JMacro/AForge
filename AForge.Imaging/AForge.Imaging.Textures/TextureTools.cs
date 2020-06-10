using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Textures
{
	public class TextureTools
	{
		private TextureTools()
		{
		}

		public unsafe static Bitmap ToBitmap(float[,] texture)
		{
			int length = texture.GetLength(1);
			int length2 = texture.GetLength(0);
			Bitmap bitmap = Image.CreateGrayscaleImage(length, length2);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, length, length2), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
			byte* ptr = (byte*)bitmapData.Scan0.ToPointer();
			int num = bitmapData.Stride - length;
			for (int i = 0; i < length2; i++)
			{
				int num2 = 0;
				while (num2 < length)
				{
					*ptr = (byte)(texture[i, num2] * 255f);
					num2++;
					ptr++;
				}
				ptr += num;
			}
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		public static float[,] FromBitmap(Bitmap image)
		{
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			float[,] result = FromBitmap(bitmapData);
			image.UnlockBits(bitmapData);
			return result;
		}

		public static float[,] FromBitmap(BitmapData imageData)
		{
			return FromBitmap(new UnmanagedImage(imageData));
		}

		public unsafe static float[,] FromBitmap(UnmanagedImage image)
		{
			if (image.PixelFormat != PixelFormat.Format8bppIndexed)
			{
				throw new UnsupportedImageFormatException("Only grayscale (8 bpp indexed images) are supported.");
			}
			int width = image.Width;
			int height = image.Height;
			float[,] array = new float[height, width];
			byte* ptr = (byte*)image.ImageData.ToPointer();
			int num = image.Stride - width;
			for (int i = 0; i < height; i++)
			{
				int num2 = 0;
				while (num2 < width)
				{
					array[i, num2] = (float)(int)(*ptr) / 255f;
					num2++;
					ptr++;
				}
				ptr += num;
			}
			return array;
		}
	}
}
