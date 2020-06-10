using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace AForge.Imaging.Formats
{
	public class FITSCodec : IImageDecoder
	{
		private Stream stream;

		private FITSImageInfo imageInfo;

		private long dataPosition;

		public Bitmap DecodeSingleFrame(Stream stream)
		{
			FITSImageInfo fITSImageInfo = ReadHeader(stream);
			if (fITSImageInfo.TotalFrames == 0)
			{
				throw new ArgumentException("The FITS stream does not contain any image in main section.");
			}
			return ReadImageFrame(stream, fITSImageInfo);
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
			if (frameIndex >= this.imageInfo.TotalFrames)
			{
				throw new ArgumentOutOfRangeException("Currently opened stream does not contain frame with specified index.");
			}
			stream.Seek(dataPosition + frameIndex * this.imageInfo.Width * this.imageInfo.Height * Math.Abs(this.imageInfo.OriginalBitsPerPixl) / 8, SeekOrigin.Begin);
			Bitmap result = ReadImageFrame(stream, this.imageInfo);
			imageInfo = (FITSImageInfo)this.imageInfo.Clone();
			imageInfo.FrameIndex = frameIndex;
			return result;
		}

		public void Close()
		{
			stream = null;
			imageInfo = null;
		}

		private FITSImageInfo ReadHeader(Stream stream)
		{
			byte[] array = new byte[80];
			int num = 1;
			bool flag = false;
			FITSImageInfo fITSImageInfo = new FITSImageInfo();
			if (Tools.ReadStream(stream, array, 0, 80) < 80 || Encoding.ASCII.GetString(array, 0, 8) != "SIMPLE  ")
			{
				throw new FormatException("The stream does not contatin FITS image.");
			}
			if (Encoding.ASCII.GetString(array, 10, 70).Split('/')[0].Trim() != "T")
			{
				throw new NotSupportedException("The stream contains not standard FITS data file.");
			}
			while (true)
			{
				if (Tools.ReadStream(stream, array, 0, 80) < 80)
				{
					throw new ArgumentException("The stream does not contain valid FITS image.");
				}
				num++;
				string @string = Encoding.ASCII.GetString(array, 0, 8);
				switch (@string)
				{
				case "COMMENT ":
				case "HISTORY ":
					continue;
				case "END     ":
					flag = true;
					break;
				}
				if (flag)
				{
					if (num % 36 == 0)
					{
						if (fITSImageInfo.BitsPerPixel == 0 || fITSImageInfo.Width == 0 || fITSImageInfo.Height == 0)
						{
							fITSImageInfo.TotalFrames = 0;
						}
						break;
					}
					continue;
				}
				string string2 = Encoding.ASCII.GetString(array, 10, 70);
				if (@string == "BITPIX  ")
				{
					int num2 = ExtractIntegerValue(string2);
					if (num2 != 8 && num2 != 16 && num2 != 32 && num2 != -32 && num2 != -64)
					{
						throw new NotSupportedException("Data format (" + num2 + ") is not supported.");
					}
					fITSImageInfo.BitsPerPixel = ((num2 == 8) ? 8 : 16);
					fITSImageInfo.OriginalBitsPerPixl = num2;
				}
				else if (Encoding.ASCII.GetString(array, 0, 5) == "NAXIS")
				{
					int num3 = ExtractIntegerValue(string2);
					switch (array[5])
					{
					case 32:
						switch (num3)
						{
						case 0:
						case 3:
							break;
						default:
							throw new NotSupportedException("FITS files with data dimension of " + num3 + " are not supported.");
						case 2:
							fITSImageInfo.TotalFrames = 1;
							break;
						}
						break;
					case 49:
						fITSImageInfo.Width = num3;
						break;
					case 50:
						fITSImageInfo.Height = num3;
						break;
					case 51:
						fITSImageInfo.TotalFrames = num3;
						break;
					}
				}
				else if (@string == "TELESCOP")
				{
					fITSImageInfo.Telescope = ExtractStringValue(string2);
				}
				else if (@string == "OBJECT  ")
				{
					fITSImageInfo.Object = ExtractStringValue(string2);
				}
				else if (@string == "OBSERVER")
				{
					fITSImageInfo.Observer = ExtractStringValue(string2);
				}
				else if (@string == "INSTRUME")
				{
					fITSImageInfo.Instrument = ExtractStringValue(string2);
				}
			}
			if (!stream.CanSeek)
			{
				throw new ArgumentException("The stream must be seekable.");
			}
			long offset = stream.Seek(0L, SeekOrigin.Current);
			int num4 = fITSImageInfo.Width * (Math.Abs(fITSImageInfo.OriginalBitsPerPixl) / 8);
			int num5 = fITSImageInfo.Height * fITSImageInfo.TotalFrames;
			int originalBitsPerPixl = fITSImageInfo.OriginalBitsPerPixl;
			byte[] array2 = new byte[num4];
			byte[] array3 = new byte[8];
			double num6 = double.MaxValue;
			double num7 = double.MinValue;
			for (int i = 0; i < num5; i++)
			{
				if (Tools.ReadStream(stream, array2, 0, num4) < num4)
				{
					throw new ArgumentException("The stream does not contain valid FITS image.");
				}
				int num8 = 0;
				while (num8 < num4)
				{
					double num9 = 0.0;
					switch (originalBitsPerPixl)
					{
					case 8:
						num9 = (int)array2[num8++];
						break;
					case 16:
					{
						short num10 = 0;
						num10 = (short)((array2[num8++] << 8) | array2[num8++]);
						num9 = num10;
						break;
					}
					case 32:
						array3[3] = array2[num8++];
						array3[2] = array2[num8++];
						array3[1] = array2[num8++];
						array3[0] = array2[num8++];
						num9 = BitConverter.ToInt32(array3, 0);
						break;
					case -32:
						array3[3] = array2[num8++];
						array3[2] = array2[num8++];
						array3[1] = array2[num8++];
						array3[0] = array2[num8++];
						num9 = BitConverter.ToSingle(array3, 0);
						break;
					case -64:
						array3[7] = array2[num8++];
						array3[6] = array2[num8++];
						array3[5] = array2[num8++];
						array3[4] = array2[num8++];
						array3[3] = array2[num8++];
						array3[2] = array2[num8++];
						array3[1] = array2[num8++];
						array3[0] = array2[num8++];
						num9 = BitConverter.ToDouble(array3, 0);
						break;
					}
					if (num9 > num7)
					{
						num7 = num9;
					}
					if (num9 < num6)
					{
						num6 = num9;
					}
				}
			}
			fITSImageInfo.MaxDataValue = num7;
			fITSImageInfo.MinDataValue = num6;
			stream.Seek(offset, SeekOrigin.Begin);
			return fITSImageInfo;
		}

		private unsafe Bitmap ReadImageFrame(Stream stream, FITSImageInfo imageInfo)
		{
			int width = imageInfo.Width;
			int height = imageInfo.Height;
			Bitmap bitmap = (imageInfo.BitsPerPixel == 8) ? Tools.CreateGrayscaleImage(width, height) : new Bitmap(width, height, PixelFormat.Format16bppGrayScale);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
			int originalBitsPerPixl = imageInfo.OriginalBitsPerPixl;
			int stride = bitmapData.Stride;
			byte* ptr = (byte*)bitmapData.Scan0.ToPointer();
			double minDataValue = imageInfo.MinDataValue;
			double maxDataValue = imageInfo.MaxDataValue;
			if (imageInfo.BitsPerPixel == 16)
			{
				double num = 65535.0 / (maxDataValue - minDataValue);
				int num2 = width * Math.Abs(originalBitsPerPixl) / 8;
				byte[] array = new byte[num2];
				byte[] array2 = new byte[8];
				for (int num3 = height - 1; num3 >= 0; num3--)
				{
					if (Tools.ReadStream(stream, array, 0, num2) < num2)
					{
						throw new ArgumentException("The stream does not contain valid FITS image.");
					}
					ushort* ptr2 = (ushort*)(ptr + (long)stride * (long)num3);
					int num4 = 0;
					int num5 = 0;
					while (num4 < width)
					{
						double num6 = 0.0;
						switch (originalBitsPerPixl)
						{
						case 16:
						{
							short num7 = 0;
							num7 = (short)((array[num5++] << 8) + array[num5++]);
							num6 = num7;
							break;
						}
						case 32:
							array2[3] = array[num5++];
							array2[2] = array[num5++];
							array2[1] = array[num5++];
							array2[0] = array[num5++];
							num6 = BitConverter.ToInt32(array2, 0);
							break;
						case -32:
							array2[3] = array[num5++];
							array2[2] = array[num5++];
							array2[1] = array[num5++];
							array2[0] = array[num5++];
							num6 = BitConverter.ToSingle(array2, 0);
							break;
						case -64:
							array2[7] = array[num5++];
							array2[6] = array[num5++];
							array2[5] = array[num5++];
							array2[4] = array[num5++];
							array2[3] = array[num5++];
							array2[2] = array[num5++];
							array2[1] = array[num5++];
							array2[0] = array[num5++];
							num6 = BitConverter.ToDouble(array2, 0);
							break;
						}
						*ptr2 = (ushort)((num6 - minDataValue) * num);
						num4++;
						ptr2++;
					}
				}
			}
			else
			{
				double num8 = 255.0 / (maxDataValue - minDataValue);
				byte[] array3 = new byte[width];
				for (int num9 = height - 1; num9 >= 0; num9--)
				{
					if (Tools.ReadStream(stream, array3, 0, width) < width)
					{
						throw new ArgumentException("The stream does not contain valid FITS image.");
					}
					byte* ptr3 = ptr + (long)stride * (long)num9;
					int num10 = 0;
					while (num10 < width)
					{
						*ptr3 = (byte)(((double)(int)array3[num10] - minDataValue) * num8);
						num10++;
						ptr3++;
					}
				}
			}
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		private int ExtractIntegerValue(string strValue)
		{
			try
			{
				string[] array = strValue.Split('/');
				return int.Parse(array[0].Trim());
			}
			catch
			{
				throw new ArgumentException("The stream does not contain valid FITS image.");
			}
		}

		private string ExtractStringValue(string strValue)
		{
			string[] array = strValue.Split('/');
			return array[0].Replace("''", "``").Replace("'", "").Replace("''", "``")
				.Trim();
		}
	}
}
