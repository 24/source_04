using System;

namespace pb.Data
{
    public abstract class ZValue
    {
        public static implicit operator ZValue(string value)
        {
            return (value != null) ? (ZValue)new ZString(value) : null;
        }

        public static implicit operator ZValue(bool value)
        {
            return new ZBool(value);
        }

        public static implicit operator ZValue(int value)
        {
            return new ZInt(value);
        }

        public static implicit operator ZValue(double value)
        {
            return new ZDouble(value);
        }

        public static implicit operator ZValue(Date value)
        {
            return new ZDate(value);
        }

        public static implicit operator ZValue(DateTime value)
        {
            return new ZDateTime(value);
        }

        public static implicit operator ZValue(TimeSpan value)
        {
            return new ZTimeSpan(value);
        }

        public static explicit operator string(ZValue v)
        {
            if (v != null)
                return ((ZString)v).Value;
            else
                return null;
        }

        public static explicit operator string[](ZValue v)
        {
            if (v != null)
                return ((ZStringArray)v).Values;
            else
                return new string[0];
        }

        public static explicit operator bool(ZValue v)
        {
            if (v != null)
                return ((ZBool)v).Value;
            else
                return false;
        }

        public static explicit operator int(ZValue v)
        {
            if (v != null)
                return ((ZInt)v).Value;
            else
                return 0;
        }

        public static explicit operator double(ZValue v)
        {
            if (v != null)
                return ((ZDouble)v).Value;
            else
                return 0;
        }

        public static explicit operator Date(ZValue v)
        {
            if (v != null)
                return ((ZDate)v).Value;
            else
                return Date.MinValue;
        }

        public static explicit operator DateTime(ZValue v)
        {
            if (v != null)
                return ((ZDateTime)v).Value;
            else
                return DateTime.MinValue;
        }

        public static explicit operator TimeSpan(ZValue v)
        {
            if (v != null)
                return ((ZTimeSpan)v).Value;
            else
                return TimeSpan.Zero;
        }

        public abstract object ToObject();
    }

    public class ZString : ZValue
    {
        public string Value;

        public ZString(string value)
        {
            Value = value;
        }

        public static explicit operator string(ZString v)
        {
            if (v != null)
                return v.Value;
            else
                return null;
        }

        public override string ToString()
        {
            return Value;
        }

        public override object ToObject()
        {
            return Value;
        }
    }

    //public enum ZStringArraySelectValueType
    //{
    //    FirstValue = 1,
    //    LastValue
    //}

    public class ZStringArray : ZValue
    {
        public string[] Values;

        public ZStringArray(string[] values)
        {
            Values = values;
        }

        public static explicit operator string[](ZStringArray v)
        {
            if (v != null)
                return v.Values;
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
            return Values.zToStringValues(s => "\"" + s + "\"");
        }

        public override object ToObject()
        {
            return Values;
        }
    }

    public class ZBool : ZValue
    {
        public bool Value;

        public ZBool(bool value)
        {
            Value = value;
        }

        public static explicit operator bool(ZBool v)
        {
            if (v != null)
                return v.Value;
            else
                return false;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override object ToObject()
        {
            return Value;
        }
    }

    public class ZInt : ZValue
    {
        public int Value;

        public ZInt(int value)
        {
            Value = value;
        }

        public static explicit operator int(ZInt v)
        {
            if (v != null)
                return v.Value;
            else
                return 0;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override object ToObject()
        {
            return Value;
        }
    }

    public class ZDouble : ZValue
    {
        public double Value;

        public ZDouble(double value)
        {
            Value = value;
        }

        public static explicit operator double(ZDouble v)
        {
            if (v != null)
                return v.Value;
            else
                return 0;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override object ToObject()
        {
            return Value;
        }
    }

    public class ZDate : ZValue
    {
        public Date Value;

        public ZDate(Date value)
        {
            Value = value;
        }

        public static explicit operator Date(ZDate v)
        {
            if (v != null)
                return v.Value;
            else
                return Date.MinValue;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override object ToObject()
        {
            return Value;
        }
    }

    public class ZDateTime : ZValue
    {
        public DateTime Value;

        public ZDateTime(DateTime value)
        {
            Value = value;
        }

        public static explicit operator DateTime(ZDateTime v)
        {
            if (v != null)
                return v.Value;
            else
                return DateTime.MinValue;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override object ToObject()
        {
            return Value;
        }
    }

    public class ZTimeSpan : ZValue
    {
        public TimeSpan Value;

        public ZTimeSpan(TimeSpan value)
        {
            Value = value;
        }

        public static explicit operator TimeSpan(ZTimeSpan v)
        {
            if (v != null)
                return v.Value;
            else
                return TimeSpan.Zero;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override object ToObject()
        {
            return Value;
        }
    }
}
