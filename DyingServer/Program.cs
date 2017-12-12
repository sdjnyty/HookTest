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

    public ConcurrentDictionary<uint, DyingClient> Clients { get; } = new ConcurrentDictionary<uint, DyingClient>();

    private static void Main(string[] args)
    {
      Instance.Run();
      Console.ReadLine();
      Instance.tcpListener.Stop();
      Console.WriteLine("server stopped");
    }

    private async Task Run()
    {
      tcpListener.Start();
      Console.WriteLine("server started");
      while (true)
      {
        var tcpClient = await tcpListener.AcceptTcpClientAsync();
        Console.WriteLine($"TcpClient accepted:");
        var ip = IpManager.AllocateIp();
        Console.WriteLine($"IP allocated:{UintToIp(ip)}");
        var client = new DyingClient(ip, tcpClient);
        Clients.TryAdd(ip, client);
        client.SendIpAsync();
        client.ReceiveAsync();
      }
    }

    public static IPAddress UintToIp(uint ip)
    {
      return new IPAddress(BitConverter.GetBytes( ip));
    }
  }

  internal enum MessageCommand
  {
    Unknown,
    Login,
  }
}
