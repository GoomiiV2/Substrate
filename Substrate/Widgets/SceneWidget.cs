using ImGuiNET;
using Substrate;
using System.Numerics;
using Veldrid;
using Vulkan.Xlib;

namespace Substrate.Widgets
{
    // A scene widget to render a commandlist to a framebuffer in ImGUI
    public class SceneWidget<T> where T : FrameBufferResourceBase, new()
    {
        protected T FrameBufferResource;

        protected IntPtr SceneTexBinding;
        protected CommandList CommandList;
        protected double LastFrameTime;
        protected double AvgFPS = 0f;

        private bool NeedsToInit = true;

        public Framebuffer GetFramebuffer() => FrameBufferResource.FrameBuffer;
        public bool IsHovered = false;

        public SceneWidget()
        {
            LastFrameTime = (DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond);
            FrameBufferResource = new();

            Init(new Vector2(256, 256));
        }

        public void DrawWindow(string title)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 0);
            if (ImGui.Begin(title))
            {
                var size = ImGui.GetContentRegionAvail();
                size.X = size.X <= 0 ? 1 : size.X;
                size.Y = size.Y <= 0 ? 1 : size.Y;

                Draw(size);

                ImGui.End();
            }
            ImGui.PopStyleVar(1);
        }

        public virtual void Draw(Vector2 size)
        {
            size.X = size.X <= 0 ? 16 : size.X;
            size.Y = size.Y <= 0 ? 16 : size.Y;

            if ((NeedsToInit || size.X != FrameBufferResource.FrameBuffer.Width || size.Y != FrameBufferResource.FrameBuffer.Height) && !ImGui.IsAnyMouseDown())
                Init(size);

            IsHovered = ImGui.IsMouseHoveringRect(ImGui.GetCursorScreenPos(), ImGui.GetCursorScreenPos() + size);

            CommandList.Begin();
            CommandList.SetFramebuffer(FrameBufferResource.FrameBuffer);

            double dt = (DateTime.UtcNow.Ticks - LastFrameTime) / TimeSpan.TicksPerSecond;
            CalcFPS(dt);
            Render(dt);
            LastFrameTime = DateTime.UtcNow.Ticks;

            //CommandList.CopyTexture(ActorIdTex, StagingTex);
            CommandList.End();
            Substrate.App.GD.SubmitCommands(CommandList);
            Substrate.App.GD.WaitForIdle();

            ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(-2, -2));
            var cursorPos = ImGui.GetCursorPos();
            ImGui.Image(SceneTexBinding, size);

            ImGui.SetCursorPos(cursorPos);

            DrawOverlays(dt, size);
        }

        protected virtual void Init(Vector2 size)
        {
            if (FrameBufferResource.SceneTex != null)
            {
                Substrate.App.Controller.RemoveImGuiBinding(FrameBufferResource.SceneTex);
            }

            CommandList ??= Substrate.App.GD.ResourceFactory.CreateCommandList();

            FrameBufferResource.InitFramebuffer((uint)size.X, (uint)size.Y);
            SceneTexBinding = Substrate.App.Controller.GetOrCreateImGuiBinding(Substrate.App.GD.ResourceFactory, FrameBufferResource.SceneTex);

            NeedsToInit = false;
        }

        // Render intot he commandlist here
        public virtual void Render(double dt)
        {
            CommandList.ClearColorTarget(0, new RgbaFloat(0.69f, 0.61f, 0.85f, 1.0f));
        }

        public virtual void DrawOverlays(double dt, Vector2 size)
        {
            ImGui.Text($"Delta Time: {dt:0.#####}, FPS: {AvgFPS:0}");
        }

        private void CalcFPS(double dt)
        {
            var expSmoothing = 0.9f;
            AvgFPS = expSmoothing * AvgFPS + (1f - expSmoothing) * 1f / dt;
        }
    }
}
