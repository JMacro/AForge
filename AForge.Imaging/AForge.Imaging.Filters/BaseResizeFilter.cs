using System;
using System.Drawing;

namespace AForge.Imaging.Filters
{
	public abstract class BaseResizeFilter : BaseTransformationFilter
	{
		protected int newWidth;

		protected int newHeight;

		public int NewWidth
		{
			get
			{
				return newWidth;
			}
			set
			{
				newWidth = System.Math.Max(1, value);
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
				newHeight = System.Math.Max(1, value);
			}
		}

		protected BaseResizeFilter(int newWidth, int newHeight)
		{
			this.newWidth = newWidth;
			this.newHeight = newHeight;
		}

		protected override Size CalculateNewImageSize(UnmanagedImage sourceData)
		{
			return new Size(newWidth, newHeight);
		}
	}
}
