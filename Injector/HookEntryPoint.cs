using System;
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

namespace YTY.HookTest
{
  public class HookEntryPoint : IEntryPoint
  {
    private StreamWriter _sw;
    private IniParser.Model.KeyDataCollection _kvs;
    private readonly HashSet<IntPtr> _dlls = new HashSet<IntPtr>();
    private readonly BlockingCollection<string> _q = new BlockingCollection<string>();

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
      //var hSend = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "send"), new SendD(SendH), this);
      //hSend.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hSendTo = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "sendto"), new SendToD(SendToH), this);
      //hSendTo.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hRecv = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "recv"), new RecvD(RecvH), this);
      //hRecv.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hPostQuitMessage = LocalHook.Create(LocalHook.GetProcAddress("user32", "PostQuitMessage"), new PostQuitMessageD(PostQuitMessageH), this);
      //hPostQuitMessage.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hTextOut = LocalHook.Create(LocalHook.GetProcAddress("gdi32", "TextOutA"), new TextOutAD(TextOutH), this);
      //hTextOut.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hGetACP = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "GetACP"), new GetACPD(GetACPH), this);
      //hGetACP.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hLoadString = LocalHook.Create(LocalHook.GetProcAddress("user32", "LoadStringA"), new LoadStringD(LoadStringH), this);
      //hLoadString.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hLoadLibrary = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "LoadLibraryA"), new LoadLibraryAD(LoadLibraryH), this);
      hLoadLibrary.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hDrawTextA = LocalHook.Create(LocalHook.GetProcAddress("user32", "DrawTextA"), new DrawTextAD(DrawTextH), this);
      //hDrawTextA.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hAccept = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "accept"), new AcceptD(acceptH), this);
      //hAccept.ThreadACL.SetExclusiveACL(new[] { 0 });
      Task.Run(() =>
      {
        while (true)
        {
          _q.TryTake(out var s, -1);
          _sw.WriteLine(s);
        }
      });
      RemoteHooking.WakeUpProcess();
      Thread.Sleep(-1);
    }

    private int SendH(IntPtr socket, byte[] buff, int len, int flags)
    {
      //_q.Add($"[send]\t{socket}\t{len}\t{flags}");
      //_q.Add(buff);
      //_q.Add("================");
      return DllImports.send(socket, buff, len, flags);
    }
    private int SendToH(IntPtr socket, byte[] buff, int len, int flags, ref sockaddr_in to, int toLen)
    {
      //var ip = new IPEndPoint(new IPAddress(to.Addr),(int)(((uint) IPAddress.NetworkToHostOrder( to.Port))>>16));
      //_q.Add($"[sendto]\t{socket}\t{len}\t{flags}\t{ip}");
      //_q.Add(buff);
      //_q.Add("================");
      return DllImports.sendto(socket, buff, len, flags, ref to, toLen);
    }

    private int RecvH(IntPtr socket, byte[] buff, int len, int flags)
    {
      var ret = DllImports.recv(socket, buff, len, flags);
      //_q.Add($"[recv]\t{socket}\t{len}\t{flags}\tret={ret}");
      //_q.Add(Marshal.PtrToStringAnsi(buff, ret));
      //_q.Add("================");
      return ret;
    }

    private void PostQuitMessageH(int exitCode)
    {
      _q.Add("[PostQuitMessage]");
      DllImports.PostQuitMessage(exitCode);
    }

    private bool TextOutH(IntPtr dc, int xStart, int yStart, string str, int strLen)
    {
      //var bytes = new byte[strLen];
      //Marshal.Copy(pStr, bytes, 0, strLen);
      //_q.Add(BitConverter.ToString(bytes));
      //var str = Encoding.GetEncoding(936).GetString(bytes);
      var g = Graphics.FromHdc(dc);
      var align = DllImports.GetTextAlign(dc);
      var color = DllImports.GetTextColor(dc).ToColor();
      var font = Font.FromHdc(dc);
      var point = new PointF(xStart, yStart);
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
      _q.Add($"[TextOutA]{font}\t{color}\t{point}\t{align}\t{str}");
      return true;
      //return TextOutW(dc, xStart, yStart, str, str.Length);
    }

    private uint GetACPH()
    {
      _q.Add($"[GetACP]\t{DllImports.GetACP()}");
      return 1252;
    }

    private int LoadStringH(IntPtr instance, uint id, IntPtr buffer, int bufferMax)
    {
      var sb = new StringBuilder(bufferMax-1);
      var ret = DllImports.LoadStringA(instance, id, sb, bufferMax);
      var bytes = Encoding.Default.GetBytes(sb.ToString()+'\0');
      Marshal.Copy(bytes, 0, buffer, bytes.Length);
      return ret;

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

      //if (!_dlls.Contains(instance))
      //{
      //  return DllImports.LoadStringA(instance, id, buffer, bufferMax);
      //}

      //if (_kvs.ContainsKey(id.ToString()))
      //{
      //  var str = _kvs[id.ToString()].Replace(@"\n", "\n") + '\0';
      //  //_q.Add(str);
      //  var bytes = Encoding.GetEncoding(936).GetBytes(str);

      //  if (bufferMax > bytes.Length)
      //  {
      //    Marshal.Copy(bytes, 0, buffer, bytes.Length);
      //    //_q.Add($"[LoadStringA]\t{id}\t{Marshal.PtrToStringUni(buffer)}\t{bufferMax}");
      //    return bytes.Length;
      //  }
      //  else
      //  {
      //    return 0;
      //  }
      //}
      //else
      //{
      //  return 0;
      //}

      #endregion
    }

    private IntPtr LoadLibraryH(string fileName)
    {
      //_q.Add($"[LoadLibraryA]\t{fileName}");
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

    private int DrawTextH(IntPtr dc, string str, int count, ref RECT rect, DT_ format)
    {
      var g = Graphics.FromHdc(dc);
      var font = Font.FromHdc(dc);
      var rectF = rect.ToRectangleF();
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
      //if ((int)rectF.X == 1 && (int)rectF.Y == 1)
      //{
      //  color = GetTextColor(dc).ToColor();
      //}
      //else
      //{
      //  color = GetTextColor(dc).ToColor(0x7f);
      //}
      g.DrawString(str, font, new SolidBrush(color), rectF, sf);
      _q.Add($"[DrawTextA]{format}\t{font}\t{color}\t{rectF}\t{str}");
      return (int)rectF.Height;
    }

    private IntPtr acceptH(IntPtr socket, out sockaddr_in addr, out int addrLen)
    {
      return DllImports.accept(socket, out addr, out addrLen);
    }
  }
}
