﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyHook;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Net;

namespace YTY.HookTest
{
  public partial class Form1 : Form
  {
    private readonly TransferProxy _proxy = new TransferProxy();

    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
     // _proxy.Start();
      var exePath = (string)Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").GetValue("EXE Path");
      var injectArgs = new InjectArgs
      {
        DllPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "age2x1injector.dll"),
        UdpProxyPort = _proxy.UdpProxyPort,
        TcpProxyPort=_proxy.TcpProxyPort,
        VirtualIp=_proxy.VirtualIp,
      };
      Console.WriteLine(injectArgs.UdpProxyPort);
      RemoteHooking.CreateAndInject(Path.Combine(exePath, @"age2_x1\age2_x1.exe"), null, 0, injectArgs.DllPath, injectArgs.DllPath, out var pid, injectArgs);
      PipeLoop();

    }


    private async Task PipeLoop()
    {
      using (var pipe = new NamedPipeServerStream("HookPipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
      {
        await Task.Factory.FromAsync(pipe.BeginWaitForConnection, pipe.EndWaitForConnection, null);
        PipeLoop();
        using (var sr = new StreamReader(pipe))
        {
          string line;
          while ((line = await sr.ReadLineAsync()) != null)
          {
            richTextBox1.AppendText(line + '\n');
          }
        }
      }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      _proxy.Close();
    }
  }
}
