using System.Collections.Generic;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public interface IFilterInformation
	{
		Dictionary<PixelFormat, PixelFormat> FormatTranslations
		{
			get;
		}
	}
}
