using System;
using System.Runtime.InteropServices;

namespace AForge.Controls
{
	internal static class JoystickAPI
	{
		[StructLayout(LayoutKind.Sequential)]
		public class JOYINFO
		{
			public int xPos;

			public int yPos;

			public int zPos;

			public int buttons;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class JOYINFOEX
		{
			public int size;

			public JoyPosFlags flags;

			public int xPos;

			public int yPos;

			public int zPos;

			public int rPos;

			public int uPos;

			public int vPos;

			public int buttons;

			public int buttonNumber;

			public int pov;

			public int reserved1;

			public int reserved2;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
		public class JOYCAPS
		{
			public short mid;

			public short pid;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string name;

			public int xMin;

			public int xMax;

			public int yMin;

			public int yMax;

			public int zMin;

			public int zMax;

			public int buttonsNumber;

			public int minPeriod;

			public int maxPeriod;

			public int rMin;

			public int rMax;

			public int uMin;

			public int uMax;

			public int vMin;

			public int vMax;

			public int caps;

			public int axesMax;

			public int axesNumber;

			public int buttonsMax;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string regKey;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string oemVxD;
		}

		public enum ResultCode : uint
		{
			NoError = 0u,
			Error = 1u,
			BadDeviceID = 2u,
			NoDriver = 6u,
			InvalidParam = 11u,
			JoystickInvalidParam = 165u,
			JoystickRequestNotCompleted = 166u,
			JoystickUnplugged = 167u
		}

		[Flags]
		public enum JoyPosFlags
		{
			ReturnX = 0x1,
			ReturnY = 0x2,
			ReturnZ = 0x4,
			ReturnR = 0x8,
			ReturnU = 0x10,
			ReturnV = 0x20,
			ReturnPov = 0x40,
			ReturnButtons = 0x80,
			ReturnXY = 0x3,
			ReturnXYZ = 0x7,
			ReturnXYZR = 0xF,
			ReturnXYZRU = 0x1F,
			ReturnXYZRUV = 0x3F,
			ReturnAll = 0xFF
		}

		[Flags]
		public enum JoyButtons
		{
			Button1 = 0x1,
			Button2 = 0x2,
			Button3 = 0x4,
			Button4 = 0x8,
			Button5 = 0x10,
			Button6 = 0x20,
			Button7 = 0x40,
			Button8 = 0x80
		}

		public const int MM_JOY1MOVE = 928;

		public const int MM_JOY2MOVE = 929;

		public const int MM_JOY1ZMOVE = 930;

		public const int MM_JOY2ZMOVE = 931;

		public const int MM_JOY1BUTTONDOWN = 949;

		public const int MM_JOY2BUTTONDOWN = 950;

		public const int MM_JOY1BUTTONUP = 951;

		public const int MM_JOY2BUTTONUP = 952;

		[DllImport("winmm.dll")]
		public static extern int joyGetNumDevs();

		[DllImport("winmm.dll")]
		public static extern ResultCode joyGetDevCapsW(int uJoyID, [In] [Out] [MarshalAs(UnmanagedType.LPStruct)] JOYCAPS pjc, int cbjc);

		[DllImport("winmm.dll")]
		public static extern ResultCode joyGetPos(int uJoyID, JOYINFO pji);

		[DllImport("winmm.dll")]
		public static extern ResultCode joyGetPosEx(int uJoyID, JOYINFOEX pji);

		[DllImport("winmm.dll")]
		public static extern ResultCode joyReleaseCapture(int uJoyID);

		[DllImport("winmm.dll")]
		public static extern ResultCode joySetCapture(IntPtr hwnd, int uJoyID, int uPeriod, bool fChanged);
	}
}
