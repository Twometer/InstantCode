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
using InstantCode.Client.Network;

namespace InstantCode.Client.GUI.Pages
{
    /// <summary>
    /// Interaction logic for SessionPage.xaml
    /// </summary>
    public partial class SessionPage : UserControl
    {
        public SessionPage()
        {
            InitializeComponent();
            Header.Content = $"Connected to session '{InstantCodeClient.Instance.CurrentSession.Name}'";

            SessionParticipantsBox.Items.Add(InstantCodeClient.Instance.CurrentUsername + " (Me)");
            foreach (var part in InstantCodeClient.Instance.CurrentSession.Participants)
                if (part != InstantCodeClient.Instance.CurrentUsername)
                    SessionParticipantsBox.Items.Add(part);
        }
    }
}
