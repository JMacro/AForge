using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace AForge.Video
{
	public class AsyncVideoSource : IVideoSource
	{
		private readonly IVideoSource nestedVideoSource;

		private Bitmap lastVideoFrame;

		private Thread imageProcessingThread;

		private AutoResetEvent isNewFrameAvailable;

		private AutoResetEvent isProcessingThreadAvailable;

		private bool skipFramesIfBusy;

		private int framesProcessed;

		public IVideoSource NestedVideoSource => nestedVideoSource;

		public bool SkipFramesIfBusy
		{
			get
			{
				return skipFramesIfBusy;
			}
			set
			{
				skipFramesIfBusy = value;
			}
		}

		public string Source => nestedVideoSource.Source;

		public int FramesReceived => nestedVideoSource.FramesReceived;

		public long BytesReceived => nestedVideoSource.BytesReceived;

		public int FramesProcessed
		{
			get
			{
				int result = framesProcessed;
				framesProcessed = 0;
				return result;
			}
		}

		public bool IsRunning
		{
			get
			{
				bool isRunning = nestedVideoSource.IsRunning;
				if (!isRunning)
				{
					Free();
				}
				return isRunning;
			}
		}

		public event NewFrameEventHandler NewFrame;

		public event VideoSourceErrorEventHandler VideoSourceError
		{
			add
			{
				nestedVideoSource.VideoSourceError += value;
			}
			remove
			{
				nestedVideoSource.VideoSourceError -= value;
			}
		}

		public event PlayingFinishedEventHandler PlayingFinished
		{
			add
			{
				nestedVideoSource.PlayingFinished += value;
			}
			remove
			{
				nestedVideoSource.PlayingFinished -= value;
			}
		}

		public AsyncVideoSource(IVideoSource nestedVideoSource)
		{
			this.nestedVideoSource = nestedVideoSource;
		}

		public AsyncVideoSource(IVideoSource nestedVideoSource, bool skipFramesIfBusy)
		{
			this.nestedVideoSource = nestedVideoSource;
			this.skipFramesIfBusy = skipFramesIfBusy;
		}

		public void Start()
		{
			if (!IsRunning)
			{
				framesProcessed = 0;
				isNewFrameAvailable = new AutoResetEvent(initialState: false);
				isProcessingThreadAvailable = new AutoResetEvent(initialState: true);
				imageProcessingThread = new Thread(imageProcessingThread_Worker);
				imageProcessingThread.Start();
				nestedVideoSource.NewFrame += nestedVideoSource_NewFrame;
				nestedVideoSource.Start();
			}
		}

		public void SignalToStop()
		{
			nestedVideoSource.SignalToStop();
		}

		public void WaitForStop()
		{
			nestedVideoSource.WaitForStop();
			Free();
		}

		public void Stop()
		{
			nestedVideoSource.Stop();
			Free();
		}

		private void Free()
		{
			if (imageProcessingThread != null)
			{
				nestedVideoSource.NewFrame -= nestedVideoSource_NewFrame;
				isProcessingThreadAvailable.WaitOne();
				lastVideoFrame = null;
				isNewFrameAvailable.Set();
				imageProcessingThread.Join();
				imageProcessingThread = null;
				isNewFrameAvailable.Close();
				isNewFrameAvailable = null;
				isProcessingThreadAvailable.Close();
				isProcessingThreadAvailable = null;
			}
		}

		private void nestedVideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
		{
			if (this.NewFrame == null)
			{
				return;
			}
			if (skipFramesIfBusy)
			{
				if (!isProcessingThreadAvailable.WaitOne(0, exitContext: false))
				{
					return;
				}
			}
			else
			{
				isProcessingThreadAvailable.WaitOne();
			}
			lastVideoFrame = CloneImage(eventArgs.Frame);
			isNewFrameAvailable.Set();
		}

		private void imageProcessingThread_Worker()
		{
			while (true)
			{
				isNewFrameAvailable.WaitOne();
				if (lastVideoFrame == null)
				{
					break;
				}
				if (this.NewFrame != null)
				{
					this.NewFrame(this, new NewFrameEventArgs(lastVideoFrame));
				}
				lastVideoFrame.Dispose();
				lastVideoFrame = null;
				framesProcessed++;
				isProcessingThreadAvailable.Set();
			}
		}

		private static Bitmap CloneImage(Bitmap source)
		{
			BitmapData bitmapData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
			Bitmap bitmap = CloneImage(bitmapData);
			source.UnlockBits(bitmapData);
			if (source.PixelFormat == PixelFormat.Format1bppIndexed || source.PixelFormat == PixelFormat.Format4bppIndexed || source.PixelFormat == PixelFormat.Format8bppIndexed || source.PixelFormat == PixelFormat.Indexed)
			{
				ColorPalette palette = source.Palette;
				ColorPalette palette2 = bitmap.Palette;
				int num = palette.Entries.Length;
				for (int i = 0; i < num; i++)
				{
					palette2.Entries[i] = palette.Entries[i];
				}
				bitmap.Palette = palette2;
			}
			return bitmap;
		}

		private static Bitmap CloneImage(BitmapData sourceData)
		{
			int width = sourceData.Width;
			int height = sourceData.Height;
			Bitmap bitmap = new Bitmap(width, height, sourceData.PixelFormat);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
			SystemTools.CopyUnmanagedMemory(bitmapData.Scan0, sourceData.Scan0, height * sourceData.Stride);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}
	}
}
