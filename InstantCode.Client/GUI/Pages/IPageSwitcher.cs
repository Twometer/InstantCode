using System.Windows.Controls;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace InstantCode.Client.GUI.Pages
{
    public interface IPageSwitcher
    {
        void SwitchPage(UserControl newPage);
        UserControl GetCurrentPage();
    }
}
