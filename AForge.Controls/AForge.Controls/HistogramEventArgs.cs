using System;

namespace AForge.Controls
{
	public class HistogramEventArgs : EventArgs
	{
		private int min;

		private int max;

		public int Min => min;

		public int Max => max;

		public int Position => min;

		public HistogramEventArgs(int pos)
		{
			min = pos;
		}

		public HistogramEventArgs(int min, int max)
		{
			this.min = min;
			this.max = max;
		}
	}
}
