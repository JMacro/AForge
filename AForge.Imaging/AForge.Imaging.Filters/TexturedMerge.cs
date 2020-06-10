using AForge.Imaging.Textures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class TexturedMerge : BaseInPlaceFilter2
	{
		private ITextureGenerator textureGenerator;

		private float[,] texture;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public float[,] Texture
		{
			get
			{
				return texture;
			}
			set
			{
				texture = value;
			}
		}

		public ITextureGenerator TextureGenerator
		{
			get
			{
				return textureGenerator;
			}
			set
			{
				textureGenerator = value;
			}
		}

		private TexturedMerge()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		public TexturedMerge(float[,] texture)
			: this()
		{
			this.texture = texture;
		}

		public TexturedMerge(ITextureGenerator generator)
			: this()
		{
			textureGenerator = generator;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, UnmanagedImage overlay)
		{
			int width = image.Width;
			int height = image.Height;
			int num = width;
			int num2 = height;
			if (textureGenerator != null)
			{
				texture = textureGenerator.Generate(width, height);
			}
			else
			{
				num = System.Math.Min(width, texture.GetLength(1));
				num2 = System.Math.Min(height, texture.GetLength(0));
			}
			int num3 = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int num4 = image.Stride - num * num3;
			int num5 = overlay.Stride - num * num3;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			byte* ptr2 = (byte*)overlay.ImageData.ToPointer();
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					double num6 = texture[i, j];
					double num7 = 1.0 - num6;
					int num8 = 0;
					while (num8 < num3)
					{
						*ptr = (byte)System.Math.Min(255.0, (double)(int)(*ptr) * num6 + (double)(int)(*ptr2) * num7);
						num8++;
						ptr++;
						ptr2++;
					}
				}
				ptr += num4;
				ptr2 += num5;
			}
		}
	}
}
