using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.ColorReduction
{
	public class ColorImageQuantizer
	{
		private IColorQuantizer quantizer;

		private bool useCaching;

		[NonSerialized]
		private Color[] paletteToUse;

		[NonSerialized]
		private Dictionary<Color, int> cache = new Dictionary<Color, int>();

		public IColorQuantizer Quantizer
		{
			get
			{
				return quantizer;
			}
			set
			{
				quantizer = value;
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

		public ColorImageQuantizer(IColorQuantizer quantizer)
		{
			this.quantizer = quantizer;
		}

		public Color[] CalculatePalette(Bitmap image, int paletteSize)
		{
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			try
			{
				return CalculatePalette(new UnmanagedImage(bitmapData), paletteSize);
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public unsafe Color[] CalculatePalette(UnmanagedImage image, int paletteSize)
		{
			if (image.PixelFormat != PixelFormat.Format24bppRgb && image.PixelFormat != PixelFormat.Format32bppRgb && image.PixelFormat != PixelFormat.Format32bppArgb && image.PixelFormat != PixelFormat.Format32bppPArgb)
			{
				throw new UnsupportedImageFormatException("Unsupported format of the source image.");
			}
			quantizer.Clear();
			int width = image.Width;
			int height = image.Height;
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			int num2 = image.Stride - width * num;
			for (int i = 0; i < height; i++)
			{
				int num3 = 0;
				while (num3 < width)
				{
					quantizer.AddColor(Color.FromArgb(ptr[2], ptr[1], *ptr));
					num3++;
					ptr += num;
				}
				ptr += num2;
			}
			return quantizer.GetPalette(paletteSize);
		}

		public Bitmap ReduceColors(Bitmap image, int paletteSize)
		{
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			try
			{
				Bitmap bitmap = ReduceColors(new UnmanagedImage(bitmapData), paletteSize);
				if (image.HorizontalResolution > 0f && image.VerticalResolution > 0f)
				{
					bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
				}
				return bitmap;
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public Bitmap ReduceColors(UnmanagedImage image, int paletteSize)
		{
			if (paletteSize < 2 || paletteSize > 256)
			{
				throw new ArgumentException("Invalid size of the target color palette.");
			}
			return ReduceColors(image, CalculatePalette(image, paletteSize));
		}

		public Bitmap ReduceColors(Bitmap image, Color[] palette)
		{
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			try
			{
				Bitmap bitmap = ReduceColors(new UnmanagedImage(bitmapData), palette);
				if (image.HorizontalResolution > 0f && image.VerticalResolution > 0f)
				{
					bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
				}
				return bitmap;
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public unsafe Bitmap ReduceColors(UnmanagedImage image, Color[] palette)
		{
			if (image.PixelFormat != PixelFormat.Format24bppRgb && image.PixelFormat != PixelFormat.Format32bppRgb && image.PixelFormat != PixelFormat.Format32bppArgb && image.PixelFormat != PixelFormat.Format32bppPArgb)
			{
				throw new UnsupportedImageFormatException("Unsupported format of the source image.");
			}
			if (palette.Length < 2 || palette.Length > 256)
			{
				throw new ArgumentException("Invalid size of the target color palette.");
			}
			paletteToUse = palette;
			cache.Clear();
			int width = image.Width;
			int height = image.Height;
			int stride = image.Stride;
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int num2 = stride - width * num;
			Bitmap bitmap = new Bitmap(width, height, (palette.Length > 16) ? PixelFormat.Format8bppIndexed : PixelFormat.Format4bppIndexed);
			ColorPalette palette2 = bitmap.Palette;
			int i = 0;
			for (int num3 = palette.Length; i < num3; i++)
			{
				palette2.Entries[i] = palette[i];
			}
			bitmap.Palette = palette2;
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
			byte* ptr = (byte*)image.ImageData.ToPointer();
			byte* ptr2 = (byte*)bitmapData.Scan0.ToPointer();
			bool flag = palette.Length > 16;
			for (int j = 0; j < height; j++)
			{
				byte* ptr3 = ptr2 + (long)j * (long)bitmapData.Stride;
				int num4 = 0;
				while (num4 < width)
				{
					byte b = (byte)GetClosestColor(ptr[2], ptr[1], *ptr);
					if (flag)
					{
						*ptr3 = b;
						ptr3++;
					}
					else if (num4 % 2 == 0)
					{
						byte* intPtr = ptr3;
						*intPtr = (byte)(*intPtr | (byte)(b << 4));
					}
					else
					{
						byte* intPtr2 = ptr3;
						*intPtr2 = (byte)(*intPtr2 | b);
						ptr3++;
					}
					num4++;
					ptr += num;
				}
				ptr += num2;
			}
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		private int GetClosestColor(int red, int green, int blue)
		{
			Color key = Color.FromArgb(red, green, blue);
			if (useCaching && cache.ContainsKey(key))
			{
				return cache[key];
			}
			int num = 0;
			int num2 = int.MaxValue;
			int i = 0;
			for (int num3 = paletteToUse.Length; i < num3; i++)
			{
				int num4 = red - paletteToUse[i].R;
				int num5 = green - paletteToUse[i].G;
				int num6 = blue - paletteToUse[i].B;
				int num7 = num4 * num4 + num5 * num5 + num6 * num6;
				if (num7 < num2)
				{
					num2 = num7;
					num = (byte)i;
				}
			}
			if (useCaching)
			{
				cache.Add(key, num);
			}
			return num;
		}
	}
}
