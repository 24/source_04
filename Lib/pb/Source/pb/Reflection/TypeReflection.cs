using System;
using System.Collections.Generic;
using System.Reflection;

namespace pb.Reflection
{
    public class TypeReflection
    {
        private BindingFlags _bindingFlags;
        private MemberTypes _memberTypes = MemberTypes.Field | MemberTypes.Property;
        private Func<ValueInfo, TreeFilter> _filter = null;
        private bool _verbose = false;
        private int _level = 1;
        private int _index = 1;
        private Stack<IEnumerator<ValueInfo>> _stack = new Stack<IEnumerator<ValueInfo>>();
        private IEnumerator<ValueInfo> _enumerator = null;
        private ValueInfo _valueInfo = null;
        private string _treeName = null;

        public static IEnumerable<TreeValue<ValueInfo>> GetAllValuesInfos(Type type, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public,
            Func<ValueInfo, TreeFilter> filter = null, bool verbose = false)
        {
            TypeReflection typeReflection = new TypeReflection();
            typeReflection._bindingFlags = bindingFlags;
            typeReflection._filter = filter;
            typeReflection._verbose = verbose;
            return typeReflection._GetAllValuesInfos(type);
        }

        //private IEnumerable<TreeValue<ValueInfo>> _GetAllValuesInfos(Type type, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public,
        //    Func<ValueInfo, TreeFilter> filter = null, bool verbose = false)
        private IEnumerable<TreeValue<ValueInfo>> _GetAllValuesInfos(Type type)
        {
            if (type == null)
                yield break;

            //_bindingFlags = bindingFlags;
            //_filter = filter;
            //_verbose = verbose;

            _valueInfo = new ValueInfo(type);

            TreeValue<ValueInfo> treeValue;
            //TreeFilter treeFilter;

            if (_verbose)
            {
                //treeValue = new TreeValue<ValueInfo> { Index = _index++, Value = valueInfo, Level = _level, TreeOpe = TreeOpe.Source, Selected = false, Skip = false, Stop = false };

                //treeFilter = TreeFilter.Select;
                //if (filter != null)
                //    treeFilter = filter(valueInfo);

                //if ((treeFilter & TreeFilter.Stop) == TreeFilter.Stop)
                //{
                //    treeValue.Stop = true;
                //    yield return treeValue;
                //    yield break;
                //}

                //if ((treeFilter & TreeFilter.Skip) == TreeFilter.Skip)
                //{
                //    treeValue.Skip = true;
                //    yield return treeValue;
                //    yield break;
                //}

                //if ((treeFilter & TreeFilter.DontSelect) == 0)
                //{
                //    treeValue.Selected = true;
                //    yield return treeValue;
                //}
                //else
                //    yield return treeValue;

                treeValue = GetTreeValue(TreeOpe.Source);
                yield return treeValue;

                if (treeValue.Stop || treeValue.Skip)
                    yield break;
            }

            //Stack<IEnumerator<ValueInfo>> stack = new Stack<IEnumerator<ValueInfo>>();
            //IEnumerator<ValueInfo> enumerator = null;
            //string treeName = null;
            while (true)
            {
                // get child
                while (true)
                {
                    //if (_valueInfo.IsValueType)
                    //    break;

                    //if (_enumerator != null)
                    //    _stack.Push(_enumerator);

                    //_level++;
                    //_enumerator = _valueInfo.ValueType.zGetTypeValuesInfos(_bindingFlags, MemberTypes.Field | MemberTypes.Property).GetEnumerator();
                    //if (!_enumerator.MoveNext())
                    //    break;

                    //_treeName = _valueInfo.TreeName;
                    //_valueInfo = _enumerator.Current;
                    //if (_treeName != null)
                    //{
                    //    _valueInfo.TreeName = _treeName + "." + _valueInfo.TreeName;
                    //    _valueInfo.ParentName = _treeName;
                    //}

                    if (!GetChild())
                        break;

                    //treeValue = new TreeValue<ValueInfo> { Index = _index++, Value = _valueInfo, Level = _level, TreeOpe = TreeOpe.Child, Selected = false, Skip = false, Stop = false };

                    //treeFilter = TreeFilter.Select;
                    //if (filter != null)
                    //    treeFilter = filter(_valueInfo);

                    //if ((treeFilter & TreeFilter.Stop) == TreeFilter.Stop)
                    //{
                    //    treeValue.Stop = true;
                    //    yield return treeValue;
                    //    yield break;
                    //}

                    //if ((treeFilter & TreeFilter.Skip) == TreeFilter.Skip)
                    //{
                    //    treeValue.Skip = true;
                    //    yield return treeValue;
                    //    break;
                    //}

                    //if ((treeFilter & TreeFilter.DontSelect) == 0)
                    //{
                    //    treeValue.Selected = true;
                    //    yield return treeValue;
                    //}
                    //else
                    //    yield return treeValue;

                    treeValue = GetTreeValue(TreeOpe.Child);
                    yield return treeValue;

                    if (treeValue.Stop)
                        yield break;

                    if (treeValue.Skip)
                        break;
                }

                if (_enumerator == null)
                    break;

                // get next sibling node or next sibling node of parent node
                bool getChild = false;
                while (true)
                {
                    // next sibling node
                    while (true)
                    {
                        //if (!_enumerator.MoveNext())
                        //    break;

                        //_valueInfo = _enumerator.Current;
                        //if (_treeName != null)
                        //{
                        //    _valueInfo.TreeName = _treeName + "." + _valueInfo.TreeName;
                        //    _valueInfo.ParentName = _treeName;
                        //}

                        if (!GetSiblin())
                            break;

                        //treeValue = new TreeValue<ValueInfo> { Index = _index++, Value = _valueInfo, Level = _level, TreeOpe = TreeOpe.Siblin, Selected = false, Skip = false, Stop = false };

                        //treeFilter = TreeFilter.Select;
                        //if (filter != null)
                        //    treeFilter = filter(_valueInfo);

                        //if ((treeFilter & TreeFilter.Stop) == TreeFilter.Stop)
                        //{
                        //    treeValue.Stop = true;
                        //    yield return treeValue;
                        //    yield break;
                        //}

                        //if ((treeFilter & TreeFilter.Skip) == 0)
                        //{
                        //    if ((treeFilter & TreeFilter.DontSelect) == 0)
                        //    {
                        //        treeValue.Selected = true;
                        //        yield return treeValue;
                        //    }
                        //    else
                        //        yield return treeValue;
                        //    getChild = true;
                        //    break;
                        //}
                        //else
                        //{
                        //    treeValue.Skip = true;
                        //    yield return treeValue;
                        //}

                        treeValue = GetTreeValue(TreeOpe.Siblin);
                        yield return treeValue;

                        if (treeValue.Stop)
                            yield break;

                        if (!treeValue.Skip)
                        {
                            getChild = true;
                            break;
                        }
                    }
                    if (getChild)
                        break;

                    //_level--;
                    yield return new TreeValue<ValueInfo> { Index = _index++, Value = null, Level = _level - 1, TreeOpe = TreeOpe.Parent, Selected = false, Skip = false, Stop = false };

                    // parent node
                    //if (_stack.Count == 0)
                    //{
                    //    yield break;
                    //}

                    //_enumerator = _stack.Pop();
                    //_treeName = _enumerator.Current.ParentName;

                    if (!GetParent())
                        yield break;
                }
            }
        }

        private TreeValue<ValueInfo> GetTreeValue(TreeOpe treeOpe)
        {
            TreeValue<ValueInfo> treeValue = new TreeValue<ValueInfo> { Index = _index++, Value = _valueInfo, Level = _level, TreeOpe = treeOpe, Selected = false, Skip = false, Stop = false };

            TreeFilter treeFilter = TreeFilter.Select;
            if (_filter != null)
                treeFilter = _filter(_valueInfo);

            if ((treeFilter & TreeFilter.Stop) == TreeFilter.Stop)
            {
                treeValue.Stop = true;
                //yield return treeValue;
                //yield break;
            }

            else if ((treeFilter & TreeFilter.Skip) == TreeFilter.Skip)
            {
                treeValue.Skip = true;
                //yield return treeValue;
                //yield break;
            }

            else if ((treeFilter & TreeFilter.DontSelect) == 0)
            {
                treeValue.Selected = true;
                //yield return treeValue;
            }
            //else
            //    yield return treeValue;
            return treeValue;
        }

        private bool GetChild()
        {
            if (_valueInfo.IsValueType)
                return false;

            if (_enumerator != null)
                _stack.Push(_enumerator);

            _level++;
            _enumerator = _valueInfo.ValueType.zGetTypeValuesInfos(_bindingFlags, _memberTypes).GetEnumerator();
            if (!_enumerator.MoveNext())
                return false;

            _treeName = _valueInfo.TreeName;
            _valueInfo = _enumerator.Current;
            SetTreeName();
            return true;
        }

        private bool GetSiblin()
        {
            if (!_enumerator.MoveNext())
                return false;

            _valueInfo = _enumerator.Current;
            SetTreeName();
            return true;
        }

        private bool GetParent()
        {
            if (_stack.Count == 0)
                return false;

            _enumerator = _stack.Pop();
            _treeName = _enumerator.Current.ParentName;
            _level--;
            return true;
        }

        private void SetTreeName()
        {
            if (_treeName != null)
            {
                _valueInfo.TreeName = _treeName + "." + _valueInfo.TreeName;
                _valueInfo.ParentName = _treeName;
            }
        }
    }

    public static partial class GlobalExtension
    {
        public static IEnumerable<TreeValue<ValueInfo>> zGetTypeAllValuesInfos_v2(this Type type, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public, Func<ValueInfo, TreeFilter> filter = null)
        {
            return TypeReflection.GetAllValuesInfos(type, bindingFlags, filter);
        }
    }
}
