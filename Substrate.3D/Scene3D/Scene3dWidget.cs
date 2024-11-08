﻿
using ImGuiNET;
//using ImGuizmoNET;
using System.Numerics;
using Veldrid;
using Vulkan.Xlib;
using Substrate.Scene3D;

namespace Substrate.Widgets
{
    public class Scene3dWidget : SceneWidget<MainFrameBufferResource>
    {
        protected bool IsExternalWorld;
        public World WorldScene;
        protected CameraActor Camera;
        private Vector2 LastMousePos;

        protected float CamMoveSpeed = 10.0f;
        protected float MouseSenstivity = 5f;

        public string[] SnapLevels = new string[] { "0.1", "0.2", "0.5", "1", "2", "5", "10" };
        //protected OPERATION TransformOperation = OPERATION.TRANSLATE;
        //protected MODE TransformMode = MODE.WORLD;
        protected float[] TransformSnap = null;
        protected int TransformSelectedSnapIdx = 3;

        public bool ShowDebugInfo = true;
        private RenderStats RenderStats;

        public SelectedOutlinePostProcess OutlinePostProc;

        // Create a scene to render a world and manage the world itself
        public Scene3dWidget() : base()
        {
            FrameBufferResource = new MainFrameBufferResource();
            FrameBufferResource.InitFramebuffer(256, 256);

            IsExternalWorld = false;
            WorldScene = new World();
            WorldScene.RegisterViewport(this);
            WorldScene.Init(this);

            OutlinePostProc = new();
            OutlinePostProc.Init();

            //ImGuizmo.Enable(true);
        }

        // Crate a scene for an exteranly managed world, eg a camera view into one
        public Scene3dWidget(World world) : base()
        {
            IsExternalWorld = true;
            WorldScene = world;

            OutlinePostProc = new();
            OutlinePostProc.Init();
            OutlinePostProc.UdpateFBResources(FrameBufferResource);
        }

        protected override void Init(Vector2 size)
        {
            base.Init(size);
            OutlinePostProc?.UdpateFBResources(FrameBufferResource);
        }

        public void SetCamera(CameraActor camera)
        {
            Camera = camera;
        }

        public CameraActor GetCamera() => Camera;

        public void HandleInput(double dt)
        {
            if (ImGui.IsWindowFocused())
            {
                if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
                {
                    var cammoveSpeed = (float)(CamMoveSpeed * dt);

                    if (ImGui.IsKeyDown(ImGuiKey.W))
                        Camera.Transform.Position += Camera.Transform.Forward * cammoveSpeed;
                    else if (ImGui.IsKeyDown(ImGuiKey.S))
                        Camera.Transform.Position += Camera.Transform.Forward * -cammoveSpeed;

                    if (ImGui.IsKeyDown(ImGuiKey.A))
                        Camera.Transform.Position += Camera.Transform.Left * cammoveSpeed;
                    else if (ImGui.IsKeyDown(ImGuiKey.D))
                        Camera.Transform.Position += Camera.Transform.Left * -cammoveSpeed;

                    if (ImGui.IsKeyDown(ImGuiKey.Q))
                        Camera.Transform.Position += Camera.Transform.Up * cammoveSpeed;
                    else if (ImGui.IsKeyDown(ImGuiKey.E))
                        Camera.Transform.Position += Camera.Transform.Up * -cammoveSpeed;

                    var mouseDelta = LastMousePos - ImGui.GetMousePos();
                    LastMousePos = ImGui.GetMousePos();
                    var angles = Camera.Transform.RotationEuler;
                    angles.Y += (float)(-mouseDelta.Y * (MouseSenstivity * dt));
                    angles.X += (float)(mouseDelta.X * (MouseSenstivity * dt));
                    Camera.Transform.RotationEuler = angles;
                }

                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    var selectionId = FrameBufferResource.GetScreenSelectedId();
                    WorldScene.SelectItem(selectionId);
                    Console.WriteLine($"SelectionId: {selectionId.Id}, {selectionId.SubId} ({selectionId.B0}, {selectionId.B1}, {selectionId.B2}, {selectionId.B3})");
                }

                if (ImGui.IsKeyDown(ImGuiKey.Escape))
                {
                    //WorldScene.SelectItem(new SelectableID(0xFFFFFFFF, 0xFF));
                    WorldScene.ClearSelected();
                }
            }

            LastMousePos = ImGui.GetMousePos();
        }

        public override void Render(double dt)
        {
            //ImGuizmo.SetDrawlist();

            base.Render(dt);
            CommandList.ClearColorTarget(1, new RgbaFloat(-float.MaxValue, -float.NaN, -float.NaN, -float.NaN));
            CommandList.ClearColorTarget(2, new RgbaFloat(0, 0, 0, 0));

            // Update camera aspect
            Camera.AspectRatio = (float)FrameBufferResource.FrameBuffer.Width / (float)FrameBufferResource.FrameBuffer.Height;

            HandleInput(dt);

            if (!IsExternalWorld)
                WorldScene.Update(dt);

            var pos = ImGui.GetWindowPos();
            //ImGuizmo.SetRect(pos.X + 4, pos.Y + 30, FrameBufferResource.SceneTex.Width, FrameBufferResource.SceneTex.Height);
            //ImGuizmo.Enable(true);
            RenderStats = WorldScene.Render(dt, CommandList, Camera);

            OutlinePostProc?.Render(CommandList, FrameBufferResource);
            //WorldScene.DrawTransform();
        }

        public override void DrawOverlays(double dt, Vector2 size)
        {
            base.DrawOverlays(dt, size);

            if (ShowDebugInfo)
                DrawDebugOverlay();

            DrawTransform();

            var pos = ImGui.GetWindowPos();
            //var size = ImGui.GetWindowSize();
            //DrawViewCube(new Vector2(pos.X + size.X - 100, pos.Y + 25));
            //DrawTransformSettings(new Vector2(size.X - (150 + 165), 40));
            //DrawFps(new Vector2(size.X - (150), 75));

            ImGui.Text($"Actors: {RenderStats.NumActors}, Rendered: {RenderStats.NumRenderedActors}");
        }

        private void DrawTransform()
        {
            if (WorldScene != null)
            {
                //WorldScene.DrawTransform(TransformOperation, TransformMode, ref TransformSnap);
            }
        }

        private void DrawTransformSettings(Vector2 pos)
        {
            /*
            var ogPos = ImGui.GetCursorPos();
            ImGui.SetCursorPos(pos);

            // Operation
            DrawTransformOpSelector(OPERATION.TRANSLATE, "", "Translate");
            UIMergeButton();
            DrawTransformOpSelector(OPERATION.ROTATE, "", "Rotate");
            UIMergeButton();
            DrawTransformOpSelector(OPERATION.SCALE, "", "Scale");

            // Mode
            //DrawTransformModeSelector();

            // Snapping
            DrawSnapSettings();

            ImGui.SetCursorPos(ogPos);
            */
        }

        /*
        private void DrawTransformModeSelector()
        {
            ImGui.PushID("Mode");
            var modeIcon = TransformMode == MODE.WORLD ? "" : "";
            var modeTxt = TransformMode == MODE.WORLD ? "" : " L ";
            var modeToolTip = TransformMode == MODE.WORLD ? "World" : "Local";
            if (Widgets.IconButton(modeIcon, modeTxt))
                TransformMode = TransformMode == MODE.WORLD ? MODE.LOCAL : MODE.WORLD;

            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(modeToolTip);
        }
        */

        private static void UIMergeButton()
        {
            ImGui.SameLine();
            ImGui.Dummy(new Vector2(-12, 0));
            ImGui.SameLine();
        }

        /*private void DrawTransformOpSelector(OPERATION op, string iconStr, string name)
        {
            ImGui.PushID(name);
            if (Widgets.IconButton(iconStr, bgColor: (TransformOperation & op) != 0 ? ImToolColors.HexSelectedUnderline : null))
                TransformOperation ^= op;

            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(name);
        }*/

        private void DrawSnapSettings()
        {
            bool updateSnaps = false;
            /*if (Widgets.IconButton("", bgColor: TransformSnap != null ? ImToolColors.HexSelectedUnderline : null))
            {
                if (TransformSnap == null)
                {
                    updateSnaps = true;
                }
                else
                {
                    TransformSnap = null;
                }
            }*/

            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("Grid Snapping");

            UIMergeButton();
            ImGui.SetNextItemWidth(55);
            if (ImGui.Combo("###SnapLevels", ref TransformSelectedSnapIdx, SnapLevels, SnapLevels.Length))
                updateSnaps = true;

            if (updateSnaps)
            {
                var snapLevel = float.TryParse(SnapLevels[TransformSelectedSnapIdx], out float res) ? res : 1f;
                TransformSnap = new float[] { snapLevel, snapLevel, snapLevel };
            }
        }

        private void DrawViewCube(Vector2 cubePos)
        {
            var pos = Camera.Transform.Position;
            Camera.Transform.Position = new Vector3(0, 0, 0);
            var viewM = Camera.Transform.World;
            var view2 = viewM.ToFloatArrray();
            Camera.Transform.Position = pos;

            //ImGuizmo.ViewManipulate(ref view2[0], 1f, cubePos, new Vector2(100, 100), 1);

            if (ImGui.IsWindowFocused())
            {
                viewM.FromFloatArray(view2);
                Camera.Transform.World = viewM;
                Camera.Transform.Position = pos;
            }
        }

        private void DrawFps(Vector2 pos)
        {
            var ogPos = ImGui.GetCursorPos();
            ImGui.SetCursorPos(pos);

            Substrate.Fonts.PushFont("Bold");
            ImGui.Text($"FPS: {AvgFPS:0}");
            Substrate.Fonts.PopFont();

            ImGui.SetCursorPos(ogPos);
        }

        private void DrawDebugOverlay()
        {

        }
    }
}
