using Protocols.Common;
using System;
using ZeroFormatter;

namespace Protocols.Request
{
    [ZeroFormattable]
    [Serializable]
    public class Move : Header
    {
        [IgnoreFormat]
        public override Id.Request Id => Protocols.Id.Request.Move;

        [Index(0)]
        public virtual Vector3 Position { get; set; }
    }
}
