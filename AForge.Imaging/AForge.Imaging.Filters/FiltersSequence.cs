using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class FiltersSequence : CollectionBase, IFilter
	{
		public IFilter this[int index] => (IFilter)base.InnerList[index];

		public FiltersSequence()
		{
		}

		public FiltersSequence(params IFilter[] filters)
		{
			base.InnerList.AddRange(filters);
		}

		public void Add(IFilter filter)
		{
			base.InnerList.Add(filter);
		}

		public Bitmap Apply(Bitmap image)
		{
			Bitmap bitmap = null;
			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			try
			{
				return Apply(bitmapData);
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public Bitmap Apply(BitmapData imageData)
		{
			UnmanagedImage unmanagedImage = Apply(new UnmanagedImage(imageData));
			Bitmap result = unmanagedImage.ToManagedImage();
			unmanagedImage.Dispose();
			return result;
		}

		public UnmanagedImage Apply(UnmanagedImage image)
		{
			int count = base.InnerList.Count;
			if (count == 0)
			{
				throw new ApplicationException("No filters in the sequence.");
			}
			UnmanagedImage unmanagedImage = null;
			UnmanagedImage unmanagedImage2 = null;
			unmanagedImage = ((IFilter)base.InnerList[0]).Apply(image);
			for (int i = 1; i < count; i++)
			{
				unmanagedImage2 = unmanagedImage;
				unmanagedImage = ((IFilter)base.InnerList[i]).Apply(unmanagedImage2);
				unmanagedImage2.Dispose();
			}
			return unmanagedImage;
		}

		public void Apply(UnmanagedImage sourceImage, UnmanagedImage destinationImage)
		{
			int count = base.InnerList.Count;
			switch (count)
			{
			case 0:
				throw new ApplicationException("No filters in the sequence.");
			case 1:
				((IFilter)base.InnerList[0]).Apply(sourceImage, destinationImage);
				return;
			}
			UnmanagedImage unmanagedImage = null;
			UnmanagedImage unmanagedImage2 = null;
			unmanagedImage = ((IFilter)base.InnerList[0]).Apply(sourceImage);
			count--;
			for (int i = 1; i < count; i++)
			{
				unmanagedImage2 = unmanagedImage;
				unmanagedImage = ((IFilter)base.InnerList[i]).Apply(unmanagedImage2);
				unmanagedImage2.Dispose();
			}
			((IFilter)base.InnerList[count]).Apply(unmanagedImage, destinationImage);
		}
	}
}
