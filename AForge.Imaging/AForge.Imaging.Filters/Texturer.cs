using AForge.Imaging.Textures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class Texturer : BaseInPlacePartialFilter
	{
		private ITextureGenerator textureGenerator;

		private float[,] texture;

		private double filterLevel = 0.5;

		private double preserveLevel = 0.5;

		private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

		public override Dictionary<PixelFormat, PixelFormat> FormatTranslations => formatTranslations;

		public double FilterLevel
		{
			get
			{
				return filterLevel;
			}
			set
			{
				filterLevel = System.Math.Max(0.0, System.Math.Min(1.0, value));
			}
		}

		public double PreserveLevel
		{
			get
			{
				return preserveLevel;
			}
			set
			{
				preserveLevel = System.Math.Max(0.0, System.Math.Min(1.0, value));
			}
		}

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

		private Texturer()
		{
			formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		public Texturer(float[,] texture)
			: this()
		{
			this.texture = texture;
		}

		public Texturer(float[,] texture, double filterLevel, double preserveLevel)
			: this()
		{
			this.texture = texture;
			this.filterLevel = filterLevel;
			this.preserveLevel = preserveLevel;
		}

		public Texturer(ITextureGenerator generator)
			: this()
		{
			textureGenerator = generator;
		}

		public Texturer(ITextureGenerator generator, double filterLevel, double preserveLevel)
			: this()
		{
			textureGenerator = generator;
			this.filterLevel = filterLevel;
			this.preserveLevel = preserveLevel;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage image, Rectangle rect)
		{
			int num = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
			int width = rect.Width;
			int height = rect.Height;
			int num2 = width;
			int num3 = height;
			if (textureGenerator != null)
			{
				texture = textureGenerator.Generate(width, height);
			}
			else
			{
				num2 = System.Math.Min(width, texture.GetLength(1));
				num3 = System.Math.Min(height, texture.GetLength(0));
			}
			int num4 = image.Stride - num2 * num;
			byte* ptr = (byte*)image.ImageData.ToPointer();
			ptr += rect.Top * image.Stride + rect.Left * num;
			for (int i = 0; i < num3; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					double num5 = texture[i, j];
					int num6 = 0;
					while (num6 < num)
					{
						*ptr = (byte)System.Math.Min(255.0, preserveLevel * (double)(int)(*ptr) + filterLevel * (double)(int)(*ptr) * num5);
						num6++;
						ptr++;
					}
				}
				ptr += num4;
			}
		}
	}
}
