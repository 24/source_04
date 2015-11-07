using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace pb.Reflection
{
    public class TypeValue
    {
        public string Name;
        public string TreeName;
        public Type ValueType;
        public bool IsValueType;
        public bool IsEnumerable;
    }

    public class MemberAccess
    {
        public MemberAccess(ValueInfo valueInfo, string treeName = null)
        {
            SourceType = valueInfo.SourceType;
            Name = valueInfo.Name;
            if (treeName == null)
                TreeName = valueInfo.Name;
            else
                TreeName = treeName;
            AccessBindingFlag = GetAccessBindingFlag(valueInfo.MemberTypes);
            ValueType = valueInfo.ValueType;
            IsValueType = valueInfo.IsValueType;
            IsEnumerable = valueInfo.IsEnumerable;
        }

        public Type SourceType;
        public string Name;
        public string TreeName;
        //public MemberTypes MemberTypes;
        public BindingFlags AccessBindingFlag;
        //public MemberInfo MemberInfo;
        public Type ValueType;
        public bool IsValueType;
        public bool IsEnumerable;

        public object GetValue(object target)
        {
            return SourceType.InvokeMember(Name, AccessBindingFlag, null, target, null);
        }

        public static MemberAccess Create(Type type, string name, MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property)
        {
            //BindingFlags bindingFlags = BindingFlags.Instance;
            //if ((memberType & pb.Reflection.MemberType.Public) == pb.Reflection.MemberType.Public)
            //    bindingFlags |= BindingFlags.Public;
            //if ((memberType & pb.Reflection.MemberType.NonPublic) == pb.Reflection.MemberType.NonPublic)
            //    bindingFlags |= BindingFlags.NonPublic;
            //foreach (MemberInfo member in type.GetMember(name, bindingFlags))
            //{
            //    BindingFlags accessBindingFlag = 0;
            //    if (member.MemberType == MemberTypes.Field)
            //        accessBindingFlag = BindingFlags.GetField;
            //    else if (member.MemberType == MemberTypes.Property)
            //        accessBindingFlag = BindingFlags.GetProperty;
            //    //return new MemberAccess { SourceType = type, Name = member.Name, MemberTypes = member.MemberType, AccessBindingFlag = accessBindingFlag, MemberInfo = member };
            //    return new MemberAccess { SourceType = type, Name = member.Name, AccessBindingFlag = accessBindingFlag };
            //}
            //return null;
            ValueInfo valueInfo = type.zGetTypeValueInfo(name, memberType);
            if (valueInfo != null)
                //return new MemberAccess { SourceType = type, Name = valueInfo.Name, AccessBindingFlag = GetAccessBindingFlag(valueInfo.MemberTypes) };
                return new MemberAccess(valueInfo);
            else
                return null;
        }

        public static IEnumerable<MemberAccess> Create(Type type, MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property)
        {
            //BindingFlags bindingFlags = BindingFlags.Instance;
            //if ((memberType & pb.Reflection.MemberType.Public) == pb.Reflection.MemberType.Public)
            //    bindingFlags |= BindingFlags.Public;
            //if ((memberType & pb.Reflection.MemberType.NonPublic) == pb.Reflection.MemberType.NonPublic)
            //    bindingFlags |= BindingFlags.NonPublic;
            //bool field = (memberType & pb.Reflection.MemberType.Field) == pb.Reflection.MemberType.Field;
            //bool property = (memberType & pb.Reflection.MemberType.Property) == pb.Reflection.MemberType.Property;
            //foreach (MemberInfo member in type.GetMembers(bindingFlags))
            //{
            //    if (field && member.MemberType == MemberTypes.Field)
            //        //yield return new MemberAccess { SourceType = type, Name = member.Name, MemberTypes = member.MemberType, AccessBindingFlag = BindingFlags.GetField, MemberInfo = member };
            //        yield return new MemberAccess { SourceType = type, Name = member.Name, AccessBindingFlag = BindingFlags.GetField };
            //    else if (property && member.MemberType == MemberTypes.Property)
            //        //yield return new MemberAccess { SourceType = type, Name = member.Name, MemberTypes = member.MemberType, AccessBindingFlag = BindingFlags.GetProperty, MemberInfo = member };
            //        yield return new MemberAccess { SourceType = type, Name = member.Name, AccessBindingFlag = BindingFlags.GetProperty };
            //}
            foreach (ValueInfo valueInfo in type.zGetTypeValuesInfos(memberType))
                //yield return new MemberAccess { SourceType = type, Name = valueInfo.Name, AccessBindingFlag = GetAccessBindingFlag(valueInfo.MemberTypes) };
                yield return new MemberAccess(valueInfo);
        }

        public static BindingFlags GetAccessBindingFlag(MemberTypes memberTypes)
        {
            BindingFlags accessBindingFlag = 0;
            if (memberTypes == MemberTypes.Field)
                accessBindingFlag = BindingFlags.GetField;
            else if (memberTypes == MemberTypes.Property)
                accessBindingFlag = BindingFlags.GetProperty;
            return accessBindingFlag;
        }
    }

    public class TypeAccess
    {
        private Type _type = null;
        private Dictionary<string, MemberAccess> _members = null;

        public TypeAccess(Type type)
        {
            _type = type;
            //InitMembersAccess();
        }

        public object GetValue(string name, object target)
        {
            MemberAccess memberAccess = _members[name];
            return _type.InvokeMember(memberAccess.Name, memberAccess.AccessBindingFlag, null, target, null);
        }

        public void AddMembers(MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property)
        {
            //BindingFlags bindingFlags = BindingFlags.Instance;
            //if ((memberType & MemberType.Public) == MemberType.Public)
            //    bindingFlags |= BindingFlags.Public;
            //if ((memberType & MemberType.NonPublic) == MemberType.NonPublic)
            //    bindingFlags |= BindingFlags.NonPublic;
            //bool field = (memberType & MemberType.Field) == MemberType.Field;
            //bool property = (memberType & MemberType.Property) == MemberType.Property;
            //foreach (MemberInfo member in _type.GetMembers(bindingFlags))
            //{
            //    if (field && member.MemberType == MemberTypes.Field)
            //    {
            //        //_members.Add(member.Name, new MemberAccess { Name = member.Name, MemberTypes = member.MemberType, AccessBindingFlag = BindingFlags.GetField, MemberInfo = member });
            //        _members.Add(member.Name, new MemberAccess { Name = member.Name, AccessBindingFlag = BindingFlags.GetField });
            //    }
            //    else if (property && member.MemberType == MemberTypes.Property)
            //    {
            //        //_members.Add(member.Name, new MemberAccess { Name = member.Name, MemberTypes = member.MemberType, AccessBindingFlag = BindingFlags.GetProperty, MemberInfo = member });
            //        _members.Add(member.Name, new MemberAccess { Name = member.Name, AccessBindingFlag = BindingFlags.GetProperty });
            //    }
            //}

            foreach (MemberAccess memberAccess in MemberAccess.Create(_type, memberType))
                _members.Add(memberAccess.Name, memberAccess);
        }

        public void AddMember(string name, MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property)
        {
            //BindingFlags bindingFlags = BindingFlags.Instance;
            //if ((memberType & MemberType.Public) == MemberType.Public)
            //    bindingFlags |= BindingFlags.Public;
            //if ((memberType & MemberType.NonPublic) == MemberType.NonPublic)
            //    bindingFlags |= BindingFlags.NonPublic;
            //foreach (MemberInfo member in _type.GetMember(name, bindingFlags))
            //{
            //    BindingFlags accessBindingFlag = 0;
            //    if (member.MemberType == MemberTypes.Field)
            //        accessBindingFlag = BindingFlags.GetField;
            //    else if (member.MemberType == MemberTypes.Property)
            //        accessBindingFlag = BindingFlags.GetProperty;
            //    //_members.Add(member.Name, new MemberAccess { Name = member.Name, MemberTypes = member.MemberType, AccessBindingFlag = accessBindingFlag, MemberInfo = member });
            //    _members.Add(member.Name, new MemberAccess { Name = member.Name, AccessBindingFlag = accessBindingFlag });
            //}

            MemberAccess memberAccess = MemberAccess.Create(_type, name, memberType);
            if (memberAccess != null)
                _members.Add(memberAccess.Name, memberAccess);
        }
    }
}
