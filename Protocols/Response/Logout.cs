using System;
using ZeroFormatter;

namespace Protocols.Response
{
    [ZeroFormattable]
    [Serializable]
    public class Logout : Header
    {
        [IgnoreFormat]
        public override Id.Response Id => Protocols.Id.Response.Logout;
    }
}
