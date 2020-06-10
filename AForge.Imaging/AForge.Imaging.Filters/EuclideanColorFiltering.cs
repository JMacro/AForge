using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class EuclideanColorFiltering : BaseInPlacePartialFilter
	{
		private short radius = 100;

		private RGB center = new RGB(byte.MaxValue, byte.MaxValue, byte.MaxValue);

		private RGB fill = new RGB(0, 0, 0);

		private bool fillOutside = true;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public short Radius
		{
			get
			{
				return radius;
			}
			set
			{
				radius = System.Math.Max((short)0, System.Math.Min((short)450, value));
			}
		}

		public RGB CenterColor
		{
			get
			{
				return center;
			}
			set
			{
				center = value;
			}
		}

		public RGB FillColor
		{
			get
			{
				return fill;
			}
			set
			{
				fill = value;
			}
		}

		public bool FillOutside
		{
			get
			{
				return fillOutside;
			}
			set
			{
				fillOutside = value;
			}
		}

		public EuclideanColorFiltering()
		{
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		public EuclideanColorFiltering(RGB center, short radius)
			: this()
		{
			this.center = center;
			this.radius = radius;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = (image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
			int left = rect.Left;
			int top = rect.Top;
			int num2 = left + rect.Width;
			int num3 = top + rect.Height;
			int num4 = image.Stride - rect.Width * num;
			int num5 = radius * radius;
			int red = center.Red;
			int green = center.Green;
			int blue = center.Blue;
			byte red2 = fill.Red;
			byte green2 = fill.Green;
			byte blue2 = fill.Blue;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + left * num;
			for (int i = top; i < num3; i++)
			{
				int num6 = left;
				while (num6 < num2)
				{
					int num7 = red - ptr[2];
					int num8 = green - ptr[1];
					int num9 = blue - *ptr;
					if (num7 * num7 + num8 * num8 + num9 * num9 <= num5)
					{
						if (!fillOutside)
						{
							ptr[2] = red2;
							ptr[1] = green2;
							*ptr = blue2;
						}
					}
					else if (fillOutside)
					{
						ptr[2] = red2;
						ptr[1] = green2;
						*ptr = blue2;
					}
					num6++;
					ptr += num;
				}
				ptr += num4;
			}
		}
	}
}
