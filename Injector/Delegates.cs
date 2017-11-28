using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace YTY.HookTest
{
  internal static unsafe class Delegates
  {
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int SendD(IntPtr socket, sbyte* buff, int len, int flags);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int SendToD(IntPtr socket, sbyte* buff, int len, int flags, sockaddr_in* to, int toLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int RecvD(IntPtr socket, sbyte* buff, int len, int flags);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int RecvFromD(IntPtr socket, sbyte* buff, int len, int flags, sockaddr_in* from, int* fromLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void PostQuitMessageD(int exitCode);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate bool TextOutAD(IntPtr dc, int xStart, int yStart, sbyte* pString, int strLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate uint GetACPD();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int LoadStringD(IntPtr instance, uint id, sbyte* buffer, int bufferMax);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate IntPtr LoadLibraryAD(sbyte* pFileName);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int DrawTextAD(IntPtr dc, sbyte* pStr, int count, RECT* rect, DT_ format);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate IntPtr AcceptD(IntPtr socket, sockaddr_in* addr, int* addrLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int BindD(IntPtr socket, sockaddr_in* addr, int addrLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int ConnectD(IntPtr socket, sockaddr_in* addr, int addrLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int GetPeerNameD(IntPtr socket, sockaddr_in* addr, int* addrLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int GetSockNameD(IntPtr socket, sockaddr_in* addr, int* addrLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int CloseSocketD(IntPtr socket);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate IntPtr SocketD(AddressFamily af, SocketType type, ProtocolType protocol);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int DirectPlayCreateD(Guid* pGuid, void** ppDp, IntPtr pUnk);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate uint CoCreateInstanceD(Guid* clsid, IntPtr pUnkOuter, int clsContext, Guid* iid, int** ppv);
  }
}
