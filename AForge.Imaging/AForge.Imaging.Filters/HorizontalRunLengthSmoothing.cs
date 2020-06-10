using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class HorizontalRunLengthSmoothing : BaseInPlacePartialFilter
	{
		private int maxGapSize = 10;

		private bool processGapsWithImageBorders;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public int MaxGapSize
		{
			get
			{
				return maxGapSize;
			}
			set
			{
				maxGapSize = System.Math.Max(1, System.Math.Min(1000, value));
			}
		}

		public bool ProcessGapsWithImageBorders
		{
			get
			{
				return processGapsWithImageBorders;
			}
			set
			{
				processGapsWithImageBorders = value;
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public HorizontalRunLengthSmoothing()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
		}

		public HorizontalRunLengthSmoothing(int maxGapSize)
			: this()
		{
			MaxGapSize = maxGapSize;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int top = rect.Top;
			int num = top + rect.Height;
			int width = rect.Width;
			int num2 = image.Stride - rect.Width;
			byte* ptr = (byte*)image.ImageData.ToPointer() + (long)top * (long)image.Stride + rect.Left;
			for (int i = top; i < num; i++)
			{
				byte* ptr2 = ptr;
				byte* ptr3 = ptr + width;
				while (ptr < ptr3)
				{
					byte* ptr4 = ptr;
					for (; ptr < ptr3 && *ptr == 0; ptr++)
					{
					}
					if (ptr - ptr4 <= maxGapSize && (processGapsWithImageBorders || (ptr4 != ptr2 && ptr != ptr3)))
					{
						for (; ptr4 < ptr; ptr4++)
						{
							*ptr4 = byte.MaxValue;
						}
					}
					for (; ptr < ptr3 && *ptr != 0; ptr++)
					{
					}
				}
				ptr += num2;
			}
		}
	}
}
