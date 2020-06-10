using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public interface IInPlaceFilter
	{
		void ApplyInPlace(Bitmap image);

		void ApplyInPlace(BitmapData imageData);

		void ApplyInPlace(UnmanagedImage image);
	}
}
