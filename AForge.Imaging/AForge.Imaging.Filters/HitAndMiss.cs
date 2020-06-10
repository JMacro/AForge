using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class HitAndMiss : BaseUsingCopyPartialFilter
	{
		public enum Modes
		{
			HitAndMiss,
			Thinning,
			Thickening
		}

		private short[,] se;

		private int size;

		private Modes mode;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public Modes Mode
		{
			get
			{
				return mode;
			}
			set
			{
				mode = value;
			}
		}

		public HitAndMiss(short[,] se)
		{
			int length = se.GetLength(0);
			if (length != se.GetLength(1) || length < 3 || length > 99 || length % 2 == 0)
			{
				throw new ArgumentException();
			}
			this.se = se;
			size = length;
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
		}

		public HitAndMiss(short[,] se, Modes mode)
			: this(se)
		{
			this.mode = mode;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData, Rectangle rect)
		{
			int left = rect.Left;
			int top = rect.Top;
			int num = left + rect.Width;
			int num2 = top + rect.Height;
			int stride = sourceData.Stride;
			int stride2 = destinationData.Stride;
			int num3 = stride - rect.Width;
			int num4 = stride2 - rect.Width;
			int num5 = size >> 1;
			byte[] array = new byte[3]
			{
				255,
				0,
				255
			};
			byte[] array2 = new byte[3];
			byte[] array3 = array2;
			int num6 = (int)mode;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			ptr += top * stride + left;
			ptr2 += top * stride2 + left;
			for (int i = top; i < num2; i++)
			{
				int num7 = left;
				while (num7 < num)
				{
					byte b;
					array3[2] = (b = *ptr);
					array3[1] = b;
					byte b2 = byte.MaxValue;
					for (int j = 0; j < size; j++)
					{
						int num8 = j - num5;
						for (int k = 0; k < size; k++)
						{
							int num9 = k - num5;
							short num10 = se[j, k];
							if (num10 != -1)
							{
								if (i + num8 < top || i + num8 >= num2 || num7 + num9 < left || num7 + num9 >= num)
								{
									b2 = 0;
									break;
								}
								byte b3 = ptr[num8 * stride + num9];
								if ((num10 != 0 || b3 != 0) && (num10 != 1 || b3 != byte.MaxValue))
								{
									b2 = 0;
									break;
								}
							}
						}
						if (b2 == 0)
						{
							break;
						}
					}
					*ptr2 = ((b2 == byte.MaxValue) ? array[num6] : array3[num6]);
					num7++;
					ptr++;
					ptr2++;
				}
				ptr += num3;
				ptr2 += num4;
			}
		}
	}
}
