using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class ExhaustiveBlockMatching : IBlockMatching
	{
		private class MatchingsSorter : IComparer<BlockMatch>
		{
			public int Compare(BlockMatch x, BlockMatch y)
			{
				float num = y.Similarity - x.Similarity;
				if (!(num > 0f))
				{
					if (!(num < 0f))
					{
						return 0;
					}
					return -1;
				}
				return 1;
			}
		}

		private int blockSize = 16;

		private int searchRadius = 12;

		private float similarityThreshold = 0.9f;

		public int SearchRadius
		{
			get
			{
				return searchRadius;
			}
			set
			{
				searchRadius = value;
			}
		}

		public int BlockSize
		{
			get
			{
				return blockSize;
			}
			set
			{
				blockSize = value;
			}
		}

		public float SimilarityThreshold
		{
			get
			{
				return similarityThreshold;
			}
			set
			{
				similarityThreshold = System.Math.Min(1f, System.Math.Max(0f, value));
			}
		}

		public ExhaustiveBlockMatching()
		{
		}

		public ExhaustiveBlockMatching(int blockSize, int searchRadius)
		{
			this.blockSize = blockSize;
			this.searchRadius = searchRadius;
		}

		public List<BlockMatch> ProcessImage(Bitmap sourceImage, List<IntPoint> coordinates, Bitmap searchImage)
		{
			BitmapData bitmapData = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, sourceImage.PixelFormat);
			BitmapData bitmapData2 = searchImage.LockBits(new Rectangle(0, 0, searchImage.Width, searchImage.Height), ImageLockMode.ReadOnly, searchImage.PixelFormat);
			try
			{
				return ProcessImage(new UnmanagedImage(bitmapData), coordinates, new UnmanagedImage(bitmapData2));
			}
			finally
			{
				sourceImage.UnlockBits(bitmapData);
				searchImage.UnlockBits(bitmapData2);
			}
		}

		public List<BlockMatch> ProcessImage(BitmapData sourceImageData, List<IntPoint> coordinates, BitmapData searchImageData)
		{
			return ProcessImage(new UnmanagedImage(sourceImageData), coordinates, new UnmanagedImage(searchImageData));
		}

		public unsafe List<BlockMatch> ProcessImage(UnmanagedImage sourceImage, List<IntPoint> coordinates, UnmanagedImage searchImage)
		{
			if (sourceImage.Width != searchImage.Width || sourceImage.Height != searchImage.Height)
			{
				throw new InvalidImagePropertiesException("Source and search images sizes must match");
			}
			if (sourceImage.PixelFormat != PixelFormat.Format8bppIndexed && sourceImage.PixelFormat != PixelFormat.Format24bppRgb)
			{
				throw new UnsupportedImageFormatException("Source images can be graysclae (8 bpp indexed) or color (24 bpp) image only");
			}
			if (sourceImage.PixelFormat != searchImage.PixelFormat)
			{
				throw new InvalidImagePropertiesException("Source and search images must have same pixel format");
			}
			int count = coordinates.Count;
			List<BlockMatch> list = new List<BlockMatch>();
			int width = sourceImage.Width;
			int height = sourceImage.Height;
			int stride = sourceImage.Stride;
			int num = (sourceImage.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int num2 = blockSize / 2;
			int num3 = 2 * searchRadius;
			int num4 = blockSize * num;
			int num5 = stride - blockSize * num;
			int num6 = blockSize * blockSize * num * 255;
			int num7 = (int)(similarityThreshold * (float)num6);
			byte* ptr = (byte*)sourceImage.ImageData.ToPointer();
			byte* ptr2 = (byte*)searchImage.ImageData.ToPointer();
			for (int i = 0; i < count; i++)
			{
				int x = coordinates[i].X;
				int y = coordinates[i].Y;
				if (x - num2 < 0 || x + num2 >= width || y - num2 < 0 || y + num2 >= height)
				{
					continue;
				}
				int num8 = x - num2 - searchRadius;
				int num9 = y - num2 - searchRadius;
				int x2 = x;
				int y2 = y;
				int num10 = int.MaxValue;
				for (int j = 0; j < num3; j++)
				{
					if (num9 + j < 0 || num9 + j + blockSize >= height)
					{
						continue;
					}
					for (int k = 0; k < num3; k++)
					{
						int num11 = num8 + k;
						int num12 = num9 + j;
						if (num11 < 0 || num12 + blockSize >= width)
						{
							continue;
						}
						byte* ptr3 = ptr + (long)(y - num2) * (long)stride + (long)(x - num2) * (long)num;
						byte* ptr4 = ptr2 + (long)num12 * (long)stride + (long)num11 * (long)num;
						int num13 = 0;
						for (int l = 0; l < blockSize; l++)
						{
							int num14 = 0;
							while (num14 < num4)
							{
								int num15 = *ptr3 - *ptr4;
								num13 = ((num15 <= 0) ? (num13 - num15) : (num13 + num15));
								num14++;
								ptr3++;
								ptr4++;
							}
							ptr3 += num5;
							ptr4 += num5;
						}
						if (num13 < num10)
						{
							num10 = num13;
							x2 = num11 + num2;
							y2 = num12 + num2;
						}
					}
				}
				int num16 = num6 - num10;
				if (num16 >= num7)
				{
					list.Add(new BlockMatch(new IntPoint(x, y), new IntPoint(x2, y2), (float)num16 / (float)num6));
				}
			}
			list.Sort(new MatchingsSorter());
			return list;
		}
	}
}
