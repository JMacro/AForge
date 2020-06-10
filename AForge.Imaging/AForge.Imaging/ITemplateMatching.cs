using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging
{
	public interface ITemplateMatching
	{
		TemplateMatch[] ProcessImage(Bitmap image, Bitmap template, Rectangle searchZone);

		TemplateMatch[] ProcessImage(BitmapData imageData, BitmapData templateData, Rectangle searchZone);

		TemplateMatch[] ProcessImage(UnmanagedImage image, UnmanagedImage template, Rectangle searchZone);
	}
}
