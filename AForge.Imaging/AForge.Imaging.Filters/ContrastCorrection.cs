using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class ContrastCorrection : BaseInPlacePartialFilter
	{
		private LevelsLinear baseFilter = new LevelsLinear();

		private int factor;

		public int Factor
		{
			get
			{
				return factor;
			}
			set
			{
				factor = System.Math.Max(-127, System.Math.Min(127, value));
				if (factor > 0)
				{
					LevelsLinear levelsLinear = baseFilter;
					LevelsLinear levelsLinear2 = baseFilter;
					LevelsLinear levelsLinear3 = baseFilter;
					IntRange intRange2 = baseFilter.InGray = new IntRange(factor, 255 - factor);
					IntRange intRange4 = levelsLinear3.InBlue = intRange2;
					IntRange intRange7 = levelsLinear.InRed = (levelsLinear2.InGreen = intRange4);
					LevelsLinear levelsLinear4 = baseFilter;
					LevelsLinear levelsLinear5 = baseFilter;
					LevelsLinear levelsLinear6 = baseFilter;
					IntRange intRange9 = baseFilter.OutGray = new IntRange(0, 255);
					IntRange intRange11 = levelsLinear6.OutBlue = intRange9;
					IntRange intRange14 = levelsLinear4.OutRed = (levelsLinear5.OutGreen = intRange11);
				}
				else
				{
					LevelsLinear levelsLinear7 = baseFilter;
					LevelsLinear levelsLinear8 = baseFilter;
					LevelsLinear levelsLinear9 = baseFilter;
					IntRange intRange16 = baseFilter.OutGray = new IntRange(-factor, 255 + factor);
					IntRange intRange18 = levelsLinear9.OutBlue = intRange16;
					IntRange intRange21 = levelsLinear7.OutRed = (levelsLinear8.OutGreen = intRange18);
					LevelsLinear levelsLinear10 = baseFilter;
					LevelsLinear levelsLinear11 = baseFilter;
					LevelsLinear levelsLinear12 = baseFilter;
					IntRange intRange23 = baseFilter.InGray = new IntRange(0, 255);
					IntRange intRange25 = levelsLinear12.InBlue = intRange23;
					IntRange intRange28 = levelsLinear10.InRed = (levelsLinear11.InGreen = intRange25);
				}
			}
		}

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => baseFilter.FormatTranslations;

		public ContrastCorrection()
		{
			Factor = 10;
		}

		public ContrastCorrection(int factor)
		{
			Factor = factor;
		}

		protected override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			baseFilter.ApplyInPlace(image, rect);
		}
	}
}
