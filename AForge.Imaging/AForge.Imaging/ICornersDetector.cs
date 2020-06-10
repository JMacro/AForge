using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public interface ICornersDetector
	{
		List<IntPoint> ProcessImage(Bitmap image);

		List<IntPoint> ProcessImage(BitmapData imageData);

		List<IntPoint> ProcessImage(UnmanagedImage image);
	}
}
