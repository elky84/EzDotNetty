using ZeroFormatter;

namespace Protocols.Common
{
    [ZeroFormattable]
    public struct Vector3
    {
        [Index(0)]
        public float x;

        [Index(1)]
        public float y;

        [Index(2)]
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return $"<x:{x}, y:{y}, z:{z}>";
        }
    }
}
