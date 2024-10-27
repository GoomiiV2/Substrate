using System.Numerics;

namespace Substrate.Scene3D
{
    public struct PerItemData
    {
        public const int SIZE = 64 + 4 + (4 * 3);

        public Matrix4x4 Mat;
        public SelectableID SelectionId;
        public uint Flags;
        public float _padding2;
        public float _padding3;
    }
}
