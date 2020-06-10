using AForge.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace AForge.Controls
{
	public class PictureBox : System.Windows.Forms.PictureBox
	{
		private System.Drawing.Image sourceImage;

		private System.Drawing.Image convertedImage;

		public new System.Drawing.Image Image
		{
			get
			{
				return sourceImage;
			}
			set
			{
				if (value != null && value is Bitmap && (value.PixelFormat == PixelFormat.Format16bppGrayScale || value.PixelFormat == PixelFormat.Format48bppRgb || value.PixelFormat == PixelFormat.Format64bppArgb))
				{
					System.Drawing.Image image2 = base.Image = AForge.Imaging.Image.Convert16bppTo8bpp((Bitmap)value);
					if (convertedImage != null)
					{
						convertedImage.Dispose();
					}
					convertedImage = image2;
				}
				else
				{
					base.Image = value;
				}
				sourceImage = value;
			}
		}
	}
}
