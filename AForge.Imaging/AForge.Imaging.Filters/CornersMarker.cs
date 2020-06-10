using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class CornersMarker : BaseInPlaceFilter
	{
		private Color markerColor = Color.White;

		private ICornersDetector detector;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public Color MarkerColor
		{
			get
			{
				return markerColor;
			}
			set
			{
				markerColor = value;
			}
		}

		public ICornersDetector Detector
		{
			get
			{
				return detector;
			}
			set
			{
				detector = value;
			}
		}

		public CornersMarker(ICornersDetector detector)
			: this(detector, Color.White)
		{
		}

		public CornersMarker(ICornersDetector detector, Color markerColor)
		{
			this.detector = detector;
			this.markerColor = markerColor;
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
			formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
			formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
		}

		protected override void ProcessFilter(UnmanagedImage image)
		{
			List<IntPoint> list = detector.ProcessImage(image);
			foreach (IntPoint item in list)
			{
				Drawing.FillRectangle(image, new Rectangle(item.X - 1, item.Y - 1, 3, 3), markerColor);
			}
		}
	}
}
