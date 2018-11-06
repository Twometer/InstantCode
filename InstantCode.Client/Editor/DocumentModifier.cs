using EnvDTE;
using InstantCode.Client.Model;
using InstantCode.Client.Utils;
using Microsoft.VisualStudio.Shell;

namespace InstantCode.Client.Editor
{
    public class DocumentModifier
    {
        public static void Apply(DocumentModification modification)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte = (DTE)Package.GetGlobalService(typeof(DTE));
            var solution = dte.Solution;
            var currentDocumentPath = dte.ActiveDocument.ProjectItem.GetRelativePath(dte.Solution);
            for (var i = 1; i <= solution.Projects.Count; i++)
            {
                var project = solution.Projects.Item(i);
                for (var j = 1; j <= project.ProjectItems.Count; j++)
                {
                    var projectItem = project.ProjectItems.Item(j);
                    var path = projectItem.GetRelativePath(solution);
                    if (path != modification.File) continue;

                    if (!projectItem.IsOpen)
                        projectItem.Open();

                    var isCurrentDocument = path == currentDocumentPath;
                    if (isCurrentDocument)
                        CursorTextAdornmentTextViewCreationListener.IgnoreChanges = true;

                    // TODO: The listener does not ignore changes made by DocumentModifier
                    // TODO: Changing the document using TextSelection is bad because it's slow and it changes the user's cursor position

                    var doc = projectItem.Document;
                    var sel = doc.Selection as TextSelection;

                    sel.MoveToAbsoluteOffset(modification.StartIndex);
                    if (modification.EndIndex != modification.StartIndex)
                        sel.MoveToAbsoluteOffset(modification.EndIndex, true);
                    sel.Insert("", (int)vsInsertFlags.vsInsertFlagsContainNewText);
                    sel.Insert(modification.Data, (int)vsInsertFlags.vsInsertFlagsInsertAtStart);

                    if (isCurrentDocument)
                        CursorTextAdornmentTextViewCreationListener.IgnoreChanges = false;
                }
            }
        }
    }
}
