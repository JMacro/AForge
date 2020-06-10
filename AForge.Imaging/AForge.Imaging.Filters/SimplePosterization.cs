using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class SimplePosterization : BaseInPlacePartialFilter
	{
		public enum PosterizationFillingType
		{
			Min,
			Max,
			Average
		}

		private byte posterizationInterval = 64;

		private PosterizationFillingType fillingType = PosterizationFillingType.Average;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public byte PosterizationInterval
		{
			get
			{
				return posterizationInterval;
			}
			set
			{
				posterizationInterval = value;
			}
		}

		public PosterizationFillingType FillingType
		{
			get
			{
				return fillingType;
			}
			set
			{
				fillingType = value;
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public SimplePosterization()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
		}

		public SimplePosterization(PosterizationFillingType fillingType)
			: this()
		{
			this.fillingType = fillingType;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int left = rect.Left;
			int top = rect.Top;
			int num2 = left + rect.Width;
			int num3 = top + rect.Height;
			int num4 = image.Stride - rect.Width * num;
			int num5 = (fillingType != 0) ? ((fillingType == PosterizationFillingType.Max) ? (posterizationInterval - 1) : ((int)posterizationInterval / 2)) : 0;
			byte[] array = new byte[256];
			for (int i = 0; i < 256; i++)
			{
				array[i] = (byte)System.Math.Min(255, i / (int)posterizationInterval * posterizationInterval + num5);
			}
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += top * image.Stride + left * num;
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				for (int j = top; j < num3; j++)
				{
					int num6 = left;
					while (num6 < num2)
					{
						*ptr = array[*ptr];
						num6++;
						ptr++;
					}
					ptr += num4;
				}
				return;
			}
			for (int k = top; k < num3; k++)
			{
				int num7 = left;
				while (num7 < num2)
				{
					ptr[2] = array[ptr[2]];
					ptr[1] = array[ptr[1]];
					*ptr = array[*ptr];
					num7++;
					ptr += num;
				}
				ptr += num4;
			}
		}
	}
}
