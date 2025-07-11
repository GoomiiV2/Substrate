using Hexa.NET.ImGui;
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

        public static bool ButtonSuccess(ReadOnlySpan<byte> label, Vector2? size = null, string font = null)
        {
            return ButtonThemed(label, Substrate.Theme.Success, size, font);
        }

        public static bool ButtonInfo(ReadOnlySpan<byte> label, Vector2? size = null, string font = null)
        {
            return ButtonThemed(label, Substrate.Theme.Info, size, font);
        }

        public static bool ButtonWarn(ReadOnlySpan<byte> label, Vector2? size = null, string font = null)
        {
            return ButtonThemed(label, Substrate.Theme.Warning, size, font);
        }

        public static bool ButtonError(ReadOnlySpan<byte> label, Vector2? size = null, string font = null)
        {
            return ButtonThemed(label, Substrate.Theme.Error, size, font);
        }

        public static bool ButtonThemed(ReadOnlySpan<byte> label, ThemeColor color, Vector2? size = null, string font = null)
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
        
        // Draw a button with a font awesome icon
        public static bool IconButton(string icon, string text = null, Vector4? iconColor = null, Vector4? bgColor = null)
        {
            var result = false;

            if (text == null)
                text = "";
    
            var sizeOfSpace = ImGui.CalcTextSize(" ").X;
    
            Substrate.Fonts.PushFont("FAS");
            var iconSize  = ImGui.CalcTextSize(icon);
            var iconWidth = (int) (Math.Ceiling(iconSize.X) / sizeOfSpace) + 2;
            Substrate.Fonts.PopFont();

            var pos = ImGui.GetCursorScreenPos();

            if (bgColor != null)
                ImGui.PushStyleColor(ImGuiCol.Button, bgColor.Value);

            result = ImGui.Button(text.PadLeft(text.Length + iconWidth));

            if (bgColor != null)
                ImGui.PopStyleColor();

            ImGui.SameLine();
            var secondPos = ImGui.GetCursorPosX();
            Substrate.Fonts.PushFont("FAS");
            ImGui.PushStyleColor(ImGuiCol.Text, iconColor ?? System.Numerics.Vector4.One);
            ImGui.SetCursorScreenPos(pos + new Vector2(text == "" ? 8 : 6, (ImGui.GetItemRectSize().Y - iconSize.Y - 6) /2));
            ImGui.Text(icon);
            //ImGui.RenderText(pos   + new Vector2(text == "" ? 8 : 6, (ImGui.GetItemRectSize().Y - iconSize.Y) /2), icon);
            ImGui.PopStyleColor(1);
            Substrate.Fonts.PopFont();

            return result;
        }
    }
}
