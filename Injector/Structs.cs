using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Net;

namespace YTY.HookTest
{

  [StructLayout(LayoutKind.Sequential)]
  public struct sockaddr_in
  {
    public short Family;
    public ushort Port;
    public uint Addr;
    public long Zero;

    public IPEndPoint ToIPEndPoint()
    {
      return new IPEndPoint(new IPAddress(Addr), (int)(((uint)IPAddress.NetworkToHostOrder(Port)) >> 16));
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct LOGBRUSH
  {
    public uint Style;
    public COLORREF Color;
    public long Hatch;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct COLORREF
  {
    public byte Red;
    public byte Green;
    public byte Blue;
    public byte Zero;

    public Color ToColor()
    {
      return Color.FromArgb(Red, Green, Blue);
    }

    public Color ToColor(byte alpha)
    {
      return Color.FromArgb(alpha, Red, Green, Blue);
    }

    public override string ToString()
    {
      return ToColor().ToString();
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct RECT
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public Rectangle ToRectangle()
    {
      return new Rectangle(Left, Top, Right - Left, Bottom - Top);
    }

    public RectangleF ToRectangleF()
    {
      return RectangleF.FromLTRB(Left, Top, Right, Bottom);
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  public unsafe struct HostEnt
  {
    public sbyte* Name;
    public sbyte** Aliases;
    public short AddrType;
    public short Length;
    public uint** AddrList;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct ProcessInformation
  {
    public IntPtr hProcess;
    public IntPtr hThread;
    public uint ProcessId;
    public uint ThreadId;
  }

  [StructLayout(LayoutKind.Sequential)]
  public unsafe struct WSABUF
  {
    public uint Len;
    public sbyte* Buf;
  }
}
