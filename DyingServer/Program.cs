using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Concurrent;
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
        string line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
          var message = JsonConvert.DeserializeObject<Message>(line);

          switch (message.Command)
          {
            case MessageCommand.Login:

              message.Ip = ip;
              await writer.WriteLineAsync(JsonConvert.SerializeObject(message));
              break;
          }
        }
      }
      IpManager.RecycleIp(ip);
    }
  }

  internal class Message
  {
    public MessageCommand Command { get; set; }
    public int Ip { get; set; }
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
