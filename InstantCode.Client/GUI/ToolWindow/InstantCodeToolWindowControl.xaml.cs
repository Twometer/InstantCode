using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using InstantCode.Client.Network;
using InstantCode.Protocol.Crypto;
using InstantCode.Protocol.Packets;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace InstantCode.Client.GUI
{
    /// <summary>
    /// Interaction logic for InstantCodeToolWindowControl.
    /// </summary>
    public partial class InstantCodeToolWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstantCodeToolWindowControl"/> class.
        /// </summary>
        public InstantCodeToolWindowControl()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var d = new AddServerDialog();
            d.ShowModal();
            var icClient = new IcClient();
            icClient.Connect(d.ServerIp, 0xC0DE, d.ServerPassword);
            icClient.SendPacket(new P00Login() { Username = "test" });
        }
    }
}