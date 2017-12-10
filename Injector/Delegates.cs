using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace YTY.HookTest
{
  internal static unsafe class Delegates
  {
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int AcceptD(int socket, sockaddr_in* addr, int* addrLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate SocketError BindD(int socket, sockaddr_in* addr, int addrLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate SocketError CloseSocketD(int socket);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate SocketError ConnectD(int socket, sockaddr_in* addr, int addrLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate HostEnt* GetHostByNameD(sbyte* name);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate SocketError GetHostNameD(sbyte* name, int nameLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate SocketError GetPeerNameD(int socket, sockaddr_in* addr, int* addrLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate SocketError GetSockNameD(int socket, sockaddr_in* addr, int* addrLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate SocketError ListenD(int socket, int backlog);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int RecvD(int socket, sbyte* buff, int len, int flags);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int RecvFromD(int socket, sbyte* buff, int len, int flags, sockaddr_in* from, int* fromLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int SendD(int socket, sbyte* buff, int len, int flags);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int SendToD(int socket, sbyte* buff, int len, int flags, sockaddr_in* to, int toLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int SocketD(AddressFamily af, SocketType type, ProtocolType protocol);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate SocketError WSARecvFromD(int socket, WSABUF* pBuffers, uint bufferCount, uint* pNumberOfBytesRecvd, uint* pFlags, sockaddr_in* pFrom, int* pFromLen, IntPtr pOverlapped, IntPtr pCompletionRoutine);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate SocketError WSASendToD(int socket, WSABUF* pBuffers, uint bufferCount, uint* pNumberOfBytesSent,uint flags, sockaddr_in* pTo, int toLen, IntPtr pOverlapped, IntPtr pCompletionRoutine);

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
    internal delegate int DirectPlayCreateD(Guid* pGuid, void** ppDp, IntPtr pUnk);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate uint CoCreateInstanceD(Guid* clsid, IntPtr pUnkOuter, int clsContext, Guid* iid, int** ppv);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate bool CreateProcessAD(sbyte* applicationName, sbyte* commandLine, IntPtr processAttributes,IntPtr threadAttributes, bool inheritHandles, uint creationFlags, IntPtr environment, sbyte* currentDirectory,IntPtr startupInfo, ProcessInformation* processInformation);
  }
}
