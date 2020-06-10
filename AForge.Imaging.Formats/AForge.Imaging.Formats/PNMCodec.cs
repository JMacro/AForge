using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace AForge.Imaging.Formats
{
	public class PNMCodec : IImageDecoder
	{
		private Stream stream;

		private PNMImageInfo imageInfo;

		private long dataPosition;

		public Bitmap DecodeSingleFrame(Stream stream)
		{
			PNMImageInfo pNMImageInfo = ReadHeader(stream);
			return ReadImageFrame(stream, pNMImageInfo);
		}

		public int Open(Stream stream)
		{
			Close();
			imageInfo = ReadHeader(stream);
			this.stream = stream;
			dataPosition = stream.Seek(0L, SeekOrigin.Current);
			return imageInfo.TotalFrames;
		}

		public Bitmap DecodeFrame(int frameIndex, out ImageInfo imageInfo)
		{
			if (frameIndex != 0)
			{
				throw new ArgumentOutOfRangeException("Currently opened stream does not contain frame with specified index.");
			}
			stream.Seek(dataPosition, SeekOrigin.Begin);
			Bitmap result = ReadImageFrame(stream, this.imageInfo);
			imageInfo = (PNMImageInfo)this.imageInfo.Clone();
			return result;
		}

		public void Close()
		{
			stream = null;
			imageInfo = null;
		}

		private PNMImageInfo ReadHeader(Stream stream)
		{
			byte b = (byte)stream.ReadByte();
			byte b2 = (byte)stream.ReadByte();
			if (b == 80)
			{
				switch (b2)
				{
				case 49:
				case 50:
				case 51:
				case 52:
					throw new NotSupportedException("Format is not supported yet. Only P5 and P6 are supported for now.");
				case 53:
				case 54:
				{
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					try
					{
						num = ReadIntegerValue(stream);
						num2 = ReadIntegerValue(stream);
						num3 = ReadIntegerValue(stream);
					}
					catch
					{
						throw new ArgumentException("The stream does not contain valid PNM image.");
					}
					if (num <= 0 || num2 <= 0 || num3 <= 0)
					{
						throw new ArgumentException("The stream does not contain valid PNM image.");
					}
					if (num3 > 255)
					{
						throw new NotSupportedException("255 is the maximum pixel's value, which is supported for now.");
					}
					PNMImageInfo pNMImageInfo = new PNMImageInfo(num, num2, (b2 == 53) ? 8 : 24, 0, 1);
					pNMImageInfo.Version = b2 - 48;
					pNMImageInfo.MaxDataValue = num3;
					return pNMImageInfo;
				}
				}
			}
			throw new FormatException("The stream does not contain PNM image.");
		}

		private Bitmap ReadImageFrame(Stream stream, PNMImageInfo imageInfo)
		{
			try
			{
				switch (imageInfo.Version)
				{
				case 5:
					return ReadP5Image(stream, imageInfo.Width, imageInfo.Height, imageInfo.MaxDataValue);
				case 6:
					return ReadP6Image(stream, imageInfo.Width, imageInfo.Height, imageInfo.MaxDataValue);
				}
			}
			catch
			{
				throw new ArgumentException("The stream does not contain valid PNM image.");
			}
			return null;
		}

		private unsafe Bitmap ReadP5Image(Stream stream, int width, int height, int maxValue)
		{
			double num = 255.0 / (double)maxValue;
			Bitmap bitmap = Tools.CreateGrayscaleImage(width, height);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
			int stride = bitmapData.Stride;
			byte* ptr = (byte*)bitmapData.Scan0.ToPointer();
			byte[] array = new byte[width];
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < height; i++)
			{
				num2 = 0;
				num3 = 0;
				for (; num2 != width; num2 += num3)
				{
					num3 = stream.Read(array, num2, width - num2);
					if (num3 == 0)
					{
						throw new Exception();
					}
				}
				byte* ptr2 = ptr + (long)stride * (long)i;
				int num4 = 0;
				while (num4 < width)
				{
					*ptr2 = (byte)(num * (double)(int)array[num4]);
					num4++;
					ptr2++;
				}
			}
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		private unsafe Bitmap ReadP6Image(Stream stream, int width, int height, int maxValue)
		{
			double num = 255.0 / (double)maxValue;
			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
			int stride = bitmapData.Stride;
			byte* ptr = (byte*)bitmapData.Scan0.ToPointer();
			int num2 = width * 3;
			byte[] array = new byte[num2];
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < height; i++)
			{
				num3 = 0;
				num4 = 0;
				for (; num3 != num2; num3 += num4)
				{
					num4 = stream.Read(array, num3, num2 - num3);
					if (num4 == 0)
					{
						throw new Exception();
					}
				}
				byte* ptr2 = ptr + (long)stride * (long)i;
				int num5 = 0;
				int num6 = 0;
				while (num5 < width)
				{
					ptr2[2] = (byte)(num * (double)(int)array[num6]);
					ptr2[1] = (byte)(num * (double)(int)array[num6 + 1]);
					*ptr2 = (byte)(num * (double)(int)array[num6 + 2]);
					num5++;
					num6 += 3;
					ptr2 += 3;
				}
			}
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		private int ReadIntegerValue(Stream stream)
		{
			byte[] array = new byte[256];
			int num = 1;
			array[0] = SkipSpaces(stream);
			num += ReadUntilSpace(stream, array, 1);
			return int.Parse(Encoding.ASCII.GetString(array, 0, num));
		}

		private byte SkipSpaces(Stream stream)
		{
			byte b = (byte)stream.ReadByte();
			while (true)
			{
				switch (b)
				{
				case 9:
				case 10:
				case 13:
				case 32:
					break;
				case 35:
					while (b != 10)
					{
						b = (byte)stream.ReadByte();
					}
					return SkipSpaces(stream);
				default:
					return b;
				}
				b = (byte)stream.ReadByte();
			}
		}

		private int ReadUntilSpace(Stream stream, byte[] buffer, int start)
		{
			byte b = (byte)stream.ReadByte();
			int num = 0;
			while (b != 32 && b != 10 && b != 13 && b != 9 && b != 35)
			{
				buffer[start + num] = b;
				num++;
				b = (byte)stream.ReadByte();
			}
			return num;
		}
	}
}
