using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public interface IInPlacePartialFilter
	{
		void ApplyInPlace(Bitmap image, Rectangle rect);

		void ApplyInPlace(BitmapData imageData, Rectangle rect);

		void ApplyInPlace(UnmanagedImage image, Rectangle rect);
	}
}
