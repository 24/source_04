using System;
using System.Collections;
using System.Collections.Generic;

namespace pb.Reflection
{
    // TreeBase
    public class NodeBase
    {
        //T Parent { get; }
        //T Child { get; }
        //T Siblin { get; }
        public NodeBase Parent;
        public NodeBase Child;
        public NodeBase Siblin;

        public void SetParent(NodeBase parent)
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
                    NodeBase siblin = parent.Child;
                    while (siblin.Siblin != null)
                        siblin = siblin.Siblin;
                    siblin.Siblin = this;
                }
            }
        }
    }

    // MemberValue
    public class TypeValueNode : NodeBase
    {
        public TypeValueAccess TypeValueAccess;
        //public MemberValue Parent;
        //public MemberValue Child;
        //public MemberValue Siblin;
        public bool ValueAvailable;              // Value is set
        public object Value;
        public IEnumerator Enumerator;
        public bool FoundNext;
        public bool ChildFoundNext;
        public TypeValueNode ParentEnumerate;

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
        private Dictionary<string, TypeValueNode> _typeValueNodes = new Dictionary<string, TypeValueNode>();
        private List<TypeValueNode> _enumerates = null;
        private T _data;
        private bool _nextValue = false;

        public TypeValues()
        {
            _type = typeof(T);
        }

        public Dictionary<string, TypeValueNode> TypeValueNodes { get { return _typeValueNodes; } }

        public void AddAllValues(MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property)
        {
            foreach (TreeValue<TypeValueInfo> treeValueInfo in _type.zGetTypeAllValuesInfos(memberType, options: TypeReflectionOptions.ValueType | TypeReflectionOptions.NotValueType))
            {
                TypeValueNode typeValueNode = new TypeValueNode { TypeValueAccess = new TypeValueAccess(treeValueInfo.Value, treeValueInfo.Value.TreeName) };
                TypeValueNode parent = null;
                if (treeValueInfo.Value.ParentName != null)
                {
                    parent = _typeValueNodes[treeValueInfo.Value.ParentName];
                    typeValueNode.SetParent(parent);
                }
                //if (typeValueNode.TypeValueAccess.IsValueType)
                //    yield return typeValueNode;
                _typeValueNodes.Add(treeValueInfo.Value.TreeName, typeValueNode);
            }
        }

        public TypeValueNode AddValue(string name, MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property, bool notEnumerable = false)
        {
            string parentName = null;
            TypeValueNode parent = null;
            Type type = _type;
            TypeValueNode typeValueNode = null;
            foreach (string valueName in name.Split('.'))
            {
                string treeName = valueName;
                if (parentName != null)
                    treeName = parentName + "." + treeName;
                typeValueNode = null;
                if (!_typeValueNodes.ContainsKey(treeName))
                {
                    TypeValueInfo typeValueInfo = type.zGetTypeValueInfo(valueName, memberType);
                    if (typeValueInfo == null)
                        throw new PBException("unknow value \"{0}\" from type {1} memberType {2}", valueName, type.zGetTypeName(), memberType.ToString());
                    typeValueNode = new TypeValueNode { TypeValueAccess = new TypeValueAccess(typeValueInfo, treeName) };
                    if (notEnumerable)
                        typeValueNode.TypeValueAccess.IsEnumerable = false;
                    typeValueNode.SetParent(parent);
                    _typeValueNodes.Add(treeName, typeValueNode);
                }
                else
                    typeValueNode = _typeValueNodes[valueName];
                parentName = treeName;
                parent = typeValueNode;
                type = typeValueNode.TypeValueAccess.ValueType;
            }
            return typeValueNode;
        }

        public IEnumerable<TypeValue> GetTypeValues()
        {
            foreach (TypeValueNode typeValueNode in _typeValueNodes.Values)
            {
                yield return typeValueNode.TypeValueAccess;
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
            foreach (TypeValueNode typeValueNode in _typeValueNodes.Values)
            {
                typeValueNode.TypeValueAccess.Value = GetValue(typeValueNode, onlyNextValue);
                yield return typeValueNode.TypeValueAccess;
            }
        }

        public bool NextValues()
        {
            RazEnumerates();
            bool found = false;
            foreach (TypeValueNode typeValueNode in _enumerates)
            {
                if (!typeValueNode.ChildFoundNext)
                {
                    IEnumerator enumerator = typeValueNode.Enumerator;
                    if (enumerator != null)
                    {
                        if (enumerator.MoveNext())
                        {
                            found = true;
                            typeValueNode.Value = enumerator.Current;
                            typeValueNode.FoundNext = true;

                            // update childs values
                            UpdateChildsValues(typeValueNode);

                            TypeValueNode typeValueNode2 = typeValueNode;
                            while (true)
                            {
                                typeValueNode2 = typeValueNode2.ParentEnumerate;
                                if (typeValueNode2 == null)
                                    break;
                                typeValueNode2.ChildFoundNext = true;
                            }
                        }
                    }
                }
            }
            _nextValue = true;
            return found;
        }

        private void UpdateChildsValues(TypeValueNode typeValueNode)
        {
            // memberValue.ValueAvailable
            // memberValue.FoundNext = false;
            // memberValue.ChildFoundNext = false;
            //object data = memberValue.Value;

            TypeValueNode typeValueNode2 = typeValueNode;
            if (typeValueNode2.Child != null)
            {
                while (true)
                {
                    // loop on childs
                    while (typeValueNode2.Child != null)
                    {
                        typeValueNode2 = (TypeValueNode)typeValueNode2.Child;
                        //memberValue2.ValueAvailable = false;
                        SetValue(typeValueNode2);
                        typeValueNode2.FoundNext = true;
                    }

                    while (true)
                    {
                        if (typeValueNode2.Siblin != null)
                        {
                            typeValueNode2 = (TypeValueNode)typeValueNode2.Siblin;
                            //memberValue2.ValueAvailable = false;
                            SetValue(typeValueNode2);
                            typeValueNode2.FoundNext = true;
                        }
                        else
                        {
                            typeValueNode2 = (TypeValueNode)typeValueNode2.Parent;
                            if (typeValueNode2 == typeValueNode)
                                return;
                        }
                    }
                }
            }
        }

        public object GetValue(string name, bool onlyNextValue = false)
        {
            if (!_typeValueNodes.ContainsKey(name))
                throw new PBException("unknow value \"{0}\"", name);
            return GetValue(_typeValueNodes[name], onlyNextValue);
        }

        public object GetValue(TypeValueNode typeValueNode, bool onlyNextValue = false)
        {
            if (!_nextValue)
                _GetValue(typeValueNode);
            if (!_nextValue || !onlyNextValue || typeValueNode.FoundNext)
                return typeValueNode.Value;
            else
                return null;
        }

        private void _GetValue(TypeValueNode typeValueNode)
        {
            if (!typeValueNode.ValueAvailable)
            {
                //object data = null;
                if (typeValueNode.Parent != null)
                {
                    _GetValue((TypeValueNode)typeValueNode.Parent);
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
                SetValue(typeValueNode);
            }
        }

        private void SetValue(TypeValueNode typeValueNode)
        {
            object data;
            if (typeValueNode.Parent != null)
                data = ((TypeValueNode)typeValueNode.Parent).Value;
            else
                data = _data;
            if (data != null)
            {
                object value = typeValueNode.TypeValueAccess.GetValue(data);
                if (typeValueNode.TypeValueAccess.IsEnumerable)
                {
                    typeValueNode.Enumerator = ((IEnumerable)value).GetEnumerator();
                    if (typeValueNode.Enumerator != null && typeValueNode.Enumerator.MoveNext())
                        typeValueNode.Value = typeValueNode.Enumerator.Current;
                }
                else
                    typeValueNode.Value = value;
            }
            else
                typeValueNode.Value = null;
            typeValueNode.ValueAvailable = true;
        }

        private void RazValues()
        {
            foreach (TypeValueNode typeValueNode in _typeValueNodes.Values)
            {
                typeValueNode.ValueAvailable = false;
            }
        }

        private void InitEnumerates()
        {
            if (_enumerates == null)
            {
                //List<MemberValue> enumerates = new List<MemberValue>();
                _enumerates = new List<TypeValueNode>();
                // loop on root values
                foreach (TypeValueNode typeValueNode in _typeValueNodes.Values)
                {
                    if (typeValueNode.Parent == null)
                    {
                        TypeValueNode typeValueNode2 = typeValueNode;
                        while (typeValueNode2 != null)
                        {
                            // loop on childs
                            while (typeValueNode2.Child != null)
                                typeValueNode2 = (TypeValueNode)typeValueNode2.Child;

                            AddEnumerate(typeValueNode2);

                            // take siblin or siblin of parent
                            while (true)
                            {
                                if (typeValueNode2.Siblin != null)
                                {
                                    typeValueNode2 = (TypeValueNode)typeValueNode2.Siblin;
                                    break;
                                }
                                else
                                {
                                    // no more siblin, test if parent is enumerate
                                    typeValueNode2 = (TypeValueNode)typeValueNode2.Parent;
                                    if (typeValueNode2 == null)
                                        break;
                                    AddEnumerate(typeValueNode2);
                                }
                            }
                        }
                    }
                }
                //_enumerates = enumerates.ToArray();
            }
        }

        private void AddEnumerate(TypeValueNode typeValueNode)
        {
            //Trace.WriteLine("test enumerate of {0}", memberValue.MemberAccess.TreeName);
            if (typeValueNode.TypeValueAccess.IsEnumerable)
            {
                _enumerates.Add(typeValueNode);

                // set ParentEnumerate
                TypeValueNode typeValueNode2 = typeValueNode;
                TypeValueNode typeValueNode3 = typeValueNode;
                while (typeValueNode3.Parent != null)
                {
                    typeValueNode3 = (TypeValueNode)typeValueNode3.Parent;
                    if (typeValueNode3.TypeValueAccess.IsEnumerable)
                    {
                        typeValueNode2.ParentEnumerate = typeValueNode3;
                        typeValueNode2 = typeValueNode3;
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
            foreach (TypeValueNode typeValueNode in _typeValueNodes.Values)
            {
                typeValueNode.FoundNext = false;
                typeValueNode.ChildFoundNext = false;
            }
        }
    }
}
