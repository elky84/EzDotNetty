using Protocols.Code;
using ZeroFormatter;

namespace Protocols.Response
{
    public class Header
    {
        [IgnoreFormat]
        public virtual Id.Response Id => Protocols.Id.Response.Undefined;

        [Index(0)]
        public virtual Result Result { get; set; } = Result.Success;
    }
}
