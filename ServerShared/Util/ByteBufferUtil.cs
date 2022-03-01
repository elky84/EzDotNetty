﻿using DotNetty.Buffers;
using ZeroFormatter;

namespace ServerShared.Util
{
    public static class ByteBufferUtil
    {
        public static IByteBuffer ToByteBuffer<T>(this T t) where T : Protocols.Response.Header
        {
            var msg = Unpooled.Buffer();
            msg.WriteInt((int)t.Id);
            msg.WriteBytes(ZeroFormatterSerializer.Serialize(t));
            return msg;
        }
    }
}
