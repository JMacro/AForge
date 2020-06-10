using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class SimpleQuadrilateralTransformation : BaseTransformationFilter
	{
		private bool automaticSizeCalculaton = true;

		private bool useInterpolation = true;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		protected int newWidth;

		protected int newHeight;

		private List<IntPoint> sourceQuadrilateral;

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public bool AutomaticSizeCalculaton
		{
			get
			{
				return automaticSizeCalculaton;
			}
			set
			{
				automaticSizeCalculaton = value;
				if (value)
				{
					CalculateDestinationSize();
				}
			}
		}

		public List<IntPoint> SourceQuadrilateral
		{
			get
			{
				return sourceQuadrilateral;
			}
			set
			{
				sourceQuadrilateral = value;
				if (automaticSizeCalculaton)
				{
					CalculateDestinationSize();
				}
			}
		}

		public int NewWidth
		{
			get
			{
				return newWidth;
			}
			set
			{
				if (!automaticSizeCalculaton)
				{
					newWidth = System.Math.Max(1, value);
				}
			}
		}

		public int NewHeight
		{
			get
			{
				return newHeight;
			}
			set
			{
				if (!automaticSizeCalculaton)
				{
					newHeight = System.Math.Max(1, value);
				}
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

		public SimpleQuadrilateralTransformation()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
		}

		public SimpleQuadrilateralTransformation(List<IntPoint> sourceQuadrilateral, int newWidth, int newHeight)
			: this()
		{
			automaticSizeCalculaton = false;
			this.sourceQuadrilateral = sourceQuadrilateral;
			this.newWidth = newWidth;
			this.newHeight = newHeight;
		}

		public SimpleQuadrilateralTransformation(List<IntPoint> sourceQuadrilateral)
			: this()
		{
			automaticSizeCalculaton = true;
			this.sourceQuadrilateral = sourceQuadrilateral;
			CalculateDestinationSize();
		}

		protected override Size CalculateNewImageSize(UnmanagedImage sourceData)
		{
			if (sourceQuadrilateral == null)
			{
				throw new NullReferenceException("Source quadrilateral was not set.");
			}
			return new Size(newWidth, newHeight);
		}

		private void CalculateDestinationSize()
		{
			if (sourceQuadrilateral == null)
			{
				throw new NullReferenceException("Source quadrilateral was not set.");
			}
			newWidth = (int)System.Math.Max(sourceQuadrilateral[0].DistanceTo(sourceQuadrilateral[1]), sourceQuadrilateral[2].DistanceTo(sourceQuadrilateral[3]));
			newHeight = (int)System.Math.Max(sourceQuadrilateral[1].DistanceTo(sourceQuadrilateral[2]), sourceQuadrilateral[3].DistanceTo(sourceQuadrilateral[0]));
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			int width2 = destinationData.Width;
			int height2 = destinationData.Height;
			int num = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
			int stride = sourceData.Stride;
			int stride2 = destinationData.Stride;
			if (sourceQuadrilateral[1].X == sourceQuadrilateral[0].X)
			{
				double num2 = 0.0;
				_ = sourceQuadrilateral[1].X;
			}
			else
			{
				double num2 = (double)(sourceQuadrilateral[1].Y - sourceQuadrilateral[0].Y) / (double)(sourceQuadrilateral[1].X - sourceQuadrilateral[0].X);
				_ = sourceQuadrilateral[0].Y;
				_ = sourceQuadrilateral[0].X;
			}
			if (sourceQuadrilateral[2].X == sourceQuadrilateral[3].X)
			{
				double num3 = 0.0;
				_ = sourceQuadrilateral[2].X;
			}
			else
			{
				double num3 = (double)(sourceQuadrilateral[2].Y - sourceQuadrilateral[3].Y) / (double)(sourceQuadrilateral[2].X - sourceQuadrilateral[3].X);
				_ = sourceQuadrilateral[3].Y;
				_ = sourceQuadrilateral[3].X;
			}
			double num4;
			double num5;
			if (sourceQuadrilateral[3].X == sourceQuadrilateral[0].X)
			{
				num4 = 0.0;
				num5 = sourceQuadrilateral[3].X;
			}
			else
			{
				num4 = (double)(sourceQuadrilateral[3].Y - sourceQuadrilateral[0].Y) / (double)(sourceQuadrilateral[3].X - sourceQuadrilateral[0].X);
				num5 = (double)sourceQuadrilateral[0].Y - num4 * (double)sourceQuadrilateral[0].X;
			}
			double num6;
			double num7;
			if (sourceQuadrilateral[2].X == sourceQuadrilateral[1].X)
			{
				num6 = 0.0;
				num7 = sourceQuadrilateral[2].X;
			}
			else
			{
				num6 = (double)(sourceQuadrilateral[2].Y - sourceQuadrilateral[1].Y) / (double)(sourceQuadrilateral[2].X - sourceQuadrilateral[1].X);
				num7 = (double)sourceQuadrilateral[1].Y - num6 * (double)sourceQuadrilateral[1].X;
			}
			double num8 = (double)(sourceQuadrilateral[3].Y - sourceQuadrilateral[0].Y) / (double)height2;
			double num9 = (double)(sourceQuadrilateral[2].Y - sourceQuadrilateral[1].Y) / (double)height2;
			int y = sourceQuadrilateral[0].Y;
			int y2 = sourceQuadrilateral[1].Y;
			byte* ptr = (byte*)sourceData.ImageData.ToPointer();
			byte* ptr2 = (byte*)destinationData.ImageData.ToPointer();
			int num10 = height - 1;
			int num11 = width - 1;
			for (int i = 0; i < height2; i++)
			{
				byte* ptr3 = ptr2 + (long)stride2 * (long)i;
				double num12 = num8 * (double)i + (double)y;
				double num13 = (num4 == 0.0) ? num5 : ((num12 - num5) / num4);
				double num14 = num9 * (double)i + (double)y2;
				double num15 = (num6 == 0.0) ? num7 : ((num14 - num7) / num6);
				double num16;
				double num17;
				if (num13 == num15)
				{
					num16 = 0.0;
					num17 = num15;
				}
				else
				{
					num16 = (num14 - num12) / (num15 - num13);
					num17 = num12 - num16 * num13;
				}
				double num18 = (num15 - num13) / (double)width2;
				if (!useInterpolation)
				{
					for (int j = 0; j < width2; j++)
					{
						double num19 = num18 * (double)j + num13;
						double num20 = num16 * num19 + num17;
						if (num19 >= 0.0 && num20 >= 0.0 && num19 < (double)width && num20 < (double)height)
						{
							byte* ptr4 = ptr + ((int)num20 * stride + (int)num19 * num);
							int num21 = 0;
							while (num21 < num)
							{
								*ptr3 = *ptr4;
								num21++;
								ptr3++;
								ptr4++;
							}
						}
						else
						{
							ptr3 += num;
						}
					}
					continue;
				}
				for (int k = 0; k < width2; k++)
				{
					double num22 = num18 * (double)k + num13;
					double num23 = num16 * num22 + num17;
					if (num22 >= 0.0 && num23 >= 0.0 && num22 < (double)width && num23 < (double)height)
					{
						int num24 = (int)num22;
						int num25 = (num24 == num11) ? num24 : (num24 + 1);
						double num26 = num22 - (double)num24;
						double num27 = 1.0 - num26;
						int num28 = (int)num23;
						int num29 = (num28 == num10) ? num28 : (num28 + 1);
						double num30 = num23 - (double)num28;
						double num31 = 1.0 - num30;
						byte* ptr5;
						byte* ptr6 = ptr5 = ptr + (long)num28 * (long)stride;
						ptr6 += (long)num24 * (long)num;
						ptr5 += (long)num25 * (long)num;
						byte* ptr7;
						byte* ptr8 = ptr7 = ptr + (long)num29 * (long)stride;
						ptr8 += (long)num24 * (long)num;
						ptr7 += (long)num25 * (long)num;
						int num32 = 0;
						while (num32 < num)
						{
							*ptr3 = (byte)(num31 * (num27 * (double)(int)(*ptr6) + num26 * (double)(int)(*ptr5)) + num30 * (num27 * (double)(int)(*ptr8) + num26 * (double)(int)(*ptr7)));
							num32++;
							ptr3++;
							ptr6++;
							ptr5++;
							ptr8++;
							ptr7++;
						}
					}
					else
					{
						ptr3 += num;
					}
				}
			}
		}
	}
}
