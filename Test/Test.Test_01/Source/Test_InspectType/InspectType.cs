using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace pb
{
    public enum InspectValueType
    {
        Unknow = 0,
        Value = 1,
        NewValue,
        ClassBegin,
        ClassEnd,
        EnumerableBegin,
        EnumerableEnd
    }

    public enum InspectValueState
    {
        None = 0,
        Value,
        EnumerableBegin,
        Enumerable,
        EnumerableEnd,
        Properties,
        Fields,
    }

    public class InspectValueObject
    {
        public string name = null;
        public object value = null;
        public InspectValueState state = InspectValueState.None;
        public int enumeratorIndex = 0;
        public IEnumerator enumerator = null;
        public bool onlyVisible = false;
        public Type type = null;
        public PropertyInfo[] properties = null;
        public IEnumerator<PropertyInfo> propertyEnumerator = null;
        public FieldInfo[] fields = null;
        public IEnumerator<FieldInfo> fieldEnumerator = null;
        private bool _isValue = false;

        public InspectValueObject(string name, object value, InspectValueState state = InspectValueState.None)
        {
            this.name = name;
            this.value = value;
            this.state = state;
            Init();
        }

        private void Init()
        {
            if (value != null)
            {
                type = value.GetType();
                properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                propertyEnumerator = properties.AsEnumerable().GetEnumerator();
                propertyEnumerator.Reset();
                fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                fieldEnumerator = fields.AsEnumerable().GetEnumerator();
                fieldEnumerator.Reset();
                if ((type.IsValueType && properties.Length == 0 && fields.Length == 0) || type == typeof(string) || type == typeof(DateTime) || type == typeof(Date))
                {
                    _isValue = true;
                }
            }
        }

        public bool IsValue
        {
            get { return _isValue; }
        }

        public InspectValue Next()
        {
            if (state == InspectValueState.None)
            {
                if (value == null)
                {
                    state = InspectValueState.Value;
                    return new InspectValue(name, InspectValueType.Value);
                }
                //if (_depth++ > _maxDepth)
                //    return false;
                if (_isValue)
                {
                    state = InspectValueState.Value;
                    return new InspectValue(name, InspectValueType.Value, value, type);
                }
                else if (value is IEnumerable)
                {
                    onlyVisible = true;
                }
                if (name == "") name = "value"; else name = name + "_value";
                state = InspectValueState.Properties;
            }
            if (state == InspectValueState.Properties)
            {
                while (propertyEnumerator.MoveNext())
                {
                    PropertyInfo property = propertyEnumerator.Current;
                    if (onlyVisible && Attribute.GetCustomAttribute(property, typeof(pb.VisibleAttribute)) == null)
                        continue;
                    object o = null;
                    try
                    {
                        o = property.GetGetMethod().Invoke(value, new object[0]);
                    }
                    catch (Exception ex)
                    {
                        o = Error.GetErrorMessage(ex);
                    }
                    string name2;
                    if (name == "") name2 = property.Name; else name2 = name + "_" + property.Name;
                    return new InspectValue(name2, InspectValueType.NewValue, o);
                }
                state = InspectValueState.Fields;
            }
            if (state == InspectValueState.Fields)
            {
                while (fieldEnumerator.MoveNext())
                {
                    FieldInfo field = fieldEnumerator.Current;
                    if (onlyVisible && Attribute.GetCustomAttribute(field, typeof(pb.VisibleAttribute)) == null)
                        continue;
                    object o = null;
                    try
                    {
                        o = field.GetValue(value);
                    }
                    catch (Exception ex)
                    {
                        o = Error.GetErrorMessage(ex);
                    }
                    string name2;
                    if (name == "") name2 = field.Name; else name2 = name + "_" + field.Name;
                    return new InspectValue(name2, InspectValueType.NewValue, o);
                }
                if (value is IEnumerable)
                    state = InspectValueState.EnumerableBegin;
            }
            if (state == InspectValueState.EnumerableBegin)
            {
                state = InspectValueState.Enumerable;
                enumerator = ((IEnumerable)value).GetEnumerator();
                enumeratorIndex = 1;
                return new InspectValue(name, InspectValueType.EnumerableBegin);
            }
            if (state == InspectValueState.Enumerable)
            {
                if (enumerator.MoveNext())
                {
                    return new InspectValue(name + enumeratorIndex++.ToString(), InspectValueType.NewValue, enumerator.Current);
                }
                state = InspectValueState.EnumerableEnd;
                return new InspectValue(name, InspectValueType.EnumerableEnd);
            }
            return null;
        }

        //public InspectValue GetInspectValue()
        //{
        //    return new InspectValue(name, InspectValueType.Value, value, type);
        //}
    }

    public class InspectValue
    {
        public string name = null;
        public object value = null;
        public Type type = null;
        public InspectValueType valueType = InspectValueType.Unknow;
        //public bool isValue = true;

        public InspectValue(string name = null, InspectValueType valueType = InspectValueType.Unknow, object value = null, Type type = null)
        {
            this.name = name;
            this.valueType = valueType;
            this.value = value;
            this.type = type;
            //this.isValue = isValue;
        }
    }

    class InspectType : IEnumerable<InspectValue>, IEnumerator<InspectValue>, System.Collections.IEnumerator
    {
        private string _name = null;
        private object _value = null;
        private int _depth = 0;
        private const int _maxDepth = 10;
        private InspectValueObject _currentValueObject = null;
        private InspectValue _currentValue = null;
        private Stack<InspectValueObject> _stack = new Stack<InspectValueObject>();

        public InspectType(object value, string name = "")
        {
            _name = name;
            _value = value;
            Init();
        }

        public void Init()
        {
            _depth = 0;
            //_currentValueObject = new InspectValueObject();
            //_currentValueObject.name = _name;
            //_currentValueObject.value = _value;
            //_currentValueObject.state = InspectValueState.None;
            _currentValueObject = new InspectValueObject(_name, _value, InspectValueState.None);
        }

        private bool Next()
        {
            while (true)
            {
                _currentValue = _currentValueObject.Next();
                if (_currentValue == null)
                {
                    if (_stack.Count == 0)
                        return false;
                    _currentValueObject = _stack.Pop();
                }
                else
                {
                    if (_currentValue.valueType == InspectValueType.NewValue)
                    {
                        _stack.Push(_currentValueObject);
                        _currentValueObject = new InspectValueObject(_currentValue.name, _currentValue.value, InspectValueState.None);
                    }
                    else
                        return true;
                }
            }
        }

        private bool Next0()
        {
        again:
            if (_currentValueObject.state == InspectValueState.None)
            {
                if (_currentValueObject.value == null)
                {
                    _currentValueObject.state = InspectValueState.Value;
                    //_currentValue = new InspectValue();
                    //_currentValue.name = _currentValueObject.name;
                    //_currentValue.value = null;
                    //_currentValue.type = null;
                    //_currentValue.valueType = InspectValueType.Value;
                    _currentValue = new InspectValue(_currentValueObject.name, InspectValueType.Value);
                    return true;
                }
                if (_depth++ > _maxDepth)
                    return false;
                _currentValueObject.type = _currentValueObject.value.GetType();
                _currentValueObject.properties = _currentValueObject.type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                _currentValueObject.propertyEnumerator = _currentValueObject.properties.AsEnumerable().GetEnumerator();
                _currentValueObject.propertyEnumerator.Reset();
                _currentValueObject.fields = _currentValueObject.type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                _currentValueObject.fieldEnumerator = _currentValueObject.fields.AsEnumerable().GetEnumerator();
                _currentValueObject.fieldEnumerator.Reset();
                if ((_currentValueObject.type.IsValueType && _currentValueObject.properties.Length == 0 && _currentValueObject.fields.Length == 0) || _currentValueObject.type == typeof(string)
                    || _currentValueObject.type == typeof(DateTime) || _currentValueObject.type == typeof(Date))
                {
                    _currentValueObject.state = InspectValueState.Value;
                    _currentValue = new InspectValue(_currentValueObject.name, InspectValueType.Value, _currentValueObject.value, _currentValueObject.type);
                    return true;
                }
                else if (_currentValueObject.value is IEnumerable)
                {
                    _currentValueObject.onlyVisible = true;
                }
                string name;
                if (_currentValueObject.name == "") name = "value"; else name = _currentValueObject.name + "_value";
                _currentValueObject.state = InspectValueState.Properties;
            }
            if (_currentValueObject.state == InspectValueState.Properties)
            {
                while (_currentValueObject.propertyEnumerator.MoveNext())
                {
                    PropertyInfo property = _currentValueObject.propertyEnumerator.Current;
                    if (_currentValueObject.onlyVisible && Attribute.GetCustomAttribute(property, typeof(pb.VisibleAttribute)) == null)
                        continue;
                    object o = null;
                    try
                    {
                        o = property.GetGetMethod().Invoke(_currentValueObject.value, new object[0]);
                    }
                    catch (Exception ex)
                    {
                        o = Error.GetErrorMessage(ex);
                    }
                    string name;
                    if (_currentValueObject.name == "") name = property.Name; else name = _currentValueObject.name + "_" + property.Name;
                    BeginValue(name, o);
                    goto again;
                }
                _currentValueObject.state = InspectValueState.Fields;
            }
            if (_currentValueObject.state == InspectValueState.Fields)
            {
                while (_currentValueObject.fieldEnumerator.MoveNext())
                {
                    FieldInfo field = _currentValueObject.fieldEnumerator.Current;
                    if (_currentValueObject.onlyVisible && Attribute.GetCustomAttribute(field, typeof(pb.VisibleAttribute)) == null)
                        continue;
                    object o = null;
                    try
                    {
                        o = field.GetValue(_currentValueObject.value);
                    }
                    catch (Exception ex)
                    {
                        o = Error.GetErrorMessage(ex);
                    }
                    string name;
                    if (_currentValueObject.name == "") name = field.Name; else name = _currentValueObject.name + "_" + field.Name;
                    BeginValue(name, o);
                    goto again;
                }
                if (_currentValueObject.value is IEnumerable)
                    _currentValueObject.state = InspectValueState.EnumerableBegin;
            }
            if (_currentValueObject.state == InspectValueState.EnumerableBegin)
            {
                _currentValueObject.state = InspectValueState.Enumerable;
                _currentValueObject.enumerator = ((IEnumerable)_currentValueObject.value).GetEnumerator();
                _currentValueObject.enumeratorIndex = 1;
                _currentValue = new InspectValue(_currentValueObject.name, InspectValueType.EnumerableBegin);
                return true;
            }
            if (_currentValueObject.state == InspectValueState.Enumerable)
            {
                if (_currentValueObject.enumerator.MoveNext())
                {
                    string name = _currentValueObject.name + _currentValueObject.enumeratorIndex++.ToString();
                    object o = null;
                    BeginValue(name, o);
                    goto again;
                }
                _currentValueObject.state = InspectValueState.EnumerableEnd;
                _currentValue = new InspectValue(_currentValueObject.name, InspectValueType.EnumerableEnd);
                return true;
            }
            if (EndValue())
                goto again;
            return false;
        }

        private void BeginValue(string name, object value)
        {
            _stack.Push(_currentValueObject);
            //_currentValueObject = new InspectValueObject();
            //_currentValueObject.name = name;
            //_currentValueObject.value = value;
            //_currentValueObject.state = InspectValueState.None;
            _currentValueObject = new InspectValueObject(name, value, InspectValueState.None);
        }

        private bool EndValue()
        {
            if (_stack.Count > 0)
            {
                _currentValueObject = _stack.Pop();
                return true;
            }
            return false;
        }

        public IEnumerator<InspectValue> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public InspectValue Current
        {
            get { return _currentValue; }
            //get { return _currentValueObject.GetInspectValue(); }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _currentValue; }
        }

        public bool MoveNext()
        {
            return Next();
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            return Next();
        }

        public void Reset()
        {
            Init();
        }

        void System.Collections.IEnumerator.Reset()
        {
            Init();
        }
    }
}
