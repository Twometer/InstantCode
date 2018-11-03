using System;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI;

namespace InstantCode.Client.GUI
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : DialogWindow
    {
        private readonly Action cancelHandler;

        public string StatusMessage { set => ProgressLabel.Content = value; }

        public int Value { set => ProgressBar.Value = value; }

       
        public ProgressDialog(string statusMessage, Action cancelHandler, bool intermediate = false)
        {
            InitializeComponent();
            StatusMessage = statusMessage;
            ProgressBar.IsIndeterminate = intermediate;
            this.cancelHandler = cancelHandler;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancelHandler();
            Close();
        }
    }
}
