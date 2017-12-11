using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;
using Newtonsoft.Json;

namespace YTY.HookTest
{
  public class TransferProxy
  {
    private readonly TcpClient _tcpClient = new TcpClient();
    private readonly UdpClient _udpProxy = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
    private NetworkStream _stream;
    private BinaryReader _br;
    private BinaryWriter _bw;
    private ushort _udpProxyPort;
    private int _virtualIp;

    public ushort UdpProxyPort => _udpProxyPort;

    public int VirtualIp => _virtualIp;

    public TransferProxy()
    {
      _udpProxyPort =(ushort) (_udpProxy.Client.LocalEndPoint as IPEndPoint).Port;
    }

    public void Start()
    {
      _tcpClient.Connect("yty1.club", 11111);
      _stream = _tcpClient.GetStream();
      _br = new BinaryReader(_stream);
      _bw = new BinaryWriter(_stream);
      _virtualIp =_br.ReadInt32();
      UdpProxyLoop();
    }

    public void Close()
    {
      _bw.Close();
      _tcpClient.Close();
      _udpProxy.Close();
    }

    private async Task UdpProxyLoop()
    {
      while (true)
      {
        var packet = (await _udpProxy.ReceiveAsync()).Buffer;
        using (var ms = new MemoryStream(packet))
          using(var br=new BinaryReader(ms))
        {
          var command = br.ReadByte();
          switch(command)
          {
            case 1://broadcast
              _bw.Write(packet);
              break;
          }
        }
      }
    }
  }
}
