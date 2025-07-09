using Hexa.NET.ImGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Substrate
{
    public class ModalBase
    {
        public string ID       = "";
        public bool ShouldShow = false;

        public void SystemDraw(float dt)
        {
            if (ShouldShow && !ImGui.IsPopupOpen(ID))
            {
                ImGui.OpenPopup(ID);
            }

            Draw(dt);
        }

        public virtual void Draw(float dt)
        {
            
        }

        public virtual void Show(bool show = true)
        {
            ShouldShow = show;
        }

        public virtual void Hide() => Show(false);

        public void Remove()
        {
            Substrate.App.RemoveModal(this);
        }
    }
}
