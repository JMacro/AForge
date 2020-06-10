using System;

namespace AForge.Imaging
{
	public class YCbCr
	{
		public const short YIndex = 0;

		public const short CbIndex = 1;

		public const short CrIndex = 2;

		public float Y;

		public float Cb;

		public float Cr;

		public YCbCr()
		{
		}

		public YCbCr(float y, float cb, float cr)
		{
			Y = System.Math.Max(0f, System.Math.Min(1f, y));
			Cb = System.Math.Max(-0.5f, System.Math.Min(0.5f, cb));
			Cr = System.Math.Max(-0.5f, System.Math.Min(0.5f, cr));
		}

		public static void FromRGB(RGB rgb, YCbCr ycbcr)
		{
			float num = (float)(int)rgb.Red / 255f;
			float num2 = (float)(int)rgb.Green / 255f;
			float num3 = (float)(int)rgb.Blue / 255f;
			ycbcr.Y = (float)(0.2989 * (double)num + 0.5866 * (double)num2 + 0.1145 * (double)num3);
			ycbcr.Cb = (float)(-0.1687 * (double)num - 0.3313 * (double)num2 + 0.5 * (double)num3);
			ycbcr.Cr = (float)(0.5 * (double)num - 0.4184 * (double)num2 - 0.0816 * (double)num3);
		}

		public static YCbCr FromRGB(RGB rgb)
		{
			YCbCr yCbCr = new YCbCr();
			FromRGB(rgb, yCbCr);
			return yCbCr;
		}

		public static void ToRGB(YCbCr ycbcr, RGB rgb)
		{
			float num = System.Math.Max(0f, System.Math.Min(1f, (float)((double)ycbcr.Y + 0.0 * (double)ycbcr.Cb + 1.4022 * (double)ycbcr.Cr)));
			float num2 = System.Math.Max(0f, System.Math.Min(1f, (float)((double)ycbcr.Y - 0.3456 * (double)ycbcr.Cb - 0.7145 * (double)ycbcr.Cr)));
			float num3 = System.Math.Max(0f, System.Math.Min(1f, (float)((double)ycbcr.Y + 1.771 * (double)ycbcr.Cb + 0.0 * (double)ycbcr.Cr)));
			rgb.Red = (byte)(num * 255f);
			rgb.Green = (byte)(num2 * 255f);
			rgb.Blue = (byte)(num3 * 255f);
			rgb.Alpha = byte.MaxValue;
		}

		public RGB ToRGB()
		{
			RGB rGB = new RGB();
			ToRGB(this, rGB);
			return rGB;
		}
	}
}
