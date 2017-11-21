using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using EasyHook;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Net;

namespace YTY.HookTest
{
  public class HookEntryPoint:IEntryPoint
  {
    private StreamWriter _sw;

    public HookEntryPoint(RemoteHooking.IContext context)
    {
    }

    public void Run(RemoteHooking.IContext context)
    {
      var pipe = new NamedPipeClientStream("HookPipe");
      pipe.Connect();
      _sw = new StreamWriter(pipe);
      _sw.AutoFlush = true;
      var hSend = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "send"), new SendD(SendH), this);
      hSend.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hSendTo = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "sendto"), new SendToD(SendToH), this);
      hSendTo.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hRecv = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "recv"), new RecvD(RecvH), this);
      hRecv.ThreadACL.SetExclusiveACL(new[] { 0 });
      RemoteHooking.WakeUpProcess();
      Thread.Sleep(-1);
    }

    private int SendH(IntPtr socket,string buff,int len,int flags)
    {
      //_sw.WriteLine($"[send]\t{socket}\t{len}\t{flags}");
      //_sw.WriteLine(buff);
      //_sw.WriteLine("================");
      return send(socket, buff, len, flags);
    }
    private int SendToH(IntPtr socket, string buff, int len, int flags, ref sockaddr_in to, int toLen)
    {
      //var ip = new IPEndPoint(new IPAddress(to.Addr),(int)(((uint) IPAddress.NetworkToHostOrder( to.Port))>>16));
      //_sw.WriteLine($"[sendto]\t{socket}\t{len}\t{flags}\t{ip}");
      //_sw.WriteLine(buff);
      //_sw.WriteLine("================");
      return sendto(socket, buff, len, flags,ref to, toLen);
    }

    private int RecvH(IntPtr socket, IntPtr buff, int len, int flags)
    {
      var ret= recv(socket, buff, len, flags);
      _sw.WriteLine($"[recv]\t{socket}\t{len}\t{flags}\tret={ret}");
      _sw.WriteLine(Marshal.PtrToStringAnsi( buff,ret));
      _sw.WriteLine("================");
      return ret;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate int SendD(IntPtr socket, string buff, int len, int flags);

    [DllImport("ws2_32",CharSet= CharSet.Ansi)]
    static extern int send(IntPtr socket, string buff, int len, int flags);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate int SendToD(IntPtr socket, string buff, int len, int flags,ref sockaddr_in to,int toLen);

    [DllImport("ws2_32",CharSet = CharSet.Ansi)]
    static extern int sendto(IntPtr socket, string buff, int len, int flags, ref sockaddr_in to, int toLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate int RecvD(IntPtr socket,IntPtr buff, int len, int flags);

    [DllImport("ws2_32", CharSet = CharSet.Ansi)]
    static extern int recv(IntPtr socket, IntPtr buff, int len, int flags);
  }

  [StructLayout (LayoutKind.Sequential)]
  public struct sockaddr_in
  {
    public short Family;
    public ushort Port;
    public uint Addr;
    public long Zero;
  }
}
