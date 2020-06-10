using System;
using System.ComponentModel;

namespace AForge.Imaging.Formats
{
	public class ImageInfo : ICloneable
	{
		protected int width;

		protected int height;

		protected int bitsPerPixel;

		protected int frameIndex;

		protected int totalFrames;

		[Category("General")]
		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		[Category("General")]
		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

		[Category("General")]
		public int BitsPerPixel
		{
			get
			{
				return bitsPerPixel;
			}
			set
			{
				bitsPerPixel = value;
			}
		}

		[Category("General")]
		public int FrameIndex
		{
			get
			{
				return frameIndex;
			}
			set
			{
				frameIndex = value;
			}
		}

		[Category("General")]
		public int TotalFrames
		{
			get
			{
				return totalFrames;
			}
			set
			{
				totalFrames = value;
			}
		}

		public ImageInfo()
		{
		}

		public ImageInfo(int width, int height, int bitsPerPixel, int frameIndex, int totalFrames)
		{
			this.width = width;
			this.height = height;
			this.bitsPerPixel = bitsPerPixel;
			this.frameIndex = frameIndex;
			this.totalFrames = totalFrames;
		}

		public virtual object Clone()
		{
			return new ImageInfo(width, height, bitsPerPixel, frameIndex, totalFrames);
		}
	}
}
