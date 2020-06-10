using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class CannyEdgeDetector : BaseUsingCopyPartialFilter
	{
		private GaussianBlur gaussianFilter = new GaussianBlur();

		private byte lowThreshold = 20;

		private byte highThreshold = 100;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public byte LowThreshold
		{
			get
			{
				return lowThreshold;
			}
			set
			{
				lowThreshold = value;
			}
		}

		public byte HighThreshold
		{
			get
			{
				return highThreshold;
			}
			set
			{
				highThreshold = value;
			}
		}

		public double GaussianSigma
		{
			get
			{
				return gaussianFilter.Sigma;
			}
			set
			{
				gaussianFilter.Sigma = value;
			}
		}

		public int GaussianSize
		{
			get
			{
				return gaussianFilter.Size;
			}
			set
			{
				gaussianFilter.Size = value;
			}
		}

		public CannyEdgeDetector()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
		}

		public CannyEdgeDetector(byte lowThreshold, byte highThreshold)
			: this()
		{
			this.lowThreshold = lowThreshold;
			this.highThreshold = highThreshold;
		}

		public CannyEdgeDetector(byte lowThreshold, byte highThreshold, double sigma)
			: this()
		{
			this.lowThreshold = lowThreshold;
			this.highThreshold = highThreshold;
			gaussianFilter.Sigma = sigma;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
		{
			int num = rect.Left + 1;
			int num2 = rect.Top + 1;
			int num3 = num + rect.Width - 2;
			int num4 = num2 + rect.Height - 2;
			int num5 = rect.Width - 2;
			int num6 = rect.Height - 2;
			int stride = destination.Stride;
			int stride2 = source.Stride;
			int num7 = stride - rect.Width + 2;
			int num8 = stride2 - rect.Width + 2;
			double num9 = 180.0 / System.Math.PI;
			float num10 = 0f;
			float num11 = 0f;
			UnmanagedImage unmanagedImage = gaussianFilter.Apply(source);
			byte[] array = new byte[num5 * num6];
			float[,] array2 = new float[source.Width, source.Height];
			float num12 = float.NegativeInfinity;
			byte* ptr = (byte*)unmanagedImage.ImageData.ToPointer();
			ptr += stride2 * num2 + num;
			int num13 = 0;
			for (int i = num2; i < num4; i++)
			{
				int num14 = num;
				while (num14 < num3)
				{
					int num15 = ptr[-stride2 + 1] + ptr[stride2 + 1] - ptr[-stride2 - 1] - ptr[stride2 - 1] + 2 * (ptr[1] - ptr[-1]);
					int num16 = ptr[-stride2 - 1] + ptr[-stride2 + 1] - ptr[stride2 - 1] - ptr[stride2 + 1] + 2 * (ptr[-stride2] - ptr[stride2]);
					array2[num14, i] = (float)System.Math.Sqrt(num15 * num15 + num16 * num16);
					if (array2[num14, i] > num12)
					{
						num12 = array2[num14, i];
					}
					double num17;
					if (num15 == 0)
					{
						num17 = ((num16 != 0) ? 90 : 0);
					}
					else
					{
						double num18 = (double)num16 / (double)num15;
						num17 = ((!(num18 < 0.0)) ? (System.Math.Atan(num18) * num9) : (180.0 - System.Math.Atan(0.0 - num18) * num9));
						num17 = ((!(num17 < 22.5)) ? ((!(num17 < 67.5)) ? ((!(num17 < 112.5)) ? ((!(num17 < 157.5)) ? 0.0 : 135.0) : 90.0) : 45.0) : 0.0);
					}
					array[num13] = (byte)num17;
					num14++;
					ptr++;
					num13++;
				}
				ptr += num8;
			}
			byte* ptr2 = (byte*)destination.ImageData.ToPointer();
			ptr2 += stride * num2 + num;
			num13 = 0;
			for (int j = num2; j < num4; j++)
			{
				int num19 = num;
				while (num19 < num3)
				{
					switch (array[num13])
					{
					case 0:
						num10 = array2[num19 - 1, j];
						num11 = array2[num19 + 1, j];
						break;
					case 45:
						num10 = array2[num19 - 1, j + 1];
						num11 = array2[num19 + 1, j - 1];
						break;
					case 90:
						num10 = array2[num19, j + 1];
						num11 = array2[num19, j - 1];
						break;
					case 135:
						num10 = array2[num19 + 1, j + 1];
						num11 = array2[num19 - 1, j - 1];
						break;
					}
					if (array2[num19, j] < num10 || array2[num19, j] < num11)
					{
						*ptr2 = 0;
					}
					else
					{
						*ptr2 = (byte)(array2[num19, j] / num12 * 255f);
					}
					num19++;
					ptr2++;
					num13++;
				}
				ptr2 += num7;
			}
			ptr2 = (byte*)destination.ImageData.ToPointer();
			ptr2 += stride * num2 + num;
			for (int k = num2; k < num4; k++)
			{
				int num20 = num;
				while (num20 < num3)
				{
					if (*ptr2 < highThreshold)
					{
						if (*ptr2 < lowThreshold)
						{
							*ptr2 = 0;
						}
						else if (ptr2[-1] < highThreshold && ptr2[1] < highThreshold && ptr2[-stride - 1] < highThreshold && ptr2[-stride] < highThreshold && ptr2[-stride + 1] < highThreshold && ptr2[stride - 1] < highThreshold && ptr2[stride] < highThreshold && ptr2[stride + 1] < highThreshold)
						{
							*ptr2 = 0;
						}
					}
					num20++;
					ptr2++;
				}
				ptr2 += num7;
			}
			Drawing.Rectangle(destination, rect, Color.Black);
			unmanagedImage.Dispose();
		}
	}
}
