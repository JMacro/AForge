using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class HistogramEqualization : BaseInPlacePartialFilter
	{
		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public HistogramEqualization()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : ((image.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4);
			int left = rect.Left;
			int top = rect.Top;
			int num2 = left + rect.Width;
			int num3 = top + rect.Height;
			int stride = image.Stride;
			int num4 = stride - rect.Width * num;
			int num5 = (num2 - left) * (num3 - top);
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				byte* ptr = (byte*)image.ImageData.ToPointer();
				ptr += top * stride + left;
				int[] array = new int[256];
				for (int i = top; i < num3; i++)
				{
					int num6 = left;
					while (num6 < num2)
					{
						array[*ptr]++;
						num6++;
						ptr++;
					}
					ptr += num4;
				}
				byte[] array2 = Equalize(array, num5);
				ptr = (byte*)image.ImageData.ToPointer();
				ptr += top * stride + left;
				for (int j = top; j < num3; j++)
				{
					int num7 = left;
					while (num7 < num2)
					{
						*ptr = array2[*ptr];
						num7++;
						ptr++;
					}
					ptr += num4;
				}
				return;
			}
			byte* ptr2 = (byte*)image.ImageData.ToPointer();
			ptr2 += top * stride + left * num;
			int[] array3 = new int[256];
			int[] array4 = new int[256];
			int[] array5 = new int[256];
			for (int k = top; k < num3; k++)
			{
				int num8 = left;
				while (num8 < num2)
				{
					array3[ptr2[2]]++;
					array4[ptr2[1]]++;
					array5[*ptr2]++;
					num8++;
					ptr2 += num;
				}
				ptr2 += num4;
			}
			byte[] array6 = Equalize(array3, num5);
			byte[] array7 = Equalize(array4, num5);
			byte[] array8 = Equalize(array5, num5);
			ptr2 = (byte*)image.ImageData.ToPointer();
			ptr2 += top * stride + left * num;
			for (int l = top; l < num3; l++)
			{
				int num9 = left;
				while (num9 < num2)
				{
					ptr2[2] = array6[ptr2[2]];
					ptr2[1] = array7[ptr2[1]];
					*ptr2 = array8[*ptr2];
					num9++;
					ptr2 += num;
				}
				ptr2 += num4;
			}
		}

		private byte[] Equalize(int[] histogram, long numPixel)
		{
			byte[] array = new byte[256];
			float num = 255f / (float)numPixel;
			float num2 = (float)histogram[0] * num;
			array[0] = (byte)num2;
			for (int i = 1; i < 256; i++)
			{
				num2 += (float)histogram[i] * num;
				array[i] = (byte)num2;
			}
			return array;
		}
	}
}
