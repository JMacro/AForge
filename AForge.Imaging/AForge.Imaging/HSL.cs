using System;

namespace AForge.Imaging
{
	public class HSL
	{
		public int Hue;

		public float Saturation;

		public float Luminance;

		public HSL()
		{
		}

		public HSL(int hue, float saturation, float luminance)
		{
			Hue = hue;
			Saturation = saturation;
			Luminance = luminance;
		}

		public static void FromRGB(RGB rgb, HSL hsl)
		{
			float num = (float)(int)rgb.Red / 255f;
			float num2 = (float)(int)rgb.Green / 255f;
			float num3 = (float)(int)rgb.Blue / 255f;
			float num4 = System.Math.Min(System.Math.Min(num, num2), num3);
			float num5 = System.Math.Max(System.Math.Max(num, num2), num3);
			float num6 = num5 - num4;
			hsl.Luminance = (num5 + num4) / 2f;
			if (num6 == 0f)
			{
				hsl.Hue = 0;
				hsl.Saturation = 0f;
				return;
			}
			hsl.Saturation = (((double)hsl.Luminance <= 0.5) ? (num6 / (num5 + num4)) : (num6 / (2f - num5 - num4)));
			float num7 = (num == num5) ? ((num2 - num3) / 6f / num6) : ((num2 != num5) ? (2f / 3f + (num - num2) / 6f / num6) : (0.333333343f + (num3 - num) / 6f / num6));
			if (num7 < 0f)
			{
				num7 += 1f;
			}
			if (num7 > 1f)
			{
				num7 -= 1f;
			}
			hsl.Hue = (int)(num7 * 360f);
		}

		public static HSL FromRGB(RGB rgb)
		{
			HSL hSL = new HSL();
			FromRGB(rgb, hSL);
			return hSL;
		}

		public static void ToRGB(HSL hsl, RGB rgb)
		{
			if (hsl.Saturation == 0f)
			{
				rgb.Red = (rgb.Green = (rgb.Blue = (byte)(hsl.Luminance * 255f)));
			}
			else
			{
				float num = (float)hsl.Hue / 360f;
				float num2 = ((double)hsl.Luminance < 0.5) ? (hsl.Luminance * (1f + hsl.Saturation)) : (hsl.Luminance + hsl.Saturation - hsl.Luminance * hsl.Saturation);
				float v = 2f * hsl.Luminance - num2;
				rgb.Red = (byte)(255f * Hue_2_RGB(v, num2, num + 0.333333343f));
				rgb.Green = (byte)(255f * Hue_2_RGB(v, num2, num));
				rgb.Blue = (byte)(255f * Hue_2_RGB(v, num2, num - 0.333333343f));
			}
			rgb.Alpha = byte.MaxValue;
		}

		public RGB ToRGB()
		{
			RGB rGB = new RGB();
			ToRGB(this, rGB);
			return rGB;
		}

		private static float Hue_2_RGB(float v1, float v2, float vH)
		{
			if (vH < 0f)
			{
				vH += 1f;
			}
			if (vH > 1f)
			{
				vH -= 1f;
			}
			if (6f * vH < 1f)
			{
				return v1 + (v2 - v1) * 6f * vH;
			}
			if (2f * vH < 1f)
			{
				return v2;
			}
			if (3f * vH < 2f)
			{
				return v1 + (v2 - v1) * (2f / 3f - vH) * 6f;
			}
			return v1;
		}
	}
}
