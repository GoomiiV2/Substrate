using ImGuiNET;
using Substrate.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Substrate
{
    // A temp wrapper
    public class FileBrowserModal : ModalBase
    {
        public FileBrowserModal()
        {
            ID = ImFileBrowser.POPUP_ID;
        }

        public void OpenFile(Action<string> openFileCb, string title = "", string baseDir = null, string filter = null, int filterIndex = 0, string defaultExt = null)
        {
            Show();
            ImFileBrowser.OpenFile(openFileCb, title, baseDir, filter, filterIndex, defaultExt);
        }

        public override void Draw(float dt)
        {
            var wasOpen = ImFileBrowser.IsOpen;
            ImFileBrowser.Draw();

            if (wasOpen && !ImFileBrowser.IsOpen)
                Hide();
        }
    }
}
