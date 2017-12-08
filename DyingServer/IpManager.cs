using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTY.HookTest
{
  internal static class IpManager
  {
    private const int STARTIP = 0xa000001;
    private static readonly HashSet<int> _set = new HashSet<int>();
    private static readonly object _locker = new object();

    public static int AllocateIp()
    {
      lock (_locker)
      {
        for (var i = STARTIP; ; i++)
        {
          if (!_set.Contains(i))
          {
            _set.Add(i);
            return i;
          }
        }
      }
    }

    public static void RecycleIp(int ip)
    {
      lock (_locker)
      {
        _set.Remove(ip);
      }
    }
  }
}
