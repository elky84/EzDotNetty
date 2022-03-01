using ZeroFormatter;

namespace Protocols.Common
{
    [ZeroFormattable]
    public struct Vector2
    {
        [Index(0)]
        public float x;
        [Index(1)]
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"<x:{x}, y:{y}>";
        }
    }
}
