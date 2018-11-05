using System.ComponentModel.Composition;
using System.Windows;
using EnvDTE;
using InstantCode.Client.Network;
using InstantCode.Client.Utils;
using InstantCode.Protocol.Packets;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace InstantCode.Client.Editor
{
    /// <summary>
    /// Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    /// that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class CursorTextAdornmentTextViewCreationListener : IWpfTextViewCreationListener
    {
        // Disable "Field is never assigned to..." and "Field is never used" compiler's warnings. Justification: the field is used by MEF.
#pragma warning disable 649, 169

        /// <summary>
        /// Defines the adornment layer for the adornment. This layer is ordered
        /// after the selection layer in the Z-order
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("CursorTextAdornment")]
        [Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
        private AdornmentLayerDefinition editorAdornmentLayer;

#pragma warning restore 649, 169

        private DTE dte;

        #region IWpfTextViewCreationListener

        /// <summary>
        /// Called when a text view having matching roles is created over a text data model having a matching content type.
        /// Instantiates a CursorTextAdornment manager when the textView is created.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment should be placed</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            // The adornment will listen to any event that changes the layout (text changes, scrolling, etc)
            dte = (DTE)Package.GetGlobalService(typeof(DTE));
            textView.TextBuffer.Changed += (sender, args) =>
            {
                if (args.Changes.Count == 0) return;
                var change = args.Changes[0];
                InstantCodeClient.Instance.SendPacket(new P07CodeChange(InstantCodeClient.Instance.CurrentSession.Id,
                    InstantCodeClient.Instance.CurrentUsername,
                    dte.ActiveDocument.ProjectItem.GetRelativePath(dte.Solution), change.OldSpan.Start,
                    change.OldSpan.End, change.NewText));
                //MessageBox.Show(change.OldSpan.Start + "; " + change.OldSpan.End + "; " + change.NewText);
            };
            new CursorTextAdornment(textView);
        }

        #endregion
    }
}
