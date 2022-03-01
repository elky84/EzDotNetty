using Protocols.Common;
using System;
using ZeroFormatter;

namespace Protocols.Response
{
    [ZeroFormattable]
    [Serializable]
    public class Enter : Header
    {
        [IgnoreFormat]
        public override Id.Response Id => Protocols.Id.Response.Enter;

        [Index(1)]
        public virtual int PlayerIndex { get; set; }

        [Index(2)]
        public virtual Vector3 Position { get; set; }

        [Index(3)]
        public virtual string Name { get; set; }
    }
}
