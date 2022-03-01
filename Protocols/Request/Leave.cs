using System;
using ZeroFormatter;

namespace Protocols.Request
{
    [ZeroFormattable]
    [Serializable]
    public class Leave : Header
    {
        [IgnoreFormat]
        public override Id.Request Id => Protocols.Id.Request.Leave;
    }
}
