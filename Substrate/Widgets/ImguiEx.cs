using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Substrate
{
    public partial class ImguiEx
    {
        public static void SetNextItemFillWidth()
        {
            ImGui.SetNextItemWidth(1000);
        }

        public static bool ButtonSuccess(string label, Vector2? size = null, string font = null)
        {
            return ButtonThemed(label, Substrate.Theme.Success, size, font);
        }

        public static bool ButtonInfo(string label, Vector2? size = null, string font = null)
        {
            return ButtonThemed(label, Substrate.Theme.Info, size, font);
        }

        public static bool ButtonWarn(string label, Vector2? size = null, string font = null)
        {
            return ButtonThemed(label, Substrate.Theme.Warning, size, font);
        }

        public static bool ButtonError(string label, Vector2? size = null, string font = null)
        {
            return ButtonThemed(label, Substrate.Theme.Error, size, font);
        }

        public static bool ButtonThemed(string label, ThemeColor color, Vector2? size = null, string font = null)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, color.Primary);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color.Lighter);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, color.Darker);
            Substrate.Fonts.PushFont(font ?? FontManager.Bold);
            var result = size != null ? ImGui.Button(label, size.Value) : ImGui.Button(label);
            Substrate.Fonts.PopFont();
            ImGui.PopStyleColor(3);

            return result;
        }
    }
}
