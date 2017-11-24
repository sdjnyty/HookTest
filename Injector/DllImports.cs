using System;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace YTY.HookTest
{
  internal static class DllImports
  {
    [DllImport("ws2_32")]
    internal static extern int send(IntPtr socket, byte[] buff, int len, int flags);

    [DllImport("ws2_32")]
    internal static extern int sendto(IntPtr socket, byte[] buff, int len, int flags, ref sockaddr_in to, int toLen);

    [DllImport("ws2_32")]
    internal static extern int recv(IntPtr socket, byte[] buff, int len, int flags);

    [DllImport("user32")]
    internal static extern void PostQuitMessage(int exitCode);

    [DllImport("gdi32", ExactSpelling = true, CharSet = CharSet.Ansi)]
    internal static extern bool TextOutA(IntPtr dc, int xStart, int yStart, string str, int strLen);

    [DllImport("gdi32", ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern bool TextOutW(IntPtr dc, int xStart, int yStart, string str, int strLen);

    [DllImport("kernel32")]
    internal static extern uint GetACP();

    [DllImport("user32", ExactSpelling = true,CharSet = CharSet.Ansi)]
    internal static extern int LoadStringA(IntPtr instance, uint id, StringBuilder buffer, int bufferMax);

    [DllImport("user32", ExactSpelling = true)]
    internal static extern int LoadStringW(IntPtr instance, uint id, IntPtr buffer, int bufferMax);

    [DllImport("gdi32")]
    internal static extern IntPtr GetCurrentObject(IntPtr dc, OBJ_ objectType);

    [DllImport("gdi32")]
    internal static extern int GetObject(IntPtr obj, int buffer, IntPtr objInfo);

    [DllImport("gdi32")]
    internal static extern COLORREF GetTextColor(IntPtr dc);

    [DllImport("user32", ExactSpelling = true)]
    internal static extern int DrawTextA(IntPtr dc, string str, int count, ref RECT rect, DT_ format);

    [DllImport("gdi32", ExactSpelling = true, CharSet = CharSet.Ansi)]
    internal static extern bool GetTextExtentPoint32A(IntPtr dc, string str, int strLen, out Size size);

    [DllImport("gdi32")]
    internal static extern TA_ GetTextAlign(IntPtr dc);

    [DllImport("ws2_32")]
    internal static extern IntPtr accept(IntPtr socket, out sockaddr_in addr, out int addrLen);

    [DllImport("ws2_32")]
    internal static extern int bind(IntPtr socket, ref sockaddr_in addr, int addrLen);

  }
}
