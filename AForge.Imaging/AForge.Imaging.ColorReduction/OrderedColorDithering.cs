using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.ColorReduction
{
	public class OrderedColorDithering
	{
		private bool useCaching;

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

		private byte[,] matrix = new byte[4, 4]
		{
			{
				2,
				18,
				6,
				22
			},
			{
				26,
				10,
				30,
				14
			},
			{
				8,
				24,
				4,
				20
			},
			{
				32,
				16,
				28,
				12
			}
		};

		[NonSerialized]
		private Dictionary<Color, byte> cache = new Dictionary<Color, byte>();

		public byte[,] ThresholdMatrix
		{
			get
			{
				return (byte[,])matrix.Clone();
			}
			set
			{
				if (value == null)
				{
					throw new NullReferenceException("Threshold matrix cannot be set to null.");
				}
				matrix = value;
			}
		}

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

		public OrderedColorDithering()
		{
		}

		public OrderedColorDithering(byte[,] matrix)
		{
			ThresholdMatrix = matrix;
		}

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
			int width = sourceImage.Width;
			int height = sourceImage.Height;
			int stride = sourceImage.Stride;
			int num = System.Drawing.Image.GetPixelFormatSize(sourceImage.PixelFormat) / 8;
			Bitmap bitmap = new Bitmap(width, height, (colorTable.Length > 16) ? PixelFormat.Format8bppIndexed : PixelFormat.Format4bppIndexed);
			ColorPalette palette = bitmap.Palette;
			int i = 0;
			for (int num2 = colorTable.Length; i < num2; i++)
			{
				palette.Entries[i] = colorTable[i];
			}
			bitmap.Palette = palette;
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
			int length = matrix.GetLength(0);
			int length2 = matrix.GetLength(1);
			byte* ptr = (byte*)sourceImage.ImageData.ToPointer();
			byte* ptr2 = (byte*)bitmapData.Scan0.ToPointer();
			bool flag = colorTable.Length > 16;
			for (int j = 0; j < height; j++)
			{
				byte* ptr3 = ptr2 + (long)j * (long)bitmapData.Stride;
				int num3 = 0;
				while (num3 < width)
				{
					int num4 = matrix[j % length, num3 % length2];
					int num5 = ptr[2] + num4;
					int num6 = ptr[1] + num4;
					int num7 = *ptr + num4;
					if (num5 > 255)
					{
						num5 = 255;
					}
					if (num6 > 255)
					{
						num6 = 255;
					}
					if (num7 > 255)
					{
						num7 = 255;
					}
					GetClosestColor(num5, num6, num7, out byte colorIndex);
					if (flag)
					{
						*ptr3 = colorIndex;
						ptr3++;
					}
					else if (num3 % 2 == 0)
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
					num3++;
					ptr += num;
				}
			}
			bitmap.UnlockBits(bitmapData);
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
