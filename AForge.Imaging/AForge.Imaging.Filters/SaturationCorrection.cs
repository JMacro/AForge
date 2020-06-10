using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class SaturationCorrection : BaseInPlacePartialFilter
	{
		private HSLLinear baseFilter = new HSLLinear();

		private float adjustValue;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public float AdjustValue
		{
			get
			{
				return adjustValue;
			}
			set
			{
				adjustValue = System.Math.Max(-1f, System.Math.Min(1f, value));
				if (adjustValue > 0f)
				{
					baseFilter.InSaturation = new Range(0f, 1f - adjustValue);
					baseFilter.OutSaturation = new Range(adjustValue, 1f);
				}
				else
				{
					baseFilter.InSaturation = new Range(0f - adjustValue, 1f);
					baseFilter.OutSaturation = new Range(0f, 1f + adjustValue);
				}
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public SaturationCorrection()
			: this(0.1f)
		{
		}

		public SaturationCorrection(float adjustValue)
		{
			AdjustValue = adjustValue;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		protected override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			baseFilter.ApplyInPlace(image, rect);
		}
	}
}
