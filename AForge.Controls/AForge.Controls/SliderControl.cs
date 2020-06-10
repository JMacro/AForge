using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AForge.Controls
{
	public class SliderControl : Control
	{
		public delegate void PositionChangedHandler(object sender, float position);

		private const int manipulatorWidth = 11;

		private const int manipulatorHeight = 20;

		private bool isHorizontal = true;

		private bool resetPositionOnMouseRelease = true;

		private Pen borderPen = new Pen(Color.Black);

		private SolidBrush positiveAreaBrush = new SolidBrush(Color.White);

		private SolidBrush negativeAreaBrush = new SolidBrush(Color.LightGray);

		private SolidBrush manipulatorBrush = new SolidBrush(Color.LightSeaGreen);

		private SolidBrush disabledBrash = new SolidBrush(Color.FromArgb(240, 240, 240));

		private int leftMargin;

		private int topMargin;

		private float manipulatatorPosition;

		private bool tracking;

		private int ticksBeforeNotificiation = -1;

		private IContainer components;

		private Timer timer;

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

		[DefaultValue(typeof(Color), "Black")]
		[Description("Color used for drawing borders.")]
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

		[Description("Background color used for filling area corresponding to positive values.")]
		[DefaultValue(typeof(Color), "White")]
		public Color PositiveAreaBrush
		{
			get
			{
				return positiveAreaBrush.Color;
			}
			set
			{
				positiveAreaBrush = new SolidBrush(value);
				Invalidate();
			}
		}

		[Description("Background color used for filling top right quarter of the control.")]
		[DefaultValue(typeof(Color), "LightGray")]
		public Color NegativeAreaBrush
		{
			get
			{
				return negativeAreaBrush.Color;
			}
			set
			{
				negativeAreaBrush = new SolidBrush(value);
				Invalidate();
			}
		}

		[Description("Color used for filling manipulator.")]
		[DefaultValue(typeof(Color), "LightSeaGreen")]
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

		[DefaultValue(true)]
		[Description("Defines if control has horizontal or vertical look.")]
		public bool IsHorizontal
		{
			get
			{
				return isHorizontal;
			}
			set
			{
				isHorizontal = value;
				if (value)
				{
					leftMargin = 7;
					topMargin = 5;
				}
				else
				{
					leftMargin = 5;
					topMargin = 7;
				}
				Invalidate();
			}
		}

		[Browsable(false)]
		public float ManipulatorPosition
		{
			get
			{
				return manipulatatorPosition;
			}
			set
			{
				manipulatatorPosition = Math.Max(-1f, Math.Min(1f, value));
				Invalidate();
				NotifyClients();
			}
		}

		[Description("Occurs when manipulator's position is changed.")]
		public event PositionChangedHandler PositionChanged;

		public SliderControl()
		{
			InitializeComponent();
			SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, value: true);
			IsHorizontal = true;
		}

		private void TurnControl_Paint(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			int width = base.ClientRectangle.Width;
			int height = base.ClientRectangle.Height;
			if (isHorizontal)
			{
				graphics.FillRectangle(base.Enabled ? negativeAreaBrush : disabledBrash, leftMargin, topMargin, width / 2 - leftMargin, 10);
				graphics.FillRectangle(base.Enabled ? positiveAreaBrush : disabledBrash, width / 2, topMargin, width / 2 - leftMargin, 10);
				graphics.DrawRectangle(borderPen, leftMargin, topMargin, width - 1 - leftMargin * 2, 10);
				graphics.DrawLine(borderPen, width / 2, topMargin, width / 2, topMargin + 10);
				int num = (int)(manipulatatorPosition * (float)(width / 2 - leftMargin) + (float)(width / 2));
				graphics.FillRectangle(base.Enabled ? manipulatorBrush : disabledBrash, num - 5, 0, 11, 20);
				graphics.DrawRectangle(borderPen, num - 5, 0, 11, 20);
			}
			else
			{
				graphics.FillRectangle(base.Enabled ? positiveAreaBrush : disabledBrash, leftMargin, topMargin, 10, height / 2 - topMargin);
				graphics.FillRectangle(base.Enabled ? negativeAreaBrush : disabledBrash, leftMargin, height / 2, 10, height / 2 - topMargin);
				graphics.DrawRectangle(borderPen, leftMargin, topMargin, 10, height - 1 - topMargin * 2);
				graphics.DrawLine(borderPen, leftMargin, height / 2, leftMargin + 10, height / 2);
				int num2 = (int)((0f - manipulatatorPosition) * (float)(height / 2 - topMargin) + (float)(height / 2));
				graphics.FillRectangle(base.Enabled ? manipulatorBrush : disabledBrash, 0, num2 - 5, 20, 11);
				graphics.DrawRectangle(borderPen, 0, num2 - 5, 20, 11);
			}
		}

		private void TurnControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			if (isHorizontal)
			{
				if (e.X >= leftMargin && e.X < base.ClientRectangle.Width - leftMargin && e.Y >= topMargin && e.Y < base.ClientRectangle.Height - topMargin)
				{
					int num = e.X - base.ClientRectangle.Width / 2;
					manipulatatorPosition = (float)num / (float)(base.ClientRectangle.Width / 2 - leftMargin);
					tracking = true;
				}
			}
			else if (e.X >= leftMargin && e.X < base.ClientRectangle.Width - leftMargin && e.Y >= topMargin && e.Y < base.ClientRectangle.Height - topMargin)
			{
				int num2 = base.ClientRectangle.Height / 2 - e.Y;
				manipulatatorPosition = (float)num2 / (float)(base.ClientRectangle.Height / 2 - topMargin);
				tracking = true;
			}
			if (tracking)
			{
				base.Capture = true;
				Cursor = Cursors.Hand;
				NotifyClients();
				ticksBeforeNotificiation = -1;
				timer.Start();
			}
		}

		private void TurnControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && tracking)
			{
				tracking = false;
				base.Capture = false;
				Cursor = Cursors.Arrow;
				if (resetPositionOnMouseRelease)
				{
					manipulatatorPosition = 0f;
				}
				Invalidate();
				timer.Stop();
				NotifyClients();
			}
		}

		private void TurnControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (tracking)
			{
				if (isHorizontal)
				{
					int num = e.X - base.ClientRectangle.Width / 2;
					manipulatatorPosition = (float)num / (float)(base.ClientRectangle.Width / 2 - leftMargin);
				}
				else
				{
					int num2 = base.ClientRectangle.Height / 2 - e.Y;
					manipulatatorPosition = (float)num2 / (float)(base.ClientRectangle.Height / 2 - topMargin);
				}
				manipulatatorPosition = Math.Max(Math.Min(1f, manipulatatorPosition), -1f);
				Invalidate();
				ticksBeforeNotificiation = 5;
			}
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
				this.PositionChanged(this, manipulatatorPosition);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
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
			base.Paint += new System.Windows.Forms.PaintEventHandler(TurnControl_Paint);
			base.MouseMove += new System.Windows.Forms.MouseEventHandler(TurnControl_MouseMove);
			base.MouseDown += new System.Windows.Forms.MouseEventHandler(TurnControl_MouseDown);
			base.MouseUp += new System.Windows.Forms.MouseEventHandler(TurnControl_MouseUp);
			ResumeLayout(false);
		}
	}
}
