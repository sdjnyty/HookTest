using System;
using System.Text;
using System.Runtime.InteropServices;

namespace YTY.HookTest
{
  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  internal delegate int SendD(IntPtr socket, string buff, int len, int flags);

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  internal delegate int SendToD(IntPtr socket, string buff, int len, int flags, ref sockaddr_in to, int toLen);

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  internal delegate int RecvD(IntPtr socket, IntPtr buff, int len, int flags);

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  internal delegate void PostQuitMessageD(int exitCode);

  [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
  internal delegate bool TextOutAD(IntPtr dc, int xStart, int yStart, string pStr, int strLen);

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  internal delegate uint GetACPD();

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  internal delegate int LoadStringD(IntPtr instance, uint id,IntPtr buffer, int bufferMax);

  [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet= CharSet.Ansi)]
  internal delegate IntPtr LoadLibraryAD(string fileName);

  [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
  internal delegate int DrawTextAD(IntPtr dc, string str, int count, ref RECT rect, DT_ format);

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  internal delegate IntPtr AcceptD(IntPtr socket, out sockaddr_in addr, out int addrLen);
}
