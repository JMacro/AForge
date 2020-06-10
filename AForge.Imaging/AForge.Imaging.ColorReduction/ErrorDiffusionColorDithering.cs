using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.ColorReduction
{
	public abstract class ErrorDiffusionColorDithering
	{
		private bool useCaching;

		protected int x;

		protected int y;

		protected int width;

		protected int height;

		protected int stride;

		protected int pixelSize;

		private Color[] colorTable = new Color[16]
		{
			Color.Black,
			Color.DarkBlue,
			Color.DarkGreen,
			Color.DarkCyan,
			Color.DarkRed,
			Color.DarkMagenta,
			Color.DarkKhaki,
			Color.LightGray,
			Color.Gray,
			Color.Blue,
			Color.Green,
			Color.Cyan,
			Color.Red,
			Color.Magenta,
			Color.Yellow,
			Color.White
		};

		[NonSerialized]
		private Dictionary<Color, byte> cache = new Dictionary<Color, byte>();

		public Color[] ColorTable
		{
			get
			{
				return colorTable;
			}
			set
			{
				if (colorTable.Length < 2 || colorTable.Length > 256)
				{
					throw new ArgumentException("Color table length must be in the [2, 256] range.");
				}
				colorTable = value;
			}
		}

		public bool UseCaching
		{
			get
			{
				return useCaching;
			}
			set
			{
				useCaching = value;
			}
		}

		protected unsafe abstract void Diffuse(int rError, int gError, int bError, byte* ptr);

		public Bitmap Apply(Bitmap sourceImage)
		{
			BitmapData bitmapData = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, sourceImage.PixelFormat);
			Bitmap bitmap = null;
			try
			{
				bitmap = Apply(new UnmanagedImage(bitmapData));
				if (!(sourceImage.HorizontalResolution > 0f))
				{
					return bitmap;
				}
				if (!(sourceImage.VerticalResolution > 0f))
				{
					return bitmap;
				}
				bitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
				return bitmap;
			}
			finally
			{
				sourceImage.UnlockBits(bitmapData);
			}
		}

		public unsafe Bitmap Apply(UnmanagedImage sourceImage)
		{
			if (sourceImage.PixelFormat != PixelFormat.Format24bppRgb && sourceImage.PixelFormat != PixelFormat.Format32bppRgb && sourceImage.PixelFormat != PixelFormat.Format32bppArgb && sourceImage.PixelFormat != PixelFormat.Format32bppPArgb)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			cache.Clear();
			UnmanagedImage unmanagedImage = sourceImage.Clone();
			width = sourceImage.Width;
			height = sourceImage.Height;
			stride = sourceImage.Stride;
			pixelSize = System.Drawing.Image.GetPixelFormatSize(sourceImage.PixelFormat) / 8;
			int num = stride - width * pixelSize;
			Bitmap bitmap = new Bitmap(width, height, (colorTable.Length > 16) ? PixelFormat.Format8bppIndexed : PixelFormat.Format4bppIndexed);
			ColorPalette palette = bitmap.Palette;
			int i = 0;
			for (int num2 = colorTable.Length; i < num2; i++)
			{
				palette.Entries[i] = colorTable[i];
			}
			bitmap.Palette = palette;
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
			byte* ptr = (byte*)unmanagedImage.ImageData.ToPointer();
			byte* ptr2 = (byte*)bitmapData.Scan0.ToPointer();
			bool flag = colorTable.Length > 16;
			for (y = 0; y < height; y++)
			{
				byte* ptr3 = ptr2 + (long)y * (long)bitmapData.Stride;
				x = 0;
				while (x < width)
				{
					int num3 = ptr[2];
					int num4 = ptr[1];
					int num5 = *ptr;
					byte colorIndex;
					Color closestColor = GetClosestColor(num3, num4, num5, out colorIndex);
					Diffuse(num3 - closestColor.R, num4 - closestColor.G, num5 - closestColor.B, ptr);
					if (flag)
					{
						*ptr3 = colorIndex;
						ptr3++;
					}
					else if (x % 2 == 0)
					{
						byte* intPtr = ptr3;
						*intPtr = (byte)(*intPtr | (byte)(colorIndex << 4));
					}
					else
					{
						byte* intPtr2 = ptr3;
						*intPtr2 = (byte)(*intPtr2 | colorIndex);
						ptr3++;
					}
					x++;
					ptr += pixelSize;
				}
				ptr += num;
			}
			bitmap.UnlockBits(bitmapData);
			unmanagedImage.Dispose();
			return bitmap;
		}

		private Color GetClosestColor(int red, int green, int blue, out byte colorIndex)
		{
			Color key = Color.FromArgb(red, green, blue);
			if (useCaching && cache.ContainsKey(key))
			{
				colorIndex = cache[key];
			}
			else
			{
				colorIndex = 0;
				int num = int.MaxValue;
				int i = 0;
				for (int num2 = colorTable.Length; i < num2; i++)
				{
					int num3 = red - colorTable[i].R;
					int num4 = green - colorTable[i].G;
					int num5 = blue - colorTable[i].B;
					int num6 = num3 * num3 + num4 * num4 + num5 * num5;
					if (num6 < num)
					{
						num = num6;
						colorIndex = (byte)i;
					}
				}
				if (useCaching)
				{
					cache.Add(key, colorIndex);
				}
			}
			return colorTable[colorIndex];
		}
	}
}
