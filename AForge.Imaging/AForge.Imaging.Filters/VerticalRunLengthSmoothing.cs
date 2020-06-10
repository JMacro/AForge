using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class VerticalRunLengthSmoothing : BaseInPlacePartialFilter
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

		public VerticalRunLengthSmoothing()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
		}

		public VerticalRunLengthSmoothing(int maxGapSize)
			: this()
		{
			MaxGapSize = maxGapSize;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int left = rect.Left;
			int num = left + rect.Width;
			int height = rect.Height;
			int stride = image.Stride;
			byte* ptr = (byte*)image.ImageData.ToPointer() + (long)rect.Top * (long)stride + left;
			for (int i = left; i < num; i++)
			{
				byte* ptr2 = ptr;
				byte* ptr3 = ptr2;
				byte* ptr4 = ptr2 + (long)stride * (long)height;
				while (ptr2 < ptr4)
				{
					byte* ptr5 = ptr2;
					int num2 = 0;
					while (ptr2 < ptr4 && *ptr2 == 0)
					{
						ptr2 += stride;
						num2++;
					}
					if (num2 <= maxGapSize && (processGapsWithImageBorders || (ptr5 != ptr3 && ptr2 != ptr4)))
					{
						for (; ptr5 < ptr2; ptr5 += stride)
						{
							*ptr5 = byte.MaxValue;
						}
					}
					for (; ptr2 < ptr4 && *ptr2 != 0; ptr2 += stride)
					{
					}
				}
				ptr++;
			}
		}
	}
}
