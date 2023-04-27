using System.Net;

namespace EzDotNetty.Config.Client
{

    public static class Settings
    {
        public static bool IsSsl
        {
            get
            {
                var ssl = Helper.Configuration["ssl"];
                return !string.IsNullOrEmpty(ssl) && bool.Parse(ssl);
            }
        }

        public static IPAddress Host => IPAddress.Parse(Helper.Configuration["host"]);

        public static int Port => int.Parse(Helper.Configuration["port"]);

        public static int Size => int.Parse(Helper.Configuration["size"]);

        public static bool UseLibuv
        {
            get
            {
                var libuv = Helper.Configuration["libuv"];
                return !string.IsNullOrEmpty(libuv) && bool.Parse(libuv);
            }
        }
    }
}