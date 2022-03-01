using System;
using ZeroFormatter;

namespace Protocols.Response
{
    [ZeroFormattable]
    [Serializable]
    public class Leave : Header
    {
        [IgnoreFormat]
        public override Id.Response Id => Protocols.Id.Response.Leave;

        [Index(1)]
        public virtual int PlayerIndex { get; set; }
    }
}
