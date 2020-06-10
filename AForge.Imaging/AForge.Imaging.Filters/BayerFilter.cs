using System.Collections.Generic;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class BayerFilter : BaseFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		private bool performDemosaicing = true;

		private int[,] bayerPattern = new int[2, 2]
		{
			{
				1,
				2
			},
			{
				0,
				1
			}
		};

		public bool PerformDemosaicing
		{
			get
			{
				return performDemosaicing;
			}
			set
			{
				performDemosaicing = value;
			}
		}

		public int[,] BayerPattern
		{
			get
			{
				return bayerPattern;
			}
			set
			{
				bayerPattern = value;
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public BayerFilter()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format24bppRgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			int num = width - 1;
			int num2 = height - 1;
			int stride = sourceData.Stride;
			int num3 = stride - width;
			int num4 = destinationData.Stride - width * 3;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			int[] array = new int[3];
			int[] array2 = new int[3];
			if (!performDemosaicing)
			{
				for (int i = 0; i < height; i++)
				{
					int num5 = 0;
					while (num5 < width)
					{
						byte* intPtr = ptr2 + 2;
						byte b;
						ptr2[1] = (b = (*ptr2 = 0));
						*intPtr = b;
						ptr2[bayerPattern[i & 1, num5 & 1]] = *ptr;
						num5++;
						ptr++;
						ptr2 += 3;
					}
					ptr += num3;
					ptr2 += num4;
				}
				return;
			}
			for (int j = 0; j < height; j++)
			{
				int num6 = 0;
				while (num6 < width)
				{
					array[0] = (array[1] = (array[2] = 0));
					array2[0] = (array2[1] = (array2[2] = 0));
					int num7 = bayerPattern[j & 1, num6 & 1];
					array[num7] += *ptr;
					array2[num7]++;
					if (num6 != 0)
					{
						num7 = bayerPattern[j & 1, (num6 - 1) & 1];
						array[num7] += ptr[-1];
						array2[num7]++;
					}
					if (num6 != num)
					{
						num7 = bayerPattern[j & 1, (num6 + 1) & 1];
						array[num7] += ptr[1];
						array2[num7]++;
					}
					if (j != 0)
					{
						num7 = bayerPattern[(j - 1) & 1, num6 & 1];
						array[num7] += ptr[-stride];
						array2[num7]++;
						if (num6 != 0)
						{
							num7 = bayerPattern[(j - 1) & 1, (num6 - 1) & 1];
							array[num7] += ptr[-stride - 1];
							array2[num7]++;
						}
						if (num6 != num)
						{
							num7 = bayerPattern[(j - 1) & 1, (num6 + 1) & 1];
							array[num7] += ptr[-stride + 1];
							array2[num7]++;
						}
					}
					if (j != num2)
					{
						num7 = bayerPattern[(j + 1) & 1, num6 & 1];
						array[num7] += ptr[stride];
						array2[num7]++;
						if (num6 != 0)
						{
							num7 = bayerPattern[(j + 1) & 1, (num6 - 1) & 1];
							array[num7] += ptr[stride - 1];
							array2[num7]++;
						}
						if (num6 != num)
						{
							num7 = bayerPattern[(j + 1) & 1, (num6 + 1) & 1];
							array[num7] += ptr[stride + 1];
							array2[num7]++;
						}
					}
					ptr2[2] = (byte)(array[2] / array2[2]);
					ptr2[1] = (byte)(array[1] / array2[1]);
					*ptr2 = (byte)(array[0] / array2[0]);
					num6++;
					ptr++;
					ptr2 += 3;
				}
				ptr += num3;
				ptr2 += num4;
			}
		}
	}
}
