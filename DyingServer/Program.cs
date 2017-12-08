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

    public static Program Instance { get; } = new Program();

    public ConcurrentDictionary<int, DyingClient> Clients { get; } = new ConcurrentDictionary<int, DyingClient>();

    private static void Main(string[] args)
    {
      Instance.Run();
      Console.ReadLine();
      Instance.tcpListener.Stop();
    }

    private async Task Run()
    {
      tcpListener.Start();
      while (true)
      {
        var tcpClient = await tcpListener.AcceptTcpClientAsync();
        var ip = IpManager.AllocateIp();
        var client = new DyingClient(ip, tcpClient);
        client.SendIpAsync();
        client.ReceiveAsync();
      }
    }
  }

  internal enum MessageCommand
  {
    Unknown,
    Login,
  }
}
