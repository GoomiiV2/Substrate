using System.Numerics;
using Veldrid;

namespace Substrate.Scene3D
{
    public class MeshSection
    {
        public string Name;
        public uint IndiceStart;
        public uint IndicesLength;
        public Texture DiffuseTex;

        public Vector4 Color { get => GPUData.Color; set { GPUData.Color = value; Update(); } }
        public DisplayModes DisplayMode { get => GPUData.DisplayMode; set { GPUData.DisplayMode = value; Update(); } }
        public bool Outline { get => GPUData.Outline; set { GPUData.Outline = value; Update(); } }

        public ResourceSet TexResourceSet;
        public DeviceBuffer BufferData;
        public MeshSectionBufferData GPUData;

        public unsafe MeshSection()
        {
            BufferData = Substrate.App.GD.ResourceFactory.CreateBuffer(new BufferDescription((uint)sizeof(MeshSectionBufferData), BufferUsage.UniformBuffer));
            Update();
        }

        public void Update()
        {
            Substrate.App.GD.UpdateBuffer(BufferData, 0, ref GPUData);
        }

        public void RemakeResourceSet()
        {
            TexResourceSet.Dispose();
            SimpleModel.CreateTexResourceSet(this);
        }
    }

    public struct MeshSectionBufferData
    {
        public const int SIZE = (4 * 4) + 1 + (15);

        public Vector4 Color;
        public DisplayModes DisplayMode;
        public bool Outline;

        public float _padding1;
        public float _padding2;
    }

    public enum DisplayModes : int
    {
        NoRender = -1,
        Normal = 0,
        SoildColor = 1,
        Unlit = 2
    }
}
