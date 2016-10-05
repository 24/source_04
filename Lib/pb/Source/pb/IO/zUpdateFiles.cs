using System;

namespace pb.IO
{
    public static class zUpdateFiles
    {
        private static bool __trace = false;

        public static bool Trace { get { return __trace; } set { __trace = value; } }

        public static bool TryUpdateFile(string file, string newDirectory, out string error)
        {
            error = null;
            if (!zPath.IsPathRooted(newDirectory))
                newDirectory = zPath.Combine(zPath.GetDirectoryName(file), newDirectory);
            string newFile = zPath.Combine(newDirectory, zPath.GetFileName(file));
            try
            {
                return zfile.CopyFile(newFile, file, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
            }
            catch (Exception exception)
            {
                //pb.Trace.WriteLine("error copying file \"{0}\" to \"{1}\"", newFile, file);
                //pb.Trace.WriteLine(exception.Message);
                error = exception.Message;
                return false;
            }
        }

        // used in runsource.launch.cs
        public static void UpdateFiles(string sourceDirectory, string destinationDirectory)
        {
            if (sourceDirectory == null)
                return;
            if (__trace)
                pb.Trace.WriteLine("update files from \"{0}\" to \"{1}\"", sourceDirectory, destinationDirectory);
            bool update = true;
            if (!zDirectory.Exists(sourceDirectory))
            {
                if (__trace)
                    pb.Trace.WriteLine("  source directory not found \"{0}\"", sourceDirectory);
                update = false;
            }
            if (!zDirectory.Exists(destinationDirectory))
            {
                if (__trace)
                    pb.Trace.WriteLine("  destination directory not found \"{0}\"", destinationDirectory);
                update = false;
            }
            if (!update)
                return;

            foreach (string file in zDirectory.EnumerateFiles(sourceDirectory))
            {
                if (__trace)
                    pb.Trace.WriteLine("  copy file \"{0}\" to directory \"{1}\"", file, destinationDirectory);
                try
                {
                    zfile.CopyFileToDirectory(file, destinationDirectory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
                }
                catch (Exception exception)
                {
                    pb.Trace.WriteLine("error copying file \"{0}\" to directory \"{1}\"", file, destinationDirectory);
                    pb.Trace.WriteLine(exception.Message);
                }
            }
        }
    }
}
