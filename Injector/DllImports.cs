using System;
using System.Text;
using System.Drawing;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace YTY.HookTest
{
  internal static unsafe class DllImports
  {
    #region ws2_32
    internal const int INVALID_SOCKET = -1;

    [DllImport("ws2_32")]
    internal static extern int accept(int socket, sockaddr_in* addr, int* addrLen);

    [DllImport("ws2_32")]
    internal static extern SocketError bind(int socket, sockaddr_in* addr, int addrLen);

    [DllImport("ws2_32")]
    internal static extern SocketError closesocket(int socket);

    [DllImport("ws2_32")]
    internal static extern SocketError connect(int socket, sockaddr_in* addr, int addrLen);

    [DllImport("ws2_32")]
    internal static extern HostEnt* gethostbyname(sbyte* name);

    [DllImport("ws2_32")]
    internal static extern SocketError gethostname(sbyte* name, int nameLen);

    [DllImport("ws2_32")]
    internal static extern SocketError getpeername(int socket, sockaddr_in* addr, int* addrLen);

    [DllImport("ws2_32")]
    internal static extern SocketError getsockname(int socket, sockaddr_in* addr, int* addrLen);

    [DllImport("ws2_32")]
    internal static extern SocketError listen(int socket, int backlog);

    [DllImport("ws2_32")]
    internal static extern int recv(int socket, sbyte* buff, int len, int flags);

    [DllImport("ws2_32")]
    internal static extern int recvfrom(int socket, sbyte* buff, int len, int flags, sockaddr_in* from, int* fromLen);

    [DllImport("ws2_32")]
    internal static extern int send(int socket, sbyte* buff, int len, int flags);

    [DllImport("ws2_32")]
    internal static extern int sendto(int socket, sbyte* buff, int len, int flags, sockaddr_in* to, int toLen);

    [DllImport("ws2_32")]
    internal static extern int socket(AddressFamily af, SocketType type, ProtocolType protocol);

    [DllImport("ws2_32")]
    internal static extern SocketError WSAGetLastError(); 
    #endregion

    #region user32
    [DllImport("user32")]
    internal static extern void PostQuitMessage(int exitCode);

    [DllImport("user32", ExactSpelling = true)]
    internal static extern int LoadStringA(IntPtr instance, uint id, sbyte* buffer, int bufferMax);

    [DllImport("user32", ExactSpelling = true)]
    internal static extern int LoadStringW(IntPtr instance, uint id, char* buffer, int bufferMax);

    [DllImport("user32", ExactSpelling = true)]
    internal static extern int DrawTextA(IntPtr dc, sbyte* pStr, int count, RECT* rect, DT_ format);
    #endregion

    #region gdi32
    [DllImport("gdi32", ExactSpelling = true)]
    internal static extern bool TextOutA(IntPtr dc, int xStart, int yStart, sbyte* pStr, int strLen);

    [DllImport("gdi32", ExactSpelling = true)]
    internal static extern bool TextOutW(IntPtr dc, int xStart, int yStart, char* pStr, int strLen);

    [DllImport("gdi32")]
    internal static extern IntPtr GetCurrentObject(IntPtr dc, OBJ_ objectType);

    [DllImport("gdi32")]
    internal static extern int GetObject(IntPtr obj, int buffer, IntPtr objInfo);

    [DllImport("gdi32")]
    internal static extern COLORREF GetTextColor(IntPtr dc);

    [DllImport("gdi32", ExactSpelling = true, CharSet = CharSet.Ansi)]
    internal static extern bool GetTextExtentPoint32A(IntPtr dc, string str, int strLen, out Size size);

    [DllImport("gdi32")]
    internal static extern TA_ GetTextAlign(IntPtr dc);
    #endregion

    #region kernel32
    [DllImport("kernel32")]
    internal static extern uint GetACP();

    [DllImport("kernel32")]
    internal static extern bool CreateProcessA(sbyte* applicationName, sbyte* commandLine, IntPtr processAttributes, IntPtr threadAttributes, bool inheritHandles, uint creationFlags, IntPtr environment, sbyte* currentDirectory, IntPtr startupInfo, ProcessInformation* processInformation);
    #endregion

    #region dplayx
    internal static Guid DPSPGUID_TCPIP = new Guid(0x36E95EE0, 0x8577, 0x11cf, 0x96, 0xc, 0x0, 0x80, 0xc7, 0x53, 0x4e, 0x82);
    internal static Guid IID_IDirectPlay4 = new Guid(0xab1c530, 0x4745, 0x11d1, 0xa7, 0xa1, 0x0, 0x0, 0xf8, 0x3, 0xab, 0xfc);

    [DllImport("dplayx")]
    internal static extern int DirectPlayCreate(Guid* pGuid, void** ppDp, IntPtr pUnk);
    #endregion

    #region ole32
    [DllImport("ole32")]
    internal static extern uint CoCreateInstance(Guid* clsid, IntPtr pUnkOuter, int clsContext, Guid* iid, int** ppv);
    #endregion
  }
}
