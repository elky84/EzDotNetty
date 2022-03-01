using System;
using ZeroFormatter;

namespace Protocols.Request
{
    [ZeroFormattable]
    [Serializable]
    public class Login : Header
    {
        [IgnoreFormat]
        public override Id.Request Id => Protocols.Id.Request.Login;

        [Index(0)]
        public virtual string Name { get; set; }
    }
}
