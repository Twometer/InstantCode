using System.Windows.Controls;
using InstantCode.Client.GUI.Model;
using InstantCode.Client.Network;

namespace InstantCode.Client.GUI.Pages
{
    /// <summary>
    /// Interaction logic for ConnectedPage.xaml
    /// </summary>
    public partial class ConnectedPage : UserControl
    {
        private ServerEntry currentServer;

        public ConnectedPage(ServerEntry currentServer)
        {
            InitializeComponent();
            Header.Content = $"Connected to {currentServer.Name} ({currentServer.Ip})";
        }
    }
}
