using System.Collections.Generic;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class TopHat : BaseInPlaceFilter
	{
		private Opening opening = new Opening();

		private Subtract subtract = new Subtract();

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public TopHat()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
		}

		public TopHat(short[,] se)
			: this()
		{
			opening = new Opening(se);
		}

		protected override void ProcessFilter(UnmanagedImage image)
		{
			UnmanagedImage unmanagedImage = opening.Apply(image);
			subtract.UnmanagedOverlayImage = unmanagedImage;
			subtract.ApplyInPlace(image);
			unmanagedImage.Dispose();
		}
	}
}
