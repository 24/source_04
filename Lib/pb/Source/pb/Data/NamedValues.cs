using System.Collections.Generic;

namespace pb.Data
{
    // NamedValues1 : 
    //   download\test_f.cs
    //   frboard.cs frboard1.cs
    //   PrintManager.cs PrintManager1.cs Print.cs Print1.cs
    //   LaCroix.cs LeFigaro.cs LeMonde.cs LeMonde0.cs LeMonde1.cs LeParisien.cs LeVifExpress.cs LExpress.cs
    //   Date.cs RegexValues.cs

    public class NamedValues<T> : Dictionary<string, T>
    {
        protected string _error = null;
        protected bool _useLowercaseKey = false;

        public NamedValues()
        {
        }

        public NamedValues(bool useLowercaseKey)
        {
            _useLowercaseKey = useLowercaseKey;
        }

        public NamedValues(Dictionary<string, T> dictionary, bool useLowercaseKey = false)
            : base(dictionary)
        {
            _useLowercaseKey = useLowercaseKey;
        }

        public string Error { get { return _error; } }

        public void SetError(string error, params object[] prm)
        {
            if (prm.Length > 0)
                error = string.Format(error, prm);
            _error = error;
        }

        public void SetValues(NamedValues<T> values, params string[] names)
        {
            if (names.Length > 0)
            {
                foreach (string name in names)
                {
                    if (values.ContainsKey(name))
                    {
                        string name2 = name;
                        if (_useLowercaseKey)
                            name2 = name2.ToLowerInvariant();
                        if (this.ContainsKey(name2))
                            this[name2] = values[name];
                        else
                            this.Add(name2, values[name]);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, T> value in values)
                {
                    string key = value.Key;
                    if (_useLowercaseKey)
                        key = key.ToLowerInvariant();
                    if (this.ContainsKey(key))
                        this[key] = value.Value;
                    else
                        this.Add(key, value.Value);
                }
            }
        }

        public void SetValue(string key, T value)
        {
            if (_useLowercaseKey)
                key = key.ToLowerInvariant();
            if (this.ContainsKey(key))
            {
                //Trace.WriteLine("replace \"{0}\" value \"{1}\" by \"{2}\"", key, this[key], value);
                this[key] = value;
            }
            else
                this.Add(key, value);
        }
    }

    //public class NamedValues1 : Dictionary<string, object>
    //{
    //    protected string _error = null;

    //    public NamedValues1()
    //    {
    //    }

    //    public NamedValues1(Dictionary<string, object> dictionary)
    //        : base(dictionary)
    //    {
    //    }

    //    public string Error { get { return _error; } }

    //    public void SetError(string error, params object[] prm)
    //    {
    //        if (prm.Length > 0)
    //            error = string.Format(error, prm);
    //        _error = error;
    //    }

    //    public void SetValues(NamedValues1 values, params string[] names)
    //    {
    //        if (names.Length > 0)
    //        {
    //            foreach (string name in names)
    //            {
    //                if (values.ContainsKey(name))
    //                {
    //                    if (this.ContainsKey(name))
    //                        this[name] = values[name];
    //                    else
    //                        this.Add(name, values[name]);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            foreach (KeyValuePair<string, object> value in values)
    //            {
    //                if (this.ContainsKey(value.Key))
    //                    this[value.Key] = value.Value;
    //                else
    //                    this.Add(value.Key, value.Value);
    //            }
    //        }
    //    }

    //    public void SetValues(string key, object value)
    //    {
    //        if (this.ContainsKey(key))
    //            this[key] = value;
    //        else
    //            this.Add(key, value);
    //    }
    //}
}
