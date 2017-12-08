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

    public StreamReader Reader { get; set; }

    public StreamWriter Writer { get; set; }

    public DyingClient(int ip, TcpClient tcpClient)
    {
      Ip = ip;
      TcpClient = tcpClient;
      Reader = new StreamReader(tcpClient.GetStream());
      Writer = new StreamWriter(tcpClient.GetStream());
    }

    public async Task ReceiveAsync()
    {
      string line;
      while ((line = await Reader.ReadLineAsync()) != null)
      {
        
      }
      Program.Instance.Clients.TryRemove(Ip, out var _);
      TcpClient.Close();
    }

    public async Task SendIpAsync()
    {
      await Writer.WriteLineAsync(Ip.ToString());
    }

    public async Task BroadCastAsync()
    {
      var tasks = Program.Instance.Clients.Values.Where(c => c.TcpClient.Connected).Select(c => c.Writer.WriteLineAsync());
      await Task.WhenAll(tasks);
    }
  }
}
