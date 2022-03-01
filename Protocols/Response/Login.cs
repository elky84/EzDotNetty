using System;
using ZeroFormatter;

namespace Protocols.Response
{
    [ZeroFormattable]
    [Serializable]
    public class Login : Header
    {
        [IgnoreFormat]
        public override Id.Response Id => Protocols.Id.Response.Login;

        [Index(1)]
        public virtual string Name { get; set; }
    }
}
