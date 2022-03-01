using System;
using ZeroFormatter;

namespace Protocols.Request
{
    [ZeroFormattable]
    [Serializable]
    public class Enter : Header
    {
        [IgnoreFormat]
        public override Id.Request Id => Protocols.Id.Request.Enter;

        [Index(0)]
        public virtual int RoomId { get; set; }
    }
}
