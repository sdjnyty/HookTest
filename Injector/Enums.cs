﻿using System;

namespace YTY.HookTest
{

  [Flags]
  public enum DT_ : uint
  {
    DT_TOP = 0x00000000,
    DT_LEFT = 0x00000000,
    DT_CENTER = 0x00000001,
    DT_RIGHT = 0x00000002,
    DT_VCENTER = 0x00000004,
    DT_BOTTOM = 0x00000008,
    DT_WORDBREAK = 0x00000010,
    DT_SINGLELINE = 0x00000020,
    DT_EXPANDTABS = 0x00000040,
    DT_TABSTOP = 0x00000080,
    DT_NOCLIP = 0x00000100,
    DT_EXTERNALLEADING = 0x00000200,
    DT_CALCRECT = 0x00000400,
    DT_NOPREFIX = 0x00000800,
    DT_INTERNAL = 0x00001000,
    DT_EDITCONTROL = 0x00002000,
    DT_PATH_ELLIPSIS = 0x00004000,
    DT_END_ELLIPSIS = 0x00008000,
    DT_MODIFYSTRING = 0x00010000,
    DT_RTLREADING = 0x00020000,
    DT_WORD_ELLIPSIS = 0x00040000,
    DT_NOFULLWIDTHCHARBREAK = 0x00080000,
    DT_HIDEPREFIX = 0x00100000,
    DT_PREFIXONLY = 0x00200000,
  }

  [Flags]
  public enum TA_ : uint
  {
    TA_NOUPDATECP = 0,
    TA_UPDATECP = 1,
    TA_LEFT = 0,
    TA_RIGHT = 2,
    TA_CENTER = 6,
    TA_TOP = 0,
    TA_BOTTOM = 8,
    TA_BASELINE = 24,
    TA_RTLREADING = 256,
  }

  public enum OBJ_ : uint
  {
    OBJ_PEN = 1,
    OBJ_BRUSH,
    OBJ_DC,
    OBJ_METADC,
    OBJ_PAL,
    OBJ_FONT,
    OBJ_BITMAP,
    OBJ_REGION,
    OBJ_METAFILE,
    OBJ_MEMDC,
    OBJ_EXTPEN,
    OBJ_ENHMETADC,
    OBJ_ENHMETAFILE,
    OBJ_COLORSPACE,
  }

  public enum AF_
  {
    AF_UNSPEC = 0,
    AF_INET = 2,
    AF_IPX = 6,
    AF_APPLETALK = 16,
    AF_NETBIOS = 17,
    AF_INET6 = 23,
    AF_IRDA = 26,
    AF_BTH = 32,
  }

  public enum SOCK_
  {
    SOCK_STREAM = 1,
    SOCK_DGRAM = 2,
    SOCK_RAW = 3,
    SOCK_RDM = 4,
    SOCK_SEQPACKET = 5,
  }

  public enum IPPROTO_
  {
    IPPROTO_ICMP = 1,
    IPPROTO_IGMP = 2,
    BTHPROTO_RFCOMM = 3,
    IPPROTO_TCP = 6,
    IPPROTO_UDP = 17,
    IPPROTO_ICMPV6 = 58,
    IPPROTO_RM = 113,
  }
}
