using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AForge.Imaging.Formats
{
	public class ImageDecoder
	{
		private static Dictionary<string, IImageDecoder> decoders;

		static ImageDecoder()
		{
			decoders = new Dictionary<string, IImageDecoder>();
			IImageDecoder decoder = new PNMCodec();
			RegisterDecoder("pbm", decoder);
			RegisterDecoder("pgm", decoder);
			RegisterDecoder("pnm", decoder);
			RegisterDecoder("ppm", decoder);
			decoder = new FITSCodec();
			RegisterDecoder("fit", decoder);
			RegisterDecoder("fits", decoder);
		}

		public static void RegisterDecoder(string fileExtension, IImageDecoder decoder)
		{
			decoders.Add(fileExtension.ToLower(), decoder);
		}

		public static Bitmap DecodeFromFile(string fileName)
		{
			ImageInfo imageInfo = null;
			return DecodeFromFile(fileName, out imageInfo);
		}

		public static Bitmap DecodeFromFile(string fileName, out ImageInfo imageInfo)
		{
			Bitmap bitmap = null;
			string text = Path.GetExtension(fileName).ToLower();
			if (text != string.Empty && text.Length != 0)
			{
				text = text.Substring(1);
				if (decoders.ContainsKey(text))
				{
					IImageDecoder imageDecoder = decoders[text];
					FileStream fileStream = new FileStream(fileName, FileMode.Open);
					imageDecoder.Open(fileStream);
					bitmap = imageDecoder.DecodeFrame(0, out imageInfo);
					imageDecoder.Close();
					fileStream.Close();
					fileStream.Dispose();
					return bitmap;
				}
			}
			bitmap = FromFile(fileName);
			imageInfo = new ImageInfo(bitmap.Width, bitmap.Height, Image.GetPixelFormatSize(bitmap.PixelFormat), 0, 1);
			return bitmap;
		}

		private static Bitmap FromFile(string fileName)
		{
			Bitmap bitmap = null;
			FileStream fileStream = null;
			try
			{
				fileStream = File.OpenRead(fileName);
				MemoryStream memoryStream = new MemoryStream();
				byte[] buffer = new byte[10000];
				while (true)
				{
					int num = fileStream.Read(buffer, 0, 10000);
					if (num == 0)
					{
						break;
					}
					memoryStream.Write(buffer, 0, num);
				}
				return (Bitmap)Image.FromStream(memoryStream);
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream.Dispose();
				}
			}
		}
	}
}
