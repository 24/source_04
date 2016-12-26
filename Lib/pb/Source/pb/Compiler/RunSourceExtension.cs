using System.Data;
using pb.Data;
using pb.Data.Mongo;
using pb.Reflection;
using System.Collections;
using System.IO;
using pb.IO;

namespace pb.Compiler
{
    public static class CompilerGlobalExtension
    {
        public static void zTraceToFile<T>(this T value, string file)
        {
            zfile.CreateFileDirectory(file);
            using (StreamWriter sw = zFile.CreateText(file))
            {
                if (value is IEnumerable)
                {
                    foreach (object o in value as IEnumerable)
                        sw.WriteLine(o.ToString());
                }
                else if (value != null)
                    sw.WriteLine(value.ToString());
                else
                    sw.WriteLine("value is null");
            }
        }

        public static void zTrace<T>(this T value)
        {
            if (value is IEnumerable && !(value is string))
            {
                foreach (object o in value as IEnumerable)
                    Trace.WriteLine(o.ToString());
            }
            else if (value != null)
                Trace.WriteLine(value.ToString());
            else
                Trace.WriteLine("value is null");
        }

        //public static void zView<T>(this T value)
        //{
        //    if (value != null)
        //        RunSource.CurrentRunSource.View(value);
        //    else
        //        Trace.WriteLine("value is null");
        //}

        public static DataTable zView<T>(this T value)
        {
            DataTable table = null;
            if (value != null)
            {
                if (!(value is DataTable))
                    table = value.zToDataTable();
                else
                    table = value as DataTable;
                RunSource.CurrentRunSource.SetResult(table);
            }
            else
                Trace.WriteLine("value is null");
            return table;
        }

        public static DataTable zView_v2<T>(this T value)
        {
            DataTable table = null;
            if (value != null)
            {
                if (!(value is DataTable))
                    table = value.zToDataTable_v2();
                else
                    table = value as DataTable;
                RunSource.CurrentRunSource.SetResult(table);
            }
            else
                Trace.WriteLine("value is null");
            return table;
        }

        public static DataTable zView_v3<T>(this T value)
        {
            DataTable table = null;
            if (value != null)
            {
                if (!(value is DataTable))
                    table = value.zToDataTable_v3();
                else
                    table = value as DataTable;
                RunSource.CurrentRunSource.SetResult(table);
            }
            else
                Trace.WriteLine("value is null");
            return table;
        }

        public static DataTable zViewType<T>(this T value)
        {
            DataTable table = null;
            if (value != null)
            {
                //table = value.zTypeToDataTable();
                //RunSource.CurrentRunSource.SetResult(table);
            }
            else
                Trace.WriteLine("value is null");
            return table;
        }

        public static DataTable zSetResult(this DataTable table)
        {
            if (table != null)
                RunSource.CurrentRunSource.SetResult(table);
            else
                Trace.WriteLine("value is null");
            return table;
        }

        //public static void ExportResult(string file, bool fieldNameHeader = false, bool append = false)
        //{
        //    //if (_trace.TraceLevel >= 1)
        //    Trace.WriteLine("Export(\"{0}\");", file);
        //    zdt_v1.Save(RunSource.CurrentRunSource.Result.DefaultView, file, append, fieldNameHeader, null, System.Text.Encoding.UTF8);
        //}

        public static string zGetRunSourceProjectVariableValue(this string value, bool throwError = false)
        {
            return RunSource.CurrentRunSource.GetProjectVariableValue(value, throwError);
        }
    }
}
