using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ColorRemapping : BaseInPlacePartialFilter
	{
		private byte[] redMap;

		private byte[] greenMap;

		private byte[] blueMap;

		private byte[] grayMap;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public byte[] RedMap
		{
			get
			{
				return redMap;
			}
			set
			{
				if (value == null || value.Length != 256)
				{
					throw new ArgumentException("A map should be array with 256 value.");
				}
				redMap = value;
			}
		}

		public byte[] GreenMap
		{
			get
			{
				return greenMap;
			}
			set
			{
				if (value == null || value.Length != 256)
				{
					throw new ArgumentException("A map should be array with 256 value.");
				}
				greenMap = value;
			}
		}

		public byte[] BlueMap
		{
			get
			{
				return blueMap;
			}
			set
			{
				if (value == null || value.Length != 256)
				{
					throw new ArgumentException("A map should be array with 256 value.");
				}
				blueMap = value;
			}
		}

		public byte[] GrayMap
		{
			get
			{
				return grayMap;
			}
			set
			{
				if (value == null || value.Length != 256)
				{
					throw new ArgumentException("A map should be array with 256 value.");
				}
				grayMap = value;
			}
		}

		public ColorRemapping()
		{
			redMap = new byte[256];
			greenMap = new byte[256];
			blueMap = new byte[256];
			grayMap = new byte[256];
			for (int i = 0; i < 256; i++)
			{
				byte[] array = redMap;
				int num = i;
				byte[] array2 = greenMap;
				int num2 = i;
				byte[] array3 = blueMap;
				int num3 = i;
				byte b;
				grayMap[i] = (b = (byte)i);
				byte b2;
				array3[num3] = (b2 = b);
				byte b3;
				array2[num2] = (b3 = b2);
				array[num] = b3;
			}
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		public ColorRemapping(byte[] redMap, byte[] greenMap, byte[] blueMap)
			: this()
		{
			RedMap = redMap;
			GreenMap = greenMap;
			BlueMap = blueMap;
		}

		public ColorRemapping(byte[] grayMap)
			: this()
		{
			GrayMap = grayMap;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int left = rect.Left;
			int top = rect.Top;
			int num2 = left + rect.Width;
			int num3 = top + rect.Height;
			int num4 = image.Stride - rect.Width * num;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + left * num;
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				for (int i = top; i < num3; i++)
				{
					int num5 = left;
					while (num5 < num2)
					{
						*ptr = grayMap[*ptr];
						num5++;
						ptr++;
					}
					ptr += num4;
				}
				return;
			}
			for (int j = top; j < num3; j++)
			{
				int num6 = left;
				while (num6 < num2)
				{
					ptr[2] = redMap[ptr[2]];
					ptr[1] = greenMap[ptr[1]];
					*ptr = blueMap[*ptr];
					num6++;
					ptr += num;
				}
				ptr += num4;
			}
		}
	}
}
