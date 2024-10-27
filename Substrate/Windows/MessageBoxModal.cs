using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Substrate
{
    public class MessageBoxModal : ModalBase
    {
        public string Title { get => ID; set => ID = value; }
        public string Message = "";

        public MessageBoxModal(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public override void Draw(float dt)
        {
            ImGui.SetNextWindowSize(new Vector2(-1f, -1f), ImGuiCond.Appearing);
            ImGui.SetNextWindowSizeConstraints(new Vector2(200, 40), new Vector2(2000, 1000));
            if (ImGui.BeginPopupModal(ID))
            {
                ImGui.TextWrapped(Message);

                if (ImguiEx.ButtonInfo("Ok", new Vector2(-1f, 0f)))
                    Remove();
            }
            ImGui.EndPopup();
        }
    }
}
