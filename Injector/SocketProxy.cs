using System;
using System.Net;
using System.Net.Sockets;

namespace YTY.HookTest
{
  public class SocketProxy
  {
    public int Socket { get; set; }
    public ProtocolType ProtocolType { get; set; }
    public IPEndPoint LocalEndPoint { get; set; }
    public IPEndPoint RemoteEndPoint { get; set; }
  }
}
