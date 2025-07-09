using Hexa.NET.ImGui;
using System.Numerics;
using Veldrid;

namespace Substrate
{
    public class FrameBufferResourceBase
    {
        public OutputDescription OutputDescription { get; protected set; }
        public Framebuffer FrameBuffer { get; protected set; }
        public Texture SceneTex { get; protected set; }
        public Texture DepthTex { get; protected set; }

        public FrameBufferResourceBase()
        {

        }

        public virtual void InitFramebuffer(uint width, uint height)
        {
            SceneTex?.Dispose();
            DepthTex?.Dispose();
            FrameBuffer?.Dispose();

            Substrate.App.GD.WaitForIdle();

            SceneTex = Substrate.App.GD.ResourceFactory.CreateTexture(TextureDescription.Texture2D(width, height, 1, 1, PixelFormat.R16_G16_B16_A16_Float, TextureUsage.RenderTarget | TextureUsage.Sampled, TextureSampleCount.Count8));
            DepthTex = Substrate.App.GD.ResourceFactory.CreateTexture(TextureDescription.Texture2D(width, height, 1, 1, PixelFormat.R32_Float, TextureUsage.DepthStencil));

            FrameBuffer = Substrate.App.GD.ResourceFactory.CreateFramebuffer(new FramebufferDescription()
            {
                DepthTarget = new FramebufferAttachmentDescription(DepthTex, 0),
                ColorTargets = new FramebufferAttachmentDescription[]
                {
                    new FramebufferAttachmentDescription(SceneTex, 0),
                }
            });
        }
    }
}
