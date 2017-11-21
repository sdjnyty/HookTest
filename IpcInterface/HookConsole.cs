using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyHook;
using System.IO;
using System.IO.Pipes;

namespace YTY.HookTest
{
  public class HookConsole
  {
    public static void Main(string[] args)
    {
      var path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "injector.dll");
      RemoteHooking.CreateAndInject(@"c:\program files (x86)\age of empires ii\age2_x1\age2_x1.exe", null, 0, path, path, out var pid);
      using (var pipe = new NamedPipeServerStream("HookPipe", PipeDirection.InOut))
      {
        pipe.WaitForConnection();
        using (var sr = new StreamReader(pipe))
        {
          while (!sr.EndOfStream)
          {
            Console.WriteLine(sr.ReadLine());
          }
        }
      }
        Console.ReadLine();
    }
  }
}
