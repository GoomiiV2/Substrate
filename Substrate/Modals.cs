using Substrate.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Substrate
{
    public partial class Modals
    {
        private FileBrowserModal FileBrowser;

        public Modals()
        {
            FileBrowser = new FileBrowserModal();
            Substrate.App.AddModal(FileBrowser);
        }

        public void ShowMessageBox(string title, string message)
        {
            var modal = new MessageBoxModal(title, message);
            modal.Show();
            Substrate.App.AddModal(modal);
        }

        public void OpenFile(Action<string> openFileCb, string title = "", string baseDir = null, string filter = null, int filterIndex = 0, string defaultExt = null) =>
            FileBrowser.OpenFile(openFileCb, title, baseDir, filter, filterIndex, defaultExt);
    }
}
