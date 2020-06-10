using System;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class FillHoles : BaseInPlaceFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		private bool coupledSizeFiltering = true;

		private int maxHoleWidth = int.MaxValue;

		private int maxHoleHeight = int.MaxValue;

		public bool CoupledSizeFiltering
		{
			get
			{
				return coupledSizeFiltering;
			}
			set
			{
				coupledSizeFiltering = value;
			}
		}

		public int MaxHoleWidth
		{
			get
			{
				return maxHoleWidth;
			}
			set
			{
				maxHoleWidth = System.Math.Max(value, 0);
			}
		}

		public int MaxHoleHeight
		{
			get
			{
				return maxHoleHeight;
			}
			set
			{
				maxHoleHeight = System.Math.Max(value, 0);
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public FillHoles()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image)
		{
			int width = image.Width;
			int height = image.Height;
			Invert invert = new Invert();
			UnmanagedImage image2 = invert.Apply(image);
			BlobCounter blobCounter = new BlobCounter();
			blobCounter.ProcessImage(image2);
			Blob[] objectsInformation = blobCounter.GetObjectsInformation();
			byte[] array = new byte[objectsInformation.Length + 1];
			array[0] = byte.MaxValue;
			int i = 0;
			for (int num = objectsInformation.Length; i < num; i++)
			{
				Blob blob = objectsInformation[i];
				if (blob.Rectangle.Left == 0 || blob.Rectangle.Top == 0 || blob.Rectangle.Right == width || blob.Rectangle.Bottom == height)
				{
					array[blob.ID] = 0;
				}
				else if ((coupledSizeFiltering && blob.Rectangle.Width <= maxHoleWidth && blob.Rectangle.Height <= maxHoleHeight) | (!coupledSizeFiltering && (blob.Rectangle.Width <= maxHoleWidth || blob.Rectangle.Height <= maxHoleHeight)))
				{
					array[blob.ID] = byte.MaxValue;
				}
				else
				{
					array[blob.ID] = 0;
				}
			}
			byte* ptr = (byte*)image.ImageData.ToPointer();
			int num2 = image.Stride - width;
			int[] objectLabels = blobCounter.ObjectLabels;
			int j = 0;
			int num3 = 0;
			for (; j < height; j++)
			{
				int num4 = 0;
				while (num4 < width)
				{
					*ptr = array[objectLabels[num3]];
					num4++;
					num3++;
					ptr++;
				}
				ptr += num2;
			}
		}
	}
}
