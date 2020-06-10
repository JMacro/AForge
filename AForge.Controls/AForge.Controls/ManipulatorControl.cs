using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AForge.Controls
{
	public class ManipulatorControl : Control
	{
		public class PositionEventArgs : EventArgs
		{
			private float x;

			private float y;

			public float X => x;

			public float Y => y;

			public float Theta
			{
				get
				{
					if (x != 0f)
					{
						double num = Math.Atan(y / x);
						num = num / Math.PI * 180.0;
						if (num < 0.0)
						{
							num += 180.0;
						}
						if (y < 0f)
						{
							num += 180.0;
						}
						return (float)num;
					}
					return (y > 0f) ? 90 : ((y < 0f) ? 270 : 0);
				}
			}

			public float R => (float)Math.Sqrt(x * x + y * y);

			public PositionEventArgs(float x, float y)
			{
				this.x = x;
				this.y = y;
			}
		}

		public delegate void PositionChangedHandler(object sender, PositionEventArgs eventArgs);

		private const int manipulatorSize = 21;

		private const int manipulatorRadius = 10;

		private Timer timer;

		private IContainer components;

		private bool isSquareLook;

		private bool drawHorizontalAxis = true;

		private bool drawVerticalAxis;

		private bool resetPositionOnMouseRelease = true;

		private Pen borderPen = new Pen(Color.Black);

		private SolidBrush topLeftBackgroundBrush = new SolidBrush(Color.White);

		private SolidBrush topRightBackgroundBrush = new SolidBrush(Color.White);

		private SolidBrush bottomLeftBackgroundBrush = new SolidBrush(Color.LightGray);

		private SolidBrush bottomRightBackgroundBrush = new SolidBrush(Color.LightGray);

		private SolidBrush manipulatorBrush = new SolidBrush(Color.LightSeaGreen);

		private SolidBrush disabledBrash = new SolidBrush(Color.FromArgb(240, 240, 240));

		private float manipulatatorX;

		private float manipulatatorY;

		private int areaSize;

		private int areaRadius;

		private int areaMargin = 12;

		private bool tracking;

		private int ticksBeforeNotificiation = -1;

		[DefaultValue(false)]
		[Description("Determines if the control has square or round look.")]
		public bool IsSquareLook
		{
			get
			{
				return isSquareLook;
			}
			set
			{
				isSquareLook = value;
				Invalidate();
			}
		}

		[DefaultValue(true)]
		[Description("Determines if horizontal axis should be drawn or not.")]
		public bool DrawHorizontalAxis
		{
			get
			{
				return drawHorizontalAxis;
			}
			set
			{
				drawHorizontalAxis = value;
				Invalidate();
			}
		}

		[Description("Determines if vertical axis should be drawn or not.")]
		[DefaultValue(false)]
		public bool DrawVerticalAxis
		{
			get
			{
				return drawVerticalAxis;
			}
			set
			{
				drawVerticalAxis = value;
				Invalidate();
			}
		}

		[Description("Determines behaviour of manipulator, when mouse button is released.")]
		[DefaultValue(true)]
		public bool ResetPositionOnMouseRelease
		{
			get
			{
				return resetPositionOnMouseRelease;
			}
			set
			{
				resetPositionOnMouseRelease = value;
			}
		}

		[Description("Color used for drawing borders and axis's.")]
		[DefaultValue(typeof(Color), "Black")]
		public Color BorderColor
		{
			get
			{
				return borderPen.Color;
			}
			set
			{
				borderPen = new Pen(value);
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "White")]
		[Description("Background color used for filling top left quarter of the control.")]
		public Color TopLeftBackgroundColor
		{
			get
			{
				return topLeftBackgroundBrush.Color;
			}
			set
			{
				topLeftBackgroundBrush = new SolidBrush(value);
				Invalidate();
			}
		}

		[Description("Background color used for filling top right quarter of the control.")]
		[DefaultValue(typeof(Color), "White")]
		public Color TopRightBackgroundColor
		{
			get
			{
				return topRightBackgroundBrush.Color;
			}
			set
			{
				topRightBackgroundBrush = new SolidBrush(value);
				Invalidate();
			}
		}

		[Description("Background color used for filling bottom left quarter of the control.")]
		[DefaultValue(typeof(Color), "LightGray")]
		public Color BottomLeftBackgroundColor
		{
			get
			{
				return bottomLeftBackgroundBrush.Color;
			}
			set
			{
				bottomLeftBackgroundBrush = new SolidBrush(value);
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "LightGray")]
		[Description("Background color used for filling bottom right quarter of the control.")]
		public Color BottomRightBackgroundColor
		{
			get
			{
				return bottomRightBackgroundBrush.Color;
			}
			set
			{
				bottomRightBackgroundBrush = new SolidBrush(value);
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "LightSeaGreen")]
		[Description("Color used for filling manipulator.")]
		public Color ManipulatorColor
		{
			get
			{
				return manipulatorBrush.Color;
			}
			set
			{
				manipulatorBrush = new SolidBrush(value);
				Invalidate();
			}
		}

		[Browsable(false)]
		public PointF ManipulatorPosition
		{
			get
			{
				return new PointF(manipulatatorX, manipulatatorY);
			}
			set
			{
				manipulatatorX = Math.Max(-1f, Math.Min(1f, value.X));
				manipulatatorY = Math.Max(-1f, Math.Min(1f, value.Y));
				if (!isSquareLook)
				{
					double num = Math.Sqrt(manipulatatorX * manipulatatorX + manipulatatorY * manipulatatorY);
					if (num > 1.0)
					{
						double num2 = 1.0 / num;
						manipulatatorX = (float)(num2 * (double)manipulatatorX);
						manipulatatorY = (float)(num2 * (double)manipulatatorY);
					}
				}
				Invalidate();
				NotifyClients();
			}
		}

		[Description("Occurs when manipulator's position is changed.")]
		public event PositionChangedHandler PositionChanged;

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			timer = new System.Windows.Forms.Timer(components);
			SuspendLayout();
			timer.Interval = 10;
			timer.Tick += new System.EventHandler(timer_Tick);
			base.Paint += new System.Windows.Forms.PaintEventHandler(ManipulatorControl_Paint);
			base.MouseMove += new System.Windows.Forms.MouseEventHandler(ManipulatorControl_MouseMove);
			base.MouseDown += new System.Windows.Forms.MouseEventHandler(ManipulatorControl_MouseDown);
			base.MouseUp += new System.Windows.Forms.MouseEventHandler(ManipulatorControl_MouseUp);
			ResumeLayout(false);
		}

		public ManipulatorControl()
		{
			InitializeComponent();
			SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, value: true);
		}

		private void ManipulatorControl_Paint(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			areaSize = Math.Min(base.ClientRectangle.Width, base.ClientRectangle.Height) - areaMargin * 2;
			areaRadius = areaSize / 2;
			if (areaSize > 1)
			{
				if (isSquareLook)
				{
					int num = areaSize / 2;
					graphics.FillRectangle((!base.Enabled) ? disabledBrash : topLeftBackgroundBrush, areaMargin, areaMargin, num, num);
					graphics.FillRectangle((!base.Enabled) ? disabledBrash : topRightBackgroundBrush, areaMargin + num, areaMargin, areaSize - num, num);
					graphics.FillRectangle((!base.Enabled) ? disabledBrash : bottomLeftBackgroundBrush, areaMargin, areaMargin + num, num, areaSize - num);
					graphics.FillRectangle((!base.Enabled) ? disabledBrash : bottomRightBackgroundBrush, areaMargin + num, areaMargin + num, areaSize - num, areaSize - num);
					graphics.DrawRectangle(borderPen, areaMargin, areaMargin, areaSize - 1, areaSize - 1);
				}
				else
				{
					graphics.FillPie(base.Enabled ? topRightBackgroundBrush : disabledBrash, areaMargin, areaMargin, areaSize - 1, areaSize - 1, 0, -90);
					graphics.FillPie(base.Enabled ? topLeftBackgroundBrush : disabledBrash, areaMargin, areaMargin, areaSize - 1, areaSize - 1, -90, -90);
					graphics.FillPie(base.Enabled ? bottomRightBackgroundBrush : disabledBrash, areaMargin, areaMargin, areaSize - 1, areaSize - 1, 0, 90);
					graphics.FillPie(base.Enabled ? bottomLeftBackgroundBrush : disabledBrash, areaMargin, areaMargin, areaSize - 1, areaSize - 1, 90, 90);
					graphics.DrawEllipse(borderPen, areaMargin, areaMargin, areaSize - 1, areaSize - 1);
				}
			}
			if (drawHorizontalAxis)
			{
				graphics.DrawLine(borderPen, areaMargin, areaMargin + areaRadius, areaMargin + areaSize - 1, areaMargin + areaRadius);
			}
			if (drawVerticalAxis)
			{
				graphics.DrawLine(borderPen, areaMargin + areaRadius, areaMargin, areaMargin + areaRadius, areaMargin + areaSize - 1);
			}
			int num2 = (int)(manipulatatorX * (float)areaRadius) + areaMargin + areaRadius;
			int num3 = (int)((0f - manipulatatorY) * (float)areaRadius) + areaMargin + areaRadius;
			graphics.FillEllipse(base.Enabled ? manipulatorBrush : disabledBrash, num2 - 10, num3 - 10, 21, 21);
			graphics.DrawEllipse(borderPen, num2 - 10, num3 - 10, 21, 21);
		}

		private void ManipulatorControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			int num = e.X - areaMargin - areaRadius;
			int num2 = e.Y - areaMargin - areaRadius;
			if (isSquareLook)
			{
				if (num <= areaRadius && num >= -areaRadius && num2 <= areaRadius && num2 >= -areaRadius)
				{
					tracking = true;
				}
			}
			else if (Math.Sqrt(num * num + num2 * num2) <= (double)areaRadius)
			{
				tracking = true;
			}
			if (tracking)
			{
				manipulatatorX = (float)num / (float)areaRadius;
				manipulatatorY = (float)(-num2) / (float)areaRadius;
				base.Capture = true;
				Cursor = Cursors.Hand;
				NotifyClients();
				ticksBeforeNotificiation = -1;
				timer.Start();
			}
		}

		private void ManipulatorControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && tracking)
			{
				tracking = false;
				base.Capture = false;
				Cursor = Cursors.Arrow;
				timer.Stop();
				if (resetPositionOnMouseRelease)
				{
					manipulatatorX = 0f;
					manipulatatorY = 0f;
				}
				NotifyClients();
				Invalidate();
			}
		}

		private void ManipulatorControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (!tracking)
			{
				return;
			}
			int num = e.X - areaMargin - areaRadius;
			int num2 = e.Y - areaMargin - areaRadius;
			if (isSquareLook)
			{
				num = Math.Min(areaRadius, Math.Max(-areaRadius, num));
				num2 = Math.Min(areaRadius, Math.Max(-areaRadius, num2));
			}
			else
			{
				int num3 = (int)Math.Sqrt(num * num + num2 * num2);
				if (num3 > areaRadius)
				{
					double num4 = (double)areaRadius / (double)num3;
					num = (int)(num4 * (double)num);
					num2 = (int)(num4 * (double)num2);
				}
			}
			manipulatatorX = (float)num / (float)areaRadius;
			manipulatatorY = (float)(-num2) / (float)areaRadius;
			Invalidate();
			ticksBeforeNotificiation = 5;
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if (ticksBeforeNotificiation != -1)
			{
				if (ticksBeforeNotificiation == 0)
				{
					NotifyClients();
				}
				ticksBeforeNotificiation--;
			}
		}

		private void NotifyClients()
		{
			if (this.PositionChanged != null)
			{
				this.PositionChanged(this, new PositionEventArgs(manipulatatorX, manipulatatorY));
			}
		}
	}
}
