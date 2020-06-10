using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	[Obsolete("The class is deprecated and SimpleQuadrilateralTransformation should be used instead")]
	public class QuadrilateralTransformationNearestNeighbor : BaseTransformationFilter
	{
		private SimpleQuadrilateralTransformation baseFilter;

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => baseFilter.FormatTranslations;

		public bool AutomaticSizeCalculaton
		{
			get
			{
				return baseFilter.AutomaticSizeCalculaton;
			}
			set
			{
				baseFilter.AutomaticSizeCalculaton = value;
			}
		}

		public List<IntPoint> SourceCorners
		{
			get
			{
				return baseFilter.SourceQuadrilateral;
			}
			set
			{
				baseFilter.SourceQuadrilateral = value;
			}
		}

		public int NewWidth
		{
			get
			{
				return baseFilter.NewWidth;
			}
			set
			{
				baseFilter.NewWidth = value;
			}
		}

		public int NewHeight
		{
			get
			{
				return baseFilter.NewHeight;
			}
			set
			{
				baseFilter.NewHeight = value;
			}
		}

		public QuadrilateralTransformationNearestNeighbor(List<IntPoint> sourceCorners, int newWidth, int newHeight)
		{
			baseFilter = new SimpleQuadrilateralTransformation(sourceCorners, newWidth, newHeight);
			baseFilter.UseInterpolation = false;
		}

		public QuadrilateralTransformationNearestNeighbor(List<IntPoint> sourceCorners)
		{
			baseFilter = new SimpleQuadrilateralTransformation(sourceCorners);
			baseFilter.UseInterpolation = false;
		}

		protected override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			baseFilter.Apply(sourceData, destinationData);
		}

		protected override Size CalculateNewImageSize(UnmanagedImage sourceData)
		{
			foreach (IntPoint item in baseFilter.SourceQuadrilateral)
			{
				if (item.X < 0 || item.Y < 0 || item.X >= sourceData.Width || item.Y >= sourceData.Height)
				{
					throw new ArgumentException("The specified quadrilateral's corners are outside of the given image.");
				}
			}
			return new Size(baseFilter.NewWidth, baseFilter.NewHeight);
		}
	}
}
