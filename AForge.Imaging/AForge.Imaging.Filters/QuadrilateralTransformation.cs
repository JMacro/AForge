using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class QuadrilateralTransformation : BaseTransformationFilter
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

		public QuadrilateralTransformation()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
			formatTranslations[PixelFormat.Format32bppPArgb] = PixelFormat.Format32bppPArgb;
		}

		public QuadrilateralTransformation(List<IntPoint> sourceQuadrilateral, int newWidth, int newHeight)
			: this()
		{
			automaticSizeCalculaton = false;
			this.sourceQuadrilateral = sourceQuadrilateral;
			this.newWidth = newWidth;
			this.newHeight = newHeight;
		}

		public QuadrilateralTransformation(List<IntPoint> sourceQuadrilateral)
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
			int num2 = stride2 - width2 * num;
			List<IntPoint> list = new List<IntPoint>();
			list.Add(new IntPoint(0, 0));
			list.Add(new IntPoint(width2 - 1, 0));
			list.Add(new IntPoint(width2 - 1, height2 - 1));
			list.Add(new IntPoint(0, height2 - 1));
			double[,] array = QuadTransformationCalcs.MapQuadToQuad(list, sourceQuadrilateral);
			byte* ptr = (byte*)destinationData.ImageData.ToPointer();
			byte* ptr2 = (byte*)sourceData.ImageData.ToPointer();
			if (!useInterpolation)
			{
				for (int i = 0; i < height2; i++)
				{
					for (int j = 0; j < width2; j++)
					{
						double num3 = array[2, 0] * (double)j + array[2, 1] * (double)i + array[2, 2];
						double num4 = (array[0, 0] * (double)j + array[0, 1] * (double)i + array[0, 2]) / num3;
						double num5 = (array[1, 0] * (double)j + array[1, 1] * (double)i + array[1, 2]) / num3;
						if (num4 >= 0.0 && num5 >= 0.0 && num4 < (double)width && num5 < (double)height)
						{
							byte* ptr3 = ptr2 + (long)(int)num5 * (long)stride + (long)(int)num4 * (long)num;
							int num6 = 0;
							while (num6 < num)
							{
								*ptr = *ptr3;
								num6++;
								ptr++;
								ptr3++;
							}
						}
						else
						{
							ptr += num;
						}
					}
					ptr += num2;
				}
				return;
			}
			int num7 = width - 1;
			int num8 = height - 1;
			for (int k = 0; k < height2; k++)
			{
				for (int l = 0; l < width2; l++)
				{
					double num9 = array[2, 0] * (double)l + array[2, 1] * (double)k + array[2, 2];
					double num10 = (array[0, 0] * (double)l + array[0, 1] * (double)k + array[0, 2]) / num9;
					double num11 = (array[1, 0] * (double)l + array[1, 1] * (double)k + array[1, 2]) / num9;
					if (num10 >= 0.0 && num11 >= 0.0 && num10 < (double)width && num11 < (double)height)
					{
						int num12 = (int)num10;
						int num13 = (num12 == num7) ? num12 : (num12 + 1);
						double num14 = num10 - (double)num12;
						double num15 = 1.0 - num14;
						int num16 = (int)num11;
						int num17 = (num16 == num8) ? num16 : (num16 + 1);
						double num18 = num11 - (double)num16;
						double num19 = 1.0 - num18;
						byte* ptr4;
						byte* ptr5 = ptr4 = ptr2 + (long)num16 * (long)stride;
						ptr5 += (long)num12 * (long)num;
						ptr4 += (long)num13 * (long)num;
						byte* ptr6;
						byte* ptr7 = ptr6 = ptr2 + (long)num17 * (long)stride;
						ptr7 += (long)num12 * (long)num;
						ptr6 += (long)num13 * (long)num;
						int num20 = 0;
						while (num20 < num)
						{
							*ptr = (byte)(num19 * (num15 * (double)(int)(*ptr5) + num14 * (double)(int)(*ptr4)) + num18 * (num15 * (double)(int)(*ptr7) + num14 * (double)(int)(*ptr6)));
							num20++;
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
				ptr += num2;
			}
		}
	}
}
