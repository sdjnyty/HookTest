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
    private readonly UdpClient _udpProxy = new UdpClient(0);
    private StreamReader _sr;
    private StreamWriter _sw;
    private int _udpProxyPort;

    public TransferProxy()
    {
      _udpProxyPort = (_udpProxy.Client.LocalEndPoint as IPEndPoint).Port;
    }

    public void Start()
    {
      _tcpClient.Connect("yty1.club", 11111);
      _sr = new StreamReader(_tcpClient.GetStream(), Encoding.UTF8);
      _sw = new StreamWriter(_tcpClient.GetStream(), Encoding.UTF8);
      _sr.ReadLine();
    }

    public int Login()
    {
      dynamic obj = new ExpandoObject();
      obj.Cmd = 1;
      _sw.WriteLine(JsonConvert.SerializeObject(obj));
      dynamic dyn=JsonConvert.DeserializeObject(_sr.ReadLine());
      return dyn.Ip;
    }
  }
}
