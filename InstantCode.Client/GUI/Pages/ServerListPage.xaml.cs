using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InstantCode.Client.GUI.Model;
using InstantCode.Client.Network;
using InstantCode.Protocol.Packets;

namespace InstantCode.Client.GUI.Pages
{
    /// <summary>
    /// Interaction logic for ServerListPage.xaml
    /// </summary>
    public partial class ServerListPage : UserControl
    {
        private readonly IPageSwitcher pageSwitcher;

        public ServerListPage(IPageSwitcher pageSwitcher)
        {
            InitializeComponent();
            this.pageSwitcher = pageSwitcher;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddServerDialog();
            dialog.ShowModal();

            if (dialog.ServerName.Trim() == "" || dialog.ServerIp.Trim() == "" || dialog.ServerUsername.Trim() == "" || dialog.ServerPassword.Trim() == "")
                return;
            ServerListView.Items.Add(new ServerEntry(dialog.ServerName, dialog.ServerIp, dialog.ServerUsername, dialog.ServerPassword));
        }

        private async void ServerListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var itm = ServerListView.SelectedItem;
            if (itm == null || !(itm is ServerEntry entry)) return;
            var progressDialog = new ProgressDialog($"Connecting to {entry.Name}...", () => { }, true);
            progressDialog.Show();
            var icClient = InstantCodeClient.Instance;
            await icClient.ConnectAsync(entry.Ip, 0xC0DE, entry.Password);
            await icClient.SendPacket(new P00Login(entry.Username))
                .WaitForReplyAsync<P01State>();
            progressDialog.Close();
            pageSwitcher.SwitchPage(new ConnectedPage(entry));
        }
    }
}
