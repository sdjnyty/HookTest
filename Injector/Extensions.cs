using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace YTY.HookTest
{
  public static unsafe class Extensions
  {
    internal static byte[] ToBytes(this EndPoint ep)
    {
      var a = ep.Serialize();
      var ret = new byte[a.Size];
      for (var i = 0; i < a.Size; i++)
      {
        ret[i] = a[i];
      }
      return ret;
    }
  }
}
