using EzDotnetty.Types;

namespace EzDotNetty.Logging
{
    public partial class LoggerId : Enumeration
    {
        protected LoggerId(int id, string name)
            : base(id, name)
        {
        }

        public static readonly LoggerId Buff = new(1, nameof(Buff));
        public static readonly LoggerId Message = new(2, nameof(Message));
    }
}
