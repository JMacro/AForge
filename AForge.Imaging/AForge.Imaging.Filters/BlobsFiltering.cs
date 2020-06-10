using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class BlobsFiltering : BaseInPlaceFilter
	{
		private BlobCounter blobCounter = new BlobCounter();

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public bool CoupledSizeFiltering
		{
			get
			{
				return blobCounter.CoupledSizeFiltering;
			}
			set
			{
				blobCounter.CoupledSizeFiltering = value;
			}
		}

		public int MinWidth
		{
			get
			{
				return blobCounter.MinWidth;
			}
			set
			{
				blobCounter.MinWidth = value;
			}
		}

		public int MinHeight
		{
			get
			{
				return blobCounter.MinHeight;
			}
			set
			{
				blobCounter.MinHeight = value;
			}
		}

		public int MaxWidth
		{
			get
			{
				return blobCounter.MaxWidth;
			}
			set
			{
				blobCounter.MaxWidth = value;
			}
		}

		public int MaxHeight
		{
			get
			{
				return blobCounter.MaxHeight;
			}
			set
			{
				blobCounter.MaxHeight = value;
			}
		}

		public IBlobsFilter BlobsFilter
		{
			get
			{
				return blobCounter.BlobsFilter;
			}
			set
			{
				blobCounter.BlobsFilter = value;
			}
		}

		public BlobsFiltering()
		{
			blobCounter.FilterBlobs = true;
			blobCounter.MinWidth = 1;
			blobCounter.MinHeight = 1;
			blobCounter.MaxWidth = int.MaxValue;
			blobCounter.MaxHeight = int.MaxValue;
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
		}

		public BlobsFiltering(int minWidth, int minHeight, int maxWidth, int maxHeight)
			: this(minWidth, minHeight, maxWidth, maxHeight, coupledSizeFiltering: false)
		{
		}

		public BlobsFiltering(int minWidth, int minHeight, int maxWidth, int maxHeight, bool coupledSizeFiltering)
			: this()
		{
			blobCounter.MinWidth = minWidth;
			blobCounter.MinHeight = minHeight;
			blobCounter.MaxWidth = maxWidth;
			blobCounter.MaxHeight = maxHeight;
			blobCounter.CoupledSizeFiltering = coupledSizeFiltering;
		}

		public BlobsFiltering(IBlobsFilter blobsFilter)
			: this()
		{
			blobCounter.BlobsFilter = blobsFilter;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image)
		{
			blobCounter.ProcessImage(image);
			int[] objectLabels = blobCounter.ObjectLabels;
			int width = image.Width;
			int height = image.Height;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				int num = image.Stride - width;
				int i = 0;
				int num2 = 0;
				for (; i < height; i++)
				{
					int num3 = 0;
					while (num3 < width)
					{
						if (objectLabels[num2] == 0)
						{
							*ptr = 0;
						}
						num3++;
						ptr++;
						num2++;
					}
					ptr += num;
				}
				return;
			}
			int num4 = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int num5 = image.Stride - width * num4;
			int j = 0;
			int num6 = 0;
			for (; j < height; j++)
			{
				int num7 = 0;
				while (num7 < width)
				{
					if (objectLabels[num6] == 0)
					{
						byte* intPtr = ptr + 2;
						byte b;
						ptr[1] = (b = (*ptr = 0));
						*intPtr = b;
					}
					num7++;
					ptr += num4;
					num6++;
				}
				ptr += num5;
			}
		}
	}
}
