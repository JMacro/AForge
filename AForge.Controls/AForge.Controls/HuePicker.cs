using AForge.Imaging;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AForge.Controls
{
	public class HuePicker : Control
	{
		public enum HuePickerType
		{
			Value,
			Range
		}

		private HuePickerType type;

		private Pen blackPen;

		private Brush blackBrush;

		private Pen whitePen;

		private Brush whiteBrush;

		private System.Drawing.Point ptCenter = new System.Drawing.Point(0, 0);

		private System.Drawing.Point ptMin = new System.Drawing.Point(0, 0);

		private System.Drawing.Point ptMax = new System.Drawing.Point(0, 0);

		private int trackMode;

		private int min;

		private int max = 359;

		[DefaultValue(0)]
		public int Value
		{
			get
			{
				return min;
			}
			set
			{
				if (type == HuePickerType.Value)
				{
					min = Math.Max(0, Math.Min(359, value));
					Invalidate();
				}
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
				if (type == HuePickerType.Range)
				{
					min = Math.Max(0, Math.Min(359, value));
					Invalidate();
				}
			}
		}

		[DefaultValue(359)]
		public int Max
		{
			get
			{
				return max;
			}
			set
			{
				if (type == HuePickerType.Range)
				{
					max = Math.Max(0, Math.Min(359, value));
					Invalidate();
				}
			}
		}

		[DefaultValue(HuePickerType.Value)]
		public HuePickerType Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
				Invalidate();
			}
		}

		public event EventHandler ValuesChanged;

		public HuePicker()
		{
			InitializeComponent();
			SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, value: true);
			blackPen = new Pen(Color.Black, 1f);
			blackBrush = new SolidBrush(Color.Black);
			whitePen = new Pen(Color.White, 1f);
			whiteBrush = new SolidBrush(Color.White);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				blackPen.Dispose();
				blackBrush.Dispose();
				whitePen.Dispose();
				whiteBrush.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.MouseUp += new System.Windows.Forms.MouseEventHandler(HSLPicker_MouseUp);
			base.MouseMove += new System.Windows.Forms.MouseEventHandler(HSLPicker_MouseMove);
			base.MouseDown += new System.Windows.Forms.MouseEventHandler(HSLPicker_MouseDown);
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			Graphics graphics = pe.Graphics;
			Rectangle clientRectangle = base.ClientRectangle;
			RGB rGB = new RGB();
			HSL hSL = new HSL();
			Rectangle rect = new Rectangle(4, 4, Math.Min(clientRectangle.Right, clientRectangle.Bottom) - 8, Math.Min(clientRectangle.Right, clientRectangle.Bottom) - 8);
			hSL.Luminance = 0.5f;
			hSL.Saturation = 1f;
			if (type == HuePickerType.Value)
			{
				for (int i = 0; i < 360; i++)
				{
					hSL.Hue = i;
					HSL.ToRGB(hSL, rGB);
					Brush brush = new SolidBrush(rGB.Color);
					graphics.FillPie(brush, rect, -i, -1f);
					brush.Dispose();
				}
			}
			else
			{
				for (int j = 0; j < 360; j++)
				{
					Brush brush;
					if ((min < max && j >= min && j <= max) || (min > max && (j >= min || j <= max)))
					{
						hSL.Hue = j;
						HSL.ToRGB(hSL, rGB);
						brush = new SolidBrush(rGB.Color);
					}
					else
					{
						brush = new SolidBrush(Color.FromArgb(128, 128, 128));
					}
					graphics.FillPie(brush, rect, -j, -1f);
					brush.Dispose();
				}
			}
			double num = (double)rect.Width / 2.0;
			double num2 = (double)(-min) * Math.PI / 180.0;
			double num3 = Math.Cos(num2);
			double num4 = Math.Sin(num2);
			double num5 = num * num3;
			double num6 = num * num4;
			ptCenter.X = rect.Left + (int)num;
			ptCenter.Y = rect.Top + (int)num;
			ptMin.X = rect.Left + (int)(num + num5);
			ptMin.Y = rect.Top + (int)(num + num6);
			graphics.FillEllipse(blackBrush, rect.Left + (int)(num + num5) - 4, rect.Top + (int)(num + num6) - 4, 8, 8);
			graphics.DrawLine(blackPen, ptCenter, ptMin);
			if (type == HuePickerType.Range)
			{
				num2 = (double)(-max) * Math.PI / 180.0;
				num3 = Math.Cos(num2);
				num4 = Math.Sin(num2);
				num5 = num * num3;
				num6 = num * num4;
				ptMax.X = rect.Left + (int)(num + num5);
				ptMax.Y = rect.Top + (int)(num + num6);
				graphics.FillEllipse(whiteBrush, rect.Left + (int)(num + num5) - 4, rect.Top + (int)(num + num6) - 4, 8, 8);
				graphics.DrawLine(whitePen, ptCenter, ptMax);
			}
			base.OnPaint(pe);
		}

		private void HSLPicker_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.X >= ptMin.X - 4 && e.Y >= ptMin.Y - 4 && e.X < ptMin.X + 4 && e.Y < ptMin.Y + 4)
			{
				trackMode = 1;
			}
			if (type == HuePickerType.Range && e.X >= ptMax.X - 4 && e.Y >= ptMax.Y - 4 && e.X < ptMax.X + 4 && e.Y < ptMax.Y + 4)
			{
				trackMode = 2;
			}
			if (trackMode != 0)
			{
				base.Capture = true;
			}
		}

		private void HSLPicker_MouseUp(object sender, MouseEventArgs e)
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

		private void HSLPicker_MouseMove(object sender, MouseEventArgs e)
		{
			Cursor cursor = Cursors.Default;
			if (trackMode != 0)
			{
				cursor = Cursors.Hand;
				int num = e.Y - ptCenter.Y;
				int num2 = e.X - ptCenter.X;
				if (trackMode == 1)
				{
					min = (int)(Math.Atan2(-num, num2) * 180.0 / Math.PI);
					if (min < 0)
					{
						min = 360 + min;
					}
				}
				else
				{
					max = (int)(Math.Atan2(-num, num2) * 180.0 / Math.PI);
					if (max < 0)
					{
						max = 360 + max;
					}
				}
				Invalidate();
			}
			else
			{
				if (e.X >= ptMin.X - 4 && e.Y >= ptMin.Y - 4 && e.X < ptMin.X + 4 && e.Y < ptMin.Y + 4)
				{
					cursor = Cursors.Hand;
				}
				if (type == HuePickerType.Range && e.X >= ptMax.X - 4 && e.Y >= ptMax.Y - 4 && e.X < ptMax.X + 4 && e.Y < ptMax.Y + 4)
				{
					cursor = Cursors.Hand;
				}
			}
			Cursor = cursor;
		}
	}
}
