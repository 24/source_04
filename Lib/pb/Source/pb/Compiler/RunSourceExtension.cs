using System;
using System.Data;
using pb.Data;
using pb.Data.Xml;
using pb.Reflection;

namespace pb.Compiler
{
    public static partial class GlobalExtension
    {
        public static void zTrace<T>(this T value)
        {
            if (value != null)
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
