using EzDotNetty.Types;

namespace EzDotNetty.Logging
{
    public partial class LoggerId : Enumeration
    {
        protected LoggerId(int id, string name)
            : base(id, name)
        {
        }

        public static readonly LoggerId Message = new(1, nameof(Message));
    }
}
