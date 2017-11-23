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
      var exe = @"c:\program files (x86)\age of empires ii\age2_x1\age2_x1.exe";
      //var exe = @"d:\hawkaoc\aoc\age2_x1\age2_x1.exe";
      var path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "injector.dll");
      RemoteHooking.CreateAndInject(exe, null, 0, path, path, out var pid);
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
