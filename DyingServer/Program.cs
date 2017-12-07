using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Concurrent;
using System.Dynamic;
using Newtonsoft.Json;

namespace YTY.HookTest
{
  internal class Program
  {
    private const int PORT = 11111;

    private readonly TcpListener tcpListener = new TcpListener(IPAddress.Any, PORT);

    private static void Main(string[] args)
    {
      var p = new Program();
      p.Run();
      Console.ReadLine();
      p.tcpListener.Stop();
    }

    private async Task Run()
    {
      tcpListener.Start();
      while (true)
      {
        var client = await tcpListener.AcceptTcpClientAsync();
        AcceptClient(client);
      }
    }

    private async Task AcceptClient(TcpClient client)
    {
      var ip = IpManager.AllocateIp();
      using (var reader = new StreamReader(client.GetStream(), Encoding.UTF8))
      using (var writer = new StreamWriter(client.GetStream(), Encoding.UTF8))
      {
        writer.WriteLine(ip);
        string line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
          dynamic req = JsonConvert.DeserializeObject(line);
          dynamic resp = new ExpandoObject();
          switch (req.Cmd)
          {
            case MessageCommand.Login:

              resp.Ip = ip;
              break;
          }
          await writer.WriteLineAsync(JsonConvert.SerializeObject(resp));
        }
      }
      IpManager.RecycleIp(ip);
    }
  }

  internal static class IpManager
  {
    private const int STARTIP = 0xa000001;
    private static readonly HashSet<int> _set = new HashSet<int>();
    private static readonly object _locker = new object();

    public static int AllocateIp()
    {
      lock (_locker)
      {
        for (var i = STARTIP; ; i++)
        {
          if (!_set.Contains(i))
          {
            _set.Add(i);
            return i;
          }
        }
      }
    }

    public static void RecycleIp(int ip)
    {
      lock (_locker)
      {
        _set.Remove(ip);
      }
    }
  }

  internal enum MessageCommand
  {
    Unknown,
    Login,
  }
}
