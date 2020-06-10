using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class BrightnessCorrection : BaseInPlacePartialFilter
	{
		private LevelsLinear baseFilter = new LevelsLinear();

		private int adjustValue;

		public int AdjustValue
		{
			get
			{
				return adjustValue;
			}
			set
			{
				adjustValue = System.Math.Max(-255, System.Math.Min(255, value));
				if (adjustValue > 0)
				{
					LevelsLinear levelsLinear = baseFilter;
					LevelsLinear levelsLinear2 = baseFilter;
					LevelsLinear levelsLinear3 = baseFilter;
					IntRange intRange2 = baseFilter.InGray = new IntRange(0, 255 - adjustValue);
					IntRange intRange4 = levelsLinear3.InBlue = intRange2;
					IntRange intRange7 = levelsLinear.InRed = (levelsLinear2.InGreen = intRange4);
					LevelsLinear levelsLinear4 = baseFilter;
					LevelsLinear levelsLinear5 = baseFilter;
					LevelsLinear levelsLinear6 = baseFilter;
					IntRange intRange9 = baseFilter.OutGray = new IntRange(adjustValue, 255);
					IntRange intRange11 = levelsLinear6.OutBlue = intRange9;
					IntRange intRange14 = levelsLinear4.OutRed = (levelsLinear5.OutGreen = intRange11);
				}
				else
				{
					LevelsLinear levelsLinear7 = baseFilter;
					LevelsLinear levelsLinear8 = baseFilter;
					LevelsLinear levelsLinear9 = baseFilter;
					IntRange intRange16 = baseFilter.InGray = new IntRange(-adjustValue, 255);
					IntRange intRange18 = levelsLinear9.InBlue = intRange16;
					IntRange intRange21 = levelsLinear7.InRed = (levelsLinear8.InGreen = intRange18);
					LevelsLinear levelsLinear10 = baseFilter;
					LevelsLinear levelsLinear11 = baseFilter;
					LevelsLinear levelsLinear12 = baseFilter;
					IntRange intRange23 = baseFilter.OutGray = new IntRange(0, 255 + adjustValue);
					IntRange intRange25 = levelsLinear12.OutBlue = intRange23;
					IntRange intRange28 = levelsLinear10.OutRed = (levelsLinear11.OutGreen = intRange25);
				}
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => baseFilter.FormatTranslations;

		public BrightnessCorrection()
		{
			AdjustValue = 10;
		}

		public BrightnessCorrection(int adjustValue)
		{
			AdjustValue = adjustValue;
		}

		protected override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			baseFilter.ApplyInPlace(image, rect);
		}
	}
}
