using System;

namespace pb
{
    public class NamedValue
    {
        public string Name;
        public Type Type;
        public object Value;

        public NamedValue()
        {
        }

        public NamedValue(string name, object value)
        {
            Name = name;
            Value = value;
            Type = null;
            if (value != null)
                Type = value.GetType();
        }

        public NamedValue(string name, Type type)
        {
            Name = name;
            Value = null;
            Type = type;
        }
    }
}
