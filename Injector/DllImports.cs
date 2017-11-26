﻿using System;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace YTY.HookTest
{
  internal static unsafe class DllImports
  {
    internal static IntPtr INVALID_SOCKET = new IntPtr(-1);

    [DllImport("ws2_32")]
    internal static extern int send(IntPtr socket, sbyte* buff, int len, int flags);

    [DllImport("ws2_32")]
    internal static extern int sendto(IntPtr socket, sbyte* buff, int len, int flags, sockaddr_in* to, int toLen);

    [DllImport("ws2_32")]
    internal static extern int recv(IntPtr socket, sbyte* buff, int len, int flags);

    [DllImport("ws2_32")]
    internal static extern int recvfrom(IntPtr socket, sbyte* buff, int len, int flags, sockaddr_in* from, int* fromLen);

    [DllImport("user32")]
    internal static extern void PostQuitMessage(int exitCode);

    [DllImport("gdi32", ExactSpelling = true)]
    internal static extern bool TextOutA(IntPtr dc, int xStart, int yStart, sbyte* pStr, int strLen);

    [DllImport("gdi32", ExactSpelling = true)]
    internal static extern bool TextOutW(IntPtr dc, int xStart, int yStart, char* pStr, int strLen);

    [DllImport("kernel32")]
    internal static extern uint GetACP();

    [DllImport("user32", ExactSpelling = true)]
    internal static extern int LoadStringA(IntPtr instance, uint id, sbyte* buffer, int bufferMax);

    [DllImport("user32", ExactSpelling = true)]
    internal static extern int LoadStringW(IntPtr instance, uint id, char* buffer, int bufferMax);

    [DllImport("gdi32")]
    internal static extern IntPtr GetCurrentObject(IntPtr dc, OBJ_ objectType);

    [DllImport("gdi32")]
    internal static extern int GetObject(IntPtr obj, int buffer, IntPtr objInfo);

    [DllImport("gdi32")]
    internal static extern COLORREF GetTextColor(IntPtr dc);

    [DllImport("user32", ExactSpelling = true)]
    internal static extern int DrawTextA(IntPtr dc, sbyte* pStr, int count, RECT* rect, DT_ format);

    [DllImport("gdi32", ExactSpelling = true, CharSet = CharSet.Ansi)]
    internal static extern bool GetTextExtentPoint32A(IntPtr dc, string str, int strLen, out Size size);

    [DllImport("gdi32")]
    internal static extern TA_ GetTextAlign(IntPtr dc);

    [DllImport("ws2_32")]
    internal static extern IntPtr accept(IntPtr socket, sockaddr_in* addr, int* addrLen);

    [DllImport("ws2_32")]
    internal static extern int bind(IntPtr socket, sockaddr_in* addr, int addrLen);

    [DllImport("ws2_32")]
    internal static extern int connect(IntPtr socket, sockaddr_in* addr, int addrLen);

    [DllImport("ws2_32")]
    internal static extern int getpeername(IntPtr socket, sockaddr_in* addr, int* addrLen);

    [DllImport("ws2_32")]
    internal static extern int getsockname(IntPtr socket, sockaddr_in* addr, int* addrLen);

    [DllImport("ws2_32")]
    internal static extern int closesocket(IntPtr socket);
  }
}
