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
using System.Diagnostics;
using System.Drawing;

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
      //var hSend = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "send"), new SendD(SendH), this);
      //hSend.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hSendTo = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "sendto"), new SendToD(SendToH), this);
      //hSendTo.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hRecv = LocalHook.Create(LocalHook.GetProcAddress("ws2_32", "recv"), new RecvD(RecvH), this);
      //hRecv.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hPostQuitMessage = LocalHook.Create(LocalHook.GetProcAddress("user32", "PostQuitMessage"), new PostQuitMessageD(PostQuitMessageH), this);
      //hPostQuitMessage.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hTextOut = LocalHook.Create(LocalHook.GetProcAddress("gdi32", "TextOutA"), new TextOutD(TextOutH), this);
      hTextOut.ThreadACL.SetExclusiveACL(new[] { 0 });
      //var hGetACP = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "GetACP"), new GetACPD(GetACPH), this);
      //hGetACP.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hLoadString = LocalHook.Create(LocalHook.GetProcAddress("user32", "LoadStringA"), new LoadStringD(LoadStringH), this);
      hLoadString.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hLoadLibrary = LocalHook.Create(LocalHook.GetProcAddress("kernel32", "LoadLibraryA"), new LoadLibraryD(LoadLibraryH), this);
      hLoadLibrary.ThreadACL.SetExclusiveACL(new[] { 0 });
      var hDrawTextA = LocalHook.Create(LocalHook.GetProcAddress("user32", "DrawTextA"), new DrawTextD(DrawTextH), this);
      hDrawTextA.ThreadACL.SetExclusiveACL(new[] { 0 });
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

    private bool TextOutH(IntPtr dc, int xStart, int yStart, string str, int strLen)
    {
      //var bytes = new byte[strLen];
      //Marshal.Copy(pStr, bytes, 0, strLen);
      //_sw.WriteLine(BitConverter.ToString(bytes));
      //var str = Encoding.GetEncoding(936).GetString(bytes);
      var g = Graphics.FromHdc(dc);
      var color = GetTextColor(dc);
      var font = Font.FromHdc(dc);
      g.DrawString(str, font, new SolidBrush(color.ToColor()), xStart, yStart);
      return true;
      //return TextOutW(dc, xStart, yStart, str, str.Length);
    }

    private uint GetACPH()
    {
      _sw.WriteLine($"[GetACP]\t{GetACP()}");
      return 1252;
    }

    private int LoadStringH(IntPtr instance, uint id, IntPtr buffer, int bufferMax)
    {

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

      if (!_dlls.Contains(instance))
      {
        return LoadStringA(instance, id, buffer, bufferMax);
      }

      if (_kvs.ContainsKey(id.ToString()))
      {
        var str = _kvs[id.ToString()].Replace(@"\n","\n") + '\0';
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

    private readonly StringFormat SF_Bottom = new StringFormat { LineAlignment = StringAlignment.Far };
    private readonly StringFormat SF_Right = new StringFormat { Alignment = StringAlignment.Far };
    private int DrawTextH(IntPtr dc, string str, int count, ref RECT rect, DT_ format)
    {
      var g = Graphics.FromHdc(dc);
      var font = Font.FromHdc(dc);
      var rectF = rect.ToRectangleF();
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
      g.DrawString(str, font, new SolidBrush(GetTextColor(dc).ToColor()), rectF, sf);
      //_sw.WriteLine($"[DrawTextA]{color}\t{rectF}\t{str}");
      return (int)rectF.Height;
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

    [UnmanagedFunctionPointer(CallingConvention.Winapi,CharSet= CharSet.Ansi)]
    delegate bool TextOutD(IntPtr dc, int xStart, int yStart, string pStr, int strLen);

    [DllImport("gdi32", ExactSpelling = true)]
    static extern bool TextOutA(IntPtr dc, int xStart, int yStart, IntPtr str, int strLen);

    [DllImport("gdi32", ExactSpelling = true, CharSet = CharSet.Unicode)]
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

    [DllImport("gdi32")]
    static extern IntPtr GetCurrentObject(IntPtr dc, OBJ_ objectType);

    [DllImport("gdi32")]
    static extern int GetObject(IntPtr obj, int buffer, IntPtr objInfo);

    [DllImport("gdi32")]
    static extern COLORREF GetTextColor(IntPtr dc);

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
    delegate int DrawTextD(IntPtr dc, string str, int count, ref RECT rect, DT_ format);

    [DllImport("user32", ExactSpelling = true)]
    static extern int DrawTextA(IntPtr dc, string str, int count, ref RECT rect, DT_ format);

    [DllImport("gdi32", ExactSpelling = true, CharSet = CharSet.Ansi)]
    static extern bool GetTextExtentPoint32A(IntPtr dc, string str, int strLen, out Size size);
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct sockaddr_in
  {
    public short Family;
    public ushort Port;
    public uint Addr;
    public long Zero;
  }

  public enum OBJ_ : uint
  {
    OBJ_PEN = 1,
    OBJ_BRUSH,
    OBJ_DC,
    OBJ_METADC,
    OBJ_PAL,
    OBJ_FONT,
    OBJ_BITMAP,
    OBJ_REGION,
    OBJ_METAFILE,
    OBJ_MEMDC,
    OBJ_EXTPEN,
    OBJ_ENHMETADC,
    OBJ_ENHMETAFILE,
    OBJ_COLORSPACE,
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct LOGBRUSH
  {
    public uint Style;
    public COLORREF Color;
    public long Hatch;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct COLORREF
  {
    public byte Red;
    public byte Green;
    public byte Blue;
    public byte Zero;

    public Color ToColor()
    {
      return Color.FromArgb(Red, Green, Blue);
    }

    public Color ToColor(byte alpha)
    {
      return Color.FromArgb(alpha, Red, Green, Blue);
    }

    public override string ToString()
    {
      return ToColor().ToString();
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct RECT
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public Rectangle ToRectangle()
    {
      return new Rectangle(Left, Top, Right - Left, Bottom - Top);
    }

    public RectangleF ToRectangleF()
    {
      return RectangleF.FromLTRB(Left, Top, Right, Bottom);
    }
  }

  [Flags]
  public enum DT_ : uint
  {
    DT_TOP = 0x00000000,
    DT_LEFT = 0x00000000,
    DT_CENTER = 0x00000001,
    DT_RIGHT = 0x00000002,
    DT_VCENTER = 0x00000004,
    DT_BOTTOM = 0x00000008,
    DT_WORDBREAK = 0x00000010,
    DT_SINGLELINE = 0x00000020,
    DT_EXPANDTABS = 0x00000040,
    DT_TABSTOP = 0x00000080,
    DT_NOCLIP = 0x00000100,
    DT_EXTERNALLEADING = 0x00000200,
    DT_CALCRECT = 0x00000400,
    DT_NOPREFIX = 0x00000800,
    DT_INTERNAL = 0x00001000,
    DT_EDITCONTROL = 0x00002000,
    DT_PATH_ELLIPSIS = 0x00004000,
    DT_END_ELLIPSIS = 0x00008000,
    DT_MODIFYSTRING = 0x00010000,
    DT_RTLREADING = 0x00020000,
    DT_WORD_ELLIPSIS = 0x00040000,
    DT_NOFULLWIDTHCHARBREAK = 0x00080000,
    DT_HIDEPREFIX = 0x00100000,
    DT_PREFIXONLY = 0x00200000,
  }
}
