using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class FilterIterator : IFilter, IFilterInformation
	{
		private IFilter baseFilter;

		private int iterations = 1;

		public Dictionary<PixelFormat, PixelFormat> FormatTranslations => ((IFilterInformation)baseFilter).FormatTranslations;

		public IFilter BaseFilter
		{
			get
			{
				return baseFilter;
			}
			set
			{
				baseFilter = value;
			}
		}

		public int Iterations
		{
			get
			{
				return iterations;
			}
			set
			{
				iterations = System.Math.Max(1, System.Math.Min(255, value));
			}
		}

		public FilterIterator(IFilter baseFilter)
		{
			this.baseFilter = baseFilter;
		}

		public FilterIterator(IFilter baseFilter, int iterations)
		{
			this.baseFilter = baseFilter;
			this.iterations = iterations;
		}

		public Bitmap Apply(Bitmap image)
		{
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			Bitmap result = Apply(bitmapData);
			image.UnlockBits(bitmapData);
			return result;
		}

		public Bitmap Apply(BitmapData imageData)
		{
			Bitmap bitmap = baseFilter.Apply(imageData);
			for (int i = 1; i < iterations; i++)
			{
				Bitmap bitmap2 = bitmap;
				bitmap = baseFilter.Apply(bitmap2);
				bitmap2.Dispose();
			}
			return bitmap;
		}

		public UnmanagedImage Apply(UnmanagedImage image)
		{
			UnmanagedImage unmanagedImage = baseFilter.Apply(image);
			for (int i = 1; i < iterations; i++)
			{
				UnmanagedImage unmanagedImage2 = unmanagedImage;
				unmanagedImage = baseFilter.Apply(unmanagedImage2);
				unmanagedImage2.Dispose();
			}
			return unmanagedImage;
		}

		public void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
		{
			if (iterations == 1)
			{
				baseFilter.Apply(sourceImage, destinationImage);
				return;
			}
			UnmanagedImage unmanagedImage = baseFilter.Apply(sourceImage);
			iterations--;
			for (int i = 1; i < iterations; i++)
			{
				UnmanagedImage unmanagedImage2 = unmanagedImage;
				unmanagedImage = baseFilter.Apply(unmanagedImage2);
				unmanagedImage2.Dispose();
			}
			baseFilter.Apply(unmanagedImage, destinationImage);
		}
	}
}
