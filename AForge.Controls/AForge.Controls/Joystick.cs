using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AForge.Controls
{
	public class Joystick
	{
		public class DeviceInfo
		{
			public readonly int ID;

			internal readonly JoystickAPI.JOYCAPS capabilities;

			public string Name => capabilities.name;

			public int Axes => capabilities.axesNumber;

			public int Buttons => capabilities.buttonsNumber;

			internal DeviceInfo(int id, JoystickAPI.JOYCAPS joyCaps)
			{
				ID = id;
				capabilities = joyCaps;
			}
		}

		public class Status
		{
			private JoystickAPI.JOYINFOEX status;

			private JoystickAPI.JOYCAPS capabilities;

			public float XAxis
			{
				get
				{
					if ((status.flags & JoystickAPI.JoyPosFlags.ReturnX) != 0)
					{
						return (float)(status.xPos - capabilities.xMin) / (float)capabilities.xMax * 2f - 1f;
					}
					return 0f;
				}
			}

			public float YAxis
			{
				get
				{
					if ((status.flags & JoystickAPI.JoyPosFlags.ReturnY) != 0)
					{
						return (float)(status.yPos - capabilities.yMin) / (float)capabilities.yMax * 2f - 1f;
					}
					return 0f;
				}
			}

			public float ZAxis
			{
				get
				{
					if ((status.flags & JoystickAPI.JoyPosFlags.ReturnZ) != 0)
					{
						return (float)(status.zPos - capabilities.zMin) / (float)capabilities.zMax * 2f - 1f;
					}
					return 0f;
				}
			}

			public float RAxis
			{
				get
				{
					if ((status.flags & JoystickAPI.JoyPosFlags.ReturnR) != 0)
					{
						return (float)(status.rPos - capabilities.rMin) / (float)capabilities.rMax * 2f - 1f;
					}
					return 0f;
				}
			}

			public float UAxis
			{
				get
				{
					if ((status.flags & JoystickAPI.JoyPosFlags.ReturnU) != 0)
					{
						return (float)(status.uPos - capabilities.uMin) / (float)capabilities.uMax * 2f - 1f;
					}
					return 0f;
				}
			}

			public float VAxis
			{
				get
				{
					if ((status.flags & JoystickAPI.JoyPosFlags.ReturnV) != 0)
					{
						return (float)(status.vPos - capabilities.vMin) / (float)capabilities.vMax * 2f - 1f;
					}
					return 0f;
				}
			}

			public Buttons Buttons => (Buttons)status.buttons;

			public float PointOfView
			{
				get
				{
					if (status.pov <= 35900)
					{
						return (float)status.pov / 100f;
					}
					return -1f;
				}
			}

			internal Status(JoystickAPI.JOYINFOEX status, JoystickAPI.JOYCAPS capabilities)
			{
				this.status = status;
				this.capabilities = capabilities;
			}

			public bool IsButtonPressed(Buttons button)
			{
				return (status.buttons & (int)button) != 0;
			}
		}

		[Flags]
		public enum Buttons
		{
			Button1 = 0x1,
			Button2 = 0x2,
			Button3 = 0x4,
			Button4 = 0x8,
			Button5 = 0x10,
			Button6 = 0x20,
			Button7 = 0x40,
			Button8 = 0x80,
			Button9 = 0x100,
			Button10 = 0x200,
			Button11 = 0x400,
			Button12 = 0x800,
			Button13 = 0x1000,
			Button14 = 0x2000,
			Button15 = 0x4000,
			Button16 = 0x8000
		}

		private DeviceInfo info;

		private static JoystickAPI.JoyPosFlags[] requestFlags = new JoystickAPI.JoyPosFlags[6]
		{
			JoystickAPI.JoyPosFlags.ReturnPov | JoystickAPI.JoyPosFlags.ReturnButtons,
			JoystickAPI.JoyPosFlags.ReturnX | JoystickAPI.JoyPosFlags.ReturnPov | JoystickAPI.JoyPosFlags.ReturnButtons,
			JoystickAPI.JoyPosFlags.ReturnX | JoystickAPI.JoyPosFlags.ReturnY | JoystickAPI.JoyPosFlags.ReturnPov | JoystickAPI.JoyPosFlags.ReturnButtons,
			JoystickAPI.JoyPosFlags.ReturnX | JoystickAPI.JoyPosFlags.ReturnY | JoystickAPI.JoyPosFlags.ReturnZ | JoystickAPI.JoyPosFlags.ReturnPov | JoystickAPI.JoyPosFlags.ReturnButtons,
			JoystickAPI.JoyPosFlags.ReturnX | JoystickAPI.JoyPosFlags.ReturnY | JoystickAPI.JoyPosFlags.ReturnZ | JoystickAPI.JoyPosFlags.ReturnR | JoystickAPI.JoyPosFlags.ReturnPov | JoystickAPI.JoyPosFlags.ReturnButtons,
			JoystickAPI.JoyPosFlags.ReturnX | JoystickAPI.JoyPosFlags.ReturnY | JoystickAPI.JoyPosFlags.ReturnZ | JoystickAPI.JoyPosFlags.ReturnR | JoystickAPI.JoyPosFlags.ReturnU | JoystickAPI.JoyPosFlags.ReturnPov | JoystickAPI.JoyPosFlags.ReturnButtons
		};

		public DeviceInfo Info
		{
			get
			{
				if (info == null)
				{
					throw new ApplicationException("Joystick was not initialized.");
				}
				return info;
			}
		}

		public static List<DeviceInfo> GetAvailableDevices()
		{
			List<DeviceInfo> list = new List<DeviceInfo>();
			int cbjc = Marshal.SizeOf(typeof(JoystickAPI.JOYCAPS));
			int num = JoystickAPI.joyGetNumDevs();
			for (int i = 0; i < num; i++)
			{
				JoystickAPI.JOYCAPS jOYCAPS = new JoystickAPI.JOYCAPS();
				if (JoystickAPI.joyGetDevCapsW(i, jOYCAPS, cbjc) == JoystickAPI.ResultCode.NoError)
				{
					list.Add(new DeviceInfo(i, jOYCAPS));
				}
			}
			return list;
		}

		public Joystick()
		{
		}

		public Joystick(int id)
		{
			Init(id);
		}

		public void Init(int id)
		{
			if (id < 0 || id > 15)
			{
				throw new ArgumentException("Invalid joystick ID was specified.");
			}
			JoystickAPI.JOYCAPS jOYCAPS = new JoystickAPI.JOYCAPS();
			if (JoystickAPI.joyGetDevCapsW(id, jOYCAPS, Marshal.SizeOf(jOYCAPS)) != 0)
			{
				throw new NotConnectedException("The requested joystick is not connected to the system.");
			}
			info = new DeviceInfo(id, jOYCAPS);
		}

		public Status GetCurrentStatus()
		{
			JoystickAPI.JOYINFOEX jOYINFOEX = new JoystickAPI.JOYINFOEX();
			jOYINFOEX.size = Marshal.SizeOf(jOYINFOEX);
			jOYINFOEX.flags = ((Info.capabilities.axesNumber > 5) ? JoystickAPI.JoyPosFlags.ReturnAll : requestFlags[Info.capabilities.axesNumber]);
			if (JoystickAPI.joyGetPosEx(Info.ID, jOYINFOEX) != 0)
			{
				throw new NotConnectedException("The requested joystick is not connected to the system.");
			}
			return new Status(jOYINFOEX, Info.capabilities);
		}
	}
}
