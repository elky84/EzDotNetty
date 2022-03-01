﻿using DotNetty.Buffers;
using ZeroFormatter;

namespace TestClientShared.Util
{
    public static class ByteBufferUtil
    {
        public static IByteBuffer ToByteBuffer<T>(this T t) where T : Protocols.Request.Header
        {
            var msg = Unpooled.Buffer();
            msg.WriteInt((int)t.Id);
            msg.WriteBytes(ZeroFormatterSerializer.Serialize(t));
            return msg;
        }
    }
}
