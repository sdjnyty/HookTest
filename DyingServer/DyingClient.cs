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
    public uint Ip { get; set; }

    public TcpClient TcpClient { get; set; }

    public BinaryReader Reader { get; set; }

    public BinaryWriter Writer { get; set; }

    public DyingClient(uint ip, TcpClient tcpClient)
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
          var fromVip = Reader.ReadUInt32();
          var fromPort = Reader.ReadUInt16();
          var toVip = Reader.ReadUInt32();
          var toPort = Reader.ReadUInt16();
          var length = Reader.ReadInt32();
          var data = Reader.ReadBytes(length);
          switch (cmd)
          {
            case 1: //udp sendto
              if (toVip == 0xffffffff) // broadcast
              {
                foreach (var client in Program.Instance.Clients.Values.ToList())
                {
                  Write(client);
                }
              }
              else
              {
                if (Program.Instance.Clients.TryGetValue(toVip, out var client)
                && client.TcpClient.Connected)
                {
                  Write(client);
                }
              }
              break;
          }

          void Write(DyingClient client)
          {
            if (client.TcpClient.Connected)
            {
              client.Writer.Write(cmd);
              client.Writer.Write(fromVip);
              client.Writer.Write(fromPort);
              client.Writer.Write(toVip);
              client.Writer.Write(toPort);
              client.Writer.Write(length);
              client.Writer.Write(data);
            }
          }
        }
      });
      Console.WriteLine($"{Program.UintToIp(Ip)} disconnecting");
      Program.Instance.Clients.TryRemove(Ip, out var _);
      IpManager.RecycleIp(Ip);
      Console.WriteLine($"{Program.UintToIp(Ip)} recycled");
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
