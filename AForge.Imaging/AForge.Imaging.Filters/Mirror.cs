using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Mirror : BaseInPlacePartialFilter
	{
		private bool mirrorX;

		private bool mirrorY;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public bool MirrorX
		{
			get
			{
				return mirrorX;
			}
			set
			{
				mirrorX = value;
			}
		}

		public bool MirrorY
		{
			get
			{
				return mirrorY;
			}
			set
			{
				mirrorY = value;
			}
		}

		public Mirror(bool mirrorX, bool mirrorY)
		{
			this.mirrorX = mirrorX;
			MirrorY = mirrorY;
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int width = rect.Width;
			int height = rect.Height;
			int top = rect.Top;
			int num2 = top + height;
			int left = rect.Left;
			int num3 = left + width;
			int num4 = left * num;
			int num5 = num3 * num;
			int stride = image.Stride;
			if (mirrorY)
			{
				byte* ptr = (byte*)image.ImageData.ToPointer();
				ptr += top * stride + left * num;
				byte* ptr2 = (byte*)image.ImageData.ToPointer();
				ptr2 += top * stride + (num3 - 1) * num;
				int num6 = stride - (width >> 1) * num;
				int num7 = stride + (width >> 1) * num;
				if (image.PixelFormat == PixelFormat.Format8bppIndexed)
				{
					for (int i = top; i < num2; i++)
					{
						int num8 = left;
						int num9 = left + (width >> 1);
						while (num8 < num9)
						{
							byte b = *ptr;
							*ptr = *ptr2;
							*ptr2 = b;
							num8++;
							ptr++;
							ptr2--;
						}
						ptr += num6;
						ptr2 += num7;
					}
				}
				else
				{
					for (int j = top; j < num2; j++)
					{
						int num10 = left;
						int num11 = left + (width >> 1);
						while (num10 < num11)
						{
							byte b = ptr[2];
							ptr[2] = ptr2[2];
							ptr2[2] = b;
							b = ptr[1];
							ptr[1] = ptr2[1];
							ptr2[1] = b;
							b = *ptr;
							*ptr = *ptr2;
							*ptr2 = b;
							num10++;
							ptr += 3;
							ptr2 -= 3;
						}
						ptr += num6;
						ptr2 += num7;
					}
				}
			}
			if (!mirrorX)
			{
				return;
			}
			int num12 = stride - rect.Width * num;
			byte* ptr3 = (byte*)image.ImageData.ToPointer();
			ptr3 += top * stride + left * num;
			byte* ptr4 = (byte*)image.ImageData.ToPointer();
			ptr4 += (num2 - 1) * stride + left * num;
			int k = top;
			for (int num13 = top + (height >> 1); k < num13; k++)
			{
				int num14 = num4;
				while (num14 < num5)
				{
					byte b2 = *ptr3;
					*ptr3 = *ptr4;
					*ptr4 = b2;
					num14++;
					ptr3++;
					ptr4++;
				}
				ptr3 += num12;
				ptr4 += num12 - stride - stride;
			}
		}
	}
}
