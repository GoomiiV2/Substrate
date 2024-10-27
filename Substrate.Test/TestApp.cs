using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Substrate.Test
{
    public class TestApp : SubstrateApp
    {
        protected override void AppInit(string[] args)
        {
            AddWindow(new ShowCaseWindow());
            AddWindow(new SceneView());
        }

        protected override void Draw(double dt)
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    ImGui.MenuItem("Test :>");

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Themes"))
                {
                    foreach (var theme in Substrate.Themes)
                    {
                        if (ImGui.MenuItem(theme.Key))
                            Substrate.SetTheme(theme.Value.Name);
                    }

                    if (ImGui.MenuItem("Reload Themes"))
                        Substrate.ReloadThemes();

                    ImGui.EndMenu();
                }
            }
            ImGui.EndMainMenuBar();

            ImGui.DockSpaceOverViewport();

            ImGui.ShowDemoWindow();

            if (ImGui.Begin("Test Window"))
            {
                // Differn't way to get config / app class
                Substrate.Get<Config>().TestString = "Test \\o/";
                ImGui.Text(Substrate.Get<Config>().TestString); // Get tempalte function
                ImGui.Text(((Config)Substrate.Config).TestString); // Just cast the base
                ImGui.Text(SubApp.Config.TestString); // Create a static wrapper
            }
            ImGui.End();

            ThemeTest();
        }

        private static float CustomThemeColorDarkenAmount = 0f;
        public void ThemeTest()
        {
            if (ImGui.Begin("Theme Test"))
            {
                ImGui.Text("Custom Darken Amount");
                ImGui.SliderFloat("###Custom Darken Amount", ref CustomThemeColorDarkenAmount, -1f, 1f);
                DrawThemeColorLine("Success", Substrate.Theme.Success, CustomThemeColorDarkenAmount);
                DrawThemeColorLine("Info", Substrate.Theme.Info, CustomThemeColorDarkenAmount);
                DrawThemeColorLine("Warning", Substrate.Theme.Warning, CustomThemeColorDarkenAmount);
                DrawThemeColorLine("Error", Substrate.Theme.Error, CustomThemeColorDarkenAmount);
            }
            ImGui.End();
        }

        private void DrawThemeColorLine(string name, ThemeColor color, float? customAmount = null)
        {
            ImGui.Text(name);
            ImGui.ColorEdit4("Default", ref color.Primary, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoPicker);
            ImGui.SameLine();
            ImGui.ColorEdit4("Darker", ref color.Darker, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoPicker);
            ImGui.SameLine();
            ImGui.ColorEdit4("Lighter", ref color.Lighter, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoPicker);

            if (customAmount != null)
            {
                ImGui.SameLine();
                var customDarkend = color.Darkened(customAmount.Value);
                ImGui.ColorEdit4("Custom", ref customDarkend, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoPicker);
            }
        }
    }
}
