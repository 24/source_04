using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pb.old
{
    public static partial class GlobalExtension
    {
        #region ZipFile ...
        public static int zNbFile(this ZipFile zip)
        {
            return (from f in zip.Entries where !f.IsDirectory select f).Count();
        }

        public static int zNbDirectory(this ZipFile zip)
        {
            return (from f in zip.Entries where f.IsDirectory select f).Count();
        }

        public static long zCompressedSize(this ZipFile zip)
        {
            return (from f in zip.Entries select f.CompressedSize).Sum();
        }

        public static long zUncompressedSize(this ZipFile zip)
        {
            return (from f in zip.Entries select f.UncompressedSize).Sum();
        }

        public static double zCompressionRatio(this ZipFile zip)
        {
            return (double)zip.zUncompressedSize() / (double)zip.zCompressedSize();
        }
        #endregion

        #region zWriteToFile ...
        public static void zWriteToFile<T>(this IEnumerable<T> list, string sPath)
        {
            list.zWriteToFile(sPath, false);
        }

        public static void zWriteToFile<T>(this IEnumerable<T> list, string sPath, bool bAppend)
        {
            if (list == null) return;
            string sDir = cu.PathGetDir(sPath);
            if (!Directory.Exists(sDir)) Directory.CreateDirectory(sDir);
            FileMode fm = FileMode.Create;
            if (bAppend) fm = FileMode.Append;
            FileStream fs = new FileStream(sPath, fm, FileAccess.Write, FileShare.Read);
            StreamWriter ws = new StreamWriter(fs, Encoding.Default);
            try
            {
                foreach (T v in list)
                {
                    if (v != null)
                    {
                        //ws.WriteLine(v.ToString());
                        TypeView view = new TypeView(v);
                        bool bFirst = true;
                        foreach (NamedValue value in view.Values)
                        {
                            if (!bFirst) ws.Write("\t");
                            bFirst = false;
                            if (value.Value != null) ws.Write(value.Value.ToString());
                        }
                        ws.WriteLine();

                    }
                    else
                        ws.WriteLine();
                }
            }
            finally
            {
                ws.Close();
            }
        }

        public static void zWriteToFile<T>(this T v, string sPath)
        {
            v.zWriteToFile(sPath, true);
        }

        public static void zWriteToFile<T>(this T v, string sPath, bool bAppend)
        {
            if (v == null) return;
            string sDir = cu.PathGetDir(sPath);
            if (!Directory.Exists(sDir)) Directory.CreateDirectory(sDir);
            FileMode fm = FileMode.Create;
            if (bAppend) fm = FileMode.Append;
            FileStream fs = new FileStream(sPath, fm, FileAccess.Write, FileShare.Read);
            StreamWriter ws = new StreamWriter(fs, Encoding.Default);
            try
            {
                TypeView view = new TypeView(v);
                bool bFirst = true;
                foreach (NamedValue value in view.Values)
                {
                    if (!bFirst) ws.Write("\t");
                    bFirst = false;
                    if (value.Value != null) ws.Write(value.Value.ToString());
                }
                ws.WriteLine();
            }
            finally
            {
                ws.Close();
            }
        }
        #endregion
    }
}
