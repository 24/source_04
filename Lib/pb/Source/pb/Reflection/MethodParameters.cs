using pb.Data;
using System.Collections.Generic;
using System.Reflection;

namespace pb.Reflection
{
    public static class MethodParameters
    {
        public static object[] GetParameters(MethodInfo methodInfo, NamedValues<ZValue> namedParameters, ErrorOptions option = ErrorOptions.None)
        {
            if (methodInfo == null)
                return null;
            List<object> parameters = new List<object>();
            //ParameterInfo[] methodParameters = methodInfo.GetParameters();
            foreach (ParameterInfo parameter in methodInfo.GetParameters())
            {
                object value;
                if (namedParameters != null && namedParameters.ContainsKey(parameter.Name))
                {
                    // no control between parameter type and named parameter type
                    value = namedParameters[parameter.Name].ToObject();
                }
                else if (parameter.HasDefaultValue)
                {
                    value = parameter.DefaultValue;
                }
                else
                {
                    Error.WriteMessage(option, $"no value for parameter \"{parameter.Name}\" of method \"{methodInfo.zGetName()}\", use default value");
                    value = zReflection.GetDefaultValue(parameter.ParameterType);
                }
                parameters.Add(value);
            }
            return parameters.ToArray();
        }
    }
}
