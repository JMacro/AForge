using System;
using System.Runtime.InteropServices;

namespace AForge.Video.DirectShow.Internals
{
	[Flags]
	[ComVisible(false)]
	internal enum VideoControlFlags
	{
		FlipHorizontal = 0x1,
		FlipVertical = 0x2,
		ExternalTriggerEnable = 0x4,
		Trigger = 0x8
	}
}
