using System.Windows.Controls;
using EnvDTE;
using InstantCode.Client.Editor;
using InstantCode.Client.GUI.Pages;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace InstantCode.Client.GUI.ToolWindow
{
    public partial class InstantCodeToolWindowControl : UserControl, IPageSwitcher
    {
        private EditorListener editorListener;

        public InstantCodeToolWindowControl()
        {
            this.InitializeComponent();
            SwitchPage(new ServerListPage(this));
            BindEvents();
        }

        public void BindEvents()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            editorListener = new EditorListener((DTE)Package.GetGlobalService(typeof(DTE)));
            editorListener.Register();
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