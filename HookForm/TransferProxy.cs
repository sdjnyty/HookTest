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
    private readonly TcpListener _tcpListener = new TcpListener(IPAddress.Loopback,0);
    private readonly TcpClient _tcpClient = new TcpClient();
    private readonly UdpClient _udpProxy = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
    private NetworkStream _stream;
    private BinaryReader _br;
    private BinaryWriter _bw;
    private ushort _udpProxyPort;
    private ushort _tcpProxyPort;
    private uint _virtualIp;

    public ushort UdpProxyPort => _udpProxyPort;

    public ushort TcpProxyPort => _tcpProxyPort;

    public uint VirtualIp => _virtualIp;

    public TransferProxy()
    {
      _udpProxyPort = (ushort)(_udpProxy.Client.LocalEndPoint as IPEndPoint).Port;
      _tcpProxyPort = (ushort)(_tcpListener.LocalEndpoint as IPEndPoint).Port;
    }

    public void Start()
    {
      _tcpClient.Connect("yty1.club", 11111);
      _stream = _tcpClient.GetStream();
      _br = new BinaryReader(_stream);
      _bw = new BinaryWriter(_stream);
      _virtualIp = _br.ReadUInt32();
      _tcpListener.Start();
      UdpProxyLoop();
      StreamLoop();
      TcpProxyLoop();
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
        using (var br = new BinaryReader(ms))
        {
          var command = br.ReadByte();
          switch (command)
          {
            case 1://broadcast
              _bw.Write(packet);
              break;
          }
        }
      }
    }

    private async Task StreamLoop()
    {
      while (true)
      {
        var command = _br.ReadByte();
        var fromVip = _br.ReadUInt32();
        var fromPort = _br.ReadUInt16();
        var toVip = _br.ReadUInt32();
        var toPort = _br.ReadUInt16();
        var length = _br.ReadInt32();
        var data = _br.ReadBytes(length);
        var packet = new byte[length + 17];
        using (var ms = new MemoryStream(packet))
        using (var bw = new BinaryWriter(ms))
        {
          bw.Write(command);
          bw.Write(fromVip);
          bw.Write(fromPort);
          bw.Write(toVip);
          bw.Write(toPort);
          bw.Write(length);
          bw.Write(data);
        }
        switch (command)
        {
          case 1://udp sendto
            _udpProxy.SendAsync(packet, packet.Length, new IPEndPoint(IPAddress.Loopback, toPort));
            break;
        }
      }
    }

    private async Task TcpProxyLoop()
    {
      while(true)
      {
        await _tcpListener.AcceptTcpClientAsync();

      }
    }
  }
}
