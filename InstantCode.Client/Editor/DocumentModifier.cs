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

                    var doc = projectItem.Document;

                    /* TODO: Implement modifications
                     * var sel = doc.Selection as TextSelection;
                     * sel.SelectAll();
                     * sel.Insert("", (int)vsInsertFlags.vsInsertFlagsContainNewText);
                     */
                }
            }
        }
    }
}
