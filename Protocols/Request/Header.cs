using System;
using ZeroFormatter;

namespace Protocols.Request
{
    public class Header
    {
        [IgnoreFormat]
        public virtual Id.Request Id => Protocols.Id.Request.Undefined;
    }
}
