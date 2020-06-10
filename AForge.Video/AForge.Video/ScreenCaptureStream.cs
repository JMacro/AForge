using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace AForge.Video
{
	public class ScreenCaptureStream : IVideoSource
	{
		private Rectangle region;

		private int frameInterval = 100;

		private int framesReceived;

		private Thread thread;

		private ManualResetEvent stopEvent;

		public virtual string Source => "Screen Capture";

		public Rectangle Region
		{
			get
			{
				return region;
			}
			set
			{
				region = value;
			}
		}

		public int FrameInterval
		{
			get
			{
				return frameInterval;
			}
			set
			{
				frameInterval = Math.Max(0, value);
			}
		}

		public int FramesReceived
		{
			get
			{
				int result = framesReceived;
				framesReceived = 0;
				return result;
			}
		}

		public long BytesReceived => 0L;

		public bool IsRunning
		{
			get
			{
				if (thread != null)
				{
					if (!thread.Join(0))
					{
						return true;
					}
					Free();
				}
				return false;
			}
		}

		public event NewFrameEventHandler NewFrame;

		public event VideoSourceErrorEventHandler VideoSourceError;

		public event PlayingFinishedEventHandler PlayingFinished;

		public ScreenCaptureStream(Rectangle region)
		{
			this.region = region;
		}

		public ScreenCaptureStream(Rectangle region, int frameInterval)
		{
			this.region = region;
			FrameInterval = frameInterval;
		}

		public void Start()
		{
			if (!IsRunning)
			{
				framesReceived = 0;
				stopEvent = new ManualResetEvent(initialState: false);
				thread = new Thread(WorkerThread);
				thread.Name = Source;
				thread.Start();
			}
		}

		public void SignalToStop()
		{
			if (thread != null)
			{
				stopEvent.Set();
			}
		}

		public void WaitForStop()
		{
			if (thread != null)
			{
				thread.Join();
				Free();
			}
		}

		public void Stop()
		{
			if (IsRunning)
			{
				stopEvent.Set();
				thread.Abort();
				WaitForStop();
			}
		}

		private void Free()
		{
			thread = null;
			stopEvent.Close();
			stopEvent = null;
		}

		private void WorkerThread()
		{
			int width = region.Width;
			int height = region.Height;
			int x = region.Location.X;
			int y = region.Location.Y;
			Size size = region.Size;
			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			Graphics graphics = Graphics.FromImage(bitmap);
			while (!stopEvent.WaitOne(0, exitContext: false))
			{
				DateTime now = DateTime.Now;
				try
				{
					graphics.CopyFromScreen(x, y, 0, 0, size, CopyPixelOperation.SourceCopy);
					framesReceived++;
					if (this.NewFrame != null)
					{
						this.NewFrame(this, new NewFrameEventArgs(bitmap));
					}
					if (frameInterval > 0)
					{
						TimeSpan timeSpan = DateTime.Now.Subtract(now);
						int num = frameInterval - (int)timeSpan.TotalMilliseconds;
						if (num > 0 && stopEvent.WaitOne(num, exitContext: false))
						{
							break;
						}
					}
				}
				catch (ThreadAbortException)
				{
					break;
				}
				catch (Exception ex2)
				{
					if (this.VideoSourceError != null)
					{
						this.VideoSourceError(this, new VideoSourceErrorEventArgs(ex2.Message));
					}
					Thread.Sleep(250);
				}
				if (stopEvent.WaitOne(0, exitContext: false))
				{
					break;
				}
			}
			graphics.Dispose();
			bitmap.Dispose();
			if (this.PlayingFinished != null)
			{
				this.PlayingFinished(this, ReasonToFinishPlaying.StoppedByUser);
			}
		}
	}
}
