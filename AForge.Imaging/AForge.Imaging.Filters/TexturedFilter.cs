using AForge.Imaging.Textures;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
	public class TexturedFilter : BaseFilter
	{
		private ITextureGenerator textureGenerator;

		private float[,] texture;

		private IFilter filter1;

		private IFilter filter2;

		private double filterLevel = 1.0;

		private double preserveLevel;

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

		public IFilter Filter1
		{
			get
			{
				return filter1;
			}
			set
			{
				if (value is IFilterInformation)
				{
					IFilterInformation filterInformation = (IFilterInformation)value;
					if (!filterInformation.FormatTranslations.ContainsKey(PixelFormat.Format24bppRgb))
					{
						throw new UnsupportedImageFormatException("The specified filter does not support 24 bpp color images.");
					}
					if (filterInformation.FormatTranslations[PixelFormat.Format24bppRgb] != PixelFormat.Format24bppRgb && filterInformation.FormatTranslations[PixelFormat.Format24bppRgb] != PixelFormat.Format8bppIndexed)
					{
						throw new UnsupportedImageFormatException("The specified filter does not produce image of supported format.");
					}
					filter1 = value;
					return;
				}
				throw new ArgumentException("The specified filter does not implement IFilterInformation interface.");
			}
		}

		public IFilter Filter2
		{
			get
			{
				return filter2;
			}
			set
			{
				if (value is IFilterInformation)
				{
					IFilterInformation filterInformation = (IFilterInformation)value;
					if (!filterInformation.FormatTranslations.ContainsKey(PixelFormat.Format24bppRgb))
					{
						throw new UnsupportedImageFormatException("The specified filter does not support 24 bpp color images.");
					}
					if (filterInformation.FormatTranslations[PixelFormat.Format24bppRgb] != PixelFormat.Format24bppRgb && filterInformation.FormatTranslations[PixelFormat.Format24bppRgb] != PixelFormat.Format8bppIndexed)
					{
						throw new UnsupportedImageFormatException("The specified filter does not produce image of supported format.");
					}
					filter2 = value;
					return;
				}
				throw new ArgumentException("The specified filter does not implement IFilterInformation interface.");
			}
		}

		private TexturedFilter()
		{
			formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
		}

		public TexturedFilter(float[,] texture, IFilter filter1)
			: this()
		{
			this.texture = texture;
			this.filter1 = filter1;
		}

		public TexturedFilter(float[,] texture, IFilter filter1, IFilter filter2)
			: this()
		{
			this.texture = texture;
			this.filter1 = filter1;
			this.filter2 = filter2;
		}

		public TexturedFilter(ITextureGenerator generator, IFilter filter1)
			: this()
		{
			textureGenerator = generator;
			this.filter1 = filter1;
		}

		public TexturedFilter(ITextureGenerator generator, IFilter filter1, IFilter filter2)
			: this()
		{
			textureGenerator = generator;
			this.filter1 = filter1;
			this.filter2 = filter2;
		}

		protected unsafe override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			if (textureGenerator != null)
			{
				texture = textureGenerator.Generate(width, height);
			}
			else if (texture.GetLength(0) != height || texture.GetLength(1) != width)
			{
				throw new InvalidImagePropertiesException("Texture size does not match image size.");
			}
			UnmanagedImage unmanagedImage = filter1.Apply(sourceData);
			if (width != unmanagedImage.Width || height != unmanagedImage.Height)
			{
				unmanagedImage.Dispose();
				throw new ApplicationException("Filters should not change image dimension.");
			}
			if (unmanagedImage.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				GrayscaleToRGB grayscaleToRGB = new GrayscaleToRGB();
				UnmanagedImage unmanagedImage2 = grayscaleToRGB.Apply(unmanagedImage);
				unmanagedImage.Dispose();
				unmanagedImage = unmanagedImage2;
			}
			UnmanagedImage unmanagedImage3 = null;
			if (filter2 != null)
			{
				unmanagedImage3 = filter2.Apply(sourceData);
				if (width != unmanagedImage3.Width || height != unmanagedImage3.Height)
				{
					unmanagedImage.Dispose();
					unmanagedImage3.Dispose();
					throw new ApplicationException("Filters should not change image dimension.");
				}
				if (unmanagedImage3.PixelFormat == PixelFormat.Format8bppIndexed)
				{
					GrayscaleToRGB grayscaleToRGB2 = new GrayscaleToRGB();
					UnmanagedImage unmanagedImage4 = grayscaleToRGB2.Apply(unmanagedImage3);
					unmanagedImage3.Dispose();
					unmanagedImage3 = unmanagedImage4;
				}
			}
			if (unmanagedImage3 == null)
			{
				unmanagedImage3 = sourceData;
			}
			byte* ptr = (byte*)destinationData.ImageData.ToPointer();
			byte* ptr2 = (byte*)unmanagedImage.ImageData.ToPointer();
			byte* ptr3 = (byte*)unmanagedImage3.ImageData.ToPointer();
			int num = destinationData.Stride - 3 * width;
			int num2 = unmanagedImage.Stride - 3 * width;
			int num3 = unmanagedImage3.Stride - 3 * width;
			if (preserveLevel != 0.0)
			{
				for (int i = 0; i < height; i++)
				{
					for (int j = 0; j < width; j++)
					{
						double num4 = texture[i, j];
						double num5 = 1.0 - num4;
						int num6 = 0;
						while (num6 < 3)
						{
							*ptr = (byte)System.Math.Min(255.0, filterLevel * (num4 * (double)(int)(*ptr2) + num5 * (double)(int)(*ptr3)) + preserveLevel * (double)(int)(*ptr3));
							num6++;
							ptr2++;
							ptr3++;
							ptr++;
						}
					}
					ptr2 += num2;
					ptr3 += num3;
					ptr += num;
				}
			}
			else
			{
				for (int k = 0; k < height; k++)
				{
					for (int l = 0; l < width; l++)
					{
						double num7 = texture[k, l];
						double num8 = 1.0 - num7;
						int num9 = 0;
						while (num9 < 3)
						{
							*ptr = (byte)System.Math.Min(255.0, num7 * (double)(int)(*ptr2) + num8 * (double)(int)(*ptr3));
							num9++;
							ptr2++;
							ptr3++;
							ptr++;
						}
					}
					ptr2 += num2;
					ptr3 += num3;
					ptr += num;
				}
			}
			unmanagedImage.Dispose();
			if (unmanagedImage3 != sourceData)
			{
				unmanagedImage3.Dispose();
			}
		}
	}
}
