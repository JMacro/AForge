using System.Drawing;
using System.IO;

namespace AForge.Imaging.Formats
{
	public interface IImageDecoder
	{
		Bitmap DecodeSingleFrame(Stream stream);

		int Open(Stream stream);

		Bitmap DecodeFrame(int frameIndex, out ImageInfo imageInfo);

		void Close();
	}
}
