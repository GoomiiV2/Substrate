using System.Numerics;
using Veldrid;
using Veldrid.SPIRV;
using Veldrid.Utilities;

namespace Substrate.Scene3D
{
    public partial class SimpleModel : IDisposable
    {
        public ResourceSet ItemResourceSet;
        public DeviceBuffer VertBuffer;
        public DeviceBuffer IndexBuffer;
        public List<MeshSection> MeshSections = new List<MeshSection>();
        public BoundingBox BoundingBox;

        public Pipeline Pipeline;
        public Pipeline OutlinePipeline;
        private ShaderSetDescription ShaderSet;
        public static ResourceLayout PerItemResourceLayout = null;
        public static ResourceLayout PerSectionResLayout = null;
        private ResourceSet DefaultPerSectionResSet;

        public SimpleModel()
        {
            Init();
        }

        public void Init()
        {
            var rf = Substrate.App.GD.ResourceFactory;
            ShaderSet = CreateShaderSet(rf);
            PerItemResourceLayout = PerItemResourceLayout ?? CreatePerItemResourceLayout(rf);

            PerSectionResLayout = PerSectionResLayout ?? rf.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("DiffuseTex", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("DiffuseSampler", ResourceKind.Sampler, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("Data", ResourceKind.UniformBuffer, ShaderStages.Fragment)
            )
            );

            var section = new MeshSection()
            {
                DiffuseTex = Substrate3D.GetMissingTex()
            };
            DefaultPerSectionResSet = CreateTexResourceSet(section);

            var blendState = new BlendStateDescription()
            {
                AttachmentStates = new[]
                {
                    BlendAttachmentDescription.AlphaBlend,      // color
                    BlendAttachmentDescription.OverrideBlend,   // id
                    BlendAttachmentDescription.Disabled    // Selected mask
                }
            };

            var depthStencil = new DepthStencilStateDescription(true, true, ComparisonKind.LessEqual)
            {
                StencilWriteMask = 0xFF
            };

            var pipelineDesc = new GraphicsPipelineDescription(
                blendState,
                depthStencil,
                new RasterizerStateDescription(FaceCullMode.Front, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, false),
                PrimitiveTopology.TriangleList,
                ShaderSet,
                new[] { Substrate3D.GetProjViewLayout(), PerItemResourceLayout, PerSectionResLayout },
                Substrate3D.GetMainFrameBufdOutputDesc());

            Pipeline = Substrate.App.GD.ResourceFactory.CreateGraphicsPipeline(pipelineDesc);

            CreateOutlinePipeline();
        }

        private void CreateOutlinePipeline()
        {
            var vert = new ShaderDescription(ShaderStages.Vertex, Resources.GetEmbeddedAsset("Assets.Shaders._3D.Mesh.MeshVert.spirv"), "main");
            var frag = new ShaderDescription(ShaderStages.Fragment, Resources.GetEmbeddedAsset("Assets.Shaders._3D.Mesh.MeshSolidFrag.spirv"), "main");
            var shaders = Substrate.App.GD.ResourceFactory.CreateFromSpirv(vert, frag);

            ShaderSetDescription shaderSet = new ShaderSetDescription(
                new[]
                {
                    // SimpleVertexDefinition
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                        new VertexElementDescription("Uvs", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                        new VertexElementDescription("Norms", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3)
                        )
                },
                shaders);

            var blendState = new BlendStateDescription()
            {
                AttachmentStates = new[]
                {
                    BlendAttachmentDescription.AlphaBlend,      // color
                    BlendAttachmentDescription.OverrideBlend,   // id
                    BlendAttachmentDescription.OverrideBlend    // Selected mask
                }
            };

            var depthStencil = new DepthStencilStateDescription(true, false, ComparisonKind.Always)
            {
                StencilWriteMask = 0xFF
            };

            var pipelineDesc = new GraphicsPipelineDescription(
                blendState,
                depthStencil,
                new RasterizerStateDescription(FaceCullMode.Front, PolygonFillMode.Solid, FrontFace.CounterClockwise, false, false),
                PrimitiveTopology.TriangleList,
                shaderSet,
                new[] { Substrate3D.GetProjViewLayout(), PerItemResourceLayout, PerSectionResLayout },
                Substrate3D.GetMainFrameBufdOutputDesc());

            OutlinePipeline = Substrate.App.GD.ResourceFactory.CreateGraphicsPipeline(pipelineDesc);
        }

        private ShaderSetDescription CreateShaderSet(ResourceFactory rf)
        {
            var vert = new ShaderDescription(ShaderStages.Vertex, Resources.GetEmbeddedAsset("Assets.Shaders._3D.Mesh.MeshVert.spirv"), "main");
            var frag = new ShaderDescription(ShaderStages.Fragment, Resources.GetEmbeddedAsset("Assets.Shaders._3D.Mesh.MeshFrag.spirv"), "main");
            var shaders = rf.CreateFromSpirv(vert, frag);

            ShaderSetDescription shaderSet = new ShaderSetDescription(
                new[]
                {
                    // SimpleVertexDefinition
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                        new VertexElementDescription("Uvs", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                        new VertexElementDescription("Norms", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3)
                        )
                },
                shaders);

            return shaderSet;
        }

        public static ResourceSet CreateTexResourceSet(MeshSection section)
        {
            var sampler = Substrate.App.GD.ResourceFactory.CreateSampler(new SamplerDescription(SamplerAddressMode.Wrap, SamplerAddressMode.Wrap, SamplerAddressMode.Wrap, SamplerFilter.MinPoint_MagPoint_MipPoint, ComparisonKind.Less, 0, 0, 0, 0, SamplerBorderColor.TransparentBlack));
            var rf = Substrate.App.GD.ResourceFactory;
            var resSet = rf.CreateResourceSet(new ResourceSetDescription(PerSectionResLayout,
                section.DiffuseTex,
                sampler,
                section.BufferData
            ));

            section.TexResourceSet = resSet;

            return resSet;
        }

        private ResourceLayout CreatePerItemResourceLayout(ResourceFactory rf)
        {
            ResourceLayout worldTextureLayout = rf.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment)
                )
            );

            return worldTextureLayout;
        }

        public unsafe void FullUpdateVertBuffer(ReadOnlySpan<SimpleVertexDefinition> verts)
        {
            if (VertBuffer != null) Substrate.App.GD.DisposeWhenIdle(VertBuffer);
            VertBuffer = Substrate.App.GD.ResourceFactory.CreateBuffer(new BufferDescription((uint)(SimpleVertexDefinition.SizeInBytes * verts.Length), BufferUsage.VertexBuffer));
            Substrate.App.GD.UpdateBuffer(VertBuffer, 0, verts);

            UpdateBoundingBox(verts);
        }

        public void FullUpdateIndices(ReadOnlySpan<uint> indices)
        {
            if (IndexBuffer != null) Substrate.App.GD.DisposeWhenIdle(IndexBuffer);
            IndexBuffer = Substrate.App.GD.ResourceFactory.CreateBuffer(new BufferDescription((uint)(sizeof(uint) * indices.Length), BufferUsage.IndexBuffer));
            Substrate.App.GD.UpdateBuffer(IndexBuffer, 0, indices);
        }

        public static SimpleModel CreateFromCube()
        {
            var model = new SimpleModel();
            var verts = new SimpleVertexDefinition[]
            {
                // Top
                new(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
                new(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
                new(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(1, 1)),
                new(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(0, 1)),
                // Bottom                                                             
                new(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(0, 0)),
                new(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(1, 0)),
                new(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(1, 1)),
                new(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0, 1)),
                // Left                                                               
                new SimpleVertexDefinition(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
                new SimpleVertexDefinition(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(1, 0)),
                new SimpleVertexDefinition(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(1, 1)),
                new SimpleVertexDefinition(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0, 1)),
                // Right                                                              
                new SimpleVertexDefinition(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(0, 0)),
                new SimpleVertexDefinition(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
                new SimpleVertexDefinition(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(1, 1)),
                new SimpleVertexDefinition(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(0, 1)),
                // Back                                                               
                new SimpleVertexDefinition(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
                new SimpleVertexDefinition(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
                new SimpleVertexDefinition(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(1, 1)),
                new SimpleVertexDefinition(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(0, 1)),
                // Front                                                              
                new SimpleVertexDefinition(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(0, 0)),
                new SimpleVertexDefinition(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(1, 0)),
                new SimpleVertexDefinition(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(1, 1)),
                new SimpleVertexDefinition(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(0, 1)),
            };
            var indices = new uint[]
            {
                0, 1, 2, 0, 2, 3,
                4, 5, 6, 4, 6, 7,
                8, 9, 10, 8, 10, 11,
                12, 13, 14, 12, 14, 15,
                16, 17, 18, 16, 18, 19,
                20, 21, 22, 20, 22, 23,
            };
            var section = new MeshSection()
            {
                Name = "Main",
                IndiceStart = 0,
                IndicesLength = (uint)indices.Length,
                DiffuseTex = Substrate3D.GetMissingTex()
            };

            section.TexResourceSet = CreateTexResourceSet(section);

            model.MeshSections.Add(section);

            model.FullUpdateVertBuffer(verts);
            model.FullUpdateIndices(indices);

            return model;
        }

        public void Dispose()
        {
            ItemResourceSet.Dispose();
            VertBuffer.Dispose();
            IndexBuffer.Dispose();
            MeshSections.Clear();
            PerSectionResLayout.Dispose();
            DefaultPerSectionResSet.Dispose();
        }

        private void UpdateBoundingBox(ReadOnlySpan<SimpleVertexDefinition> verts)
        {
            Vector3 min = Vector3.Zero;
            Vector3 max = Vector3.Zero;

            for (int i = 0; i < verts.Length; i++)
            {
                if (min.X > verts[i].X) min.X = verts[i].X;
                if (max.X < verts[i].X) max.X = verts[i].X;

                if (min.Y > verts[i].Y) min.Y = verts[i].Y;
                if (max.Y < verts[i].Y) max.Y = verts[i].Y;

                if (min.Z > verts[i].Z) min.Z = verts[i].Z;
                if (max.Z < verts[i].Z) max.Z = verts[i].Z;
            }

            BoundingBox = new BoundingBox(min, max);
        }

        public struct SimpleVertexDefinition
        {
            public const uint SizeInBytes = 12 + 8 + 12;

            public float X;
            public float Y;
            public float Z;

            public float U;
            public float V;

            public float NormX;
            public float NormY;
            public float NormZ;

            public SimpleVertexDefinition(Vector3 pos, Vector2 uv)
            {
                X = pos.X;
                Y = pos.Y;
                Z = pos.Z;
                U = uv.X;
                V = uv.Y;
            }
        }
    }
}
