using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Opening : IFilter, IInPlaceFilter, IInPlacePartialFilter, IFilterInformation
	{
		private Erosion errosion = new Erosion();

		private Dilatation dilatation = new Dilatation();

		public Dictionary<PixelFormat, PixelFormat> FormatTranslations => errosion.FormatTranslations;

		public Opening()
		{
		}

		public Opening(short[,] se)
		{
			errosion = new Erosion(se);
			dilatation = new Dilatation(se);
		}

		public Bitmap Apply(Bitmap image)
		{
			Bitmap bitmap = errosion.Apply(image);
			Bitmap result = dilatation.Apply(bitmap);
			bitmap.Dispose();
			return result;
		}

		public Bitmap Apply(BitmapData imageData)
		{
			Bitmap bitmap = errosion.Apply(imageData);
			Bitmap result = dilatation.Apply(bitmap);
			bitmap.Dispose();
			return result;
		}

		public UnmanagedImage Apply(UnmanagedImage image)
		{
			UnmanagedImage unmanagedImage = errosion.Apply(image);
			dilatation.ApplyInPlace(unmanagedImage);
			return unmanagedImage;
		}

		public void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
		{
			errosion.Apply(sourceImage, destinationImage);
			dilatation.ApplyInPlace(destinationImage);
		}

		public void ApplyInPlace(Bitmap image)
		{
			errosion.ApplyInPlace(image);
			dilatation.ApplyInPlace(image);
		}

		public void ApplyInPlace(BitmapData imageData)
		{
			errosion.ApplyInPlace(imageData);
			dilatation.ApplyInPlace(imageData);
		}

		public void ApplyInPlace(UnmanagedImage image)
		{
			errosion.ApplyInPlace(image);
			dilatation.ApplyInPlace(image);
		}

		public void ApplyInPlace(Bitmap image, Rectangle rect)
		{
			errosion.ApplyInPlace(image, rect);
			dilatation.ApplyInPlace(image, rect);
		}

		public void ApplyInPlace(BitmapData imageData, Rectangle rect)
		{
			errosion.ApplyInPlace(imageData, rect);
			dilatation.ApplyInPlace(imageData, rect);
		}

		public void ApplyInPlace(UnmanagedImage image, Rectangle rect)
		{
			errosion.ApplyInPlace(image, rect);
			dilatation.ApplyInPlace(image, rect);
		}
	}
}
