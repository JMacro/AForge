using System.Collections.Generic;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class BottomHat : BaseInPlaceFilter
	{
		private Closing closing = new Closing();

		private Subtract subtract = new Subtract();

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public BottomHat()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
			formatTranslations[PixelFormat.Format48bppRgb] = PixelFormat.Format48bppRgb;
		}

		public BottomHat(short[,] se)
			: this()
		{
			closing = new Closing(se);
		}

		protected override void ProcessFilter(UnmanagedImage image)
		{
			UnmanagedImage unmanagedImage = image.Clone();
			closing.ApplyInPlace(image);
			subtract.UnmanagedOverlayImage = unmanagedImage;
			subtract.ApplyInPlace(image);
			unmanagedImage.Dispose();
		}
	}
}
