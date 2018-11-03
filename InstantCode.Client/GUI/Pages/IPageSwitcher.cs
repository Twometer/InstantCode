using System.Windows.Controls;

namespace InstantCode.Client.GUI.Pages
{
    public interface IPageSwitcher
    {
        void SwitchPage(UserControl newPage);
    }
}
