using System;
using System.Net.Sockets;

namespace YTY.HookTest
{
  public class SocketProxy
  {
    public int Socket { get; set; }
    public ProtocolType ProtocolType { get; set; }
    public uint LocalVip { get; set; }
    public ushort LocalPort { get; set; }
    public uint RemoteVip{ get; set; }
    public ushort RemotePort { get; set; }
    public bool IsBound { get; set; } = false;

    public static string UintIpToString(uint ip)
    {
      var bytes = BitConverter.GetBytes(ip);
      Array.Reverse(bytes);
      return string.Join(".", bytes);
    }

    public static ushort NetworkToHostOrder(ushort network)
    {
      var bytes = BitConverter.GetBytes(network);
      Array.Reverse(bytes);
      return BitConverter.ToUInt16(bytes,0);
    }

    public string LocalEndPointToString()
    {
      return $"{UintIpToString(LocalVip)}:{NetworkToHostOrder(LocalPort)}";
    }

    public string RemoteEndPointToString()
    {
      return $"{UintIpToString(RemoteVip)}:{NetworkToHostOrder(RemotePort)}";
    }
  }
}
