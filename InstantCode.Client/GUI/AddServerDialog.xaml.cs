using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
