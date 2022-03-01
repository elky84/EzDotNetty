using System;
using ZeroFormatter;

namespace Protocols.Response
{
    [ZeroFormattable]
    [Serializable]
    public class Error : Header
    {
        [IgnoreFormat]
        public override Id.Response Id => Protocols.Id.Response.Error;

        [Index(1)]
        public virtual string Message { get; set; }
    }
}
