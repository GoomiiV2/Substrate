using Hexa.NET.ImGui;

namespace Substrate.Test
{
    public class ShowCaseWindow : WindowBase
    {
        public override void Draw(float dt)
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2 (300, 400), ImGuiCond.Once);
            if (ImGui.Begin("Showcase", ref ShouldClose))
            {
                if (ImGui.CollapsingHeader("Fonts", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    DrawFonts(dt);
                }

                if (ImGui.CollapsingHeader("Buttons", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    DrawButtons(dt);
                }

                if (ImGui.CollapsingHeader("Images"))
                {
                    DrawImages(dt);
                }
            }
            ImGui.End();
        }

        private void DrawFonts(float dt)
        {
            Substrate.Fonts.PushFont(FontManager.Bold);
            ImGui.Text("Bold");
            Substrate.Fonts.PopFont();

            Substrate.Fonts.PushFont(FontManager.Regular);
            ImGui.Text("Regular");
            Substrate.Fonts.PopFont();

            Substrate.Fonts.PushFont(FontManager.Italic);
            ImGui.Text("Italic");
            Substrate.Fonts.PopFont();

            Substrate.Fonts.PushFont(FontManager.BoldItalic);
            ImGui.Text("BoldItalic");
            Substrate.Fonts.PopFont();

            Substrate.Fonts.PushFont(FontManager.FAS);
            ImGui.Text($"{FASIcons.ArrowDown91}");
            ImGui.SameLine();
            ImGui.Text($"{FASIcons.UpLong}");
            ImGui.SameLine();
            ImGui.Text($"{FASIcons.HardDrive}");
            ImGui.SameLine();
            ImGui.Text($"{FASIcons.Plus}");
            Substrate.Fonts.PopFont();
        }

        private void DrawImages(float dt)
        {

        }

        private void DrawButtons(float dt)
        {
            ImguiEx.ButtonInfo("Info"u8);
            ImGui.SameLine();
            ImguiEx.ButtonSuccess("Success"u8);
            ImGui.SameLine();
            ImguiEx.ButtonWarn("Warn"u8);
            ImGui.SameLine();
            ImguiEx.ButtonError("Error"u8);

            if (ImGui.Button("Show Message box"))
                Substrate.Modals.ShowMessageBox("Test Message Box", "<3");

            ImGui.SameLine();
            if (ImGui.Button("Open File"))
            {
                Substrate.Modals.OpenFile((string filePath) =>
                {
                    Substrate.Modals.ShowMessageBox("File Path", filePath);
                },
                "Test File Browser");
            }
        }
    }
}
