using System;

namespace pb
{
    public class NamedValue
    {
        public string Name;
        public Type type;
        public object Value;

        public NamedValue()
        {
        }

        public NamedValue(string name, object value)
        {
            Name = name;
            Value = value;
            if (value != null) type = value.GetType();
        }

        public NamedValue(string name, Type type)
        {
            Name = name;
            Value = null;
            this.type = type;
        }
    }
}
