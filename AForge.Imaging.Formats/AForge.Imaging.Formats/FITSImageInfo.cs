using System.ComponentModel;

namespace AForge.Imaging.Formats
{
	public sealed class FITSImageInfo : ImageInfo
	{
		private int originalBitsPerPixl;

		private double minDataValue;

		private double maxDataValue;

		private string telescope;

		private string acquiredObject;

		private string observer;

		private string instrument;

		[Category("FITS Info")]
		public int OriginalBitsPerPixl
		{
			get
			{
				return originalBitsPerPixl;
			}
			set
			{
				originalBitsPerPixl = value;
			}
		}

		[Category("FITS Info")]
		public double MinDataValue
		{
			get
			{
				return minDataValue;
			}
			set
			{
				minDataValue = value;
			}
		}

		[Category("FITS Info")]
		public double MaxDataValue
		{
			get
			{
				return maxDataValue;
			}
			set
			{
				maxDataValue = value;
			}
		}

		[Category("FITS Info")]
		public string Telescope
		{
			get
			{
				return telescope;
			}
			set
			{
				telescope = value;
			}
		}

		[Category("FITS Info")]
		public string Object
		{
			get
			{
				return acquiredObject;
			}
			set
			{
				acquiredObject = value;
			}
		}

		[Category("FITS Info")]
		public string Observer
		{
			get
			{
				return observer;
			}
			set
			{
				observer = value;
			}
		}

		[Category("FITS Info")]
		public string Instrument
		{
			get
			{
				return instrument;
			}
			set
			{
				instrument = value;
			}
		}

		public FITSImageInfo()
		{
		}

		public FITSImageInfo(int width, int height, int bitsPerPixel, int frameIndex, int totalFrames)
			: base(width, height, bitsPerPixel, frameIndex, totalFrames)
		{
		}

		public override object Clone()
		{
			FITSImageInfo fITSImageInfo = new FITSImageInfo(width, height, bitsPerPixel, frameIndex, totalFrames);
			fITSImageInfo.originalBitsPerPixl = originalBitsPerPixl;
			fITSImageInfo.minDataValue = minDataValue;
			fITSImageInfo.maxDataValue = maxDataValue;
			fITSImageInfo.telescope = telescope;
			fITSImageInfo.acquiredObject = acquiredObject;
			fITSImageInfo.observer = observer;
			fITSImageInfo.instrument = instrument;
			return fITSImageInfo;
		}
	}
}
