using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ConnectedComponentsLabeling : BaseFilter
	{
		private static Color[] colorTable = new Color[32]
		{
			Color.Red,
			Color.Green,
			Color.Blue,
			Color.Yellow,
			Color.Violet,
			Color.Brown,
			Color.Olive,
			Color.Cyan,
			Color.Magenta,
			Color.Gold,
			Color.Indigo,
			Color.Ivory,
			Color.HotPink,
			Color.DarkRed,
			Color.DarkGreen,
			Color.DarkBlue,
			Color.DarkSeaGreen,
			Color.Gray,
			Color.DarkKhaki,
			Color.DarkGray,
			Color.LimeGreen,
			Color.Tomato,
			Color.SteelBlue,
			Color.SkyBlue,
			Color.Silver,
			Color.Salmon,
			Color.SaddleBrown,
			Color.RosyBrown,
			Color.PowderBlue,
			Color.Plum,
			Color.PapayaWhip,
			Color.Orange
		};

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		private BlobCounterBase blobCounter = new BlobCounter();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public BlobCounterBase BlobCounter
		{
			get
			{
				return blobCounter;
			}
			set
			{
				blobCounter = value;
			}
		}

		public static Color[] ColorTable
		{
			get
			{
				return colorTable;
			}
			set
			{
				colorTable = value;
			}
		}

		public bool FilterBlobs
		{
			get
			{
				return blobCounter.FilterBlobs;
			}
			set
			{
				blobCounter.FilterBlobs = value;
			}
		}

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

		public int ObjectCount => blobCounter.ObjectsCount;

		public ConnectedComponentsLabeling()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format24bppRgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			blobCounter.ProcessImage(sourceData);
			int[] objectLabels = blobCounter.ObjectLabels;
			int width = sourceData.Width;
			int height = sourceData.Height;
			int num = destinationData.Stride - width * 3;
			byte* ptr = (byte*)destinationData.ImageData.ToPointer();
			int num2 = 0;
			for (int i = 0; i < height; i++)
			{
				int num3 = 0;
				while (num3 < width)
				{
					if (objectLabels[num2] != 0)
					{
						Color color = colorTable[(objectLabels[num2] - 1) % colorTable.Length];
						ptr[2] = color.R;
						ptr[1] = color.G;
						*ptr = color.B;
					}
					num3++;
					ptr += 3;
					num2++;
				}
				ptr += num;
			}
		}
	}
}
