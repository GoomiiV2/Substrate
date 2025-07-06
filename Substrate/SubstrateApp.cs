using Veldrid.Sdl2;
using Veldrid;
using Veldrid.StartupUtilities;
using System.Diagnostics;
using System.Numerics;
using ImGuiNET;
using Serilog.Core;
using Substrate.Logging;
using System.Reflection.Metadata.Ecma335;
using Vortice.DXGI;
using Vulkan;

namespace Substrate
{
    public class SubstrateApp
    {
        public Sdl2Window Window { get; protected set; }
        public GraphicsDevice GD { get; protected set; }
        public CommandList CL { get; protected set; }
        public ImGuiController Controller { get; protected set; }

        protected List<WindowBase> Windows             = new List<WindowBase>();
        protected List<WindowBase> PendingCloseWindows = new List<WindowBase>();
        protected List<WindowBase> PendingAddWindows   = new List<WindowBase>();

        protected List<ModalBase> Modals              = new List<ModalBase>();
        protected List<ModalBase> PendingRemoveModals = new List<ModalBase>();
        protected List<ModalBase> PendingAddModals    = new List<ModalBase>();

        protected string Title = "Substrate App";

        public void Init(string[] args)
        {
            Substrate.Log.Info(LogCat.Substrate, "Starting {appName} App", Substrate.Config.AppName);
            
            var gfxBackend = GetGraphicsBackendAPI();
            /*VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(Sdl2Native.SDL_WINDOWPOS_CENTERED, Sdl2Native.SDL_WINDOWPOS_CENTERED, 1280, 720, WindowState.Normal, Title),
                new GraphicsDeviceOptions(Substrate.Config.Rendering.DebugRenderingDevice, null, true, ResourceBindingModel.Improved, true, true),
                gfxBackend,
                out var window, out var gd);*/
            
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(Sdl2Native.SDL_WINDOWPOS_CENTERED, Sdl2Native.SDL_WINDOWPOS_CENTERED, 1280, 720, WindowState.Normal, Title),
                new GraphicsDeviceOptions(false, null, true, ResourceBindingModel.Improved, true, true),
                GraphicsBackend.Vulkan,
                out var window, out var gd);

            Window = window;
            GD     = gd;

            Substrate.Log.Trace(LogCat.Substrate, "Created window with {gfxBackend}", gfxBackend);

            Window.Resized += () =>
            {
                GD.MainSwapchain.Resize((uint)Window.Width, (uint)Window.Height);
                Controller.WindowResized(Window.Width, Window.Height);
            };

            CL = GD.ResourceFactory.CreateCommandList();
            Controller = new ImGuiController(GD, GD.MainSwapchain.Framebuffer.OutputDescription, Window.Width, Window.Height);

            AppInit(args);
        }

        public void StartRenderLoop()
        {
            var stopwatch = Stopwatch.StartNew();
            float deltaTime = 0f;
            while (Window.Exists)
            {
                deltaTime = stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
                stopwatch.Restart();

                if (!Window.Exists)
                    break;

                InnerDraw(deltaTime);
            }
        }

        private GraphicsBackend GetGraphicsBackendAPI()
        {
            var configGfxApi = Substrate.Config.Rendering.GfxAPI;
            if (GraphicsDevice.IsBackendSupported(configGfxApi))
                return configGfxApi;

            if (OperatingSystem.IsWindows())
                return GraphicsBackend.Direct3D11;

            if (GraphicsDevice.IsBackendSupported(GraphicsBackend.Vulkan))
                return GraphicsBackend.Vulkan;

            if (OperatingSystem.IsMacOS() && GraphicsDevice.IsBackendSupported(GraphicsBackend.Metal))
                return GraphicsBackend.Metal;

            return GraphicsBackend.OpenGL;
        }

        protected void InnerDraw(float dt)
        {
            InputSnapshot snapshot = Window.PumpEvents();
            Controller.Update(dt, snapshot);

            if (Substrate.Theme.Font != null && Substrate.Fonts.Fonts.ContainsKey(Substrate.Theme.Font))
                Substrate.Fonts.PushFont(Substrate.Theme.Font);
            else
                Substrate.Fonts.PushFont(FontManager.ProggyClean);

            Draw(dt);
            DrawWindows(dt);
            DrawModals(dt);

            Substrate.Fonts.PopFont();

            CL.Begin();
            CL.SetFramebuffer(GD.MainSwapchain.Framebuffer);
            CL.ClearColorTarget(0, new RgbaFloat(0.45f, 0.55f, 0.6f, 1f));
            Controller.Render(GD, CL);
            CL.End();
            GD.SubmitCommands(CL);
            GD.SwapBuffers(GD.MainSwapchain);

            //Thread.Sleep(100);
        }

        protected virtual void AppInit(string[] args)
        {

        }

        protected virtual void Draw(double dt)
        {
            ImGui.ShowDemoWindow();
        }

        public virtual void AddWindow(WindowBase window)
        {
            PendingAddWindows.Add(window);
        }

        public virtual void CloseWindow(WindowBase window)
        {
            PendingCloseWindows.Add(window);
        }

        private void DrawWindows(float dt)
        {
            foreach (var window in Windows)
            {
                window.SystemDraw(dt);
            }

            foreach (var pendignAddWindow in PendingAddWindows)
            {
                Windows.Add(pendignAddWindow);
            }
            PendingAddWindows.Clear();

            foreach (var pendingClose in PendingCloseWindows)
            {
                Windows.Remove(pendingClose);
            }

            PendingCloseWindows.Clear();
        }

        public virtual void AddModal(ModalBase modal)
        {
            PendingAddModals.Add(modal);
        }

        public virtual void RemoveModal(ModalBase modal)
        {
            PendingRemoveModals.Add(modal);
        }

        private void DrawModals(float dt)
        {
            foreach (var modal in Modals)
            {
                modal.SystemDraw(dt);
            }

            foreach (var pendingAdd in PendingAddModals)
            {
                Modals.Add(pendingAdd);
            }
            PendingAddModals.Clear();

            foreach (var pendingRemove in PendingRemoveModals)
            {
                Modals.Remove(pendingRemove);
            }

            PendingRemoveModals.Clear();
        }
    }
}
