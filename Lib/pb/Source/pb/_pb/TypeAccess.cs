using System;
using System.Collections.Generic;
using System.Reflection;

namespace pb
{
    [Flags]
    public enum MemberType
    {
        Public        = 0x0001,
        NonPublic     = 0x0002,
        Field         = 0x0004,
        Property      = 0x0008
    }

    public class MemberAccess
    {
        public Type SourceType;
        public string Name;
        public MemberTypes MemberType;
        public BindingFlags AccessBindingFlag;
        public MemberInfo MemberInfo;

        public object GetValue(object target)
        {
            return SourceType.InvokeMember(Name, AccessBindingFlag, null, target, null);
        }

        public static MemberAccess Create(Type type, string name, MemberType memberType = pb.MemberType.Public)
        {
            BindingFlags bindingFlags = BindingFlags.Instance;
            if ((memberType & pb.MemberType.Public) == pb.MemberType.Public)
                bindingFlags |= BindingFlags.Public;
            if ((memberType & pb.MemberType.NonPublic) == pb.MemberType.NonPublic)
                bindingFlags |= BindingFlags.NonPublic;
            foreach (MemberInfo member in type.GetMember(name, bindingFlags))
            {
                BindingFlags accessBindingFlag = 0;
                if (member.MemberType == MemberTypes.Field)
                    accessBindingFlag = BindingFlags.GetField;
                else if (member.MemberType == MemberTypes.Property)
                    accessBindingFlag = BindingFlags.GetProperty;
                return new MemberAccess { SourceType = type, Name = member.Name, MemberType = member.MemberType, AccessBindingFlag = accessBindingFlag, MemberInfo = member };
            }
            return null;
        }

        public static IEnumerable<MemberAccess> Create(Type type, MemberType memberType = pb.MemberType.Public | pb.MemberType.Field | pb.MemberType.Property)
        {
            BindingFlags bindingFlags = BindingFlags.Instance;
            if ((memberType & pb.MemberType.Public) == pb.MemberType.Public)
                bindingFlags |= BindingFlags.Public;
            if ((memberType & pb.MemberType.NonPublic) == pb.MemberType.NonPublic)
                bindingFlags |= BindingFlags.NonPublic;
            bool field = (memberType & pb.MemberType.Field) == pb.MemberType.Field;
            bool property = (memberType & pb.MemberType.Property) == pb.MemberType.Property;
            foreach (MemberInfo member in type.GetMembers(bindingFlags))
            {
                if (field && member.MemberType == MemberTypes.Field)
                    yield return new MemberAccess { SourceType = type, Name = member.Name, MemberType = member.MemberType, AccessBindingFlag = BindingFlags.GetField, MemberInfo = member };
                else if (property && member.MemberType == MemberTypes.Property)
                    yield return new MemberAccess { SourceType = type, Name = member.Name, MemberType = member.MemberType, AccessBindingFlag = BindingFlags.GetProperty, MemberInfo = member };
            }
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

        public void AddMembers(MemberType memberType = MemberType.Public | MemberType.Field | MemberType.Property)
        {
            BindingFlags bindingFlags = BindingFlags.Instance;
            if ((memberType & MemberType.Public) == MemberType.Public)
                bindingFlags |= BindingFlags.Public;
            if ((memberType & MemberType.NonPublic) == MemberType.NonPublic)
                bindingFlags |= BindingFlags.NonPublic;
            bool field = (memberType & MemberType.Field) == MemberType.Field;
            bool property = (memberType & MemberType.Property) == MemberType.Property;
            foreach (MemberInfo member in _type.GetMembers(bindingFlags))
            {
                if (field && member.MemberType == MemberTypes.Field)
                {
                    _members.Add(member.Name, new MemberAccess { Name = member.Name, MemberType = member.MemberType, AccessBindingFlag = BindingFlags.GetField, MemberInfo = member });
                }
                else if (property && member.MemberType == MemberTypes.Property)
                {
                    _members.Add(member.Name, new MemberAccess { Name = member.Name, MemberType = member.MemberType, AccessBindingFlag = BindingFlags.GetProperty, MemberInfo = member });
                }
            }
        }

        public void AddMember(string name, MemberType memberType = MemberType.Public)
        {
            BindingFlags bindingFlags = BindingFlags.Instance;
            if ((memberType & MemberType.Public) == MemberType.Public)
                bindingFlags |= BindingFlags.Public;
            if ((memberType & MemberType.NonPublic) == MemberType.NonPublic)
                bindingFlags |= BindingFlags.NonPublic;
            foreach (MemberInfo member in _type.GetMember(name, bindingFlags))
            {
                BindingFlags accessBindingFlag = 0;
                if (member.MemberType == MemberTypes.Field)
                    accessBindingFlag = BindingFlags.GetField;
                else if (member.MemberType == MemberTypes.Property)
                    accessBindingFlag = BindingFlags.GetProperty;
                _members.Add(member.Name, new MemberAccess { Name = member.Name, MemberType = member.MemberType, AccessBindingFlag = accessBindingFlag, MemberInfo = member });
            }
        }
    }
}
