using System.ComponentModel;

namespace AForge.Imaging.Formats
{
	public sealed class PNMImageInfo : ImageInfo
	{
		private int version;

		private int maxDataValue;

		[Category("PNM Info")]
		public int Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

		[Category("PNM Info")]
		public int MaxDataValue
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

		public PNMImageInfo()
		{
		}

		public PNMImageInfo(int width, int height, int bitsPerPixel, int frameIndex, int totalFrames)
			: base(width, height, bitsPerPixel, frameIndex, totalFrames)
		{
		}

		public override object Clone()
		{
			PNMImageInfo pNMImageInfo = new PNMImageInfo(width, height, bitsPerPixel, frameIndex, totalFrames);
			pNMImageInfo.version = version;
			pNMImageInfo.maxDataValue = maxDataValue;
			return pNMImageInfo;
		}
	}
}
