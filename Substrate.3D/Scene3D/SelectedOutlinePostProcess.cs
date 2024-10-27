using System.Numerics;
using Veldrid;
using Veldrid.SPIRV;

namespace Substrate.Scene3D
{
    public class SelectedOutlinePostProcess
    {
        public DeviceBuffer SettingsBuffer;
        public ResourceSet ItemResourceSet;
        public OutlineData OutlineSettings = new OutlineData();
        private DeviceBuffer VertBuffer;
        private DeviceBuffer IndexBuffer;
        private Pipeline Pipeline;
        private ShaderSetDescription ShaderSet;
        private ResourceLayout PerItemResourceLayout;

        public void Init()
        {
            CreateResources();
        }

        public void SetSettings(OutlineData settings)
        {
            OutlineSettings = settings;
            Substrate.App.GD.UpdateBuffer(SettingsBuffer, 0, ref OutlineSettings);
        }

        private void CreateResources()
        {
            var halfSize = 1f;
            var verts = new VertexDefinition[]
            {
                new (1f, 1f, 0, 1f, 1f),
                new (-1f, -1f, 0f, 0f, 0f),
                new (-1, 1f, 0, 0f, 1f),
                new (-1, -1, 0, 0f, 0f),
                new (1, 1, 0, 1f, 1f),
                new (1, -1, 0, 1f, 0f)
            };

            var indices = new ushort[]
            {
                0, 1, 2, 3, 4, 5
            };

            VertBuffer = Substrate.App.GD.ResourceFactory.CreateBuffer(new BufferDescription((uint)(VertexDefinition.SizeInBytes * verts.Length), BufferUsage.VertexBuffer));
            Substrate.App.GD.UpdateBuffer(VertBuffer, 0, verts);

            IndexBuffer = Substrate.App.GD.ResourceFactory.CreateBuffer(new BufferDescription(sizeof(ushort) * (uint)indices.Length, BufferUsage.IndexBuffer));
            Substrate.App.GD.UpdateBuffer(IndexBuffer, 0, indices);

            SettingsBuffer = Substrate.App.GD.ResourceFactory.CreateBuffer(new BufferDescription(OutlineData.SIZE, BufferUsage.UniformBuffer));

            ShaderSet = CreateShaderSet();
            PerItemResourceLayout = CreatePerItemResourceLayout();

            var sampler = Substrate.App.GD.ResourceFactory.CreateSampler(SamplerDescription.Point);
            //ItemResourceSet = Resources.GD.ResourceFactory.CreateResourceSet(new ResourceSetDescription(PerItemResourceLayout, WorldBuffer,
            //World.GetVieewports()[0].ActorIdTex, sampler));

            //var world = Matrix4x4.CreateTranslation(Vector3.Zero);
            SetSettings(OutlineSettings);

            Pipeline = Substrate.App.GD.ResourceFactory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleAlphaBlend,
                new DepthStencilStateDescription(false, false, ComparisonKind.Never),
                RasterizerStateDescription.CullNone,
                PrimitiveTopology.TriangleList,
                ShaderSet,
                new[] { Substrate3D.GetProjViewLayout(), PerItemResourceLayout },
                Substrate3D.GetMainFrameBufdOutputDesc()));
        }

        private ShaderSetDescription CreateShaderSet()
        {
            //ImTool.Shaders.SPIR_V._3D.Grid.GridFrag.glsl
            var gridVert = new ShaderDescription(ShaderStages.Vertex, Resources.GetEmbeddedAsset("Assets.Shaders._3D.Fullscreen.OutlineVert.spirv"), "main");
            var gridFrag = new ShaderDescription(ShaderStages.Fragment, Resources.GetEmbeddedAsset("Assets.Shaders._3D.Fullscreen.OutlineFrag.spirv"), "main");
            var gridShaders = Substrate.App.GD.ResourceFactory.CreateFromSpirv(gridVert, gridFrag);

            ShaderSetDescription shaderSet = new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                        new VertexElementDescription("Uv", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)
                        )
                },
                gridShaders);

            return shaderSet;
        }

        private ResourceLayout CreatePerItemResourceLayout()
        {
            ResourceLayout worldTextureLayout = Substrate.App.GD.ResourceFactory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("OutlineData", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("IdBuffer", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("DiffusIdBufferSampler", ResourceKind.Sampler, ShaderStages.Fragment)
                    )
                );

            return worldTextureLayout;
        }

        public void UdpateFBResources(MainFrameBufferResource fbRes)
        {
            //var sampler = Resources.GD.ResourceFactory.CreateSampler(SamplerDescription.Point);

            if (ItemResourceSet != null && !ItemResourceSet.IsDisposed)
            {
                ItemResourceSet.Dispose();
            }

            ItemResourceSet = Substrate.App.GD.ResourceFactory.CreateResourceSet(new ResourceSetDescription(PerItemResourceLayout, SettingsBuffer,
            fbRes.SelectedMaskTex, Substrate.App.GD.Aniso4xSampler));
        }

        public void Render(CommandList cmdList, MainFrameBufferResource fbRes)
        {
            cmdList.SetPipeline(Pipeline);

            cmdList.SetVertexBuffer(0, VertBuffer);
            cmdList.SetIndexBuffer(IndexBuffer, IndexFormat.UInt16);

            cmdList.SetGraphicsResourceSet(1, ItemResourceSet);
            cmdList.DrawIndexed(6, 1, 0, 0, 0);
        }

        public struct VertexDefinition
        {
            public const uint SizeInBytes = 20;

            public float X;
            public float Y;
            public float Z;

            public float U;
            public float V;

            public VertexDefinition(float x, float y, float z, float u, float v)
            {
                X = x;
                Y = y;
                Z = z;

                U = u;
                V = v;
            }
        }

        public struct OutlineData
        {
            public const uint SIZE = 48;

            public Vector4 Color;
            public float Thickness;

            public float _padding1;
            public float _padding2;
            public float _padding3;

            public OutlineData()
            {
                Color = new Vector4(0.5f, 0.2f, 0.2f, 1f);
                Thickness = 10;
            }
        }
    }
}
