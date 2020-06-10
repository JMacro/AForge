using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AForge.Controls
{
	public class Histogram : Control
	{
		private Color color = Color.Black;

		private bool logarithmic;

		private int[] values;

		private int max;

		private double maxLogarithmic;

		private bool allowSelection;

		private bool vertical;

		private Pen blackPen = new Pen(Color.Black, 1f);

		private Pen whitePen = new Pen(Color.White, 1f);

		private Pen drawPen = new Pen(Color.Black);

		private int width;

		private int height;

		private bool tracking;

		private bool over;

		private int start;

		private int stop;

		[DefaultValue(typeof(Color), "Black")]
		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
				drawPen.Dispose();
				drawPen = new Pen(color);
				Invalidate();
			}
		}

		[DefaultValue(false)]
		public bool AllowSelection
		{
			get
			{
				return allowSelection;
			}
			set
			{
				allowSelection = value;
			}
		}

		[DefaultValue(false)]
		public bool IsLogarithmicView
		{
			get
			{
				return logarithmic;
			}
			set
			{
				logarithmic = value;
				Invalidate();
			}
		}

		[DefaultValue(false)]
		public bool IsVertical
		{
			get
			{
				return vertical;
			}
			set
			{
				vertical = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		public int[] Values
		{
			get
			{
				return values;
			}
			set
			{
				values = value;
				if (values != null)
				{
					max = 0;
					int[] array = values;
					foreach (int num in array)
					{
						if (num < 0)
						{
							throw new ArgumentException("Histogram values should be non-negative.");
						}
						if (num > max)
						{
							max = num;
							maxLogarithmic = Math.Log10(max);
						}
					}
				}
				Invalidate();
			}
		}

		public event HistogramEventHandler PositionChanged;

		public event HistogramEventHandler SelectionChanged;

		public Histogram()
		{
			InitializeComponent();
			SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, value: true);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				blackPen.Dispose();
				whitePen.Dispose();
				drawPen.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.MouseUp += new System.Windows.Forms.MouseEventHandler(Histogram_MouseUp);
			base.MouseMove += new System.Windows.Forms.MouseEventHandler(Histogram_MouseMove);
			base.MouseLeave += new System.EventHandler(Histogram_MouseLeave);
			base.MouseDown += new System.Windows.Forms.MouseEventHandler(Histogram_MouseDown);
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			Graphics graphics = pe.Graphics;
			width = ((values == null || vertical) ? (base.ClientRectangle.Width - 2) : Math.Min(values.Length, base.ClientRectangle.Width - 2));
			height = ((values == null || !vertical) ? (base.ClientRectangle.Height - 2) : Math.Min(values.Length, base.ClientRectangle.Height - 2));
			int num = 1;
			int num2 = 1;
			graphics.DrawRectangle(blackPen, num - 1, num2 - 1, width + 1, height + 1);
			if (values != null)
			{
				int num3 = Math.Min(start, stop);
				int num4 = Math.Max(start, stop);
				if (tracking)
				{
					Brush brush = new SolidBrush(Color.FromArgb(92, 92, 92));
					if (vertical)
					{
						graphics.FillRectangle(brush, num, num2 + num3, width, Math.Abs(num3 - num4) + 1);
					}
					else
					{
						graphics.FillRectangle(brush, num + num3, num2, Math.Abs(num3 - num4) + 1, height);
					}
					brush.Dispose();
				}
				if (max != 0)
				{
					double num5 = (double)(vertical ? width : height) / (logarithmic ? maxLogarithmic : ((double)max));
					int i = 0;
					for (int num6 = vertical ? height : width; i < num6; i++)
					{
						int num7 = (!logarithmic) ? ((int)((double)values[i] * num5)) : ((values[i] != 0) ? ((int)(Math.Log10(values[i]) * num5)) : 0);
						if (num7 != 0)
						{
							if (vertical)
							{
								graphics.DrawLine((tracking && i >= num3 && i <= num4) ? whitePen : drawPen, new System.Drawing.Point(num, num2 + i), new System.Drawing.Point(num + num7, num2 + i));
							}
							else
							{
								graphics.DrawLine((tracking && i >= num3 && i <= num4) ? whitePen : drawPen, new System.Drawing.Point(num + i, num2 + height - 1), new System.Drawing.Point(num + i, num2 + height - num7));
							}
						}
					}
				}
			}
			base.OnPaint(pe);
		}

		private void Histogram_MouseDown(object sender, MouseEventArgs e)
		{
			if (allowSelection && values != null)
			{
				int num = 1;
				int num2 = 1;
				if (e.X >= num && e.Y >= num2 && e.X < num + width && e.Y < num2 + height)
				{
					tracking = true;
					start = (vertical ? (e.Y - num2) : (e.X - num));
					base.Capture = true;
				}
			}
		}

		private void Histogram_MouseUp(object sender, MouseEventArgs e)
		{
			if (tracking)
			{
				tracking = false;
				base.Capture = false;
				Invalidate();
			}
		}

		private void Histogram_MouseMove(object sender, MouseEventArgs e)
		{
			if (!allowSelection || values == null)
			{
				return;
			}
			int num = 1;
			int num2 = 1;
			if (!tracking)
			{
				if (e.X >= num && e.Y >= num2 && e.X < num + width && e.Y < num2 + height)
				{
					over = true;
					Cursor = Cursors.Cross;
					if (this.PositionChanged != null)
					{
						this.PositionChanged(this, new HistogramEventArgs(vertical ? (e.Y - num2) : (e.X - num)));
					}
					return;
				}
				Cursor = Cursors.Default;
				if (over)
				{
					over = false;
					if (this.PositionChanged != null)
					{
						this.PositionChanged(this, new HistogramEventArgs(-1));
					}
				}
			}
			else
			{
				stop = (vertical ? (e.Y - num2) : (e.X - num));
				stop = Math.Min(stop, (vertical ? height : width) - 1);
				stop = Math.Max(stop, 0);
				Invalidate();
				if (this.SelectionChanged != null)
				{
					this.SelectionChanged(this, new HistogramEventArgs(Math.Min(start, stop), Math.Max(start, stop)));
				}
			}
		}

		private void Histogram_MouseLeave(object sender, EventArgs e)
		{
			if (allowSelection && values != null && !tracking && this.PositionChanged != null)
			{
				this.PositionChanged(this, new HistogramEventArgs(-1));
			}
		}
	}
}
