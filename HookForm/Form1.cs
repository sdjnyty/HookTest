using System;
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

namespace HookForm
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
      var exePath =(string) Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Microsoft Games\Age of Empires II: The Conquerors Expansion\1.0").GetValue("EXE Path");
      var injectorPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "injector.dll");
      RemoteHooking.CreateAndInject(Path.Combine(exePath,@"age2_x1\age2_x1.exe"), null, 0, injectorPath, injectorPath, out var pid);
      using (var pipe = new NamedPipeServerStream("HookPipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
      {
        await Task.Factory.FromAsync(pipe.BeginWaitForConnection, pipe.EndWaitForConnection, null);
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
  }
}
