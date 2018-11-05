using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace InstantCode.Client.Utils
{
    public static class Extensions
    {
        public static Task WaitOneAsync(this WaitHandle waitHandle)
        {
            if (waitHandle == null)
                throw new ArgumentNullException("waitHandle");

            var tcs = new TaskCompletionSource<bool>();
            var rwh = ThreadPool.RegisterWaitForSingleObject(waitHandle,
                delegate { tcs.TrySetResult(true); }, null, -1, true);
            var t = tcs.Task;
            t.ContinueWith((antecedent) => rwh.Unregister(null));
            return t;
        }

        public static string GetRelativePath(this ProjectItem projectItem, Solution owner)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var directoryInfo = new FileInfo(owner.FileName).Directory;
            return directoryInfo != null ? projectItem.FileNames[0].Substring(directoryInfo.FullName.Length) : null;
        }
    }
}
