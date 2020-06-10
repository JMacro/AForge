using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AForge.Controls
{
	public class Chart : Control
	{
		public enum SeriesType
		{
			Line,
			Dots,
			ConnectedDots
		}

		private class DataSeries
		{
			public double[,] data;

			public Color color = Color.Blue;

			public SeriesType type;

			public int width = 1;

			public bool updateYRange = true;
		}

		private Dictionary<string, DataSeries> seriesTable = new Dictionary<string, DataSeries>();

		private Pen blackPen = new Pen(Color.Black);

		private Range rangeX = new Range(0f, 1f);

		private Range rangeY = new Range(0f, 1f);

		private Container components;

		[Browsable(false)]
		public Range RangeX
		{
			get
			{
				return rangeX;
			}
			set
			{
				rangeX = value;
				UpdateYRange();
				Invalidate();
			}
		}

		[Browsable(false)]
		public Range RangeY
		{
			get
			{
				return rangeY;
			}
			set
			{
				rangeY = value;
				Invalidate();
			}
		}

		public Chart()
		{
			InitializeComponent();
			SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, value: true);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
				blackPen.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			SuspendLayout();
			base.Paint += new System.Windows.Forms.PaintEventHandler(Chart_Paint);
			ResumeLayout(false);
		}

		private void Chart_Paint(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			int width = base.ClientRectangle.Width;
			int height = base.ClientRectangle.Height;
			Brush brush = new SolidBrush(BackColor);
			graphics.FillRectangle(brush, 0, 0, width - 1, height - 1);
			brush.Dispose();
			graphics.DrawRectangle(blackPen, 0, 0, width - 1, height - 1);
			graphics.SetClip(new Rectangle(2, 2, width - 4, height - 4));
			if (rangeX.Length != 0f)
			{
				double num = (double)(width - 10) / (double)rangeX.Length;
				double num2 = (double)(height - 10) / (double)((rangeY.Length != 0f) ? rangeY.Length : 1f);
				foreach (KeyValuePair<string, DataSeries> item in seriesTable)
				{
					DataSeries value = item.Value;
					double[,] data = value.data;
					if (data != null)
					{
						if (value.type == SeriesType.Dots)
						{
							Brush brush2 = new SolidBrush(value.color);
							int width2 = value.width;
							int num3 = width2 >> 1;
							int i = 0;
							for (int length = data.GetLength(0); i < length; i++)
							{
								int num4 = (int)((data[i, 0] - (double)rangeX.Min) * num);
								int num5 = (int)((data[i, 1] - (double)rangeY.Min) * num2);
								num4 += 5;
								num5 = height - 6 - num5;
								graphics.FillRectangle(brush2, num4 - num3, num5 - num3, width2, width2);
							}
							brush2.Dispose();
						}
						else if (value.type == SeriesType.ConnectedDots)
						{
							Brush brush3 = new SolidBrush(value.color);
							Pen pen = new Pen(value.color, 1f);
							int width3 = value.width;
							int num6 = width3 >> 1;
							int num7 = (int)((data[0, 0] - (double)rangeX.Min) * num);
							int num8 = (int)((data[0, 1] - (double)rangeY.Min) * num2);
							num7 += 5;
							num8 = height - 6 - num8;
							graphics.FillRectangle(brush3, num7 - num6, num8 - num6, width3, width3);
							int j = 1;
							for (int length2 = data.GetLength(0); j < length2; j++)
							{
								int num9 = (int)((data[j, 0] - (double)rangeX.Min) * num);
								int num10 = (int)((data[j, 1] - (double)rangeY.Min) * num2);
								num9 += 5;
								num10 = height - 6 - num10;
								graphics.FillRectangle(brush3, num9 - num6, num10 - num6, width3, width3);
								graphics.DrawLine(pen, num7, num8, num9, num10);
								num7 = num9;
								num8 = num10;
							}
							pen.Dispose();
							brush3.Dispose();
						}
						else if (value.type == SeriesType.Line)
						{
							Pen pen2 = new Pen(value.color, value.width);
							int num11 = (int)((data[0, 0] - (double)rangeX.Min) * num);
							int num12 = (int)((data[0, 1] - (double)rangeY.Min) * num2);
							num11 += 5;
							num12 = height - 6 - num12;
							int k = 1;
							for (int length3 = data.GetLength(0); k < length3; k++)
							{
								int num13 = (int)((data[k, 0] - (double)rangeX.Min) * num);
								int num14 = (int)((data[k, 1] - (double)rangeY.Min) * num2);
								num13 += 5;
								num14 = height - 6 - num14;
								graphics.DrawLine(pen2, num11, num12, num13, num14);
								num11 = num13;
								num12 = num14;
							}
							pen2.Dispose();
						}
					}
				}
			}
		}

		public void AddDataSeries(string name, Color color, SeriesType type, int width)
		{
			AddDataSeries(name, color, type, width, updateYRange: true);
		}

		public void AddDataSeries(string name, Color color, SeriesType type, int width, bool updateYRange)
		{
			DataSeries dataSeries = new DataSeries();
			dataSeries.color = color;
			dataSeries.type = type;
			dataSeries.width = width;
			dataSeries.updateYRange = updateYRange;
			seriesTable.Add(name, dataSeries);
		}

		public void UpdateDataSeries(string name, double[,] data)
		{
			if (!seriesTable.ContainsKey(name))
			{
				throw new ArgumentException("The chart does not contain data series with name: " + name);
			}
			DataSeries dataSeries = seriesTable[name];
			dataSeries.data = ((data != null) ? ((double[,])data.Clone()) : null);
			if (dataSeries.updateYRange)
			{
				UpdateYRange();
			}
			Invalidate();
		}

		public void RemoveDataSeries(string name)
		{
			seriesTable.Remove(name);
			Invalidate();
		}

		public void RemoveAllDataSeries()
		{
			seriesTable.Clear();
			Invalidate();
		}

		private void UpdateYRange()
		{
			float num = float.MaxValue;
			float num2 = float.MinValue;
			foreach (KeyValuePair<string, DataSeries> item in seriesTable)
			{
				DataSeries value = item.Value;
				double[,] data = value.data;
				if (value.updateYRange && data != null)
				{
					int i = 0;
					for (int length = data.GetLength(0); i < length; i++)
					{
						if (rangeX.IsInside((float)data[i, 0]))
						{
							float num3 = (float)data[i, 1];
							if (num3 > num2)
							{
								num2 = num3;
							}
							if (num3 < num)
							{
								num = num3;
							}
						}
					}
				}
			}
			if ((double)num != double.MaxValue || (double)num2 != double.MinValue)
			{
				rangeY = new Range(num, num2);
			}
		}
	}
}
