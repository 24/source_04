using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pb.Text;

namespace pb
{
    public static partial class GlobalExtension
    {
        public static T zFunc<T>(this T value, Func<T, T> func)
        {
            return func(value);
        }

        public static T zNotNullFunc<T>(this T value, Func<T, T> func) where T : class
        {
            if (value != null)
                return func(value);
            else
                return null;
        }

        public static IEnumerable<T> zAction<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
                yield return item;
            }
        }
    }

    public static partial class GlobalExtension
    {
        public static string zToStringOrNull<T>(this T v)
        {
            if (v == null)
                return "-null-";
            else
                return v.ToString();
        }

        public static string zToStringValues<T>(this IEnumerable<T> list)
        {
            if (list == null) return null;
            StringBuilder sb = new StringBuilder();
            list.zForEach((T value) => { sb.zAddValue(value); });
            return sb.ToString();
        }

        public static string zToStringValues<T>(this IEnumerable<T> list, string separator)
        {
            if (list == null) return null;
            StringBuilder sb = new StringBuilder();
            list.zForEach((T value) => { sb.zAddValue(value, separator); });
            return sb.ToString();
        }

        public static string zToStringValues<T>(this IEnumerable<T> list, Func<T, string> toString)
        {
            if (list == null || toString == null) return null;
            StringBuilder sb = new StringBuilder();
            list.zForEach((T v) => { sb.zAddValue(toString(v)); });
            return sb.ToString();
        }

        public static string zToStringValues<T>(this IEnumerable<T> list, Func<T, string> toString, string separator)
        {
            if (list == null || toString == null) return null;
            StringBuilder sb = new StringBuilder();
            list.zForEach((T v) => { sb.zAddValue(toString(v), separator); });
            return sb.ToString();
        }

        public static string zReadLine(this BinaryReader br)
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                char c;
                try
                {
                    c = br.ReadChar();
                }
                catch (Exception ex)
                {
                    if (!(ex is EndOfStreamException))
                        throw;
                    if (sb.Length == 0)
                        return null;
                    break;
                }
                if (c == '\n')
                    break;
                else if (c != '\r')
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public static string zzToString(this byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            bool cr = false;
            foreach (byte b in bytes)
            {
                if (b == 10 && !cr)
                    sb.Append('\r');
                if (b == 13)
                    cr = true;
                else
                    cr = false;
                if (b < 32 && b != 10 && b != 13)
                    sb.Append("\\x" + b.zToHex());
                else
                    sb.Append((char)b);
            }
            return sb.ToString();
        }

        public static void zForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (list == null || action == null) return;
            foreach (T v in list) action(v);
        }

        public static void zForEach<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            if (list == null || action == null) return;
            int i = 0;
            foreach (T v in list) action(v, i++);
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            // from Recursive List Flattening http://stackoverflow.com/questions/141467/recursive-list-flattening
            foreach (IEnumerable<T> sequence in sequences)
            {
                foreach (T element in sequence)
                {
                    yield return element;
                }
            }
        }
    }
}
