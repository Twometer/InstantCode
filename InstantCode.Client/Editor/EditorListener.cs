using System.Collections.Generic;
using System.Windows;
using EnvDTE;
using InstantCode.Client.Network;
using InstantCode.Protocol.Packets;
using Microsoft.VisualStudio.Shell;

namespace InstantCode.Client.Editor
{
    public class EditorListener
    {
        private DTE dte;
        private DocumentEvents documentEvents;

        public EditorListener(DTE dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.dte = dte;
            this.documentEvents = dte.Events.DocumentEvents;
        }

        public void Register()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            documentEvents.DocumentSaved += document =>
            {
                InstantCodeClient.Instance.SendPacket(new P09Save(InstantCodeClient.Instance.CurrentSession.Id));
            };
        }

    }
}
