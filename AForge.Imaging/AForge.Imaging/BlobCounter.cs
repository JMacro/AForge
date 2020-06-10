using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class BlobCounter : BlobCounterBase
	{
		private byte backgroundThresholdR;

		private byte backgroundThresholdG;

		private byte backgroundThresholdB;

		public Color BackgroundThreshold
		{
			get
			{
				return Color.FromArgb(backgroundThresholdR, backgroundThresholdG, backgroundThresholdB);
			}
			set
			{
				backgroundThresholdR = value.R;
				backgroundThresholdG = value.G;
				backgroundThresholdB = value.B;
			}
		}

		public BlobCounter()
		{
		}

		public BlobCounter(Bitmap image)
			: base(image)
		{
		}

		public BlobCounter(BitmapData imageData)
			: base(imageData)
		{
		}

		public BlobCounter(UnmanagedImage image)
			: base(image)
		{
		}

		protected unsafe override void BuildObjectsMap(UnmanagedImage image)
		{
			int stride = image.Stride;
			if (image.PixelFormat != PixelFormat.Format8bppIndexed && image.PixelFormat != PixelFormat.Format24bppRgb && image.PixelFormat != PixelFormat.Format32bppRgb && image.PixelFormat != PixelFormat.Format32bppArgb && image.PixelFormat != PixelFormat.Format32bppPArgb)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
			}
			if (imageWidth == 1)
			{
				throw new InvalidImagePropertiesException("BlobCounter cannot process images that are one pixel wide. Rotate the image or use RecursiveBlobCounter.");
			}
			int num = imageWidth - 1;
			objectLabels = new int[imageWidth * imageHeight];
			int num2 = 0;
			int num3 = (imageWidth / 2 + 1) * (imageHeight / 2 + 1) + 1;
			int[] array = new int[num3];
			for (int i = 0; i < num3; i++)
			{
				array[i] = i;
			}
			byte* ptr = (byte*)image.ImageData.ToPointer();
			int num4 = 0;
			if (image.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				int num5 = stride - imageWidth;
				if (*ptr > backgroundThresholdG)
				{
					num2 = (objectLabels[num4] = num2 + 1);
				}
				ptr++;
				num4++;
				int num6 = 1;
				while (num6 < imageWidth)
				{
					if (*ptr > backgroundThresholdG)
					{
						if (ptr[-1] > backgroundThresholdG)
						{
							objectLabels[num4] = objectLabels[num4 - 1];
						}
						else
						{
							num2 = (objectLabels[num4] = num2 + 1);
						}
					}
					num6++;
					ptr++;
					num4++;
				}
				ptr += num5;
				for (int j = 1; j < imageHeight; j++)
				{
					if (*ptr > backgroundThresholdG)
					{
						if (ptr[-stride] > backgroundThresholdG)
						{
							objectLabels[num4] = objectLabels[num4 - imageWidth];
						}
						else if (ptr[1 - stride] <= backgroundThresholdG)
						{
							num2 = (objectLabels[num4] = num2 + 1);
						}
						else
						{
							objectLabels[num4] = objectLabels[num4 + 1 - imageWidth];
						}
					}
					ptr++;
					num4++;
					int num7 = 1;
					while (num7 < num)
					{
						if (*ptr > backgroundThresholdG)
						{
							if (ptr[-1] > backgroundThresholdG)
							{
								objectLabels[num4] = objectLabels[num4 - 1];
							}
							else if (ptr[-1 - stride] > backgroundThresholdG)
							{
								objectLabels[num4] = objectLabels[num4 - 1 - imageWidth];
							}
							else if (ptr[-stride] > backgroundThresholdG)
							{
								objectLabels[num4] = objectLabels[num4 - imageWidth];
							}
							if (ptr[1 - stride] > backgroundThresholdG)
							{
								if (objectLabels[num4] == 0)
								{
									objectLabels[num4] = objectLabels[num4 + 1 - imageWidth];
								}
								else
								{
									int num8 = objectLabels[num4];
									int num9 = objectLabels[num4 + 1 - imageWidth];
									if (num8 != num9 && array[num8] != array[num9])
									{
										if (array[num8] == num8)
										{
											array[num8] = array[num9];
										}
										else if (array[num9] == num9)
										{
											array[num9] = array[num8];
										}
										else
										{
											array[array[num8]] = array[num9];
											array[num8] = array[num9];
										}
										for (int k = 1; k <= num2; k++)
										{
											if (array[k] != k)
											{
												int num10;
												for (num10 = array[k]; num10 != array[num10]; num10 = array[num10])
												{
												}
												array[k] = num10;
											}
										}
									}
								}
							}
							if (objectLabels[num4] == 0)
							{
								num2 = (objectLabels[num4] = num2 + 1);
							}
						}
						num7++;
						ptr++;
						num4++;
					}
					if (*ptr > backgroundThresholdG)
					{
						if (ptr[-1] > backgroundThresholdG)
						{
							objectLabels[num4] = objectLabels[num4 - 1];
						}
						else if (ptr[-1 - stride] > backgroundThresholdG)
						{
							objectLabels[num4] = objectLabels[num4 - 1 - imageWidth];
						}
						else if (ptr[-stride] > backgroundThresholdG)
						{
							objectLabels[num4] = objectLabels[num4 - imageWidth];
						}
						else
						{
							num2 = (objectLabels[num4] = num2 + 1);
						}
					}
					ptr++;
					num4++;
					ptr += num5;
				}
			}
			else
			{
				int num11 = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
				int num12 = stride - imageWidth * num11;
				int num13 = stride - num11;
				int num14 = stride + num11;
				if ((ptr[2] | ptr[1] | *ptr) != 0)
				{
					num2 = (objectLabels[num4] = num2 + 1);
				}
				ptr += num11;
				num4++;
				int num15 = 1;
				while (num15 < imageWidth)
				{
					if (ptr[2] > backgroundThresholdR || ptr[1] > backgroundThresholdG || *ptr > backgroundThresholdB)
					{
						if (ptr[2 - num11] > backgroundThresholdR || ptr[1 - num11] > backgroundThresholdG || ptr[-num11] > backgroundThresholdB)
						{
							objectLabels[num4] = objectLabels[num4 - 1];
						}
						else
						{
							num2 = (objectLabels[num4] = num2 + 1);
						}
					}
					num15++;
					ptr += num11;
					num4++;
				}
				ptr += num12;
				for (int l = 1; l < imageHeight; l++)
				{
					if (ptr[2] > backgroundThresholdR || ptr[1] > backgroundThresholdG || *ptr > backgroundThresholdB)
					{
						if (ptr[2 - stride] > backgroundThresholdR || ptr[1 - stride] > backgroundThresholdG || ptr[-stride] > backgroundThresholdB)
						{
							objectLabels[num4] = objectLabels[num4 - imageWidth];
						}
						else if (ptr[2 - num13] <= backgroundThresholdR && ptr[1 - num13] <= backgroundThresholdG && ptr[-num13] <= backgroundThresholdB)
						{
							num2 = (objectLabels[num4] = num2 + 1);
						}
						else
						{
							objectLabels[num4] = objectLabels[num4 + 1 - imageWidth];
						}
					}
					ptr += num11;
					num4++;
					int num16 = 1;
					while (num16 < imageWidth - 1)
					{
						if (ptr[2] > backgroundThresholdR || ptr[1] > backgroundThresholdG || *ptr > backgroundThresholdB)
						{
							if (ptr[2 - num11] > backgroundThresholdR || ptr[1 - num11] > backgroundThresholdG || ptr[-num11] > backgroundThresholdB)
							{
								objectLabels[num4] = objectLabels[num4 - 1];
							}
							else if (ptr[2 - num14] > backgroundThresholdR || ptr[1 - num14] > backgroundThresholdG || ptr[-num14] > backgroundThresholdB)
							{
								objectLabels[num4] = objectLabels[num4 - 1 - imageWidth];
							}
							else if (ptr[2 - stride] > backgroundThresholdR || ptr[1 - stride] > backgroundThresholdG || ptr[-stride] > backgroundThresholdB)
							{
								objectLabels[num4] = objectLabels[num4 - imageWidth];
							}
							if (ptr[2 - num13] > backgroundThresholdR || ptr[1 - num13] > backgroundThresholdG || ptr[-num13] > backgroundThresholdB)
							{
								if (objectLabels[num4] == 0)
								{
									objectLabels[num4] = objectLabels[num4 + 1 - imageWidth];
								}
								else
								{
									int num17 = objectLabels[num4];
									int num18 = objectLabels[num4 + 1 - imageWidth];
									if (num17 != num18 && array[num17] != array[num18])
									{
										if (array[num17] == num17)
										{
											array[num17] = array[num18];
										}
										else if (array[num18] == num18)
										{
											array[num18] = array[num17];
										}
										else
										{
											array[array[num17]] = array[num18];
											array[num17] = array[num18];
										}
										for (int m = 1; m <= num2; m++)
										{
											if (array[m] != m)
											{
												int num19;
												for (num19 = array[m]; num19 != array[num19]; num19 = array[num19])
												{
												}
												array[m] = num19;
											}
										}
									}
								}
							}
							if (objectLabels[num4] == 0)
							{
								num2 = (objectLabels[num4] = num2 + 1);
							}
						}
						num16++;
						ptr += num11;
						num4++;
					}
					if (ptr[2] > backgroundThresholdR || ptr[1] > backgroundThresholdG || *ptr > backgroundThresholdB)
					{
						if (ptr[2 - num11] > backgroundThresholdR || ptr[1 - num11] > backgroundThresholdG || ptr[-num11] > backgroundThresholdB)
						{
							objectLabels[num4] = objectLabels[num4 - 1];
						}
						else if (ptr[2 - num14] > backgroundThresholdR || ptr[1 - num14] > backgroundThresholdG || ptr[-num14] > backgroundThresholdB)
						{
							objectLabels[num4] = objectLabels[num4 - 1 - imageWidth];
						}
						else if (ptr[2 - stride] > backgroundThresholdR || ptr[1 - stride] > backgroundThresholdG || ptr[-stride] > backgroundThresholdB)
						{
							objectLabels[num4] = objectLabels[num4 - imageWidth];
						}
						else
						{
							num2 = (objectLabels[num4] = num2 + 1);
						}
					}
					ptr += num11;
					num4++;
					ptr += num12;
				}
			}
			int[] array2 = new int[array.Length];
			objectsCount = 0;
			for (int n = 1; n <= num2; n++)
			{
				if (array[n] == n)
				{
					array2[n] = ++objectsCount;
				}
			}
			for (int num20 = 1; num20 <= num2; num20++)
			{
				if (array[num20] != num20)
				{
					array2[num20] = array2[array[num20]];
				}
			}
			int num21 = 0;
			for (int num22 = objectLabels.Length; num21 < num22; num21++)
			{
				objectLabels[num21] = array2[objectLabels[num21]];
			}
		}
	}
}
