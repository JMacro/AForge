using AForge.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class BackwardQuadrilateralTransformation : BaseInPlaceFilter
	{
		private Bitmap sourceImage;

		private UnmanagedImage sourceUnmanagedImage;

		private List<IntPoint> destinationQuadrilateral;

		private bool useInterpolation = true;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public Bitmap SourceImage
		{
			get
			{
				return sourceImage;
			}
			set
			{
				sourceImage = value;
				if (value != null)
				{
					sourceUnmanagedImage = null;
				}
			}
		}

		public UnmanagedImage SourceUnmanagedImage
		{
			get
			{
				return sourceUnmanagedImage;
			}
			set
			{
				sourceUnmanagedImage = value;
				if (value != null)
				{
					sourceImage = null;
				}
			}
		}

		public List<IntPoint> DestinationQuadrilateral
		{
			get
			{
				return destinationQuadrilateral;
			}
			set
			{
				destinationQuadrilateral = value;
			}
		}

		public bool UseInterpolation
		{
			get
			{
				return useInterpolation;
			}
			set
			{
				useInterpolation = value;
			}
		}

		public BackwardQuadrilateralTransformation()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
		}

		public BackwardQuadrilateralTransformation(Bitmap sourceImage)
			: this()
		{
			this.sourceImage = sourceImage;
		}

		public BackwardQuadrilateralTransformation(UnmanagedImage sourceUnmanagedImage)
			: this()
		{
			this.sourceUnmanagedImage = sourceUnmanagedImage;
		}

		public BackwardQuadrilateralTransformation(Bitmap sourceImage, List<IntPoint> destinationQuadrilateral)
			: this()
		{
			this.sourceImage = sourceImage;
			this.destinationQuadrilateral = destinationQuadrilateral;
		}

		public BackwardQuadrilateralTransformation(UnmanagedImage sourceUnmanagedImage, List<IntPoint> destinationQuadrilateral)
			: this()
		{
			this.sourceUnmanagedImage = sourceUnmanagedImage;
			this.destinationQuadrilateral = destinationQuadrilateral;
		}

		protected override void ProcessFilter(UnmanagedImage image)
		{
			if (destinationQuadrilateral == null)
			{
				throw new NullReferenceException("Destination quadrilateral was not set.");
			}
			if (sourceImage != null)
			{
				if (image.PixelFormat != sourceImage.PixelFormat)
				{
					throw new InvalidImagePropertiesException("Source and destination images must have same pixel format.");
				}
				BitmapData bitmapData = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, sourceImage.PixelFormat);
				try
				{
					ProcessFilter(image, new UnmanagedImage(bitmapData));
				}
				finally
				{
					sourceImage.UnlockBits(bitmapData);
				}
				return;
			}
			if (sourceUnmanagedImage != null)
			{
				if (image.PixelFormat != sourceUnmanagedImage.PixelFormat)
				{
					throw new InvalidImagePropertiesException("Source and destination images must have same pixel format.");
				}
				ProcessFilter(image, sourceUnmanagedImage);
				return;
			}
			throw new NullReferenceException("Source image is not set.");
		}

		private unsafe void ProcessFilter(UnmanagedImage dstImage, UnmanagedImage srcImage)
		{
			int width = srcImage.Width;
			int height = srcImage.Height;
			int width2 = dstImage.Width;
			int height2 = dstImage.Height;
			int num = System.Drawing.Image.GetPixelFormatSize(srcImage.PixelFormat) / 8;
			int stride = srcImage.Stride;
			int stride2 = dstImage.Stride;
			PointsCloud.GetBoundingRectangle(destinationQuadrilateral, out IntPoint minXY, out IntPoint maxXY);
			if (maxXY.X < 0 || maxXY.Y < 0 || minXY.X >= width2 || minXY.Y >= height2)
			{
				return;
			}
			if (minXY.X < 0)
			{
				minXY.X = 0;
			}
			if (minXY.Y < 0)
			{
				minXY.Y = 0;
			}
			if (maxXY.X >= width2)
			{
				maxXY.X = width2 - 1;
			}
			if (maxXY.Y >= height2)
			{
				maxXY.Y = height2 - 1;
			}
			int x = minXY.X;
			int y = minXY.Y;
			int num2 = maxXY.X + 1;
			int num3 = maxXY.Y + 1;
			int num4 = stride2 - (num2 - x) * num;
			List<IntPoint> list = new List<IntPoint>();
			list.Add(new IntPoint(0, 0));
			list.Add(new IntPoint(width - 1, 0));
			list.Add(new IntPoint(width - 1, height - 1));
			list.Add(new IntPoint(0, height - 1));
			double[,] array = QuadTransformationCalcs.MapQuadToQuad(destinationQuadrilateral, list);
			byte* ptr = (byte*)dstImage.ImageData.ToPointer();
			byte* ptr2 = (byte*)srcImage.ImageData.ToPointer();
			ptr += y * stride2 + x * num;
			if (!useInterpolation)
			{
				for (int i = y; i < num3; i++)
				{
					for (int j = x; j < num2; j++)
					{
						double num5 = array[2, 0] * (double)j + array[2, 1] * (double)i + array[2, 2];
						double num6 = (array[0, 0] * (double)j + array[0, 1] * (double)i + array[0, 2]) / num5;
						double num7 = (array[1, 0] * (double)j + array[1, 1] * (double)i + array[1, 2]) / num5;
						if (num6 >= 0.0 && num7 >= 0.0 && num6 < (double)width && num7 < (double)height)
						{
							byte* ptr3 = ptr2 + (long)(int)num7 * (long)stride + (long)(int)num6 * (long)num;
							int num8 = 0;
							while (num8 < num)
							{
								*ptr = *ptr3;
								num8++;
								ptr++;
								ptr3++;
							}
						}
						else
						{
							ptr += num;
						}
					}
					ptr += num4;
				}
				return;
			}
			int num9 = width - 1;
			int num10 = height - 1;
			for (int k = y; k < num3; k++)
			{
				for (int l = x; l < num2; l++)
				{
					double num11 = array[2, 0] * (double)l + array[2, 1] * (double)k + array[2, 2];
					double num12 = (array[0, 0] * (double)l + array[0, 1] * (double)k + array[0, 2]) / num11;
					double num13 = (array[1, 0] * (double)l + array[1, 1] * (double)k + array[1, 2]) / num11;
					if (num12 >= 0.0 && num13 >= 0.0 && num12 < (double)width && num13 < (double)height)
					{
						int num14 = (int)num12;
						int num15 = (num14 == num9) ? num14 : (num14 + 1);
						double num16 = num12 - (double)num14;
						double num17 = 1.0 - num16;
						int num18 = (int)num13;
						int num19 = (num18 == num10) ? num18 : (num18 + 1);
						double num20 = num13 - (double)num18;
						double num21 = 1.0 - num20;
						byte* ptr4;
						byte* ptr5 = ptr4 = ptr2 + (long)num18 * (long)stride;
						ptr5 += (long)num14 * (long)num;
						ptr4 += (long)num15 * (long)num;
						byte* ptr6;
						byte* ptr7 = ptr6 = ptr2 + (long)num19 * (long)stride;
						ptr7 += (long)num14 * (long)num;
						ptr6 += (long)num15 * (long)num;
						int num22 = 0;
						while (num22 < num)
						{
							*ptr = (byte)(num21 * (num17 * (double)(int)(*ptr5) + num16 * (double)(int)(*ptr4)) + num20 * (num17 * (double)(int)(*ptr7) + num16 * (double)(int)(*ptr6)));
							num22++;
							ptr++;
							ptr5++;
							ptr4++;
							ptr7++;
							ptr6++;
						}
					}
					else
					{
						ptr += num;
					}
				}
				ptr += num4;
			}
		}
	}
}
