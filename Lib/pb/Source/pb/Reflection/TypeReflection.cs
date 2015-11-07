using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace pb.Reflection
{
    [Flags]
    public enum MemberType
    {
        Instance      = 0x0001,
        Static        = 0x0002,
        Public        = 0x0004,
        NonPublic     = 0x0008,
        Field         = 0x0010,
        Property      = 0x0020
    }

    public class ValueInfo
    {                                        // ex class Test { public string Title; }
        public Type SourceType;              // ex pb.Test.Test_01
        public string Name;                  // ex "Title"
        public string TreeName;              // ex "Title"
        public string ParentName;            // ex null
        public Type ValueType;               // ex System.String
        public bool IsValueType;             // ex true, true for int string DateTime Date Bitmap
        public bool IsEnumerable;            // ex false
        public Type DeclaringType;           // ex pb.Test.Test_01
        public Type ReflectedType;           // ex pb.Test.Test_01
        public MemberTypes MemberTypes;       // ex MemberTypes.Field
        public int MetadataToken;            // ex 67108935
        public Module Module;                // ex RunCode_00002.dll

        public ValueInfo(Type sourceType, MemberInfo memberInfo)
        {
            SourceType = sourceType;
            Name = memberInfo.Name;
            TreeName = memberInfo.Name;
            ParentName = null;

            Type valueType = memberInfo.zGetValueType();
            Type enumerableType = null;
            if (valueType != typeof(string))
                enumerableType = zReflection.GetEnumerableType(valueType);
            if (enumerableType != null)
            {
                ValueType = enumerableType;
                IsEnumerable = true;
            }
            else
            {
                ValueType = valueType;
                IsEnumerable = false;
            }

            IsValueType = TypeReflection.IsValueType(ValueType);
            DeclaringType = memberInfo.DeclaringType;
            ReflectedType = memberInfo.ReflectedType;
            MemberTypes = memberInfo.MemberType;
            MetadataToken = memberInfo.MetadataToken;
            Module = memberInfo.Module;
        }

        public ValueInfo(Type valueType)
        {
            SourceType = null;
            Name = null;
            TreeName = null;
            ParentName = null;

            Type enumerableType = null;
            if (valueType != typeof(string))
                enumerableType = zReflection.GetEnumerableType(valueType);
            if (enumerableType != null)
            {
                ValueType = enumerableType;
                IsEnumerable = true;
            }
            else
            {
                ValueType = valueType;
                IsEnumerable = false;
            }

            IsValueType = TypeReflection.IsValueType(ValueType);
            DeclaringType = null;
            ReflectedType = null;
            MemberTypes = 0;
            MetadataToken = 0;
            Module = valueType.Module;
        }
    }

    public class TraceValueInfo
    {
        public string Name;
        public string TreeName;
        public string ParentName;
        public string ValueType;
        public bool IsValueType;
        public bool IsEnumerable;
        public string DeclaringType;
        public string ReflectedType;
        public string MemberType;
        public int MetadataToken;
        public string Module;

        public TraceValueInfo(ValueInfo valueInfo)
        {
            Name = valueInfo.Name;
            TreeName = valueInfo.TreeName;
            ParentName = valueInfo.ParentName;
            ValueType = valueInfo.ValueType.zGetTypeName();
            IsValueType = valueInfo.IsValueType;
            IsEnumerable = valueInfo.IsEnumerable;
            DeclaringType = valueInfo.DeclaringType.zGetTypeName();
            ReflectedType = valueInfo.ReflectedType.zGetTypeName();
            MemberType = valueInfo.MemberTypes.ToString();
            MetadataToken = valueInfo.MetadataToken;
            Module = valueInfo.Module.Name;
        }
    }

    [Flags]
    public enum TypeReflectionOptions
    {
        Source           = 0x0001,
        Parent           = 0x0002,
        ValueType        = 0x0004,
        NotValueType     = 0x0008
    }

    public class TypeReflection
    {
        private BindingFlags _bindingFlags;
        private MemberTypes _memberTypes;
        private Func<ValueInfo, TreeFilter> _filter = null;
        //private bool _verbose = false;
        private bool _returnSource = false;
        private bool _returnParent = false;
        private bool _returnValueType = false;
        private bool _returnNotValueType = false;
        private int _level = 1;
        private int _index = 1;
        private Stack<IEnumerator<ValueInfo>> _stack = new Stack<IEnumerator<ValueInfo>>();
        private IEnumerator<ValueInfo> _enumerator = null;
        private ValueInfo _valueInfo = null;
        private string _treeName = null;

        //public static ValueInfo GetValueInfo(Type type, string name, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public, MemberTypes memberType = MemberTypes.All)
        public static ValueInfo GetValueInfo(Type type, string name, MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property)
        {
            MemberInfo memberInfo = type.GetMember(name, GetBindingFlags(memberType)).FirstOrDefault();
            if (memberInfo != null && (memberInfo.MemberType & GetMemberTypes(memberType)) != 0)
                return new ValueInfo(type, memberInfo);
            else
                return null;
        }

        public static IEnumerable<ValueInfo> GetValuesInfos(Type type, MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property)
        {
            return GetValuesInfos(type, GetBindingFlags(memberType), GetMemberTypes(memberType));
        }

        public static IEnumerable<ValueInfo> GetValuesInfos(Type type, BindingFlags bindingFlags, MemberTypes memberTypes)
        {
            if (type == null)
                yield break;
            foreach (MemberInfo memberInfo in type.GetMembers(bindingFlags))
            {
                if ((memberInfo.MemberType & memberTypes) == 0)
                    continue;
                yield return new ValueInfo(type, memberInfo);
            }
        }

        public static IEnumerable<TreeValue<ValueInfo>> GetAllValuesInfos(Type type, MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property,
            TypeReflectionOptions options = TypeReflectionOptions.ValueType | TypeReflectionOptions.NotValueType, Func<ValueInfo, TreeFilter> filter = null)
        {
            TypeReflection typeReflection = new TypeReflection();
            typeReflection._bindingFlags = GetBindingFlags(memberType);
            typeReflection._memberTypes = GetMemberTypes(memberType);
            typeReflection._filter = filter;
            //typeReflection._verbose = verbose;
            if ((options & TypeReflectionOptions.Source) == TypeReflectionOptions.Source)
                typeReflection._returnSource = true;
            if ((options & TypeReflectionOptions.Parent) == TypeReflectionOptions.Parent)
                typeReflection._returnParent = true;
            if ((options & TypeReflectionOptions.ValueType) == TypeReflectionOptions.ValueType)
                typeReflection._returnValueType = true;
            if ((options & TypeReflectionOptions.NotValueType) == TypeReflectionOptions.NotValueType)
                typeReflection._returnNotValueType = true;
            return typeReflection._GetAllValuesInfos(type);
        }

        // Type.IsValueType :
        //   true  : int, DateTime, Date, struct
        //   false : string
        // Type.GetTypeCode() :
        //   int = TypeCode.Int32, string = TypeCode.String, DateTime = TypeCode.DateTime, TypeCode (enum) = TypeCode.Int32
        //   Date = TypeCode.Object, Test_01 (class) = TypeCode.Object
        public static bool IsValueType(Type type)
        {
            // from TypeView_v2.GetValuesFromVariable() : type.IsValueType && properties.Length == 0 && fields.Length == 0
            if (Type.GetTypeCode(type) != TypeCode.Object && type != typeof(Date) && type != typeof(Bitmap))
                return true;
            else
                return false;
        }

        public static BindingFlags GetBindingFlags(MemberType memberType)
        {
            BindingFlags bindingFlags = 0;
            if ((memberType & MemberType.Instance) == MemberType.Instance)
                bindingFlags |= BindingFlags.Instance;
            if ((memberType & MemberType.Static) == MemberType.Static)
                bindingFlags |= BindingFlags.Static;
            if ((memberType & MemberType.Public) == MemberType.Public)
                bindingFlags |= BindingFlags.Public;
            if ((memberType & MemberType.NonPublic) == MemberType.NonPublic)
                bindingFlags |= BindingFlags.NonPublic;
            return bindingFlags;
        }

        public static MemberTypes GetMemberTypes(MemberType memberType)
        {
            MemberTypes memberTypes = 0;
            if ((memberType & MemberType.Field) == MemberType.Field)
                memberTypes |= MemberTypes.Field;
            if ((memberType & MemberType.Property) == MemberType.Property)
                memberTypes |= MemberTypes.Property;
            return memberTypes;
        }

        private IEnumerable<TreeValue<ValueInfo>> _GetAllValuesInfos(Type type)
        {
            if (type == null)
                yield break;

            _valueInfo = new ValueInfo(type);

            TreeValue<ValueInfo> treeValue;

            treeValue = GetTreeValue(TreeOpe.Source);
            if (_returnSource)
            {
                _index++;
                yield return treeValue;
            }

            if (treeValue.Stop || treeValue.Skip)
                yield break;

            while (true)
            {
                // get child
                while (true)
                {
                    if (!GetChild())
                        break;

                    treeValue = GetTreeValue(TreeOpe.Child);
                    //if (_verbose || _valueInfo.IsValueType)
                    if ((_valueInfo.IsValueType && _returnValueType) || (!_valueInfo.IsValueType && _returnNotValueType))
                    {
                        _index++;
                        yield return treeValue;
                    }

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
                        if (!GetSiblin())
                            break;

                        treeValue = GetTreeValue(TreeOpe.Siblin);
                        //if (_verbose || _valueInfo.IsValueType)
                        if ((_valueInfo.IsValueType && _returnValueType) || (!_valueInfo.IsValueType && _returnNotValueType))
                        {
                            _index++;
                            yield return treeValue;
                        }

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

                    if (_returnParent)
                        yield return new TreeValue<ValueInfo> { Index = _index++, Value = null, Level = _level - 1, TreeOpe = TreeOpe.Parent, Selected = false, Skip = false, Stop = false };

                    if (!GetParent())
                        yield break;
                }
            }
        }

        private TreeValue<ValueInfo> GetTreeValue(TreeOpe treeOpe)
        {
            TreeValue<ValueInfo> treeValue = new TreeValue<ValueInfo> { Index = _index, Value = _valueInfo, Level = _level, TreeOpe = treeOpe, Selected = false, Skip = false, Stop = false };

            TreeFilter treeFilter = TreeFilter.Select;
            if (_filter != null)
                treeFilter = _filter(_valueInfo);

            if ((treeFilter & TreeFilter.Stop) == TreeFilter.Stop)
            {
                treeValue.Stop = true;
            }

            else if ((treeFilter & TreeFilter.Skip) == TreeFilter.Skip)
            {
                treeValue.Skip = true;
            }

            else if ((treeFilter & TreeFilter.DontSelect) == 0)
            {
                treeValue.Selected = true;
            }
            return treeValue;
        }

        private bool GetChild()
        {
            if (_valueInfo.IsValueType)
                return false;

            if (_enumerator != null)
                _stack.Push(_enumerator);

            _level++;
            _enumerator = GetValuesInfos(_valueInfo.ValueType, _bindingFlags, _memberTypes).GetEnumerator();
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
        public static ValueInfo zGetTypeValueInfo(this Type type, string name, MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property)
        {
            return TypeReflection.GetValueInfo(type, name, memberType);
        }

        public static IEnumerable<ValueInfo> zGetTypeValuesInfos(this Type type, MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property)
        {
            return TypeReflection.GetValuesInfos(type, memberType);
        }

        public static IEnumerable<ValueInfo> zGetTypeValuesInfos(this Type type, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public, MemberTypes memberTypes = MemberTypes.All)
        {
            return TypeReflection.GetValuesInfos(type, bindingFlags, memberTypes);
        }

        public static IEnumerable<TreeValue<ValueInfo>> zGetTypeAllValuesInfos(this Type type, MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property,
            TypeReflectionOptions options = TypeReflectionOptions.ValueType | TypeReflectionOptions.NotValueType, Func<ValueInfo, TreeFilter> filter = null)
        {
            return TypeReflection.GetAllValuesInfos(type, memberType, options, filter);
        }

        public static IEnumerable<TraceValueInfo> zToTraceValuesInfos(this IEnumerable<ValueInfo> valuesInfos)
        {
            return valuesInfos.Select(valueInfo => new TraceValueInfo(valueInfo));
        }

        public static IEnumerable<TraceTreeValue<TraceValueInfo>> zToTraceTreeValuesInfos(this IEnumerable<TreeValue<ValueInfo>> treeValuesInfos)
        {
            return treeValuesInfos.Select(treeValueInfo => new TraceTreeValue<TraceValueInfo>
            {
                Index = treeValueInfo.Index,
                Value = treeValueInfo.Value != null ? new TraceValueInfo(treeValueInfo.Value) : null,
                Level = treeValueInfo.Level,
                TreeOpe = treeValueInfo.TreeOpe.ToString(),
                Selected = treeValueInfo.Selected,
                Skip = treeValueInfo.Skip,
                Stop = treeValueInfo.Stop
            });
        }
    }
}
