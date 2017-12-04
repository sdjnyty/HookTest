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

    [DllImport("dplayx")]
    internal static extern int DirectPlayCreate(Guid* pGuid, void** ppDp, IntPtr pUnk);

    [DllImport("ole32")]
    internal static extern uint CoCreateInstance(Guid* clsid, IntPtr pUnkOuter, int clsContext, Guid* iid, int** ppv);

    internal static Guid DPSPGUID_TCPIP =new Guid(0x36E95EE0, 0x8577, 0x11cf, 0x96, 0xc, 0x0, 0x80, 0xc7, 0x53, 0x4e, 0x82);
    internal static Guid IID_IDirectPlay4 = new Guid(0xab1c530, 0x4745, 0x11d1, 0xa7, 0xa1, 0x0, 0x0, 0xf8, 0x3, 0xab, 0xfc);

    [DllImport("ws2_32")]
    internal static extern HostEnt* gethostbyname(sbyte* name);

    [DllImport("ws2_32")]
    internal static extern int gethostname(sbyte* name, int nameLen);

    [DllImport("ws2_32")]
    internal static extern int listen(IntPtr socket, int backlog);

    [DllImport("kernel32")]
    internal static extern bool CreateProcessA(sbyte* applicationName, sbyte* commandLine, IntPtr processAttributes, IntPtr threadAttributes, bool inheritHandles, uint creationFlags, IntPtr environment, sbyte* currentDirectory, IntPtr startupInfo, ProcessInformation* processInformation);
  }
}
