using Protocols.Common;
using System;
using ZeroFormatter;

namespace Protocols.Response
{
    [ZeroFormattable]
    [Serializable]
    public class Move : Header
    {
        [IgnoreFormat]
        public override Id.Response Id => Protocols.Id.Response.Move;

        [Index(1)]
        public virtual int PlayerIndex { get; set; }

        [Index(2)]
        public virtual Vector3 Position { get; set; }
    }
}
