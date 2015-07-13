using System;
using System.Collections.Generic;
using System.Text;

namespace pb.Data
{
    public abstract class ZValue
    {
        public static implicit operator ZValue(string value)
        {
            return (value != null) ? (ZValue)new ZString(value) : null;
        }

        public static implicit operator ZValue(int value)
        {
            return new ZInt(value);
        }

        public static explicit operator string(ZValue v)
        {
            if (v != null)
                return ((ZString)v).value;
            else
                return null;
        }

        public static explicit operator string[](ZValue v)
        {
            if (v != null)
                return ((ZStringArray)v).values;
            else
                return new string[0];
        }

        public static explicit operator int(ZValue v)
        {
            if (v != null)
                return ((ZInt)v).value;
            else
                return 0;
        }
    }

    public class ZString : ZValue
    {
        public string value;

        public ZString(string value)
        {
            this.value = value;
        }

        public static explicit operator string(ZString v)
        {
            if (v != null)
                return v.value;
            else
                return null;
        }

        public override string ToString()
        {
            return value;
        }
    }

    //public enum ZStringArraySelectValueType
    //{
    //    FirstValue = 1,
    //    LastValue
    //}

    public class ZStringArray : ZValue
    {
        public string[] values;

        public ZStringArray(string[] values)
        {
            this.values = values;
        }

        public static explicit operator string[](ZStringArray v)
        {
            if (v != null)
                return v.values;
            else
                return new string[0];
        }

        //public ZString SelectArrayValue(ZStringArraySelectValueType arrayValueType = ZStringArraySelectValueType.FirstValue)
        //{
        //    if (arrayValueType == ZStringArraySelectValueType.FirstValue)
        //    {
        //        foreach (string value in values)
        //        {
        //            if (value != null && value != "")
        //                return new ZString(value);
        //        }
        //    }
        //    else // ArrayValueType.LastValue
        //    {
        //        for (int i = values.Length - 1; i >= 0; i--)
        //        {
        //            string value = values[i];
        //            if (value != null && value != "")
        //                return new ZString(value);
        //        }
        //    }
        //    return null;
        //}

        public override string ToString()
        {
            return values.zToStringValues(s => "\"" + s + "\"");
        }
    }

    public class ZInt : ZValue
    {
        public int value;

        public ZInt(int value)
        {
            this.value = value;
        }

        public static explicit operator int(ZInt v)
        {
            if (v != null)
                return v.value;
            else
                return 0;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

}
