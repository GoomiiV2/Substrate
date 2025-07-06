using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.ImageSharp;

namespace Substrate.Scene3D
{
    public static class Substrate3D
    {
        private static OutputDescription? MainFrameBufferOutputDescription;
        private static ResourceLayout ProjViewLayout;
        private static Texture MissingTex;

        public static OutputDescription GetMainFrameBufdOutputDesc()
        {
            MainFrameBufferOutputDescription = MainFrameBufferOutputDescription ?? new OutputDescription()
            {
                DepthAttachment = new OutputAttachmentDescription(PixelFormat.R32_Float),
                ColorAttachments = new OutputAttachmentDescription[]
                {
                    new OutputAttachmentDescription(PixelFormat.R16_G16_B16_A16_UNorm),
                    new OutputAttachmentDescription(PixelFormat.R32_UInt),
                    new OutputAttachmentDescription(PixelFormat.R8_UInt)
                }
            };

            return MainFrameBufferOutputDescription.Value;
        }

        public static ResourceLayout GetProjViewLayout()
        {
            ProjViewLayout = ProjViewLayout ?? Substrate.App.GD.ResourceFactory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ViewStateBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)
                )
            );

            return ProjViewLayout;
        }

        public static Texture GetMissingTex()
        {
            if (MissingTex == null)
            {
                var imgBytes = Resources.GetEmbeddedAsset("Assets.Textures.MissingTex.png");
                var img      = Image.Load<Rgba32>(imgBytes);
                var texImg   = new ImageSharpTexture(img);
                MissingTex   = texImg.CreateDeviceTexture(Substrate.App.GD, Substrate.App.GD.ResourceFactory);
            }

            return MissingTex;
        }
    }
}
