using System.Windows.Controls;
using InstantCode.Client.GUI.Pages;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace InstantCode.Client.GUI.ToolWindow
{
    public partial class InstantCodeToolWindowControl : UserControl, IPageSwitcher
    {
        public InstantCodeToolWindowControl()
        {
            this.InitializeComponent();
            SwitchPage(new ServerListPage(this));
        }

        public void SwitchPage(UserControl newPage)
        {
            ContentPresenter.Content = newPage;
        }

        public UserControl GetCurrentPage()
        {
            return ContentPresenter.Content as UserControl;
        }
    }
}