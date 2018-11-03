using System.Windows;
using Microsoft.VisualStudio.PlatformUI;

namespace InstantCode.Client.GUI
{
    /// <summary>
    /// Interaction logic for AddServerDialog.xaml
    /// </summary>
    public partial class AddServerDialog : DialogWindow
    {
        public string ServerName { get; private set; }
        public string ServerIp { get; private set; }
        public string ServerUsername { get; private set; }
        public string ServerPassword { get; private set; }

        public AddServerDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ServerName = ServerNameBox.Text;
            ServerIp = ServerIpBox.Text;
            ServerUsername = ServerUsrBox.Text;
            ServerPassword = ServerPwdBox.Password;
            Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
