using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class CanvasMove : BaseInPlaceFilter
	{
		private byte fillRed = byte.MaxValue;

		private byte fillGreen = byte.MaxValue;

		private byte fillBlue = byte.MaxValue;

		private byte fillAlpha = byte.MaxValue;

		private byte fillGray = byte.MaxValue;

		private IntPoint movePoint;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public Color FillColorRGB
		{
			get
			{
				return Color.FromArgb(fillAlpha, fillRed, fillGreen, fillBlue);
			}
			set
			{
				fillRed = value.R;
				fillGreen = value.G;
				fillBlue = value.B;
				fillAlpha = value.A;
			}
		}

		public byte FillColorGray
		{
			get
			{
				return fillGray;
			}
			set
			{
				fillGray = value;
			}
		}

		public IntPoint MovePoint
		{
			get
			{
				return movePoint;
			}
			set
			{
				movePoint = value;
			}
		}

		private CanvasMove()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
			formatTranslations[PixelFormat.Format64bppArgb] = PixelFormat.Format64bppArgb;
		}

		public CanvasMove(IntPoint movePoint)
			: this()
		{
			this.movePoint = movePoint;
		}

		public CanvasMove(IntPoint movePoint, Color fillColorRGB)
			: this()
		{
			this.movePoint = movePoint;
			fillRed = fillColorRGB.R;
			fillGreen = fillColorRGB.G;
			fillBlue = fillColorRGB.B;
			fillAlpha = fillColorRGB.A;
		}

		public CanvasMove(IntPoint movePoint, byte fillColorGray)
			: this()
		{
			this.movePoint = movePoint;
			fillGray = fillColorGray;
		}

		public CanvasMove(IntPoint movePoint, Color fillColorRGB, byte fillColorGray)
			: this()
		{
			this.movePoint = movePoint;
			fillRed = fillColorRGB.R;
			fillGreen = fillColorRGB.G;
			fillBlue = fillColorRGB.B;
			fillAlpha = fillColorRGB.A;
			fillGray = fillColorGray;
		}

		protected override void ProcessFilter(UnmanagedImage image)
		{
			switch (System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8)
			{
			case 5:
			case 7:
				break;
			case 1:
			case 3:
			case 4:
				ProcessFilter8bpc(image);
				break;
			case 2:
			case 6:
			case 8:
				ProcessFilter16bpc(image);
				break;
			}
		}

		private unsafe void ProcessFilter8bpc(UnmanagedImage image)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			bool flag = num == 4;
			int width = image.Width;
			int height = image.Height;
			int stride = image.Stride;
			int x = movePoint.X;
			int y = movePoint.Y;
			Rectangle rectangle = Rectangle.Intersect(new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height));
			int num2 = 0;
			int num3 = height;
			int num4 = 1;
			int num5 = 0;
			int num6 = width;
			int num7 = 1;
			if (y > 0)
			{
				num2 = height - 1;
				num3 = -1;
				num4 = -1;
			}
			if (x > 0)
			{
				num5 = width - 1;
				num6 = -1;
				num7 = -1;
			}
			byte* ptr = (byte*)image.ImageData.ToPointer();
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				for (int i = num2; i != num3; i += num4)
				{
					for (int j = num5; j != num6; j += num7)
					{
						byte* ptr2 = ptr + (long)i * (long)stride + j;
						if (rectangle.Contains(j, i))
						{
							byte* ptr3 = ptr + (long)(i - y) * (long)stride + (j - x);
							*ptr2 = *ptr3;
						}
						else
						{
							*ptr2 = fillGray;
						}
					}
				}
				return;
			}
			for (int k = num2; k != num3; k += num4)
			{
				for (int l = num5; l != num6; l += num7)
				{
					byte* ptr2 = ptr + (long)k * (long)stride + (long)l * (long)num;
					if (rectangle.Contains(l, k))
					{
						byte* ptr3 = ptr + (long)(k - y) * (long)stride + (long)(l - x) * (long)num;
						ptr2[2] = ptr3[2];
						ptr2[1] = ptr3[1];
						*ptr2 = *ptr3;
						if (flag)
						{
							ptr2[3] = ptr3[3];
						}
					}
					else
					{
						ptr2[2] = fillRed;
						ptr2[1] = fillGreen;
						*ptr2 = fillBlue;
						if (flag)
						{
							ptr2[3] = fillAlpha;
						}
					}
				}
			}
		}

		private unsafe void ProcessFilter16bpc(UnmanagedImage image)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			bool flag = num == 8;
			ushort num2 = (ushort)(fillRed << 8);
			ushort num3 = (ushort)(fillGreen << 8);
			ushort num4 = (ushort)(fillBlue << 8);
			ushort num5 = (ushort)(fillAlpha << 8);
			int width = image.Width;
			int height = image.Height;
			int stride = image.Stride;
			int x = movePoint.X;
			int y = movePoint.Y;
			Rectangle rectangle = Rectangle.Intersect(new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height));
			int num6 = 0;
			int num7 = height;
			int num8 = 1;
			int num9 = 0;
			int num10 = width;
			int num11 = 1;
			if (y > 0)
			{
				num6 = height - 1;
				num7 = -1;
				num8 = -1;
			}
			if (x > 0)
			{
				num9 = width - 1;
				num10 = -1;
				num11 = -1;
			}
			byte* ptr = (byte*)image.ImageData.ToPointer();
			if (image.PixelFormat == PixelFormat.Format16bppGrayScale)
			{
				for (int i = num6; i != num7; i += num8)
				{
					for (int j = num9; j != num10; j += num11)
					{
						ushort* ptr2 = (ushort*)(ptr + (long)i * (long)stride + (long)j * 2L);
						if (rectangle.Contains(j, i))
						{
							ushort* ptr3 = (ushort*)(ptr + (long)(i - y) * (long)stride + (long)(j - x) * 2L);
							*ptr2 = *ptr3;
						}
						else
						{
							*ptr2 = fillGray;
						}
					}
				}
				return;
			}
			for (int k = num6; k != num7; k += num8)
			{
				for (int l = num9; l != num10; l += num11)
				{
					ushort* ptr2 = (ushort*)(ptr + (long)k * (long)stride + (long)l * (long)num);
					if (rectangle.Contains(l, k))
					{
						ushort* ptr3 = (ushort*)(ptr + (long)(k - y) * (long)stride + (long)(l - x) * (long)num);
						ptr2[2] = ptr3[2];
						ptr2[1] = ptr3[1];
						*ptr2 = *ptr3;
						if (flag)
						{
							ptr2[3] = ptr3[3];
						}
					}
					else
					{
						ptr2[2] = num2;
						ptr2[1] = num3;
						*ptr2 = num4;
						if (flag)
						{
							ptr2[3] = num5;
						}
					}
				}
			}
		}
	}
}
