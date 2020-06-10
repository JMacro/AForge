using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace AForge.Controls
{
	public class ColorSlider : Control
	{
		public enum ColorSliderType
		{
			Gradient,
			InnerGradient,
			OuterGradient,
			Threshold
		}

		private Pen blackPen = new Pen(Color.Black, 1f);

		private Color startColor = Color.Black;

		private Color endColor = Color.White;

		private Color fillColor = Color.Black;

		private ColorSliderType type;

		private bool doubleArrow = true;

		private Bitmap arrow;

		private int min;

		private int max = 255;

		private int width = 256;

		private int height = 10;

		private int trackMode;

		private int dx;

		[DefaultValue(typeof(Color), "Black")]
		public Color StartColor
		{
			get
			{
				return startColor;
			}
			set
			{
				startColor = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "White")]
		public Color EndColor
		{
			get
			{
				return endColor;
			}
			set
			{
				endColor = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		public Color FillColor
		{
			get
			{
				return fillColor;
			}
			set
			{
				fillColor = value;
				Invalidate();
			}
		}

		[DefaultValue(ColorSliderType.Gradient)]
		public ColorSliderType Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
				if (type != 0 && type != ColorSliderType.Threshold)
				{
					DoubleArrow = true;
				}
				Invalidate();
			}
		}

		[DefaultValue(0)]
		public int Min
		{
			get
			{
				return min;
			}
			set
			{
				min = value;
				Invalidate();
			}
		}

		[DefaultValue(255)]
		public int Max
		{
			get
			{
				return max;
			}
			set
			{
				max = value;
				Invalidate();
			}
		}

		[DefaultValue(true)]
		public bool DoubleArrow
		{
			get
			{
				return doubleArrow;
			}
			set
			{
				doubleArrow = value;
				if (!doubleArrow && type != ColorSliderType.Threshold)
				{
					Type = ColorSliderType.Gradient;
				}
				Invalidate();
			}
		}

		public event EventHandler ValuesChanged;

		public ColorSlider()
		{
			InitializeComponent();
			SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, value: true);
			Assembly assembly = GetType().Assembly;
			arrow = new Bitmap(assembly.GetManifestResourceStream("AForge.Controls.Resources.arrow.bmp"));
			arrow.MakeTransparent(Color.FromArgb(255, 255, 255));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				blackPen.Dispose();
				arrow.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			SuspendLayout();
			base.Paint += new System.Windows.Forms.PaintEventHandler(ColorSlider_Paint);
			base.MouseMove += new System.Windows.Forms.MouseEventHandler(ColorSlider_MouseMove);
			base.MouseDown += new System.Windows.Forms.MouseEventHandler(ColorSlider_MouseDown);
			base.MouseUp += new System.Windows.Forms.MouseEventHandler(ColorSlider_MouseUp);
			ResumeLayout(false);
		}

		private void ColorSlider_Paint(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			int num = (base.ClientRectangle.Right - width) / 2;
			int num2 = 2;
			graphics.DrawRectangle(blackPen, num - 1, num2 - 1, width + 1, height + 1);
			switch (type)
			{
			case ColorSliderType.Gradient:
			case ColorSliderType.InnerGradient:
			case ColorSliderType.OuterGradient:
			{
				Brush brush = new LinearGradientBrush(new System.Drawing.Point(num, 0), new System.Drawing.Point(num + width, 0), startColor, endColor);
				graphics.FillRectangle(brush, num, num2, width, height);
				brush.Dispose();
				if (type == ColorSliderType.InnerGradient)
				{
					brush = new SolidBrush(fillColor);
					if (min != 0)
					{
						graphics.FillRectangle(brush, num, num2, min, height);
					}
					if (max != 255)
					{
						graphics.FillRectangle(brush, num + max + 1, num2, 255 - max, height);
					}
					brush.Dispose();
				}
				else if (type == ColorSliderType.OuterGradient)
				{
					brush = new SolidBrush(fillColor);
					graphics.FillRectangle(brush, num + min, num2, max - min + 1, height);
					brush.Dispose();
				}
				break;
			}
			case ColorSliderType.Threshold:
			{
				Brush brush = new SolidBrush(startColor);
				graphics.FillRectangle(brush, num, num2, width, height);
				brush.Dispose();
				brush = new SolidBrush(endColor);
				graphics.FillRectangle(brush, num + min, num2, max - min + 1, height);
				brush.Dispose();
				break;
			}
			}
			num -= 4;
			num2 += 1 + height;
			graphics.DrawImage(arrow, num + min, num2, 9, 6);
			if (doubleArrow)
			{
				graphics.DrawImage(arrow, num + max, num2, 9, 6);
			}
		}

		private void ColorSlider_MouseDown(object sender, MouseEventArgs e)
		{
			int num = (base.ClientRectangle.Right - width) / 2 - 4;
			int num2 = 3 + height;
			if (e.Y >= num2 && e.Y < num2 + 6)
			{
				if (e.X >= num + min && e.X < num + min + 9)
				{
					trackMode = 1;
					dx = e.X - min;
				}
				if (doubleArrow && e.X >= num + max && e.X < num + max + 9)
				{
					trackMode = 2;
					dx = e.X - max;
				}
				if (trackMode != 0)
				{
					base.Capture = true;
				}
			}
		}

		private void ColorSlider_MouseUp(object sender, MouseEventArgs e)
		{
			if (trackMode != 0)
			{
				base.Capture = false;
				trackMode = 0;
				if (this.ValuesChanged != null)
				{
					this.ValuesChanged(this, new EventArgs());
				}
			}
		}

		private void ColorSlider_MouseMove(object sender, MouseEventArgs e)
		{
			if (trackMode != 0)
			{
				if (trackMode == 1)
				{
					min = e.X - dx;
					min = Math.Max(min, 0);
					min = Math.Min(min, max);
				}
				if (trackMode == 2)
				{
					max = e.X - dx;
					max = Math.Max(max, min);
					max = Math.Min(max, 255);
				}
				Invalidate();
			}
		}
	}
}
