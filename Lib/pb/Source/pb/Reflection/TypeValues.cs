using System;
using System.Collections;
using System.Collections.Generic;

namespace pb.Reflection
{
    public class TreeBase
    {
        //T Parent { get; }
        //T Child { get; }
        //T Siblin { get; }
        public TreeBase Parent;
        public TreeBase Child;
        public TreeBase Siblin;

        public void SetParent(TreeBase parent)
        {
            if (Parent != null)
                throw new PBException("TreeBase has already a parent");
            Parent = parent;
            if (parent != null)
            {
                if (parent.Child == null)
                    parent.Child = this;
                else
                {
                    TreeBase siblin = parent.Child;
                    while (siblin.Siblin != null)
                        siblin = siblin.Siblin;
                    siblin.Siblin = this;
                }
            }
        }
    }

    public class MemberValue : TreeBase
    {
        public MemberAccess MemberAccess;
        //public MemberValue Parent;
        //public MemberValue Child;
        //public MemberValue Siblin;
        public bool ValueAvailable;              // Value is set
        public object Value;
        public IEnumerator Enumerator;
        public bool FoundNext;
        public bool ChildFoundNext;
        public MemberValue ParentEnumerate;

        //public void SetParent(MemberValue parent)
        //{
        //    if (Parent != null)
        //        throw new PBException("MemberValue has already a parent");
        //    Parent = parent;
        //    if (parent != null)
        //    {
        //        if (parent.Child == null)
        //            parent.Child = this;
        //        else
        //        {
        //            MemberValue siblin = parent.Child;
        //            while (siblin.Siblin != null)
        //                siblin = siblin.Siblin;
        //            siblin.Siblin = this;
        //        }
        //    }
        //}
    }

    public partial class TypeValues<T>
    {
        private Type _type = null;
        private Dictionary<string, MemberValue> _memberValues = new Dictionary<string, MemberValue>();
        private List<MemberValue> _enumerates = null;
        private T _data;
        private bool _nextValue = false;

        public TypeValues()
        {
            _type = typeof(T);
        }

        public Dictionary<string, MemberValue> MemberValues { get { return _memberValues; } }

        public void AddAllValues(MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property)
        {
            foreach (TreeValue<ValueInfo> treeValueInfo in _type.zGetTypeAllValuesInfos(memberType, options: TypeReflectionOptions.ValueType | TypeReflectionOptions.NotValueType))
            {
                MemberValue memberValue = new MemberValue { MemberAccess = new MemberAccess(treeValueInfo.Value, treeValueInfo.Value.TreeName) };
                MemberValue parent = null;
                if (treeValueInfo.Value.ParentName != null)
                {
                    parent = _memberValues[treeValueInfo.Value.ParentName];
                    memberValue.SetParent(parent);
                }
                _memberValues.Add(treeValueInfo.Value.TreeName, memberValue);
            }
        }

        public void AddValue(string name, MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property)
        {
            string parentName = null;
            MemberValue parent = null;
            Type type = _type;
            foreach (string valueName in name.Split('.'))
            {
                string treeName = valueName;
                if (parentName != null)
                    treeName = parentName + "." + treeName;
                MemberValue memberValue = null;
                if (!_memberValues.ContainsKey(treeName))
                {
                    ValueInfo valueInfo = type.zGetTypeValueInfo(valueName, memberType);
                    if (valueInfo == null)
                        throw new PBException("unknow value \"{0}\" from type {1} memberType {2}", valueName, type.zGetTypeName(), memberType.ToString());
                    memberValue = new MemberValue { MemberAccess = new MemberAccess(valueInfo, treeName) };
                    memberValue.SetParent(parent);
                    _memberValues.Add(treeName, memberValue);
                }
                else
                    memberValue = _memberValues[valueName];
                parentName = treeName;
                parent = memberValue;
                type = memberValue.MemberAccess.ValueType;
            }
        }

        public IEnumerable<TypeValue> GetTypeValues()
        {
            foreach (MemberValue memberValue in _memberValues.Values)
            {
                yield return memberValue.MemberAccess;
            }
        }

        public void SetData(T data)
        {
            _data = data;
            _nextValue = false;
            RazValues();
        }

        public IEnumerable<TypeValue> GetValues(bool onlyNextValue = false)
        {
            foreach (MemberValue memberValue in _memberValues.Values)
            {
                memberValue.MemberAccess.Value = GetValue(memberValue, onlyNextValue);
                yield return memberValue.MemberAccess;
            }
        }

        public bool NextValues()
        {
            RazEnumerates();
            bool found = false;
            foreach (MemberValue memberValue in _enumerates)
            {
                if (!memberValue.ChildFoundNext)
                {
                    IEnumerator enumerator = memberValue.Enumerator;
                    if (enumerator != null)
                    {
                        if (enumerator.MoveNext())
                        {
                            found = true;
                            memberValue.Value = enumerator.Current;
                            memberValue.FoundNext = true;

                            // update childs values
                            UpdateChildsValues(memberValue);

                            MemberValue memberValue2 = memberValue;
                            while (true)
                            {
                                memberValue2 = memberValue2.ParentEnumerate;
                                if (memberValue2 == null)
                                    break;
                                memberValue2.ChildFoundNext = true;
                            }
                        }
                    }
                }
            }
            _nextValue = true;
            return found;
        }

        private void UpdateChildsValues(MemberValue memberValue)
        {
            // memberValue.ValueAvailable
            // memberValue.FoundNext = false;
            // memberValue.ChildFoundNext = false;
            //object data = memberValue.Value;

            MemberValue memberValue2 = memberValue;
            if (memberValue2.Child != null)
            {
                while (true)
                {
                    // loop on childs
                    while (memberValue2.Child != null)
                    {
                        memberValue2 = (MemberValue)memberValue2.Child;
                        //memberValue2.ValueAvailable = false;
                        SetValue(memberValue2);
                        memberValue2.FoundNext = true;
                    }

                    while (true)
                    {
                        if (memberValue2.Siblin != null)
                        {
                            memberValue2 = (MemberValue)memberValue2.Siblin;
                            //memberValue2.ValueAvailable = false;
                            SetValue(memberValue2);
                            memberValue2.FoundNext = true;
                        }
                        else
                        {
                            memberValue2 = (MemberValue)memberValue2.Parent;
                            if (memberValue2 == memberValue)
                                return;
                        }
                    }
                }
            }
        }

        public object GetValue(string name, bool onlyNextValue = false)
        {
            if (!_memberValues.ContainsKey(name))
                throw new PBException("unknow value \"{0}\"", name);
            MemberValue memberValue = _memberValues[name];
            return GetValue(memberValue, onlyNextValue);
        }

        private object GetValue(MemberValue memberValue, bool onlyNextValue)
        {
            if (!_nextValue)
                _GetValue(memberValue);
            if (!_nextValue || !onlyNextValue || memberValue.FoundNext)
                return memberValue.Value;
            else
                return null;
        }

        private void _GetValue(MemberValue memberValue)
        {
            if (!memberValue.ValueAvailable)
            {
                //object data = null;
                if (memberValue.Parent != null)
                {
                    _GetValue((MemberValue)memberValue.Parent);
                    //data = memberValue.Parent.Value;
                }
                //else
                //    data = _data;

                //if (data != null)
                //{
                //    object value = memberValue.MemberAccess.GetValue(data);
                //    if (memberValue.MemberAccess.IsEnumerable)
                //    {
                //        memberValue.Enumerator = ((IEnumerable)value).GetEnumerator();
                //        if (memberValue.Enumerator != null && memberValue.Enumerator.MoveNext())
                //            memberValue.Value = memberValue.Enumerator.Current;
                //    }
                //    else
                //        memberValue.Value = value;
                //}
                //memberValue.ValueAvailable = true;
                SetValue(memberValue);
            }
        }

        private void SetValue(MemberValue memberValue)
        {
            object data;
            if (memberValue.Parent != null)
                data = ((MemberValue)memberValue.Parent).Value;
            else
                data = _data;
            if (data != null)
            {
                object value = memberValue.MemberAccess.GetValue(data);
                if (memberValue.MemberAccess.IsEnumerable)
                {
                    memberValue.Enumerator = ((IEnumerable)value).GetEnumerator();
                    if (memberValue.Enumerator != null && memberValue.Enumerator.MoveNext())
                        memberValue.Value = memberValue.Enumerator.Current;
                }
                else
                    memberValue.Value = value;
            }
            else
                memberValue.Value = null;
            memberValue.ValueAvailable = true;
        }

        private void RazValues()
        {
            foreach (MemberValue memberValue in _memberValues.Values)
            {
                memberValue.ValueAvailable = false;
            }
        }

        private void InitEnumerates()
        {
            if (_enumerates == null)
            {
                //List<MemberValue> enumerates = new List<MemberValue>();
                _enumerates = new List<MemberValue>();
                // loop on root values
                foreach (MemberValue memberValue in _memberValues.Values)
                {
                    if (memberValue.Parent == null)
                    {
                        MemberValue memberValue2 = memberValue;
                        while (memberValue2 != null)
                        {
                            // loop on childs
                            while (memberValue2.Child != null)
                                memberValue2 = (MemberValue)memberValue2.Child;

                            AddEnumerate(memberValue2);

                            // take siblin or siblin of parent
                            while (true)
                            {
                                if (memberValue2.Siblin != null)
                                {
                                    memberValue2 = (MemberValue)memberValue2.Siblin;
                                    break;
                                }
                                else
                                {
                                    // no more siblin, test if parent is enumerate
                                    memberValue2 = (MemberValue)memberValue2.Parent;
                                    if (memberValue2 == null)
                                        break;
                                    AddEnumerate(memberValue2);
                                }
                            }
                        }
                    }
                }
                //_enumerates = enumerates.ToArray();
            }
        }

        private void AddEnumerate(MemberValue memberValue)
        {
            //Trace.WriteLine("test enumerate of {0}", memberValue.MemberAccess.TreeName);
            if (memberValue.MemberAccess.IsEnumerable)
            {
                _enumerates.Add(memberValue);

                // set ParentEnumerate
                MemberValue memberValue2 = memberValue;
                MemberValue memberValue3 = memberValue;
                while (memberValue3.Parent != null)
                {
                    memberValue3 = (MemberValue)memberValue3.Parent;
                    if (memberValue3.MemberAccess.IsEnumerable)
                    {
                        memberValue2.ParentEnumerate = memberValue3;
                        memberValue2 = memberValue3;
                    }
                }
            }
        }

        private void RazEnumerates()
        {
            InitEnumerates();
            // problem il faut faire un raz sur les childs de chaque enumerate
            //foreach (MemberValue memberValue in _enumerates)
            //{
            //    memberValue.FoundNext = false;
            //    memberValue.ChildFoundNext = false;
            //}
            foreach (MemberValue memberValue in _memberValues.Values)
            {
                memberValue.FoundNext = false;
                memberValue.ChildFoundNext = false;
            }
        }
    }
}
