using ImGuiNET;
using Substrate.Scene3D;
using System.Numerics;
using Veldrid;

namespace Substrate
{
    public class MainFrameBufferResource : FrameBufferResourceBase
    {
        public Texture ActorIdTex { get; protected set; }
        public Texture SelectedMaskTex { get; protected set; }

        public override void InitFramebuffer(uint width, uint height)
        {
            SceneTex?.Dispose();
            DepthTex?.Dispose();
            ActorIdTex?.Dispose();
            SelectedMaskTex?.Dispose();
            FrameBuffer?.Dispose();

            Substrate.App.GD.WaitForIdle();

            SceneTex        = Substrate.App.GD.ResourceFactory.CreateTexture(TextureDescription.Texture2D(width, height, 1, 1, PixelFormat.R16_G16_B16_A16_UNorm, TextureUsage.RenderTarget | TextureUsage.Sampled));
            DepthTex        = Substrate.App.GD.ResourceFactory.CreateTexture(TextureDescription.Texture2D(width, height, 1, 1, PixelFormat.R32_Float, TextureUsage.DepthStencil             | TextureUsage.RenderTarget | TextureUsage.Sampled));
            ActorIdTex      = Substrate.App.GD.ResourceFactory.CreateTexture(TextureDescription.Texture2D(width, height, 1, 1, PixelFormat.R32_UInt, TextureUsage.RenderTarget              | TextureUsage.Sampled));
            SelectedMaskTex = Substrate.App.GD.ResourceFactory.CreateTexture(TextureDescription.Texture2D(width, height, 1, 1, PixelFormat.R8_UInt, TextureUsage.RenderTarget               | TextureUsage.Sampled));

            FrameBuffer = Substrate.App.GD.ResourceFactory.CreateFramebuffer(new FramebufferDescription()
            {
                DepthTarget = new FramebufferAttachmentDescription(DepthTex, 0),
                ColorTargets = new FramebufferAttachmentDescription[]
                {
                    new FramebufferAttachmentDescription(SceneTex, 0),
                    new FramebufferAttachmentDescription(ActorIdTex, 0),
                    new FramebufferAttachmentDescription(SelectedMaskTex, 0),
                }
            });
        }

        public SelectableID GetScreenSelectedId(Vector2? pos = null)
        {
            pos = pos ?? ImGui.GetMousePos() - ImGui.GetWindowPos();
            uint posX = (uint)pos.Value.X;
            uint posY = (uint)pos.Value.Y;
            uint width = 1;
            uint height = 1;

            if (ActorIdTex.Width <= posX || ActorIdTex.Height <= posY)
                return new SelectableID(SelectableID.NO_ID_VALUE, 0);

            try
            {
                var stagingTex = Substrate.App.GD.ResourceFactory.CreateTexture(TextureDescription.Texture2D(width, height, 1, 1, PixelFormat.R32_UInt, TextureUsage.Staging));
                Substrate.App.GD.WaitForIdle();

                var cmdList = Substrate.App.GD.ResourceFactory.CreateCommandList();
                cmdList.Begin();
                cmdList.CopyTexture(ActorIdTex, posX, posY, 0, 0, 0, stagingTex, 0, 0, 0, 0, 0, width, height, 1, 1);
                cmdList.End();
                Substrate.App.GD.SubmitCommands(cmdList);
                Substrate.App.GD.WaitForIdle();
                cmdList.Dispose();

                var mappedTex = Substrate.App.GD.Map(stagingTex, MapMode.Read);
                var texData = new MappedResourceView<SelectableID>(mappedTex);
                var selId = texData[0, 0];
                Substrate.App.GD.Unmap(stagingTex);

                stagingTex.Dispose();

                return selId;
            }
            catch (Exception ex)
            {
                return new SelectableID(SelectableID.NO_ID_VALUE, 0);
            }
        }
    }
}
