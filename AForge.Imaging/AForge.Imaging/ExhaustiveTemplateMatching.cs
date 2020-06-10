using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public class ExhaustiveTemplateMatching : ITemplateMatching
	{
		private class MatchingsSorter : IComparer
		{
			public int Compare(object x, object y)
			{
				float num = ((TemplateMatch)y).Similarity - ((TemplateMatch)x).Similarity;
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

		private float similarityThreshold = 0.9f;

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

		public ExhaustiveTemplateMatching()
		{
		}

		public ExhaustiveTemplateMatching(float similarityThreshold)
		{
			this.similarityThreshold = similarityThreshold;
		}

		public TemplateMatch[] ProcessImage(Bitmap image, Bitmap template)
		{
			return ProcessImage(image, template, new Rectangle(0, 0, image.Width, image.Height));
		}

		public TemplateMatch[] ProcessImage(Bitmap image, Bitmap template, Rectangle searchZone)
		{
			if ((image.PixelFormat != PixelFormat.Format8bppIndexed && image.PixelFormat != PixelFormat.Format24bppRgb) || image.PixelFormat != template.PixelFormat)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source or template image.");
			}
			if (template.Width > image.Width || template.Height > image.Height)
			{
				throw new InvalidImagePropertiesException("Template's size should be smaller or equal to source image's size.");
			}
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			BitmapData bitmapData2 = template.LockBits(new Rectangle(0, 0, template.Width, template.Height), ImageLockMode.ReadOnly, template.PixelFormat);
			try
			{
				return ProcessImage(new UnmanagedImage(bitmapData), new UnmanagedImage(bitmapData2), searchZone);
			}
			finally
			{
				image.UnlockBits(bitmapData);
				template.UnlockBits(bitmapData2);
			}
		}

		public TemplateMatch[] ProcessImage(BitmapData imageData, BitmapData templateData)
		{
			return ProcessImage(new UnmanagedImage(imageData), new UnmanagedImage(templateData), new Rectangle(0, 0, imageData.Width, imageData.Height));
		}

		public TemplateMatch[] ProcessImage(BitmapData imageData, BitmapData templateData, Rectangle searchZone)
		{
			return ProcessImage(new UnmanagedImage(imageData), new UnmanagedImage(templateData), searchZone);
		}

		public TemplateMatch[] ProcessImage(UnmanagedImage image, UnmanagedImage template)
		{
			return ProcessImage(image, template, new Rectangle(0, 0, image.Width, image.Height));
		}

		public unsafe TemplateMatch[] ProcessImage(UnmanagedImage image, UnmanagedImage template, Rectangle searchZone)
		{
			if ((image.PixelFormat != PixelFormat.Format8bppIndexed && image.PixelFormat != PixelFormat.Format24bppRgb) || image.PixelFormat != template.PixelFormat)
			{
				throw new UnsupportedImageFormatException("Unsupported pixel format of the source or template image.");
			}
			Rectangle rectangle = searchZone;
			rectangle.Intersect(new Rectangle(0, 0, image.Width, image.Height));
			int x = rectangle.X;
			int y = rectangle.Y;
			int width = rectangle.Width;
			int height = rectangle.Height;
			int width2 = template.Width;
			int height2 = template.Height;
			if (width2 > width || height2 > height)
			{
				throw new InvalidImagePropertiesException("Template's size should be smaller or equal to search zone.");
			}
			int num = (image.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
			int stride = image.Stride;
			int num2 = width - width2 + 1;
			int num3 = height - height2 + 1;
			int[,] array = new int[num3 + 4, num2 + 4];
			int num4 = width2 * height2 * num * 255;
			int num5 = (int)(similarityThreshold * (float)num4);
			int num6 = width2 * num;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			byte* ptr2 = (byte*)template.ImageData.ToPointer();
			int num7 = image.Stride - width2 * num;
			int num8 = template.Stride - width2 * num;
			for (int i = 0; i < num3; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					byte* ptr3 = ptr + (long)stride * (long)(i + y) + (long)num * (long)(j + x);
					byte* ptr4 = ptr2;
					int num9 = 0;
					for (int k = 0; k < height2; k++)
					{
						int num10 = 0;
						while (num10 < num6)
						{
							int num11 = *ptr3 - *ptr4;
							num9 = ((num11 <= 0) ? (num9 - num11) : (num9 + num11));
							num10++;
							ptr3++;
							ptr4++;
						}
						ptr3 += num7;
						ptr4 += num8;
					}
					int num12 = num4 - num9;
					if (num12 >= num5)
					{
						array[i + 2, j + 2] = num12;
					}
				}
			}
			List<TemplateMatch> list = new List<TemplateMatch>();
			int l = 2;
			for (int num13 = num3 + 2; l < num13; l++)
			{
				int m = 2;
				for (int num14 = num2 + 2; m < num14; m++)
				{
					int num15 = array[l, m];
					int num16 = -2;
					while (num15 != 0 && num16 <= 2)
					{
						for (int n = -2; n <= 2; n++)
						{
							if (array[l + num16, m + n] > num15)
							{
								num15 = 0;
								break;
							}
						}
						num16++;
					}
					if (num15 != 0)
					{
						list.Add(new TemplateMatch(new Rectangle(m - 2 + x, l - 2 + y, width2, height2), (float)num15 / (float)num4));
					}
				}
			}
			TemplateMatch[] array2 = new TemplateMatch[list.Count];
			list.CopyTo(array2);
			Array.Sort(array2, new MatchingsSorter());
			return array2;
		}
	}
}
