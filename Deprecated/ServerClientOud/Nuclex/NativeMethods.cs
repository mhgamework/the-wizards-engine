using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security;
using System.Runtime.InteropServices;

namespace Nuclex {

  internal sealed class NativeMethods {
    // Methods
    private NativeMethods() {
    }

    [return: MarshalAs(UnmanagedType.Bool)]
    [SuppressUnmanagedCodeSecurity, DllImport("user32.dll")]
    internal static extern bool ClientToScreen(IntPtr hWnd, out POINT point);
    [return: MarshalAs(UnmanagedType.Bool)]
    [SuppressUnmanagedCodeSecurity, DllImport("user32.dll")]
    internal static extern bool GetClientRect(IntPtr hWnd, out RECT rect);
    [return: MarshalAs(UnmanagedType.Bool)]
    [SuppressUnmanagedCodeSecurity, DllImport("user32.dll")]
    internal static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
    [SuppressUnmanagedCodeSecurity, DllImport("user32.dll")]
    internal static extern IntPtr MonitorFromWindow(IntPtr hWnd, uint flags);
    [return: MarshalAs(UnmanagedType.Bool)]
    [SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);
    [return: MarshalAs(UnmanagedType.Bool)]
    [SuppressUnmanagedCodeSecurity, DllImport("kernel32")]
    internal static extern bool QueryPerformanceCounter(ref long PerformanceCount);
    [return: MarshalAs(UnmanagedType.Bool)]
    [SuppressUnmanagedCodeSecurity, DllImport("kernel32")]
    internal static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

    // Nested Types
    [StructLayout(LayoutKind.Sequential)]
    public struct Message {
      public IntPtr hWnd;
      public NativeMethods.WindowMessage msg;
      public IntPtr wParam;
      public IntPtr lParam;
      public uint time;
      public Point p;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MinMaxInformation {
      public Point reserved;
      public Point MaxSize;
      public Point MaxPosition;
      public Point MinTrackSize;
      public Point MaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MonitorInformation {
      public uint Size;
      public Rectangle MonitorRectangle;
      public Rectangle WorkRectangle;
      public uint Flags;
    }

    public enum MouseButtons {
      Left = 1,
      Middle = 0x10,
      Right = 2,
      Side1 = 0x20,
      Side2 = 0x40
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT {
      public int X;
      public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
      public int Left;
      public int Top;
      public int Right;
      public int Bottom;
    }

    internal enum WindowMessage : uint {
      ActivateApplication = 0x1c,
      Character = 0x102,
      Close = 0x10,
      Destroy = 2,
      EnterMenuLoop = 0x211,
      EnterSizeMove = 0x231,
      ExitMenuLoop = 530,
      ExitSizeMove = 0x232,
      GetMinMax = 0x24,
      KeyDown = 0x100,
      KeyUp = 0x101,
      LeftButtonDoubleClick = 0x203,
      LeftButtonDown = 0x201,
      LeftButtonUp = 0x202,
      MiddleButtonDoubleClick = 0x209,
      MiddleButtonDown = 0x207,
      MiddleButtonUp = 520,
      MouseFirst = 0x201,
      MouseLast = 0x20d,
      MouseMove = 0x200,
      MouseWheel = 0x20a,
      NonClientHitTest = 0x84,
      Paint = 15,
      PowerBroadcast = 0x218,
      Quit = 0x12,
      RightButtonDoubleClick = 0x206,
      RightButtonDown = 0x204,
      RightButtonUp = 0x205,
      SetCursor = 0x20,
      Size = 5,
      SystemCharacter = 0x106,
      SystemCommand = 0x112,
      SystemKeyDown = 260,
      SystemKeyUp = 0x105,
      XButtonDoubleClick = 0x20d,
      XButtonDown = 0x20b,
      XButtonUp = 0x20c
    }
  }

} // namespace Nuclex
