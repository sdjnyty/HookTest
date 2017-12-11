using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace YTY.HookTest
{
  public class DyingClient
  {
    public int Ip { get; set; }

    public TcpClient TcpClient { get; set; }

    public BinaryReader Reader { get; set; }

    public BinaryWriter Writer { get; set; }

    public DyingClient(int ip, TcpClient tcpClient)
    {
      Ip = ip;
      TcpClient = tcpClient;
      Reader = new BinaryReader(tcpClient.GetStream());
      Writer = new BinaryWriter(tcpClient.GetStream());
    }

    public async Task ReceiveAsync()
    {
      await Task.Run(() =>
      {
        byte cmd;
        while ((cmd = Reader.ReadByte()) != 0)
        {
          switch (cmd)
          {
            case 1: //broadcast
              var length = Reader.ReadInt32();
              var data = Reader.ReadBytes(length);
              BroadCastAsync(BitConverter.GetBytes(length).Concat(data).ToArray());
              break;
          }
        }
      });
      Console.WriteLine($"{Program.IntToIp(Ip)} disconnecting");
      Program.Instance.Clients.TryRemove(Ip, out var _);
      IpManager.RecycleIp(Ip);
      Console.WriteLine($"{Program.IntToIp(Ip)} recycled");
      TcpClient.Close();
    }

    public async Task SendIpAsync()
    {
      await Task.Run(() =>
      {
        Writer.Write(Ip);
        Console.WriteLine("IP sent");
      });
    }

    public async Task BroadCastAsync(byte[] packet)
    {
      var tasks = Program.Instance.Clients.Values
        .Where(c => c.TcpClient.Connected)
        .Select(c => Task.Run(() => c.Writer.Write(packet)));
      await Task.WhenAll(tasks);
    }
  }
}
