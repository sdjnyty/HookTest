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
  public class HookEntryPoint : IEntryPoint
  {
    private StreamWriter _sw;
    private IniParser.Model.KeyDataCollection _kvs;
    private HashSet<IntPtr> _dlls = new HashSet<IntPtr>();

    public HookEntryPoint(RemoteHooking.IContext context)
    {
    }

    public void Run(RemoteHooking.IContext context)
    {
      _kvs = new IniParser.FileIniDataParser().ReadFile("language.dll.ini", Encoding.UTF8).Sections["Language.dll"];

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
      var hPostQuitMessage = LocalHook.Create(LocalHook.GetProcAddress("user32", "PostQuitMessage"), new PostQuitMessageD(PostQuitMessageH), this);
      hPostQuitMessage.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hTextOut = LocalHook.Create(LocalHook.GetProcAddress("gdi32", "TextOutA"), new TextOutD(TextOutH), this);
      hTextOut.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hGetACP = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "GetACP"), new GetACPD(GetACPH), this);
      hGetACP.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hLoadString = LocalHook.Create(LocalHook.GetProcAddress("user32", "LoadStringA"), new LoadStringD(LoadStringH), this);
      hLoadString.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hLoadLibrary = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "LoadLibraryA"), new LoadLibraryD(LoadLibraryH), this);
      hLoadLibrary.ThreadACL.SetExclusiveACL(new[] { 0 });
      RemoteHooking.WakeUpProcess();
      Thread.Sleep(-1);
    }

    private int SendH(IntPtr socket, string buff, int len, int flags)
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
      return sendto(socket, buff, len, flags, ref to, toLen);
    }

    private int RecvH(IntPtr socket, IntPtr buff, int len, int flags)
    {
      var ret = recv(socket, buff, len, flags);
      //_sw.WriteLine($"[recv]\t{socket}\t{len}\t{flags}\tret={ret}");
      //_sw.WriteLine(Marshal.PtrToStringAnsi(buff, ret));
      //_sw.WriteLine("================");
      return ret;
    }

    private void PostQuitMessageH(int exitCode)
    {
      _sw.WriteLine("[PostQuitMessage]");
      PostQuitMessage(exitCode);
    }

    private bool TextOutH(IntPtr dc, int xStart, int yStart, IntPtr pStr, int strLen)
    {
      var bytes = new byte[strLen];
      Marshal.Copy(pStr, bytes, 0, strLen);
      _sw.WriteLine(BitConverter.ToString(bytes));
      var str = Encoding.GetEncoding(949).GetString(bytes);
      _sw.WriteLine($"[TextOut]\t{xStart}\t{yStart}\t{str}");
      return TextOutW(dc, xStart, yStart, str, str.Length);
    }

    private uint GetACPH()
    {
      _sw.WriteLine($"[GetACP]\t{GetACP()}");
      return 1252;
    }

    private int LoadStringH(IntPtr instance, uint id, IntPtr buffer, int bufferMax)
    {
      
      var p=Marshal.AllocHGlobal(bufferMax * 2);
      
      var ret = LoadStringW(instance, id, p, bufferMax);
      if (ret > 0)
      {
        var y = new byte[ret*2];
        Marshal.Copy(p, y, 0, ret*2);
        var s = Encoding.Unicode.GetString(y) + '\0';
        var ary = Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding(949), Encoding.Unicode.GetBytes(s));
        Marshal.Copy(ary, 0, buffer, ary.Length);
      }
      Marshal.FreeHGlobal(p);
      return ret;

      if (!_dlls.Contains(instance))
      {
        return LoadStringA(instance, id, buffer, bufferMax);
      }

      if (_kvs.ContainsKey(id.ToString()))
      {
        var str = _kvs[id.ToString()] + '\0';
        //_sw.WriteLine(str);
        var bytes = Encoding.GetEncoding(936).GetBytes(str);

        if (bufferMax > bytes.Length)
        {
          Marshal.Copy(bytes, 0, buffer, bytes.Length);
          //_sw.WriteLine($"[LoadStringA]\t{id}\t{Marshal.PtrToStringUni(buffer)}\t{bufferMax}");
          return bytes.Length;
        }
        else
        {
          return 0;
        }
      }
      else
      {
        return 0;
      }
    }

    private IntPtr LoadLibraryH(string fileName)
    {
      //_sw.WriteLine($"[LoadLibraryA]\t{fileName}");
      var ret = NativeAPI.LoadLibrary(fileName);
      if (fileName.Equals("language.dll", StringComparison.InvariantCultureIgnoreCase)
        || fileName.Equals("language_x1.dll", StringComparison.InvariantCultureIgnoreCase)
        || fileName.Equals("language_x1_p1.dll", StringComparison.InvariantCultureIgnoreCase))
      {
        _dlls.Add(ret);
      }
      return ret;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate int SendD(IntPtr socket, string buff, int len, int flags);

    [DllImport("ws2_32", CharSet = CharSet.Ansi)]
    static extern int send(IntPtr socket, string buff, int len, int flags);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate int SendToD(IntPtr socket, string buff, int len, int flags, ref sockaddr_in to, int toLen);

    [DllImport("ws2_32", CharSet = CharSet.Ansi)]
    static extern int sendto(IntPtr socket, string buff, int len, int flags, ref sockaddr_in to, int toLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate int RecvD(IntPtr socket, IntPtr buff, int len, int flags);

    [DllImport("ws2_32", CharSet = CharSet.Ansi)]
    static extern int recv(IntPtr socket, IntPtr buff, int len, int flags);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate void PostQuitMessageD(int exitCode);

    [DllImport("user32")]
    static extern void PostQuitMessage(int exitCode);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate bool TextOutD(IntPtr dc, int xStart, int yStart, IntPtr pStr, int strLen);

    [DllImport("gdi32", ExactSpelling = true)]
    static extern bool TextOutA(IntPtr dc, int xStart, int yStart, IntPtr str, int strLen);

    [DllImport("gdi32", ExactSpelling = true,CharSet = CharSet.Unicode)]
    static extern bool TextOutW(IntPtr dc, int xStart, int yStart, string str, int strLen);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate uint GetACPD();

    [DllImport("kernel32")]
    static extern uint GetACP();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate int LoadStringD(IntPtr instance, uint id, IntPtr buffer, int bufferMax);

    [DllImport("user32", ExactSpelling = true)]
    static extern int LoadStringA(IntPtr instance, uint id, IntPtr buffer, int bufferMax);

    [DllImport("user32", ExactSpelling = true)]
    static extern int LoadStringW(IntPtr instance, uint id, IntPtr buffer, int bufferMax);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate IntPtr LoadLibraryD(string fileName);
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct sockaddr_in
  {
    public short Family;
    public ushort Port;
    public uint Addr;
    public long Zero;
  }
}
