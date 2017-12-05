﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyHook;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Net;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using static YTY.HookTest.Delegates;

namespace YTY.HookTest
{
  public unsafe class HookEntryPoint : IEntryPoint
  {
    private const string AGE2_X1 = "age2_x1";
    private const string DPLAYSVR = "dplaysvr";

    private Process _currentProcess;
    private InjectArgs _injectArgs;
    private StreamWriter _sw;
    private IniParser.Model.KeyDataCollection _kvs;
    private readonly HashSet<IntPtr> _dlls = new HashSet<IntPtr>();
    private readonly BlockingCollection<string> _q = new BlockingCollection<string>();
    private ConcurrentDictionary<IntPtr, Socket> _sockets = new ConcurrentDictionary<IntPtr, Socket>();
    private string _fakeHostName = "条顿武士";
    private string _trueHostName;
    private int _ip = BitConverter.ToInt32(new byte[] { 10, 11, 12, 13 }, 0);

    public HookEntryPoint(RemoteHooking.IContext context, InjectArgs injectArgs)
    {
      _injectArgs = injectArgs;
      _currentProcess = Process.GetCurrentProcess();
    }

    public void Run(RemoteHooking.IContext context, InjectArgs injectArgs)
    {
      if (_currentProcess.ProcessName.Equals(AGE2_X1, StringComparison.InvariantCultureIgnoreCase))
      {
        _kvs = new IniParser.FileIniDataParser().ReadFile("language.dll.ini", Encoding.UTF8).Sections["Language.dll"];
        //var hTextOut = LocalHook.Create(LocalHook.GetProcAddress("gdi32", "TextOutA"), new TextOutAD(TextOutH), this);
        //hTextOut.ThreadACL.SetExclusiveACL(new[] { 0 });
        //var hLoadString = LocalHook.Create(LocalHook.GetProcAddress("user32", "LoadStringA"), new LoadStringD(LoadStringH), this);
        //hLoadString.ThreadACL.SetExclusiveACL(new[] { 0 });
        //var hLoadLibrary = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "LoadLibraryA"), new LoadLibraryAD(LoadLibraryH), this);
        //hLoadLibrary.ThreadACL.SetExclusiveACL(new[] { 0 });
        //var hDrawTextA = LocalHook.Create(LocalHook.GetProcAddress("user32", "DrawTextA"), new DrawTextAD(DrawTextH), this);
        //hDrawTextA.ThreadACL.SetExclusiveACL(new[] { 0 });
        var hCreateProcessA = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "CreateProcessA"), new CreateProcessAD(CreateProcessAH), this);
        hCreateProcessA.ThreadACL.SetExclusiveACL(new[] { 0 });
      }
      var pipe = new NamedPipeClientStream("HookPipe");
      pipe.Connect();
      _sw = new StreamWriter(pipe);
      _sw.AutoFlush = true;
      //var hPostQuitMessage = LocalHook.Create(LocalHook.GetProcAddress("user32", "PostQuitMessage"), new PostQuitMessageD(PostQuitMessageH), this);
      //hPostQuitMessage.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hGetACP = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "GetACP"), new GetACPD(GetACPH), this);
      //hGetACP.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hAccept = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "accept"), new AcceptD(acceptH), this);
      //hAccept.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hSocket = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "socket"), new SocketD(SocketH), this);
      hSocket.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hCloseSocket = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "closesocket"), new CloseSocketD(CloseSocketH), this);
      hCloseSocket.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hBind = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "bind"), new BindD(BindH), this);
      hBind.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hConnect = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "connect"), new ConnectD(ConnectH), this);
      //hConnect.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hGetPeerName = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "getpeername"), new GetPeerNameD(GetPeerNameH), this);
      //hGetPeerName.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hGetSockName = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "getsockname"), new GetSockNameD(GetSockNameH), this);
      //hGetSockName.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hSend = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "send"), new SendD(SendH), this);
      //hSend.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hSendTo = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "sendto"), new SendToD(SendToH), this);
      //hSendTo.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hRecv = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "recv"), new RecvD(RecvH), this);
      //hRecv.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hRecvFrom = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "recvfrom"), new RecvFromD(RecvFromH), this);
      //hRecvFrom.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hDirectPlayCreate = LocalHook.Create(LocalHook.GetProcAddress("dplayx", "DirectPlayCreate"), new DirectPlayCreateD(DirectPlayCreateH), this);
      //hDirectPlayCreate.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hCoCreateInstance = LocalHook.Create(LocalHook.GetProcAddress("ole32", "CoCreateInstance"), new CoCreateInstanceD(CoCreateInstanceH), this);
      //hCoCreateInstance.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hGetHostByName = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "gethostbyname"), new GetHostByNameD(GetHostByNameH), this);
      hGetHostByName.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hGetHostName = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "gethostname"), new GetHostNameD(GetHostNameH), this);
      hGetHostName.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hListen = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "listen"), new ListenD(ListenH), this);
      hListen.ThreadACL.SetExclusiveACL(new[] { 0 });
      Task.Run(() =>
      {
        while (true)
        {
          _q.TryTake(out var s, -1);
          _sw.Write(s);
        }
      });
      RemoteHooking.WakeUpProcess();
      Thread.Sleep(-1);
    }

    private MakeArgs DebugOutput([System.Runtime.CompilerServices.CallerMemberName()]string caller = "")
    {
      return objs => _q.Add($"{_currentProcess.ProcessName.PadRight(9)}[{caller.PadRight(15)}] {string.Join("\t", objs)}\n");
    }

    private delegate void MakeArgs(params object[] args);

    private void PostQuitMessageH(int exitCode)
    {
      DebugOutput()();
      DllImports.PostQuitMessage(exitCode);
    }

    private bool TextOutH(IntPtr dc, int xStart, int yStart, sbyte* pString, int strLen)
    {
      var g = Graphics.FromHdc(dc);
      var align = DllImports.GetTextAlign(dc);
      var color = DllImports.GetTextColor(dc).ToColor();
      var font = Font.FromHdc(dc);
      var point = new PointF(xStart, yStart);
      var str = new string(pString);
      StringFormat sf;
      if (align.HasFlag(TA_.TA_BOTTOM))
      {
        sf = SF_Bottom;
      }
      else if (align.HasFlag(TA_.TA_RIGHT))
      {
        sf = SF_Right;
      }
      else
      {
        sf = StringFormat.GenericDefault;
      }
      g.DrawString(str, font, new SolidBrush(color), point, sf);
      DebugOutput()(font, color, point, align, str);
      return true;
      //return TextOutW(dc, xStart, yStart, str, str.Length);
    }

    private uint GetACPH()
    {
      DebugOutput()(DllImports.GetACP());
      return 1252;
    }

    private int LoadStringH(IntPtr instance, uint id, sbyte* buffer, int bufferMax)
    {
      #region get unicode

      //var p = Marshal.AllocHGlobal(bufferMax * 2);

      //var ret = LoadStringW(instance, id, p, bufferMax);
      //if (ret > 0)
      //{
      //  var y = new byte[ret * 2];
      //  Marshal.Copy(p, y, 0, ret * 2);
      //  var s = Encoding.Unicode.GetString(y) + '\0';
      //  var ary = Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding(949), Encoding.Unicode.GetBytes(s));
      //  Marshal.Copy(ary, 0, buffer, ary.Length);
      //}
      //Marshal.FreeHGlobal(p);
      //return ret;

      #endregion

      #region get from ini

      if (!_dlls.Contains(instance))
      {
        return DllImports.LoadStringA(instance, id, buffer, bufferMax);
      }

      if (_kvs.ContainsKey(id.ToString()))
      {
        var str = _kvs[id.ToString()].Replace(@"\n", "\n") + '\0';
        //_q.Add($"{str}\n");
        var bytes = Encoding.Default.GetBytes(str);

        if (bufferMax > bytes.Length)
        {
          Marshal.Copy(bytes, 0, new IntPtr(buffer), bytes.Length);
          //_q.Add($"[LoadStringA]\t{id}\t{Marshal.PtrToStringUni(buffer)}\t{bufferMax}\n");
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

      #endregion
    }

    private IntPtr LoadLibraryH(sbyte* pFileName)
    {
      var fileName = new string(pFileName);
      DebugOutput()(fileName);
      var ret = NativeAPI.LoadLibrary(fileName);
      if (fileName.Equals("language.dll", StringComparison.InvariantCultureIgnoreCase)
        || fileName.Equals("language_x1.dll", StringComparison.InvariantCultureIgnoreCase)
        || fileName.Equals("language_x1_p1.dll", StringComparison.InvariantCultureIgnoreCase))
      {
        _dlls.Add(ret);
      }
      return ret;
    }

    private readonly StringFormat SF_Bottom = new StringFormat { LineAlignment = StringAlignment.Far };
    private readonly StringFormat SF_Right = new StringFormat { Alignment = StringAlignment.Far };

    private int DrawTextH(IntPtr dc, sbyte* pStr, int count, RECT* rect, DT_ format)
    {
      var g = Graphics.FromHdc(dc);
      var font = Font.FromHdc(dc);
      var str = new string(pStr);
      var rectF = rect->ToRectangleF();
      var color = DllImports.GetTextColor(dc).ToColor();
      StringFormat sf;
      if (format.HasFlag(DT_.DT_BOTTOM))
      {
        sf = SF_Bottom;
      }
      else if (format.HasFlag(DT_.DT_RIGHT))
      {
        sf = SF_Right;
      }
      else
      {
        sf = StringFormat.GenericDefault;
      }
      g.DrawString(str, font, new SolidBrush(color), rectF, sf);
      DebugOutput()(format, font, color, rectF, str);
      return (int)rectF.Height;
    }

    private IntPtr acceptH(IntPtr socket, sockaddr_in* addr, int* addrLen)
    {
      var ret = DllImports.accept(socket, addr, addrLen);
      DebugOutput()(socket, addr->ToIPEndPoint());
      return ret;
    }

    private SocketError BindH(IntPtr socket, sockaddr_in* addr, int addrLen)
    {
      //var ret = DllImports.bind(socket, addr, addrLen);
      //if (ret == 0)
      //{
      //  _q.Add($"[bind]{socket}\t{addr->}\n");
      //}
      //return ret;
      try
      {
        var s = _sockets[socket];
        s.Bind(addr->ToIPEndPoint());
        DebugOutput()(s.Handle, s.ProtocolType, s.LocalEndPoint);
        return SocketError.Success;
      }
      catch
      {
        return SocketError.SocketError;
      }
    }

    private SocketError ConnectH(IntPtr socket, sockaddr_in* addr, int addrLen)
    {
      var ep = addr->ToIPEndPoint();
      DebugOutput()(socket, ep);
      try
      {
        _sockets[socket].Connect(ep);
        return SocketError.Success;
      }
      catch (SocketException ex)
      {
        DebugOutput()(ex);
        return SocketError.SocketError;
      }
    }

    private int GetPeerNameH(IntPtr socket, sockaddr_in* addr, int* addrLen)
    {
      _q.Add($"[getpeername]{socket}\t");
      var rep = _sockets[socket].RemoteEndPoint.ToBytes();
      for (var i = 0; i < rep.Length; i++)
      {
        ((byte*)addr)[i] = rep[i];
      }
      *addrLen = rep.Length;
      _q.Add($"{_sockets[socket].RemoteEndPoint}\n");
      return (int)SocketError.Success;
      var ret = DllImports.getpeername(socket, addr, addrLen);
      _q.Add($"[getpeername]{socket}\t{addr->ToIPEndPoint()}");
      return ret;
    }

    private int GetSockNameH(IntPtr socket, sockaddr_in* addr, int* addrLen)
    {
      _q.Add($"[getsockname]{socket}\t");
      try
      {
        var rep = _sockets[socket].LocalEndPoint.ToBytes();
        _q.Add(BitConverter.ToString(rep));
        for (var i = 0; i < rep.Length; i++)
        {
          ((byte*)addr)[i] = rep[i];
        }
        *addrLen = rep.Length;
        _q.Add($"{_sockets[socket].LocalEndPoint}\n");
        return (int)SocketError.Success;
      }
      catch (SocketException ex)
      {
        _q.Add($"{ex}\n");
        return -1; //(int)ex.SocketErrorCode;
      }
      catch (NullReferenceException)
      {
        _q.Add("\n");
        return (int)SocketError.InvalidArgument;
      }
      var ret = DllImports.getsockname(socket, addr, addrLen);
      _q.Add($"[getsockname]={ret}\t{socket}\t{addr->ToIPEndPoint()}");
      return ret;
    }

    private SocketError CloseSocketH(IntPtr socket)
    {
      if (_sockets.TryRemove(socket, out var s))
      {
        DebugOutput()(s.Handle);
        s.Close();
        return SocketError.Success;
      }
      else
      {
        return SocketError.NotSocket;
      }
    }

    private int SendH(IntPtr socket, sbyte* buff, int len, int flags)
    {
      var str = new string(buff);
      _q.Add($"[send]{socket}\t{len}\t{flags}");
      _q.Add(str);
      return DllImports.send(socket, buff, len, flags);
    }
    private int SendToH(IntPtr socket, sbyte* buff, int len, int flags, sockaddr_in* to, int toLen)
    {
      try
      {
        var y = new byte[len];
        Marshal.Copy(new IntPtr(buff), y, 0, len);
        var s = _sockets[socket];
        var ret = s.SendTo(y, to->ToIPEndPoint());
        _q.Add($"[sndto]{socket}\t{s.ProtocolType}\t{s.LocalEndPoint}\t{to->ToIPEndPoint()}\t{BitConverter.ToString(y)}\n");
        return ret;
      }
      catch
      {
        return -1;
      }
      var str = new string(buff);
      _q.Add($"[sendto]{socket}\t{len}\t{flags}\t{to->ToIPEndPoint()}");
      _q.Add(str);
      return DllImports.sendto(socket, buff, len, flags, to, toLen);
    }

    private int RecvH(IntPtr socket, sbyte* buff, int len, int flags)
    {
      var ret = DllImports.recv(socket, buff, len, flags);
      var str = new string(buff);
      _q.Add($"[recv]={ret}\t{socket}\t{flags}");
      _q.Add(str);
      return ret;
    }

    private int RecvFromH(IntPtr socket, sbyte* buff, int len, int flags, sockaddr_in* from, int* fromLen)
    {
      var ret = DllImports.recvfrom(socket, buff, len, flags, from, fromLen);
      var str = new string(buff);
      _q.Add($"[recv]={ret}\t{socket}\t{len}\t{flags}\t{from->ToIPEndPoint()}");
      _q.Add(str);
      return ret;
    }

    private IntPtr SocketH(AddressFamily af, SocketType type, ProtocolType protocol)
    {
      try
      {
        if (type == SocketType.Dgram)
        {
          protocol = ProtocolType.Udp;
        }
        else if (type == SocketType.Stream)
        {
          protocol = ProtocolType.Tcp;
        }
        var s = new Socket(af, type, protocol);
        _sockets.TryAdd(s.Handle, s);
        DebugOutput()(s.Handle, s.ProtocolType);
        return s.Handle;
      }
      catch (SocketException)
      {
        return DllImports.INVALID_SOCKET;
      }
    }

    private int DirectPlayCreateH(Guid* pGuid, void** ppDp, IntPtr pUnk)
    {
      var ret = DllImports.DirectPlayCreate(pGuid, ppDp, pUnk) & 0x0000ffff;
      if (*pGuid == DllImports.DPSPGUID_TCPIP)
      {
        //var obj=Marshal.GetObjectForIUnknown(new IntPtr( ppDp.ToPointer()));
        _q.Add($"[DirectPlayCreate]={ret}\t{*pGuid}\t{**(byte**)ppDp }\t{pUnk}\n");
      }
      return ret;
    }

    private uint CoCreateInstanceH(Guid* clsid, IntPtr pUnkOuter, int clsContext, Guid* iid, int** ppv)
    {
      var ret = DllImports.CoCreateInstance(clsid, pUnkOuter, clsContext, iid, ppv);
      if (*iid == DllImports.IID_IDirectPlay4)
      {
        var p = (int*)new IntPtr(**ppv).ToPointer();
        _q.Add($"[CoCreateInstance]{*clsid}\t{pUnkOuter}\t{clsContext}\t{*iid}\t{new IntPtr(p).ToString("X")}\n");
        for (var i = 0; i < 10; i++)
        {
          _q.Add($" {new IntPtr(*p++).ToString("X")}\n");
        }
      }
      return ret;
    }

    private HostEnt* GetHostByNameH(sbyte* name)
    {
      DebugOutput()(new string(name));
      if (new string(name) == _fakeHostName)
      {
        fixed (byte* n = Encoding.Default.GetBytes(_trueHostName + "\0"))
        {
          var ret = DllImports.gethostbyname((sbyte*)n);
          **ret->AddrList = _ip;
          return ret;
        }
      }
      else
      {
        return null;
      }
    }

    private int GetHostNameH(sbyte* name, int nameLen)
    {
      var ret = DllImports.gethostname(name, nameLen);
      if (ret == 0)
      {
        _trueHostName = new string(name);
        var y = Encoding.Default.GetBytes(_fakeHostName + "\0");
        Marshal.Copy(y, 0, new IntPtr(name), y.Length);
        DebugOutput()(_fakeHostName);
      }
      return ret;
    }

    private SocketError ListenH(IntPtr socket, int backlog)
    {
      try
      {
        var s = _sockets[socket];
        s.Listen(backlog);
        DebugOutput()(socket, s.ProtocolType, s.LocalEndPoint, backlog);
        return SocketError.Success;
      }
      catch
      {
        return SocketError.SocketError;
      }
    }

    private bool CreateProcessAH(sbyte* applicationName, sbyte* commandLine, IntPtr processAttributes, IntPtr threadAttributes, bool inheritHandles, uint creationFlags, IntPtr environment, sbyte* currentDirectory, IntPtr startupInfo, ProcessInformation* processInformation)
    {
      DebugOutput()(new string(applicationName), new string(commandLine), processInformation->ProcessId);
      try
      {
        RemoteHooking.CreateAndInject(new string(applicationName), new string(commandLine), 0, _injectArgs.DllPath, _injectArgs.DllPath, out var pid, _injectArgs);
        return true;
      }
      catch (Exception ex)
      {
        DebugOutput()(ex);
        return false;
      }
    }
  }
}
