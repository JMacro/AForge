using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Erosion : BaseUsingCopyPartialFilter
	{
		private short[,] se = new short[3, 3]
		{
			{
				1,
				1,
				1
			},
			{
				1,
				1,
				1
			},
			{
				1,
				1,
				1
			}
		};

		private int size = 3;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public Erosion()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
		}

		public Erosion(short[,] se)
			: this()
		{
			int length = se.GetLength(0);
			if (length != se.GetLength(1) || length < 3 || length > 99 || length % 2 == 0)
			{
				throw new ArgumentException("Invalid size of structuring element.");
			}
			this.se = se;
			size = length;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData, Rectangle rect)
		{
			PixelFormat pixelFormat = sourceData.PixelFormat;
			int left = rect.Left;
			int top = rect.Top;
			int num = left + rect.Width;
			int num2 = top + rect.Height;
			int num3 = size >> 1;
			int num4;
			switch (pixelFormat)
			{
			case PixelFormat.Format24bppRgb:
			case PixelFormat.Format8bppIndexed:
			{
				int num5 = (pixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
				int stride = destinationData.Stride;
				int stride2 = sourceData.Stride;
				byte* ptr = (byte*)sourceData.ImageData.ToPointer();
				byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
				ptr += (long)left * (long)num5;
				ptr2 += (long)left * (long)num5;
				if (pixelFormat == PixelFormat.Format8bppIndexed)
				{
					for (int i = top; i < num2; i++)
					{
						byte* ptr3 = ptr + (long)i * (long)stride2;
						byte* ptr4 = ptr2 + (long)i * (long)stride;
						int num6 = left;
						while (num6 < num)
						{
							byte b = byte.MaxValue;
							for (int j = 0; j < size; j++)
							{
								int num7 = j - num3;
								int num8 = i + num7;
								if (num8 < top)
								{
									continue;
								}
								if (num8 >= num2)
								{
									break;
								}
								for (int k = 0; k < size; k++)
								{
									int num9 = k - num3;
									num8 = num6 + num9;
									if (num8 >= left && num8 < num && se[j, k] == 1)
									{
										byte b2 = ptr3[num7 * stride2 + num9];
										if (b2 < b)
										{
											b = b2;
										}
									}
								}
							}
							*ptr4 = b;
							num6++;
							ptr3++;
							ptr4++;
						}
					}
					return;
				}
				for (int l = top; l < num2; l++)
				{
					byte* ptr5 = ptr + (long)l * (long)stride2;
					byte* ptr6 = ptr2 + (long)l * (long)stride;
					int num10 = left;
					while (num10 < num)
					{
						byte b4;
						byte b3;
						byte b5 = b4 = (b3 = byte.MaxValue);
						for (int m = 0; m < size; m++)
						{
							int num11 = m - num3;
							int num12 = l + num11;
							if (num12 < top)
							{
								continue;
							}
							if (num12 >= num2)
							{
								break;
							}
							for (int n = 0; n < size; n++)
							{
								int num13 = n - num3;
								num12 = num10 + num13;
								if (num12 >= left && num12 < num && se[m, n] == 1)
								{
									byte* ptr7 = ptr5 + (num11 * stride2 + num13 * 3);
									byte b6 = ptr7[2];
									if (b6 < b5)
									{
										b5 = b6;
									}
									b6 = ptr7[1];
									if (b6 < b4)
									{
										b4 = b6;
									}
									b6 = *ptr7;
									if (b6 < b3)
									{
										b3 = b6;
									}
								}
							}
						}
						ptr6[2] = b5;
						ptr6[1] = b4;
						*ptr6 = b3;
						num10++;
						ptr5 += 3;
						ptr6 += 3;
					}
				}
				return;
			}
			default:
				num4 = 3;
				break;
			case PixelFormat.Format16bppGrayScale:
				num4 = 1;
				break;
			}
			int num14 = num4;
			int num15 = destinationData.Stride / 2;
			int num16 = sourceData.Stride / 2;
			ushort* ptr8 = (ushort*)sourceData.ImageData.ToPointer();
			ushort* ptr9 = (ushort*)destinationData.ImageData.ToPointer();
			ptr8 += left * num14;
			ptr9 += left * num14;
			if (pixelFormat == PixelFormat.Format16bppGrayScale)
			{
				for (int num17 = top; num17 < num2; num17++)
				{
					ushort* ptr10 = ptr8 + num17 * num16;
					ushort* ptr11 = ptr9 + num17 * num15;
					int num18 = left;
					while (num18 < num)
					{
						ushort num19 = ushort.MaxValue;
						for (int num20 = 0; num20 < size; num20++)
						{
							int num21 = num20 - num3;
							int num22 = num17 + num21;
							if (num22 < top)
							{
								continue;
							}
							if (num22 >= num2)
							{
								break;
							}
							for (int num23 = 0; num23 < size; num23++)
							{
								int num24 = num23 - num3;
								num22 = num18 + num24;
								if (num22 >= left && num22 < num && se[num20, num23] == 1)
								{
									ushort num25 = ptr10[num21 * num16 + num24];
									if (num25 < num19)
									{
										num19 = num25;
									}
								}
							}
						}
						*ptr11 = num19;
						num18++;
						ptr10++;
						ptr11++;
					}
				}
				return;
			}
			for (int num26 = top; num26 < num2; num26++)
			{
				ushort* ptr12 = ptr8 + num26 * num16;
				ushort* ptr13 = ptr9 + num26 * num15;
				int num27 = left;
				while (num27 < num)
				{
					ushort num29;
					ushort num28;
					ushort num30 = num29 = (num28 = ushort.MaxValue);
					for (int num31 = 0; num31 < size; num31++)
					{
						int num32 = num31 - num3;
						int num33 = num26 + num32;
						if (num33 < top)
						{
							continue;
						}
						if (num33 >= num2)
						{
							break;
						}
						for (int num34 = 0; num34 < size; num34++)
						{
							int num35 = num34 - num3;
							num33 = num27 + num35;
							if (num33 >= left && num33 < num && se[num31, num34] == 1)
							{
								ushort* ptr14 = ptr12 + (num32 * num16 + num35 * 3);
								ushort num36 = ptr14[2];
								if (num36 < num30)
								{
									num30 = num36;
								}
								num36 = ptr14[1];
								if (num36 < num29)
								{
									num29 = num36;
								}
								num36 = *ptr14;
								if (num36 < num28)
								{
									num28 = num36;
								}
							}
						}
					}
					ptr13[2] = num30;
					ptr13[1] = num29;
					*ptr13 = num28;
					num27++;
					ptr12 += 3;
					ptr13 += 3;
				}
			}
		}
	}
}
