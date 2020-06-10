using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public interface IBlockMatching
	{
		List<BlockMatch> ProcessImage(Bitmap sourceImage, List<IntPoint> coordinates, Bitmap searchImage);

		List<BlockMatch> ProcessImage(BitmapData sourceImageData, List<IntPoint> coordinates, BitmapData searchImageData);

		List<BlockMatch> ProcessImage(UnmanagedImage sourceImage, List<IntPoint> coordinates, UnmanagedImage searchImage);
	}
}
