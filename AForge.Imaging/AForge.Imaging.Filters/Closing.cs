using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Closing : IFilter, IInPlaceFilter, IInPlacePartialFilter, IFilterInformation
	{
		private Erosion errosion = new Erosion();

		private Dilatation dilatation = new Dilatation();

		public Dictionary<PixelFormat, PixelFormat> FormatTranslations => errosion.FormatTranslations;

		public Closing()
		{
		}

		public Closing(short[,] se)
		{
			errosion = new Erosion(se);
			dilatation = new Dilatation(se);
		}

		public Bitmap Apply(Bitmap image)
		{
			Bitmap bitmap = dilatation.Apply(image);
			Bitmap result = errosion.Apply(bitmap);
			bitmap.Dispose();
			return result;
		}

		public Bitmap Apply(BitmapData imageData)
		{
			Bitmap bitmap = dilatation.Apply(imageData);
			Bitmap result = errosion.Apply(bitmap);
			bitmap.Dispose();
			return result;
		}

		public UnmanagedImage Apply(UnmanagedImage image)
		{
			UnmanagedImage unmanagedImage = dilatation.Apply(image);
			errosion.ApplyInPlace(unmanagedImage);
			return unmanagedImage;
		}

		public void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
		{
			dilatation.Apply(sourceImage, destinationImage);
			errosion.ApplyInPlace(destinationImage);
		}

		public void ApplyInPlace(Bitmap image)
		{
			dilatation.ApplyInPlace(image);
			errosion.ApplyInPlace(image);
		}

		public void ApplyInPlace(BitmapData imageData)
		{
			dilatation.ApplyInPlace(imageData);
			errosion.ApplyInPlace(imageData);
		}

		public void ApplyInPlace(UnmanagedImage image)
		{
			dilatation.ApplyInPlace(image);
			errosion.ApplyInPlace(image);
		}

		public void ApplyInPlace(Bitmap image, Rectangle rect)
		{
			dilatation.ApplyInPlace(image, rect);
			errosion.ApplyInPlace(image, rect);
		}

		public void ApplyInPlace(BitmapData imageData, Rectangle rect)
		{
			dilatation.ApplyInPlace(imageData, rect);
			errosion.ApplyInPlace(imageData, rect);
		}

		public void ApplyInPlace(UnmanagedImage image, Rectangle rect)
		{
			dilatation.ApplyInPlace(image, rect);
			errosion.ApplyInPlace(image, rect);
		}
	}
}
