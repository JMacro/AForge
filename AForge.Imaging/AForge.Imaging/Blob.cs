using System.ComponentModel;
using System.Drawing;

namespace AForge.Imaging
{
	public class Blob
	{
		private UnmanagedImage image;

		private bool originalSize;

		private Rectangle rect;

		private int id;

		private int area;

		private Point cog;

		private double fullness;

		private Color colorMean = Color.Black;

		private Color colorStdDev = Color.Black;

		[Browsable(false)]
		public UnmanagedImage Image
		{
			get
			{
				return image;
			}
			internal set
			{
				image = value;
			}
		}

		[Browsable(false)]
		public bool OriginalSize
		{
			get
			{
				return originalSize;
			}
			internal set
			{
				originalSize = value;
			}
		}

		public Rectangle Rectangle => rect;

		[Browsable(false)]
		public int ID
		{
			get
			{
				return id;
			}
			internal set
			{
				id = value;
			}
		}

		public int Area
		{
			get
			{
				return area;
			}
			internal set
			{
				area = value;
			}
		}

		public double Fullness
		{
			get
			{
				return fullness;
			}
			internal set
			{
				fullness = value;
			}
		}

		public Point CenterOfGravity
		{
			get
			{
				return cog;
			}
			internal set
			{
				cog = value;
			}
		}

		public Color ColorMean
		{
			get
			{
				return colorMean;
			}
			internal set
			{
				colorMean = value;
			}
		}

		public Color ColorStdDev
		{
			get
			{
				return colorStdDev;
			}
			internal set
			{
				colorStdDev = value;
			}
		}

		public Blob(int id, Rectangle rect)
		{
			this.id = id;
			this.rect = rect;
		}

		public Blob(Blob source)
		{
			id = source.id;
			rect = source.rect;
			cog = source.cog;
			area = source.area;
			fullness = source.fullness;
			colorMean = source.colorMean;
			colorStdDev = source.colorStdDev;
		}
	}
}
